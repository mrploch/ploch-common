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
            JsonSerializer.CreateDefault(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented });
        public static void Record<TParent>(this TParent parent, Expression<Action<TParent>> operationExpression)
        {
            var operationName = operationExpression.GetMemberName();
            Record(parent, operationExpression, operationName);
        }
        public static void Record<TParent, TResult>(this TParent parent, Expression<Func<TParent, TResult>> operationExpression)
        {
            var operationName = operationExpression.GetMemberName();
            Record(parent, operationExpression, operationName);
        }

        private static void Record<TParent>(TParent parent, LambdaExpression operationExpression, string operationName)
        {
            var parameters = new Dictionary<string, object>();
            var number = Counter++;
            var methodExpr = operationExpression.Body as MethodCallExpression;
            if (methodExpr != null)
            {
                foreach (var expression in methodExpr.Arguments)
                {
                    var argMemberExpression = (MemberExpression)expression;
                    var constantExpression = (ConstantExpression)argMemberExpression.Expression;
                    var parametersValue = constantExpression.Value;
                    var f = (FieldInfo)argMemberExpression.Member;
                    var value = f.GetValue(parametersValue);
                    parameters.Add(f.Name, value);
                }
            }

            using (var writer = new JsonTextWriter(new StreamWriter($"{typeof(TParent).Name}.{operationName}-{number}.json")))
            {
                Serializer.Serialize(writer, parameters);
            }
        }
    }
}