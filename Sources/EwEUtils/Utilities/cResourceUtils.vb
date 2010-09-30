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
Imports System.Text

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a button text from Windows.
        ''' </summary>
        ''' <param name="dlr">Dialog result to return the button text for.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetButtonText(ByVal dlr As DialogResult) As String
            Dim id As Win32Api.User32.eSystemStringTypes = Win32Api.User32.eSystemStringTypes.Abort

            Select Case dlr
                Case DialogResult.OK : id = Win32Api.User32.eSystemStringTypes.OK
                Case DialogResult.Yes : id = Win32Api.User32.eSystemStringTypes.Yes
                Case DialogResult.No : id = Win32Api.User32.eSystemStringTypes.No
                Case DialogResult.Ignore : id = Win32Api.User32.eSystemStringTypes.Ignore
                Case DialogResult.Abort : id = Win32Api.User32.eSystemStringTypes.Abort
                Case DialogResult.Cancel : id = Win32Api.User32.eSystemStringTypes.Cancel
                Case DialogResult.Retry : id = Win32Api.User32.eSystemStringTypes.Retry
            End Select
            Return LoadString(id)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a system text from Windows.
        ''' </summary>
        ''' <param name="id"><see cref="Win32Api.User32.eSystemStringTypes">Windows string ID</see> to obtain.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        <CLSCompliant(False)> _
        Public Shared Function LoadString(ByVal Id As Win32Api.User32.eSystemStringTypes) As String

            Dim m_hUser32 As IntPtr = Win32Api.Kernel32.LoadLibrary("user32.dll")
            Dim sb As New StringBuilder(100)
            Dim n As Int32 = 0
            Try
                n = Win32Api.User32.LoadString(m_hUser32, Id, sb, sb.Length)
            Catch ex As Exception
                ' Whoah!
            End Try

            If Not Win32Api.Kernel32.FreeLibrary(m_hUser32) Then
                Debug.Assert(False, "Woops")
            End If

            ' Return default?
            If n = 0 Then
                Select Case Id
                    Case Win32Api.User32.eSystemStringTypes.Abort : Return "Abort"
                    Case Win32Api.User32.eSystemStringTypes.Cancel : Return "Cancel"
                    Case Win32Api.User32.eSystemStringTypes.Close : Return "Close"
                    Case Win32Api.User32.eSystemStringTypes.Continue : Return "Continue"
                    Case Win32Api.User32.eSystemStringTypes.Help : Return "Help"
                    Case Win32Api.User32.eSystemStringTypes.Repeat : Return "Repeat"
                    Case Win32Api.User32.eSystemStringTypes.Retry : Return "Retry"
                    Case Win32Api.User32.eSystemStringTypes.Ignore : Return "Ignore"
                    Case Win32Api.User32.eSystemStringTypes.No : Return "No"
                    Case Win32Api.User32.eSystemStringTypes.OK : Return "Ok"
                    Case Win32Api.User32.eSystemStringTypes.Yes : Return "Yes"
                    Case Else
                        Return ""
                End Select
            End If

            ' Truncate sb
            sb.Length = n
            Return sb.ToString

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load a resource string from a .NET assembly.
        ''' </summary>
        ''' <param name="strName"></param>
        ''' <param name="ass"></param>
        ''' <param name="strNamespace"></param>
        ''' <param name="culture"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function LoadString(ByVal strName As String, _
                                          Optional ByVal ass As Assembly = Nothing, _
                                          Optional ByVal strNamespace As String = "", _
                                          Optional ByVal culture As CultureInfo = Nothing) As String

            Dim rm As ResourceManager = Nothing
            Dim strRes As String = ""

            ' Provide defaults
            If (ass Is Nothing) Then ass = Assembly.GetExecutingAssembly()
            If (culture Is Nothing) Then culture = Threading.Thread.CurrentThread.CurrentUICulture
            If (String.IsNullOrEmpty(strNamespace)) Then strNamespace = ass.GetName.Name

            rm = New ResourceManager(strNamespace & ".resources", ass)
            Try
                strRes = rm.GetString(strName, culture)
            Catch ex As Exception

            End Try
            Return strRes

        End Function

    End Class

End Namespace
