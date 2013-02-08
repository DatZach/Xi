using System;
using System.Collections.Generic;

namespace Xi.Vm
{
	static class VirtualMachine
	{
		public static void Execute(State state)
		{
			List<Instruction> stream = state.CurrentMethod.Instructions;

			// Prepare stack
			state.Stack.PushScope(state.CurrentMethod.Variables.Count);

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
						state.Stack.Push(new Variant((state.Stack.Pop().ObjectValue).Base));
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

					case Opcode.LogicalNegate:
						state.Stack.Push(-state.Stack.Pop());
						break;

					case Opcode.BitwiseNegate:
						state.Stack.Push(~state.Stack.Pop());
						break;

					case Opcode.BitwiseNot:
						state.Stack.Push(!state.Stack.Pop());
						break;

					case Opcode.BitwiseAnd:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b & a);
							break;
						}

					case Opcode.BitwiseOr:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b | a);
							break;
						}

					case Opcode.BitwiseXor:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b ^ a);
							break;
						}

					case Opcode.BitwiseShiftLeft:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(b << (int)a.IntValue);
							break;
						}

					case Opcode.BitwiseShiftRight:
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
							state.Stack[(int)instruction.Operands[0].IntValue] += instruction.Operands[1];
							break;
						}

					case Opcode.IncrementField:
						{
							Class classHandle = state.Stack.Pop().ObjectValue;
							classHandle.Fields[(int)instruction.Operands[0].IntValue] += instruction.Operands[1];
							break;
						}

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
						{
							// If there are no more calls in the stack then we must be at the end of our spawn method
							if (state.CallStack.Count == 0)
								return;

							state.CurrentCall = state.CallStack.Pop();
							stream = state.CurrentMethod.Instructions;
							state.Stack.PopScope();

							continue;
						}

					case Opcode.ClassSetFieldStatic:
						state.Classes[(int)instruction.Operands[0].IntValue].Fields[(int)instruction.Operands[1].IntValue] = state.Stack.Pop();
						break;

					case Opcode.ClassGetFieldStatic:
						state.Stack.Push(state.Classes[(int)instruction.Operands[0].IntValue].Fields[(int)instruction.Operands[1].IntValue]);
						break;

					case Opcode.ClassSetField:
						{
							Class classHandle = state.Stack.Pop().ObjectValue;
							classHandle.Fields[(int)instruction.Operand.IntValue] = state.Stack.Pop();
							break;
						}

					case Opcode.ClassGetField:
						{
							Class classHandle = state.Stack.Pop().ObjectValue;
							state.Stack.Push(classHandle.Fields[(int)instruction.Operand.IntValue]);
							break;
						}

					case Opcode.ClassCall:
						{
							/*
							 * Probably should clean this up... a lot
							 * Also variable argument lists aren't supported with this method
							 */

							// Grab class to call from stack
							Class classHandle = state.Stack.Pop().ObjectValue;

							// Push reentrant info onto call stack
							state.CallStack.Push(new CallInfo(state.CurrentClass,
														state.CurrentClass.GetMethodIndex(state.CurrentMethod.Name),
														state.InstructionPointer + 1));

							// Set state's call info to the new call & grab all requested arguments
							state.CurrentCall = new CallInfo(classHandle, (int)instruction.Operand.IntValue, 0);
							Stack<Variant> arguments = new Stack<Variant>();
							for (int i = 0; i < state.CurrentMethod.ArgumentCount; ++i)
								arguments.Push(state.Stack.Pop());

							// Change stream to current method's instruction stream
							stream = state.CurrentMethod.Instructions;

							// Push stack scope
							state.Stack.PushScope(state.CurrentMethod.Variables.Count + state.CurrentMethod.ArgumentCount);

							// Pop arguments from transition stack into local arguments
							for (int i = 0; i < state.CurrentMethod.ArgumentCount; ++i)
								state.Stack[i] = arguments.Pop();

							continue;
						}

					case Opcode.ClassCallStatic:
						{
							/*
							 * Probably should clean this up... a lot
							 * Also variable argument lists aren't supported with this method
							 */

							// Grab class to call from stack
							Class classHandle = state.Classes[(int)instruction.Operands[0].IntValue];

							// Push reentrant info onto call stack
							state.CallStack.Push(new CallInfo(state.CurrentClass,
														state.CurrentClass.GetMethodIndex(state.CurrentMethod.Name),
														state.InstructionPointer + 1));

							// Set state's call info to the new call & grab all requested arguments
							state.CurrentCall = new CallInfo(classHandle, (int)instruction.Operand.IntValue, 0);
							Stack<Variant> arguments = new Stack<Variant>();
							for (int i = 0; i < state.CurrentMethod.ArgumentCount; ++i)
								arguments.Push(state.Stack.Pop());

							// Change stream to current method's instruction stream
							stream = state.CurrentMethod.Instructions;

							// Push stack scope
							state.Stack.PushScope(state.CurrentMethod.Variables.Count + state.CurrentMethod.ArgumentCount);

							// Pop arguments from transition stack into local arguments
							for (int i = 0; i < state.CurrentMethod.ArgumentCount; ++i)
								state.Stack[i] = arguments.Pop();

							continue;
						}

					case Opcode.ClassCallVirtual:
						// Is this even needed? We're not Java
						break;

					case Opcode.New:
						state.Stack.Push(new Variant(new Class(state.Classes[(int)instruction.Operand.IntValue])));
						break;

					case Opcode.CastVariant:
						state.Stack.Push(state.Stack.Pop().Cast((VariantType)instruction.Operand.IntValue));
						break;

					case Opcode.SetGlobalVariable:
						{
							var dictScope = state.Scope as IDictionary<string, object>;
							if (dictScope == null)
								break;

							if (dictScope.ContainsKey(instruction.Operand.StringValue))
								dictScope[instruction.Operand.StringValue] = state.Stack.Pop();
							else
								dictScope.Add(instruction.Operand.StringValue, state.Stack.Pop());
							break;
						}

					case Opcode.GetGlobalVariable:
						{
							var dictScope = state.Scope as IDictionary<string, object>;
							if (dictScope == null)
								break;

							if (dictScope.ContainsKey(instruction.Operand.StringValue))
								state.Stack.Push((Variant)dictScope[instruction.Operand.StringValue]);
							else
								state.Stack.Push(new Variant());

							break;
						}

					case Opcode.Print:
						Console.WriteLine(state.Stack.Pop());
						break;

					case Opcode.Breakpoint:
						// Primitive breakpoint
						System.Diagnostics.Debugger.Break();
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}

				++state.InstructionPointer;
			}

			state.Stack.PopScope();
		}
	}
}
