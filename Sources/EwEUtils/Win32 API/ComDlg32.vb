'==============================================================================
'
' $Log: ComDlg32.vb,v $
' Revision 1.1  2008/09/26 07:31:13  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2007/10/22 01:31:02  jeroens
' * Fixed failing Windows calls
'
' Revision 1.3  2007/09/17 16:04:56  jeroens
' + [In()], Out()
'
' Revision 1.2  2007/09/17 02:43:52  jeroens
' * Marshalling headache replaced by intptr; makes struct harder to use but it works significantly better
'
' Revision 1.1  2007/09/16 22:58:07  jeroens
' Initial version
'
'==============================================================================

Option Strict Off
Imports System.Runtime.InteropServices

Namespace Win32Api

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
