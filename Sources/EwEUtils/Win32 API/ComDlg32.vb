#Region " Imports "

Option Strict Off
Imports System
Imports System.Runtime.InteropServices

#End Region ' Imports

Namespace Win32Api

    ''' <summary>
    ''' Helper class providing access to a selection of comdlg32.dll API calls.
    ''' </summary>
    Public Class ComDlg32

        Public Const OFN_ENABLEHOOK As Integer = 32
        Public Const OFN_EXPLORER As Integer = 524288
        Public Const OFN_FILEMUSTEXIST As Integer = 4096
        Public Const OFN_HIDEREADONLY As Integer = 4
        Public Const OFN_CREATEPROMPT As Integer = 8192
        Public Const OFN_NOTESTFILECREATE As Integer = 65536
        Public Const OFN_OVERWRITEPROMPT As Integer = 2
        Public Const OFN_PATHMUSTEXIST As Integer = 2048

        Public Const CDN_FILEOK As Integer = -606

        Public Delegate Function OFNHookProcDelegate(ByVal hdlg As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer

        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
        Public Structure OPENFILENAME
            Public lStructSize As Integer
            Public hwndOwner As IntPtr
            Public hInstance As Integer
            Public lpstrFilter As IntPtr
            Public lpstrCustomFilter As IntPtr
            Public nMaxCustFilter As Integer
            Public nFilterIndex As Integer
            Public lpstrFile As IntPtr
            Public nMaxFile As Integer
            Public lpstrFileTitle As IntPtr
            Public nMaxFileTitle As Integer
            Public lpstrInitialDir As IntPtr
            Public lpstrTitle As IntPtr
            Public Flags As Integer
            Public nFileOffset As Short
            Public nFileExtension As Short
            Public lpstrDefExt As IntPtr
            Public lCustData As Integer
            Public lpfnHook As OFNHookProcDelegate
            Public lpTemplateName As IntPtr
            'only if on nt 5.0 or higher
            Public pvReserved As Integer
            Public dwReserved As Integer
            Public FlagsEx As Integer
        End Structure

        Public Declare Auto Function GetSaveFileName Lib "Comdlg32.dll" Alias "GetSaveFileNameA" (<[In](), Out()> ByRef lpofn As OPENFILENAME) As Boolean

        Public Declare Function CommDlgExtendedError Lib "Comdlg32.dll" () As Integer

    End Class

End Namespace ' WIn32APi
