// Rewritten from libil2cpp.so 1.0.605 (FTextBuilderPools @ 0x1698bb8..0x1698d08, Ghidra vaddr).
public class FTextBuilderPools
{
	public static FMemoryPoolSimple<FLetterQuad> FLetterQuadPool = new FMemoryPoolSimple<FLetterQuad>();

	public static FMemoryPoolSimple<FLetterQuadGroup> FLetterQuadGroupPool = new FMemoryPoolSimple<FLetterQuadGroup>();

	public static FMemoryPoolSimple<FLetterQuadLine> FLetterQuadLinePool = new FMemoryPoolSimple<FLetterQuadLine>();
}
