# Parallel Checker

The Parallel Checker is a static analysis tool that detects data races, deadlocks, and other concurrency issues in C#. The checker is available as a NuGet package (e.g. for Visual Studio Code) and as a Visual Studio extension.

## Concurrency Issues

The checker detects the following concurrency (multi-threading) issues with a static analysis:

* **Data Races**: Unsynchronized concurrent accesses to same variable or array element, involving at least a write.
* **Deadlocks**: Constellations in which multiple threads block each other cyclically forever.
* **Thread-Unsafe Usage**: Unsynchronized concurrent calls or accesses of application programming interfaces that are not specified to be thread safe.

The abovementioned issues are all fundamental programming bugs that can lead to program errors. These issues occur non-deterministically, possibly sporadically or very seldom. They are therefore hard to identify in tests and are not easily reproducible. For this reason, it makes sense to use a static analysis that examines various program traces, including very specific or seldom cases, as to whether they suffer from such issues.

## How It Works

The checker is implemented based on the compiler framework Roslyn and analyzes the C# source code without executing the program, called static analysis. It screens as many interesting program traces as possible within defined deterministic bounds. The analysis maintains exact and complete context information for the program, where possible. Exceptions are e.g. external input/output and missing or incorrect source code parts, in which case conservative assumptions are made. The properties of the checker can be summarized as:

* **Precise**: Real issues are reported without false positives (no false alarms), except when making conservative assumptions.
* **Incomplete**: The checker may miss potential issues (possible false negatives) as there exists no precise and complete analysis.
* **Deterministic**: The same issues are repeatedly reported for the same program.

The checker engages a new algorithm that has been designed to efficiently deal with large software projects and find as many issues with high precision as possible.

## Related Publication

More technical information on the checker algorithm and design can be found in the following academic publication (open access):

* L. Bläser. *Practical Detection of Concurrency Issues at Coding Time*. International Symposium on Software Testing and Analysis (ISSTA) 2018, Amsterdam, The Netherlands, In ACM Digital Library, July 2018.

## Scope

The checker supports the following .NET programming concepts and C# language features:

* .NET threads, Task Parallel Library, Async/Await, Parallel Invoke/For/ForEach
* Exact information about objects (aliases, shapes), array indexes etc.
* DLLs, WPF, WinForm, unit test libraries, console applications
* Solution-wide analysis
* C# 10 downwards
* All standard synchronization primitives, incl. monitor
* Memory barriers, atomic / Interlocked, volatile, thread-local variables
* Collection API analysis
* Finalizer-related concurrency

Certain limitations apply, please see the download section for more details.

## Impression

The following screenshot shows a detected data race issue for a C# code in Visual Studio: The two methods `Deposit()` and `Withdraw()` are concurrently invoked on the same instance of `BankAccount`. The checker has verified that there indeed exist multiple threads that concurrently call `Deposit()`/`Withdraw()` on the same instance. There is no mutual lock exclusion between these two method executions, as only `Withdraw()` establishes a monitor lock on `_sync`. Thus, unsynchronized concurrent read/write, write/read and write/write accesses are effected by the corresponding method bodies.

![Screenshot of a data race identified by the parallel checker](https://user-images.githubusercontent.com/108720770/177280283-8aacdbc4-7f37-4c37-b47b-6022f9e05f79.png)

More examples are contained in the samples section.

---

**Notice**: Microsoft, Visual Studio, and Visual Basic are either registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

All trademarks, trade names etc. are the property of their respective owners.

OST, Parallel Checker, and the contributors DO NOT have any affiliation with any mentioned trademark holders.
