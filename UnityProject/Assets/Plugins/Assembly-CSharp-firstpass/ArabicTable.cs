using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (ArabicTable @ 0x1600174..0x1601adc, Ghidra vaddr).
internal class ArabicTable
{
	private static List<ArabicMapping> mapList;

	private static ArabicTable arabicMapper;

	internal static ArabicTable ArabicMapper
	{
		get
		{
			if (arabicMapper == null)
			{
				arabicMapper = new ArabicTable();
			}
			return arabicMapper;
		}
	}

	private ArabicTable()
	{
		mapList = new List<ArabicMapping>
		{
			new ArabicMapping(1569, 65152), new ArabicMapping(1575, 65165), new ArabicMapping(1571, 65155), new ArabicMapping(1572, 65157),
			new ArabicMapping(1573, 65159), new ArabicMapping(1609, 64508), new ArabicMapping(1574, 65161), new ArabicMapping(1576, 65167),
			new ArabicMapping(1578, 65173), new ArabicMapping(1579, 65177), new ArabicMapping(1580, 65181), new ArabicMapping(1581, 65185),
			new ArabicMapping(1582, 65189), new ArabicMapping(1583, 65193), new ArabicMapping(1584, 65195), new ArabicMapping(1585, 65197),
			new ArabicMapping(1586, 65199), new ArabicMapping(1587, 65201), new ArabicMapping(1588, 65205), new ArabicMapping(1589, 65209),
			new ArabicMapping(1590, 65213), new ArabicMapping(1591, 65217), new ArabicMapping(1592, 65221), new ArabicMapping(1593, 65225),
			new ArabicMapping(1594, 65229), new ArabicMapping(1601, 65233), new ArabicMapping(1602, 65237), new ArabicMapping(1603, 65241),
			new ArabicMapping(1604, 65245), new ArabicMapping(1605, 65249), new ArabicMapping(1606, 65253), new ArabicMapping(1607, 65257),
			new ArabicMapping(1608, 65261), new ArabicMapping(1610, 65265), new ArabicMapping(1570, 65153), new ArabicMapping(1577, 65171),
			new ArabicMapping(1662, 64342), new ArabicMapping(1670, 64378), new ArabicMapping(1688, 64394), new ArabicMapping(1711, 64402),
			new ArabicMapping(1705, 64398), new ArabicMapping(1740, 64508)
		};
	}

	internal int Convert(int toBeConverted)
	{
		foreach (ArabicMapping map in mapList)
		{
			if (map.from == toBeConverted)
			{
				return map.to;
			}
		}
		return toBeConverted;
	}
}
