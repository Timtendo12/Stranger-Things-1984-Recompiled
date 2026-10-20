public interface ISaveNonRef
{
	void Save(FSavegameWriter writer);

	void Load(FSavegameReader reader);
}
