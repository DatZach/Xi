using System.Collections.Generic;

namespace Xi.Vm
{
	public class Instruction
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
	}

	public enum Opcode : byte
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
		SetArrayVariable,
		GetArrayVariable,
		GetVariableLength,
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
		AbsoluteValue,

		/* Execution Flow */
		ModuleCall,

		CompareEqual,
		CompareNotEqual,
		CompareGreaterThan,
		CompareLesserThan,
		CompareGreaterThanOrEqual,
		CompareLesserThanOrEqual,
		IfTrue,
		IfFalse,
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
