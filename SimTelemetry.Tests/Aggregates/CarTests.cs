using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Tests.Aggregates
{
    [TestFixture]
    class CarTests : IDisposable
    {
        private Car car;

        public CarTests()
        {
            car = new Car("file.ini", "Name", "Driver Sir", "Description", 1337);
        }

        public void Dispose()
        {
            car = null;
            GlobalEvents.Reset();
            Assert.AreEqual(0, GlobalEvents.Count);
        }

        [Test]
        public void Basic()
        {
            

            Assert.AreEqual("file.ini", car.ID);
            Assert.AreEqual("Name", car.Name);
            Assert.AreEqual("Driver Sir", car.Driver);
            //Assert.AreEqual("D. Sir", car.Driver); // TODO: implement official names
            Assert.AreEqual("Description", car.Description);
            Assert.AreEqual(1337, car.StartNumber);

            Assert.AreEqual(0, car.CarClass.ToList().Count);
            Assert.AreEqual(null, car.Team);
            Assert.AreEqual(null, car.Engine);
            Assert.AreEqual(null, car.Chassis);
            Assert.AreEqual(0, car.Wheels.ToList().Count);
            Assert.AreEqual(0, car.Brakes.ToList().Count);
            Assert.AreEqual(null, car.Drivetrain);
        }

        [Test]
        public void Classes()
        {
            Assert.AreNotEqual(null, car);


            Assert.AreEqual(0, car.CarClass.ToList().Count);

            car.Assign("Test Group");

            Assert.AreEqual(1, car.CarClass.ToList().Count);
            Assert.AreEqual(true, car.BelongsTo("Test Group"));
            Assert.AreEqual(false, car.BelongsTo("Test 1"));
            Assert.AreEqual(false, car.BelongsTo("Test 2"));
            Assert.AreEqual(true, car.BelongsTo(new[] { "Test Group" }));
            Assert.AreEqual(true, car.BelongsTo(new[] { "Test Group" , "Test 1"}));

            car.Assign(new[] { "Test 1", "Test 2" });
            Assert.AreEqual(3, car.CarClass.ToList().Count);
            Assert.AreEqual(true, car.BelongsTo("Test 1"));
            Assert.AreEqual(true, car.BelongsTo("Test 2"));
            Assert.AreEqual(true, car.BelongsTo(new[] { "Test 1" }));
            Assert.AreEqual(true, car.BelongsTo(new[] { "Test 1" , "Test 2" }));

            car.Unassign("Test Group");
            Assert.AreEqual(2, car.CarClass.ToList().Count);
            Assert.AreEqual(true, car.BelongsTo("Test 1"));
            Assert.AreEqual(true, car.BelongsTo("Test 2"));
            Assert.AreEqual(true, car.BelongsTo(new[] { "Test 1" }));
            Assert.AreEqual(true, car.BelongsTo(new[] { "Test 1", "Test 2" }));
            Assert.AreEqual(true, car.BelongsTo(new[] { "Test Group", "Test 1" }));

            car.Unassign(new[] { "Test 1", "Test 2" });
            Assert.AreEqual(0, car.CarClass.ToList().Count);
            Assert.AreEqual(false, car.BelongsTo("Test 1"));
            Assert.AreEqual(false, car.BelongsTo("Test 2"));
            Assert.AreEqual(false, car.BelongsTo(new[] { "Test 1" }));
            Assert.AreEqual(false, car.BelongsTo(new[] { "Test 1", "Test 2" }));
            Assert.AreEqual(false, car.BelongsTo(new[] { "Test Group", "Test 1" }));

        }
    }
}
