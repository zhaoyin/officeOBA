#region Copyright and License Information

// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg., Weegen Patrick 2009-2013.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license

#endregion

using System.Collections.ObjectModel;

namespace Fluent.Sample.Mvvm.Model
{
    /// <summary>
    /// Represents collection of persons
    /// </summary>
    public class PersonCollection : ObservableCollection<Person>
    {
        /// <summary>
        /// Generates sample persons
        /// </summary>
        /// <returns></returns>
        public static PersonCollection Generate()
        {
            PersonCollection persons = new PersonCollection();
            persons.Add(new Person("Jane Lopes", "jane@lopes.com", "9 (679) 89086878", null));
            persons.Add(new Person("Abel Tomas", "abel@tomas.com", "4 (456) 78797897", null));
            persons.Add(new Person("Zig Perscot", "zig@perscot.com", "5 (568) 12489445", null));
            persons.Add(new Person("John Verwolf", "john@verwolf.com", "3 (454) 851384294", null));
            persons.Add(new Person("Denis Macdaff", "denis@macdaff.com", "9 (545) 454548489", null));
            persons.Add(new Person("Luka Madock", "luka@madock.com", "9 (545) 454548489", null));
            persons.Add(new Person("Mary Nickor", "mary@nickor.com", "9 (545) 454548489", null));
            persons.Add(new Person("David Avel", "david@avel.com", "9 (545) 454548489", null));
            persons.Add(new Person("Arnold Neferson", "arnold@eferson.com", "9 (545) 454548489", null));
            persons.Add(new Person("Mike Anderson", "mike@anderson.com", "9 (545) 454548489", null));
            return persons;
        }
    }
}
