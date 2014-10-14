﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Utils
{
    public class IniValueObject
    {
        public IEnumerable<string> NestedGroup { get; private set; }

        public string Group
        {
            get { return NestedGroup.ElementAt(NestedGroup.Count() - 1); }
        }

        public string NestedGroupName
        {
            get { return string.Join(".", NestedGroup); }
        }

        public string Key { get; private set; }
        public string RawValue { get; private set; }

        public int ValueCount
        {
            get { return !IsTuple ? 1 : ValueArray.Count(); }
        }

        protected string Value { get; private set; }
        protected string[] ValueArray { get; private set; }
        public bool IsTuple { get; private set; }

        public IniValueObject(IEnumerable<string> nestedGroup, string key, string rawValue)
        {
            NestedGroup = nestedGroup;
            Key = key;
            RawValue = rawValue;

            var value = rawValue;

            // Does this rawValue contain multiple values?
            if (value.StartsWith("(") && value.EndsWith(")", StringComparison.Ordinal) && value.Length > 2)
                value = value.Substring(1, value.Length - 2);
            if (value.StartsWith("\"") && value.EndsWith("\"", StringComparison.Ordinal) && value.Length > 2)
                value = value.Substring(1, value.Length - 2);

            if (value.Contains(","))
            {
                IsTuple = true;

                var values = value.Split(new[] {','});
                ValueArray = new string[values.Length];

                for (var i = 0; i < values.Length; i++)
                {
                    var val = values[i];
                    if (val.StartsWith("\"") && val.EndsWith("\"", StringComparison.Ordinal) && val.Length > 2)
                        val = val.Substring(1, val.Length - 2);

                    ValueArray[i] = val.Trim();

                }
            }
            else
            {

                IsTuple = false;
                Value = value;
            }
        }

        public bool BelongsTo(string group)
        {
            return NestedGroup.Contains(group);
        }

        public int ReadAsInteger(int index)
        {
            if (!IsTuple && index == 0) return Convert.ToInt32(Value);
            if (!IsTuple) throw new Exception("This is not a tuple value");
            try
            {
                return Convert.ToInt32(ReadAsDouble(index));
            }
            catch
            {
                return -1;
            }
            //return int.Parse(ValueArray[index]);
        }

        public double ReadAsDouble(int index)
        {
            if (!IsTuple && index == 0) return double.Parse(Value);
            if (!IsTuple) throw new Exception("This is not a tuple value");
            try
            {
                return double.Parse(ValueArray[index]);
            }
            catch
            {
                return -1;
            }
        }

        public float ReadAsFloat(int index)
        {
            if (!IsTuple && index == 0) return float.Parse(Value);
            if (!IsTuple) throw new Exception("This is not a tuple value");
            try
            {
                return float.Parse(ValueArray[index]);
            }
            catch
            {
                return -1;
            }
        }

        public string ReadAsString(int index)
        {
            if (!IsTuple && index == 0) return Value;
            if (!IsTuple) throw new Exception("This is not a tuple value");
            return ValueArray[index];
        }

        public string ReadAsString()
        {
            return IsTuple ? ReadAsString(0) : Value;
        }

        public int ReadAsInteger()
        {
            try
            {
                return IsTuple ? ReadAsInteger(0) : int.Parse(Value);
            }
            catch
            {
                return -1;
            }
        }

        public double ReadAsDouble()
        {
            try
            {
                return IsTuple ? ReadAsDouble(0) : double.Parse(Value);
            }
            catch
            {
                return -1;
            }
        }


        public float ReadAsFloat()
        {
            try
            {
                return IsTuple ? ReadAsFloat(0) : float.Parse(Value);
            }
            catch
            {
                return -1;
            }
        }

        public IEnumerable<string> ReadAsStringArray()
        {
            return IsTuple ? ValueArray : new string[] {Value};
        }
    }
}