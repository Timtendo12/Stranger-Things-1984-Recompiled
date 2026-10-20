using System;

[Serializable]
public class FProtoDoorStartData : FProtoUnitStartData
{
	public bool Locked { get; set; }

	public bool CloseOnSpawn { get; set; }

	public bool UnlockPersists { get; set; }
}
