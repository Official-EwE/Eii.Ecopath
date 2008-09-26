'==============================================================================
'
' $Log: Kernel32.vb,v $
' Revision 1.1  2008/09/26 07:31:13  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/08/12 23:59:41  jeroens
' Fixed GetShortPathName
'
' Revision 1.3  2008/07/24 19:42:27  jeroens
' More!
'
' Revision 1.2  2008/07/22 18:55:09  jeroens
' I like it when things grow
'
' Revision 1.1  2007/06/20 12:52:26  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports system.Runtime.InteropServices

Namespace Win32Api

    ''' <summary>
    ''' Helper class providing access to a selection of Win32 kernel32 API calls.
    ''' </summary>
    Public Class Kernel32

        <DllImport("kernel32.dll")> _
        Public Shared Function GetCurrentThreadId() As Integer
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> _
        Public Shared Function GetShortPathName(ByVal strLongPath As String, <MarshalAs(UnmanagedType.LPTStr)> ByVal strShortPath As String, <MarshalAs(UnmanagedType.U4)> ByVal bufferSize As Integer) As Integer
        End Function

    End Class

End Namespace ' Win32Api