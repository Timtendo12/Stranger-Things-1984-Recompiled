using System.Collections.Generic;
using System.Linq;

// Rewritten from libil2cpp.so 1.0.605 (FDialogueLine @ 0x17eb5f8..0x17eb984, Ghidra vaddr).
public class FDialogueLine
{
	public string Speaker { get; set; }

	public string SpeakerName => FCoreUtil.Valid(Speaker) ? Speaker.Split('.').Last() : null;

	public string LocSpeaker => FCoreUtil.Valid(Speaker) ? FLoc.GetString(Speaker) : null;

	public string Line { get; set; }

	public string LocLine => FCoreUtil.Valid(Line) ? FLoc.GetString(Line) : null;

	public bool RunTagsFirst { get; set; }

	public List<FAnimTag> Tags { get; set; }

	public void EvaluateTags(IAnimTagResponder TagResponder)
	{
		if (Tags == null)
		{
			throw new System.NullReferenceException();
		}
		for (int i = 0; i < Tags.Count; i++)
		{
			TagResponder.FireTag(Tags[i], -1);
		}
	}
}
