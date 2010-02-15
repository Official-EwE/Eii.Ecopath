#Region " Imports "

Option Strict On
Imports System
Imports System.Diagnostics
Imports System.Reflection
Imports System.Globalization
Imports System.Resources
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms

#End Region ' Imports

Namespace Utilities

    Public Class cResourceUtils

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Saves an embedded resource to a file
        ''' </summary>
        ''' <param name="strResourceName">The name of the resource, including  in the current assembly, current namespace.</param>
        ''' <param name="strFileName">The name of the file to save the resource to</param>
        ''' <param name="bOverwrite">States whether an existing file is allowed to be overwritten</param>
        ''' <param name="ass">The assembly to obtain the resource from.</param>
        ''' <param name="strNamespace">The namespace to obtain the resource from.</param>
        ''' <returns>True if succesful</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function SaveResourceToFile(ByVal strResourceName As String, _
                                                  ByVal strFileName As String, _
                                                  Optional ByVal bOverwrite As Boolean = False, _
                                                  Optional ByVal ass As Assembly = Nothing, _
                                                  Optional ByVal strNamespace As String = "") As Boolean

            Dim sResource As Stream = Nothing
            Dim sFile As FileStream = Nothing
            Dim nBufLen As Integer = 256
            Dim byBuffer(nBufLen) As Byte
            Dim nBytesRead As Integer = 0

            If ass Is Nothing Then
                ass = Assembly.GetExecutingAssembly()
            End If

            If String.IsNullOrEmpty(strNamespace) Then
                strNamespace = ass.GetName().Name.ToString()
            End If

            ' Cheat
            Dim astrNames As String() = ass.GetManifestResourceNames()

            sResource = ass.GetManifestResourceStream(strNamespace & "." & strResourceName)

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the system icon for a <see cref="MessageBoxIcon">message box 
        ''' icon</see> identifier.
        ''' </summary>
        ''' <param name="mbi"><see cref="MessageBoxIcon">message box icon</see>
        ''' identifier to get the system icon for.</param>
        ''' <returns>An <see cref="Icon">Icon</see>, or Nothing if the icon
        ''' could not be found.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetMessageBoxIcon(ByVal mbi As MessageBoxIcon) As Icon

            Dim objIcon As Icon = Nothing

            Select Case mbi
                Case MessageBoxIcon.Asterisk
                    objIcon = SystemIcons.Asterisk
                Case MessageBoxIcon.Error
                    objIcon = SystemIcons.Error
                Case MessageBoxIcon.Exclamation
                    objIcon = SystemIcons.Exclamation
                Case MessageBoxIcon.Hand, _
                     MessageBoxIcon.Stop
                    objIcon = SystemIcons.Hand
                Case MessageBoxIcon.Information
                    objIcon = SystemIcons.Information
                Case MessageBoxIcon.Question
                    objIcon = SystemIcons.Question
                Case MessageBoxIcon.Warning
                    objIcon = SystemIcons.Warning
                Case Else
                    ' NOP
            End Select

            Return objIcon

        End Function

        Public Shared Function GetButtonText(ByVal dlr As DialogResult) As String
            ' ToDo: localize this method
            Select Case dlr
                Case DialogResult.OK : Return "&Ok"
                Case DialogResult.Yes : Return "&Yes"
                Case DialogResult.No : Return "&No"
                Case DialogResult.Ignore : Return "&Ignore"
                Case DialogResult.Abort : Return "&Abort"
                Case DialogResult.Cancel : Return "&Cancel"
                Case DialogResult.Retry : Return "&Retry"
            End Select
            Return ""
        End Function

    End Class

End Namespace
