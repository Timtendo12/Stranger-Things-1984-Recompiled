using System;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FVisualDefinition @ 0x17e302c..0x17e3774, Ghidra vaddr).
public class FVisualDefinition
{
	protected string mSprite;

	protected string mImageName;

	protected string mGOTreeName;

	protected FAnimDef mAnimDef;

	public string GOTreeName => mGOTreeName;

	public FAnimDef AnimDef => mAnimDef;

	public static FVisualDefinition AllocateForXmlNode(XmlElement root)
	{
		return new FVisualDefinition();
	}

	public static void AddConstructorInfo(string name, Type type)
	{
	}

	public static FVisualDefinition CreateFromXmlNode(XmlElement root)
	{
		FVisualDefinition fVisualDefinition = AllocateForXmlNode(root);
		if (fVisualDefinition != null)
		{
			fVisualDefinition.LoadXml(root);
		}
		return fVisualDefinition;
	}

	public FVisualDefinition DeepCopy()
	{
		return new FVisualDefinition
		{
			mSprite = mSprite,
			mImageName = mImageName,
			mGOTreeName = mGOTreeName,
			mAnimDef = mAnimDef
		};
	}

	public bool LoadXml(XmlElement root)
	{
		mSprite = root.GetAttrib("Sprite", string.Empty);
		if (mSprite.Valid() && !mSprite.Contains(".png"))
		{
			mSprite += ".png";
		}
		mGOTreeName = root.GetAttrib("GOTreeName", string.Empty);
		mImageName = root.GetAttrib("Image", string.Empty);
		mAnimDef = FAnimDefManager.GetAnimDef(root.GetAttrib("AnimDef", string.Empty));
		return true;
	}

	public bool SetData(string sprite, string goTree, string animDef, string imageName = "")
	{
		mSprite = sprite;
		if (mSprite.Valid() && !mSprite.Contains(".png"))
		{
			mSprite += ".png";
		}
		mGOTreeName = goTree;
		mAnimDef = FAnimDefManager.GetAnimDef(animDef);
		mImageName = imageName;
		return true;
	}

	public FNode AllocateNode()
	{
		FNode fNode;
		if (mGOTreeName.Valid())
		{
			FGOTreeInstance fGOTreeInstance = FGOTreeManager.Build(mGOTreeName, null, FRandom.Generic);
			if (fGOTreeInstance == null)
			{
				return null;
			}
			FGameObjectNode fGameObjectNode = new FGameObjectNode(fGOTreeInstance.Base, clone: false, fGOTreeInstance.BasePrefab);
			fGameObjectNode.AnimDef = mAnimDef;
			fGameObjectNode.Anchor = fGOTreeInstance.AnchorOffset;
			fGameObjectNode.CanonicalSize = fGOTreeInstance.CanonicalSize;
			fNode = fGameObjectNode;
		}
		else
		{
			string text;
			if (mSprite.Valid())
			{
				text = mSprite;
			}
			else
			{
				if (!mImageName.Valid())
				{
					return null;
				}
				text = mImageName;
			}
			fNode = new FSprite(text);
		}
		fNode.sortZ = 0f;
		return fNode;
	}

	public static void ReleaseNode(FNode node)
	{
		Go.killAllAbstractTweensWithTarget(node);
		if (node.OnStage)
		{
			node.Unparent();
		}
		node.Dispose();
	}
}
