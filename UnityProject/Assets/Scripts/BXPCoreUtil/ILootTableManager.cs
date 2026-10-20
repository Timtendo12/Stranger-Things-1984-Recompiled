using System.Xml;

public interface ILootTableManager<ContextType, ResultType> : IRequirementManager<ContextType> where ResultType : class
{
	FLootNode<ContextType, ResultType> GetRootNode(string name);

	FLootNode<ContextType, ResultType> AllocateNodeForXmlElement(XmlElement xmlNode);

	ResultType GetResultForXmlElement(XmlElement xmlNode);
}
