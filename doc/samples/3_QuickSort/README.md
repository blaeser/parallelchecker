# QuickSort Sample

## Issue Description (Data Races on Array Elements, TPL)

* The quicksort implementation is broken.
* The boundaries on concurrent quicksort recursion calls are overlapping.
* Thus, data races can be provoked on certain array elements.
* The algorithm may also suffer from an endless loop.

File `QuickSort.cs`:

    private static void Sort(int[] array, int left, int right)
    {
        var pivot = array[(left + right) / 2];
        var lower = left;
        var upper = right;
        do
        { 
            // VARIOUS DATA RACE WARNINGS FOR SUBSEQUENT ARRAY ELEMENT ACCESSES
            while (array[lower] < pivot) lower++; 
            while (array[upper] > pivot) upper--; 
            if (lower <= upper) 
            { 
                var temp = array[lower]; 
                array[lower] = array[upper]; 
                array[upper] = temp; 
                lower++; 
                upper--; 
            }
        } while (lower <= upper);
        var leftTask = Task.Run(() =>
        { 
            if (left < upper) Sort(array, left, lower); // MUST BE upper INSTEAD OF lower
        });
        var rightTask = Task.Run(() =>
        { 
            if (lower < right) Sort(array, upper, right); //MUST BE lower INSTEAD OF upper
        });
        rightTask.Wait();
        leftTask.Wait();
    }

## Checker Output (Various Issues and Locations)

    Issue: #0 Data race on array
        caused by write at "array[lower] = array[upper]" in QuickSort.cs line 24
            caused by call Sort at "Sort(array, upper, right)" in QuickSort.cs line 36
                caused by thread or task at "() => { if (lower < right) Sort(array..." in QuickSort.cs line 34
                    caused by call Sort at "Sort(array, 0, array.Length - 1)" in QuickSort.cs line 9
                        caused by call Sort at "QuickSort.Sort(array)" in Program.cs line 10
                            caused by call QuickSort.Program.Main()
                                caused by initial thread at "Main" in Program.cs line 7
        caused by read at "array[upper]" in QuickSort.cs line 20
            caused by call Sort at "Sort(array, left, lower)" in QuickSort.cs line 32
                caused by thread or task at "() => { if (left < upper) Sort(array..." in QuickSort.cs line 30
                    caused by call Sort at "Sort(array, 0, array.Length - 1)" in QuickSort.cs line 9
                        caused by call Sort at "QuickSort.Sort(array)" in Program.cs line 10
                            caused by call QuickSort.Program.Main()
                                caused by initial thread at "Main" in Program.cs line 7
    
    … (various more issues, e.g. 8 more)

## Problem Fixing

Correct the algorithm by using the following two bounds (highlighted locations).

File `QuickSort.cs`:

    ...
    var leftTask = Task.Run(() =>
    { 
        if (left < upper) Sort(array, left, upper);  // SECOND ARGUMENT MUST BE 'UPPER'
    });
    var rightTask = Task.Run(() =>
    { 
        if (lower < right) Sort(array, lower, right);   // THIRD ARGUMENT MUST BE 'LOWER'
    });
