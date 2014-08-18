using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XmlRpc.Methods;

namespace XmlRpc.Testing
{
    /// <summary>
    /// Contains convenience methods to test MethodCalls.
    /// </summary>
    public static class MethodCalls
    {
        /// <summary>
        /// Takes an Assembly and an action that is performed on the result of the round-trip check, and checks every <see cref="XmlRpcMethodCall{TReturn,TReturnBase}"/> derivative
        /// that doesn't have generic parameters and isn't abstract for round-trip safety.
        /// <para/>
        /// This assumes that all the XmlRpcType&lt;&gt; that are supposed to be checked, are private fields of the class, and are the only things that are serialized.
        /// </summary>
        /// <param name="assemblies">The assemblies to check the types in.</param>
        /// <param name="assertIsTrue">
        /// The action that is performed on the results of the round-trip check.
        /// First parameter is whether it was successful, second is the Type of the tested MethodCall, third is the reason it failed (if it did).
        /// </param>
        public static void AreRoundTripSave(Action<bool, Type, string> assertIsTrue, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var methodCallTypes =
                    assembly.GetExportedTypes().Where(t => t.InheritsOrImplements(typeof(XmlRpcMethodCall<,>)) && !t.IsAbstract && !t.ContainsGenericParameters);
                foreach (var methodCallType in methodCallTypes)
                {
                    var filledMethod = fillMethod(methodCallType);
                    var generatedCallXml = filledMethod.GetType().GetMethod("GenerateCallXml").Invoke(filledMethod, new object[0]);
                    var generatedCallXmlString = generatedCallXml.ToString();
                    var generatedResponseXml = filledMethod.GetType().GetMethod("GenerateResponseXml").Invoke(filledMethod, new object[0]);

                    var methodInstance = createMethodInstance(methodCallType);

                    if (!(bool)methodInstance.GetType().GetMethod("ParseCallXml").Invoke(methodInstance, new[] { generatedCallXml }))
                        assertIsTrue(false, methodCallType, "Failed Call Parsing.");

                    if (!(bool)methodInstance.GetType().GetMethod("ParseResponseXml").Invoke(methodInstance, new[] { generatedResponseXml }))
                        assertIsTrue(false, methodCallType, "Failed Response Parsing");

                    var generatedCallAfterParsing = methodInstance.GetType().GetMethod("GenerateCallXml").Invoke(methodInstance, new object[0]);
                    var generatedResponseAfterParsing = methodInstance.GetType().GetMethod("GenerateResponseXml").Invoke(methodInstance, new object[0]);

                    assertIsTrue(
                        generatedCallXmlString.Equals(generatedCallAfterParsing.ToString())
                        && generatedResponseXml.ToString().Equals(generatedResponseAfterParsing.ToString()),
                        methodCallType, "Failed Equality Check");
                }
            }
        }

        private static object createMethodInstance(Type type)
        {
            var constructor = type.GetConstructors().First();
            var parameters = constructor.GetParameters().Select(parameter =>
                                                                {
                                                                    if (parameter.ParameterType.IsArray)
                                                                        return Array.CreateInstance(parameter.ParameterType.GetElementType(), 0);

                                                                    if (parameter.ParameterType.InheritsOrImplements(typeof(IEnumerable<>))
                                                                        && parameter.ParameterType != typeof(string))
                                                                    {
                                                                        return typeof(Enumerable).GetMethod("Empty")
                                                                                                 .MakeGenericMethod(parameter.ParameterType.GetGenericArguments()[0])
                                                                                                 .Invoke(null, new object[0]);
                                                                    }

                                                                    return parameter.ParameterType.GetDefaultValue();
                                                                }).ToArray();

            return Activator.CreateInstance(type, parameters);
        }

        private static object fillMethod(Type methodCallType)
        {
            var methodInstance = createMethodInstance(methodCallType);

            var contentFields = Structs.getContentFields(methodCallType);

            foreach (var contentField in contentFields)
            {
                var contentFieldValue = contentField.GetValue(methodInstance);
                var contentFieldValueValue = contentFieldValue.GetType().GetProperty("Value");
                var valueValueType = contentFieldValueValue.PropertyType;
                var value = Structs.resolveXmlRpcType(valueValueType);

                contentFieldValueValue.GetSetMethod().Invoke(contentFieldValue, new[] { value });
                contentField.SetValue(methodInstance, contentFieldValue);
            }

            return methodInstance;
        }
    }
}