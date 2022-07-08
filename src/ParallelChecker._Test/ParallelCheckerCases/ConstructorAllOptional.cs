using System;

namespace ParallelChecker._Test {
  class Message {
    public string Content { get; set; }

    public Message(string content = null) {
      Content = content;
    }
  }

  public class Program {
    public static void Main() {
      var m = new Message {
        Content = "Hello"
      };
      Console.WriteLine(m);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}
