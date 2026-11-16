using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace DialogChecker
{
    // Given a process ID, lists every window currently open for that process - one per line,
    // tab-separated: Name, ControlType, NativeWindowHandle, X, Y, Width, Height, IsOffscreen,
    // TextContent (the Name of the first ControlType.Text descendant found, if any - the dialog's
    // own message text, for MessageBox-style dialogs).
    //
    // Two scopes are combined to catch every kind of window this toolkit has encountered:
    //   1. Top-level windows belonging to this PID, directly under the desktop root - catches the
    //      main window itself, AND any OTHER top-level sibling window for this same process
    //      (confirmed this session: the startup "database will be updated" prompt appears exactly
    //      this way, before the main window's own handle even exists yet).
    //   2. Descendants of each such top-level window, scoped to ControlType.Window - confirmed this
    //      session that dialogs like "Save As" and the lock-contention prompt are NESTED
    //      descendants of the main window in the tree, not separate desktop-level siblings of it,
    //      even though each appears as its own floating window on screen. Scope 1 alone misses
    //      these entirely - confirmed directly (a real Save As dialog, open at the time, did not
    //      show up at all until this second scope was added).
    //
    // Deliberately a plain, synchronous call-and-respond tool, not an event watcher: a PowerShell
    // scriptblock cast to a .NET delegate does not reliably execute when invoked from a thread other
    // than PowerShell's own (confirmed directly this session - UI Automation's WindowOpenedEvent
    // registers without error, but the callback never actually runs for ANY window). A plain
    // synchronous call like this one has no such problem.
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int pid))
            {
                Console.Error.WriteLine("Usage: DialogChecker.exe <processId> [dump]");
                return 1;
            }

            // DIAGNOSTIC ONLY - "dump" mode: no filter at all, just every descendant the .NET UI
            // Automation client can reach under the main window, to settle directly whether "Save
            // As" (or anything else that goes missing) is reachable AT ALL via this API from here,
            // regardless of which condition is used to search for it.
            if (args.Length > 1 && args[1].Equals("dump", StringComparison.OrdinalIgnoreCase))
            {
                return Dump(pid);
            }

            try
            {
                var seenHandles = new HashSet<long>();
                var results = new List<AutomationElement>();

                var pidCondition = new PropertyCondition(AutomationElement.ProcessIdProperty, pid);
                var topLevelWindows = AutomationElement.RootElement.FindAll(TreeScope.Children, pidCondition);

                // TrueCondition (everything), filtered CLIENT-SIDE by ControlType == Window - NOT a
                // PropertyCondition passed to FindAll itself, even a plain positive one: CONFIRMED
                // (the hard way, twice now) that a server-side PropertyCondition filter on
                // ControlType - negated OR positive - fails to match the Save As dialog when passed
                // directly to FindAll, while TrueCondition (no filtering at the provider level at
                // all) reliably reaches it, per this tool's own "dump" mode. So the fix combines both
                // lessons: search everything, then filter client-side afterward. ControlType.Window
                // specifically, not "has its own native window handle" (an earlier version of this
                // tool): CONFIRMED that gave false positives for ordinary, harmless UI that also gets
                // its own handle - a transient tooltip (and its small companion pane), and a menu's
                // own dropdown surface while expanded. Empirically, every genuine dialog this tool
                // has actually needed to catch (Save As, the lock-contention prompt, the
                // overwrite-confirmation prompt, even "Basic input") has consistently reported
                // exactly this ControlType.
                var textTypeCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text);

                foreach (AutomationElement topLevel in topLevelWindows)
                {
                    string topLevelControlType = SafeGetString(() => topLevel.Current.ControlType.ProgrammaticName);
                    if (topLevelControlType == "ControlType.Window")
                    {
                        AddIfNew(topLevel, results, seenHandles);
                    }

                    AutomationElementCollection nested;
                    try
                    {
                        nested = topLevel.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                    }
                    catch
                    {
                        nested = null;
                    }
                    if (nested != null)
                    {
                        foreach (AutomationElement candidate in nested)
                        {
                            string candidateControlType = SafeGetString(() => candidate.Current.ControlType.ProgrammaticName);
                            if (candidateControlType == "ControlType.Window")
                            {
                                AddIfNew(candidate, results, seenHandles);
                            }
                        }
                    }
                }

                foreach (var window in results)
                {
                    string name = SafeGetString(() => window.Current.Name);
                    string controlType = SafeGetString(() => window.Current.ControlType.ProgrammaticName);
                    long handle = SafeGetLong(() => window.Current.NativeWindowHandle);
                    var rect = window.Current.BoundingRectangle;
                    bool isOffscreen = SafeGetBool(() => window.Current.IsOffscreen);

                    string textContent = "";
                    try
                    {
                        var textElement = window.FindFirst(TreeScope.Descendants, textTypeCondition);
                        if (textElement != null) textContent = SafeGetString(() => textElement.Current.Name);
                    }
                    catch { }

                    Console.WriteLine(string.Join("\t",
                        Escape(name), controlType, handle,
                        SafeNum(rect.X), SafeNum(rect.Y), SafeNum(rect.Width), SafeNum(rect.Height),
                        isOffscreen, Escape(textContent)));
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                return 2;
            }
        }

        static int Dump(int pid)
        {
            try
            {
                var pidCondition = new PropertyCondition(AutomationElement.ProcessIdProperty, pid);
                var topLevelWindows = AutomationElement.RootElement.FindAll(TreeScope.Children, pidCondition);

                foreach (AutomationElement topLevel in topLevelWindows)
                {
                    string topName = SafeGetString(() => topLevel.Current.Name);
                    Console.WriteLine("TOP-LEVEL: " + topName);

                    AutomationElementCollection all;
                    try
                    {
                        all = topLevel.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("  (FindAll Descendants threw: " + ex.Message + ")");
                        continue;
                    }

                    Console.WriteLine("  (" + all.Count + " descendants found)");
                    foreach (AutomationElement el in all)
                    {
                        string name = SafeGetString(() => el.Current.Name);
                        string controlType = SafeGetString(() => el.Current.ControlType.ProgrammaticName);
                        long handle = SafeGetLong(() => el.Current.NativeWindowHandle);
                        Console.WriteLine(string.Join("\t", "  ", Escape(name), controlType, handle));
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                return 2;
            }
        }

        static void AddIfNew(AutomationElement element, List<AutomationElement> results, HashSet<long> seenHandles)
        {
            long handle = SafeGetLong(() => element.Current.NativeWindowHandle);
            if (handle != 0 && seenHandles.Contains(handle)) return;
            if (handle != 0) seenHandles.Add(handle);
            results.Add(element);
        }

        // Some elements (confirmed: at least one encountered mid-session) report an infinite or
        // NaN BoundingRectangle value - likely an invalid/not-yet-laid-out element. .NET's default
        // double formatting renders these as "∞"/"NaN", which a PowerShell [double] cast then fails
        // to parse. 0 is a safe substitute here: nothing downstream (Assert-NoNewDialogs) uses these
        // coordinates for anything, only Name/ControlType/NativeWindowHandle/TextContent matter.
        static double SafeNum(double value)
        {
            return (double.IsInfinity(value) || double.IsNaN(value)) ? 0 : value;
        }

        static string Escape(string s)
        {
            return (s ?? "").Replace("\t", " ").Replace("\n", " ").Replace("\r", "");
        }

        static string SafeGetString(Func<string> getter)
        {
            try { return getter() ?? ""; } catch { return ""; }
        }

        static long SafeGetLong(Func<int> getter)
        {
            try { return getter(); } catch { return 0; }
        }

        static bool SafeGetBool(Func<bool> getter)
        {
            try { return getter(); } catch { return false; }
        }
    }
}
