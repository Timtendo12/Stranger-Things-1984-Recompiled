using System;

[Flags]
public enum FClipCode
{
	None = 0,
	MinX = 1,
	MaxX = 2,
	MinY = 4,
	MaxY = 8,
	MinZ = 0x10,
	MaxZ = 0x20,
	All = -1
}
