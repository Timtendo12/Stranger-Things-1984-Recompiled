using System.Threading;
using UnityEngine;

public class TextureScale
{
	public class ThreadData
	{
		public TextureScale Owner;

		public int start;

		public int end;

		public ThreadData(TextureScale owner, int s, int e)
		{
		}
	}

	private Color[] texColors;

	private Color[] newColors;

	private int w;

	private float ratioX;

	private float ratioY;

	private int w2;

	private int h2;

	private int finishCount;

	private int cores;

	private Mutex mutex;

	public static TextureScale Point(Texture2D tex, int newWidth, int newHeight)
	{
		return null;
	}

	public static TextureScale Bilinear(Texture2D tex, int newWidth, int newHeight)
	{
		return null;
	}

	private void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
	{
	}

	public Texture2D GetTextureIfDone()
	{
		return null;
	}

	public static void BilinearScale(object obj)
	{
	}

	public static void PointScale(object obj)
	{
	}

	private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
	{
		return default;
	}
}
