using System.Collections.Generic;

namespace Xi.Vm
{
	class VmStack<T>
	{
		private const int InitialStackSize = 256;

		private T[] stack;
		private int stackIndex, stackSize, stackBase;
		private readonly Stack<int> indices; 

		public T this[int index]
		{
			get { return stack[stackBase + index]; }
			set { stack[stackBase + index] = value; }
		}

		public T Top
		{
			get { return stack[stackIndex - 1]; }
			set { stack[stackIndex - 1] = value; }
		}

		public long Count
		{
			get { return stackIndex; }
		}

		public long ScopeDepth
		{
			get { return indices.Count; }
		}

		public VmStack()
		{
			indices = new Stack<int>();
			stack = new T[InitialStackSize];
			stackIndex = 0;
			stackSize = InitialStackSize;
			stackBase = 0;
		}

		public void Push(T value)
		{
			if (stackIndex >= stackSize)
			{
				stackSize += 256;
				var newStack = new T[stackSize];
				stack.CopyTo(newStack, 0);
				stack = newStack;
			}

			stack[stackIndex++] = value;
		}

		public T Pop()
		{
			return stack[--stackIndex];
		}

		public void PushScope(int size)
		{
			indices.Push(stackBase);
			stackBase = stackIndex;

			Resize(stackBase + size);
		}

		public void PopScope()
		{
			Resize(stackBase);
			stackBase = indices.Pop();
		}

		private void Resize(int length)
		{
			stackIndex = length;
			if (stackIndex < stackSize)
				return;
			
			stackSize += 256;
			var newStack = new T[stackSize];
			stack.CopyTo(newStack, 0);
			stack = newStack;
		}
	}
}
