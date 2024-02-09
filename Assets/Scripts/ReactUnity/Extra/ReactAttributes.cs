using System;

namespace Kukumberman.ReactUnity.Extra
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ReactCallbackAttribute : Attribute
    {
        public int Order;
    }

    public sealed class ReactBeforeStartAttribute : ReactCallbackAttribute { }

    public sealed class ReactAfterStartAttribute : ReactCallbackAttribute { }

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true
    )]
    public sealed class ReactInjectGlobalAttribute : Attribute
    {
        public readonly string Name;

        public ReactInjectGlobalAttribute(string name)
        {
            Name = name;
        }
    }
}
