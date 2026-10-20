using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;

[JsonConverter(typeof(FResourceBucketSerializer))]
[SaveVersion(0)]
public class FResourceBucket
{
	protected List<FResourceEntry> mResources;

	public int NumberResources => 0;

	public void Initialize()
	{
	}

	public Dictionary<string, Fix64> ToDictionary()
	{
		return null;
	}

	public void SaveXML(XmlElement root, string createParentName = null)
	{
	}

	public void LoadXML(XmlElement root, string findParentName = null)
	{
	}

	public FResourceBucket DeepCopy()
	{
		return null;
	}

	public void ClearAmounts()
	{
	}

	public int GetNumberNonZeroResources()
	{
		return 0;
	}

	public bool GetNonZeroResource(int index, out int resourceID, out Fix64 resourceAmount)
	{
		resourceID = default;
		resourceAmount = default;
		return false;
	}

	public bool GetFirstNonZeroResource(out int resourceID, out Fix64 resourceAmount)
	{
		resourceID = default;
		resourceAmount = default;
		return false;
	}

	public Fix64 GetAmount(int id)
	{
		return default;
	}

	public Fix64 GetAmount(string name)
	{
		return default;
	}

	public bool HasAnything()
	{
		return false;
	}

	public bool HasMaximum(int id)
	{
		return false;
	}

	public bool SetAmount(int id, Fix64 v)
	{
		return false;
	}

	public bool SetAmount(string name, Fix64 v)
	{
		return false;
	}

	public bool AdjustAmount(int id, Fix64 v)
	{
		return false;
	}

	public bool AdjustAmount(string name, Fix64 v)
	{
		return false;
	}

	public bool AdjustAmount(FResourceBucket amount)
	{
		return false;
	}

	public bool SubtractAmount(int id, Fix64 v)
	{
		return false;
	}

	public bool SubtractAmount(string name, Fix64 v)
	{
		return false;
	}

	public bool SubtractAmount(FResourceBucket amount)
	{
		return false;
	}

	public bool MultiplyAmount(int id, Fix64 factor)
	{
		return false;
	}

	public bool MultiplyAmount(Fix64 factor)
	{
		return false;
	}

	public bool IsMaxed(int id)
	{
		return false;
	}

	public bool IsMaxed(string name)
	{
		return false;
	}

	public Fix64 GetMaximum(int id)
	{
		return default;
	}

	public Fix64 GetMaximum(string name)
	{
		return default;
	}

	public bool SetMaximum(string name, Fix64 v)
	{
		return false;
	}

	public bool SetMaximum(int id, Fix64 v)
	{
		return false;
	}

	public bool AdjustMaximum(int id, Fix64 v)
	{
		return false;
	}

	public bool AdjustMaximum(string name, Fix64 v)
	{
		return false;
	}

	public bool CanPayAmount(FResourceBucket b)
	{
		return false;
	}

	public bool CanPayAmount(int id, Fix64 amount)
	{
		return false;
	}

	public bool PayAmount(FResourceBucket b)
	{
		return false;
	}

	public bool PayAmount(int id, Fix64 amount)
	{
		return false;
	}

	public static Fix64 GetAmount(FResourceBucket bucket, string name)
	{
		return default;
	}

	public static bool SetAmount(FResourceBucket bucket, string name, Fix64 value)
	{
		return false;
	}
}
