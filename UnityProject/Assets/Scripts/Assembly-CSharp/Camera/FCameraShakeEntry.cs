using UnityEngine;

public abstract class FCameraShakeEntry
{
	public abstract bool Update(Camera camera, float deltaTime, ref Vector3 position);
}
