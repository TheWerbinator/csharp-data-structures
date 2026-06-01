using DataStructures.StackQueue;
using Xunit;

namespace DataStructures.Tests;

public class ArrayStackTests
{
    [Fact]
    public void Push_Pop_IsLifo()
    {
        var stack = new ArrayStack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        Assert.Equal(3, stack.Pop());
        Assert.Equal(2, stack.Pop());
        Assert.Equal(1, stack.Pop());
    }

    [Fact]
    public void GrowsBeyondInitialCapacity()
    {
        var stack = new ArrayStack<int>(capacity: 2);
        for (var i = 0; i < 100; i++) stack.Push(i);
        Assert.Equal(100, stack.Count);
        Assert.Equal(99, stack.Peek());
    }

    [Fact]
    public void Pop_OnEmpty_Throws()
    {
        var stack = new ArrayStack<int>();
        Assert.Throws<InvalidOperationException>(() => stack.Pop());
    }

    [Fact]
    public void Enumerates_TopToBottom()
    {
        var stack = new ArrayStack<int>();
        stack.Push(1);
        stack.Push(2);
        Assert.Equal([2, 1], stack);
    }
}

public class LinkedStackTests
{
    [Fact]
    public void Push_Pop_Peek_IsLifo()
    {
        var stack = new LinkedStack<string>();
        stack.Push("a");
        stack.Push("b");
        Assert.Equal("b", stack.Peek());
        Assert.Equal("b", stack.Pop());
        Assert.Equal("a", stack.Pop());
    }

    [Fact]
    public void Pop_OnEmpty_Throws()
    {
        var stack = new LinkedStack<int>();
        Assert.Throws<InvalidOperationException>(() => stack.Pop());
    }
}

public class CircularQueueTests
{
    [Fact]
    public void Enqueue_Dequeue_IsFifo()
    {
        var queue = new CircularQueue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        Assert.Equal(1, queue.Dequeue());
        Assert.Equal(2, queue.Dequeue());
        Assert.Equal(3, queue.Dequeue());
    }

    [Fact]
    public void WrapsAroundRingBuffer()
    {
        var queue = new CircularQueue<int>(capacity: 4);
        // Fill, drain partway, refill — exercises the wrap-around indices.
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        Assert.Equal(1, queue.Dequeue());
        Assert.Equal(2, queue.Dequeue());
        queue.Enqueue(4);
        queue.Enqueue(5);
        queue.Enqueue(6);  // forces wrap and/or grow
        Assert.Equal([3, 4, 5, 6], queue);
    }

    [Fact]
    public void GrowsBeyondInitialCapacity()
    {
        var queue = new CircularQueue<int>(capacity: 2);
        for (var i = 0; i < 50; i++) queue.Enqueue(i);
        Assert.Equal(50, queue.Count);
        Assert.Equal(0, queue.Peek());
        Assert.Equal(Enumerable.Range(0, 50), queue);
    }

    [Fact]
    public void Dequeue_OnEmpty_Throws()
    {
        var queue = new CircularQueue<int>();
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
    }
}
