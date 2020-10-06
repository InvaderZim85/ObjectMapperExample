using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectMapperExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an empty person
            var person = new Person();

            // Create a person db model
            var dbModel = new PersonDbModel
            {
                Id = 1,
                Name = "Bender",
                Birthday = new DateTime(1985, 7, 12)
            };

            var birthdayValue = new BirthdayValue
            {
                Id = "100",
                PersonId = 1,
                Birthday = new DateTime(2000, 1, 1)
            };

            // Existing objects
            Console.WriteLine("Objects before mapping:");
            Console.WriteLine("Empty person");
            PrintObject(person);

            Console.WriteLine("PersonDbModel");
            PrintObject(dbModel);

            Console.WriteLine("BirthdayValue");
            PrintObject(birthdayValue);

            // Map the objects
            ObjectMapper.Map(person, dbModel, birthdayValue);

            Console.WriteLine("Main object after mapping");
            PrintObject(person);

            Console.WriteLine("Map of a dynamic object");
            ObjectMapper.Map(person, new
            {
                Birthday = DateTime.Now
            });
            PrintObject(person);

            // Creates a complete new object
            var result = ObjectMapper.Map<Person>(dbModel, new
            {
                Birthday = DateTime.Now
            });
            Console.WriteLine("Creation of new type");
            PrintObject(result);


            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        /// <summary>
        /// Prints the content of the given object
        /// </summary>
        /// <param name="obj">The object which should be printed</param>
        private static void PrintObject(object obj)
        {
            var printList = new SortedList<string, object>();

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                printList.Add(property.Name, ObjectMapper.GetPropertyValue(obj, property.Name));
            }

            var maxLength = printList.Keys.Max(m => m.Length);

            foreach (var (key, value) in printList)
            {
                Console.WriteLine($"{key.PadRight(maxLength, '.')}: {value}");
            }

            Console.WriteLine();
        }
    }

    internal class Person
    {
        public int Id { get; set; }
        [Mapping("Name")]
        public string FirstName { get; set; }
        public DateTime Birthday { get; set; }
    }

    internal class PersonDbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [IgnoreProperty]
        internal DateTime Birthday { get; set; }
    }

    internal class BirthdayValue
    {
        public string Id { get; set; } // Test with the id
        public int PersonId { get; set; }
        public DateTime Birthday { get; set; }
    }
}
