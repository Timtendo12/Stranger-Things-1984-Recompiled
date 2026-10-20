using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUtil @ 0x1666ef8..0x1672f00, Ghidra vaddr).
public static class FUtil
{
	public static Vector3 Vec3Epsilon;

	public static Vector3 Vec3Min;

	public static Dictionary<Type, object> mLerpFuncDictionary;

	public static Color TransparentBlack;

	public static int ASCIIBackspace;

	public static int ASCIIHorizontalTab;

	public static int ASCIIVerticalTab;

	public static int ASCIILineFeed;

	public static int ASCIICarriageReturn;

	public static int ASCIIEscape;

	public static int ASCIIDelete;

	public static Dictionary<string, GameObject> cLoadedPrefabs;

	private static string mPersistentDataPath;

	public static string DataPath { get; set; }

	public static string PersistentDataPath
	{
		get
		{
			if (mPersistentDataPath == null)
			{
				SetPersistentDataPathStem(null);
			}
			return mPersistentDataPath;
		}
	}

	public static Vector3 mousePosition => PCDisplay.MousePosition;

	static FUtil()
	{
		Vec3Epsilon = new Vector3(FMath.Epsilon, FMath.Epsilon, FMath.Epsilon);
		Vec3Min = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		mLerpFuncDictionary = new Dictionary<Type, object>();
		TransparentBlack = new Color(0f, 0f, 0f, 0f);
		ASCIIBackspace = 8;
		ASCIIHorizontalTab = 9;
		ASCIIVerticalTab = 11;
		ASCIILineFeed = 10;
		ASCIICarriageReturn = 13;
		ASCIIEscape = 27;
		ASCIIDelete = 127;
		cLoadedPrefabs = new Dictionary<string, GameObject>();
		mPersistentDataPath = null;
		FMath.AddLerpFunc(typeof(Vector2), new Func<Vector2, Vector2, float, Vector2>(Vector2.Lerp));
		FMath.AddLerpFunc(typeof(Vector3), new Func<Vector3, Vector3, float, Vector3>(Vector3.Lerp));
		FMath.AddLerpFunc(typeof(Vector4), new Func<Vector4, Vector4, float, Vector4>(Vector4.Lerp));
		FMath.AddLerpFunc(typeof(Color), new Func<Color, Color, float, Color>(Color.Lerp));
	}

	// 0x166df64
	public static string BalancedWordWrap(string input, int lines)
	{
		string result = input;
		if (lines > 1)
		{
			result = string.Empty;
			int perLine = input.Length / lines;
			int count = 0;
			for (int i = 0; i < input.Length; i++)
			{
				if (count < perLine || input[i] != ' ')
				{
					result += input[i].ToString();
				}
				else
				{
					result += "\n"; // literal slot 0x32085a8
					count = -1;
				}
				count++;
			}
		}
		return result;
	}

	public static Texture2D LoadSplitTexture(string colorAssetName, string alphaAssetName, bool quietOnMissing = false)
	{
		// TODO(port): split colour/alpha atlases are not present in this build; kept as a straight texture load. [left: Android split atlases / asset bundles do not exist in the PC build]
		return LoadAssetAsTexture(colorAssetName, quietOnMissing) as Texture2D;
	}

	public static Color ParseXMLColorNode(XmlElement root)
	{
		string hex = root.GetAttrib("Color");
		if (FCoreUtil.Valid(hex))
		{
			return ColorFromHex(hex);
		}
		if (!root.HasAttribute("R"))
		{
			return Color.white;
		}
		return new Color(root.GetAttrib("R", 1f), root.GetAttrib("G", 1f), root.GetAttrib("B", 1f), root.GetAttrib("A", 1f));
	}

	public static Color ColorFromHex(string hex)
	{
		return default(Color).FromHex(hex);
	}

	public static Color FromHex(this Color color, string hex)
	{
		if (hex == null || (hex.Length != 8 && hex.Length != 6))
		{
			return new Color(0f, 0f, 0f, 0f);
		}
		int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
		int a = hex.Length == 6 ? 255 : int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
		return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
	}

	public static string HexFromColor(Color color)
	{
		return "" + ((int)(color.r * 255f)).ToString("X2") + ((int)(color.g * 255f)).ToString("X2") + ((int)(color.b * 255f)).ToString("X2");
	}

	public static Color FromDWORD(this Color color, uint dwordColor)
	{
		return new Color(((dwordColor >> 16) & 0xFF) / 255f, ((dwordColor >> 8) & 0xFF) / 255f, (dwordColor & 0xFF) / 255f, ((dwordColor >> 24) & 0xFF) / 255f);
	}

	public static Color FromDWORDNA(this Color color, uint dwordColor)
	{
		return new Color(((dwordColor >> 16) & 0xFF) / 255f, ((dwordColor >> 8) & 0xFF) / 255f, (dwordColor & 0xFF) / 255f, 1f);
	}

	public static float DistanceToLineSegment(Vector3 point, Vector3 lineSegmentPoint1, Vector3 lineSegmentPoint2, out Vector3 closestPoint)
	{
		return Mathf.Sqrt(DistanceToLineSegmentSquared(point, lineSegmentPoint1, lineSegmentPoint2, out closestPoint));
	}

	public static float DistanceToLineSegmentSquared(Vector3 point, Vector3 lineSegmentPoint1, Vector3 lineSegmentPoint2, out Vector3 closestPoint)
	{
		Vector3 vector = lineSegmentPoint2 - lineSegmentPoint1;
		float sqrMagnitude = vector.sqrMagnitude;
		if (FMath.Epsilon <= sqrMagnitude)
		{
			float num = Vector3.Dot(point - lineSegmentPoint1, vector) / sqrMagnitude;
			if (!(num > 0f))
			{
				closestPoint = lineSegmentPoint1;
			}
			else if (!(num < 1f))
			{
				closestPoint = lineSegmentPoint2;
			}
			else
			{
				closestPoint = lineSegmentPoint1 + vector * num;
			}
			return (point - closestPoint).sqrMagnitude;
		}
		closestPoint = lineSegmentPoint1;
		return (point - lineSegmentPoint1).sqrMagnitude;
	}

	public static float DistancePointToRectangle(Vector3 point, FRect rect, out Vector3 closestPoint)
	{
		// the shipped code passes the same out parameter to all four edge tests, so closestPoint ends up belonging to the last edge
		float a = DistanceToLineSegment(point, rect.Min, rect.MinXMaxZ, out closestPoint);
		float b = DistanceToLineSegment(point, rect.MinXMaxZ, rect.Max, out closestPoint);
		float c = DistanceToLineSegment(point, rect.Max, rect.MaxXMinZ, out closestPoint);
		float d = DistanceToLineSegment(point, rect.MaxXMinZ, rect.Min, out closestPoint);
		float num = Mathf.Min(Mathf.Min(Mathf.Min(a, b), c), d);
		return Mathf.Max(0f, num);
	}

	public static float DistanceCircleToRectangle(Vector3 point, float radius, FRect rect, out Vector3 closestPoint)
	{
		return Mathf.Max(0f, DistancePointToRectangle(point, rect, out closestPoint) - radius);
	}

	public static void ComputeProjectileTimeVelocityAndGravity(Vector3 startPt, Vector3 endPt, float maxHeight, float xzSpeed, out float flightTime, out Vector3 initialVel, out float gravity)
	{
		Vector3 vector = new Vector3(endPt.x - startPt.x, 0f, endPt.z - startPt.z);
		flightTime = vector.magnitude / xzSpeed;
		Vector3 vector2 = ((vector.magnitude > 1E-05f) ? (vector / vector.magnitude) : Vector3.zero);
		initialVel = new Vector3(vector2.x * xzSpeed, 0f, vector2.z * xzSpeed);
		float num = Mathf.Max(startPt.y + maxHeight, endPt.y);
		float num2 = num + Mathf.Sqrt((num - endPt.y) * (num - startPt.y)) - startPt.y;
		num2 = (num2 + num2) / flightTime;
		initialVel.y = num2;
		if (num <= startPt.y)
		{
			gravity = (startPt.y - endPt.y + (startPt.y - endPt.y)) / (flightTime * flightTime);
		}
		else
		{
			gravity = num2 * num2 * 0.5f / (num - startPt.y);
		}
	}

	public static bool EpsilonEqual(Vector3 p1, Vector3 p2)
	{
		return Mathf.Abs(p2.x - p1.x) < FMath.Epsilon && Mathf.Abs(p2.y - p1.y) < FMath.Epsilon && Mathf.Abs(p2.z - p1.z) < FMath.Epsilon;
	}

	public static bool IntersectLineAABB(Vector2 startPt, Vector2 endPt, Vector2 boxMin, Vector2 boxMax, out Vector2 isectPt)
	{
		isectPt = Vector2.zero;
		Vector2 vector = endPt - startPt;
		float num = vector.magnitude;
		if (!(FMath.Epsilon <= num))
		{
			return false;
		}
		vector /= num;
		float num2 = 0f;
		for (int i = 0; i < 2; i++)
		{
			float num3 = ((i == 0) ? vector.x : vector.y);
			float num4 = ((i == 0) ? startPt.x : startPt.y);
			float num5 = ((i == 0) ? boxMin.x : boxMin.y);
			float num6 = ((i == 0) ? boxMax.x : boxMax.y);
			if (FMath.Epsilon <= Mathf.Abs(num3))
			{
				float num7 = 1f / num3;
				float num8 = (num5 - num4) * num7;
				float num9 = (num6 - num4) * num7;
				if (num8 > num9)
				{
					float num10 = num8;
					num8 = num9;
					num9 = num10;
				}
				if (num2 <= num8)
				{
					num2 = num8;
				}
				if (num9 <= num)
				{
					num = num9;
				}
				if (num < num2)
				{
					return false;
				}
			}
			else if (num4 < num5 || num4 > num6)
			{
				return false;
			}
		}
		isectPt = startPt + vector * num2;
		return true;
	}

	public static Cardinal IncomingDirection2D(Vector3 sourceToTarget, Vector3 targetFacing)
	{
		return Cardinal.Left;
	}

	public static float AngleBetween2D(Vector3 vector1, Vector3 vector2)
	{
		float num = Mathf.Atan2(vector2.z, vector2.x) - Mathf.Atan2(vector1.z, vector1.x);
		if (num < 0f)
		{
			num += (float)Math.PI * 2f;
		}
		return num;
	}

	public static string LoadTextAsset(string filename)
	{
		return LoadTextAsset(filename, out byte[] _, false);
	}

	public static string LoadTextAsset(string filename, out byte[] hash)
	{
		return LoadTextAsset(filename, out hash, true);
	}

	public static string LoadTextAsset(string filename, out byte[] hash, bool computeHash)
	{
		hash = null;
		TextAsset asset = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
		if (asset == null)
		{
			return null;
		}
		string text = asset.text;
		Resources.UnloadAsset(asset);
		if (computeHash)
		{
			hash = FCoreUtil.SHA1Hash(text);
		}
		return text;
	}

	public static XmlElement LoadXmlAsset(string filename)
	{
		return LoadXmlAsset(filename, out byte[] _, false);
	}

	public static XmlElement LoadXmlAsset(string filename, out byte[] hash)
	{
		return LoadXmlAsset(filename, out hash, true);
	}

	public static XmlElement LoadXmlAsset(string filename, out byte[] hash, bool computeHash)
	{
		XmlDocument doc = new XmlDocument();
		string text = LoadTextAsset(filename, out hash, computeHash);
		if (text == null)
		{
			return null;
		}
		doc.LoadXml(text);
		if (doc.DocumentElement != null)
		{
			return doc.DocumentElement;
		}
		FLog.UnityError("Failed to load/parse: " + filename);
		return null;
	}

	public static Transform FindBoneRecursive(string name, Transform root)
	{
		if (root.name == name)
		{
			return root;
		}
		for (int i = 0; i < root.childCount; i++)
		{
			Transform found = FindBoneRecursive(name, root.GetChild(i));
			if (found != null)
			{
				return found;
			}
		}
		return null;
	}

	public static Transform FindBone(string name, Transform[] skeleton)
	{
		for (int i = 0; i < skeleton.Length; i++)
		{
			Transform found = FindBoneRecursive(name, skeleton[i]);
			if (found != null)
			{
				return found;
			}
		}
		return null;
	}

	public static Dictionary<string, object> ParseJSON(this string json)
	{
		return MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
	}

	public static Texture LoadAssetAsTexture(string assetName, bool quietOnMissing = false)
	{
		// TODO(port): the shipped build also tried asset bundles; on PC all content is in Resources. [left: Android split atlases / asset bundles do not exist in the PC build]
		Texture asset = Resources.Load<Texture>(assetName);
		if (asset != null)
		{
			return asset;
		}
		TextAsset bytes = Resources.Load<TextAsset>(assetName);
		if (bytes == null)
		{
			if (!quietOnMissing)
			{
				FLog.UnityError("Failed to load texture: " + assetName);
			}
			return null;
		}
		Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
		tex.LoadImage(bytes.bytes, false);
		Resources.UnloadAsset(bytes);
		return tex;
	}

	public static XmlDocument LoadXmlAssetDoc(string filename, bool preserveWhitespace = false)
	{
		XmlDocument doc = new XmlDocument();
		doc.PreserveWhitespace = preserveWhitespace;
		TextAsset asset = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
		if (asset == null)
		{
			return null;
		}
		doc.LoadXml(asset.text);
		if (doc.DocumentElement == null)
		{
			FLog.UnityError("Failed to load/parse: " + filename);
			return null;
		}
		Resources.UnloadAsset(asset);
		return doc;
	}

	private static bool NearestNonTrigger(RaycastHit[] hits, out RaycastHit hitInfo)
	{
		int num = -1;
		float num2 = float.MaxValue;
		for (int i = 0; i < hits.Length; i++)
		{
			if (!hits[i].collider.isTrigger && hits[i].distance < num2)
			{
				num2 = hits[i].distance;
				num = i;
			}
		}
		if (num >= 0)
		{
			hitInfo = hits[num];
			return true;
		}
		hitInfo = default(RaycastHit);
		return false;
	}

	public static bool RaycastWithoutTriggers(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance = 1f / 0f, int layerMask = -5)
	{
		RaycastHit[] array = Physics.RaycastAll(origin, direction, distance, layerMask);
		return NearestNonTrigger(array, out hitInfo);
	}

	public static bool SphereCastWithoutTriggers(Vector3 origin, Vector3 direction, float radius, out RaycastHit hitInfo, float distance = 1f / 0f, int layerMask = -5)
	{
		RaycastHit[] array = Physics.SphereCastAll(origin, radius, direction, distance, layerMask);
		return NearestNonTrigger(array, out hitInfo);
	}

	public static Vector3 ConvertTouchToWorldPoint(Vector2 p, float worldZ)
	{
		return ConvertTouchToWorldPoint(Camera.main, p, worldZ);
	}

	public static Vector3 ConvertTouchToWorldPoint(Camera camera, Vector2 p, float worldZ)
	{
		return camera.ScreenToWorldPoint(new Vector3(p.x, p.y, worldZ));
	}

	public static bool GetBoundsRecursive(GameObject gameObject, out Bounds resultBounds)
	{
		resultBounds = default(Bounds);
		bool flag = false;
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!flag)
			{
				resultBounds = renderer.bounds;
				flag = true;
			}
			else
			{
				resultBounds.Encapsulate(renderer.bounds);
			}
		}
		return flag;
	}

	public static void EnableChildrenRecursive(GameObject parent, List<string> names, bool enable)
	{
		// 0x167099c: verified, children whose name is in the list get SetActive(enable), recursing into every child
		foreach (Transform item in parent.transform)
		{
			if (names.Contains(item.gameObject.name))
			{
				item.gameObject.SetActive(enable);
			}
			EnableChildrenRecursive(item.gameObject, names, enable);
		}
	}

	public static GameObject SearchForPrefab(string prefabName)
	{
		string[] folders = { "Prefabs/", "Prefabs/Collectables/", "Prefabs/Misc/", "Prefabs/Platforms/", "Prefabs/Hazards/", "Prefabs/Mobs/", "Prefabs/Gibs/" };
		foreach (string folder in folders)
		{
			GameObject prefab = Resources.Load(folder + prefabName) as GameObject;
			if (prefab != null)
			{
				return prefab;
			}
		}
		return null;
	}

	public static byte[] PlatformDecodeAssetAsImage(string assetName, out int width, out int height, out int bpp, bool quietOnMissing = false)
	{
		// the shipped code decodes with a native image library; Unity's own decoder is used on PC
		width = 0;
		height = 0;
		bpp = 4;
		Texture2D texture2D = Resources.Load<Texture2D>(assetName);
		if (texture2D == null)
		{
			if (!quietOnMissing)
			{
				FLog.UnityWarning("PlatformDecodeAssetAsImage: could not load " + assetName);
			}
			return null;
		}
		width = texture2D.width;
		height = texture2D.height;
		Color32[] pixels = texture2D.GetPixels32();
		byte[] array = new byte[pixels.Length * 4];
		for (int i = 0; i < pixels.Length; i++)
		{
			array[i * 4] = pixels[i].r;
			array[i * 4 + 1] = pixels[i].g;
			array[i * 4 + 2] = pixels[i].b;
			array[i * 4 + 3] = pixels[i].a;
		}
		return array;
	}

	public static Color32[] DecodeAssetAsImage(string assetName, out int width, out int height, bool quietOnMissing = false)
	{
		width = 0;
		height = 0;
		Texture2D texture2D = Resources.Load<Texture2D>(assetName);
		if (texture2D == null)
		{
			if (!quietOnMissing)
			{
				FLog.UnityWarning("DecodeAssetAsImage: could not load " + assetName);
			}
			return null;
		}
		width = texture2D.width;
		height = texture2D.height;
		return texture2D.GetPixels32();
	}

	public static GameObject LoadPrefab(string name)
	{
		if (!cLoadedPrefabs.ContainsKey(name))
		{
			cLoadedPrefabs[name] = Resources.Load<GameObject>(name);
		}
		return cLoadedPrefabs[name];
	}

	public static GameObject ClonePrefab(string name, Vector3 position = default(Vector3))
	{
		GameObject prefab = LoadPrefab(name);
		if (prefab == null)
		{
			return null;
		}
		if (position == Vector3.zero)
		{
			// the shipped code instantiates at the prefab's own position when no position is given (e.g. WorldCamera z = -11.3)
			position = prefab.transform.position;
		}
		return prefab.Clone(position);
	}

	public static GameObject ClonePrefabUsePrefabPos(string name)
	{
		GameObject prefab = LoadPrefab(name);
		if (prefab == null)
		{
			return null;
		}
		return prefab.Clone(prefab.transform.position);
	}

	public static byte[] DecodeFromData(byte[] data, out int width, out int height, out int bpp)
	{
		Texture2D texture2D = new Texture2D(2, 2);
		if (!texture2D.LoadImage(data))
		{
			width = 0;
			height = 0;
			bpp = 4;
			return null;
		}
		width = texture2D.width;
		height = texture2D.height;
		bpp = 4;
		Color32[] pixels = texture2D.GetPixels32();
		byte[] array = new byte[pixels.Length * 4];
		for (int i = 0; i < pixels.Length; i++)
		{
			array[i * 4] = pixels[i].r;
			array[i * 4 + 1] = pixels[i].g;
			array[i * 4 + 2] = pixels[i].b;
			array[i * 4 + 3] = pixels[i].a;
		}
		return array;
	}

	public static string ToMixpanelString(this DateTime dateTime)
	{
		return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
	}

	public static int ToMixpanelInt(this DateTime dateTime)
	{
		return (int)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
	}

	public static bool Equal(Color32 color1, Color32 color2)
	{
		return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b && color1.a == color2.a;
	}

	public static float HackyGetFloatFromCurrency(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0f;
		}
		System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
		foreach (char c in str)
		{
			if (char.IsDigit(c))
			{
				stringBuilder.Append(c);
			}
			else if (c == '.' || c == ',')
			{
				stringBuilder.Append('.');
			}
		}
		float.TryParse(stringBuilder.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var result);
		return result;
	}

	public static bool IntersectRayTriangle(Ray ray, Vector3 a, Vector3 b, Vector3 c, out float intersectDist)
	{
		intersectDist = 0f;
		Vector3 vector = b - a;
		Vector3 vector2 = c - a;
		Vector3 rhs = Vector3.Cross(ray.direction, vector2);
		float num = Vector3.Dot(vector, rhs);
		if (num > -1E-06f && num < 1E-06f)
		{
			return false;
		}
		float num2 = 1f / num;
		Vector3 lhs = ray.origin - a;
		float num3 = Vector3.Dot(lhs, rhs) * num2;
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		Vector3 rhs2 = Vector3.Cross(lhs, vector);
		float num4 = Vector3.Dot(ray.direction, rhs2) * num2;
		if (num4 < 0f || num3 + num4 > 1f)
		{
			return false;
		}
		float num5 = Vector3.Dot(vector2, rhs2) * num2;
		if (num5 < 0f)
		{
			return false;
		}
		intersectDist = num5;
		return true;
	}

	public static bool IntersectMeshRecursive(GameObject gameObject, Ray ray, out float intersectDist)
	{
		intersectDist = float.MaxValue;
		bool result = false;
		MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			Mesh sharedMesh = meshFilter.sharedMesh;
			if (sharedMesh == null)
			{
				continue;
			}
			Vector3[] vertices = sharedMesh.vertices;
			int[] triangles = sharedMesh.triangles;
			Transform transform = meshFilter.transform;
			for (int j = 0; j + 2 < triangles.Length; j += 3)
			{
				Vector3 a = transform.TransformPoint(vertices[triangles[j]]);
				Vector3 b = transform.TransformPoint(vertices[triangles[j + 1]]);
				Vector3 c = transform.TransformPoint(vertices[triangles[j + 2]]);
				if (IntersectRayTriangle(ray, a, b, c, out var intersectDist2) && intersectDist2 < intersectDist)
				{
					intersectDist = intersectDist2;
					result = true;
				}
			}
		}
		return result;
	}

	public static void SafeSetColor(this Material material, string name, Color color)
	{
		if (material.HasProperty(name))
		{
			material.SetColor(name, color);
		}
	}

	public static void SetPersistentDataPathStem(string stem)
	{
		if (string.IsNullOrEmpty(stem))
		{
			mPersistentDataPath = Application.persistentDataPath;
			return;
		}
		mPersistentDataPath = Application.persistentDataPath + "/" + stem;
		System.IO.Directory.CreateDirectory(mPersistentDataPath);
	}

	public static void DrawAxesGizmos(Vector3 position, Quaternion rotation, float size)
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(position, position + rotation * Vector3.right * size);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(position, position + rotation * Vector3.up * size);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(position, position + rotation * Vector3.forward * size);
	}

	public static Vector2 ClampToViewRect(this Camera cam, Vector2 position, Vector2 size)
	{
		// Port of FUtil.ClampToViewRect @ 0x1671f8c (formula from the map-widgets notes).
		float ortho = cam.orthographicSize;
		float halfW = ortho * (float)PCDisplay.Width / (float)PCDisplay.Height * cam.rect.width;
		Vector3 c = cam.transform.position;
		float x = Mathf.Clamp(position.x, c.x - halfW + size.x / 2f, c.x + halfW - size.x / 2f);
		float y = Mathf.Clamp(position.y, c.y - ortho + size.y / 2f, c.y + ortho - size.y / 2f);
		return new Vector2(x, y);
	}

	public static T GetOrCreateComponent<T>(this GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		if (component != null)
		{
			return component;
		}
		return go.AddComponent<T>();
	}

	public static Vector3 WorldToCenteredViewportPoint(this Camera camera, Vector3 pos)
	{
		if (camera == null)
		{
			return Vector3.zero;
		}
		Vector3 vector = camera.WorldToViewportPoint(pos);
		return new Vector3((vector.x - 0.5f) * 2f, (vector.y - 0.5f) * 2f, vector.z);
	}

	public static string CreateGetUrl(string url, params string[] paramPairs)
	{
		System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(url);
		for (int i = 0; i + 1 < paramPairs.Length; i += 2)
		{
			stringBuilder.Append((i == 0) ? "?" : "&");
			stringBuilder.Append(Uri.EscapeDataString(paramPairs[i]));
			stringBuilder.Append("=");
			stringBuilder.Append(Uri.EscapeDataString(paramPairs[i + 1]));
		}
		return stringBuilder.ToString();
	}

	public static string GuessGitHash(string gitDir, bool appendRefToHash)
	{
		try
		{
			string path = System.IO.Path.Combine(gitDir, "HEAD");
			if (!System.IO.File.Exists(path))
			{
				return null;
			}
			string text = System.IO.File.ReadAllText(path).Trim();
			if (!text.StartsWith("ref: "))
			{
				return text;
			}
			string text2 = text.Substring(5);
			string path2 = System.IO.Path.Combine(gitDir, text2);
			if (!System.IO.File.Exists(path2))
			{
				return null;
			}
			string text3 = System.IO.File.ReadAllText(path2).Trim();
			return appendRefToHash ? (text3 + " " + text2) : text3;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
