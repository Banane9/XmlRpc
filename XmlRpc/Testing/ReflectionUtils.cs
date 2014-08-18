using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlRpc.Testing
{
    internal static class ReflectionUtils
    {
        public static bool InheritsOrImplements(this Type child, Type parent)
        {
            parent = resolveGenericTypeDefinition(parent);

            var currentChild = child.IsGenericType
                                   ? child.GetGenericTypeDefinition()
                                   : child;

            while (currentChild != typeof(object))
            {
                if (parent == currentChild || hasAnyInterfaces(parent, currentChild))
                    return true;

                currentChild = currentChild.BaseType != null
                               && currentChild.BaseType.IsGenericType
                                   ? currentChild.BaseType.GetGenericTypeDefinition()
                                   : currentChild.BaseType;

                if (currentChild == null)
                    return false;
            }
            return false;
        }

        private static bool hasAnyInterfaces(Type parent, Type child)
        {
            return child.GetInterfaces()
                        .Any(childInterface =>
                             {
                                 var currentInterface = childInterface.IsGenericType
                                                            ? childInterface.GetGenericTypeDefinition()
                                                            : childInterface;

                                 return currentInterface == parent;
                             });
        }

        private static Type resolveGenericTypeDefinition(Type parent)
        {
            var shouldUseGenericType = !(parent.IsGenericType && parent.GetGenericTypeDefinition() != parent);

            if (parent.IsGenericType && shouldUseGenericType)
                parent = parent.GetGenericTypeDefinition();
            return parent;
        }
    }
}