using System;

// Rewritten from libil2cpp.so 1.0.605 (FPersona @ 0x17b7784..0x17b7934, Ghidra vaddr).
public class FPersona
{
	protected static string Empty;

	public string PersonaId => personaId;

	public string Username => name;

	public string personaId { get; set; }

	public string profileId { get; set; }

	public string providerId { get; set; }

	public string providerTypeId { get; set; }

	public string name { get; set; }

	public string avatarUrl { get; set; }

	public string createDate { get; set; }

	static FPersona()
	{
		Empty = Guid.Empty.ToString();
	}

	public Guid GetGuid()
	{
		return new Guid(personaId);
	}

	public bool IsEmpty()
	{
		return personaId == Empty;
	}
}
