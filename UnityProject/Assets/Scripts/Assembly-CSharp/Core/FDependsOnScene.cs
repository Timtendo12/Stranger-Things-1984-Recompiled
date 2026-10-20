using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Rewritten from libil2cpp.so 1.0.605 (FDependsOnScene @ 0x16dfe38..0x16dffb8, Ghidra vaddr).
[ExecuteInEditMode]
public class FDependsOnScene : MonoBehaviour
{
	public string SceneName;

	public void Awake()
	{
		if (Application.isPlaying)
		{
			SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
		}
	}

	protected IEnumerator EditorLoadScene()
	{
		yield return null;
	}
}
