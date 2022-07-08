using System;

class Test {

  enum TestEnum { A, B }

  public static void Main() {
    Console.Write(sizeof(char));
    Console.Write(sizeof(byte));
    Console.Write(sizeof(bool));
    Console.Write(sizeof(short));
    Console.Write(sizeof(ushort));
    Console.Write(sizeof(int));
    Console.Write(sizeof(uint));
    Console.Write(sizeof(long));
    Console.Write(sizeof(ulong));
    Console.Write(sizeof(float));
    Console.Write(sizeof(double));
    Console.Write(sizeof(decimal));
    Console.Write(sizeof(TestEnum));
  }
}
