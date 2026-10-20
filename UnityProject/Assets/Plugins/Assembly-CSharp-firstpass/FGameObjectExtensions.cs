using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FGameObjectExtensions @ 0x164bee8..0x1658b00, Ghidra vaddr).
public static class FGameObjectExtensions
{
	public const int cParticleLifetimeMaxCount = 500;

	public static ParticleSystem.Particle[] sParticleTemp;

	public const float cLDRCutoff = 2f;

	public static GameObject GetParentGameObject(this GameObject obj)
	{
		if (obj.transform.parent == null)
		{
			return null;
		}
		return obj.transform.parent.gameObject;
	}

	public static bool HasAncestorStrict(this GameObject obj, GameObject ancestor)
	{
		do
		{
			obj = obj.GetParentGameObject();
		}
		while (obj != null && obj != ancestor);
		return obj != null;
	}

	public static bool HasAncestor(this GameObject obj, GameObject ancestor)
	{
		while (obj != null && obj != ancestor)
		{
			obj = obj.GetParentGameObject();
		}
		return obj != null;
	}

	public static void PlayParticleSystemsAndTrailsRecursive(this GameObject obj)
	{
		ParticleSystem component = obj.GetComponent<ParticleSystem>();
		if (component != null)
		{
			component.Simulate(0f, true);
			component.Play(true);
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.PlayParticleSystemsAndTrailsRecursive();
		}
	}

	public static void StopParticleSystemsAndTrailsRecursive(this GameObject obj, float capParticleLifetime = -1f)
	{
		ParticleSystem component = obj.GetComponent<ParticleSystem>();
		if (component != null)
		{
			component.Stop();
			if (capParticleLifetime >= 0f && component.particleCount > 0)
			{
				if (sParticleTemp == null || sParticleTemp.Length < 500)
				{
					sParticleTemp = new ParticleSystem.Particle[500];
				}
				int particles = component.GetParticles(sParticleTemp);
				for (int i = 0; i < particles; i++)
				{
					if (capParticleLifetime < sParticleTemp[i].remainingLifetime)
					{
						sParticleTemp[i].remainingLifetime = capParticleLifetime;
					}
				}
				component.SetParticles(sParticleTemp, particles);
			}
		}
		for (int j = 0; j < obj.transform.childCount; j++)
		{
			obj.transform.GetChild(j).gameObject.StopParticleSystemsAndTrailsRecursive(capParticleLifetime);
		}
	}

	public static void ResetParticleSystemsRecursive(this GameObject obj, bool includeStopped)
	{
		if (obj == null)
		{
			return;
		}
		ParticleSystem component = obj.GetComponent<ParticleSystem>();
		if (component != null && (includeStopped || component.isPlaying))
		{
			component.Stop();
			component.Clear();
			component.Play(true);
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.ResetParticleSystemsRecursive(includeStopped);
		}
	}

	public static void StopLightAnimationsRecursive(this GameObject obj)
	{
		if (obj.GetComponent<Light>() != null)
		{
			Animation component = obj.GetComponent<Animation>();
			if (component != null)
			{
				component.Stop();
			}
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.StopLightAnimationsRecursive();
		}
	}

	public static void FadeLightRecursive(this GameObject obj, float ratio)
	{
		Light component = obj.GetComponent<Light>();
		if (component != null)
		{
			component.intensity *= ratio;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.FadeLightRecursive(ratio);
		}
	}

	public static bool HasAnimEnded(this GameObject obj, string animName, float endTolerance = 0.01f)
	{
		Animation component = obj.GetComponent<Animation>();
		if (component != null)
		{
			AnimationState animationState = component[animName];
			if (animationState != null && animationState.enabled)
			{
				return 1f <= animationState.normalizedTime + endTolerance;
			}
		}
		return true;
	}

	public static float PlayAnimRecursive(this GameObject gameObj, string animName, float speed = 1f, bool crossfade = true, float startAnimTimeNormalized = -1f)
	{
		float num = gameObj.PlayAnim(animName, speed, crossfade, startAnimTimeNormalized);
		for (int i = 0; i < gameObj.transform.childCount; i++)
		{
			float num2 = gameObj.transform.GetChild(i).gameObject.PlayAnimRecursive(animName, speed, crossfade, startAnimTimeNormalized);
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public static float PlayAnim(this GameObject gameObj, string animName, float speed = 1f, bool crossfade = true, float startAnimTimeNormalized = -1f)
	{
		float num = 0f;
		if (gameObj != null && animName != null && animName.Length > 0)
		{
			Animation componentInChildren = gameObj.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				AnimationState animationState = componentInChildren[animName];
				if (animationState != null)
				{
					if (animationState.enabled)
					{
						animationState.time = 0f;
					}
					if (startAnimTimeNormalized >= 0f)
					{
						animationState.normalizedTime = startAnimTimeNormalized;
					}
					num = animationState.length;
					if (!crossfade || !componentInChildren.isPlaying)
					{
						componentInChildren.clip = componentInChildren.GetClip(animName);
						componentInChildren.Play();
					}
					else
					{
						componentInChildren.CrossFade(animName, Mathf.Min(animationState.length * 0.5f / speed, 0.2f));
					}
					num /= speed;
					if (Mathf.Abs(speed - animationState.speed) > FMath.Epsilon)
					{
						animationState.speed = speed;
					}
				}
			}
		}
		return num;
	}

	public static float GetAnimLength(this GameObject gameObj, string animName, float speed = 1f)
	{
		float result = 0f;
		if (gameObj != null && animName != null && animName.Length > 0)
		{
			Animation componentInChildren = gameObj.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				AnimationState animationState = componentInChildren[animName];
				if (animationState != null)
				{
					result = animationState.length / speed;
				}
			}
		}
		return result;
	}

	public static float GetAnimClipLength(this GameObject gameObj, string animName, float speed = 1f)
	{
		float result = 0f;
		if (gameObj != null && animName != null && animName.Length > 0)
		{
			Animation componentInChildren = gameObj.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				AnimationState animationState = componentInChildren[animName];
				if (animationState != null)
				{
					result = animationState.clip.length / speed;
				}
			}
		}
		return result;
	}

	public static void SetAnimSpeed(this GameObject gameObj, float speed)
	{
		gameObj.ApplyFunctionRecursive(delegate(GameObject obj)
		{
			Animation component = obj.GetComponent<Animation>();
			if (component != null)
			{
				foreach (AnimationState item in component)
				{
					item.speed = speed;
				}
			}
			FAnimateUVOffset component2 = obj.GetComponent<FAnimateUVOffset>();
			if (component2 != null)
			{
				component2.AnimSpeed = speed;
			}
			ParticleSystem component3 = obj.GetComponent<ParticleSystem>();
			if (component3 != null)
			{
				ParticleSystem.MainModule main = component3.main;
				main.simulationSpeed = speed;
			}
		});
	}

	public static void SetAnimTimeAndSpeed(this GameObject gameObj, float time, float speed)
	{
		gameObj.ApplyFunctionRecursive(delegate(GameObject obj)
		{
			Animation component = obj.GetComponent<Animation>();
			if (component != null)
			{
				foreach (AnimationState item in component)
				{
					item.speed = speed;
					item.time = time;
				}
			}
			FAnimateUVOffset component2 = obj.GetComponent<FAnimateUVOffset>();
			if (component2 != null)
			{
				component2.AnimSpeed = speed;
			}
			ParticleSystem component3 = obj.GetComponent<ParticleSystem>();
			if (component3 != null)
			{
				ParticleSystem.MainModule main = component3.main;
				main.simulationSpeed = speed;
			}
		});
	}

	public static void SetAnimNormalizedTime(this GameObject gameObj, string animName, float normalizedAnimTime)
	{
		if (!(gameObj == null) && animName != null && animName.Length > 0)
		{
			gameObj.ApplyFunctionRecursive(delegate(GameObject obj)
			{
				Animation component = obj.GetComponent<Animation>();
				if (!(component == null))
				{
					AnimationState animationState = component[animName];
					if (animationState != null)
					{
						animationState.normalizedTime = normalizedAnimTime;
					}
				}
			});
		}
	}

	public static void PrimeAnimation(this GameObject gameObj)
	{
		Animation[] components = gameObj.GetComponents<Animation>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Sample();
		}
	}

	public static GameObject FindFirstChild(this GameObject parent, string name)
	{
		Transform transform = parent.transform.Find(name);
		if (transform == null)
		{
			return null;
		}
		return transform.gameObject;
	}

	public static GameObject FindFirstRecursive(this GameObject parent, string name)
	{
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			GameObject gameObject = parent.transform.GetChild(i).gameObject;
			if (gameObject.name == name)
			{
				return gameObject;
			}
			GameObject gameObject2 = gameObject.FindFirstRecursive(name);
			if (gameObject2 != null)
			{
				return gameObject2;
			}
		}
		return null;
	}

	public static GameObject FindFirstRecursiveMatchingPrefab(this GameObject parent, string name, GameObject prefab)
	{
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			GameObject gameObject = parent.transform.GetChild(i).gameObject;
			GameObject gameObject2 = prefab.FindFirstChild(gameObject.name);
			if (!(gameObject2 == null))
			{
				if (gameObject.name == name)
				{
					return gameObject;
				}
				GameObject gameObject3 = gameObject.FindFirstRecursiveMatchingPrefab(name, gameObject2);
				if (gameObject3 != null)
				{
					return gameObject3;
				}
			}
		}
		return null;
	}

	public static void ResetMaterialsRecursive(this GameObject obj, GameObject prefab)
	{
		// TODO(port): restores the shared materials from the prefab's matching renderer (0x1653f08); matched by child name here [left: 3D character / UI-preview pipeline, never reached by the 2D game]
		Renderer component = obj.GetComponent<Renderer>();
		Renderer renderer = ((prefab != null) ? prefab.GetComponent<Renderer>() : null);
		if (component != null && renderer != null)
		{
			component.sharedMaterials = renderer.sharedMaterials;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			GameObject gameObject = obj.transform.GetChild(i).gameObject;
			GameObject gameObject2 = ((prefab != null) ? prefab.FindFirstChild(gameObject.name) : null);
			gameObject.ResetMaterialsRecursive(gameObject2);
		}
	}

	public static void SetMaterialRenderQueueRecursive(this GameObject obj, int renderQueue, int newLayer = -1)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetMaterialRenderQueueRecursive(renderQueue, newLayer);
		}
		if (newLayer >= 0)
		{
			obj.layer = newLayer;
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component != null)
		{
			Material[] materials = component.materials;
			for (int j = 0; j < materials.Length; j++)
			{
				obj.TrackMaterialForCleanup(materials[j]);
				materials[j].renderQueue = renderQueue;
			}
		}
	}

	public static void SetMaterialMiscRecursive(this GameObject obj, Color color, Color tintColor, int newRenderQueue = -1, int newLayer = -1)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetMaterialMiscRecursive(color, tintColor, newRenderQueue, newLayer);
		}
		if (newLayer >= 0)
		{
			obj.layer = newLayer;
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component != null)
		{
			Material[] materials = component.materials;
			for (int j = 0; j < materials.Length; j++)
			{
				obj.TrackMaterialForCleanup(materials[j]);
				if (newRenderQueue >= 0)
				{
					materials[j].renderQueue = newRenderQueue;
				}
				if (materials[j].HasProperty("_Color"))
				{
					materials[j].color = color;
				}
				if (materials[j].HasProperty("_TintColor"))
				{
					materials[j].SetColor("_TintColor", tintColor);
				}
			}
		}
	}

	public static void SetMaterialTintColorRecursive(this GameObject obj, Color tintColor)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetMaterialTintColorRecursive(tintColor);
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component != null)
		{
			Material[] materials = component.materials;
			for (int j = 0; j < materials.Length; j++)
			{
				obj.TrackMaterialForCleanup(materials[j]);
				if (materials[j].HasProperty("_TintColor"))
				{
					materials[j].SetColor("_TintColor", tintColor);
				}
			}
		}
	}

	public static void ReplaceTextureRecursive(this GameObject obj, string oldTexture, string newTexture)
	{
		obj.ReplaceTextureRecursive(oldTexture, Resources.Load<Texture>(newTexture));
	}

	public static void ReplaceTextureRecursive(this GameObject obj, string oldTexture, Texture newTexture)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.ReplaceTextureRecursive(oldTexture, newTexture);
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component == null)
		{
			return;
		}
		Material[] materials = component.materials;
		for (int j = 0; j < materials.Length; j++)
		{
			Texture mainTexture = materials[j].mainTexture;
			if (mainTexture != null && mainTexture.name == oldTexture)
			{
				obj.TrackMaterialForCleanup(materials[j]);
				materials[j].mainTexture = newTexture;
			}
		}
	}

	public static void ReplaceMaterialRecursive(this GameObject obj, string oldMaterial, string newMaterial)
	{
		obj.ReplaceMaterialRecursive(oldMaterial, Resources.Load<Material>(newMaterial));
	}

	public static void ReplaceMaterialRecursive(this GameObject obj, string oldMaterial, Material newMaterial)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.ReplaceMaterialRecursive(oldMaterial, newMaterial);
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component == null)
		{
			return;
		}
		Material[] sharedMaterials = component.sharedMaterials;
		bool flag = false;
		for (int j = 0; j < sharedMaterials.Length; j++)
		{
			if (sharedMaterials[j] != null && sharedMaterials[j].name == oldMaterial)
			{
				sharedMaterials[j] = newMaterial;
				flag = true;
			}
		}
		if (flag)
		{
			component.sharedMaterials = sharedMaterials;
		}
	}

	public static void AddAttachment(this GameObject obj, GameObject attachment)
	{
		obj.transform.AddAttachment(attachment.transform);
	}

	public static void AddAttachment(this GameObject obj, Transform attachment)
	{
		obj.transform.AddAttachment(attachment);
	}

	public static void AddSkinnedAttachment(this GameObject obj, GameObject attachmentInstance, string attachToMeshName = null, string remapTagName = null, List<string> destinationBoneOverrides = null)
	{
		// TODO(port): the skinned-mesh bone remap (FSkeletonRemap, 0x1655e04) belongs to the 3D character pipeline that the 2D game never reaches [left: 3D character / UI-preview pipeline, never reached by the 2D game]
		obj.AddAttachment(attachmentInstance);
	}

	public static GameObject AddAttachment(this GameObject obj, string attachToChildName, GameObject attachment)
	{
		GameObject gameObject = ((attachToChildName != null) ? obj.FindFirstRecursive(attachToChildName) : obj);
		if (gameObject == null)
		{
			return null;
		}
		gameObject.AddAttachment(attachment);
		return attachment;
	}

	public static GameObject AddAttachment(this GameObject obj, string attachToChildName, string attachmentPrefabName, GameObject basePrefab = null, bool usePrefabPosition = false)
	{
		GameObject gameObject = FUtil.LoadPrefab(attachmentPrefabName);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = (usePrefabPosition ? gameObject.Clone(gameObject.transform.position) : gameObject.Clone());
		if (gameObject2 == null)
		{
			return null;
		}
		GameObject gameObject3 = ((attachToChildName == null) ? obj : ((basePrefab != null) ? obj.FindFirstRecursiveMatchingPrefab(attachToChildName, basePrefab) : obj.FindFirstRecursive(attachToChildName)));
		if (gameObject3 == null)
		{
			UnityEngine.Object.Destroy(gameObject2);
			return null;
		}
		gameObject3.AddAttachment(gameObject2);
		return gameObject2;
	}

	public static GameObject AddSkinnedAttachment(this GameObject obj, string attachmentPrefabName, string attachToMeshName = null, string remapTagName = null, List<string> destinationBones = null)
	{
		// TODO(port): see AddSkinnedAttachment above (0x1655204) [left: 3D character / UI-preview pipeline, never reached by the 2D game]
		GameObject gameObject = FUtil.LoadPrefab(attachmentPrefabName);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = gameObject.Clone();
		obj.AddSkinnedAttachment(gameObject2, attachToMeshName, remapTagName, destinationBones);
		return gameObject2;
	}

	public static GameObject Clone(this GameObject obj, Vector3 position = default(Vector3))
	{
		return UnityEngine.Object.Instantiate(obj, position, obj.transform.rotation);
	}

	public static GameObject Clone(this GameObject obj, Vector3 position, Quaternion rotation)
	{
		return UnityEngine.Object.Instantiate(obj, position, rotation * obj.transform.rotation);
	}

	public static T GetComponentInChildrenWithInactive<T>(this GameObject parent, string name = null) where T : Component
	{
		return null;
	}

	public static void SetAlphaRecursive(this GameObject obj, float alpha, bool affectParticlesOrTrails = true, bool affectProjectors = false)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetAlphaRecursive(alpha, affectParticlesOrTrails, affectProjectors);
		}
		if (affectProjectors)
		{
			Projector component = obj.GetComponent<Projector>();
			if (component != null)
			{
				Material material;
				if (obj.GetComponent<FProjectorMaterialIsUnique>() == null)
				{
					obj.AddComponent<FProjectorMaterialIsUnique>();
					material = new Material(component.material);
					component.material = material;
					obj.TrackMaterialForCleanup(material);
				}
				else
				{
					material = component.material;
				}
				if (material.HasProperty("_Color"))
				{
					Color color = material.color;
					color.a = alpha;
					material.color = color;
				}
			}
		}
		Renderer component2 = obj.GetComponent<Renderer>();
		if (component2 == null)
		{
			return;
		}
		if (!affectParticlesOrTrails && (obj.GetComponent<ParticleSystem>() != null || obj.GetComponent<TrailRenderer>() != null))
		{
			return;
		}
		Material[] materials = component2.materials;
		for (int j = 0; j < materials.Length; j++)
		{
			obj.TrackMaterialForCleanup(materials[j]);
			if (materials[j].HasProperty("_Color"))
			{
				Color color2 = materials[j].color;
				color2.a = alpha;
				materials[j].color = color2;
			}
		}
	}

	public static void SetColorRecursive(this GameObject obj, Color color)
	{
		obj.SetMaterialColorRecursive("_Color", color);
	}

	public static void SetMaterialColorRecursive(this GameObject obj, string name, Color color)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetMaterialColorRecursive(name, color);
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component == null)
		{
			return;
		}
		Material[] materials = component.materials;
		for (int j = 0; j < materials.Length; j++)
		{
			obj.TrackMaterialForCleanup(materials[j]);
			if (materials[j].HasProperty(name))
			{
				materials[j].SetColor(name, color);
			}
		}
	}

	public static void SetColorXformRecursive(this GameObject obj, Color xform1, Color xform2, Color xform3)
	{
		obj.SetMaterialColorRecursive("_Xform1", xform1);
		obj.SetMaterialColorRecursive("_Xform2", xform2);
		obj.SetMaterialColorRecursive("_Xform3", xform3);
	}

	public static Color? GetMaterialColorRecursive(this GameObject obj, string name, bool useProjectors = false)
	{
		if (useProjectors)
		{
			Projector component = obj.GetComponent<Projector>();
			if (component != null && component.material != null && component.material.HasProperty(name))
			{
				return component.material.GetColor(name);
			}
		}
		Renderer component2 = obj.GetComponent<Renderer>();
		if (component2 != null && component2.sharedMaterial != null && component2.sharedMaterial.HasProperty(name))
		{
			return component2.sharedMaterial.GetColor(name);
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			Color? materialColorRecursive = obj.transform.GetChild(i).gameObject.GetMaterialColorRecursive(name, useProjectors);
			if (materialColorRecursive.HasValue)
			{
				return materialColorRecursive;
			}
		}
		return null;
	}

	public static void AddCleanupObject(this GameObject obj, UnityEngine.Object cleanupObj)
	{
		FCleanupHelper fCleanupHelper = obj.GetComponent<FCleanupHelper>();
		if (fCleanupHelper == null)
		{
			fCleanupHelper = obj.AddComponent<FCleanupHelper>();
		}
		fCleanupHelper.AddCleanupObject(cleanupObj);
	}

	public static void SetRendererEnabledRecursive(this GameObject obj, bool enable)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetRendererEnabledRecursive(enable);
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component != null)
		{
			component.enabled = enable;
		}
	}

	public static void GetObjectsAndMaterialsRecursive(this GameObject obj, ref List<GameObject> outObjects, ref List<Material> outMaterials)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.GetObjectsAndMaterialsRecursive(ref outObjects, ref outMaterials);
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component != null)
		{
			outObjects.Add(obj);
			outMaterials.AddRange(component.materials);
		}
	}

	public static void SetMaterialMiscRecursive(this GameObject obj, int renderQueue, Color color, Color tintColor, int newLayer = -1)
	{
		obj.SetMaterialMiscRecursive(color, tintColor, renderQueue, newLayer);
	}

	public static void ApplyFunctionRecursive(this GameObject obj, Action<GameObject> function)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.ApplyFunctionRecursive(function);
		}
		function(obj);
	}

	public static void SetLayerRecursive(this GameObject obj, int layer)
	{
		obj.layer = layer;
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetLayerRecursive(layer);
		}
	}

	public static Transform FindBoneRecursive(this GameObject obj, string name)
	{
		return FUtil.FindBoneRecursive(name, obj.transform);
	}

	public static void SetupForUILighting(this GameObject root)
	{
		// TODO(port): configures light-probe / shadow flags on the renderers for the UI 3D preview camera (0x1656a40) [left: 3D character / UI-preview pipeline, never reached by the 2D game]
		Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			componentsInChildren[i].receiveShadows = false;
		}
	}

	public static string GetPathToGameObject(this GameObject gameObject, GameObject root)
	{
		if (gameObject == root)
		{
			return gameObject.name;
		}
		System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(gameObject.name);
		for (Transform parent = gameObject.transform.parent; parent != null; parent = parent.parent)
		{
			stringBuilder.Insert(0, "/");
			stringBuilder.Insert(0, parent.gameObject.name);
			if (parent.gameObject == root)
			{
				break;
			}
		}
		return stringBuilder.ToString();
	}

	public static void IgnoreCollisionRecursive(this GameObject gameObject, GameObject ignoreObject)
	{
		Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = ignoreObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Physics.IgnoreCollision(componentsInChildren[i], componentsInChildren2[j]);
			}
		}
	}

	public static AnimationState GetHighestWeightedAnimationState(this GameObject gameObject)
	{
		Animation componentInChildren = gameObject.GetComponentInChildren<Animation>();
		if (componentInChildren == null)
		{
			return null;
		}
		float num = float.MinValue;
		AnimationState result = null;
		foreach (AnimationState item in componentInChildren)
		{
			if (item.enabled && num < item.weight)
			{
				num = item.weight;
				result = item;
			}
		}
		return result;
	}

	public static float GetHighestWeightedAnimationTime(this GameObject gameObject)
	{
		AnimationState highestWeightedAnimationState = gameObject.GetHighestWeightedAnimationState();
		if (highestWeightedAnimationState == null)
		{
			return 0f;
		}
		return highestWeightedAnimationState.time;
	}

	public static float GetHighestWeightedAnimationTimeNormalized(this GameObject gameObject)
	{
		AnimationState highestWeightedAnimationState = gameObject.GetHighestWeightedAnimationState();
		if (highestWeightedAnimationState == null)
		{
			return 0f;
		}
		return highestWeightedAnimationState.normalizedTime;
	}

	public static void HackSetLDREmissionRecursive(this GameObject obj)
	{
		// TODO(port): clamps the emission color to the LDR cutoff (cLDRCutoff = 2, 0x1657150) [left: 3D character / UI-preview pipeline, never reached by the 2D game]
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.HackSetLDREmissionRecursive();
		}
		Renderer component = obj.GetComponent<Renderer>();
		if (component == null)
		{
			return;
		}
		Material[] materials = component.materials;
		for (int j = 0; j < materials.Length; j++)
		{
			if (materials[j].HasProperty("_EmissionColor"))
			{
				Color color = materials[j].GetColor("_EmissionColor");
				float num = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
				if (num > 2f)
				{
					color *= 2f / num;
					materials[j].SetColor("_EmissionColor", color);
				}
			}
		}
	}

	public static bool IsFlippedHolderObject(this GameObject obj)
	{
		return obj != null && obj.name != null && obj.name.EndsWith("holder");
	}
}
