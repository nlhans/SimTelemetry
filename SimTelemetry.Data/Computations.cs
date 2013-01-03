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
                    try
                    {
                        car = sim.Garage.SearchCar(sim.Drivers.Player.CarClass, sim.Drivers.Player.CarModel);
                    }catch(Exception ex)
                    {

                    }
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

        /// <summary>
        /// Calculates(approximation) the engine power for a given throttle position at a given RPM.
        /// Automatically uses the active sim  + drivers car.
        /// </summary>
        /// <param name="engineRpm">Engine RPM</param>
        /// <param name="pedalsThrottle">Throttle position 0.0-1.0</param>
        /// <returns>Engine power (kW)</returns>
        public double GetPower(double engineRpm, double pedalsThrottle)
        {
            return GetPower(Telemetry.m.Sim, Telemetry.m.Sim.Car, engineRpm, pedalsThrottle);
        }

        /// <summary>
        /// Calculates(approximation) the engine power for a given throttle position at a given RPM
        /// It always needs a combination of both ISimulator and Car. Car value can be null, and it will search for the Player car.
        /// </summary>
        /// <param name="sim">Simulator for the garage plug-in to search in</param>
        /// <param name="car">Direct car to search in</param>
        /// <param name="engineRpm">Engine RPM</param>
        /// <param name="pedalsThrottle">Throttle position 0.0-1.0</param>
        /// <returns>Engine power (kW)</returns>
        public double GetPower(ISimulator sim, ICar car, double engineRpm, double pedalsThrottle)
        {
            if (sim.Garage != null && sim.Garage.Available)
            {
                if (car == null)
                {
                    // Look up car.
                    try
                    {
                        car = sim.Garage.SearchCar(sim.Drivers.Player.CarClass, sim.Drivers.Player.CarModel);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (car == null)
                        return 0;
                    car.ScanEngine();
                    if (car.Engine == null)
                        return 0;
                }

                // We got engine info:
                var engineCurve = car.Engine.GetPowerCurve(0, pedalsThrottle, 0);

                double engineRpmBefore = 0, engineRpmAfter = 0, enginePowerBefore = 0, enginePowerAfter = 0;

                foreach(var engineCurveKvp in engineCurve)
                {
                    if (engineRpmBefore < engineRpm && engineCurveKvp.Key >= engineRpm)
                    {
                        engineRpmAfter = engineCurveKvp.Key;
                        enginePowerAfter = engineCurveKvp.Value;
                        break;
                    }

                    engineRpmBefore = engineCurveKvp.Key;
                    enginePowerBefore = engineCurveKvp.Value;

                }
                if(engineRpmAfter == 0) // didn't find our RPM in the curve:
                    return enginePowerBefore;

                double engineRpmDutyCycle = (engineRpm - engineRpmBefore) / (engineRpmAfter - engineRpmBefore);
                double enginePowerSlope = (enginePowerAfter - enginePowerBefore);

                double enginePower = engineRpmDutyCycle*enginePowerSlope + enginePowerBefore;
                return Power.HP_KW(enginePower);

            }
            else
            {
                return 0;
            }
        }
    }
}
