using System;
using System.Collections.Generic;

namespace Xi.Vm
{
	static class VirtualMachine
	{
		public const ushort Version = 1;

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
							List<Variant> arguments = new List<Variant>();

							// If there are no more calls in the stack then we must be at the end of our spawn method
							if (state.CallStack.Count == 0)
								return;

							for (int i = 0; i < instruction.Operand.IntValue; ++i)
								arguments.Add(state.Stack.Pop());

							state.CallStack.Pop();
							stream = state.CurrentMethod.Instructions;
							state.Stack.PopScope();

							for (int i = 0; i < instruction.Operand.IntValue; ++i)
								state.Stack.Push(arguments[i]);

							continue;
						}

					case Opcode.ModuleCall:
						{
							// Grab class to call from stack
							Module moduleHandle = state.Modules[(int)instruction.Operands[0].IntValue];

							// Update call info reentrant IP
							++state.InstructionPointer;

							// Set state's call info to the new call & grab all requested arguments
							state.CallStack.Push(new CallInfo(moduleHandle,
															  null,
															  (int)instruction.Operands[1].IntValue == -1
																? moduleHandle.Body
																: moduleHandle.Methods[(int)instruction.Operands[1].IntValue],
															  0));

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

					case Opcode.ClassCall:
						{
							// Grab class to call from stack
							Class classHandle = (Class)state.Stack.Pop().ObjectValue;

							// Update call info reentrant IP
							++state.InstructionPointer;

							// Set state's call info to the new call & grab all requested arguments
							state.CallStack.Push(new CallInfo(classHandle.Module,
														classHandle,
														classHandle.Methods[(int)instruction.Operand.IntValue],
														0));

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
						// TODO Reimplement
						break;

					case Opcode.ClassCallVirtual:
						// Is this even needed? We're not Java
						break;

					case Opcode.ClassSetFieldStatic:
						{
							Module module = state.Modules[(int)instruction.Operands[0].IntValue];
							module.Classes[(int)instruction.Operands[1].IntValue].Fields[(int)instruction.Operands[2].IntValue] =
								state.Stack.Pop();
							break;
						}

					case Opcode.ClassGetFieldStatic:
						{
							Module module = state.Modules[(int)instruction.Operands[0].IntValue];
							state.Stack.Push(module.Classes[(int)instruction.Operands[1].IntValue].Fields[(int)instruction.Operands[2].IntValue]);
							break;
						}

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

					case Opcode.New:
						{
							Module module = state.Modules[(int)instruction.Operands[0].IntValue];
							state.Stack.Push(new Variant(new Class(module.Classes[(int)instruction.Operands[1].IntValue]) { Module = module }));
							break;
						}

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
						throw new Exception(String.Format("Opcode {0:X} is invalid!", (int)instruction.Opcode));
				}

				++state.InstructionPointer;
			}

			state.Stack.PopScope();
		}
	}
}
