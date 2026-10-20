using System.Linq;
using System.Text;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnityObjectSample @ 0x17dde2c..0x17dee4c, Ghidra vaddr). Debug-only memory sampler.
public class FUnityObjectSample
{
	private static StringBuilder mStringBuilder = new StringBuilder(1024);

	private string mSampleDescription = string.Empty;

	private AnimationClip[] mAnimationClips = new AnimationClip[0];

	private GameObject[] mGameObjects = new GameObject[0];

	private Material[] mMaterials = new Material[0];

	private Mesh[] mMeshes = new Mesh[0];

	private ParticleSystem[] mParticleSystems = new ParticleSystem[0];

	private Shader[] mShaders = new Shader[0];

	private Texture2D[] mTexture2Ds = new Texture2D[0];

	private Transform[] mTransforms = new Transform[0];

	public static StringBuilder StringBuilder => mStringBuilder;

	public int TotalObjects => mGameObjects.Length + mAnimationClips.Length + mMaterials.Length + mMeshes.Length + mParticleSystems.Length + mShaders.Length + mTexture2Ds.Length + mTransforms.Length;

	public void Sample(string description, bool sampleAnimClip = true, bool sampleGameObject = true, bool sampleMaterial = true, bool sampleMesh = true, bool sampleParticleSystem = true, bool sampleShader = true, bool sampleTexture2D = true, bool sampleTransform = true)
	{
		if (string.IsNullOrEmpty(description))
		{
			description = string.Empty;
		}
		mSampleDescription = description;
		mAnimationClips = (sampleAnimClip ? Resources.FindObjectsOfTypeAll<AnimationClip>() : new AnimationClip[0]);
		mGameObjects = (sampleGameObject ? Resources.FindObjectsOfTypeAll<GameObject>() : new GameObject[0]);
		mMaterials = (sampleMaterial ? Resources.FindObjectsOfTypeAll<Material>() : new Material[0]);
		mMeshes = (sampleMesh ? Resources.FindObjectsOfTypeAll<Mesh>() : new Mesh[0]);
		mParticleSystems = (sampleParticleSystem ? Resources.FindObjectsOfTypeAll<ParticleSystem>() : new ParticleSystem[0]);
		mShaders = (sampleShader ? Resources.FindObjectsOfTypeAll<Shader>() : new Shader[0]);
		mTexture2Ds = (sampleTexture2D ? Resources.FindObjectsOfTypeAll<Texture2D>() : new Texture2D[0]);
		mTransforms = (sampleTransform ? Resources.FindObjectsOfTypeAll<Transform>() : new Transform[0]);
	}

	public static FUnityObjectSample CreateDiff(FUnityObjectSample current, FUnityObjectSample previous)
	{
		FUnityObjectSample fUnityObjectSample = new FUnityObjectSample();
		mStringBuilder.Length = 0;
		mStringBuilder.AppendFormat("Delta between {0} and {1}", previous.mSampleDescription, current.mSampleDescription);
		fUnityObjectSample.mSampleDescription = mStringBuilder.ToString();
		fUnityObjectSample.mAnimationClips = current.mAnimationClips.Except(previous.mAnimationClips).ToArray();
		fUnityObjectSample.mGameObjects = current.mGameObjects.Except(previous.mGameObjects).ToArray();
		fUnityObjectSample.mMaterials = current.mMaterials.Except(previous.mMaterials).ToArray();
		fUnityObjectSample.mMeshes = current.mMeshes.Except(previous.mMeshes).ToArray();
		fUnityObjectSample.mParticleSystems = current.mParticleSystems.Except(previous.mParticleSystems).ToArray();
		fUnityObjectSample.mShaders = current.mShaders.Except(previous.mShaders).ToArray();
		fUnityObjectSample.mTexture2Ds = current.mTexture2Ds.Except(previous.mTexture2Ds).ToArray();
		fUnityObjectSample.mTransforms = current.mTransforms.Except(previous.mTransforms).ToArray();
		return fUnityObjectSample;
	}

	public override string ToString()
	{
		mStringBuilder.Length = 0;
		mStringBuilder.AppendFormat("[{0}]", mSampleDescription);
		mStringBuilder.AppendFormat("{0} Total\n", TotalObjects);
		mStringBuilder.AppendFormat("\t{0} AnimationClip\n", mAnimationClips.Length);
		mStringBuilder.AppendFormat("\t{0} GameObject\n", mGameObjects.Length);
		mStringBuilder.AppendFormat("\t{0} Material\n", mMaterials.Length);
		mStringBuilder.AppendFormat("\t{0} Mesh\n", mMeshes.Length);
		mStringBuilder.AppendFormat("\t{0} ParticleSystem\n", mParticleSystems.Length);
		mStringBuilder.AppendFormat("\t{0} Shader\n", mShaders.Length);
		mStringBuilder.AppendFormat("\t{0} Texture2D\n", mTexture2Ds.Length);
		mStringBuilder.AppendFormat("\t{0} Transform\n\n", mTransforms.Length);
		return mStringBuilder.ToString();
	}
}
