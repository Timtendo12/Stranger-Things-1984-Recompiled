using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FPoint2 @ 0x188c970..0x188d784, Ghidra vaddr).
public struct FPoint2
{
	public int x;

	public int y;

	public static FPoint2 zero;

	public static FPoint2 invalid;

	public float Magnitude => (float)Math.Sqrt((float)(x * x + y * y));

	public int this[int index]
	{
		get
		{
			switch (index)
			{
			case 0:
				return x;
			case 1:
				return y;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		set
		{
			switch (index)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	public bool IsInvalid => x == int.MinValue;

	public bool IsValid => x != invalid.x;

	static FPoint2()
	{
		zero = new FPoint2(0, 0);
		invalid = new FPoint2(int.MinValue, int.MinValue);
	}

	public FPoint2(int newX, int newY)
	{
		x = newX;
		y = newY;
	}

	public FPoint2(Fix64 newX, Fix64 newY)
	{
		x = Fix64.FloorToInt(newX);
		y = Fix64.FloorToInt(newY);
	}

	public FPoint2(FPoint2 p)
	{
		x = p.x;
		y = p.y;
	}

	public void Set(int newX, int newY)
	{
		x = newX;
		y = newY;
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public static FPoint2 operator +(FPoint2 p1, FPoint2 p2)
	{
		return new FPoint2(p1.x + p2.x, p1.y + p2.y);
	}

	public static FPoint2 operator -(FPoint2 p1, FPoint2 p2)
	{
		return new FPoint2(p1.x - p2.x, p1.y - p2.y);
	}

	public static FPoint2 operator *(FPoint2 p1, int v)
	{
		return new FPoint2(p1.x * v, p1.y * v);
	}

	public static FPoint2 operator *(int v, FPoint2 p1)
	{
		return new FPoint2(p1.x * v, p1.y * v);
	}

	public override bool Equals(object o)
	{
		if (o == null || o.GetType() != typeof(FPoint2))
		{
			return false;
		}
		FPoint2 p = (FPoint2)o;
		return x == p.x && y == p.y;
	}

	public bool Equals(FPoint2 p)
	{
		return x == p.x && y == p.y;
	}

	public static bool operator ==(FPoint2 p1, FPoint2 p2)
	{
		return p1.x == p2.x && p1.y == p2.y;
	}

	public static bool operator !=(FPoint2 p1, FPoint2 p2)
	{
		return !(p1 == p2);
	}

	public void Zero()
	{
		x = 0;
		y = 0;
	}

	public int ConvertToID(int xSize)
	{
		return x + y * xSize;
	}

	public void SetFromID(int id, int xSize)
	{
		int row = (xSize != 0) ? (id / xSize) : 0;
		x = id - row * xSize;
		y = row;
	}

	public void Invalidate()
	{
		x = int.MinValue;
		y = int.MinValue;
	}

	public float DistanceTo(FPoint2 p)
	{
		float dx = p.x - x;
		float dy = p.y - y;
		return (float)Math.Sqrt(dx * dx + dy * dy);
	}

	public int ManhattanDistanceTo(int px, int pz)
	{
		if (this == invalid)
		{
			return int.MaxValue;
		}
		return Math.Abs(px - x) + Math.Abs(pz - y);
	}

	public int ManhattanDistanceTo(FPoint2 p)
	{
		if (p == invalid || this == invalid)
		{
			return int.MaxValue;
		}
		return Math.Abs(p.x - x) + Math.Abs(p.y - y);
	}

	public int MinimumCardinalDistance(FPoint2 p)
	{
		return Math.Min(Math.Abs(p.x - x), Math.Abs(p.y - y));
	}

	public override string ToString()
	{
		return "(" + x.ToString() + ", " + y.ToString() + ")";
	}

	public string ToStringSimple()
	{
		return x.ToString() + ", " + y.ToString();
	}

	public void AddCardinalPoints(List<FPoint2> list)
	{
		list.Add(new FPoint2(x - 1, y));
		list.Add(new FPoint2(x, y + 1));
		list.Add(new FPoint2(x + 1, y));
		list.Add(new FPoint2(x, y - 1));
	}

	public static FPoint2 Parse(string text)
	{
		string[] parts = text.Split(',');
		if (parts.Length == 2 && int.TryParse(parts[0], out var px) && int.TryParse(parts[1], out var py))
		{
			return new FPoint2(px, py);
		}
		return invalid;
	}
}
