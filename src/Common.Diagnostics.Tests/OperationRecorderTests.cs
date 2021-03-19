
using System.Collections.Generic;
using System.IO;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Ploch.TestingSupport.RecordReplay;
using Xunit;

namespace Ploch.TestingSupport.Tests
{
    public class MyTestClass
    {
        public void MyOp1(MyType1 param1, string param2)
        {
            OperationRecorder.Record(this, c => c.MyOp1(param1, param2));
        }

        public string MyOp2(MyType1 param1, string param2)
        {
            OperationRecorder.Record(this, c => c.MyOp2(param1, param2));
            return "test";
        }
    }

    public class MyType1
    {
        public string StrProp { get; set; }

        public int IntProp { get; set; }


    }
    
    public class OperationRecorderTests
    {
        [Theory, AutoData]
        public void Record_should_write_all_parameters_to_json(MyTestClass sut, MyType1 param1, string param2)
        {
            sut.MyOp2(param1, param2);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText("MyTestClass.MyOp2-0.json"), new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto});
            dictionary["param1"].Should().BeEquivalentTo(param1);
            dictionary["param2"].Should().BeEquivalentTo(param2);
        }

       
    }
}