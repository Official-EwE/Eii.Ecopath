using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace DialogChecker
{
    // Given a process ID, lists every window currently open for that process - one per line,
    // tab-separated: Name, ControlType, NativeWindowHandle, X, Y, Width, Height, IsOffscreen,
    // TextContent (the Name of the first ControlType.Text descendant found, if any - the dialog's
    // own message text, for MessageBox-style dialogs; falling back to the Value of the first Edit
    // descendant's ValuePattern when no Text descendant has anything - a log-console window like
    // the legacy-import helper's progress dialog keeps its whole scrollback there instead).
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
                    // The WHOLE per-window body is wrapped here, not just the individually-risky
                    // calls below: CONFIRMED (a real run against gulfmexico.MDB, where the import
                    // fails fast and the console window can close out from under this enumeration
                    // mid-poll) that a window captured into "results" above can become stale by the
                    // time its OWN properties are read here, a few lines later - "the parent window
                    // has closed" surfaces from the UI Automation proxy layer itself
                    // (MS.Internal.AutomationProxies...) on a property that looked no different from
                    // any other up to that point (BoundingRectangle, in the case that was actually
                    // hit - previously read unprotected, unlike Name/ControlType/handle/IsOffscreen
                    // which were already behind SafeGetXXX). One stale window used to take down the
                    // ENTIRE call (exit code 2, nothing printed for ANY window, including ones that
                    // were perfectly fine) - now it's skipped on its own, with the rest of the
                    // windows still reported normally.
                    try
                    {
                        string name = SafeGetString(() => window.Current.Name);
                        string controlType = SafeGetString(() => window.Current.ControlType.ProgrammaticName);
                        long handle = SafeGetLong(() => window.Current.NativeWindowHandle);
                        var rect = SafeGetRect(() => window.Current.BoundingRectangle);
                        bool isOffscreen = SafeGetBool(() => window.Current.IsOffscreen);

                        string textContent = "";
                        try
                        {
                            var textElement = window.FindFirst(TreeScope.Descendants, textTypeCondition);
                            if (textElement != null) textContent = SafeGetString(() => textElement.Current.Name);
                        }
                        catch { }

                        // Fallback: a multi-line log console (the legacy-import helper's progress
                        // window) keeps its whole log in an Edit-shaped control's text, not any
                        // element's Name via the ControlType.Text search above (which finds nothing
                        // there at all). Only tried when the Text search came up empty, so an actual
                        // MessageBox-style dialog's real message text (found above) is never
                        // overwritten.
                        if (string.IsNullOrEmpty(textContent))
                        {
                            textContent = SafeGetEditValue(window);
                        }

                        Console.WriteLine(string.Join("\t",
                            Escape(name), controlType, handle,
                            SafeNum(rect.X), SafeNum(rect.Y), SafeNum(rect.Width), SafeNum(rect.Height),
                            isOffscreen, Escape(textContent)));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("(skipped one window - became stale while reading it: " + ex.Message + ")");
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

        // Finds the first Edit-control descendant's live Value (e.g. the full scrollback of a
        // log-console window) and returns it. Tries a direct server-side PropertyCondition search
        // first (the same kind of search that already works fine for ControlType.Text above); if
        // that comes up empty, falls back to the client-side TrueCondition+filter approach this
        // tool already relies on elsewhere for ControlType.Window, in case Edit hits the same
        // provider-boundary gap that forced that fallback originally - cheap insurance, not yet
        // confirmed to actually be needed for Edit specifically.
        static string SafeGetEditValue(AutomationElement window)
        {
            try
            {
                var editCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit);
                AutomationElement editElement = null;
                try { editElement = window.FindFirst(TreeScope.Descendants, editCondition); } catch { }

                if (editElement == null)
                {
                    try
                    {
                        var all = window.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                        foreach (AutomationElement el in all)
                        {
                            string ct = SafeGetString(() => el.Current.ControlType.ProgrammaticName);
                            if (ct == "ControlType.Edit") { editElement = el; break; }
                        }
                    }
                    catch { }
                }

                if (editElement != null)
                {
                    object patternObj;
                    if (editElement.TryGetCurrentPattern(ValuePattern.Pattern, out patternObj))
                    {
                        var valuePattern = (ValuePattern)patternObj;
                        string viaValue = SafeGetString(() => valuePattern.Current.Value);
                        if (!string.IsNullOrEmpty(viaValue)) return viaValue;
                    }
                }

                // Fallback: CONFIRMED (via the PowerShell-side Debug-EditControlText.ps1 diagnostic
                // against this exact app) that this import-log console does NOT actually expose a
                // ControlType.Edit element with a working ValuePattern at all through this process's
                // own UI Automation client stack - Accessibility Insights sees it that way, but this
                // app doesn't. What IS actually there is a plain ControlType.Pane whose underlying
                // Win32 window class name contains "EDIT" (WindowsForms10.EDIT.*), with the
                // control's entire text sitting directly in its own Name property instead of behind
                // any pattern. Matched by ClassName (not by ControlType, since ControlType.Edit is
                // exactly what this control doesn't report as, and not by searching for known log
                // text, since the log text IS the Name here - that would be circular).
                try
                {
                    var all = window.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                    foreach (AutomationElement el in all)
                    {
                        string className = SafeGetString(() => el.Current.ClassName);
                        if (className.IndexOf("EDIT", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string viaName = SafeGetString(() => el.Current.Name);
                            if (!string.IsNullOrEmpty(viaName)) return viaName;
                        }
                    }
                }
                catch { }
            }
            catch { }
            return "";
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

        static System.Windows.Rect SafeGetRect(Func<System.Windows.Rect> getter)
        {
            try { return getter(); } catch { return new System.Windows.Rect(0, 0, 0, 0); }
        }
    }
}
