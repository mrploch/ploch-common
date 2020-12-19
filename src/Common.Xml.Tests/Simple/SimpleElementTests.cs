using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ploch.Common.Xml.Simple;
using Xunit;

namespace Ploch.Common.Xml.Tests.Simple
{
    public class SimpleElementTests
    {
        [Fact]
        public void Add_should_allow_adding_element_with_attributes_and_inner_elements()
        {
            var element = new SimpleElement("root_el");
            element.Add("elem1", "val_elem1", Attributes.Create("elem1_attr1", "val_elem1_attr1").Add("elem1_attr2", "val_elem1_attr2"));
            element.Add("elem2", "val_elem2", Attributes.Create("elem2_attr1", "val_elem2_attr1").Add("elem2_attr2", "val_elem2_attr2"),
                Elements.Create().Add("inner_elem1").Add("inner_elem2", "val_inner_elem2"));
            element.Should().ContainSingle(el => el.Name == "elem1");
            var elem1 = element["elem1"].First();
            elem1.Value.Should().Be("val_elem1");
            elem1.Attributes.Should().Contain("elem1_attr1", "val_elem1_attr1").And.Contain("elem1_attr2", "val_elem1_attr2");

            var elem2 = element["elem2"].First();
            elem2.Value.Should().Be("val_elem2");
            elem2.Attributes.Should().Contain("elem2_attr1", "val_elem2_attr1").And.Contain("elem2_attr2", "val_elem2_attr2");
            elem2.Should()
                 .ContainSingle(el => (el.Name == "inner_elem1") & (el.Value == null))
                 .And
                 .ContainSingle(el => (el.Name == "inner_elem2") & (el.Value == "val_inner_elem2"));
        }

        [Fact]
        public void Add_should_allow_adding_element_with_value()
        {
            var element = new SimpleElement("root_el");
            element.Add("elem1", "val_elem1");
            element.Add("elem2", "val_elem2");
            element.Should()
                   .Contain(el => (el.Name == "elem1") & (el.Value == "val_elem1"))
                   .And.Contain(el => (el.Name == "elem2") & (el.Value == "val_elem2"));
        }

        [Fact]
        public void Add_should_allows_adding_element_with_value()
        {
            var element = new SimpleElement("root_el");
            element.Add("elem1");
            element.Add("elem2");
            element.Should().Contain(el => (el.Name == "elem1") & (el.Value == null)).And.Contain(el => (el.Name == "elem2") & (el.Value == null));
        }

        [Fact]
        public void SimpleElement_should_allow_building_complex_structure()
        {
            var element = new SimpleElement("root_el")
            {
                {"elem1", "val_elem1_1"},
                {"elem1", "val_elem1_2"},
                {"elem2", "val_elem2"},
                "elem3",
                "elem4",
                {
                    "elem5", new Dictionary<string, string>
                    {
                        {"attr_elem5", "val_attr_elem5"}
                    }
                }
            };
            element.Name.Should().Be("root_el");
            element.Should().HaveCount(6);

            element["elem1"].Should().HaveCount(2);
            element["elem1"].Should().Contain(el => el.Value == "val_elem1_1").And.Contain(el => el.Value == "val_elem1_2");
        }
    }
}