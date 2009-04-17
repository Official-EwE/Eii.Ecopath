'==============================================================================
'
' $Log: SystemUtilities.vb,v $
' Revision 1.3  2009/04/17 03:15:58  jeroens
' Removed global message box
'
' Revision 1.2  2008/10/07 21:58:05  jeroens
' Added Is64Bit
'
' Revision 1.1  2008/09/26 07:31:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/08/20 23:07:40  sherman
' error handing for missing 16bit applications
'
' Revision 1.7  2008/08/20 00:08:47  sherman
' Allowed Flow diagram to execute programs from different locations
'
' Revision 1.6  2008/08/11 23:08:49  sherman
' Launched 16bit exe from CommonFiles folder
'
' Revision 1.5  2008/08/01 21:25:59  antonior
' *** empty log message ***
'
' Revision 1.4  2008/07/30 00:33:19  antonior
' -AppExec
'
' Revision 1.3  2008/07/29 19:32:30  antonior
' *** empty log message ***
'
' Revision 1.2  2008/07/23 19:46:42  antonior
' - Flow Diagram
' - App execution function
'
' Revision 1.1  2008/02/13 03:50:19  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.Security.Principal
Imports System.IO
Imports EwEUtils.Win32Api

#End Region ' Imports

Public Class SystemUtilities

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
    Public Shared Function AppExec(ByVal strAppName As String, ByVal strOutputParameters As String, _
                                   Optional ByVal strPath As String = "", _
                                   Optional ByVal strSecondaryOutputDirectory As String = "") As Boolean

        ' Check if Directory is forced 
        If Not String.IsNullOrEmpty(strPath) Then
            Return ExecuteApplication(strPath, strAppName, strSecondaryOutputDirectory, strOutputParameters)
        Else
            ' Loop through all the file locations to find the files
            For Each strLocation As String In ApplicationLaunchLocations()
                ' Execute with directory parameter
                If ExecuteApplication(strLocation, strAppName, strOutputParameters, strSecondaryOutputDirectory) Then Return True
                ' Execute without directory parameter
                If ExecuteApplication(strLocation, strAppName, strOutputParameters) Then Return True
            Next
        End If
        Return False

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' [to document]
    ''' </summary>
    ''' <returns></returns>
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
    ''' <param name="strOutputFileName"></param>
    ''' <param name="strSecondaryOutputDirectory"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Shared Function ExecuteApplication(ByVal strLocationDir As String, _
                                               ByVal strAppName As String, _
                                               ByVal strOutputFileName As String, _
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
            ElseIf Not File.Exists(strOutputFileName) Then
                Throw New Exception("The parameter file " & strOutputFileName & " can not be accessed in " & strFullAppPath & strOutputFileName & "\ .")
                bSuccess = False
            Else
                'Execute external application
                Shell(strFullAppPath & " " & strOutputFileName, AppWinStyle.NormalFocus)
                bSuccess = True
            End If

        Catch ex As Exception
            Throw New Exception(String.Format("Failed to load {0} with parameters {1}.  Please check if the application exist and reinstall the application.  If the issue still persist contact your application vendor.  Error: {2}.", _
                                               strFullAppPath, strOutputFileName, ex.ToString))
            bSuccess = False
        End Try

        ' Restore the current directory
        Environment.CurrentDirectory = strTemDir

        Return bSuccess
    End Function

    Public Shared Function MakeTempFile(ByVal strFileName As String) As String

        ' TODO: Concat application file name after the file directory.
        ' TODO: Check if file is writeable!!!

        Dim strOutputDir As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        'Dim strOutputDir As String = System.IO.Path.GetTempPath

        Return Path.Combine(strOutputDir, strFileName)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether this application is executing in 64 bit mode.
    ''' </summary>
    ''' <returns>True if executing in 64 bit mode.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function Is64Bit() As Boolean

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

End Class
