# Thread-Unsafe Usage

## Definition

These are concurrent unsynchronized calls or accesses performed on an application programming interface (API) that are not specified to be thread-safe.

## Problem

Many APIs are not thread-safe, i.e. do not support concurrent calls or accesses on it in general or in specific cases. By violating this assumption and performing concurrent unsynchronized accesses or calls on such thread-unsafe APIs, various race conditions can happen.

## Example

The following cases describe examples of thread-unsafe API usage in .NET:

* Collections API: Concurrent calls, unless using synchronized or concurrent collections.
* Concurrent iterations: Iterating collections that are concurrently modified, unless being a concurrent collection.

The effect of such errors is unspecified: It can result in inconsistent or broken collection states, sudden exceptions, unobserved erroneous internal states etc.
