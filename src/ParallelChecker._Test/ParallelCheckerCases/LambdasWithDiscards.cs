using System;
using System.Threading.Tasks;

int race = 0;
Func<int, int, int> f1 = (_, _) => race++;
Func<int, int, int> f2 = (_, a) => _ + race++;
Task.Run(() => f1(1, 2));
f2(3, 4);
