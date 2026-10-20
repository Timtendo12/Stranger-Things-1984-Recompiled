// Rewritten from libil2cpp.so 1.0.605 (FPageStringPickerString ctor, FPageStringPicker data class).
public class FPageStringPickerString
{
	public string Name;

	public string VisibleName;

	public string Details;

	public string Icon;

	public bool Selectable;

	public FPageStringPickerString(string n, string v, string d, string icon, bool selectable)
	{
		Name = n;
		VisibleName = v;
		Details = d;
		Icon = icon;
		Selectable = selectable;
	}
}
