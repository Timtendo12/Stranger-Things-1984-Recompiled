// Standard LZMA SDK (public domain), restored to match the shipped signatures.
namespace SevenZip
{
	internal class CRC
	{
		public static readonly uint[] Table;

		private uint _value = uint.MaxValue;

		static CRC()
		{
			Table = new uint[256];
			for (uint i = 0u; i < 256; i++)
			{
				uint num = i;
				for (int j = 0; j < 8; j++)
				{
					if ((num & 1) != 0)
					{
						num = (num >> 1) ^ 0xEDB88320u;
					}
					else
					{
						num >>= 1;
					}
				}
				Table[i] = num;
			}
		}

		public void Init()
		{
			_value = uint.MaxValue;
		}

		public void UpdateByte(byte b)
		{
			_value = Table[(byte)_value ^ b] ^ (_value >> 8);
		}

		public void Update(byte[] data, uint offset, uint size)
		{
			for (uint i = 0u; i < size; i++)
			{
				_value = Table[(byte)_value ^ data[offset + i]] ^ (_value >> 8);
			}
		}

		public uint GetDigest()
		{
			return _value ^ uint.MaxValue;
		}

		private static uint CalculateDigest(byte[] data, uint offset, uint size)
		{
			CRC cRC = new CRC();
			cRC.Update(data, offset, size);
			return cRC.GetDigest();
		}

		private static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
		{
			return CalculateDigest(data, offset, size) == digest;
		}
	}
}
