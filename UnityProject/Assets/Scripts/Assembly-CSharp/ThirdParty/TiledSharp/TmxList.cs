using System.Collections.Generic;
using System.Collections.ObjectModel;

// Rewritten from libil2cpp.so 1.0.605 (TiledSharp.TmxList<T> @ 0x244c9a0..0x244cd00, Ghidra vaddr).
namespace TiledSharp
{
	public class TmxList<T> : KeyedCollection<string, T> where T : ITmxElement
	{
		private Dictionary<string, int> nameCount = new Dictionary<string, int>();

		public new void Add(T t)
		{
			string name = t.name;
			if (!Contains(name))
			{
				nameCount.Add(name, 0);
			}
			else
			{
				nameCount[name]++;
			}
			base.Add(t);
		}

		protected override string GetKeyForItem(T item)
		{
			string text = item.name;
			if (!text.Valid())
			{
				text = "Unnamed";
			}
			int num = (nameCount.ContainsKey(text) ? nameCount[text] : 0);
			int num2 = 0;
			while (Contains(text))
			{
				text = text + new string('_', num2) + num;
				num2++;
			}
			return text;
		}
	}
}
