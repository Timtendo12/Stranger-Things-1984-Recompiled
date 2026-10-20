using System.Collections.Generic;
using UnityEngine;

public class FDynamicAtlas : FAtlas
{
	protected class FDynamicAtlasEntry
	{
		public string Name;

		public long FileOffset;

		public int CompressedSize;

		public int Width;

		public int Height;

		public FAtlasElement AtlasElement;

		public int CacheX;

		public int CacheY;

		public LinkedListNode<FDynamicAtlasEntry> mMRUNode;
	}

	protected Dictionary<string, FDynamicAtlasEntry> mEntries;

	protected int mCacheWidth;

	protected int mCacheHeight;

	protected float mCacheWidthInverse;

	protected float mCacheHeightInverse;

	protected int mCellWidth;

	protected int mCellHeight;

	protected int mNumCellsX;

	protected int mNumCellsY;

	protected Color32[] mCellBuffer;

	protected Color32[] mClearBuffer;

	protected byte[] mCellByteBuffer;

	protected Texture2D mCellTexture;

	protected bool mDirty;

	protected int mNextEmptyCacheX;

	protected int mNextEmptyCacheY;

	protected LinkedList<FDynamicAtlasEntry> mCachedEntriesInMRUOrder;

	protected string mRootPath;

	protected const int cMaxSupportedVersion = 0;

	public FDynamicAtlas(string name, int index, int cacheWidth, int cacheHeight)
		: base(null, null, 0)
	{
	}

	public override bool UnloadTexture()
	{
		return false;
	}

	public override void OnDestroy()
	{
	}

	protected void LoadEntryIntoCache(FDynamicAtlasEntry entry, int cacheX, int cacheY)
	{
	}

	public override bool Redraw(FAtlasElement element)
	{
		return false;
	}

	protected void GetLRUCacheLocation(out int x, out int y)
	{
		x = default;
		y = default;
	}

	public override void Flush()
	{
	}
}
