using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FTextBuilder @ 0x1699ee0..0x169e660, Ghidra vaddr).
public static class FTextBuilder
{
	[CompilerGenerated]
	private static Func<string> m_NicknameEvent;

	private static bool LoggedUnavailableCharError;

	private static event Func<string> NicknameEvent;

	public static void AddNicknameEvent(Func<string> func)
	{
		NicknameEvent += func;
	}

	// Shipped body is a bare return (4 bytes): the handler is never removed.
	public static void RemoveNicknameEvent(Func<string> func)
	{
	}

	public static List<FLetterQuadLine> GetQuadInfoForText(List<FTextSpan> spans, FTextParams textParams, bool allowShrink, float wrapScale, bool convertToEllipses, FFont baseFont, out bool fontFailed, bool maxFallBackReached, int dynamicFontSize = 24, FontStyle dynamicFontStyle = FontStyle.Normal)
	{
		FLineBreakStatus[] breaks = null;
		if (FLoc.IsRightToLeft && textParams.ClipWidth > 0f)
		{
			breaks = GetTextWrappingBreaks(spans, textParams, wrapScale, convertToEllipses, baseFont, maxFallBackReached, dynamicFontSize, dynamicFontStyle);
		}
		return GetQuadInfoForText_main(spans, textParams, allowShrink, wrapScale, convertToEllipses, baseFont, out fontFailed, maxFallBackReached, dynamicFontSize, dynamicFontStyle, breaks);
	}

	// TODO(port): right-to-left only (0x169a0e0, 4864 bytes). It repeats the layout loop of GetQuadInfoForText_main just to compute [left: RTL languages only (main pass computes its own breaks) / FTextParams.Spline is never assigned anywhere]
	// line-break positions; returning null makes the main pass compute its own breaks (identical for left-to-right languages).
	private static FLineBreakStatus[] GetTextWrappingBreaks(List<FTextSpan> spans, FTextParams textParams, float wrapScale, bool convertToEllipses, FFont baseFont, bool maxFallBackReached, int dynamicFontSize = 24, FontStyle dynamicFontStyle = FontStyle.Normal)
	{
		return null;
	}

	private static void ReverseFix(ref List<FLetterQuadLine> reverselist, ref List<FLetterQuadLine> lineList, FLetterQuadLine line)
	{
		reverselist.Add(line);
		List<float> ys = new List<float>();
		foreach (FLetterQuadLine l in reverselist)
		{
			ys.Add(l.Bounds.y);
		}
		for (int i = 0; i < reverselist.Count; i++)
		{
			float target = ys[ys.Count - 1 - i];
			reverselist[i].ApplyOffset(0f, target - reverselist[i].Bounds.y);
		}
		reverselist.Reverse();
		foreach (FLetterQuadLine l in reverselist)
		{
			lineList.Add(l);
		}
		reverselist.Clear();
	}

	private static float RoundHalfEven(float v)
	{
		return (float)Math.Round((double)v);
	}

	public static List<FLetterQuadLine> GetQuadInfoForText_main(List<FTextSpan> spans, FTextParams textParams, bool allowShrink, float wrapScale, bool convertToEllipses, FFont baseFont, out bool fontFailed, bool maxFallBackReached, int dynamicFontSize = 24, FontStyle dynamicFontStyle = FontStyle.Normal, FLineBreakStatus[] inBreaks = null)
	{
		fontFailed = false;
		float displayScale = Futile.DisplayScale;
		if (baseFont.mIsDynamic)
		{
			dynamicFontSize = (int)RoundHalfEven(baseFont.mDynamicPointScaleMult * (float)dynamicFontSize);
		}
		float superSample = Mathf.Min(displayScale * 2f, 1f);
		float ptSize = dynamicFontSize;
		if (FConfigCore.DynamicSuperSample)
		{
			superSample = 2f;
		}
		int requestSize = (int)(superSample * ptSize);
		List<FLetterQuadLine> lines = new List<FLetterQuadLine>();
		if (spans == null || spans.Count < 1)
		{
			return lines;
		}
		FTextSpanListHelper helper = new FTextSpanListHelper(spans);
		int total = helper.TotalLength;
		if (total < 1)
		{
			return lines;
		}
		string wrapString = helper.GetWrapString();
		FLineBreakStatus[] breaks = inBreaks;
		if (breaks == null)
		{
			breaks = FLineBreak.GetBreaks(wrapString, FLoc.UILanguageShort);
		}
		float lineHeight;
		if (!baseFont.mIsDynamic)
		{
			lineHeight = baseFont.LineHeight + textParams.scaledLineHeightOffset + baseFont.ScaledLineHeightOffset;
		}
		else
		{
			int unityFontSize = baseFont.mUnityFont.fontSize;
			float h = textParams.scaledLineHeightOffset + baseFont.LineHeight * (ptSize / (float)unityFontSize) + textParams.scaledLineHeightOffset;
			lineHeight = RoundHalfEven(h);
			if (baseFont.mSpritFontRefs != null && baseFont.mSpritFontRefs.ContainsKey(dynamicFontSize))
			{
				FFont spriteFont = baseFont.mSpritFontRefs[dynamicFontSize];
				lineHeight = textParams.scaledLineHeightOffset + spriteFont.LineHeight + textParams.scaledLineHeightOffset;
			}
			for (int s = 0; s < spans.Count; s++)
			{
				FTextSpanString spanString = spans[s] as FTextSpanString;
				if (spanString != null && !baseFont.WrappedRequestCharactersInTexture(spanString.Text, requestSize, dynamicFontStyle))
				{
					return null;
				}
			}
		}
		FLetterQuadLine line = FTextBuilderPools.FLetterQuadLinePool.Get();
		float clipHeight = textParams.ClipHeight;
		float clipWidth = textParams.ClipWidth;
		float bottomLimit = -clipHeight - baseFont.VerticalCenteringOffset;
		List<FLetterQuadLine> reverseList = new List<FLetterQuadLine>();
		const float cMinInit = 100000f;
		const float cMaxInit = -100000f;
		int lastBreak = -1;
		float penX = 0f;
		float baseY = 0f;
		float lineTop = 0f;
		float lineBottom = 0f - lineHeight;
		uint prevChar = 0u;
		float minX = cMinInit;
		float maxX = cMaxInit;
		bool stop = false;
		int i = 0;
		do
		{
			helper.SetIndex(i);
			FTextSpan span = helper.Span;
			Color spanColor = span.Color;
			FTextSpanString spanString = span as FTextSpanString;
			uint ch;
			FFont spanFont;
			if (spanString == null)
			{
				ch = 0xFFFF;
				spanFont = baseFont;
			}
			else
			{
				ch = spanString.Text[helper.IndexWithinSpan];
				spanFont = spanString.Font;
			}
			float offsetX = 0f;
			float offsetY = 0f;
			if (baseFont.mIsDynamic)
			{
				Vector3 typesetting;
				if (baseFont.TryGetTypesettingOffsetsBySize(ptSize, out typesetting))
				{
					offsetX = typesetting.x;
					offsetY = typesetting.y;
				}
			}
			if (i == 0 && spanFont.mIsDynamic)
			{
				penX += offsetX;
			}
			FLineBreakStatus status = breaks[i];
			if ((byte)status < 2 && i != total - 1)
			{
				// end of line
				line.Bounds = new Rect(minX, lineBottom, maxX - minX, lineTop - lineBottom);
				penX = 0f;
				if (spanFont.mIsDynamic)
				{
					penX = offsetX;
				}
				if (status == FLineBreakStatus.MustBreak)
				{
					if (reverseList.Count < 1)
					{
						lines.Add(line);
					}
					else
					{
						ReverseFix(ref reverseList, ref lines, line);
					}
				}
				else
				{
					reverseList.Add(line);
				}
				lineBottom = baseY - lineHeight - lineHeight;
				if (stop)
				{
					lineTop = baseY - lineHeight;
					line = null;
					minX = cMinInit;
					maxX = cMaxInit;
					break;
				}
				line = new FLetterQuadLine();
				lineTop = baseY - lineHeight;
				stop = false;
				minX = cMinInit;
				lastBreak = -1;
				maxX = cMaxInit;
				baseY = lineTop;
				prevChar = ch;
				continue;
			}
			FLetterQuad quad = FTextBuilderPools.FLetterQuadPool.Get();
			FCharInfo info;
			float kern;
			if (spanString == null)
			{
				FTextSpanImage image = span as FTextSpanImage;
				if (image == null)
				{
					throw new NullReferenceException();
				}
				FAtlasElement element = image.AtlasElement;
				quad.uvTopLeft = element.uvTopLeft;
				quad.uvTopRight = element.uvTopRight;
				quad.uvBottomRight = element.uvBottomRight;
				quad.uvBottomLeft = element.uvBottomLeft;
				quad.shader = FShader.Normal;
				quad.IsImage = true;
				info = new FCharInfo();
				info.charID = '￿';
				float imageHeight = spanFont.LineHeight;
				if (spanFont.mIsDynamic)
				{
					imageHeight = imageHeight * ptSize / (float)spanFont.mUnityFont.fontSize;
				}
				info.height = imageHeight;
				float imageWidth = imageHeight * element.SourceSize.x / element.SourceSize.y;
				info.width = imageWidth;
				info.xadvance = imageWidth;
			}
			else
			{
				char c = (char)ch;
				if (c == '₩')
				{
					c = '￦';
				}
				if (baseFont.mIsDynamic)
				{
					CharacterInfo charInfo;
					bool ok = baseFont.WrappedGetCharacterInfo(c, out charInfo, requestSize, dynamicFontStyle);
					if (!ok && !maxFallBackReached && ShouldFontFallback(c, baseFont))
					{
						fontFailed = true;
						return null;
					}
					if (!ok)
					{
						if (baseFont.HasCharacter(c))
						{
							return null;
						}
						if (char.IsWhiteSpace(c))
						{
							continue;
						}
						baseFont.WrappedGetCharacterInfo('?', out charInfo, requestSize, dynamicFontStyle);
					}
					// The shipped code optionally reuses quad.charInfo (FConfigCore.PoolFLetterQuad); a fresh object is equivalent.
					info = new FCharInfo();
					info.Init(charInfo, baseFont, dynamicFontSize, requestSize);
				}
				else
				{
					info = spanFont.CharInfosByID.Get(c);
					if (info == null)
					{
						if (char.IsWhiteSpace(c))
						{
							continue;
						}
						if (ShouldFontFallback(c, baseFont) && !maxFallBackReached)
						{
							fontFailed = true;
							return null;
						}
						info = spanFont.CharInfosByID.Get('?');
						if (info == null)
						{
							continue;
						}
					}
				}
				quad.uvTopLeft = info.uvTopLeft;
				quad.uvTopRight = info.uvTopRight;
				quad.uvBottomRight = info.uvBottomRight;
				quad.uvBottomLeft = info.uvBottomLeft;
				quad.shader = spanFont.FontShader;
				if (!baseFont.mIsDynamic)
				{
					float pairKern = 0f;
					if (spanFont.KerningInfos != null)
					{
						spanFont.KerningInfos.TryGetValue((int)(c | (prevChar << 16)), out pairKern);
					}
					float k1 = pairKern + textParams.scaledKerningOffset;
					float k2 = spanFont.ScaledKerningOffset;
					kern = k1 + k2;
					penX += kern;
				}
				else
				{
					float k2;
					if (baseFont.mDyanimcKerningPairs == null)
					{
						k2 = 0f;
					}
					else
					{
						int pair = baseFont.mDyanimcKerningPairs.GetPair((int)(c | (prevChar << 16)));
						if (textParams.Spline != null)
						{
							int adjusted = (c == 'd' && (prevChar & 0xFFFF) == 'a') ? (-35) : pair;
							pair = ((prevChar & 0xFFFF) == 'r') ? 30 : adjusted;
						}
						k2 = ptSize / 48f * (float)pair * 0.046875f;
					}
					float k1 = textParams.scaledKerningOffset;
					k2 += k1;
					penX += k1 + k2;
				}
				ch = c;
			}
			quad.charInfo = info;
			float qWidth = info.width;
			float qHeight = info.height;
			float qY = offsetY + (baseY - info.offsetY - qHeight);
			float qOffsetX = info.offsetX;
			if (clipHeight > 0f && !allowShrink)
			{
				float top = qHeight + qY;
				if (top < bottomLimit)
				{
					stop = true;
					continue;
				}
				if (qY < bottomLimit)
				{
					stop = true;
					qHeight = top - bottomLimit;
					float r = (bottomLimit - qY) / (top - qY);
					quad.uvBottomLeft = new Vector2(quad.uvBottomLeft.x + (info.uvTopLeft.x - info.uvBottomLeft.x) * r, quad.uvBottomLeft.y + (info.uvTopLeft.y - info.uvBottomLeft.y) * r);
					quad.uvBottomRight = new Vector2(quad.uvBottomRight.x + (info.uvTopRight.x - info.uvBottomRight.x) * r, quad.uvBottomRight.y + (info.uvTopRight.y - info.uvBottomRight.y) * r);
					qY = bottomLimit;
					lineBottom = bottomLimit;
				}
			}
			float qX = penX + qOffsetX;
			bool place = true;
			if (clipWidth > 0f)
			{
				bool fits = inBreaks != null || (qX + qWidth) * wrapScale <= textParams.ClipWidth * 0.95f;
				if (textParams.Wrap)
				{
					if (!fits)
					{
						place = false;
						bool wrapLine = line.HasAnyCharacters;
						if (wrapLine)
						{
							int back;
							if (lastBreak < 0)
							{
								back = 1;
							}
							else
							{
								back = i - lastBreak;
								int removeCount = back - 1;
								if (removeCount < 1)
								{
									back = 1;
								}
								else
								{
									helper.SetIndex(lastBreak);
									int j = lastBreak + 1;
									uint toRemove;
									do
									{
										FTextSpan s2 = helper.Span;
										toRemove = (uint)removeCount;
										if (s2 != null && !s2.IsWhiteSpaceAtIndex(helper.IndexWithinSpan))
										{
											break;
										}
										removeCount++;
										helper.SetIndex(j - 2);
										j--;
										toRemove = (uint)i;
									}
									while (0 < j);
									maxX = line.RemoveLast((int)toRemove);
								}
							}
							line.Bounds = new Rect(minX, lineBottom, maxX - minX, lineTop - lineBottom);
							penX = 0f;
							if (spanFont.mIsDynamic)
							{
								penX = offsetX;
							}
							lines.Add(line);
							lineTop = baseY - lineHeight;
							lineBottom = lineTop - lineHeight;
							if (stop)
							{
								line = null;
								minX = cMinInit;
								maxX = cMaxInit;
								break;
							}
							line = FTextBuilderPools.FLetterQuadLinePool.Get();
							i -= back;
							helper.SetIndex(i);
							FTextSpan rewound = helper.Span;
							if (textParams.WrapAddSpaces < 1)
							{
								lineTop = baseY - lineHeight;
								stop = false;
								minX = cMinInit;
								lastBreak = -1;
								maxX = cMaxInit;
								baseY = lineTop;
								prevChar = 0u;
								continue;
							}
							Color rewoundColor = rewound.Color;
							FCharInfo space = spanFont.CharInfosByID.Get(' ');
							FLetterQuad spaceQuad = FTextBuilderPools.FLetterQuadPool.Get();
							spaceQuad.charInfo = space;
							spaceQuad.uvTopLeft = space.uvTopLeft;
							spaceQuad.uvTopRight = space.uvTopRight;
							spaceQuad.uvBottomRight = space.uvBottomRight;
							spaceQuad.uvBottomLeft = space.uvBottomLeft;
							spaceQuad.shader = spanFont.FontShader;
							float spaceX = penX + space.offsetX;
							float spaceY = lineTop - space.offsetY - space.height;
							float spaceW = space.width * (float)textParams.WrapAddSpaces;
							spaceQuad.rect = new Rect(spaceX, spaceY, spaceW, space.height);
							line.AddLetterQuad(((FTextSpan)rewound).Atlas, rewoundColor, spaceQuad.shader, spaceQuad);
							minX = Math.Min(cMinInit, spaceQuad.rect.x);
							maxX = Math.Max(cMaxInit, spaceQuad.rect.x + spaceQuad.rect.width);
							stop = false;
							lastBreak = -1;
							penX += space.xadvance * spanFont.mDynamicSpacingMult * (float)textParams.WrapAddSpaces;
							baseY = lineTop;
							prevChar = 0u;
							continue;
						}
						// nothing on this line yet: fall through to the clip / ellipsis handling
					}
				}
				else if (qX <= clipWidth && !fits)
				{
					place = false;
				}
				if (!place && !(textParams.Wrap && line.HasAnyCharacters))
				{
					// LAB_0169cd2c: character does not fit and cannot wrap
					if (convertToEllipses)
					{
						FCharInfo dot;
						if (!baseFont.mIsDynamic)
						{
							dot = spanFont.CharInfosByID.Get('.');
							if (dot == null)
							{
								dot = spanFont.CharInfosByID.Get('?');
								if (dot == null)
								{
									throw new NullReferenceException();
								}
							}
						}
						else
						{
							baseFont.WrappedRequestCharactersInTexture(".", requestSize, dynamicFontStyle);
							CharacterInfo dotInfo;
							if (!baseFont.WrappedGetCharacterInfo('.', out dotInfo, requestSize, dynamicFontStyle))
							{
								return null;
							}
							dot = new FCharInfo();
							dot.Init(dotInfo, baseFont, dynamicFontSize, requestSize);
						}
						float dotAdvance = dot.xadvance * spanFont.mDynamicSpacingMult;
						if (i >= 5 && line.LetterQuadGroups != null)
						{
							int g = line.LetterQuadGroups.Count - 1;
							if (g >= 0)
							{
								int removed = 0;
								for (;;)
								{
									List<FLetterQuad> groupQuads = line.LetterQuadGroups[g].Quads;
									int idx = groupQuads.Count;
									bool found = false;
									if (0 < idx - 1)
									{
										do
										{
											idx--;
											FLetterQuad q = groupQuads[idx];
											penX = q.rect.width + q.rect.x;
											if (dotAdvance * 3f + penX <= textParams.ClipWidth)
											{
												found = true;
												break;
											}
											removed++;
										}
										while (1 < idx);
									}
									if (found)
									{
										break;
									}
									bool wasFirst = g < 1;
									g--;
									if (wasFirst)
									{
										break;
									}
								}
								if (0 < removed)
								{
									line.RemoveLast(removed);
								}
							}
						}
						for (int d = 3; d != 0; d--)
						{
							FLetterQuad dotQuad = FTextBuilderPools.FLetterQuadPool.Get();
							dotQuad.charInfo = dot;
							dotQuad.uvTopLeft = dot.uvTopLeft;
							dotQuad.uvTopRight = dot.uvTopRight;
							dotQuad.uvBottomRight = dot.uvBottomRight;
							dotQuad.uvBottomLeft = dot.uvBottomLeft;
							dotQuad.shader = spanFont.FontShader;
							dotQuad.rect = new Rect(penX + dot.offsetX, baseY - dot.offsetY - dot.height, dot.width, dot.height);
							line.AddLetterQuad(span.Atlas, spanColor, dotQuad.shader, dotQuad);
							minX = Math.Min(minX, dotQuad.rect.x);
							maxX = Math.Max(maxX, dotQuad.rect.width + dotQuad.rect.x);
							penX += dotAdvance;
						}
						break;
					}
					// partial glyph clipped at the clip width, then stop
					float partialW = textParams.ClipWidth - qX;
					float ratio = partialW / qWidth;
					quad.uvTopRight = new Vector2(info.uvTopLeft.x + (info.uvTopRight.x - info.uvTopLeft.x) * ratio, info.uvTopLeft.y + (info.uvTopRight.y - info.uvTopLeft.y) * ratio);
					quad.uvBottomRight = new Vector2(info.uvBottomLeft.x + (info.uvBottomRight.x - info.uvBottomLeft.x) * ratio, info.uvBottomLeft.y + (info.uvBottomRight.y - info.uvBottomLeft.y) * ratio);
					quad.rect = new Rect(qX, qY, partialW, qHeight);
					line.AddLetterQuad(span.Atlas, spanColor, quad.shader, quad);
					line.Bounds = new Rect(minX, lineBottom, maxX - minX, lineTop - lineBottom);
					lines.Add(line);
					break;
				}
				if (!place)
				{
					continue;
				}
			}
			// LAB_0169c618: place the glyph
			int newLastBreak = (status == FLineBreakStatus.AllowBreak) ? i : lastBreak;
			if (qWidth == 0f)
			{
				qWidth = info.xadvance;
			}
			quad.rect = new Rect(qX, qY, qWidth, qHeight);
			line.AddLetterQuad(span.Atlas, spanColor, quad.shader, quad);
			minX = Math.Min(minX, qX);
			maxX = Math.Max(maxX, qX + qWidth);
			penX += info.xadvance * spanFont.mDynamicSpacingMult;
			prevChar = ch;
			lastBreak = newLastBreak;
		}
		while (++i < total);
		// LAB_0169ca50
		if (reverseList.Count < 1)
		{
			if (line != null && line.HasAnyCharacters)
			{
				line.Bounds = new Rect(minX, lineBottom, maxX - minX, lineTop - lineBottom);
				lines.Add(line);
			}
		}
		else
		{
			line.Bounds = new Rect(minX, lineBottom, maxX - minX, lineTop - lineBottom);
			ReverseFix(ref reverseList, ref lines, line);
		}
		if (baseFont.mIsDynamic)
		{
			for (int l = 0; l < lines.Count; l++)
			{
				FLetterQuadLine dynLine = lines[l];
				Rect b = dynLine.Bounds;
				Vector3 typesetting;
				float extra = baseFont.TryGetTypesettingOffsetsBySize(ptSize, out typesetting) ? typesetting.z : (-0f);
				dynLine.Bounds = new Rect(b.x, b.y, b.width + extra, b.height);
			}
		}
		if (textParams.Spline == null)
		{
			return lines;
		}
		// TODO(port): spline evaluation results were lost by the decompiler; assumed rect.y += Evaluate(t).y and rotation = atan2(tangent.y, tangent.x) [left: RTL languages only (main pass computes its own breaks) / FTextParams.Spline is never assigned anywhere]
		float lastKey = textParams.Spline.GetLastKey().x;
		float firstKey = textParams.Spline.GetFirstKey().x;
		for (int l = 0; l < lines.Count; l++)
		{
			FLetterQuadLine splineLine = lines[l];
			float lineX = splineLine.Bounds.x;
			float lineW = splineLine.Bounds.width;
			for (int g = 0; g < splineLine.LetterQuadGroups.Count; g++)
			{
				List<FLetterQuad> quads = splineLine.LetterQuadGroups[g].Quads;
				for (int k = 0; k < quads.Count; k++)
				{
					FLetterQuad sq = quads[k];
					float t = (sq.rect.x + sq.rect.width * 0.5f + ((lastKey - firstKey) - ((lineX + lineW) - lineX)) * 0.5f) / (lastKey - firstKey);
					Vector2 pos = textParams.Spline.Evaluate(t);
					Rect r2 = sq.rect;
					r2.y += pos.y;
					sq.rect = r2;
					Vector2 tangent = textParams.Spline.EvaluateTangent(t);
					sq.rotation = Mathf.Atan2(tangent.y, tangent.x);
				}
			}
		}
		return lines;
	}

	private static bool ShouldFontFallback(char letter, FFont baseFont)
	{
		if (!FConfigCore.DynamicFontsFallback)
		{
			return false;
		}
		return !baseFont.HasCharacter(letter);
	}

	public static List<FTextSpan> GetSpansFromMarkup(string markedUpText, FFont defaultFont, Color defaultColor, out XmlDocument doc)
	{
		string xml = "<markup xml:space='preserve'>" + markedUpText + "</markup>";
		doc = new XmlDocument();
		doc.PreserveWhitespace = true;
		doc.LoadXml(xml);
		return GetSpansFromMarkup(doc, defaultFont, defaultColor);
	}

	public static List<FTextSpan> GetSpansFromMarkup(XmlDocument doc, FFont defaultFont, Color defaultColor)
	{
		List<FTextSpan> spans = new List<FTextSpan>();
		if (doc != null && doc.DocumentElement != null)
		{
			Stack<Color> colorStack = new Stack<Color>();
			colorStack.Push(defaultColor);
			doc.DocumentElement.DoDepthFirst(delegate(XmlNode node)
			{
				if (node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.SignificantWhitespace)
				{
					FTextSpanString textSpan = new FTextSpanString();
					textSpan.Text = node.Value;
					textSpan.Color = colorStack.Peek();
					textSpan.Font = defaultFont;
					spans.Add(textSpan);
				}
				else if (node.NodeType == XmlNodeType.Element)
				{
					XmlElement element = (XmlElement)node;
					if (element.Name == "color")
					{
						string hex = element.GetAttrib("hex");
						Color color;
						if (hex != null)
						{
							color = FUtil.ColorFromHex(hex);
						}
						else
						{
							string colorName = element.GetAttrib("name");
							if (colorName == null)
							{
								return;
							}
							color = FColorNames.Get(colorName);
						}
						colorStack.Push(color);
					}
					else if (element.Name == "image")
					{
						string imageName = element.GetAttrib("name");
						if (imageName != null)
						{
							FTextSpanImage imageSpan = new FTextSpanImage();
							imageSpan.SetElementByName(imageName);
							spans.Add(imageSpan);
						}
					}
					else
					{
						string text;
						if (element.Name == "lookup")
						{
							string lookupName = element.GetAttrib("name");
							if (string.IsNullOrEmpty(lookupName) || lookupName != "nickname" || NicknameEvent == null)
							{
								return;
							}
							text = FCoreUtil.UnEscapeXML(NicknameEvent());
							if (string.IsNullOrEmpty(text))
							{
								text = FLoc.GetString("PlayerName.Default");
							}
						}
						else
						{
							text = FLoc.ParseSpecialTag(element);
							if (!text.Valid())
							{
								return;
							}
						}
						FTextSpanString textSpan2 = new FTextSpanString();
						textSpan2.Text = text;
						textSpan2.Color = colorStack.Peek();
						textSpan2.Font = defaultFont;
						spans.Add(textSpan2);
					}
				}
			}, delegate(XmlNode node)
			{
				if (node.NodeType == XmlNodeType.Element && node.Name == "color")
				{
					colorStack.Pop();
				}
			});
		}
		return spans;
	}
}
