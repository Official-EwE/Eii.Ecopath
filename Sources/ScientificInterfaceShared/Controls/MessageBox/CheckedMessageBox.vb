'==============================================================================
'
' $Log: CheckedMessageBox.vb,v $
' Revision 1.1  2008/09/26 07:31:18  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/07/24 22:43:12  jeroens
' Cleaned-up self-created checkbox properly to avoid garbage collection clashes on non-managed memory
'
' Revision 1.3  2008/07/22 20:15:56  jeroens
' Box now returns a result
'
' Revision 1.2  2008/07/22 18:58:15  jeroens
' Added namespace
'
' Revision 1.1  2008/07/22 18:56:24  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports EwEUtils.Win32Api
Imports System.Threading

#End Region ' Imports directive

Namespace Controls

    ''' ===========================================================================
    ''' <summary>
    ''' Message box extending class that adds a check box with a custom text.
    ''' </summary>
    ''' ===========================================================================
    Public Class CheckedMessageBox

#Region " Imports directive "

        Private Shared s_iHook As Integer = 0
        Private Shared s_hwnd As IntPtr = IntPtr.Zero
        Private Shared s_hwndCheckbox As IntPtr = IntPtr.Zero
        Private Shared s_bInit As Boolean = False
        Private Shared s_bChecked As Boolean = True
        Private Shared s_strCheckText As String = ""

#End Region ' Imports directive

#Region " Public interfaces "

        Public Shared Function Show(ByVal strMessage As String, ByVal strPrompt As String, _
                ByVal buttons As MessageBoxButtons, ByVal icon As MessageBoxIcon, _
                ByRef bChecked As Boolean, ByVal strCheckText As String, _
                Optional ByVal buttonDef As MessageBoxDefaultButton = 0) As DialogResult

            Dim hookProc As New Win32.HookProc(AddressOf MessageBoxHookProc)
            Dim cb As CheckBox = Nothing
            Dim t As Thread = Thread.CurrentThread
            Dim dlr As DialogResult = DialogResult.Yes

            Try

                ' == CREATE HOOK ==
                '' This throws a warning but works in VS 2005
                'CheckedMessageBox.s_iHook = User32.SetWindowsHookEx(Win32.WH.WH_CBT, hookProc, Nothing, Kernel32.GetCurrentThreadId())

                '' Does not work for Interop (http://forums.msdn.microsoft.com/en-US/csharpgeneral/thread/2ec019ad-6ba7-4791-bc2d-c05dcd133ff7/)
                'CheckedMessageBox.s_iHook = User32.SetWindowsHookEx(Win32.WH.WH_CBT, hookProc, Nothing, AppDomain.GetCurrentThreadId())

                ' Interop needs a Win32 thread. This works in VS 2005 whoohoo
                CheckedMessageBox.s_iHook = User32.SetWindowsHookEx(Win32.WH.WH_CBT, hookProc, Nothing, Kernel32.GetCurrentThreadId())

                ' Store properties for dynamic use
                CheckedMessageBox.s_bChecked = bChecked
                CheckedMessageBox.s_strCheckText = strCheckText
                dlr = MessageBox.Show(strMessage, strPrompt, buttons, icon, buttonDef)
                bChecked = CheckedMessageBox.s_bChecked

                ' == RELEASE HOOK ==
                User32.UnhookWindowsHookEx(s_iHook)
                hookProc = Nothing
                CheckedMessageBox.s_iHook = 0

                CheckedMessageBox.s_bInit = False

            Catch ex As Exception

            End Try

            Return dlr

        End Function

#End Region ' Public interfaces

#Region " Under the hood "

        Private Shared Function MessageBoxHookProc(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer

            Select Case nCode

                Case Win32.HCBT.HCBT_CREATEWND
                    ' NOP

                Case Win32.HCBT.HCBT_ACTIVATE

                    ' Not the first time
                    If (s_bInit) Then Return 0

                    CheckedMessageBox.s_bInit = True
                    CheckedMessageBox.s_hwnd = wParam

                    Dim hFont As IntPtr = IntPtr.Zero
                    Dim hwndIcon As IntPtr = User32.GetDlgItem(CheckedMessageBox.s_hwnd, &H14)
                    Dim hwndText As IntPtr = User32.GetDlgItem(CheckedMessageBox.s_hwnd, &HFFFF)
                    Dim x As Integer = 0
                    Dim y As Integer = 0
                    Dim fCur As Font = Nothing
                    Dim rc As New Win32.RECT()

                    ' Get the current font, either from the static text window or the message box itself
                    If (hwndText <> IntPtr.Zero) Then
                        hFont = User32.SendMessage(hwndText, Win32.WM.WM_GETFONT, IntPtr.Zero, IntPtr.Zero)
                    Else
                        hFont = User32.SendMessage(s_hwnd, Win32.WM.WM_GETFONT, IntPtr.Zero, IntPtr.Zero)
                    End If
                    fCur = Font.FromHfont(hFont)

                    ' Get the x coordinate for the check box.  Align it with the icon if possible, or one character height in
                    If (hwndIcon <> IntPtr.Zero) Then
                        Dim rcIcon As New Win32.RECT()
                        Dim pt As New Win32.POINT()

                        User32.GetWindowRect(hwndIcon, rcIcon)
                        pt.X = rcIcon.Left
                        pt.Y = rcIcon.Top
                        User32.ScreenToClient(s_hwnd, pt)
                        x = pt.X
                    Else
                        x = CInt(fCur.GetHeight())
                    End If

                    ' Get the y coordinate for the check box, which is the bottom of the current message box client area
                    User32.GetClientRect(CheckedMessageBox.s_hwnd, rc)
                    y = rc.Bottom - rc.Top

                    ' Resize the message box with room for the check box
                    User32.GetWindowRect(CheckedMessageBox.s_hwnd, rc)
                    User32.MoveWindow(CheckedMessageBox.s_hwnd, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top + CInt(fCur.GetHeight() * 2), True)

                    CheckedMessageBox.s_hwndCheckbox = User32.CreateWindowEx(0, "button", CheckedMessageBox.s_strCheckText, _
                            Win32.BS.BS_AUTOCHECKBOX Or Win32.WS.WS_CHILD Or Win32.WS.WS_VISIBLE Or Win32.WS.WS_TABSTOP, _
                            x, y, rc.Right - rc.Left - x, CInt(fCur.GetHeight()), _
                            CheckedMessageBox.s_hwnd, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)

                    User32.SendMessage(s_hwndCheckbox, Win32.WM.WM_SETFONT, hFont, New IntPtr(1))
                    User32.SendMessage(s_hwndCheckbox, Win32.BM.BM_SETCHECK, CType(CheckedMessageBox.s_bChecked, IntPtr), IntPtr.Zero)

                    fCur.Dispose()
                    fCur = Nothing

                Case Win32.HCBT.HCBT_DESTROYWND

                    If (s_hwnd = wParam) Then

                        Console.WriteLine("{0}: {1}, {2}", wParam, s_hwnd, s_hwndCheckbox)

                        ' Grab check box check result
                        CheckedMessageBox.s_bChecked = CBool(User32.SendMessage(s_hwndCheckbox, Win32.BM.BM_GETCHECK, IntPtr.Zero, IntPtr.Zero))

                        User32.DestroyWindow(CheckedMessageBox.s_hwndCheckbox)
                        CheckedMessageBox.s_hwndCheckbox = IntPtr.Zero

                        ' Release
                        CheckedMessageBox.s_hwnd = IntPtr.Zero
                    End If

            End Select

            Return User32.CallNextHookEx(CheckedMessageBox.s_iHook, nCode, wParam, lParam)

        End Function

#End Region ' Under the hood

    End Class

End Namespace ' Controls
