using System;

// Rewritten from libil2cpp.so 1.0.605 (AbstractTweenProperty @ 0x016d2034..0x016d510c, Ghidra vaddr).
public abstract class AbstractTweenProperty
{
	protected bool _isInitialized;

	protected bool _isRelative;

	protected GoTween _ownerTween;

	protected Func<float, float, float, float, float> _easeFunction;

	public bool isInitialized => _isInitialized;

	public string PropertyName => ((IGenericProperty)this).propertyName;

	public AbstractTweenProperty(bool isRelative = false)
	{
		_isRelative = isRelative;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this is IGenericProperty && obj is IGenericProperty)
		{
			return ((IGenericProperty)this).propertyName == ((IGenericProperty)obj).propertyName;
		}
		if (obj.GetType() == GetType())
		{
			return true;
		}
		return base.Equals(obj);
	}

	public virtual void init(GoTween owner)
	{
		_isInitialized = true;
		_ownerTween = owner;
		if (_easeFunction == null)
		{
			_easeFunction = GoTweenUtils.easeFunctionForType(owner.easeType);
		}
	}

	public AbstractTweenProperty clone()
	{
		AbstractTweenProperty obj = MemberwiseClone() as AbstractTweenProperty;
		obj._ownerTween = null;
		obj._isInitialized = false;
		obj._easeFunction = null;
		return obj;
	}

	public void setEaseType(GoEaseType easeType)
	{
		_easeFunction = GoTweenUtils.easeFunctionForType(easeType);
	}

	public virtual bool validateTarget(object target)
	{
		return true;
	}

	public abstract void tick(float totalElapsedTime);

	public abstract void prepareForUse();
}
