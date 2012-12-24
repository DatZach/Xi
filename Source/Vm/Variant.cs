using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Xi.Vm
{
	class Variant
	{
		public VariantType Type { get; private set; }
		public Int64 IntValue;
		public double DoubleValue;
		public string StringValue;
		public Class ObjectValue;
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

		public Variant(Class value)
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
					return "object";

				case VariantType.Array:
					{
						StringBuilder builder = new StringBuilder();
						builder.Append("[ ");

						for (int i = 0; i < ArrayValue.Count; ++i)
						{
							builder.Append(ArrayValue[i]);

							if (i + 1 != ArrayValue.Count)
								builder.Append(", ");
						}

						builder.Append(" ]");

						return builder.ToString();
					}
			}

			return "nill";
		}

		public override bool Equals(object obj)
		{
			return obj.Equals(this);
		}

		public override int GetHashCode()
		{
			int hashCode = 0x7FFFFFFF;

			switch (Type)
			{
				case VariantType.Int64:
					hashCode = (int)IntValue;
					break;

				case VariantType.Double:
					hashCode = (int)DoubleValue;
					break;

				case VariantType.String:
					hashCode = StringValue.GetHashCode();
					break;

				case VariantType.Array:
					hashCode = ArrayValue.GetHashCode();
					break;

				case VariantType.Object:
					hashCode = ObjectValue.GetHashCode();
					break;
			}

			return hashCode ^ ((int)Type << 8) ^ ((int)Type << 6) ^ ((int)Type << 4) ^ ((int)Type << 2);
		}

		public Variant Cast(VariantType type)
		{
			switch (Type)
			{
				case VariantType.Int64:
					if (type == VariantType.String)
						return new Variant(ToString());

					if (type == VariantType.Double)
						return new Variant((double)IntValue);

					break;

				case VariantType.Double:
					if (type == VariantType.String)
						return new Variant(ToString());

					if (type == VariantType.Int64)
						return new Variant((Int64)DoubleValue);

					break;

				case VariantType.String:
					if (type == VariantType.Int64)
						return new Variant(Int64.Parse(StringValue, NumberStyles.Number, CultureInfo.InvariantCulture));

					if (type == VariantType.Double)
						return new Variant(Double.Parse(StringValue, NumberStyles.Number, CultureInfo.InvariantCulture));

					break;
			}

			throw new Exception(String.Format("Cannot cast type \"{0}\" to type \"{1}\"!", Type, type));
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
			if (a.Type == VariantType.String && b.Type == VariantType.Int64)
			{
				StringBuilder builder = new StringBuilder();
				for (Int64 i = 0; i < b.IntValue; ++i)
					builder.Append(a.StringValue);

				return new Variant(builder.ToString());
			}

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

	public enum VariantType
	{
		Int64,
		Double,
		String,
		Object,
		Array,
		Nill
	}
}
