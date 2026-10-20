public interface IEntity
{
	void AddAction(FEntityAction action);

	void RemoveAction(FEntityAction action);

	float PlayAnim(string name, bool recursive);

	float GetAnimLength(string name, bool recursive);

	FAnimState PlayAnimType(int animType, float animSpeed, bool recursive, IAnimTagResponder responder);
}
