using System.Xml;

public interface IRequirementManager<ContextType>
{
	FRequirement<ContextType> AllocateRequirementForXmlElement(XmlElement element);
}
