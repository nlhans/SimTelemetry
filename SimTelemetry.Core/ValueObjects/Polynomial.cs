using System;
using System.Collections.Generic;

namespace SimTelemetry.Core.ValueObjects
{
    public class Polynomial
    {
        public List<double> Factors { get; private set; }

        public Polynomial(List<double> factors)
        {
            Factors = factors;
        }

        public Polynomial(double order0)
        {
            Factors = new List<double>();
            Factors.Add(order0);
        }

        public Polynomial(double order0, double order1)
        {
            Factors = new List<double>();
            Factors.Add(order0);
            Factors.Add(order1);
        }

        public Polynomial(double order0, double order1, double order2)
        {
            Factors = new List<double>();
            Factors.Add(order0);
            Factors.Add(order1);
            Factors.Add(order2);
        }

        public double Get(double x)
        {
            double r = 0;
            int exponent = 0;

            foreach (double c in Factors)
                r += Math.Pow(x, exponent++) * c;

            return r;
        }
    }
}