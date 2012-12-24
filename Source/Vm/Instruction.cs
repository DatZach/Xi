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

		public static bool OpcodeHasOperands(Opcode opcode)
		{
			return new List<Opcode>
			    {
				    Opcode.Push,
					Opcode.SetVariable,
					Opcode.GetVariable,
					Opcode.IncrementVariable,
					Opcode.IncrementField,
					Opcode.IfEqual,
					Opcode.IfNotEqual,
					Opcode.IfLessThan,
					Opcode.IfGreaterThan,
					Opcode.IfLessThanOrEqual,
					Opcode.IfGreaterThanOrEqual,
					Opcode.Jump,
					Opcode.ClassSetFieldStatic,
					Opcode.ClassGetFieldStatic,
					Opcode.ClassSetField,
					Opcode.ClassGetField,
					Opcode.ClassCall,
					Opcode.ClassCallStatic,
					Opcode.ClassCallVirtual,
					Opcode.NewClass,
					Opcode.New,
					Opcode.CastVariant,
					Opcode.SetGlobalVariable,
					Opcode.GetGlobalVariable
			    }.Contains(opcode);
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
		Negate,
		Not,
		And,
		Or,
		Xor,
		ShiftLeft,
		ShiftRight,

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
		NewClass,
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
