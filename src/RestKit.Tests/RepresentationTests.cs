using System;
using System.Net;
using System.Xml;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestKit.TestLib;
using RestKit.Tests.Content;

namespace RestKit.Tests
{
    [TestClass]
    public class RepresentationTests
    {
        [TestMethod]
        public void HasNoContentIsTrueForStatus204()
        {
            var response = HttpStatusCode.NoContent.CreateResponseMessage("".CreateStream(), "application/json");
            var representation = new Representation(response);
            representation.HasNoContent.Should().BeTrue();
        }

        [TestMethod]
        public void HasNoContentIsTrueForEmptyContentStatus200()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("".CreateStream(), "application/json");
            var representation = new Representation(response);
            representation.HasNoContent.Should().BeTrue();
        }

        [TestMethod]
        public void DisposeWillDisposeUnderlyingMessage()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("{ \"test\":\"value\"}".CreateStream(), "application/json");
            var representation = new Representation(response);
            representation.Dispose();

            Action test = () => { var s = representation.Message.Content.ReadAsStreamAsync().Result; };
            test.ShouldThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void DeserializeThrowsInvalidOperationExceptionWhenCanSerializeIsFalse()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("{ \"test\":\"value\"}".CreateStream(), "application/json");
            var representation = new Representation(response);
            representation.CanDeserialize.Should().BeFalse();

            Action test = () => representation.Deserialize<SimpleItem>();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void RepresentationIdentifiesUnexpectedContent()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("{ \"test\":\"value\"}".CreateStream(), "application/json", "text/html");
            var representation = new Representation(response);
            representation.IsUnexpectedMediaType.Should().BeTrue();
        }

        [TestMethod]
        public void RepresentationIdentifiesExpectedContent()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("{ \"test\":\"value\"}".CreateStream(), "application/json", "application/json");
            var representation = new Representation(response);
            representation.IsUnexpectedMediaType.Should().BeFalse();
        }

        [TestMethod]
        public void JsonContentAsDynamicWorksWithFlatJsonAsIsCasing()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("{ \"test\":\"value\"}".CreateStream(), "application/json");
            var representation = new Representation(response);
            var json = representation.GetContentAsJson();

            string value = json.test;
            value.Should().Be("value");
        }

        [TestMethod]
        public void JsonContentAsDynamicWorksWithFlatJsonTitleCasing()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("{ \"test\":\"value\"}".CreateStream(), "application/json");
            var representation = new Representation(response);
            var json = representation.GetContentAsJson(CasingConvention.Pascalish);

            string value = json.Test;
            value.Should().Be("value");
        }

        [TestMethod]
        public void JsonContentAsDynamicWorksWithFlatJsonCamelCasing()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("{ \"Test\":\"value\"}".CreateStream(), "application/json");
            var representation = new Representation(response);
            var json = representation.GetContentAsJson(CasingConvention.Camelish);

            string value = json.test;
            value.Should().Be("value");
        }

        [TestMethod]
        public void JsonContentAsDynamicWorksWithJsonArray()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage("{ \"test\":[\"value1\",\"value2\",\"value3\"]}".CreateStream(), "application/json");
            var representation = new Representation(response);
            var json = representation.GetContentAsJson();

            var values = json.test;
            int count = values.Count;
            count.Should().Be(3);
        }

        [TestMethod]
        public void JsonContentAsDynamicWorksWithNestedObjects()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage(
                "{ \"test\": {\"prop1\":\"value1\",\"prop2\":\"value2\",\"prop3\":\"value3\"}}".CreateStream(),
                "application/json");
          
            var representation = new Representation(response);
            var json = representation.GetContentAsJson();

            var obj = json.test;

            string value1 = obj.prop1; 
            string value2 = obj.prop2; 
            string value3 = obj.prop3;

            value1.Should().Be("value1");
            value2.Should().Be("value2");
            value3.Should().Be("value3");
        }

        [TestMethod]
        public void JsonContentAsDynamicWorksWithNestedArrayOfObjects()
        {
            var response = HttpStatusCode.OK.CreateResponseMessage(
                "{ \"test\": [{\"prop1\":\"value1\",\"prop2\":\"value2\",\"prop3\":\"value3\"}, {\"prop1\":\"value1\",\"prop2\":\"value2\",\"prop3\":\"value3\"}]}".CreateStream(),
                "application/json");

            var representation = new Representation(response);
            var json = representation.GetContentAsJson();

            var jarray = json.test;
            int count = jarray.Count;

            count.Should().Be(2);

            string value1_1 = jarray[0].prop1;
            string value2_2 = jarray[1].prop2;

            value1_1.Should().Be("value1");
            value2_2.Should().Be("value2");
        }

        [TestMethod]
        public void XmlContentAsDynamicWorksWithSimpleElementsNestedElementsAndAttributes()
        {
            var xmlData =
            @"<root attr1='rootAttribute'>
                <aNode>
                    <Child value='attributeValue1'>elementValue1</Child>
                    <Child value='attributeValue2'>elementValue2</Child>
                    <Child value='attributeValue3'>elementValue3</Child>
                </aNode>
                <anotherNode>simple</anotherNode>
            </root>";

            var response = HttpStatusCode.OK.CreateResponseMessage(xmlData.CreateStream(), "text/xml");
            var representation = new Representation(response);
            var root = representation.GetContentAsXml();

            string attr1 = root.Attributes.attr1;
            attr1.Should().Be("rootAttribute");

            string child1 = root.aNode.Child[0].Value;
            child1.Should().Be("elementValue1");

            string child2 = root.aNode.Child[1].Value;
            child2.Should().Be("elementValue2");

            string child3 = root.aNode.Child[2].Value;
            child3.Should().Be("elementValue3");

            string value3 = root.aNode.Child[2].Attributes.value;
            value3.Should().Be("attributeValue3");

            string simple = root.anotherNode.Value;
            simple.Should().Be("simple");
        }

        [TestMethod]
        public void XmlContentAsDynamicWorksWithMixedComplexContent()
        {
            var xmlData =
            @"<root attr1='rootAttribute'>
                <aNode>
                    <Child value='attributeValue1'>elementValue1</Child>
                    <Child value='attributeValue2'>elementValue2</Child>
                    <Child value='attributeValue3'>elementValue3</Child>
                    <BadSeed>Garg</BadSeed>
                    <BadSeed>Blarg</BadSeed>
                    <Child>elementValue4</Child>
                </aNode>
                <anotherNode>simple</anotherNode>
            </root>";

            var response = HttpStatusCode.OK.CreateResponseMessage(xmlData.CreateStream(), "text/xml");
            var representation = new Representation(response);
            var root = representation.GetContentAsXml();

            string attr1 = root.Attributes.attr1;
            attr1.Should().Be("rootAttribute");

            string child1 = root.aNode.Child[0].Value;
            child1.Should().Be("elementValue1");

            string child2 = root.aNode.Child[1].Value;
            child2.Should().Be("elementValue2");

            string child3 = root.aNode.Child[2].Value;
            child3.Should().Be("elementValue3");

            string value3 = root.aNode.Child[2].Attributes.value;
            value3.Should().Be("attributeValue3");

            string value4 = root.aNode.Child[3].Value;
            value4.Should().Be("elementValue4");

            string bad1 = root.aNode.BadSeed[0].Value;
            bad1.Should().Be("Garg");

            string bad2 = root.aNode.BadSeed[1].Value;
            bad2.Should().Be("Blarg");

            string simple = root.anotherNode.Value;
            simple.Should().Be("simple");
        }

        [TestMethod]
        public void XmlContentAsDynamicWorksWithSimpleRootNodeContent()
        {
            var xmlData =
            @"<root>simple</root>";

            var response = HttpStatusCode.OK.CreateResponseMessage(xmlData.CreateStream(), "text/xml");
            var representation = new Representation(response);
            var root = representation.GetContentAsXml();

            string simple = root.Value;
            simple.Should().Be("simple");
        }

        [TestMethod]
        public void XmlContentAsDynamicHonorsConventions()
        {
            var xmlData =
             @"<root attr1='rootAttribute'>
                <aNode>
                    <Child value='attributeValue1'>elementValue1</Child>
                    <Child value='attributeValue2'>elementValue2</Child>
                    <Child value='attributeValue3'>elementValue3</Child>
                </aNode>
                <anotherNode>simple</anotherNode>
            </root>";

            var response = HttpStatusCode.OK.CreateResponseMessage(xmlData.CreateStream(), "text/xml");
            var representation = new Representation(response);
            var conventions = new XmlConventions
            {
                AttributeContainerName = "Attrs",
                Casing = CasingConvention.Pascalish,
                ElementValueName = "Val",
            };

            var root = representation.GetContentAsXml(conventions);

            string attr1 = root.Attrs.Attr1;
            attr1.Should().Be("rootAttribute");

            string child1 = root.ANode.Child[0].Val;
            child1.Should().Be("elementValue1");

            string child2 = root.ANode.Child[1].Val;
            child2.Should().Be("elementValue2");

            string child3 = root.ANode.Child[2].Val;
            child3.Should().Be("elementValue3");

            string value3 = root.ANode.Child[2].Attrs.Value;
            value3.Should().Be("attributeValue3");

            string simple = root.AnotherNode.Val;
            simple.Should().Be("simple");
        }

        [TestMethod]
        public void XmlReaderHasExpectedNodes()
        {
            var xmlData =
             @"<root attr1='rootAttribute'>
                <aNode>
                    <Child value='attributeValue1'>elementValue1</Child>
                    <Child value='attributeValue2'>elementValue2</Child>
                    <Child value='attributeValue3'>elementValue3</Child>
                </aNode>
                <anotherNode>simple</anotherNode>
            </root>";

            var response = HttpStatusCode.OK.CreateResponseMessage(xmlData.CreateStream(), "text/xml");
            var representation = new Representation(response);
            var reader = representation.GetContentAsXmlReader();
            reader.MoveToContent().Should().Be(XmlNodeType.Element);
            // Trust the XmlReader works from here...
        }

        [TestMethod]
        public void XDocumentHasExpectedNodes()
        {
            var xmlData =
             @"<root attr1='rootAttribute'>
                <aNode>
                    <Child value='attributeValue1'>elementValue1</Child>
                    <Child value='attributeValue2'>elementValue2</Child>
                    <Child value='attributeValue3'>elementValue3</Child>
                </aNode>
                <anotherNode>simple</anotherNode>
            </root>";

            var response = HttpStatusCode.OK.CreateResponseMessage(xmlData.CreateStream(), "text/xml");
            var representation = new Representation(response);
            var xdoc = representation.GetContentAsXDocument();
            xdoc.Element("root").Should().NotBeNull();
            // trust XDocument works from here...
        }
    }
}
