﻿using NUnit.Framework;
using Onbox.Core.VDev.Mapping;
using Onbox.Revit.Tests.Mapping.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onbox.Revit.Tests.Mapping
{
    public partial class MapperShould
    {
        private Mapper SetupMapper()
        {
            var mapperManager = new MapperActionManager();
            var mapperOperator = new MapperOperator(mapperManager);
            var mapper = new Mapper(mapperOperator);
            return mapper;
        }

        private List<Person> SetupPersonList()
        {
            return new List<Person>
            {
                new Person { Age = 10, FirstName = "Jon", LastName = "Snow"},
                new Person { Age = 8, FirstName = "Sansa", LastName = "Stark"},
                new Person { Age = 4, FirstName = "Arya", LastName = "Stark"},
                new Person { Age = 12, FirstName = "Robb", LastName = "Stark"},
            };
        }

        private Person SetupPerson()
        {
            var father = new Person
            {
                FirstName = "Rickard",
                LastName = "Stark",
                Age = 65,
            };

            var person = new Person
            {
                FirstName = "Eddard",
                LastName = "Stark",
                Age = 46,
                Father = father,
            };

            var sister = new Person
            {
                FirstName = "Allana",
                LastName = "Stark",
                Age = 40,
                Father = father,
            };

            father.Children = new List<Person> { person, sister };

            person.Children = new List<Person>
            {
                new Person
                {
                    FirstName = "Robb",
                    LastName = "Stark",
                    Father = person,
                    Age = 19,
                },
            };

            return person;
        }

        [TestCase]
        public void CloneObject()
        {
            var sut = this.SetupMapper();

            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            Assert.AreNotSame(clone, person1);
        }

        [TestCase]
        public void CloneObjectAndCopySimpleProperties()
        {
            var sut = this.SetupMapper();

            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            Assert.AreEqual(clone.FirstName, person1.FirstName);
            Assert.AreEqual(clone.LastName, person1.LastName);
            Assert.AreEqual(clone.Age, person1.Age);
        }

        [TestCase]
        public void CloneNestedObjectsAndCopySimpleProperties()
        {
            var sut = this.SetupMapper();

            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            Assert.AreEqual(clone.Father.FirstName, person1.Father.FirstName);
            Assert.AreEqual(clone.Father.LastName, person1.Father.LastName);
            Assert.AreEqual(clone.Father.Age, person1.Father.Age);
        }

        [TestCase]
        public void CloneLists()
        {
            var sut = this.SetupMapper();
            List<Person> personList = SetupPersonList();

            var clone = sut.Clone(personList);

            Assert.AreNotSame(sut, clone);
        }

        [TestCase]
        public void CloneListObjects()
        {
            var sut = this.SetupMapper();
            List<Person> personList = SetupPersonList();

            var clone = sut.Clone(personList);

            for (int i = 0; i < clone.Count; i++)
            {
                var person1 = personList[i];
                var clone1 = clone[i];

                Assert.AreNotSame(clone1, person1);
                Assert.AreEqual(clone1.FirstName, person1.FirstName);
                Assert.AreEqual(clone1.LastName, person1.LastName);
                Assert.AreEqual(clone1.Age, person1.Age);
            }
        }

        [TestCase]
        public void CloneNestedListsInNestedObject()
        {
            var sut = this.SetupMapper();

            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            Assert.AreNotSame(clone.Father.Children, person1.Father.Children);
            Assert.AreEqual(clone.Father.Children.Count, person1.Father.Children.Count);
        }

        [TestCase]
        public void CloneObjectAndReferenceCircularReferences()
        {
            var sut = this.SetupMapper();

            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            Assert.AreSame(clone, clone.Children[0].Father);
        }

        [TestCase]
        public void CloneObjectAndReferenceCircularReferencesWhenNestedInLists()
        {
            var sut = this.SetupMapper();

            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            Assert.AreSame(clone, clone.Father.Children[0]);
        }

        [TestCase]
        public void CloneNestedLists()
        {
            var sut = this.SetupMapper();

            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            Assert.AreNotSame(clone.Children, person1.Children);
            Assert.AreEqual(clone.Children.Count, person1.Children.Count);
        }

        [TestCase]
        public void CloneObjectsInsideLists()
        {
            var sut = this.SetupMapper();

            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            for (int i = 0; i < person1.Children.Count; i++)
            {
                var person1Child = person1.Children[i];
                var cloneChild = clone.Children[i];

                Assert.AreNotSame(cloneChild, person1Child);
                Assert.AreEqual(cloneChild.FirstName, person1Child.FirstName);
                Assert.AreEqual(cloneChild.LastName, person1Child.LastName);
                Assert.AreEqual(cloneChild.Children, person1Child.Children);
                Assert.AreEqual(cloneChild.Age, person1Child.Age);
            }
        }

        [TestCase]
        public void CloneListOfPrimitives()
        {
            var sut = this.SetupMapper();

            var list = new List<int>
            {
                0, 1, 2, 3, 4, 5
            };

            var clone = sut.Clone(list);

            Assert.AreNotSame(list, clone);
        }

        [TestCase]
        public void CloneListOfPrimitiveValues()
        {
            var sut = this.SetupMapper();

            var list = new List<int>
            {
                0, 1, 2, 3, 4, 5
            };

            var clone = sut.Clone(list);

            for (int i = 0; i < list.Count; i++)
            {
                var n1 = list[i];
                var n2 = clone[i];

                Assert.AreEqual(n1, n2);
            }
        }

        [TestCase]
        public void CloneArrayOfPrimitives()
        {
            var sut = this.SetupMapper();

            var persons = new ArrayObject();
            persons.Array = new int[]
            {
                0, 1, 2, 3, 4, 5
            };

            var clone = sut.Clone(persons);

            Assert.IsNotNull(clone.Array);
            Assert.AreNotSame(persons.Array, clone.Array);
        }

        [TestCase]
        public void CloneArrayOfClasses()
        {
            var sut = this.SetupMapper();
            var persons = new ArrayObject()
            {
                ChildrenArray = new ArrayObject[]
                {
                    new ArrayObject(), new ArrayObject()
                }
            };

            var clone = sut.Clone(persons);

            Assert.IsNotNull(clone.ChildrenArray);
            Assert.AreNotSame(persons.ChildrenArray, clone.ChildrenArray);
        }

        [TestCase]
        public void CloneArrayOfClassesAndTheirClasses()
        {
            var sut = this.SetupMapper();
            var persons = new ArrayObject();

            persons.ChildrenArray = new ArrayObject[]
            {
                new ArrayObject(), new ArrayObject()
            };

            var clone = sut.Clone(persons);

            for (int i = 0; i < persons.ChildrenArray.Length; i++)
            {
                var n1 = persons.ChildrenArray[i];
                var n2 = clone.ChildrenArray[i];

                Assert.AreNotSame(n1, n2);
            }
        }

        [TestCase]
        public void CloneArrayOfClassesAndTheirClasses2()
        {
            var sut = this.SetupMapper();
            var persons = new ArrayObject();

            persons.ChildrenArray = new ArrayObject[]
            {
                persons
            };

            var clone = sut.Clone(persons);

            Assert.AreSame(clone, clone.ChildrenArray[0]);

            for (int i = 0; i < persons.ChildrenArray.Length; i++)
            {
                var n1 = persons.ChildrenArray[i];
                var n2 = clone.ChildrenArray[i];

                Assert.AreNotSame(n1, n2);
            }
        }

        [TestCase]
        public void CloneAndCopyObjects()
        {
            var sut = this.SetupMapper();
            
            var person1 = this.SetupPerson();
            var clone = sut.Clone(person1);

            //Assert.AreNotEqual(clone, person1);

            //Assert.That(clone.Age == person1.Age);
            //Assert.That(clone.FirstName == person1.FirstName);
            //Assert.That(clone.LastName == person1.LastName);
            //Assert.That(clone.Children != null);
            //Assert.That(clone.Children.Count == person1.Children.Count);

            //Assert.That(clone.Father != null);
            //Assert.That(clone.Father.Children != null);
            //Assert.That(clone.Father.Children.Count == person1.Father.Children.Count);
        }
    }
}
