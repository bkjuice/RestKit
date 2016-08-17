namespace RestKit
{
    public class XmlConventions
    {
        public static readonly XmlConventions Default = new XmlConventions()
        {
            GroupSuffix = "Group",
            AttributeContainerName = "Attributes",
            ComplexElementValueName = "Value",
            Casing = CasingConvention.AsIs
        };

        public string GroupSuffix { get; set; }

        public string AttributeContainerName { get; set; }

        public string ComplexElementValueName { get; set; }

        public CasingConvention Casing { get; set; }
    }
}
