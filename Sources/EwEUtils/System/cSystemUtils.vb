#Region " Imports "

Option Strict On
Imports System
Imports System.Security.Principal
Imports System.IO
Imports Microsoft.VisualBasic
Imports EwEUtils.Win32Api
Imports EwEUtils.Utilities
Imports System.Security.AccessControl

#End Region ' Imports

Namespace SystemUtilities

    Public Class cSystemUtils

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the username for the active logged-in user.
        ''' </summary>
        ''' <returns>The username for the active logged-in user.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetUserName() As String
            Return System.Security.Principal.WindowsIdentity.GetCurrent.Name
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Function that execute external applications for all plug-ins
        ''' </summary>
        ''' <param name="strAppName">Name of the executable to execute (including extension)</param>
        ''' <param name="strPath">Application path to use. Provide an empty string
        ''' here to detect the application file in all possible locations.</param>
        ''' <param name="strOutputParameters">Arguments to pass to the executable.</param>
        ''' -----------------------------------------------------------------------
        Public Shared Function AppExec(ByVal strAppName As String, _
                                       ByVal strOutputParameters As String, _
                                       ByRef strError As String, _
                                       Optional ByVal strPath As String = "") As Boolean

            ' Check if Directory is forced 
            If Not String.IsNullOrEmpty(strPath) Then
                Return ExecuteApplication(strPath, strAppName, strOutputParameters, strError)
            Else
                ' Loop through all the file locations to find the files
                For Each strLocation As String In ApplicationLaunchLocations()
                    ' Execute without directory parameter
                    If ExecuteApplication(strLocation, strAppName, strOutputParameters, strError) Then Return True
                Next
            End If
            Return False

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns an array of possible application locations.</summary>
        ''' <returns>
        ''' </returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function ApplicationLaunchLocations() As String()
            Return New String() {Mid(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), 7), _
                                 Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) & "\Ecopath"}
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' [to document]
        ''' </summary>
        ''' <param name="strLocationDir"></param>
        ''' <param name="strAppName"></param>
        ''' <param name="strOutputParameters"></param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Private Shared Function ExecuteApplication(ByVal strLocationDir As String, _
                                                   ByVal strAppName As String, _
                                                   ByVal strOutputParameters As String, _
                                                   ByRef strError As String) As Boolean
            Dim proc As New System.Diagnostics.Process()
            Dim bSuccess As Boolean = False
            Dim strFullAppPath As String = ""

            ' Preserve the current directory
            Dim strTemDir As String = Environment.CurrentDirectory

            Try
                Environment.CurrentDirectory = strLocationDir
                strFullAppPath = Path.Combine(strLocationDir, strAppName)
                'Check if the application EcoPath install this application or it was deleted
                If Not File.Exists(strFullAppPath) Then
                    bSuccess = False
                Else
                    'Execute external application
                    proc.EnableRaisingEvents = False
                    proc.StartInfo.FileName = strFullAppPath
                    proc.StartInfo.Arguments = strOutputParameters
                    proc.Start()

                    bSuccess = True
                End If

            Catch ex As Exception
                ' JS 19ap09 (happy 4th birthday Sascha!) do not throw exception; the calling code is not ready for this
                'Throw New Exception(String.Format("Failed to load {0} with parameters {1}.  Please check if the application exist and reinstall the application.  If the issue still persist contact your application vendor.  Error: {2}.", _
                '                                   strFullAppPath, strOutputFileName, ex.ToString))
                strError = ex.Message
                ' Fix faulty Win7 exception text
                If strError.IndexOf("%1") > -1 Then
                    strError = strError.Replace("%1", strAppName)
                End If
                bSuccess = False
            End Try

            ' Restore the current directory
            Environment.CurrentDirectory = strTemDir

            Return bSuccess
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether this application is running in 64 bit mode.
        ''' </summary>
        ''' <returns>True if running in 64 bit mode.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function Is64Bit() As Boolean

            Return (System.Runtime.InteropServices.Marshal.SizeOf(GetType(IntPtr)) = 8)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the OS is 64 bit.
        ''' </summary>
        ''' <returns>True if the OS is 64 bit.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function Is64BitOS() As Boolean

            Return System.Environment.Is64BitOperatingSystem

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether this application is running with administrator privileges.
        ''' </summary>
        ''' <returns>True if running with administrator privileges.</returns>
        ''' <remarks>
        ''' http://www.codekeep.net/snippets/16758a1f-6186-47a7-98ba-30449fe74cda.aspx
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function IsAdministrator() As Boolean

            Dim identity As WindowsIdentity = WindowsIdentity.GetCurrent()
            Dim principal As New WindowsPrincipal(identity)
            Return principal.IsInRole(WindowsBuiltInRole.Administrator)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' States if running Win 7 or higher.
        ''' </summary>
        ''' <returns>True if running Win 7 or higher.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function IsRunningWin7OrHigher() As Boolean

            Dim os As System.OperatingSystem = System.Environment.OSVersion
            Dim ver As Version = os.Version

            If (os.Platform <> PlatformID.Win32NT) Then Return False
            If (ver.Major < 6) Then Return False
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the path for storing application settings
        ''' </summary>
        ''' <param name="bPerUserSetting">States if this is a per-user setting
        ''' (True) or a setting for all users (False).</param>
        ''' <param name="strApplication">Application to obtain the path for.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ApplicationSettingsPath(Optional ByVal bPerUserSetting As Boolean = True, _
                                                       Optional ByVal strApplication As String = "Ecopath with Ecosim") As String

            Dim strBaseDir As String = ""
            Dim strPath As String = ""

            If bPerUserSetting Then
                strBaseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            Else
                strBaseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            End If

            If (Not String.IsNullOrEmpty(strApplication)) Then
                strPath = Path.Combine(strBaseDir, cFileUtils.ToValidFileName(strApplication, False))
            End If
            If Not Directory.Exists(strPath) Then
                Try
                    Directory.CreateDirectory(strPath)
                Catch ex As Exception

                End Try
            End If
            Return strPath

        End Function

    End Class

End Namespace ' SystemUtilities
