using System;

namespace ObjectMapperExample
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MappingAttribute : Attribute
    {
        public string Name { get; private set; }

        public MappingAttribute(string name)
        {
            Name = name;
        }
    }
}
