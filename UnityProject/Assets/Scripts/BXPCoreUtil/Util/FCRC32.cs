using System.Text;

// Rewritten from libil2cpp.so 1.0.605 (FCRC32 @ 0x185d438..0x186a6a0, Ghidra vaddr).
public class FCRC32
{
	public static readonly uint[] Table;

	public static FCRC32 sCRC32;

	private uint _value = uint.MaxValue;

	static FCRC32()
	{
		sCRC32 = new FCRC32();
		Table = new uint[256];
		for (uint num = 0u; num < 256; num++)
		{
			uint num2 = num;
			for (int i = 0; i < 8; i++)
			{
				num2 = (((num2 & 1) == 0) ? (num2 >> 1) : ((num2 >> 1) ^ 0xEDB88320u));
			}
			Table[num] = num2;
		}
	}

	public void Init()
	{
		_value = uint.MaxValue;
	}

	public void UpdateByte(byte b)
	{
		_value = Table[(_value ^ b) & 0xFF] ^ (_value >> 8);
	}

	public void Update(byte[] data, uint offset, long size)
	{
		for (long num = 0L; num < size; num++)
		{
			_value = Table[(_value & 0xFF) ^ data[offset + num]] ^ (_value >> 8);
		}
	}

	public uint GetDigest()
	{
		return _value ^ uint.MaxValue;
	}

	public static uint CalculateDigest(byte[] data, uint offset, long size)
	{
		sCRC32.Init();
		sCRC32.Update(data, offset, size);
		return sCRC32.GetDigest();
	}

	public static uint CalculateDigest(byte[] data)
	{
		return CalculateDigest(data, 0u, data.Length);
	}

	public static uint CalculateDigest(string str)
	{
		sCRC32.Init();
		return CalculateDigest(Encoding.ASCII.GetBytes(str));
	}

	public static bool VerifyDigest(uint digest, byte[] data, uint offset, long size)
	{
		return CalculateDigest(data, offset, size) == digest;
	}
}
