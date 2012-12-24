using System;
using System.Collections.Generic;

namespace Xi.Vm
{
	class Variant
	{
		public enum VariantType
		{
			Int64,
			Double,
			String,
			Object,
			Array,
			Nill
		}

		public VariantType Type { get; private set; }
		public Int64 IntValue;
		public double DoubleValue;
		public string StringValue;
		public object ObjectValue;
		public List<Variant> ArrayValue;

		public Variant()
		{
			Type = VariantType.Nill;
		}

		public Variant(Int64 value)
		{
			Type = VariantType.Int64;
			IntValue = value;
		}

		public Variant(double value)
		{
			Type = VariantType.Double;
			DoubleValue = value;
		}

		public Variant(string value)
		{
			Type = VariantType.String;
			StringValue = value;
		}

		public Variant(object value)
		{
			Type = VariantType.Object;
			ObjectValue = value;
		}

		public Variant(List<Variant> value)
		{
			Type = VariantType.Array;
			ArrayValue = value;
		}

		public override string ToString()
		{
			switch (Type)
			{
				case VariantType.Int64:
					return IntValue.ToString("G");

				case VariantType.Double:
					return DoubleValue.ToString("F");

				case VariantType.String:
					return StringValue;

				case VariantType.Object:
					return ObjectValue.ToString();

				case VariantType.Array:
					return "array";
			}

			return "nill";
		}

		public override bool Equals(object obj)
		{
			return obj.Equals(this);
		}

		public override int GetHashCode()
		{
			return (int)DateTime.Now.Ticks;
		}

		public static Variant operator +(Variant a, Variant b)
		{
			if (a.Type != b.Type)
				throw new Exception(String.Format("Variable types do not match ({0} != {1})", a.Type, b.Type));

			switch (a.Type)
			{
				case VariantType.Int64:
					return new Variant(a.IntValue + b.IntValue);

				case VariantType.Double:
					return new Variant(a.DoubleValue + b.DoubleValue);

				case VariantType.String:
					return new Variant(a.StringValue + b.StringValue);

				case VariantType.Array:
					{
						var c = new List<Variant>();
						c.AddRange(a.ArrayValue);
						c.AddRange(b.ArrayValue);

						return new Variant(c);
					}
			}

			throw new Exception(String.Format("Cannot + variant type \"{0}\"", a.Type));
		}

		public static Variant operator -(Variant a, Variant b)
		{
			if (a.Type != b.Type)
				throw new Exception(String.Format("Variable types do not match ({0} != {1})", a.Type, b.Type));

			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue - b.IntValue);

			if (a.Type == VariantType.Double)
				return new Variant(a.DoubleValue - b.DoubleValue);

			throw new Exception(String.Format("Cannot - variant type \"{0}\"", a.Type));
		}

		public static Variant operator *(Variant a, Variant b)
		{
			if (a.Type != b.Type)
				throw new Exception(String.Format("Variable types do not match ({0} != {1})", a.Type, b.Type));

			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue * b.IntValue);

			if (a.Type == VariantType.Double)
				return new Variant(a.DoubleValue * b.DoubleValue);

			throw new Exception(String.Format("Cannot * variant type \"{0}\"", a.Type));
		}

		public static Variant operator /(Variant a, Variant b)
		{
			if (a.Type != b.Type)
				throw new Exception(String.Format("Variable types do not match ({0} != {1})", a.Type, b.Type));

			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue / b.IntValue);

			if (a.Type == VariantType.Double)
				return new Variant(a.DoubleValue / b.DoubleValue);

			throw new Exception(String.Format("Cannot / variant type \"{0}\"", a.Type));
		}

		public static Variant operator %(Variant a, Variant b)
		{
			if (a.Type != b.Type)
				throw new Exception(String.Format("Variable types do not match ({0} != {1})", a.Type, b.Type));

			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue % b.IntValue);

			if (a.Type == VariantType.Double)
				return new Variant(a.DoubleValue % b.DoubleValue);

			throw new Exception(String.Format("Cannot % variant type \"{0}\"", a.Type));
		}

		public static Variant operator ~(Variant a)
		{
			if (a.Type == VariantType.Int64)
				return new Variant(~a.IntValue);

			throw new Exception(String.Format("Cannot ~ variant type \"{0}\"", a.Type));
		}

		public static Variant operator !(Variant a)
		{
			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue == 0 ? 1 : 0);

			throw new Exception(String.Format("Cannot ! variant type \"{0}\"", a.Type));
		}

		public static Variant operator &(Variant a, Variant b)
		{
			if (a.Type != b.Type)
				throw new Exception(String.Format("Variable types do not match ({0} != {1})", a.Type, b.Type));

			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue & b.IntValue);

			throw new Exception(String.Format("Cannot & variant type \"{0}\"", a.Type));
		}

		public static Variant operator |(Variant a, Variant b)
		{
			if (a.Type != b.Type)
				throw new Exception(String.Format("Variable types do not match ({0} != {1})", a.Type, b.Type));

			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue | b.IntValue);

			throw new Exception(String.Format("Cannot | variant type \"{0}\"", a.Type));
		}

		public static Variant operator ^(Variant a, Variant b)
		{
			if (a.Type != b.Type)
				throw new Exception(String.Format("Variable types do not match ({0} != {1})", a.Type, b.Type));

			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue ^ b.IntValue);

			throw new Exception(String.Format("Cannot ^ variant type \"{0}\"", a.Type));
		}

		public static Variant operator >>(Variant a, int b)
		{
			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue >> b);

			throw new Exception(String.Format("Cannot >> variant type \"{0}\"", a.Type));
		}

		public static Variant operator <<(Variant a, int b)
		{
			if (a.Type == VariantType.Int64)
				return new Variant(a.IntValue << b);

			throw new Exception(String.Format("Cannot << variant type \"{0}\"", a.Type));
		}

		public static bool operator <(Variant a, Variant b)
		{
			if (a.Type == VariantType.Int64)
				return a.IntValue < b.IntValue;

			if (a.Type == VariantType.Double)
				return a.DoubleValue < b.DoubleValue;

			throw new Exception(String.Format("Cannot < variant type \"{0}\"", a.Type));
		}

		public static bool operator >(Variant a, Variant b)
		{
			if (a.Type == VariantType.Int64)
				return a.IntValue > b.IntValue;

			if (a.Type == VariantType.Double)
				return a.DoubleValue > b.DoubleValue;

			throw new Exception(String.Format("Cannot < variant type \"{0}\"", a.Type));
		}

		public static int operator ==(Variant a, Variant b)
		{
			if (a > b)
				return 1;

			if (a < b)
				return -1;

			return 0;
		}

		public static int operator !=(Variant a, Variant b)
		{
			if (a > b)
				return -1;

			if (a < b)
				return 1;

			return 1;
		}
	}
}
