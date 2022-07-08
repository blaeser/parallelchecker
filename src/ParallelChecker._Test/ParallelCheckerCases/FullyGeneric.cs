using System;
using System.Collections.Generic;
using System.Threading;

class BoundedBuffer<T> {
  private readonly int capacity;
  private readonly Queue<T> elements = new Queue<T>();

  public BoundedBuffer(int capacity) {
    this.capacity = capacity;
  }

  public void Put(T item) {
    elements.Enqueue(item);
  }

  public T Get() {
    return elements.Dequeue();
  }
}

public class Program {
  const int N = 10;
  public static void Main() {
    var buffer = new BoundedBuffer<int>(1);
    var list = new List<Thread> {
        new Thread(() => {
          for (int round = 0; round < N; round++) {
            buffer.Put(round);
          }
        }),
        new Thread(() => {
          for (int round = 0; round < N; round++) {
            try {
              buffer.Get();
            } catch (InvalidOperationException) {
              //Console.Error.WriteLine(e);
            }
          }
        })
      };
    foreach (var thread in list) {
      thread.Start();
    }
  }
}
