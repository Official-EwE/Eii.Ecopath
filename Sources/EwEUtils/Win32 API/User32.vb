#Region " Imports "

Option Strict On
Imports System
Imports EwEUtils.Win32Api.Win32
Imports System.Runtime.InteropServices

#End Region ' Imports

Namespace Win32Api

    ''' <summary>
    ''' Helper class providing access to a selection of user32.dll API calls.
    ''' </summary>
    <CLSCompliant(False)> _
       Public Class User32

        Public Enum eSystemStringTypes As UInteger
            OK = 800
            Cancel = 801
            Abort = 802
            Retry = 803
            Ignore = 804
            Yes = 805
            No = 806
            Close = 807
            Help = 808
            Repeat = 809    ' ?
            [Continue] = 810
        End Enum

        <DllImport("User32", SetLastError:=True)> _
        Public Shared Function LoadString(ByVal hInstance As IntPtr, ByVal uID As UInt32, ByVal lpBuffer As Text.StringBuilder, ByVal nBufferMax As Integer) As Integer
        End Function

    End Class

End Namespace
