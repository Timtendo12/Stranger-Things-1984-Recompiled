using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FNodeStack @ 0x16bb854..0x16bcb70, Ghidra vaddr).
public class FNodeStack : FDimNode
{
	public enum Modes
	{
		FromBottom = 0,
		FromTop = 1,
		CenteredY = 2,
		FromLeft = 3,
		FromRight = 4,
		CenteredX = 5,
		FannedX = 6,
		FannedXInverted = 7
	}

	protected List<FStackEntry> mStackEntries = new List<FStackEntry>();

	protected Modes mMode = Modes.FromTop;

	protected Sides mAlignment = Sides.CenterX;

	private Action LayoutStackedNodesFunc;

	public float BetweenItemDefaultMargin { get; set; }

	public Vector2 FanDimensions { get; set; }

	public float FanEndRotation { get; set; }

	public float FanRadius { get; set; }

	public float FanExtraWidth { get; set; }

	public Modes Mode => mMode;

	public Sides Alignment => mAlignment;

	public List<FStackEntry> StackEntries => mStackEntries;

	public FNodeStack(string name, Modes mode, float betweenItemDefaultMargin)
		: base(name)
	{
		BetweenItemDefaultMargin = betweenItemDefaultMargin;
		mMode = mode;
		SetModeValues();
	}

	public void SetMode(Modes mode)
	{
		mMode = mode;
		SetModeValues();
	}

	protected void SetModeValues()
	{
		switch (mMode)
		{
		case Modes.FromBottom:
			mAlignment = Sides.CenterX;
			LayoutStackedNodesFunc = LayoutFromBottom;
			break;
		case Modes.FromTop:
			mAlignment = Sides.CenterX;
			LayoutStackedNodesFunc = LayoutFromTop;
			break;
		case Modes.CenteredY:
			mAlignment = Sides.CenterX;
			LayoutStackedNodesFunc = LayoutCenteredY;
			break;
		case Modes.FromLeft:
			mAlignment = Sides.CenterY;
			LayoutStackedNodesFunc = LayoutFromLeft;
			break;
		case Modes.FromRight:
			mAlignment = Sides.CenterY;
			LayoutStackedNodesFunc = LayoutFromRight;
			break;
		case Modes.CenteredX:
			mAlignment = Sides.CenterY;
			LayoutStackedNodesFunc = LayoutCenteredX;
			break;
		case Modes.FannedX:
			mAlignment = Sides.CenterY;
			LayoutStackedNodesFunc = LayoutFannedX;
			break;
		case Modes.FannedXInverted:
			mAlignment = Sides.CenterY;
			LayoutStackedNodesFunc = LayoutFannedXInverted;
			break;
		}
	}

	public void ClearStack(bool cleanup = false)
	{
		for (int i = mStackEntries.Count - 1; i >= 0; i--)
		{
			FNode node = mStackEntries[i].Node;
			if (node != null)
			{
				if (cleanup)
				{
					FNode.Cleanup(node);
				}
				else
				{
					node.Unparent();
				}
			}
		}
		mStackEntries.Clear();
		MarkDirtyRecursive();
	}

	public void StackChild(FNode node, Vector2 size, Vector2 offset = default(Vector2))
	{
		if (node == null)
		{
			return;
		}
		if (node.Parent != null)
		{
			if (node.Parent == this)
			{
				RemoveStackEntry(node);
			}
			else
			{
				node.Unparent();
			}
		}
		FStackEntry entry = new FStackEntry(node, mAlignment, size, offset);
		entry.Node = node;
		entry.Alignment = mAlignment;
		entry.Size = size;
		entry.Offset = offset;
		mStackEntries.Add(entry);
		node.SetParent(this);
		MarkDirtyRecursive();
	}

	protected void RemoveStackEntry(FNode node)
	{
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			FStackEntry entry = mStackEntries[i];
			if (entry.Node == node)
			{
				mStackEntries.Remove(entry);
				break;
			}
		}
	}

	public void UpdateChildOffset(FNode node, Vector2 offset)
	{
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			FStackEntry entry = mStackEntries[i];
			if (entry.Node == node)
			{
				entry.Offset = offset;
				MarkDirtyRecursive();
				break;
			}
		}
	}

	public void UpdateChildSize(FNode node, Vector2 size)
	{
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			FStackEntry entry = mStackEntries[i];
			if (entry.Node == node)
			{
				entry.Size = size;
				MarkDirtyRecursive();
				break;
			}
		}
	}

	public override void RemoveChild(FNode node)
	{
		if (node != null)
		{
			RemoveStackEntry(node);
			base.RemoveChild(node);
		}
	}

	protected void LayoutFromBottom()
	{
		FNode prev = this;
		for (int i = mStackEntries.Count - 1; i >= 0; i--)
		{
			FStackEntry entry = mStackEntries[i];
			FNode node = entry.Node;
			if (prev == this)
			{
				node.DockInside(this, InnerDock.Bottom, entry.Size.x, entry.Size.y, entry.Offset.x, entry.Offset.y);
			}
			else
			{
				node.DockOutside(prev, OuterDock.Top, entry.Size.x, entry.Size.y, entry.Offset.x, BetweenItemDefaultMargin + entry.Offset.y);
			}
			prev = node;
		}
	}

	protected void LayoutFromTop()
	{
		FNode prev = this;
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			FStackEntry entry = mStackEntries[i];
			FNode node = entry.Node;
			if (prev == this)
			{
				node.DockInside(this, InnerDock.Top, entry.Size.x, entry.Size.y, entry.Offset.x, entry.Offset.y);
			}
			else
			{
				node.DockOutside(prev, OuterDock.Bottom, entry.Size.x, entry.Size.y, entry.Offset.x, entry.Offset.y - BetweenItemDefaultMargin);
			}
			prev = node;
		}
	}

	protected void LayoutFromRight()
	{
		FNode prev = this;
		for (int i = mStackEntries.Count - 1; i >= 0; i--)
		{
			FStackEntry entry = mStackEntries[i];
			FNode node = entry.Node;
			if (prev == this)
			{
				node.DockInside(this, InnerDock.Right, entry.Size.x, entry.Size.y, entry.Offset.x, entry.Offset.y);
			}
			else
			{
				node.DockOutside(prev, OuterDock.Left, entry.Size.x, entry.Size.y, entry.Offset.x - BetweenItemDefaultMargin, entry.Offset.y);
			}
			prev = node;
		}
	}

	protected void LayoutFromLeft()
	{
		FNode prev = this;
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			FStackEntry entry = mStackEntries[i];
			FNode node = entry.Node;
			if (prev == this)
			{
				node.DockInside(this, InnerDock.Left, entry.Size.x, entry.Size.y, entry.Offset.x, entry.Offset.y);
			}
			else
			{
				node.DockOutside(prev, OuterDock.Right, entry.Size.x, entry.Size.y, BetweenItemDefaultMargin + entry.Offset.x, entry.Offset.y);
			}
			prev = node;
		}
	}

	protected void LayoutCenteredY()
	{
		if (mStackEntries.Count < 1)
		{
			return;
		}
		float total = BetweenItemDefaultMargin * 0.5f * (float)(mStackEntries.Count - 1);
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			total += (mStackEntries[i].Size.y + mStackEntries[i].Offset.y) * 0.5f;
		}
		FNode prev = this;
		VertSide side = VertSide.CenterY;
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			FStackEntry entry = mStackEntries[i];
			float top = (i == 0) ? (total + entry.Offset.y) : (0f - BetweenItemDefaultMargin - entry.Offset.y);
			float halfWidth = entry.Size.x * 0.5f;
			entry.Node.DockCustom(this, HorizSide.CenterX, entry.Offset.x - halfWidth, this, HorizSide.CenterX, entry.Offset.x + halfWidth, prev, side, top, prev, side, top - entry.Size.y);
			side = VertSide.Bottom;
			prev = entry.Node;
		}
	}

	protected void LayoutCenteredX()
	{
		if (mStackEntries.Count < 1)
		{
			return;
		}
		float total = BetweenItemDefaultMargin * -0.5f * (float)(mStackEntries.Count - 1);
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			total += (mStackEntries[i].Size.x + mStackEntries[i].Offset.x) * -0.5f;
		}
		FNode prev = this;
		HorizSide side = HorizSide.CenterX;
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			FStackEntry entry = mStackEntries[i];
			float left = ((i == 0) ? total : BetweenItemDefaultMargin) + entry.Offset.x;
			float halfHeight = entry.Size.y * 0.5f;
			entry.Node.DockCustom(prev, side, left, prev, side, left + entry.Size.x, this, VertSide.CenterY, entry.Offset.y + halfHeight, this, VertSide.CenterY, entry.Offset.y - halfHeight);
			side = HorizSide.Right;
			prev = entry.Node;
		}
	}

	protected void LayoutFannedX()
	{
		LayoutFannedXBase(false);
	}

	protected void LayoutFannedXInverted()
	{
		LayoutFannedXBase(true);
	}

	protected void LayoutFannedXBase(bool isInverted)
	{
		int count = mStackEntries.Count;
		if (count < 1)
		{
			return;
		}
		float totalWidth = BetweenItemDefaultMargin * (float)(count - 1);
		for (int i = 0; i < count; i++)
		{
			totalWidth += mStackEntries[i].Size.x + mStackEntries[i].Offset.x;
		}
		float fanWidth = FanDimensions.x;
		if (count < 5)
		{
			fanWidth = ((float)count / 5f) * fanWidth;
		}
		float spacingScale = fanWidth / totalWidth;
		if (totalWidth <= fanWidth)
		{
			spacingScale = 1f;
			fanWidth = totalWidth;
		}
		float rotationStep = 0f;
		if (count > 1)
		{
			rotationStep = (FanEndRotation + FanEndRotation) / (float)(count - 1);
		}
		float radius = FanRadius;
		float baseSin = Mathf.Sin((90f - FanEndRotation) * 0.017453292f);
		float extraWidth = FanExtraWidth;
		float rotation = 0f - FanEndRotation;
		float x = fanWidth * -0.5f;
		float sign = isInverted ? -1f : 1f;
		for (int i = 0; i < mStackEntries.Count; i++)
		{
			float currentRadius = FanRadius;
			float sin = Mathf.Sin((90f - rotation) * 0.017453292f);
			FStackEntry entry = mStackEntries[i];
			float pad = 0f;
			if (extraWidth > float.Epsilon && count > 1)
			{
				pad = (extraWidth + entry.Size.x) * 0.5f;
			}
			FNode node = entry.Node;
			float yOffset = sign * (sin * currentRadius - radius * baseSin) * 3f;
			float left = x + entry.Offset.x - pad;
			float y = yOffset + entry.Offset.y;
			float halfHeight = entry.Size.y * 0.5f;
			node.DockCustom(this, HorizSide.CenterX, left, this, HorizSide.CenterX, left + extraWidth + entry.Size.x, this, VertSide.CenterY, y + halfHeight, this, VertSide.CenterY, y - halfHeight);
			if (count > 1)
			{
				node.SetRotation(sign * rotation);
			}
			node.HackFinalRotation = rotation;
			node.HackFinalY = yOffset + entry.Offset.y;
			rotation = rotationStep + rotation;
			x += spacingScale * (extraWidth + entry.Size.x);
		}
	}

	public override void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			Rect extents = Extents;
			Position = new Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
			Width = extents.width;
			Height = extents.height;
		}
		if (LayoutStackedNodesFunc != null)
		{
			LayoutStackedNodesFunc();
		}
	}
}
