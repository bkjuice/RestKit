namespace RestKit
{
    public class XmlConventions
    {
        public static readonly XmlConventions Default = new XmlConventions()
        {
            AttributeContainerName = "Attributes",
            ElementValueName = "Value",
            Casing = CasingConvention.AsIs
        };

        public string AttributeContainerName { get; set; }

        public string ElementValueName { get; set; }

        public CasingConvention Casing { get; set; }
    }
}
