using System;
using System.Collections.Generic;
using System.Linq;

namespace Xi.Vm
{
	class VirtualMachine
	{
		public List<Class> Classes { get; private set; }

		public VirtualMachine()
		{
			Classes = new List<Class>();
		}

		public void Execute(State state)
		{
			List<Instruction> stream = state.CurrentMethod.Instructions;

			// Prepare stack
			state.Stack.PushScope(state.CurrentMethod.VariableCount);

			while (state.InstructionPointer < stream.Count)
			{
				Instruction instruction = stream[state.InstructionPointer];

				switch (instruction.Opcode)
				{
					case Opcode.PushNull:
						state.Stack.Push(new Variant());
						break;

					case Opcode.Push:
						state.Stack.Push(instruction.Operand);
						break;

					case Opcode.Pop:
						state.Stack.Pop();
						break;

					case Opcode.Swap:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();
							state.Stack.Push(a);
							state.Stack.Push(b);
							break;
						}

					case Opcode.Duplicate:
						state.Stack.Push(state.Stack.Top);
						break;

					case Opcode.SetVariable:
						state.Stack[(int)instruction.Operand.IntValue] = state.Stack.Pop();
						break;

					case Opcode.GetVariable:
						state.Stack.Push(state.Stack[(int)instruction.Operand.IntValue]);
						break;

					case Opcode.GetArgument:
						/*
						 * May not be needed?
						 * The stack should remain unclobbered between calls
						 * The CallStack should contain all callback information
						 * So arguments can be pushed onto operand stack
						 */
						break;

					case Opcode.GetThis:
						state.Stack.Push(new Variant(state.CurrentClass));
						break;

					case Opcode.GetBase:
						state.Stack.Push(new Variant(state.CurrentClass.Base));
						break;

					case Opcode.GetBaseOf:
						state.Stack.Push(new Variant(((Class)state.Stack.Pop().ObjectValue).Base));
						break;

					case Opcode.Add:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b + a);
							break;
						}

					case Opcode.Subtract:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b - a);
							break;
						}

					case Opcode.Multiply:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b * a);
							break;
						}

					case Opcode.Divide:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b / a);
							break;
						}

					case Opcode.Modulo:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b % a);
							break;
						}

					case Opcode.Negate:
						state.Stack.Push(~state.Stack.Pop());
						break;

					case Opcode.Not:
						state.Stack.Push(!state.Stack.Pop());
						break;

					case Opcode.And:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b & a);
							break;
						}

					case Opcode.Or:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b | a);
							break;
						}

					case Opcode.Xor:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b ^ a);
							break;
						}

					case Opcode.ShiftLeft:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b << (int)a.IntValue);
							break;
						}

					case Opcode.ShiftRight:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b >> (int)a.IntValue);
							break;
						}

					case Opcode.IncrementVariable:
						{
							// SetVar(*(unsigned short*)instr->buffer.data,
							//			GetVar(*(unsigned short*)instr->buffer.data) +
							//					*(short*)(instr->buffer.data + 4));
							Variant a = state.Stack[(int)instruction.Operands[0].IntValue];
							a += instruction.Operands[1];
							state.Stack[(int)instruction.Operands[0].IntValue] = a;
							break;
						}

					case Opcode.IncrementField:
						break;

					case Opcode.Compare:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(new Variant(b == a));
							break;
						}

					case Opcode.IfEqual:
						if (state.Stack.Pop().IntValue == 0)
							state.InstructionPointer = (int)instruction.Operand.IntValue;

						break;

					case Opcode.IfNotEqual:
						if (state.Stack.Pop().IntValue != 0)
							state.InstructionPointer = (int)instruction.Operand.IntValue;

						break;

					case Opcode.IfLessThan:
						if (state.Stack.Pop().IntValue == -1)
							state.InstructionPointer = (int)instruction.Operand.IntValue;

						break;

					case Opcode.IfGreaterThan:
						if (state.Stack.Pop().IntValue == 1)
							state.InstructionPointer = (int)instruction.Operand.IntValue;

						break;

					case Opcode.IfLessThanOrEqual:
						{
							Variant a = state.Stack.Pop();
							if (a.IntValue == -1 || a.IntValue == 0)
								state.InstructionPointer = (int)instruction.Operand.IntValue;

							break;
						}

					case Opcode.IfGreaterThanOrEqual:
						{
							Variant a = state.Stack.Pop();
							if (a.IntValue == 1 || a.IntValue == 0)
								state.InstructionPointer = (int)instruction.Operand.IntValue;

							break;
						}

					case Opcode.Jump:
						state.InstructionPointer = (int)instruction.Operand.IntValue;
						break;

					case Opcode.Return:
						// Primitive return for now
						state.CallStack.Pop();
						if (state.CallStack.Count == 0)
							return;
						break;

					case Opcode.ClassSetFieldStatic:
						break;

					case Opcode.ClassGetFieldStatic:
						break;

					case Opcode.ClassSetField:
						break;

					case Opcode.ClassGetField:
						break;

					case Opcode.ClassCall:
						break;

					case Opcode.ClassCallStatic:
						break;

					case Opcode.ClassCallVirtual:
						break;

					case Opcode.NewClass:
						break;

					case Opcode.New:
						break;

					case Opcode.CastVariant:
						break;

					case Opcode.SetGlobalVariable:
						break;

					case Opcode.GetGlobalVariable:
						break;

					case Opcode.Print:
						Console.WriteLine(state.Stack.Pop());
						break;

					case Opcode.Breakpoint:
						// Primitive breakpoint
						Console.WriteLine("Breakpoint!");
						System.Diagnostics.Debugger.Break();
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}

				++state.InstructionPointer;
			}

			state.Stack.PopScope();
		}

		public State CreateState(string className, string methodName)
		{
			Class classHandle = GetClass(className);
			if (classHandle == null)
				return null;

			int methodIndex = classHandle.GetMethodIndex(methodName);
			if (methodIndex == -1)
				return null;

			State state = new State();
			state.CallStack.Push(new CallInfo(classHandle, methodIndex, 0));

			return state;
		}

		public State CreateState(int classIndex, int methodIndex)
		{
			Class classHandle = Classes[classIndex];
			if (classHandle == null)
				return null;

			State state = new State();
			state.CallStack.Push(new CallInfo(classHandle, methodIndex, 0));

			return state;
		}

		private Class GetClass(string name)
		{
			return Classes.FirstOrDefault(m => m.Name == name);
		}
	}
}
