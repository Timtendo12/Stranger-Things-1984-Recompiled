using System.Collections.Generic;

public class Arc4RandomNumberGenerator
{
	private class Arc4Stream
	{
		public byte i;

		public byte j;

		public byte[] s;
	}

	private const int STIR_INCREMENT_CONST = 1600000;

	private static readonly Arc4RandomNumberGenerator instance;

	private Arc4Stream stream;

	private int count;

	public int RandomNumber()
	{
		return 0;
	}

	public void RandomValues(List<byte> result, int offset, int length)
	{
	}

	private void AddRandomData(byte[] data)
	{
	}

	private void Stir()
	{
	}

	private void StirIfNeeded()
	{
	}

	private byte GetByte()
	{
		return 0;
	}

	private int GetWord()
	{
		return 0;
	}

	public static int CryptographicallyRandomNumber()
	{
		return 0;
	}

	public static void CryptographicallyRandomValues(List<byte> buffer, int offset, int length)
	{
	}

	private static void CryptographicallyRandomValuesFromOS(byte[] buffer)
	{
	}
}
