using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Ploch.Common;

namespace Ploch.TestingSupport.RecordReplay
{
    public static class OperationRecorder
    {
        private static int Counter = 0;

        private static readonly JsonSerializer Serializer =
            JsonSerializer.CreateDefault(new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented});

        public static void Record<TParent>(TParent parent, Expression<Action<TParent>> operationExpression)
        {
            var operationName = operationExpression.GetMemberName();
            Record(parent, operationExpression, operationName);
        }

        public static void Record<TParent, TResult>(TParent parent, Expression<Func<TParent, TResult>> operationExpression)
        {
            var operationName = operationExpression.GetMemberName();
            Record(parent, operationExpression, operationName);
        }

        private static void Record<TParent>(TParent parent, LambdaExpression operationExpression, string operationName)
        {
            var parameters = new Dictionary<string, object>();
            var number = Counter++;
            if (operationExpression.Body is MethodCallExpression methodExpr)
            {
                foreach (var expression in methodExpr.Arguments)
                {
                    var argMemberExpression = (MemberExpression)expression;
                    RecordMember(parameters, argMemberExpression);
                }
            }

            if (operationExpression.Body is MemberExpression memberExpression)
            {
                RecordMember(parameters, memberExpression);
            }
            //var expressionBody = operationExpression.Body as MemberExpression;


            using (var writer = new JsonTextWriter(new StreamWriter($"{typeof(TParent).Name}.{operationName}-{number}.json")))
            {
                Serializer.Serialize(writer, parameters);
            }
        }

        private static void RecordMember(IDictionary<string, object> parameters, MemberExpression expression)
        {
            var nodeType = expression.NodeType;
            var innerExpr = expression.Expression;
            var innerExpressionNodeType = innerExpr.NodeType;
            if (innerExpr is ParameterExpression parameterExpr)
            {
                // var outerProp = (PropertyInfo)expression.Member;

                var outerProp = (FieldInfo)expression.Member;
                MemberExpression innerMember = (MemberExpression)expression.Expression;
                FieldInfo innerField = (FieldInfo)innerMember.Member;
                ConstantExpression ce = (ConstantExpression)innerMember.Expression;
                object innerObj = ce.Value;
                object outerObj = innerField.GetValue(innerObj);

                string value1 = (string)outerProp.GetValue(outerObj);
                // string value1 = (string)outerProp.GetValue(outerObj, null);
            }

            var constantExpression = (ConstantExpression)innerExpr;
            var parametersValue = constantExpression.Value;
            var f = (FieldInfo)expression.Member;
            var value = f.GetValue(parametersValue);
            parameters.Add(f.Name, value);
        }

    }
}