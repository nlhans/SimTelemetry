using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects.Garage
{
    public class Polynomial
    {
        public List<double> factors;

        public Polynomial(List<double> factors)
        {
            this.factors = factors;
        }

        public Polynomial(double order0)
        {
            this.factors = new List<double>();
            this.factors.Add(order0);
        }

        public Polynomial(double order0, double order1)
        {
            this.factors = new List<double>();
            this.factors.Add(order0);
            this.factors.Add(order1);
        }

        public Polynomial(double order0, double order1, double order2)
        {
            this.factors = new List<double>();
            this.factors.Add(order0);
            this.factors.Add(order1);
            this.factors.Add(order2);
        }

        public double Calculate(double x)
        {
            double r = 0;
            int exponent = 0;

            foreach(double c in factors)
                r += Math.Pow(x, exponent++)*c;

            return r;
        }
    }
}
