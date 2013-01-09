using System;
using System.Collections.Generic;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class Polynomial : IValueObject<Polynomial>
    {
        public IList<double> Factors { get; private set; }

        public Polynomial(IEnumerable<double> factors)
        {
            Factors = new List<double>(factors);
        }

        public Polynomial(double order0)
        {
            Factors = new List<double> {order0};
        }

        public Polynomial(double order0, double order1)
        {
            Factors = new List<double> {order0, order1};
        }

        public Polynomial(double order0, double order1, double order2)
        {
            Factors = new List<double> {order0, order1, order2};
        }

        public double Get(double x)
        {
            double r = 0;
            int exponent = 0;

            foreach (double c in Factors)
                r += Math.Pow(x, exponent++) * c;

            return r;
        }

        public bool Equals(Polynomial other)
        {
            if (Factors.Count == other.Factors.Count)
            {
                for (int i = 0; i < Factors.Count; i++)
                {
                    if (Factors[i] != other.Factors[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}