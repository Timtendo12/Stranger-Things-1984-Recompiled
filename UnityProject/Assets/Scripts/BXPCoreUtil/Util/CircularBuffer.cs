using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// Rewritten from libil2cpp.so 1.0.605 (CircularBuffer<T> @ 0x1c33cdc..0x1c73a1c, Ghidra vaddr).
public class CircularBuffer<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
{
	private int capacity;

	private int size;

	private int head;

	private int tail;

	private T[] buffer;

	[NonSerialized]
	private object syncRoot;

	public bool AllowOverflow { get; set; }

	public int Capacity
	{
		get
		{
			return capacity;
		}
		set
		{
			if (value == capacity)
			{
				return;
			}
			if (value < size)
			{
				throw new ArgumentOutOfRangeException("value", "value must be greater than or equal to the buffer size.");
			}
			T[] array = new T[value];
			if (size > 0)
			{
				CopyTo(0, array, 0, size);
			}
			buffer = array;
			capacity = value;
		}
	}

	public int Size => size;

	public int Count => size;

	int ICollection<T>.Count => size;

	bool ICollection<T>.IsReadOnly => false;

	int ICollection.Count => size;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (syncRoot == null)
			{
				Interlocked.CompareExchange(ref syncRoot, new object(), null);
			}
			return syncRoot;
		}
	}

	public CircularBuffer(int capacity)
		: this(capacity, allowOverflow: false)
	{
	}

	public CircularBuffer(int capacity, bool allowOverflow)
	{
		if (capacity < 0)
		{
			throw new ArgumentException("capacity must be greater than or equal to zero.", "capacity");
		}
		this.capacity = capacity;
		size = 0;
		head = 0;
		tail = 0;
		buffer = new T[capacity];
		AllowOverflow = allowOverflow;
	}

	public bool Contains(T item)
	{
		int num = head;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		for (int i = 0; i < size; i++)
		{
			int num2 = ((num != capacity) ? num : 0);
			if (equalityComparer.Equals(item, buffer[num2]))
			{
				return true;
			}
			num = num2 + 1;
		}
		return false;
	}

	public void Clear()
	{
		size = 0;
		head = 0;
		tail = 0;
	}

	public int Put(T[] src)
	{
		return Put(src, 0, src.Length);
	}

	public int Put(T[] src, int offset, int count)
	{
		if (!AllowOverflow)
		{
			count = Math.Min(count, capacity - size);
		}
		int num = tail;
		for (int i = 0; i < count; i++)
		{
			if (num == capacity)
			{
				num = 0;
			}
			buffer[num++] = src[offset + i];
		}
		tail = num;
		size = Math.Min(size + count, capacity);
		return count;
	}

	public void Put(T item)
	{
		if (size == capacity)
		{
			if (!AllowOverflow)
			{
				throw new SystemException("Buffer is full.");
			}
			head = ((head + 1 < capacity) ? (head + 1) : 0);
		}
		buffer[tail] = item;
		tail = ((tail + 1 < capacity) ? (tail + 1) : 0);
		size = ((size + 1 < capacity) ? (size + 1) : capacity);
	}

	public void Skip(int count)
	{
		head += count;
		if (head >= capacity)
		{
			head -= capacity;
		}
	}

	public T[] Get(int count)
	{
		T[] array = new T[count];
		Get(array);
		return array;
	}

	public int Get(T[] dst)
	{
		return Get(dst, 0, dst.Length);
	}

	public int Get(T[] dst, int offset, int count)
	{
		int num = Math.Min(count, size);
		int num2 = head;
		for (int i = 0; i < num; i++)
		{
			if (num2 == capacity)
			{
				num2 = 0;
			}
			dst[offset + i] = buffer[num2++];
		}
		head = num2;
		size -= num;
		return num;
	}

	public T Get()
	{
		if (size == 0)
		{
			throw new InvalidOperationException("Buffer is empty.");
		}
		T result = buffer[head];
		head = ((head != capacity) ? (head + 1) : 0);
		size--;
		return result;
	}

	public T PeekLast()
	{
		if (size == 0)
		{
			throw new InvalidOperationException("Buffer is empty.");
		}
		return buffer[head];
	}

	public T PeekFirst()
	{
		if (size == 0)
		{
			throw new InvalidOperationException("Buffer is empty.");
		}
		int num = tail - 1;
		if (num < 0)
		{
			num = capacity - 1;
		}
		return buffer[num];
	}

	public T PeekFirstOrDefault(T defaultValue)
	{
		if (size != 0)
		{
			int num = tail - 1;
			if (num < 0)
			{
				num = capacity - 1;
			}
			defaultValue = buffer[num];
		}
		return defaultValue;
	}

	public T Peek(int index)
	{
		if (size == 0)
		{
			throw new InvalidOperationException("Buffer is empty.");
		}
		int num = head + index;
		int num2 = ((capacity > num) ? 0 : capacity);
		return buffer[num - num2];
	}

	public T PeekBack(int index)
	{
		if (size == 0)
		{
			throw new InvalidOperationException("Buffer is empty.");
		}
		int num = tail + ~index;
		if (num < 0)
		{
			num += capacity;
		}
		return buffer[num];
	}

	public bool IsFull()
	{
		return size == capacity;
	}

	public bool IsEmpty()
	{
		return size == 0;
	}

	public void CopyTo(T[] array)
	{
		CopyTo(0, array, 0, size);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		CopyTo(0, array, arrayIndex, size);
	}

	public void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		if (count > size)
		{
			throw new ArgumentOutOfRangeException("count", "count cannot be greater than the buffer size.");
		}
		int num = head;
		for (int i = 0; i < count; i++)
		{
			int num2 = ((num != capacity) ? num : 0);
			array[arrayIndex + i] = buffer[num2];
			num = num2 + 1;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		int bufferIndex = head;
		for (int i = 0; i < size; i++, bufferIndex++)
		{
			if (bufferIndex == capacity)
			{
				bufferIndex = 0;
			}
			yield return buffer[bufferIndex];
		}
	}

	public T[] GetBuffer()
	{
		return buffer;
	}

	public T[] ToArray()
	{
		T[] array = new T[size];
		CopyTo(0, array, 0, size);
		return array;
	}

	void ICollection<T>.Add(T item)
	{
		Put(item);
	}

	bool ICollection<T>.Remove(T item)
	{
		if (size == 0)
		{
			return false;
		}
		Get();
		return true;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		CopyTo(0, (T[])array, arrayIndex, size);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
