using System;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Data
{
    public class Computations
    {

        private const double TopSpeed_Stock = 360;

        public double TopSpeed()
        {
            return TopSpeed(Telemetry.m.Sim, null);
        }

        /// <summary>
        /// Returns power in HP
        /// </summary>
        /// <param name="sim"></param>
        /// <param name="car"></param>
        /// <returns></returns>
        public double MaximumPower(ISimulator sim, ICar car)
        {
            if (sim.Garage == null)
                return 0;

            if (car == null)
            {
                // Look up car.
                car = sim.Garage.SearchCar(sim.Drivers.Player.CarClass, sim.Drivers.Player.CarModel);

                if (car == null)
                    return 0;

                car.ScanEngine();

                if (car.Engine == null )
                    return 0;
            }

            return car.Engine.GetMaximumPower();
        }

        /// <summary>
        /// Returns top speed in km/h
        /// </summary>
        /// <param name="sim"></param>
        /// <param name="car"></param>
        /// <returns></returns>
        public double TopSpeed(ISimulator sim, ICar car)
        {
            if (sim.Garage != null && sim.Garage.Available)
            {
                if (car == null)
                {
                    // Look up car.
                    car = sim.Garage.SearchCar(sim.Drivers.Player.CarClass, sim.Drivers.Player.CarModel);

                    if (car == null)
                        return TopSpeed_Stock;

                    car.ScanAerodynamics();
                    car.ScanEngine();

                    if (car.Engine == null || car.Aerodynamics == null)
                        return TopSpeed_Stock;
                }

                double power = MaximumPower(sim, car);
                double aero = car.Aerodynamics.GetAerodynamicDrag(sim.Setup);

                return 3.6 * Math.Pow(power * 1000.0 / aero, 1 / 3.0);
            }
            else
            {
                return TopSpeed_Stock;
            }
        }
    }
}
