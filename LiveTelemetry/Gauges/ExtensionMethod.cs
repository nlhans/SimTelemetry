using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveTelemetry.Gauges
{
    public static class ExtensionMethod
    {
        public static T MinBy<T>(this IEnumerable<T> source, Func<T, float> selector)
        {
            if (source == null || !source.Any()) throw new ArgumentException("mag niet");
            var min = float.MaxValue;
            var result = default(T);

            foreach (var elem in source)
            {
                var value = selector(elem);
                if (value < min)
                {
                    min = value;
                    result = elem;
                }
            }

            return result;
        }
    }
}
