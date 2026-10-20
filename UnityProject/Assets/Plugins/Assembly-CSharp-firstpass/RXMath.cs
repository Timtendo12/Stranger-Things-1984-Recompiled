using System;

// Rewritten from libil2cpp.so 1.0.605 (RXMath @ 0x16cb964..0x16cbaac, Ghidra vaddr).
public class RXMath
{
	public const float RTOD = 180f / (float)Math.PI;

	public const float DTOR = (float)Math.PI / 180f;

	public const float DOUBLE_PI = (float)Math.PI * 2f;

	public const float HALF_PI = (float)Math.PI / 2f;

	public static int Wrap(int input, int range)
	{
		input += range * 1000000;
		return input % range;
	}

	public static float Wrap(float input, float range)
	{
		return (input + range * 1000000f) % range;
	}

	public static float getDegreeDelta(float startAngle, float endAngle)
	{
		float num = (endAngle - startAngle) % 360f;
		if (num != num % 180f)
		{
			num += ((!(num >= 0f)) ? 360f : (-360f));
		}
		return num;
	}

	public static float getRadianDelta(float startAngle, float endAngle)
	{
		float num = (endAngle - startAngle) % ((float)Math.PI * 2f);
		if (num != num % (float)Math.PI)
		{
			num += ((!(num >= 0f)) ? ((float)Math.PI * 2f) : (-(float)Math.PI * 2f));
		}
		return num;
	}

	public static float PingPong(float input, float range)
	{
		float num = (input + range * 1000000f) % range / range;
		if (!(num >= 0.5f))
		{
			return num + num;
		}
		return 1f - (num - 0.5f + (num - 0.5f));
	}
}
