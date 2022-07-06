# Installation

The Parallel Checker works in Visual Studio® and VS Code. Please refer to the corresponding section for download and installation instructions.

## Instructions for Visual Studio®

You can find the Parallel Checker as a Visual Studio® extension in the Visual Studio® Marketplace:

* [Parallel Checker Extension for Visual Studio® 2022](https://marketplace.visualstudio.com/items?itemName=LBHSR.ParallelChecker) (most recent release).
* [Parallel Checker Extension for Visual Studio® 2019](https://marketplace.visualstudio.com/items?itemName=LBHSR.HSRParallelCheckerforC7VS2017) (older version).

This is the simplest way to install the latest version.

### Prerequisites:

    Visual Studio® 2022 (Version 17.2.0 or higher)

### Notes:

1. **The checker will automatically enable C# entire solution analysis.** If you prefer to deactivate this behavior, go to `Tools->Options->Parallel Checker`, then set `Automatic full solution analysis` to `false`. Subsequently, navigate to the option `Text Editor->C#->Advanced->Run background code analysis for:` and select another value than `entire solution`. Restart VS.

2. **Checker information messages (with blue icom) are turned on by default.** If you prefer to suppress these messages, go to `Tools->Options->Parallel Checker` and set `Diagnostic Checker Information` to `false`. Reload the solution or restart VS.

3. **If checker messages are not being displayed, please activate full solution analysis manually:** Go to menu `Tools->Options`, navigate to `Text Editor->C#->Advanced`, and select `Entire solution` under `Run background code analysis for:`.

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


7. Enable information messages with the blue icon to see whether the checker has run. Checker messages will here be listed with a delay of a few seconds. The information messages can be disabled on the checker options (see notes above).
8. **Hint: To trigger the analysis, it may help to build the solution or edit and save the source code.**
9. **If checker messages are not being displayed, please activate full solution analysis manually**: Go to menu `Tools->Options`, navigate to `Text Editor->C#->Advanced`, and select `Entire solution` under `Run background code analysis for:`.
10. Get started by trying the samples.

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
3. In the list of installed extensions, select "C# Parallel Checker".
4. Click on `Uninstall`, confirm with Yes.
5. Restart Visual Studio®.

### Known Limitations

The checker has certain implementation restrictions that are planned to be addressed in future versions. Currently, it does not analyze the following language features (skips analysis and possibly takes conservative assumptions):

* yield statement, async iterators (skipped)
* Various specific API calls
* Cancellation token
* Parallel loop break and stop
* Weak references

---

**Notice**: Microsoft, Visual Studio, and Visual Basic are either registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

All trademarks, trade names etc. are the property of their respective owners.

OST, Parallel Checker, and the contributors DO NOT have any affiliation with any mentioned trademark holders.
