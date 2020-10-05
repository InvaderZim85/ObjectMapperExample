using System;
using System.Linq;
using System.Reflection;

namespace ObjectMapperExample
{
    /// <summary>
    /// Provides functions to map two objects
    /// </summary>
    internal static class ObjectMapper
    {
        /// <summary>
        /// Maps two objects into one
        /// </summary>
        /// <param name="main">The main object</param>
        /// <param name="mapObjects">The objects which should be mapped</param>
        public static void Map(object main, params object[] mapObjects)
        {
            foreach (var mapObject in mapObjects)
            {
                Map(main, mapObject);
            }
        }

        /// <summary>
        /// Maps two objects into one
        /// </summary>
        /// <param name="main">The main object</param>
        /// <param name="other">The other objects</param>
        public static void Map(object main, object other)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var mainProperties = main.GetType().GetProperties();
            var otherProperties = other.GetType().GetProperties();

            foreach (var mainProperty in mainProperties)
            {
                // Check if the other object contains a property with the same name
                var otherProperty = otherProperties.FirstOrDefault(f =>
                    f.Name.Equals(mainProperty.Name, StringComparison.OrdinalIgnoreCase));
                
                if (otherProperty == null)
                    continue; // No property found, skip the rest

                if (mainProperty.GetType() != otherProperty.GetType())
                    continue; // The types are not equal, skip the rest

                if (IgnoreProperty(otherProperty)) 
                    continue; // The property should be ignored, skip the rest

                var otherValue = GetPropertyValue(other, otherProperty.Name);
                mainProperty.SetValue(main, otherValue);
            }
        }

        /// <summary>
        /// Gets the value of a property
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="propertyName">The property name</param>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
                return null;

            if (!obj.GetType().GetProperties().Any(a => a.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)))
                return null;

            return obj.GetType().GetProperty(propertyName) == null
                ? null
                : obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }

        /// <summary>
        /// Checks if the property should be ignored
        /// </summary>
        /// <param name="type">The type of the property</param>
        /// <returns>true when the property should be ignored, otherwise false</returns>
        private static bool IgnoreProperty(MemberInfo type)
        {
            var attribute = Attribute.GetCustomAttribute(type, typeof(IgnorePropertyAttribute));
            return attribute != null;
        }
    }
}
