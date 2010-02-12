#Region " Imports "

Option Strict On
Imports System
Imports Microsoft.Win32

#End Region ' Imports

Namespace Utilities

    ''' =======================================================================
    ''' <summary>
    ''' Helper class providing standardized access to the Windows registry.
    ''' </summary>
    ''' =======================================================================
    Public Class cRegistryUtils

        Public Shared Function ReadKey(ByVal keyParent As RegistryKey, ByVal strSubKey As String, ByVal strValueName As String) As String

            Dim Key As RegistryKey = Nothing
            Dim strValue As String = ""

            Try
                'Open the registry key.
                Key = keyParent.OpenSubKey(strSubKey, True)
                If Key Is Nothing Then 'if the key doesn't exist
                    strValue = ""
                End If

                'Get the value.
                strValue = Convert.ToString(Key.GetValue(strValueName))

            Catch e As Exception
            End Try

            Return strValue

        End Function

    End Class

End Namespace ' Utilities
