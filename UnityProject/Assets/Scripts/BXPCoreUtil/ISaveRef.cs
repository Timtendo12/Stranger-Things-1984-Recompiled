public interface ISaveRef
{
	void SaveInternal(FSavegameWriter writer);

	void LoadInternal(FSavegameReader reader);
}
