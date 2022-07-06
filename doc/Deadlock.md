# Deadlocks

## Definition

A deadlock is an error in multi-threaded programs. It denotes a constellation in which multiple threads block each other in a circular way, such that none of them can proceed.

## Properties

A deadlock has the following preconditions:

* The involved threads all use nested locking.
* Each involved thread holds at least one lock, while awaiting a lock held by another involved thread.
* The wait dependencies among the involved threads is cyclic.
* Locking is exclusive and blocking without timeouts.

A deadlock can occur by a cycle of an arbitrary number of threads. A livelock shares similarities with a deadlock: Threads are also cyclically waiting on each other forever, but looping while being in the wait state.

## Problem

Deadlocks happen non-deterministically, meaning they can occur arbitrarily seldom or often. When occurred, the involved threads are certainly blocked forever, often leading to a suddenly hanging program.

## Example

A deadlock requires nested locking and nesting is inherent to method calls. This example shows the method `Transfer()` that first establishes a lock on its own instance `this.sync` and therein effects the method call `other.Deposit()`. This call consequently acquires a lock on the other instance `other.sync`.

    class BankAccount {
        private int balance;
        private object sync = new object();

        public void Transfer(BankAccount other, int amount) {
            lock(sync) {
                balance -= amount;
                other.Deposit(amount);
            }
        }


        public void Deposit(int amount) {
            lock(sync) {
                balance += amount;
            }
        }
    }

This arises when threads invoke `Transfer()` with cyclic payment directions, e.g.

    var a = new BankAccount();
    var b = new BankAccount();

Thread 1

    a.Transfer(b); (nested lock a.sync -> b.sync)

Thread 2

 	b.Transfer(a); (nested lock b.sync -> a.sync)

The following schedule eventually leads to a deadlock:

* Thread 1: Acquires the lock on `a.sync`
* Thread 2: Acquires the lock on `b.sync`
* Thread 1: Awaits the lock `b.sync`
* Thread 2: Awaits the lock `a.sync`

The following diagram depicts the deadlock situation. Each thread holds a lock and awaits the lock that is acquired by the other thread.

Deadlock cycle

## Correction

Deadlocks can be avoided by at least one of the following countermeasures:

* Eliminate nested locking, e.g. use a common lock for all `BankAccount` instances.
* Hierarchical lock order, e.g. assign unique numbers to `BankAccount` instances and only acquire nested locks on instances with a number greater than the numbers of the currently locked instances.

Using locks with timeouts and performing retries on timeouts usually leads to another concurrency issue, namely starvation: One or multiple threads may continuously retry on timeouts, thus never making progress. In contrast to deadlocks or livelocks, where the situation is blocked forever, there is always a chance (but not a guarantee) of progress in a starvation situation.
