using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Ploch.Common;
using Ploch.TestingSupport.RecordReplay;
using Xunit;
using Xunit.Sdk;

namespace Ploch.TestingSupport.Tests
{
    public class MyTestClass
    {
        public MyType1 MyTestType1Property { get; set; }

        public MyType2 MyTestType2Field;

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

        public MyType2 MyType2 { get; set; }
    }

    public class MyType2
    {
        public string StrProp { get; set; }

        public int IntProp { get; set; }

        public string PublicStringField = null;

    }

    public class OperationRecorderTests
    {
        private MyTestClass? _myTestClass;

        [Theory, AutoData]
        public void Record_should_write_all_parameters_to_json(MyTestClass sut, MyType1 param1, string param2)
        {
            sut.MyOp2(param1, param2);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText("MyTestClass.MyOp2-0.json"), new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto});
            dictionary["param1"].Should().BeEquivalentTo(param1);
            dictionary["param2"].Should().BeEquivalentTo(param2);
        }

        [Theory, AutoData]
        public void TheoryMethodName(MyTestClass testClass)
        {
            _myTestClass = testClass;
            var expression = Body(() => _myTestClass);
            var value = ExpressionUtilities.GetValue(expression);
            var valueWithoutCompiling = ExpressionUtilities.GetValueWithoutCompiling(expression);
            OperationRecorder.Record(this, tests => tests._myTestClass);

        }

        public static Expression Body<TParent>(TParent parent, Expression<Action<TParent>> operationExpression)
        {
            return operationExpression.Body;
        }

        public static Expression Body<TParent>(Expression<Func<TParent>> operationExpression)
        {
            return operationExpression.Body;
        }

        public static Expression Body<TParent, TResult>(TParent parent, Expression<Func<TParent, TResult>> operationExpression)
        {
            return operationExpression.Body;
        }
    }
}
