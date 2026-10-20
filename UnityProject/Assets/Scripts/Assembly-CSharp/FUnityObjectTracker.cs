// Rewritten from libil2cpp.so 1.0.605 (FUnityObjectTracker @ 0x17deecc..0x17df60c, Ghidra vaddr). Debug-only memory sampler.
public class FUnityObjectTracker
{
	protected Fix64 mTimeUntilNextAutoDiff = Fix64.Zero;

	protected int mSampleCount;

	protected FUnityObjectSample mCurrentSample = new FUnityObjectSample();

	protected FUnityObjectSample mPreviousSample = new FUnityObjectSample();

	protected FUnityObjectSample mDiff = new FUnityObjectSample();

	public string CurrentSampleString => mCurrentSample.ToString();

	public string PreviouSampleString => mPreviousSample.ToString();

	public string DiffSampleString => mDiff.ToString();

	public void CreateSample(string comment = null, bool sampleAnimClip = true, bool sampleGameObject = true, bool sampleMaterial = true, bool sampleMesh = true, bool sampleParticleSystem = true, bool sampleShader = true, bool sampleTexture2D = true, bool sampleTransform = true)
	{
		FUnityObjectSample fUnityObjectSample = mCurrentSample;
		mCurrentSample = mPreviousSample;
		mPreviousSample = fUnityObjectSample;
		mSampleCount++;
		FUnityObjectSample.StringBuilder.Length = 0;
		FUnityObjectSample.StringBuilder.AppendFormat("Sample #{0}", mSampleCount);
		if (!string.IsNullOrEmpty(comment))
		{
			FUnityObjectSample.StringBuilder.AppendFormat(" - {0}", comment);
		}
		// The shipped code always samples every object type regardless of the flag arguments.
		mCurrentSample.Sample(FUnityObjectSample.StringBuilder.ToString(), true, true, true, true, true, true, true, true);
	}

	public void DiffSamples()
	{
		mDiff = FUnityObjectSample.CreateDiff(mCurrentSample, mPreviousSample);
	}

	public void ClearSamples()
	{
		mCurrentSample = new FUnityObjectSample();
		mPreviousSample = new FUnityObjectSample();
		mDiff = new FUnityObjectSample();
	}

	public void UpdateAutoObjectDiff(Fix64 simDeltaTime)
	{
		if (FConfig.AutoObjectDiffInterval >= Fix64.One)
		{
			mTimeUntilNextAutoDiff -= simDeltaTime;
			if (mTimeUntilNextAutoDiff <= Fix64.Zero)
			{
				FApp.UnityObjectTracker.CreateSample("AutoSample", false, false, false, false, false, false, false, false);
				FLog.Log(FApp.UnityObjectTracker.mCurrentSample.ToString());
				FApp.UnityObjectTracker.DiffSamples();
				FLog.Log(FApp.UnityObjectTracker.mDiff.ToString());
				mTimeUntilNextAutoDiff = FConfig.AutoObjectDiffInterval;
			}
		}
	}
}
