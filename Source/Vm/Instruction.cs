using System.Collections.Generic;

namespace Xi.Vm
{
	class Instruction
	{
		public Opcode Opcode { get; private set; }
		public List<Variant> Operands { get; private set; }

		public Variant Operand
		{
			get { return Operands[0]; }
		}

		public Instruction(Opcode opcode)
		{
			Opcode = opcode;
			Operands = null;
		}

		public Instruction(Opcode opcode, Variant operand)
		{
			Opcode = opcode;
			Operands = new List<Variant> { operand };
		}

		public Instruction(Opcode opcode, List<Variant> operands)
		{
			Opcode = opcode;
			Operands = operands;
		}

		public static int GetOperandCount(Opcode opcode)
		{
			return new Dictionary<Opcode, int>
			    {
				    { Opcode.NoOperation, 0 },

					{ Opcode.PushNull, 0 },
					{ Opcode.Push, 1 },
					{ Opcode.Pop, 0 },
					{ Opcode.Swap, 0 },
					{ Opcode.Duplicate, 0 },

					{ Opcode.SetVariable, 1 },
					{ Opcode.GetVariable, 1 },
					{ Opcode.GetArgument, 1 },
					{ Opcode.GetThis, 0 },
					{ Opcode.GetBase, 0 },
					{ Opcode.GetBaseOf, 0 },

					{ Opcode.Add, 0 },
					{ Opcode.Subtract, 0 },
					{ Opcode.Multiply, 0 },
					{ Opcode.Divide, 0 },
					{ Opcode.Modulo, 0 },

					{ Opcode.BitwiseNegate, 0 },
					{ Opcode.BitwiseNot, 0 },
					{ Opcode.BitwiseAnd, 0 },
					{ Opcode.BitwiseOr, 0 },
					{ Opcode.BitwiseXor, 0 },
					{ Opcode.BitwiseShiftLeft, 0 },
					{ Opcode.BitwiseShiftRight, 0 },

					{ Opcode.IncrementVariable, 2 },
					{ Opcode.IncrementField, 2 },

					{ Opcode.Compare, 0 },
					{ Opcode.IfEqual, 1 },
					{ Opcode.IfNotEqual, 1 },
					{ Opcode.IfLessThan, 1 },
					{ Opcode.IfGreaterThan, 1 },
					{ Opcode.IfLessThanOrEqual, 1 },
					{ Opcode.IfGreaterThanOrEqual, 1 },
					{ Opcode.Jump, 1 },
					{ Opcode.Return, 0 },

					{ Opcode.ClassSetFieldStatic, 2 },
					{ Opcode.ClassGetFieldStatic, 2 },

					{ Opcode.ClassSetField, 1 },
					{ Opcode.ClassGetField, 1 },

					{ Opcode.ClassCall, 1 },
					{ Opcode.ClassCallStatic, 2 },
					{ Opcode.ClassCallVirtual, 1 },

					{ Opcode.New, 1 },

					{ Opcode.CastVariant, 1 },

					{ Opcode.SetGlobalVariable, 1 },
					{ Opcode.GetGlobalVariable, 1 },

					{ Opcode.Print, 0 },
					{ Opcode.Breakpoint, 0 }
			    }[opcode];
		}
	}

	enum Opcode : byte
	{
		NoOperation,

		/* Stack */
		PushNull,
		Push,
		Pop,
		Swap,
		Duplicate,

		/* Stack (Frames) */
		SetVariable,
		GetVariable,
		GetArgument,
		GetThis,
		GetBase,
		GetBaseOf,

		/* Arithmetics */
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulo,

		/* Binary Arithmetics */
		LogicalNegate,
		BitwiseNegate,
		BitwiseNot,
		BitwiseAnd,
		BitwiseOr,
		BitwiseXor,
		BitwiseShiftLeft,
		BitwiseShiftRight,

		/* Miscellaneous Arithmetics */
		IncrementVariable,
		IncrementField,

		/* Execution Flow */
		Compare,
		IfEqual,
		IfNotEqual,
		IfLessThan,
		IfGreaterThan,
		IfLessThanOrEqual,
		IfGreaterThanOrEqual,
		Jump,
		Return,

		/* Classes */
		ClassSetFieldStatic,
		ClassGetFieldStatic,

		ClassSetField,
		ClassGetField,

		ClassCall,
		ClassCallStatic,
		ClassCallVirtual,

		/* Memory */
		New,

		/* Variant conversion */
		CastVariant,

		/* C# 4.0 Integration */
		SetGlobalVariable,
		GetGlobalVariable,

		/* Debugging */
		Print,
		Breakpoint
	}
}
