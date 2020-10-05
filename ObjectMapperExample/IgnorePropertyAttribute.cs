using System;

namespace ObjectMapperExample
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnorePropertyAttribute : Attribute { }
}
