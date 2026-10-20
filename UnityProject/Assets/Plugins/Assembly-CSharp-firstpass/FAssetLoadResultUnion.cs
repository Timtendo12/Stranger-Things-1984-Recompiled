using UnityEngine;

// PC port: no asset bundles exist; all assets come from Unity Resources. Bundle download/preload paths are no-ops (shipped: libil2cpp.so 1.0.605).
public class FAssetLoadResultUnion
{
	public AssetBundleCreateRequest mAssetBundleCreateRequest;

	public ResourceRequest mResourceRequest;

	public AssetBundleRequest mAssetBundleRequest;

	public Object mLoadResult;

	public object mOwner;

	public string GetStatusString()
	{
		return (mResourceRequest != null) ? (mResourceRequest.isDone ? "done" : "loading") : ((mLoadResult != null) ? "done" : "none");
	}
}
