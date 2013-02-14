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

					case Opcode.SetArrayVariable:
						state.Stack[(int)instruction.Operands[0].IntValue][(int)state.Stack.Pop().IntValue] = state.Stack.Pop();
						break;

					case Opcode.GetArrayVariable:
						state.Stack.Push(state.Stack[(int)instruction.Operands[0].IntValue][(int)state.Stack.Pop().IntValue]);
						break;

					case Opcode.GetVariableLength:
						state.Stack.Push(new Variant(state.Stack.Pop().Length));
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
						state.Stack.Push(state.CurrentClass == null ? new Variant(state.CurrentModule) : new Variant(state.CurrentClass));
						break;

					case Opcode.GetBase:
						if (state.CurrentClass == null)
							throw new Exception("Cannot GetBase outside of the scope of a class.");

						state.Stack.Push(new Variant(state.CurrentClass.Base));
						break;

					case Opcode.GetBaseOf:
						state.Stack.Push(new Variant(((Class)(state.Stack.Pop().ObjectValue)).Base));
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
							Class classHandle = (Class)state.Stack.Pop().ObjectValue;
							classHandle.Fields[(int)instruction.Operands[0].IntValue] += instruction.Operands[1];
							break;
						}

					case Opcode.AbsoluteValue:
						state.Stack.Push(+state.Stack.Pop());
						break;

					case Opcode.CompareEqual:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(new Variant(b == a ? 1 : 0));
							break;
						}

					case Opcode.CompareNotEqual:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(new Variant(b != a ? 1 : 0));
							break;
						}

					case Opcode.CompareGreaterThan:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(new Variant(b > a ? 1 : 0));
							break;
						}

					case Opcode.CompareLesserThan:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(new Variant(b < a ? 1 : 0));
							break;
						}

					case Opcode.CompareGreaterThanOrEqual:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(new Variant(b >= a ? 1 : 0));
							break;
						}

					case Opcode.CompareLesserThanOrEqual:
						{
							Variant a = state.Stack.Pop();
							Variant b = state.Stack.Pop();

							state.Stack.Push(new Variant(b <= a ? 1 : 0));
							break;
						}

					case Opcode.IfTrue:
						if (state.Stack.Pop().IntValue == 1)
							state.InstructionPointer = (int)instruction.Operand.IntValue;
						else
							++state.InstructionPointer;
						continue;

					case Opcode.IfFalse:
						if (state.Stack.Pop().IntValue == 0)
							state.InstructionPointer = (int)instruction.Operand.IntValue;
						else
							++state.InstructionPointer;
						continue;

					case Opcode.Jump:
						state.InstructionPointer = (int)instruction.Operand.IntValue;
						continue;

					case Opcode.Return:
						{
							// If there are no more calls in the stack then we must be at the end of our spawn method
							if (state.CallStack.Count == 0)
								return;

							state.CallStack.Pop();
							stream = state.CurrentMethod.Instructions;
							state.Stack.PopScope();

							continue;
						}

					case Opcode.ClassSetFieldStatic:
						// TODO Eventually also reference module
						// TODO Also reimplement this
						//state.Classes[(int)instruction.Operands[0].IntValue].Fields[(int)instruction.Operands[1].IntValue] = state.Stack.Pop();
						break;

					case Opcode.ClassGetFieldStatic:
						// TODO Eventually also reference module
						// TODO Also reimplement this
						//state.Stack.Push(state.Classes[(int)instruction.Operands[0].IntValue].Fields[(int)instruction.Operands[1].IntValue]);
						break;

					case Opcode.ClassSetField:
						{
							Class classHandle = (Class)state.Stack.Pop().ObjectValue;
							classHandle.Fields[(int)instruction.Operand.IntValue] = state.Stack.Pop();
							break;
						}

					case Opcode.ClassGetField:
						{
							Class classHandle = (Class)state.Stack.Pop().ObjectValue;
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
							Class classHandle = (Class)state.Stack.Pop().ObjectValue;

							// TODO This is sort of a hack (cannot call cross modules)
							// Push reentrant info onto call stack
							state.CallStack.Push(new CallInfo(state.CurrentModule, state.CurrentClass,
														classHandle.GetMethod(state.CurrentMethod.Name),
														state.InstructionPointer + 1));

							// Set state's call info to the new call & grab all requested arguments
							//state.CurrentCall = new CallInfo(classHandle, (int)instruction.Operand.IntValue, 0);
							// TODO Also a bit of a hack
							state.CallStack.Push(new CallInfo(state.CurrentModule, classHandle, classHandle.GetMethod(instruction.Operand.StringValue), 0));
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

							// TODO Probably should reimplement this up... a lot

							/*

							// Grab class to call from stack
							Class classHandle = state.Classes[(int)instruction.Operands[0].IntValue];

							// Push reentrant info onto call stack
							// TODO Hack cannot call cross blah bloh blab
							state.CallStack.Push(new CallInfo(state.CurrentModule, state.CurrentClass,
														classHandle.GetMethodIndex(state.CurrentMethod.Name),
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
							 * 
							 */

							continue;
						}

					case Opcode.ClassCallVirtual:
						// Is this even needed? We're not Java
						break;

					case Opcode.New:
						// TODO Reimplement
						//state.Stack.Push(new Variant(new Class(state.Classes[(int)instruction.Operand.IntValue])));
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
