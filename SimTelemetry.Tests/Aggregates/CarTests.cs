using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Entities;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Exceptions;
using SimTelemetry.Domain.ValueObjects;

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
#if DEBUG
            GlobalEvents.Reset();
            Assert.AreEqual(0, GlobalEvents.Count);
#endif
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

        [Test]
        public void TestEngine()
        {
            var hasException = false;
            var testEng = new CarTestEngine("V10 3.0L", "Cosworth", 10, 3000, new Range(3000, 4000),
                                            new Range(17500, 18500), new List<EngineMode>(), new List<EngineTorque>(),
                                            new EngineLifetime(new NormalDistrbution(3600, 200),
                                                               new NormalDistrbution(15000, 100),
                                                               new NormalDistrbution(100, 5)));

            car.Assign(testEng);


            Assert.IsNotNull(car.Engine);
            Assert.AreEqual(car.Engine, testEng);
            try
            {
                car.Assign(testEng);
            }
            catch (CarAlreadyHasEngineException ex)
            {
                // good
                hasException = true;
            }
            catch (Exception ex)
            {
                // Bad
                Assert.Fail();
            }
            Assert.True(hasException);
        }

        [Test]
        public void TestChassis()
        {
            var hasException = false;
            var testChassis = new CarTestChassis(500, 125, 0.3f, new Polynomial(0.3), new Polynomial(0.3),
                                                 new Polynomial(0.05), new Polynomial(0.025), 1);

            car.Assign(testChassis);

            Assert.IsNotNull(car.Chassis);
            Assert.AreEqual(car.Chassis, testChassis);
            try
            {
                car.Assign(testChassis);
            }
            catch (CarAlreadyHasChassisException ex)
            {
                // good
                hasException = true;
            }
            catch (Exception ex)
            {
                // Bad
                Assert.Fail();
            }
            Assert.True(hasException);
        }

        [Test]
        public void TestDrivetrain()
        {
            var hasException = false;
            var testDrivetrain = new CarTestDrivetrain(0, 0, 0.3f, 0.7f, 6, new List<float>(), new List<float>(), new List<int>(), 0, DriveTrainSetup.REAR, 0);

            car.Assign(testDrivetrain);

            Assert.IsNotNull(car.Drivetrain);
            Assert.AreEqual(car.Drivetrain, testDrivetrain);
            try
            {
                car.Assign(testDrivetrain);
            }
            catch (CarAlreadyHasDrivetrainException ex)
            {
                // good
                hasException = true;
            }
            catch (Exception ex)
            {
                // Bad
                Assert.Fail();
            }
            Assert.True(hasException);
        }

        [Test]
        public void TestWheels()
        {
            var hasException = false;
            var WheelLF = new CarTestWheel(WheelLocation.FRONTLEFT, 0.6f, 1000, 90, 105, 50, 0.05f);
            var WheelRF = new CarTestWheel(WheelLocation.FRONTRIGHT, 0.6f, 1000, 90, 105, 50, 0.05f);
            var WheelLR = new CarTestWheel(WheelLocation.REARLEFT, 0.6f, 1000, 90, 105, 50, 0.05f);
            var WheelRR = new CarTestWheel(WheelLocation.REARRIGHT, 0.6f, 1000, 90, 105, 50, 0.05f);
            
            // Left - front
            car.Assign(WheelLF);

            Assert.IsNotNull(car.Wheels);
            Assert.AreEqual(1, car.Wheels.Count());
            Assert.IsNotNull(car.Wheels.FirstOrDefault());
            Assert.IsNotNull(car.Wheels.Where(x => x.Location == WheelLF.Location).FirstOrDefault());
            try
            {
                car.Assign(WheelLF);
            }
            catch (CarAlreadyHasWheelException ex)
            {
                // good
                hasException = true;
            }
            catch (Exception ex)
            {
                // Bad
                Assert.Fail();
            }
            Assert.True(hasException);

            // Right - front
            hasException = false;
            car.Assign(WheelRF);

            Assert.AreEqual(2, car.Wheels.Count());
            Assert.IsNotNull(car.Wheels.Where(x => x.Location == WheelRF.Location).FirstOrDefault());
            try
            {
                car.Assign(WheelRF);
            }
            catch (CarAlreadyHasWheelException ex)
            {
                // good
                hasException = true;
            }
            catch (Exception ex)
            {
                // Bad
                Assert.Fail();
            }
            Assert.True(hasException);

            // Left - rear
            hasException = false;
            car.Assign(WheelLR);

            Assert.AreEqual(3, car.Wheels.Count());
            Assert.IsNotNull(car.Wheels.Where(x => x.Location == WheelLR.Location).FirstOrDefault());
            try
            {
                car.Assign(WheelLR);
            }
            catch (CarAlreadyHasWheelException ex)
            {
                // good
                hasException = true;
            }
            catch (Exception ex)
            {
                // Bad
                Assert.Fail();
            }
            Assert.True(hasException);

            // Right - rear
            hasException = false;
            car.Assign(WheelRR);

            Assert.AreEqual(4, car.Wheels.Count());
            Assert.IsNotNull(car.Wheels.Where(x => x.Location == WheelRR.Location).FirstOrDefault());
            try
            {
                car.Assign(WheelRR);
            }
            catch (CarAlreadyHasWheelException ex)
            {
                // good
                hasException = true;
            }
            catch (Exception ex)
            {
                // Bad
                Assert.Fail();
            }
            Assert.True(hasException);

            Assert.AreEqual(car.Wheels.ElementAt(0), WheelLF);
            Assert.AreEqual(car.Wheels.ElementAt(1), WheelRF);
            Assert.AreEqual(car.Wheels.ElementAt(2), WheelLR);
            Assert.AreEqual(car.Wheels.ElementAt(3), WheelRR);
        }

        [Test]
        public void TestBrakes()
        {
            var hasException = false;
            var testBrakeLF = new CarTestBrake(WheelLocation.FRONTLEFT, new Range(600, 800, 750), new Range(2.2f, 2.5f),
                                             1.8f, 3600);
            var testBrakeRF = new CarTestBrake(WheelLocation.FRONTRIGHT, new Range(600, 800, 750), new Range(2.2f, 2.5f),
                                             1.8f, 3600);
            var testBrakeLR = new CarTestBrake(WheelLocation.REARLEFT, new Range(600, 800, 750), new Range(2.2f, 2.5f),
                                             1.8f, 3600);
            var testBrakeRR = new CarTestBrake(WheelLocation.REARRIGHT, new Range(600, 800, 750), new Range(2.2f, 2.5f),
                                             1.8f, 3600);

            // Left-front
            car.Assign(testBrakeLF);
            Assert.IsNotNull(car.Brakes);
            Assert.IsNotNull(car.Brakes.ElementAt(0));
            Assert.AreEqual(1, car.Brakes.Count());
            Assert.AreEqual(testBrakeLF, car.Brakes.ElementAt(0));
            try
            {
                car.Assign(testBrakeLF);
            }
            catch (CarAlreadyHasBrakeException ex)
            {
                hasException = true;
            }
            catch(Exception ex)
            {
                Assert.Fail();
            }
            Assert.True(hasException);

            // Right-front
            car.Assign(testBrakeRF);
            Assert.IsNotNull(car.Brakes);
            Assert.IsNotNull(car.Brakes.ElementAt(1));
            Assert.AreEqual(2, car.Brakes.Count());
            Assert.AreEqual(testBrakeRF, car.Brakes.ElementAt(1));
            try
            {
                car.Assign(testBrakeRF);
            }
            catch (CarAlreadyHasBrakeException ex)
            {
                hasException = true;
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
            Assert.True(hasException);

            // Left-rear
            car.Assign(testBrakeLR);
            Assert.IsNotNull(car.Brakes);
            Assert.IsNotNull(car.Brakes.ElementAt(2));
            Assert.AreEqual(3, car.Brakes.Count());
            Assert.AreEqual(testBrakeLR, car.Brakes.ElementAt(2));
            try
            {
                car.Assign(testBrakeLR);
            }
            catch (CarAlreadyHasBrakeException ex)
            {
                hasException = true;
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
            Assert.True(hasException);

            // Right-rear
            car.Assign(testBrakeRR);
            Assert.IsNotNull(car.Brakes);
            Assert.IsNotNull(car.Brakes.ElementAt(3));
            Assert.AreEqual(4, car.Brakes.Count());
            Assert.AreEqual(testBrakeRR, car.Brakes.ElementAt(3));
            try
            {
                car.Assign(testBrakeRR);
            }
            catch (CarAlreadyHasBrakeException ex)
            {
                hasException = true;
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
            Assert.True(hasException);
        }
    }

    internal class CarTestBrake : Brake
    {
        public CarTestBrake(WheelLocation location, Range optimumTemperature, Range thicknessStart, float thicknessFailure, float torque) : base(location, optimumTemperature, thicknessStart, thicknessFailure, torque)
        {
        }
    }

    internal class CarTestWheel : Wheel
    {
        public CarTestWheel(WheelLocation location, float perimeter, float rollResistance, float pitsTemperature, float peakTemperature, float peakPressure, float peakPressureWeightSlope) : base(location, perimeter, rollResistance, pitsTemperature, peakTemperature, peakPressure, peakPressureWeightSlope)
        {
        }
    }

    internal class CarTestDrivetrain : Drivetrain
    {
        public CarTestDrivetrain(float clutchSlipTorque, float clutchFriction, float upshiftTime, float downshiftTime, int gears, IEnumerable<float> gearRatios, IEnumerable<float> finalRatios, IEnumerable<int> stockRatios, int stockFinal, DriveTrainSetup drive, float driveDistribution) : base(clutchSlipTorque, clutchFriction, upshiftTime, downshiftTime, gears, gearRatios, finalRatios, stockRatios, stockFinal, drive, driveDistribution)
        {
        }
    }


    internal class CarTestChassis : Chassis
    {
        public CarTestChassis(float weight, float fuelTankSize, float dragBody, Polynomial dragFrontWing, Polynomial dragRearWing, Polynomial dragRadiator, Polynomial dragBrakeDucts, float rideheightFront) : base(weight, fuelTankSize, dragBody, dragFrontWing, dragRearWing, dragRadiator, dragBrakeDucts, rideheightFront)
        {
        }
    }

    public class CarTestEngine : Engine
    {
        public CarTestEngine(string name, string manufacturer, int cilinders, int displacement, Range idleRpm, Range maximumRpm, IEnumerable<EngineMode> modes, IEnumerable<EngineTorque> torqueCurve, EngineLifetime lifetime) : base(name, manufacturer, cilinders, displacement, idleRpm, maximumRpm, modes, torqueCurve, lifetime)
        {
            // FUMS
        }
    }
}
