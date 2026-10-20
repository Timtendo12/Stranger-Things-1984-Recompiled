using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAtlas @ 0x1682f1c..0x1686540, Ghidra vaddr).
public class FAtlas
{
	public enum eUnloadStrategy
	{
		NeverUnload = 0,
		UnloadAnytime = 1,
		NeverUnloadInPregame = 2,
		NeverUnloadInGame = 3
	}

	protected string mName;

	protected string mImagePath;

	protected string mDataPath;

	protected string mInfoPath;

	protected int mIndex;

	protected List<FAtlasElement> mElements = new List<FAtlasElement>();

	protected Dictionary<string, FAtlasElement> mElementsByName = new Dictionary<string, FAtlasElement>();

	protected Texture mTexture;

	protected Vector2 mTextureSize;

	protected bool mSingleImage;

	protected bool mManageTextureAsset;

	protected int mUseCount;

	public eUnloadStrategy UnloadStrategy = eUnloadStrategy.UnloadAnytime;

	protected FilterMode mFilterMode = FilterMode.Bilinear;

	public int mGeneration;

	public int mLastFrameUsed;

	public float mLastUseTime;

	public float mKeepAliveTime;

	public int mUseCount3d;

	public List<FAtlasElement> Elements => mElements;

	public int Index => mIndex;

	public Texture Texture => mTexture;

	public Vector2 TextureSize => mTextureSize;

	public string Name => mName;

	public string ImagePath => mImagePath;

	public string DataPath => mDataPath;

	public bool SingleImage => mSingleImage;

	public int UseCount => mUseCount;

	public bool IsLoaded => mTexture != null;

	public bool Unloadable
	{
		get
		{
			switch (UnloadStrategy)
			{
			case eUnloadStrategy.UnloadAnytime:
				return true;
			case eUnloadStrategy.NeverUnloadInGame:
				return !Futile.IsInAGame;
			case eUnloadStrategy.NeverUnloadInPregame:
				return Futile.IsInAGame;
			default:
				return false;
			}
		}
	}

	public FAtlas(string name, Texture texture, int index)
	{
		mName = name;
		mImagePath = "";
		mDataPath = "";
		mInfoPath = "";
		mIndex = index;
		SetTexture(texture);
		UnloadStrategy = eUnloadStrategy.UnloadAnytime;
		CreateAtlasFromSingleImage();
	}

	public void SetTexture(Texture texture)
	{
		mTexture = texture;
		if (mTexture != null)
		{
			mTextureSize = new Vector2(mTexture.width, mTexture.height);
		}
		else
		{
			mTextureSize = Vector2.zero;
		}
	}

	public Texture GetOrLoadTexture(out bool requiredReload)
	{
		requiredReload = false;
		if (mTexture == null)
		{
			requiredReload = true;
			LoadTexture();
		}
		return mTexture;
	}

	public FAtlas(string name, string imagePath, string dataPath, string infoPath, int index, bool shouldLoadAsSingleImage, eUnloadStrategy unloadStrategy, FilterMode filterMode)
	{
		mName = name;
		mImagePath = imagePath;
		mDataPath = dataPath;
		mInfoPath = infoPath;
		mIndex = index;
		UnloadStrategy = unloadStrategy;
		mFilterMode = filterMode;
		if (shouldLoadAsSingleImage)
		{
			mSingleImage = true;
			LoadTexture();
			CreateAtlasFromSingleImage();
			return;
		}
		mSingleImage = false;
		LoadAtlasData();
		LoadTexture();
	}

	protected void LoadTexture()
	{
		if (mTexture != null)
		{
			FAssert.Fail("Tried to load an already loaded texture! " + mName);
			return;
		}
		if (FConfigCore.CompressedAtlases && FAtlasManager.CompressedAtlasesEnabled && (mImagePath.StartsWith("Atlases") || mImagePath.StartsWith("Phase02/Atlases")))
		{
			string path = mImagePath.Replace("Atlases", "AtlasesCompressed") + "Combined" + "_comp";
			mTexture = FAssetBundleManager.Get<Texture>(path);
			FLog.Log("loaded compressed at " + path + ((mTexture == null) ? " -failed" : " -success"), true);
			mManageTextureAsset = true;
		}
		if (mTexture == null)
		{
			mTexture = FUtil.LoadAssetAsTexture(mImagePath + "Combined", true);
			if (FConfigCore.AssetBundleUnloading && FConfigCore.AbAtlas)
			{
				bool compressed = FConfigCore.CompressedAtlases;
				// The shipped code performs this check twice in a row; the second call is a no-op once the bundle is unloaded.
				if (mTexture != null && UnloadStrategy == eUnloadStrategy.NeverUnload)
				{
					FAssetBundleManager.UnloadBundleByPath(mImagePath + "Combined", !compressed);
				}
			}
			if (mTexture == null)
			{
				mTexture = FUtil.LoadSplitTexture(mImagePath + "Color", mImagePath + "Alpha", true);
			}
		}
		if (mTexture == null)
		{
			mTexture = FAssetBundleManager.Get<Texture>(mImagePath);
			if (!mTexture)
			{
				FLog.UnityError("Futile: Couldn't load the atlas texture from: " + mImagePath);
				return;
			}
			mManageTextureAsset = true;
		}
		mTextureSize = new Vector2(mTexture.width, mTexture.height);
		mTexture.filterMode = mFilterMode;
		mTexture.name = mName;
		Resources.UnloadUnusedAssets();
	}

	protected void LoadAtlasData()
	{
		TextAsset dataAsset = FAssetBundleManager.Get<TextAsset>(mDataPath);
		if (dataAsset == null)
		{
			FLog.UnityError("Futile: Couldn't load the atlas data from: " + mDataPath);
			return;
		}
		Dictionary<string, object> json = dataAsset.text.dictionaryFromJson();
		Dictionary<string, object> meta = (Dictionary<string, object>)json["meta"];
		Dictionary<string, object> size = (Dictionary<string, object>)meta["size"];
		if (size == null)
		{
			FLog.UnityError("Could not find texture size in meta section of atlas '" + mName + "'");
		}
		else
		{
			mTextureSize.x = FParse.ParseFloat(size["w"].ToString());
			mTextureSize.y = FParse.ParseFloat(size["h"].ToString());
		}
		Dictionary<string, object> frames = (Dictionary<string, object>)json["frames"];
		float scaleInverse = Futile.ResourceScaleInverse;
		float uvOffsetX;
		float uvOffsetY;
		if (Futile.NeedHalfTexelOffset)
		{
			uvOffsetX = 0.5f;
			uvOffsetY = -0.5f;
		}
		else
		{
			uvOffsetX = 0f;
			uvOffsetY = 0f;
		}
		uvOffsetX /= mTextureSize.x;
		uvOffsetY /= mTextureSize.y;
		int index = 0;
		foreach (KeyValuePair<string, object> item in frames)
		{
			FAtlasElement element = new FAtlasElement();
			element.Name = item.Key;
			element.IndexInAtlas = index;
			IDictionary frameData = (IDictionary)item.Value;
			element.IsTrimmed = (bool)frameData["trimmed"];
			element.IsRotated = (bool)frameData["rotated"];
			IDictionary frame = (IDictionary)frameData["frame"];
			float frameX = FParse.ParseFloat(frame["x"].ToString());
			float frameY = FParse.ParseFloat(frame["y"].ToString());
			float frameW = FParse.ParseFloat(frame["w"].ToString());
			float frameH = FParse.ParseFloat(frame["h"].ToString());
			float texW = mTextureSize.x;
			float texH = mTextureSize.y;
			if (!element.IsRotated)
			{
				float left = uvOffsetX + frameX / texW;
				float bottom = uvOffsetY + (texH - frameY - frameH) / texH;
				float right = frameW / texW + left;
				float top = frameH / texH + bottom;
				element.uvRect = new Rect(left, bottom, frameW / texW, frameH / texH);
				element.uvTopLeft = new Vector2(left, top);
				element.uvTopRight = new Vector2(right, top);
				element.uvBottomRight = new Vector2(right, bottom);
				element.uvBottomLeft = new Vector2(left, bottom);
			}
			else
			{
				float left = uvOffsetY + frameX / texW;
				float right = frameH / texW + left;
				float bottom = (texH - frameY - frameW) / texH - uvOffsetX;
				float top = frameW / texH + bottom;
				element.uvRect = new Rect(left, bottom, frameH / texW, frameW / texH);
				element.uvTopLeft = new Vector2(right, top);
				element.uvTopRight = new Vector2(right, bottom);
				element.uvBottomRight = new Vector2(left, bottom);
				element.uvBottomLeft = new Vector2(left, top);
			}
			IDictionary sourceRect = (IDictionary)frameData["spriteSourceSize"];
			float rectX = FParse.ParseFloat(sourceRect["x"].ToString());
			float rectY = FParse.ParseFloat(sourceRect["y"].ToString());
			float rectW = FParse.ParseFloat(sourceRect["w"].ToString());
			float rectH = FParse.ParseFloat(sourceRect["h"].ToString());
			element.SourceRect = new Rect(scaleInverse * rectX, scaleInverse * rectY, scaleInverse * rectW, scaleInverse * rectH);
			IDictionary sourceSize = (IDictionary)frameData["sourceSize"];
			element.SourceSize.x = scaleInverse * FParse.ParseFloat(sourceSize["w"].ToString());
			element.SourceSize.y = scaleInverse * FParse.ParseFloat(sourceSize["h"].ToString());
			mElements.Add(element);
			index++;
			mElementsByName.Add(element.Name, element);
		}
		Resources.UnloadAsset(dataAsset);
		TextAsset infoAsset = FAssetBundleManager.Get<TextAsset>(mInfoPath);
		if (infoAsset != null)
		{
			Dictionary<string, object> info = infoAsset.text.dictionaryFromJson();
			if (info != null)
			{
				Dictionary<string, object> infoFrames = (Dictionary<string, object>)info["frames"];
				foreach (KeyValuePair<string, object> item in infoFrames)
				{
					FAtlasElement element = null;
					mElementsByName.TryGetValue(item.Key, out element);
					if (element == null)
					{
						FLog.UnityWarning("could not find '" + item.Key + "' in atlas '" + mName + "' when trying to apply info file settings");
					}
					else
					{
						IDictionary settings = (IDictionary)item.Value;
						if (settings != null)
						{
							IDictionary border = (IDictionary)settings["borderSize"];
							if (border != null)
							{
								// float.Parse in the original (device culture); invariant culture here so PCs with a decimal comma parse the same.
								element.BorderSize.x = float.Parse(border["left"].ToString(), CultureInfo.InvariantCulture);
								element.BorderSize.y = float.Parse(border["top"].ToString(), CultureInfo.InvariantCulture);
								element.BorderSize.z = float.Parse(border["right"].ToString(), CultureInfo.InvariantCulture);
								element.BorderSize.w = float.Parse(border["bottom"].ToString(), CultureInfo.InvariantCulture);
							}
							object showInterior = settings["showSlicedInterior"];
							if (showInterior != null)
							{
								bool value = true;
								if (FParse.TryParseBool(showInterior.ToString(), out value))
								{
									element.ShowSlicedInterior = value;
								}
							}
						}
					}
				}
			}
		}
		if (infoAsset != null)
		{
			Resources.UnloadAsset(infoAsset);
		}
	}

	public virtual void Flush()
	{
	}

	protected void CreateAtlasFromSingleImage()
	{
		FAtlasElement element = new FAtlasElement();
		element.Name = mName;
		element.IndexInAtlas = 0;
		float uvOffsetX = Futile.NeedHalfTexelOffset ? (0.5f / mTextureSize.x + 0f) : 0f;
		float uvOffsetY = Futile.NeedHalfTexelOffset ? (-0.5f / mTextureSize.y + 0f) : 0f;
		float scaleInverse = Futile.ResourceScaleInverse;
		element.uvRect = new Rect(uvOffsetX, uvOffsetY, 1f, 1f);
		element.uvTopLeft = new Vector2(uvOffsetX, uvOffsetY + 1f);
		element.uvTopRight = new Vector2(uvOffsetX + 1f, uvOffsetY + 1f);
		element.uvBottomRight = new Vector2(uvOffsetX + 1f, uvOffsetY);
		element.uvBottomLeft = new Vector2(uvOffsetX, uvOffsetY);
		element.SourceRect = new Rect(0f, 0f, mTextureSize.x * scaleInverse, mTextureSize.y * scaleInverse);
		element.SourceSize = new Vector2(mTextureSize.x * scaleInverse, mTextureSize.y * scaleInverse);
		element.IsTrimmed = false;
		element.IsRotated = false;
		mElements.Add(element);
		mElementsByName.Add(element.Name, element);
	}

	public virtual bool UnloadTexture()
	{
		if (Unloadable && mTexture != null)
		{
			Texture texture = mTexture;
			mGeneration++;
			if (!mManageTextureAsset)
			{
				Object.Destroy(texture);
			}
			else
			{
				Resources.UnloadAsset(texture);
			}
			mTexture = null;
			Resources.UnloadUnusedAssets();
			if (FConfigCore.GCFix1)
			{
				Futile.QueueGCCollect(true, 10f);
				return true;
			}
			System.GC.Collect();
			System.GC.Collect();
			return true;
		}
		return false;
	}

	public virtual void OnDestroy()
	{
	}

	public virtual bool Redraw(FAtlasElement element)
	{
		return false;
	}

	public bool IncrementUseCount(bool is3D = false)
	{
		mUseCount++;
		if (is3D)
		{
			mUseCount3d++;
		}
		if (FAtlasManager.AtlasUnloadingEnabled && mTexture == null)
		{
			LoadTexture();
			return true;
		}
		return false;
	}

	public bool DecrementUseCount(bool is3D = false)
	{
		mUseCount--;
		if (is3D)
		{
			mUseCount3d = Mathf.Max(0, mUseCount3d - 1);
		}
		if (mUseCount < 0)
		{
			FLog.Error("FAtlas::DecUC: '" + mName + "' has UseCount of " + mUseCount + ", ATLASLOADERROR.");
			mUseCount = 0;
			if (!FAtlasManager.AtlasUnloadingEnabled)
			{
				return false;
			}
		}
		else if (!FAtlasManager.AtlasUnloadingEnabled || mUseCount != 0)
		{
			return false;
		}
		return Unloadable;
	}

	public void ClearUseCount3D()
	{
		mUseCount3d = 0;
	}

	public bool LoadUnload()
	{
		if (mUseCount > 0)
		{
			if (mTexture == null)
			{
				LoadTexture();
				return true;
			}
			if (mUseCount > 0)
			{
				return false;
			}
		}
		if (mTexture != null && Unloadable)
		{
			UnloadTexture();
			return true;
		}
		return false;
	}
}
