![Parallel Checker](https://user-images.githubusercontent.com/108720770/178003266-bb35dd6b-d865-4b51-8d21-2bf542fe3073.png)

# Installation

The Parallel Checker works in Visual Studio® and VS Code. Please refer to the corresponding section for download and installation instructions.

## Instructions for Visual Studio®

You can find the Parallel Checker as a Visual Studio® extension in the Visual Studio® Marketplace:

* [Parallel Checker Extension for Visual Studio® 2022](https://marketplace.visualstudio.com/items?itemName=LBHSR.ParallelChecker) (most recent release).
* [Parallel Checker Extension for Visual Studio® 2019](https://marketplace.visualstudio.com/items?itemName=LBHSR.HSRParallelCheckerforC7VS2017) (older version).

This is the simplest way to install the latest version.

### Prerequisites:

* Visual Studio® 2022 (Version 17.4.0 or higher)

### Notes:

* To improve compatibility and ease installation, the Parallel Checker does no longer require full solution analysis mode activated in Visual Studio.
* Parallel Checker does no longer display diagnostic information messages on successful analysis. Only in the case of internal checker errors (e.g. implementation restrictions), the information messages are displayed.

### Installation:

1. In Visual Studio® 2022: Select `Tools -> Extensions and Updates...`
2. Select `Online` and search for "Parallel Checker".
3. Select "Parallel Checker for C# 10 (VS 2022)" and install it.
4. Restart Visual Studio®.
5. Open Visual Studio® and open a solution with an empty C# console .NET application project.
6. Paste the following code into `Program.cs` that contains a data race:

        using System;
        using System.Threading.Tasks;

        class Program {
            static void Main() {
                int x = 0;
                Task.Run(() => x++);
                Console.Write(x);
            }
        }


7. Get started by trying the samples.

**Hint**: To trigger the analysis, it may help:
* Increase analysis scope: 
    - Menu "Tools"->"Options"
    - Select option page "Text Editor"->"C#"->"Advanced"
    - Change "Run background code analysis for:" to "Entire solution".
    - Change "Show compiler errors and warning for:" also to "Entire solution".
* Build the solution or edit and save the source code.

## Instructions for VS Code

To use the Parallel Checker in VS Code, you need to integrate the Parallel Checker as a NuGet analyzer package to your C# projects. Once the package is added to the projects, concurrency analysis is triggered and reported within VS Code.

* [Parallel Checker NuGet Package](https://www.nuget.org/packages/ConcurrencyLab.ParallelChecker/)

### Prerequisites:

* VS Code 1.66.2 or higher
* C# language extension in VS Code

### Installation:

1. In VS Code: Install the C# extension, if not yet done.
    * Menu `File->Preferences->Extensions`
    * Search for "C# for Visual Studio Code (powered by Omnisharp)" and install it.
2. Enable Roslyn analyzers in the setting
    * Menu `File->Preferences->Settings`
    * Select `Extensions->C# configuration`
    * Go to section `Omnisharp: Enable Roslyn Analyzers`
    * Enable the option `Enable support for roslyn analyzers, code fixes and rulesets.`
3. Add the NuGet package `ConcurrencyLab.ParallelChecker` to the desired C# projects.
    * Menu `File->Preferences->Extensions`
    * Enter: 

            dotnet add package ConcurrencyLab.ParallelChecker

4. Checker issues are displayed in the "problems" view and are also highlighted in the code.
    * Menu `View->Problems`
    * If issues do not appear: Close and reopen the C# projects, wait for a moment, optionally run the project.

### Uninstall in Visual Studio®

1. Open Visual Studio®.
2. Menu: `Tools->Extensions and Updates`.
3. In the list of installed extensions, select "Parallel Checker for C# 10 (VS 2022)".
4. Click on `Uninstall`, confirm with Yes.
5. Restart Visual Studio®.

### Known Limitations

The checker has certain implementation restrictions that are planned to be addressed in future versions. Currently, it does not analyze the following language features (skips analysis and possibly takes conservative assumptions):

* yield statement, async iterators (skipped)
* Various specific API calls
* Cancellation token
* Parallel loop break and stop
* Weak references
* Limited analysis precision on collection

---

**Notice**: Microsoft, Visual Studio, and Visual Basic are either registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

All trademarks, trade names etc. are the property of their respective owners.

OST, Parallel Checker, and the contributors DO NOT have any affiliation with any mentioned trademark holders.
