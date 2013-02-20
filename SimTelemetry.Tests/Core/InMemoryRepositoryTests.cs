using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Tests.Core
{
#if DEBUG
    [TestFixture]
    public class InMemoryRepositoryTests
    {
        [Test]
        public void InMemoryTests()
        {
            var obj1 = new InMemoryObject("1", "Test 1");
            var obj2 = new InMemoryObject("A", "Test 2");
            var obj3 = new InMemoryObject("!", "Test 3");
            var rep = new InMemoryRepository<InMemoryObject, string>();

            Assert.AreEqual(rep.GetAll().Count(x => true), 0);
            Assert.AreEqual(rep.Contains(obj1), false);
            Assert.AreEqual(rep.Contains(obj2), false);
            Assert.AreEqual(rep.Contains(obj3), false);

            // Add 1
            rep.Add(obj1);

            Assert.AreEqual(rep.GetAll().Count(x => true), 1);
            Assert.AreEqual(rep.Contains(obj1), true);
            Assert.AreEqual(rep.Contains(obj2), false);
            Assert.AreEqual(rep.Contains(obj3), false);

            // Add 2
            rep.Add(obj2);

            Assert.AreEqual(rep.GetAll().Count(x => true), 2);
            Assert.AreEqual(rep.Contains(obj1), true);
            Assert.AreEqual(rep.Contains(obj2), true);
            Assert.AreEqual(rep.Contains(obj3), false);

            // Add 3
            rep.Add(obj3);

            Assert.AreEqual(rep.GetAll().Count(x => true), 3);
            Assert.AreEqual(rep.Contains(obj1), true);
            Assert.AreEqual(rep.Contains(obj2), true);
            Assert.AreEqual(rep.Contains(obj3), true);

            // Readd
            rep.Add(obj3);

            Assert.AreEqual(rep.GetAll().Count(x => true), 3);
            Assert.AreEqual(rep.Contains(obj1), true);
            Assert.AreEqual(rep.Contains(obj2), true);
            Assert.AreEqual(rep.Contains(obj3), true);

            // Store
            var obj1Get = rep.GetAll().Where(x => x.ID == "1").FirstOrDefault();
            obj1Get.UpdateTest("Test 1a");
            Assert.AreEqual(true, rep.Store(obj1Get));

            // Remove 2
            rep.Remove(obj2);

            Assert.AreEqual(rep.GetAll().Count(x => true), 2);
            Assert.AreEqual(rep.Contains(obj1), true);
            Assert.AreEqual(rep.Contains(obj2), false);
            Assert.AreEqual(rep.Contains(obj3), true);

            // Remove 1
            rep.Remove(obj1);

            Assert.AreEqual(rep.GetAll().Count(x => true), 1);
            Assert.AreEqual(rep.Contains(obj1), false);
            Assert.AreEqual(rep.Contains(obj2), false);
            Assert.AreEqual(rep.Contains(obj3), true);

            // Add 1 & 2
            rep.AddRange(new[] { obj1, obj2 });

            Assert.AreEqual(rep.GetAll().Count(x => true), 3);
            Assert.AreEqual(rep.Contains(obj1), true);
            Assert.AreEqual(rep.Contains(obj2), true);
            Assert.AreEqual(rep.Contains(obj3), true);

            rep.Clear();

            Assert.AreEqual(rep.GetAll().Count(x => true), 0);
            Assert.AreEqual(rep.Contains(obj1), false);
            Assert.AreEqual(rep.Contains(obj2), false);
            Assert.AreEqual(rep.Contains(obj3), false);

        }

        [Test]
        public void LazyInMemoryReadOnlyTest()
        {
            var myObjectCreator = new InMemoryObjectDataSource();
            var lazyRepo = new LazyInMemoryRepository<InMemoryObject, string>(myObjectCreator);

            // Unitialized
            Assert.AreEqual(0, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(0, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(0, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(0, myObjectCreator.RemoveObjectCalls);

            // Only peek what's out there:
            Assert.AreEqual(false, lazyRepo.Contains("A"));
            Assert.AreEqual(true, lazyRepo.Contains("B"));
            Assert.AreEqual(false, lazyRepo.Contains("C"));
            Assert.AreEqual(true, lazyRepo.Contains("1"));
            Assert.AreEqual(false, lazyRepo.Contains("2"));
            Assert.AreEqual(true, lazyRepo.Contains("!"));
            Assert.AreEqual(false, lazyRepo.Contains("@"));

            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(0, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(0, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(0, myObjectCreator.RemoveObjectCalls);

            // Now let's get an object:
            var myObjectB = lazyRepo.GetById("B");
            Assert.AreEqual("B", myObjectB.ID);
            Assert.AreEqual("Test 2", myObjectB.Test);

            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(0, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(0, myObjectCreator.RemoveObjectCalls);

            // Try to add an object.
            var myObjectA = new InMemoryObject("A", "Test A");
            Assert.AreEqual(false, lazyRepo.Add(myObjectA)); // read-only!
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(0, myObjectCreator.RemoveObjectCalls);

            // Try to remove an object.
            Assert.AreEqual(false, lazyRepo.Remove(myObjectB)); // read-only!
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(1, myObjectCreator.RemoveObjectCalls);

            // Try to remove an object (that doens't even exist).
            Assert.AreEqual(false, lazyRepo.Remove(myObjectA)); // read-only!
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(2, myObjectCreator.RemoveObjectCalls);

            // After trying to remove it, try to get object B again:
            var myObjectB_2 = lazyRepo.GetById("B");
            Assert.AreEqual(true, myObjectB_2.Equals(myObjectB));
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls); // we already had this object in data list.
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(2, myObjectCreator.RemoveObjectCalls);
            Assert.AreEqual(1, lazyRepo.GetDataCount() );

            // Try to get object C (doesn't exist)
            var myObjectC = lazyRepo.GetById("C");
            Assert.AreEqual(null, myObjectC);
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls); // tried to get C
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(2, myObjectCreator.RemoveObjectCalls);
            Assert.AreEqual(1, lazyRepo.GetDataCount());

            // Have object '!' ready, and try to see if it contains it.
            var objectExclamationMark = new InMemoryObject("!", "Test 3");
            Assert.AreEqual(false, lazyRepo.Contains(objectExclamationMark)); // we haven't pulled the object out of the repo!
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(2, myObjectCreator.RemoveObjectCalls);
            Assert.AreEqual(1, lazyRepo.GetDataCount());

            // Try to store it:
            var objectDollarSign = new InMemoryObject("$", "Money");
            Assert.AreEqual(false, lazyRepo.Store(objectDollarSign));
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(0, myObjectCreator.StoreObjectCalls); // No DataSource::Store() is called because this object isn't in the ID list.
            Assert.AreEqual(2, myObjectCreator.RemoveObjectCalls);
            Assert.AreEqual(1, lazyRepo.GetDataCount());

            // Try to update/store existing object
            var myOwnObjectA = new InMemoryObject("B", "Test B");
            Assert.AreEqual(false, lazyRepo.Store(myOwnObjectA)); // read-only repo
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(1, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(1, myObjectCreator.StoreObjectCalls); // Now it does try to store it, because the ID exists.
            Assert.AreEqual(2, myObjectCreator.RemoveObjectCalls);
            Assert.AreEqual(1, lazyRepo.GetDataCount());

            // Get all:
            Assert.AreEqual(3, lazyRepo.GetAll().ToList().Count);
            Assert.AreEqual(1, myObjectCreator.GetIdsCalls);
            Assert.AreEqual(1, myObjectCreator.AddObjectCalls);
            Assert.AreEqual(3, myObjectCreator.GetObjectCalls);
            Assert.AreEqual(1, myObjectCreator.StoreObjectCalls);
            Assert.AreEqual(2, myObjectCreator.RemoveObjectCalls);

        }

        [Test]
        public void ThreadingTests()
        {

            var myObjectCreator = new InMemoryObjectDataRepository();
            var lazyRepo = new LazyInMemoryRepository<InMemoryObject, string>(myObjectCreator);

            Task slowIterator = new Task(() =>
                                    {
                                        foreach (var item in lazyRepo.GetAll())
                                        {
                                            // Let's iterate very slowly :)
                                            Thread.Sleep(100);
                                        }
                                    });
            Task slowAdded = new Task(() =>
                                          {
                                              Thread.Sleep(50);
                                              lazyRepo.Add(new InMemoryObject("T", "Mwhuahua"));
                                              Thread.Sleep(100);
                                              lazyRepo.Store(new InMemoryObject("T", "Mwhuahua2"));
                                              Thread.Sleep(100);
                                              lazyRepo.Remove(new InMemoryObject("T", "Mwhuahua"));
                                          });
            slowIterator.Start();
            slowAdded.Start();

            slowIterator.Wait();

        }

    }

    public class InMemoryObjectDataRepository : ILazyRepositoryDataSource<InMemoryObject, string>
    {
        // Specific to testing:
        public int GetIdsCalls { get; private set; }
        public int AddObjectCalls { get; private set; }
        public int GetObjectCalls { get; private set; }
        public int StoreObjectCalls { get; private set; }
        public int RemoveObjectCalls { get; private set; }

        private ConcurrentDictionary<string, InMemoryObject> ids = new ConcurrentDictionary<string, InMemoryObject>();

        public InMemoryObjectDataRepository()
        {
            GetIdsCalls = 0;
            AddObjectCalls = 0;
            GetObjectCalls = 0;
            StoreObjectCalls = 0;
            RemoveObjectCalls = 0;

            ids = new ConcurrentDictionary<string, InMemoryObject>();
            ids.TryAdd("1", new InMemoryObject("1", "Test 1"));
            ids.TryAdd("B", new InMemoryObject("B", "Test 2"));
            ids.TryAdd("!", new InMemoryObject("!", "Test 3"));
        }

        public IList<string> GetIds()
        {
            GetIdsCalls++;

            // Do I/O, DB, stuff here
            return ids.Keys.ToList();
        }

        public bool Add(InMemoryObject obj)
        {
            AddObjectCalls++;
            
            return ids.TryAdd(obj.ID, obj);
        }

        public bool Store(InMemoryObject obj)
        {
            StoreObjectCalls++;
            return ids.TryUpdate(obj.ID, obj, Get(obj.ID));
        }

        public InMemoryObject Get(string id)
        {
            GetObjectCalls++;

            return ids.ContainsKey(id) ? ids[id] : new InMemoryObject();
        }

        public bool Remove(string Id)
        {
            RemoveObjectCalls++;

            // It's get only repo (in this test)
            // It will however, remove the ID.
            InMemoryObject tmp;
            return ids.TryRemove(Id, out tmp);
        }

        public bool Clear()
        {
            ids.Clear();
            return true; // read-only
        }
    }

    public class InMemoryObjectDataSource : ILazyRepositoryDataSource<InMemoryObject, string>
    {
        // Specific to testing:
        public int GetIdsCalls { get; private set; }
        public int AddObjectCalls { get; private set; }
        public int GetObjectCalls { get; private set; }
        public int StoreObjectCalls { get; private set; }
        public int RemoveObjectCalls { get; private set; }

        public InMemoryObjectDataSource()
        {
            GetIdsCalls = 0;
            AddObjectCalls = 0;
            GetObjectCalls = 0;
            StoreObjectCalls = 0;
            RemoveObjectCalls = 0;

        }

        public IList<string> GetIds()
        {
            GetIdsCalls++;

            // Do I/O, DB, stuff here
            var ids2 = new List<string> {"1", "B", "!"};
            return ids2;
        }

        public bool Add(InMemoryObject obj)
        {
            AddObjectCalls++;

            // It's get-only repo.
            return false;
        }

        public bool Store(InMemoryObject obj)
        {
            StoreObjectCalls++;
            return false;
        }

        public InMemoryObject Get(string id)
        {
            GetObjectCalls++;

            switch (id)
            {
                case "1":
                    return new InMemoryObject("1", "Test 1");
                    break;
                case "B":
                    return new InMemoryObject("B", "Test 2");
                    break;
                case "!":
                    return new InMemoryObject("!", "Test 3");
                    break;
            }
            return new InMemoryObject();
        }

        public bool Remove(string Id)
        {
            RemoveObjectCalls++;

            // It's get only repo (in this test)
            // It will however, remove the ID.
            return false;
        }

        public bool Clear()
        {
            return false; // read-only
        }
    }

    public class InMemoryObject : IEntity<string>, IEquatable<InMemoryObject>
    {
        public string ID { get; private set; }
        public string Test { get; private set; }

        public InMemoryObject()
        {
        }

        public InMemoryObject(string id, string test)
        {
            ID = id;
            Test = test;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(InMemoryObject)))
                return Equals((InMemoryObject)obj);
            else if (obj.GetType().Equals(typeof(string)))
                return Equals((string) obj);
            else
                return false;
        }


        public bool Equals(InMemoryObject other)
        {
            return ID.Equals(other.ID);
        }

        public bool Equals(string other)
        {
            return ID.Equals(other);
        }

        public void UpdateTest(string test)
        {
            Test = test;
        }
    }
#endif
}
