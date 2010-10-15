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
        ''' <param name="strSecondaryOutputDirectory">Working directory</param>
        ''' -----------------------------------------------------------------------
        Public Shared Function AppExec(ByVal strAppName As String, _
                                       ByVal strOutputParameters As String, _
                                       ByRef strError As String, _
                                       Optional ByVal strPath As String = "", _
                                       Optional ByVal strSecondaryOutputDirectory As String = "") As Boolean

            ' Check if Directory is forced 
            If Not String.IsNullOrEmpty(strPath) Then
                Return ExecuteApplication(strPath, strAppName, strSecondaryOutputDirectory, strOutputParameters)
            Else
                ' Loop through all the file locations to find the files
                For Each strLocation As String In ApplicationLaunchLocations()
                    ' Execute with directory parameter
                    If ExecuteApplication(strLocation, strAppName, strOutputParameters, strError, strSecondaryOutputDirectory) Then Return True
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
        ''' <param name="strSecondaryOutputDirectory"></param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Private Shared Function ExecuteApplication(ByVal strLocationDir As String, _
                                                   ByVal strAppName As String, _
                                                   ByVal strOutputParameters As String, _
                                                   ByRef strError As String, _
                                                   Optional ByVal strSecondaryOutputDirectory As String = "") As Boolean
            Dim bSuccess As Boolean = False
            Dim strFullAppPath As String = ""

            ' Preserve the current directory
            Dim strTemDir As String = Environment.CurrentDirectory

            Try
                Environment.CurrentDirectory = strLocationDir
                strFullAppPath = Path.Combine(Path.Combine(strLocationDir, strSecondaryOutputDirectory), strAppName)
                'Check if the application EcoPath install this application or it was deleted
                If Not File.Exists(strFullAppPath) Then
                    bSuccess = False
                Else
                    'Execute external application
                    Shell(strFullAppPath & " " & strOutputParameters, AppWinStyle.NormalFocus)
                    bSuccess = True
                End If

            Catch ex As Exception
                ' JS 19ap09 (happy 4th birthday Sascha!) do not throw exception; the calling code is not ready for this
                'Throw New Exception(String.Format("Failed to load {0} with parameters {1}.  Please check if the application exist and reinstall the application.  If the issue still persist contact your application vendor.  Error: {2}.", _
                '                                   strFullAppPath, strOutputFileName, ex.ToString))
                strError = ex.Message
                bSuccess = False
            End Try

            ' Restore the current directory
            Environment.CurrentDirectory = strTemDir

            Return bSuccess
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether this application is executing in 64 bit mode.
        ''' </summary>
        ''' <returns>True if executing in 64 bit mode.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function Is64Bit() As Boolean

            ' ToDo_JS: solve this with .NET calls, does not need Win32 API calls.

            Dim hFN As Long = 0L
            Dim bIs64Bit As Boolean = False

            ' Assume initially that this is not a Wow64 process
            bIs64Bit = False

            ' Now check to see if IsWow64Process function exists
            hFN = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32"), "IsWow64Process")

            ' Does IsWow64Process function exist?
            If (hFN > 0) Then
                ' #Yes: Use the function to determine if running under Wow64
                Kernel32.IsWow64Process(Kernel32.GetCurrentProcess(), bIs64Bit)
            End If

            Return bIs64Bit

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
