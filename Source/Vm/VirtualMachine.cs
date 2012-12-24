﻿using System;
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
							state.Stack[(int)instruction.Operands[0].IntValue] += instruction.Operands[1];
							break;
						}

					case Opcode.IncrementField:
						{
							Class classHandle = (Class)state.Stack.Pop().ObjectValue;
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
						Classes[(int)instruction.Operands[0].IntValue].Fields[(int)instruction.Operands[1].IntValue] = state.Stack.Pop();
						break;

					case Opcode.ClassGetFieldStatic:
						state.Stack.Push(Classes[(int)instruction.Operands[0].IntValue].Fields[(int)instruction.Operands[1].IntValue]);
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
							state.Stack.PushScope(state.CurrentMethod.VariableCount + state.CurrentMethod.ArgumentCount);

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
							Class classHandle = Classes[(int)instruction.Operands[0].IntValue];

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
							state.Stack.PushScope(state.CurrentMethod.VariableCount + state.CurrentMethod.ArgumentCount);

							// Pop arguments from transition stack into local arguments
							for (int i = 0; i < state.CurrentMethod.ArgumentCount; ++i)
								state.Stack[i] = arguments.Pop();

							continue;
						}

					case Opcode.ClassCallVirtual:
						break;

					case Opcode.New:
						state.Stack.Push(new Variant(new Class(Classes[(int)instruction.Operand.IntValue])));
						break;

					case Opcode.CastVariant:
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
			state.CurrentCall = new CallInfo(classHandle, methodIndex, 0);
			//state.CallStack.Push(new CallInfo(classHandle, methodIndex, 0));

			return state;
		}

		public State CreateState(int classIndex, int methodIndex)
		{
			Class classHandle = Classes[classIndex];
			if (classHandle == null)
				return null;

			State state = new State();
			state.CurrentCall = new CallInfo(classHandle, methodIndex, 0);
			//state.CallStack.Push(new CallInfo(classHandle, methodIndex, 0));

			return state;
		}

		private Class GetClass(string name)
		{
			return Classes.FirstOrDefault(m => m.Name == name);
		}
	}
}
