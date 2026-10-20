using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPather @ 0x1815270..0x18170ec, Ghidra vaddr).
public class FPather
{
	public enum PathResult
	{
		None = 0,
		Fail = 1,
		Partial = 2,
		Complete = 3,
		InRangeAtStart = 4
	}

	protected FMinHeap<FPatherPoint> mOpenPoints;

	protected int[] mClosedPoints;

	protected int[] mProcessedPoints;

	protected int[] mPassablePoints;

	protected FPatherPoint[] mPoints;

	protected List<int> mIgnoreUnits;

	protected FPatherPoint mBest;

	protected float mBestEstimate;

	protected FPoint2 mStart;

	protected FPoint2 mGoal;

	protected int mStopRange = -1;

	protected int mTerrainLayer;

	public Action<string> mLog;

	protected bool mClean;

	protected FWorld mWorld;

	protected int mSizeX;

	protected int mSizeY;

	protected int mGridSize;

	private const int cIndexShift = 5;

	private const int cBitMask = 31;

	public const int cDirNone = 0;

	public const int cDirE = 1;

	public const int cDirNE = 2;

	public const int cDirN = 3;

	public const int cDirNW = 4;

	public const int cDirW = 5;

	public const int cDirSW = 6;

	public const int cDirS = 7;

	public const int cDirSE = 8;

	public const int cDirFlagNone = 1;

	public const int cDirFlagE = 2;

	public const int cDirFlagNE = 4;

	public const int cDirFlagN = 8;

	public const int cDirFlagNW = 16;

	public const int cDirFlagW = 32;

	public const int cDirFlagSW = 64;

	public const int cDirFlagS = 128;

	public const int cDirFlagSE = 256;

	private static readonly int[] ValidNextDirMask = new int[9] { 510, 398, 286, 62, 124, 248, 496, 482, 454 };

	private static readonly int[] DirectionFlags = new int[9] { 1, 2, 4, 8, 16, 32, 64, 128, 256 };

	private static readonly int[] cDeltasByDirX = new int[9] { 0, 1, 1, 0, -1, -1, -1, 0, 1 };

	private static readonly int[] cDeltasByDirY = new int[9] { 0, 0, 1, 1, 1, 0, -1, -1, -1 };

	private RaycastHit2D[] mRaycastHits = new RaycastHit2D[40];

	public FPather(FWorld world)
	{
		mWorld = world;
		mSizeX = mWorld.ProtoWorld.Width;
		mSizeY = mWorld.ProtoWorld.Height;
		mGridSize = mSizeY * mSizeX;
		mOpenPoints = new FMinHeap<FPatherPoint>(mGridSize);
		mClosedPoints = new int[(mGridSize + 31) / 32];
		mProcessedPoints = new int[(mGridSize + 31) / 32];
		mPassablePoints = new int[(mGridSize + 31) / 32];
		mPoints = new FPatherPoint[mGridSize];
		for (int i = 0; i < mGridSize; i++)
		{
			mPoints[i] = new FPatherPoint();
			mPoints[i].Init();
		}
		mIgnoreUnits = new List<int>(16);
		mStart.Invalidate();
		mGoal.Invalidate();
		CleanUp();
	}

	public void CleanUp()
	{
		mOpenPoints.Clear();
		Array.Clear(mClosedPoints, 0, mClosedPoints.Length);
		Array.Clear(mProcessedPoints, 0, mProcessedPoints.Length);
		Array.Clear(mPassablePoints, 0, mPassablePoints.Length);
		mIgnoreUnits.Clear();
		mBest = null;
		mBestEstimate = 2.1474836E+09f;
		mStart.Invalidate();
		mGoal.Invalidate();
		mLog = null;
		mClean = true;
		mStopRange = -1;
	}

	public void AddPassablePoint(FPoint2 p)
	{
		int num = p.x + mSizeX * p.y;
		mPassablePoints[num >> 5] |= 1 << (num & 0x1F);
	}

	public int GetTileIndex(int x, int y)
	{
		return x + mSizeX * y;
	}

	public FPoint2 GetTileCoords(int index)
	{
		int num = ((mSizeX != 0) ? (index / mSizeX) : 0);
		return new FPoint2(index - num * mSizeX, num);
	}

	protected bool IsTerrainTilePassable(int x, int y)
	{
		FTile tile = mWorld.GetTile(new FPoint2(x, y));
		return tile != null && tile.TerrainPassable;
	}

	protected bool IsTilePassable(int x, int y, List<int> ignoreUnits, bool checkUnits)
	{
		FTile tile = mWorld.GetTile(new FPoint2(x, y));
		if (tile == null)
		{
			return false;
		}
		if (!checkUnits)
		{
			return tile.TerrainPassable;
		}
		return tile.GetPassable(null, ignoreUnits, null);
	}

	protected int DistanceToGoal(FPoint2 point, FPoint2 goal, bool singleAxis = false)
	{
		int num = point.ManhattanDistanceTo(goal);
		int num2 = num;
		if (point.y != goal.y)
		{
			num2 = 1000;
		}
		if (!(singleAxis & (point.x != goal.x)))
		{
			num2 = num;
		}
		return num2;
	}

	public void SetStopRange(int v)
	{
		mStopRange = v;
	}

	public void SetTerrainLayer(int v)
	{
		mTerrainLayer = v;
	}

	public void AddIgnoreUnit(FUnit ignoreUnit, bool clean)
	{
		AddIgnoreUnit(ignoreUnit.ID, clean);
	}

	public void AddIgnoreUnit(int ignoreUnitID, bool clean)
	{
		if (clean)
		{
			CleanUp();
		}
		if (!mIgnoreUnits.Contains(ignoreUnitID))
		{
			mIgnoreUnits.Add(ignoreUnitID);
		}
	}

	public void AddIgnoreUnits(List<FUnit> ignoreUnits, bool clean)
	{
		if (clean)
		{
			CleanUp();
		}
		for (int i = 0; i < ignoreUnits.Count; i++)
		{
			mIgnoreUnits.Add(ignoreUnits[i].ID);
		}
	}

	public void AddIgnoreUnits(List<int> ignoreUnitIds, bool clean)
	{
		if (clean)
		{
			CleanUp();
		}
		for (int i = 0; i < ignoreUnitIds.Count; i++)
		{
			mIgnoreUnits.Add(ignoreUnitIds[i]);
		}
	}

	public PathResult FindPath(FPoint2 start, FPoint2 goal, FUnit sourceUnit, FUnit targetUnit, FPath path, int stopRange, bool smooth, FPoint2 minTile, FPoint2 maxTile, bool singleAxis, bool checkUnits = true)
	{
		if (!mClean)
		{
			CleanUp();
		}
		mClean = false;
		mStart = start;
		mStopRange = stopRange;
		mGoal = goal;
		bool sourceIsPlayer;
		bool noTarget;
		bool checkStartRange = true;
		if (sourceUnit == null)
		{
			if (targetUnit != null)
			{
				AddIgnoreUnit(targetUnit.ID, clean: false);
			}
			sourceIsPlayer = false;
			noTarget = true;
		}
		else
		{
			AddIgnoreUnit(sourceUnit.ID, clean: false);
			FTile startTile = mWorld.GetTile(start);
			if (startTile != null)
			{
				for (int i = 0; i < startTile.Units.Count; i++)
				{
					AddIgnoreUnit(startTile.Units[i].ID, clean: false);
				}
			}
			if (targetUnit == null)
			{
				sourceIsPlayer = sourceUnit.IsPlayer;
				noTarget = true;
			}
			else
			{
				AddIgnoreUnit(targetUnit.ID, clean: false);
				sourceIsPlayer = sourceUnit.IsPlayer;
				if (!targetUnit.Obstructs)
				{
					noTarget = false;
				}
				else
				{
					float num = sourceUnit.DistanceToUnitEdge(targetUnit, singleAxis);
					if (num <= FUnitAIBase.cMoveTolerance + (float)stopRange && CanUnitsSeeEachother(sourceUnit, targetUnit, num, checkUnits))
					{
						return PathResult.InRangeAtStart;
					}
					noTarget = false;
					checkStartRange = false;
				}
			}
		}
		if (checkStartRange && DistanceToGoal(start, goal, singleAxis) <= stopRange)
		{
			return PathResult.InRangeAtStart;
		}
		if (!AddFirstOpenNode(start.x, start.y))
		{
			return PathResult.Fail;
		}
		AddPassablePoint(start);
		bool reachedStopRange = false;
		bool reachedGoal = false;
		int iterations = 0;
		while (true)
		{
			if (iterations >= 10000)
			{
				break;
			}
			if (mOpenPoints.Count < 1)
			{
				break;
			}
			FPatherPoint current = mOpenPoints.ExtractMin();
			bool currentPassable = IsTilePassable(current.Point.x, current.Point.y, null, checkUnits);
			bool considerBest = current.Point == mGoal || current.Point == mStart || (current.Estimate < mBestEstimate && currentPassable);
			if (considerBest)
			{
				mBest = current;
				mBestEstimate = current.Estimate;
				int distToGoal = DistanceToGoal(current.Point, goal, singleAxis);
				bool canSee;
				bool inRange;
				if (noTarget || !targetUnit.Obstructs)
				{
					inRange = false;
					canSee = true;
				}
				else
				{
					float num2 = targetUnit.DistanceToUnitEdge(new Vector3(current.Point.x, current.Point.y, 0f), singleAxis, 0.5f);
					canSee = CanUnitsSeeEachother(sourceUnit, targetUnit, num2, checkUnits);
					inRange = canSee && num2 <= FUnitAIBase.cMoveTolerance + (float)stopRange;
				}
				bool atGoal = distToGoal < 1 || current.Point == mGoal;
				if (atGoal || inRange)
				{
					reachedGoal = true;
					break;
				}
				if (mStopRange >= 0 && distToGoal <= mStopRange && current.Point != mStart && canSee)
				{
					reachedStopRange = true;
					break;
				}
			}
			int cx = current.Point.x;
			int cy = current.Point.y;
			int dir = current.Direction;
			int currentIndex = cx + mSizeX * cy;
			for (int k = 0; k < 8; k++)
			{
				if ((DirectionFlags[k + 1] & ValidNextDirMask[dir]) == 0)
				{
					continue;
				}
				int dx = cDeltasByDirX[k + 1];
				int dy = cDeltasByDirY[k + 1];
				int nx = dx + cx;
				if (singleAxis && dx != 0 && dy != 0)
				{
					continue;
				}
				if (nx < 0)
				{
					continue;
				}
				int ny = dy + cy;
				if (ny < 0 || nx >= mSizeX || ny >= mSizeY)
				{
					continue;
				}
				if (minTile != FPoint2.invalid && maxTile != FPoint2.invalid && (nx < minTile.x || maxTile.x < nx || ny < minTile.y || maxTile.y < ny))
				{
					continue;
				}
				int nIndex = nx + mSizeX * ny;
				bool nPassable = (((mPassablePoints[nIndex >> 5] & (1 << (nIndex & 0x1F))) != 0) ? IsTerrainTilePassable(nx, ny) : IsTilePassable(nx, ny, mIgnoreUnits, checkUnits));
				if (!nPassable)
				{
					continue;
				}
				if ((mProcessedPoints[nIndex >> 5] & (1 << (nIndex & 0x1F))) == 0 && IsTerrainTilePassable(cx, ny) && IsTerrainTilePassable(nx, cy))
				{
					if (checkUnits && !CheckUnits(current.Point, new FPoint2(nx, ny), sourceUnit != null && sourceUnit.IsPlayer))
					{
						continue;
					}
					AddOpenNode(nx, ny, current, k + 1, goal, sourceIsPlayer);
				}
			}
			mClosedPoints[currentIndex >> 5] |= 1 << (currentIndex & 0x1F);
			iterations++;
		}
		if (mBest == null)
		{
			return PathResult.Fail;
		}
		BuildPath(mBest, path);
		if (!reachedStopRange && !reachedGoal && path.NumberWaypoints < 2)
		{
			return PathResult.Fail;
		}
		if (smooth)
		{
			SmoothPath(path);
		}
		if (reachedStopRange || reachedGoal)
		{
			return PathResult.Complete;
		}
		return PathResult.Partial;
	}

	protected void SmoothPath(FPath path)
	{
		int num = 1;
		if (path.NumberWaypoints - 1 <= 1)
		{
			return;
		}
		do
		{
			FPoint2 a = path.Waypoints[num - 1];
			FPoint2 b = path.Waypoints[num];
			int next = num + 1;
			FPoint2 c = path.Waypoints[next];
			if (a.x == b.x && b.x == c.x)
			{
				if (num >= 0 && num < path.NumberWaypoints)
				{
					path.RemoveWaypoint(num);
					next = num;
				}
			}
			else if (num >= 0 && a.y == b.y && b.y == c.y && num < path.NumberWaypoints)
			{
				path.RemoveWaypoint(num);
				next = num;
			}
			num = next;
		}
		while (num < path.NumberWaypoints - 1);
	}

	protected void BuildPath(FPatherPoint end, FPath waypoints)
	{
		while (end != null)
		{
			waypoints.InsertWaypoint(0, end.Point);
			end = end.PrevPoint;
		}
	}

	protected bool CanUnitsSeeEachother(FUnit unit1, FUnit unit2, float distance, bool checkUnits)
	{
		return unit1.CanSee(unit2, checkUnits) || distance < FUnitAIBase.cMoveTolerance;
	}

	protected void AddOpenNode(int x, int y, FPatherPoint prev, int direction, FPoint2 goalPoint, bool isPlayer)
	{
		int num = x + mSizeX * y;
		if (num < 0 || num >= mPoints.Length)
		{
			FLog.Warning("FPather::AddOpenNode: OpenNode is off the map!");
			return;
		}
		FPatherPoint fPatherPoint = mPoints[num];
		fPatherPoint.Init();
		fPatherPoint.PrevPoint = prev;
		fPatherPoint.Point.x = x;
		fPatherPoint.Point.y = y;
		fPatherPoint.Direction = direction;
		float num2 = 1f;
		if (prev.Point.x != x && prev.Point.y != y)
		{
			num2 = 1.414f;
		}
		fPatherPoint.Cost = prev.Cost + num2;
		FTile tile = mWorld.GetTile(fPatherPoint.Point);
		if (tile != null)
		{
			if (isPlayer && tile.AdditionalPathingCost >= 100 && goalPoint != fPatherPoint.Point)
			{
				return;
			}
			fPatherPoint.Cost += tile.AdditionalPathingCost;
		}
		fPatherPoint.Estimate = Math.Abs(x - mGoal.x) + Math.Abs(y - mGoal.y);
		mOpenPoints.Insert(fPatherPoint);
		mProcessedPoints[num >> 5] |= 1 << (num & 0x1F);
	}

	protected bool AddFirstOpenNode(int x, int y)
	{
		int num = x + mSizeX * y;
		if (num < 0 || num >= mPoints.Length)
		{
			FLog.Warning("FPather::AddFirstOpenNode: FirstOpenNode is off the map!");
			return false;
		}
		FPatherPoint fPatherPoint = mPoints[num];
		fPatherPoint.Init();
		fPatherPoint.PrevPoint = null;
		fPatherPoint.Point.x = x;
		fPatherPoint.Point.y = y;
		fPatherPoint.Direction = 0;
		fPatherPoint.Cost = 0f;
		fPatherPoint.Estimate = Math.Abs(x - mGoal.x) + Math.Abs(y - mGoal.y);
		mOpenPoints.Insert(fPatherPoint);
		mProcessedPoints[num >> 5] |= 1 << (num & 0x1F);
		return true;
	}

	protected bool CheckUnits(FPoint2 start, FPoint2 end, bool sourceIsPlayer)
	{
		FTile tile = FApp.World.GetTile(start);
		if (tile == null)
		{
			return false;
		}
		float dx = end.x - start.x;
		float dy = end.y - start.y;
		Vector2 origin = tile.Position.ToVector2();
		float distance = ((start.y != end.y && end.x != start.x) ? 1.414f : 1f);
		int num = Physics2D.CircleCastNonAlloc(origin, 0.5f, new Vector2(dx, dy), mRaycastHits, distance);
		for (int i = 0; i < num; i++)
		{
			if (mRaycastHits[i].collider == null || mRaycastHits[i].collider.gameObject == null || mRaycastHits[i].collider.isTrigger)
			{
				continue;
			}
			Vector2 point = mRaycastHits[i].point;
			if ((point.x - origin.x) * dx + (point.y - origin.y) * dy < 0f)
			{
				continue;
			}
			FUnit unitFromGO = FApp.World.GetUnitFromGO(mRaycastHits[i].collider.gameObject);
			if (unitFromGO != null && unitFromGO.Obstructs && !mIgnoreUnits.Contains(unitFromGO.ID) && (!sourceIsPlayer || !unitFromGO.IsTag("IgnoreInPather")))
			{
				return false;
			}
		}
		return true;
	}
}
