# csharp-data-structures

Classic data structures and algorithms implemented from scratch in **C# 14 /
.NET 10**, with xUnit + FsCheck property tests and BenchmarkDotNet comparisons
against the `System.Collections.Generic` equivalents.

These are not meant to replace the BCL collections — `List<T>`,
`Dictionary<TKey,TValue>`, and `SortedSet<T>` beat everything here in
production. The point is to make the mechanics inspectable and to show the
engineering discipline (typing, docs, property tests, benchmarks, CI) you'd
apply to real code.

## About this repo

Original implementations: assignments from **CS 2420 (Data Structures and
Algorithms)** at Southern Utah University, originally written in Java. This
repo is the C# rewrite — same structures, reorganized by topic, with:

- Idiomatic C# generics, `IEnumerable<T>` implementations, nullable reference
  types, and full XML doc comments
- xUnit unit tests + **FsCheck property tests** (e.g. "BST in-order traversal
  is sorted for *any* input", "every sort matches the framework sort")
- **BenchmarkDotNet** suites comparing each structure to its BCL counterpart
- GitHub Actions CI: `dotnet format` + build + test
- `TreatWarningsAsErrors` — the build fails on any warning

The structures are coursework; the C# port, property tests, benchmarks, and
CI are recent (2026).

## Structures

| Module | Types | Notable detail |
|---|---|---|
| `LinkedList/` | `SinglyLinkedList<T>`, `DoublyLinkedList<T>`, `SortedLinkedList<T>` | tail pointer for O(1) append; doubly for O(1) ends + reverse enumeration |
| `StackQueue/` | `ArrayStack<T>`, `LinkedStack<T>`, `CircularQueue<T>` | ring buffer queue avoids the O(n)-per-dequeue shifting bug |
| `Searching/` | `BinarySearch` | iterative + recursive; overflow-safe midpoint |
| `Sorting/` | `Sorts` (merge, quick, heap) | merge is stable; quick uses median-of-three + recurse-smaller-side |
| `Trees/` | `BinarySearchTree<T>` | full three-case delete via in-order successor; lazy in-order iterator |
| `Hashing/` | `ChainingHashTable`, `OpenAddressingHashTable` | chaining vs linear probing with tombstones |

## Complexity reference

| Operation | Singly LL | Doubly LL | ArrayStack | CircularQueue | BST (avg) | Chaining HT | OpenAddr HT |
|---|---|---|---|---|---|---|---|
| Insert | O(1) head/tail | O(1) ends | O(1)* | O(1)* | O(log n) | O(1)* | O(1)* |
| Search | O(n) | O(n) | — | — | O(log n) | O(1)* | O(1)* |
| Delete | O(n) | O(1) ends | O(1) | O(1) | O(log n) | O(1)* | O(1)* |

\* amortized. BST degrades to O(n) on sorted input (it's unbalanced — by design, to show the core mechanics before balancing).

## When to use what (vs the BCL)

- **Need a list?** Use `List<T>`. These linked lists exist to show pointer
  mechanics; `List<T>`'s contiguous storage wins on almost every real workload.
- **Need a map?** Use `Dictionary<TKey,TValue>`. The open-addressing table
  here is the same family of design, minus a decade of tuning.
- **Need a sorted set?** Use `SortedSet<T>` (a red-black tree). The BST here
  is unbalanced and will degrade on sorted input.
- **Need to sort?** Use `Array.Sort` / `List<T>.Sort` (introsort). The sorts
  here are textbook implementations for understanding, not for speed.

The benchmarks quantify each of these gaps.

## Build, test, benchmark

```bash
dotnet build -c Release
dotnet test -c Release
dotnet run -c Release --project benchmarks/DataStructures.Benchmarks
```

Requires the .NET 10 SDK.

## Stack

- C# 14 / .NET 10 (LTS), nullable enabled, warnings-as-errors
- xUnit + FsCheck (property-based testing)
- BenchmarkDotNet
- GitHub Actions: `dotnet format --verify-no-changes` + build + test

## License

MIT — see [LICENSE](LICENSE).
