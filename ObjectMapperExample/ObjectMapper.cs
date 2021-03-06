﻿using System;
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
        /// Maps multiples objects
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

            // Get the properties of the main and the other object
            var mainProperties = main.GetType().GetProperties();
            var otherProperties = other.GetType().GetProperties();

            // Iterate through every main property and check if the other
            // object contains a property with the same name and type
            foreach (var mainProperty in mainProperties)
            {
                if (mainProperty.IgnoreProperty())
                    continue; // The property should be ignored, skip the rest

                var name = mainProperty.GetMappingName();
                // Check if the other object contains a property with the same name
                var otherProperty = otherProperties.FirstOrDefault(f =>
                    f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                
                if (otherProperty == null)
                    continue; // No property found, skip the rest

                var mainType = mainProperty.PropertyType;
                var otherType = otherProperty.PropertyType;
                if (mainType != otherType)
                    continue; // The properties are not equal, skip the rest

                if (otherProperty.IgnoreProperty())
                    continue; // The property should be ignored, skip the rest

                var otherValue = GetPropertyValue(other, otherProperty.Name);
                mainProperty.SetValue(main, otherValue);
            }
        }


        /// <summary>
        /// Creates a new object of the given type and maps the given data
        /// </summary>
        /// <typeparam name="T">The type of the new object</typeparam>
        /// <param name="mapObjects">The objects which should be mapped</param>
        /// <returns>The new type</returns>
        public static T Map<T>(params object[] mapObjects) where T: class, new()
        {
            var mainObject = Activator.CreateInstance<T>();

            Map(mainObject, mapObjects);

            return mainObject;
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
        private static bool IgnoreProperty(this MemberInfo type)
        {
            var attribute = type.GetCustomAttribute<IgnorePropertyAttribute>();
            return attribute != null;
        }

        /// <summary>
        /// Gets the mapping name
        /// </summary>
        /// <param name="type">The type of the property</param>
        /// <returns>The mapping name</returns>
        private static string GetMappingName(this MemberInfo type)
        {
            var attribute = type.GetCustomAttribute<MappingAttribute>();

            return attribute?.Name ?? type.Name;
        }
    }
}
