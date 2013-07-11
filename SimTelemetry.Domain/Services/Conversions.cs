using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Domain.Services
{
    public class Limits
    {
        public static double Clamp(double val, double min, double max)
        {
            return Math.Max(min, Math.Min(max, val));
        }

        public static float Clamp(float val, float min, float max)
        {
            return Math.Max(min, Math.Min(max, val));
        }

        public static int Clamp(int val, int min, int max)
        {
            return Math.Max(min, Math.Min(max, val));
        }

        public static int uClamp(int val, int max)
        {
            return Math.Min(val, max);
        }

        public static double uClamp(double val, double max)
        {
            return Math.Min(val, max);
        }

        public static float uClamp(float val, float max)
        {
            return Math.Min(val, max);
        }

        public static int lClamp(int val, int min)
        {
            return Math.Max(val, min);
        }

        public static double lClamp(double val, double min)
        {
            return Math.Max(val, min);
        }

        public static float lClamp(float val, float min)
        {
            return Math.Max(val, min);
        }

    }

    public class Rotations
    {
        public static double Rads_RPM(double rads)
        {
            return rads * 30 / Math.PI; // * 60 / 2 = *30
        }

        public static double RPM_Rads(double rpm)
        {
            return rpm / 30 * Math.PI; // / 60 * 2 = /30
        }

        public static double RPM_RPS(double rpm)
        {
            return rpm / 60;
        }

        public static double RPS_RPM(double rps)
        {
            return rps * 60;

        }

        public static float Rads_RPM(float rads)
        {
            return rads * 30f / Convert.ToSingle(Math.PI); // * 60 / 2 = *30
        }

        public static float RPM_Rads(float rpm)
        {
            return rpm / 30f * Convert.ToSingle(Math.PI);  // / 60 * 2 = /30
        }

        public static float RPM_RPS(float rpm)
        {
            return rpm / 60f;
        }

        public static float RPS_RPM(float rps)
        {
            return rps * 60f;
        }
    }

    public class Power
    {
        public static double HP_KW(double HP)
        {
            return HP * 0.735499;
        }

        public static double BHP_KW(double BHP)
        {
            return BHP * 0.7457;
        }

        public static double HP_W(double HP)
        {
            return HP * 735.499;
        }

        public static double BHP_W(double BHP)
        {
            return BHP * 745.7;
        }

        public static double KW_HP(double KW)
        {
            return KW / 0.735499;
        }

        public static double KW_BHP(double BHP)
        {
            return BHP / 0.7457;
        }

        public static double W_HP(double W)
        {
            return W / 735.499;
        }

        public static double W_BHP(double W)
        {
            return W / 745.7;
        }
        public static float HP_KW(float HP)
        {
            return HP * 0.735499f;
        }

        public static float BHP_KW(float BHP)
        {
            return BHP * 0.7457f;
        }

        public static float HP_W(float HP)
        {
            return HP * 735.499f;
        }

        public static float BHP_W(float BHP)
        {
            return BHP * 745.7f;
        }

        public static float KW_HP(float KW)
        {
            return KW / 0.735499f;
        }

        public static float KW_BHP(float BHP)
        {
            return BHP / 0.7457f;
        }

        public static float W_HP(float W)
        {
            return W / 735.499f;
        }

        public static float W_BHP(float W)
        {
            return W / 745.7f;
        }
    }

    public class Temperature
    {
        public static double CK(double Celcius)
        {
            return Celcius + 273.15;
        }

        public static double CF(double Celcius)
        {
            return Celcius * 9 / 5 + 32;
        }

        public static double KC(double Kelvin)
        {
            return Kelvin - 273.15;
        }

        public static double FC(double Fahrenheit)
        {
            return (Fahrenheit - 32) * 5 / 9;
        }

        public static double FK(double Fahrenheit)
        {
            return (Fahrenheit + 459.69) * 5 / 9;
        }

        public static double KF(double Kelvin)
        {
            return Kelvin * 9 / 5 - 459.67;
        }

        public static float CK(float Celcius)
        {
            return Celcius + 273.15f;
        }

        public static float CF(float Celcius)
        {
            return Celcius * 9f / 5f + 32f;
        }

        public static float KC(float Kelvin)
        {
            return Kelvin - 273.15f;
        }

        public static float FC(float Fahrenheit)
        {
            return (Fahrenheit - 32f) * 5f / 9f;
        }

        public static float FK(float Fahrenheit)
        {
            return (Fahrenheit + 459.69f) * 5f / 9f;
        }

        public static float KF(float Kelvin)
        {
            return Kelvin * 9f / 5f - 459.67f;
        }
    }

    public class Distance
    {
        public static double KM_Mi(double KM)
        {
            return KM / 1.609344;
        }

        public static double Mi_KM(double Mi)
        {
            return Mi * 1.609344;
        }

        public static double KM_M(double KM)
        {
            return KM / 1000;
        }

        public static double M_KM(double M)
        {
            return M / 1000.0;
        }

        public static double M_Mi(double M)
        {
            return M / 1609.344;
        }

        public static double Mi_M(double Mi)
        {
            return Mi * 1609.344;
        }

        public static double Perimeter(double radius)
        {
            return 2 * radius * Math.PI;
        }

        public static float KM_Mi(float KM)
        {
            return KM / 1.609344f;
        }

        public static float Mi_KM(float Mi)
        {
            return Mi * 1.609344f;
        }

        public static float KM_M(float KM)
        {
            return KM / 1000f;
        }

        public static float M_KM(float M)
        {
            return M / 1000.0f;
        }

        public static float M_Mi(float M)
        {
            return M / 1609.344f;
        }

        public static float Mi_M(float Mi)
        {
            return Mi * 1609.344f;
        }

        public static float Perimeter(float radius)
        {
            return 2f * radius * Convert.ToSingle(Math.PI);
        }
    }

    public class Speed
    {
        public static double KPH_MPH(double KPH)
        {
            return KPH / 1.609344;
        }

        public static double MPH_KPH(double MPH)
        {
            return MPH * 1.609344;
        }

        public static double KPH_MS(double KPH)
        {
            return KPH / 3.6;
        }

        public static double MPH_MS(double MPH)
        {
            return MPH * 0.44704;
        }

        public static double MS_KPH(double MS)
        {
            return MS * 3.6;
        }

        public static double MS_MPH(double MS)
        {
            return MS / 0.44704;
        }
        public static float KPH_MPH(float KPH)
        {
            return KPH / 1.609344f;
        }

        public static float MPH_KPH(float MPH)
        {
            return MPH * 1.609344f;
        }

        public static float KPH_MS(float KPH)
        {
            return KPH / 3.6f;
        }

        public static float MPH_MS(float MPH)
        {
            return MPH * 0.44704f;
        }

        public static float MS_KPH(float MS)
        {
            return MS * 3.6f;
        }

        public static float MS_MPH(float MS)
        {
            return MS / 0.44704f;
        }
    }
}
