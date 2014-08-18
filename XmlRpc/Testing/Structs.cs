using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XmlRpc.Types;
using XmlRpc.Types.Structs;

namespace XmlRpc.Testing
{
    /// <summary>
    /// Contains convenience methods to test Structs.
    /// </summary>
    public static class Structs
    {
        /// <summary>
        /// Takes an Assembly and an action that is performed on the result of the round-trip check, and checks every <see cref="XmlRpc.Types.Structs.BaseStruct"/> derivative
        /// that doesn't have generic parameters and isn't abstract for round-trip safety.
        /// <para/>
        /// This assumes that all the XmlRpcType&lt;&gt; that are supposed to be checked, are private fields of the class.
        /// </summary>
        /// <param name="assemblies">The assemblies to check the types in.</param>
        /// <param name="assertIsTrue">The action that is performed on the results of the round-trip check.
        /// First parameter is whether it was successful, second is the Type of the tested Struct, third is the reason it failed (if it did).</param>
        public static void AreRoundTripSave(Action<bool, Type, string> assertIsTrue, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var structTypes = assembly.GetExportedTypes().Where(t => t.InheritsOrImplements(typeof(BaseStruct)) && !t.IsAbstract && !t.ContainsGenericParameters);
                foreach (var structType in structTypes)
                {
                    var filledStruct = (BaseStruct)fillStruct(structType);
                    var generatedXml = filledStruct.GenerateXml();

                    var structInstance = (BaseStruct)Activator.CreateInstance(structType);
                    if (!structInstance.ParseXml(generatedXml))
                        assertIsTrue(false, structType, "Failed Parsing.");

                    assertIsTrue(generatedXml.ToString().Equals(filledStruct.GenerateXml().ToString()), structType, "Failed Equality Check");
                }
            }
        }

        private static object fillStruct(Type structType)
        {
            var structInstance = Activator.CreateInstance(structType);
            var contentFields = getContentFields(structType);

            foreach (var contentField in contentFields)
            {
                var contentFieldValue = contentField.GetValue(structInstance);
                var contentFieldValueValue = contentFieldValue.GetType().GetProperty("Value");
                var valueValueType = contentFieldValueValue.PropertyType;
                var value = resolveXmlRpcType(valueValueType);

                contentFieldValueValue.GetSetMethod().Invoke(contentFieldValue, new[] { value });
                contentField.SetValue(structInstance, contentFieldValue);
            }

            return structInstance;
        }

        private static IEnumerable<FieldInfo> getContentFields(Type structType)
        {
            return structType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField)
                             .Where(field => field.FieldType.InheritsOrImplements(typeof(XmlRpcType<>)));
        }

        private static object resolveXmlRpcType(Type type)
        {
            var value = new object();

            if (type == typeof(string))
                value = TestValues.TString;
            else if (type == typeof(int))
                value = TestValues.TInt;
            else if (type == typeof(double))
                value = TestValues.TDouble;
            else if (type == typeof(bool))
                value = TestValues.TBool;
            else if (type == typeof(DateTime))
                value = TestValues.TDateTime;
            else if (type == typeof(byte[])) // Base64
                value = TestValues.TByteArray;
            else if (type.InheritsOrImplements(typeof(BaseStruct)))
                value = fillStruct(type);
            else if (type.HasElementType && type.GetElementType().InheritsOrImplements(typeof(XmlRpcType<>)))
            {
                var genericList = Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetElementType()));

                var capsuledType = Activator.CreateInstance(type.GetElementType(),
                    type.GetElementType().InheritsOrImplements(typeof(XmlRpcStruct<>))
                        ? fillStruct(type.GetElementType().GetGenericArguments()[0])
                        : resolveXmlRpcType(type.GetElementType().GetProperty("Value").PropertyType));

                genericList.GetType().GetMethod("Add").Invoke(genericList, new[] { capsuledType });
                value = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(type.GetElementType()).Invoke(null, new[] { genericList });
            }

            return value;
        }

        private static class TestValues
        {
            public static readonly bool TBool = true;
            public static readonly byte[] TByteArray = { 4, 2 };
            public static readonly DateTime TDateTime = new DateTime(1970, 1, 1, 0, 0, 1);
            public static readonly double TDouble = 42;
            public static readonly int TInt = 42;
            public static readonly string TString = "Test";
        }
    }
}