// Rewritten from libil2cpp.so 1.0.605 (FTip @ 0x17d25c4..0x17d268c, Ghidra vaddr).
public class FTip
{
	public int ID { get; set; }

	public string Name { get; set; }

	public string Details { get; set; }

	public FTip(int id)
	{
		Initialize(id);
	}

	public void Initialize(int id)
	{
		ID = id;
		Name = string.Empty;
		Details = string.Empty;
	}
}
