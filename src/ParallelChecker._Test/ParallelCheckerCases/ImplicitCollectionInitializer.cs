using System;
using System.Collections.Generic;
using System.Threading.Tasks;

List<int> list = new() { 1 };
if (list[0] == 1) {
  Task.Run(() => list.Add(2));
}
Console.WriteLine(list.Count);




