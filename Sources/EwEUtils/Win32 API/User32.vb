'==============================================================================
'
' $Log: User32.vb,v $
' Revision 1.1  2008/09/26 07:31:13  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/07/22 18:18:03  jeroens
' I like it when things grow
'
' Revision 1.6  2008/01/29 15:55:32  jeroens
' Fixed CLS compliancy issues
'
' Revision 1.5  2007/10/22 01:31:02  jeroens
' * Fixed failing Windows calls
'
' Revision 1.4  2007/10/14 01:44:44  jeroens
' * Added more code. MORE CODE!
'
' Revision 1.3  2007/09/17 01:10:24  jeroens
' - Moved constants to Windows.h
'
' Revision 1.2  2007/09/16 22:58:55  jeroens
' + Mapped more methods
'
' Revision 1.1  2007/09/07 16:14:54  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEUtils.Win32Api.Win32
Imports System.Runtime.InteropServices

Namespace Win32Api

    <CLSCompliant(False)> _
    Public Class User32

        ''' <summary>
        ''' The GetAsyncKeyState function determines whether a key is up or down 
        ''' at the time the function is called, and whether the key was pressed 
        ''' after a previous call to GetAsyncKeyState.
        ''' </summary>
        ''' <param name="iKey">Virtual key code</param>
        ''' <returns>
        ''' If the function succeeds, the return value specifies whether the key 
        ''' was pressed since the last call to GetAsyncKeyState, and whether the 
        ''' key is currently up or down. If the most significant bit is set, the 
        ''' key is down, and if the least significant bit is set, the key was 
        ''' pressed after the previous call to GetAsyncKeyState. However, you 
        ''' should not rely on this last behavior; for more information, see MSDN ;)
        ''' </returns>
        Public Declare Function GetAsyncKeyState Lib "user32.dll" (ByVal iKey As Integer) As Integer

        <DllImport("user32.dll")> _
        Public Shared Function SetWindowPos(ByVal inWindow As IntPtr, ByVal hWndInsertAfter As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInteger) As Boolean
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function GetWindowRect(ByVal inWindow As IntPtr, ByRef lpRect As Win32.RECT) As Boolean
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function GetParent(ByVal inWindow As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function SetWindowText(ByVal inWindow As IntPtr, ByVal lpString As String) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Shared Function SendMessage(ByVal hWnd As HandleRef, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByRef lParam As System.Text.StringBuilder) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByRef rc As Win32.RECT) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByRef POINT As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function CreateWindowEx(ByVal dwExStyle As Integer, ByVal lpClassName As String, _
                ByVal lpWindowName As String, ByVal dwStyle As UInteger, _
                ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, _
                ByVal hWndParent As IntPtr, ByVal hMenu As IntPtr, ByVal hInstance As IntPtr, ByVal lpParam As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function GetClassName(ByVal hwnd As IntPtr, ByVal lpClassName As System.Text.StringBuilder, ByVal nMaxCount As Integer) As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function DestroyWindow(ByVal inWindow As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll")> _
        Public Shared Sub MoveWindow(ByVal hwnd As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Boolean)
        End Sub

        <DllImport("user32.dll")> _
        Public Shared Function GetDlgItem(ByVal inWindow As IntPtr, ByVal nIDDlgItem As Integer) As IntPtr
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function SetWindowsHookEx(ByVal idHook As Integer, ByVal lpfn As HookProc, ByVal hInstance As IntPtr, ByVal threadId As Integer) As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function UnhookWindowsHookEx(ByVal idHook As Integer) As Boolean
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function CallNextHookEx(ByVal idHook As Integer, ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
        End Function

        'The ToAscii function translates the specified virtual-key code 
        'and keyboard state to the corresponding character or characters. 
        'The function translates the code using the input language and 
        'physical keyboard layout identified by the keyboard layout handle.
        <DllImport("user32")> _
        Public Shared Function ToAscii(ByVal uVirtKey As Integer, ByVal uScanCode As Integer, ByVal lpbKeyState() As Byte, ByVal lpwTransKey() As Byte, ByVal fuState As Integer) As Integer
        End Function

        'The GetKeyboardState function copies the status of the 256 virtual keys to the specified buffer. 
        <DllImport("user32")> _
        Public Shared Function GetKeyboardState(ByVal pbKeyState() As Byte) As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function GetKeyState(ByVal nVirtKey As Integer) As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function GetClientRect(ByVal hwnd As IntPtr, ByRef rc As RECT) As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Shared Function ScreenToClient(ByVal hwnd As IntPtr, ByRef pt As POINT) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Shared Function GetWindowText(ByVal hwnd As IntPtr, ByVal lpString As System.Text.StringBuilder, ByVal iNumChars As Integer) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Shared Function GetWindowTextLength(ByVal hwnd As IntPtr) As Integer
        End Function

    End Class

End Namespace
