using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Xi.Vm
{
	public class Variant
	{
		public static double Epsilon = Double.Epsilon;

		private readonly long timestamp;

		public VariantType Type { get; private set; }
		public Int64 IntValue;
		public double DoubleValue;
		public string StringValue;
		public object ObjectValue;
		public List<Variant> ArrayValue;

		public int Length
		{
			get
			{
				switch(Type)
				{
					case VariantType.String:
						return StringValue.Length;

					case VariantType.Double:
						return 8;

					case VariantType.Int64:
						return 8;

					case VariantType.Nill:
						return 0;

					case VariantType.Array:
						return ArrayValue.Count;

					case VariantType.Object:
						return 8;
				}

				return 0;
			}
		}

		public Variant this[int index]
		{
			get
			{
				if (Type != VariantType.Array)
					throw new Exception(String.Format("Cannot index type \"{0}\"", Type));

				return ArrayValue[index];
			}

			set
			{
				if (Type != VariantType.Array)
					throw new Exception(String.Format("Cannot index type \"{0}\"", Type));

				ArrayValue[index] = value;
			}
		}

		public Variant()
		{
			timestamp = DateTime.Now.Ticks;
			Type = VariantType.Nill;
		}

		public Variant(Int64 value)
		{
			timestamp = DateTime.Now.Ticks;
			Type = VariantType.Int64;
			IntValue = value;
		}

		public Variant(double value)
		{
			timestamp = DateTime.Now.Ticks;
			Type = VariantType.Double;
			DoubleValue = value;
		}

		public Variant(string value)
		{
			timestamp = DateTime.Now.Ticks;
			Type = VariantType.String;
			StringValue = value;
		}

		public Variant(object value)
		{
			timestamp = DateTime.Now.Ticks;
			Type = VariantType.Object;
			ObjectValue = value;
		}

		public Variant(List<Variant> value)
		{
			timestamp = DateTime.Now.Ticks;
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
			int hashCode = 0x7FFFFFFF ^ (int)timestamp;

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

		public static Variant operator -(Variant a)
		{
			if (a.Type == VariantType.Int64)
				return new Variant(-a.IntValue);

			if (a.Type == VariantType.Double)
				return new Variant(-a.DoubleValue);

			throw new Exception(String.Format("Cannot unary - variant type \"{0}\"", a.Type));
		}

		public static Variant operator +(Variant a)
		{
			if (a.Type == VariantType.Int64)
				return new Variant(Math.Abs(a.IntValue));

			if (a.Type == VariantType.Double)
				return new Variant(Math.Abs(a.DoubleValue));

			throw new Exception(String.Format("Cannot unary + variant type \"{0}\"", a.Type));
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

			throw new Exception(String.Format("Cannot > variant type \"{0}\"", a.Type));
		}

		public static bool operator <=(Variant a, Variant b)
		{
			if (a.Type == VariantType.Int64)
				return a.IntValue <= b.IntValue;

			if (a.Type == VariantType.Double)
				return a.DoubleValue <= b.DoubleValue;

			throw new Exception(String.Format("Cannot <= variant type \"{0}\"", a.Type));
		}

		public static bool operator >=(Variant a, Variant b)
		{
			if (a.Type == VariantType.Int64)
				return a.IntValue >= b.IntValue;

			if (a.Type == VariantType.Double)
				return a.DoubleValue >= b.DoubleValue;

			throw new Exception(String.Format("Cannot >= variant type \"{0}\"", a.Type));
		}

		public static bool operator ==(Variant a, Variant b)
		{
			if (((object)a) == null || ((object)b) == null)
				throw new ArgumentException("Cannot compare null variant.");

			if (a.Type != b.Type)
				throw new Exception(String.Format("Cannot compare variants of differing types \"{0}\" and \"{1}\"", a.Type, b.Type));

			switch (a.Type)
			{
				case VariantType.Int64:
					return a.IntValue == b.IntValue;

				case VariantType.Double:
					return Math.Abs(a.DoubleValue - b.DoubleValue) < Epsilon;

				case VariantType.String:
					return a.StringValue == b.StringValue;

				case VariantType.Array:
				case VariantType.Object:
					return a.Equals(b);
			}

			return false;
		}

		public static bool operator !=(Variant a, Variant b)
		{
			if (((object)a) == null || ((object)b) == null)
				throw new ArgumentException("Cannot compare null variant.");

			if (a.Type != b.Type)
				throw new Exception(String.Format("Cannot compare variants of differing types \"{0}\" and \"{1}\"", a.Type, b.Type));

			switch (a.Type)
			{
				case VariantType.Int64:
					return a.IntValue != b.IntValue;

				case VariantType.Double:
					return !(Math.Abs(a.DoubleValue - b.DoubleValue) < Epsilon);

				case VariantType.String:
					return a.StringValue != b.StringValue;

				case VariantType.Array:
				case VariantType.Object:
					return !a.Equals(b);
			}

			return false;
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
