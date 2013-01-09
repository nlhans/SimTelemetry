using System;
using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Core.Common;
using SimTelemetry.Core.Entities;
using SimTelemetry.Core.Exceptions;

namespace SimTelemetry.Core.Aggregates
{
    public class Car : IEntity, IEquatable<Car>
    {

        private IList<string> _carClass = new List<string>();
        private IList<Wheel> _wheels = new List<Wheel>();
        private IList<Brake> _brakes = new List<Brake>();

        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Driver { get; private set; }
        public string Description { get; private set; }
        public int StartNumber { get; private set; }
        public IEnumerable<string> CarClass { get { return _carClass; } }

        public Team Team { get; private set; } 

        public Engine Engine { get; private set; }
        public Chassis Chassis { get; private set; }
        public Drivetrain Drivetrain { get; private set; }
        public IEnumerable<Wheel> Wheels { get { return _wheels; } }
        public IEnumerable<Brake> Brakes { get { return _brakes; } }

        public bool Equals(Car other) { return other.ID == ID; }

        public Car(int id, string name, string driver, string description, int startNumber)
        {
            ID = id;
            Name = name;
            Driver = driver;
            Description = description;
            StartNumber = startNumber;
        }

        /********* CAR PARTS ***********/
        public void Assign(Engine engine)
        {
            if (this.Engine == null)
            {
                this.Engine = engine;
            }
            else
            {
                throw new CarAlreadyHasEngineException(this);
            }
        }

        public void Assign(Chassis chassis)
        {
            if (this.Chassis == null)
            {
                this.Chassis = chassis;
            }
            else
            {
                throw new CarAlreadyHasChassisException(this);
            }
        }

        public void Assign(Wheel wheel)
        {
            if (this.Wheels.Any(x => x.Location == wheel.Location) == false)
            {
                this._wheels.Add(wheel);
                this._wheels.OrderBy(x => x.Location);
            }
            else
            {
                throw new CarAlreadyHasWheelException(this);
            }
        }

        public void Assign(Brake brake)
        {
            if (this.Brakes.Any(x => x.Location == brake.Location) == false)
            {
                this._brakes.Add(brake);
                this._brakes.OrderBy(x => x.Location);
            }
            else
            {
                throw new CarAlreadyHasBrakeException(this);
            }
        }


        /********** CAR CLASS *********/
        public void Assign(string cls)
        {
            if (_carClass.Contains(cls) == false)
                _carClass.Add(cls);
        }

        public void Assign(IEnumerable<string> classes)
        {
            foreach (var cls in classes)
            {
                Assign(cls);
            }
        }

        public void Unassign(string cls)
        {
            if (_carClass.Contains(cls))
                _carClass.Remove(cls);
        }

        public void Unassign(IEnumerable<string> classes)
        {
            foreach (var cls in classes)
            {
                Unassign(cls);
            }
        }

        public bool BelongsTo(string cls)
        {
            return CarClass.Any(x => x == cls);
        }

        public bool BelongsTo(IEnumerable<string> cls)
        {
            return (CarClass.Intersect(cls).Count(x => true) > 0);
        }
    }
}
