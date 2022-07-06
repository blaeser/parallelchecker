# Data Race

## Definition

A data race is a formal programming error in multi-threading. It fulfills all the following properties in C#:

* A pair of accesses, effected by different threads
* No mutual synchronization between these accesses
* Accesses to the same variable or array element
* Read-write, write-read, or write-write accesses
* No access to a variable with `volatile` declaration

## Problem

Data races happen non-deterministically, meaning they can occur arbitrarily seldom or often. Data races are often, but not necessarily, the reason for race condition, non-deterministic wrong program behavior or results. This is because:

* The instructions of concurrent unsynchronized code may be arbitrarily interleaved or executed in parallel.
* The accesses of a thread may be perceived differently or not at all in another thread, unless synchronized.

The .NET memory model has a so-called weak memory consistency. This allows the compiler, the runtime system, or the processors to reorder or skip instructions, if there is no mutual synchronization (memory barrier) between the instructions and the serial isolated effect within each thread is maintained (as-if-serial semantics).

## Sources

There exist different sources of multi-threading, even very implicit ones, in a C# program that may provoke data races:

* Explicit threads
* TPL tasks, other thread pool tasks
* Parallel invoke, for/foreach, queries
* Timer events (except in UI)
* Async/await in certain cases
* Finalizer (Destructor)
* Concurrency by libraries/frameworks

## Examples

The following examples show frequent cases of data races. There are many more constellations.

### Lost Update

    int balance = 0;

Thread 1

    balance += 50;

Thread 2

    balance += 100;

Problem

*New balance may finally be only 50, or only 100. Moreover, none of the updates may be visible to unsynchronized reading thread.*

## Inconsistent Tests

    bool open = true;

Thread 1

    if (!open) {
        open = true;
        // work
    }

Thread 2

	if (!open) {
        open = true;
        // work
    }

Problem

*Both threads may go into the if-block, since the if-statement is not atomic.*

## Invisible Write

Thread 1

    ok = true;

Thread 2

 	while (!ok) {
    }

Problem

*Thread 2 could loop infinitely, as it may never see the write of thread 1.*

## Correction

Data races can be eliminated by correct synchronization, using any of these concepts:

* Monitor synchronization, e.g. lock statements
* Synchronization primitives, such as semaphores, read-write locks, mutexes, barriers etc.
* Thread start/joins, Task start/awaits
* Atomic instructions/Interlocked, volatile, memory barriers
* Concurrent collections

Correct and efficient synchronization is a non-trivial task that requires experience. New problems may be introduced with incorrect synchronization:

* Race conditions, even without data races, if synchronization is not sufficiently large or incorrect
* Deadlocks, livelocks, and starvation errors
* Performance issues
