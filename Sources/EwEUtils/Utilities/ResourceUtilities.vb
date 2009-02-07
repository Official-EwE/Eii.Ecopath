'==============================================================================
'
' $Log: ResourceUtilities.vb,v $
' Revision 1.1  2009/02/07 20:10:25  jeroens
' Extracted from EwECore
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports System.Reflection
Imports System.Globalization
Imports System.Resources
Imports System.IO

#End Region ' Imports

Namespace Utilities

    Public Class ResourceUtilities

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Saves an embedded resource to a file
        ''' </summary>
        ''' <param name="strResourceName">The name of the resource, including  in the current assembly, current namespace.</param>
        ''' <param name="strFileName">The name of the file to save the resource to</param>
        ''' <param name="bOverwrite">States whether an existing file is allowed to be overwritten</param>
        ''' <param name="assembly">The assembly to obtain the resource from.</param>
        ''' <param name="strNamespace">The namespace to obtain the resource from.</param>
        ''' <returns>True if succesful</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function SaveResourceToFile(ByVal strResourceName As String, _
                                                  ByVal strFileName As String, _
                                                  Optional ByVal bOverwrite As Boolean = False, _
                                                  Optional ByVal assembly As Assembly = Nothing, _
                                                  Optional ByVal strNamespace As String = "") As Boolean

            Dim sResource As Stream = Nothing
            Dim sFile As FileStream = Nothing
            Dim nBufLen As Integer = 256
            Dim byBuffer(nBufLen) As Byte
            Dim nBytesRead As Integer = 0

            If Assembly Is Nothing Then
                Assembly = Assembly.GetExecutingAssembly()
            End If

            If String.IsNullOrEmpty(strNamespace) Then
                strNamespace = Assembly.GetName().Name.ToString()
            End If

            Assembly.GetManifestResourceStream(strNamespace & "." & strResourceName)

            ' Pre
            Debug.Assert(Not String.IsNullOrEmpty(strFileName), "Required target file name missing")
            Debug.Assert(sResource IsNot Nothing, String.Format("Resource {0} not found in {1}", strResourceName, strNamespace))

            ' Work with full path
            strFileName = Path.GetFullPath(strFileName)

            Try
                If (bOverwrite) Then
                    ' Create the file, overwriting any existing file with the same path
                    sFile = New FileStream(strFileName, FileMode.Create, FileAccess.Write)
                Else
                    ' Create the file but do not overwrite
                    sFile = New FileStream(strFileName, FileMode.CreateNew, FileAccess.Write)
                End If
            Catch ex As Exception
                ' Just so you know
                Debug.Print("Unable to create or overwrite file {0}", strFileName)
                ' Report failure
                Return False
            End Try

            ' Copy embedded resource to file
            nBytesRead = sResource.Read(byBuffer, 0, nBufLen)
            While (nBytesRead > 0)
                sFile.Write(byBuffer, 0, nBytesRead)
                nBytesRead = sResource.Read(byBuffer, 0, nBufLen)
            End While
            ' Done
            sFile.Close()
            Return True

        End Function

    End Class

End Namespace
