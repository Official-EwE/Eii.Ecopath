'==============================================================================
'
' $Log: SystemUtilities.vb,v $
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
    ''' <summary>
    ''' Get the current username
    ''' </summary>
    ''' <returns>current username</returns>
    ''' <remarks></remarks>
    Public Shared Function GetUserName() As String
        Return System.Security.Principal.WindowsIdentity.GetCurrent.Name
    End Function

    ''' <summary>
    ''' Function that execute external applications for all plug-ins
    ''' </summary>
    ''' <param name="PlugInDir">PlugInDir point to the plug-in folder</param>
    ''' <param name="App">App point to application file with extension</param>
    ''' <param name="OutPutParam">OutPutParam point filename with extension to use as a paramater to the external application</param>
    ''' <param name="SecondaryOutputDirectory">OutPutFildeDir point to the folder of the external application should read the parameter file</param>
    Public Shared Function AppExec(ByVal App As String, ByVal OutPutParam As String, Optional ByVal PlugInDir As String = "", Optional ByVal SecondaryOutputDirectory As String = "") As Boolean
        Dim retVal As Boolean = False

        ' Check if Directory is forced 
        If PlugInDir <> "" Then
            Return ExecuteApplication(PlugInDir, App, SecondaryOutputDirectory, OutPutParam)
        Else
            ' Loop through all the file locations to find the files
            For Each location As String In ApplicationLaunchLocations()
                ' Execute with directory parameter
                If ExecuteApplication(location, App, OutPutParam, SecondaryOutputDirectory) Then Return True
                ' Execute without directory parameter
                If ExecuteApplication(location, App, OutPutParam) Then Return True
            Next
        End If

        Return False
    End Function

    Public Shared Function ApplicationLaunchLocations() As String()
        Return New String() {Mid(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), 7), _
                                     Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) & "\Ecopath"}
    End Function

    Public Shared Sub PrintFileNotFoundError()
        Dim errorMessage As String = "Failed to load application.  Application may not exist.  Please check the following locations then contact your application vendor: "
        For Each str As String In EwEUtils.SystemUtilities.ApplicationLaunchLocations
            errorMessage = errorMessage & " " & str & ";"
        Next
        MsgBox(errorMessage)
    End Sub

    Private Shared Function ExecuteApplication(ByVal LocationDir As String, ByVal App As String, ByVal OutPutFileName As String, Optional ByVal SecondaryOutputDirectory As String = "") As Boolean
        Dim retVal As Boolean = False
        Dim FullAppPath As String

        ' Set the current directory
        Dim tempCurrentDirectory As String = Environment.CurrentDirectory
        Environment.CurrentDirectory = LocationDir
        FullAppPath = Path.Combine(Path.Combine(LocationDir, SecondaryOutputDirectory), App)

        Try
            'Check if the application EcoPath install this application or it was deleted
            If Not File.Exists(FullAppPath) Then
                retVal = False
            ElseIf Not File.Exists(OutPutFileName) Then
                Throw New Exception("The parameter file " & OutPutFileName & " can not be accessed in " & FullAppPath & OutPutFileName & "\ .")
                retVal = False
            Else
                'Execute external application
                Shell(FullAppPath & " " & OutPutFileName, AppWinStyle.NormalFocus)
                retVal = True
            End If

        Catch ex As Exception
            Throw New Exception(String.Format("Failed to load {0} with parameters {1}.  Please check if the application exist and reinstall the application.  If the issue still persist contact your application vendor.  Error: {2}.", _
                                                FullAppPath, OutPutFileName, ex.ToString))
            retVal = False
        End Try

        ' Set the app diretory back
        Environment.CurrentDirectory = tempCurrentDirectory

        Return retVal
    End Function

    Public Shared Function MakeTempFile(ByVal strFileName As String) As String

        ' TODO: Concat application file name after the file directory.
        ' TODO: Check if file is writeable!!!

        Dim strOutputDir As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        'Dim strOutputDir As String = System.IO.Path.GetTempPath

        Return Path.Combine(strOutputDir, strFileName)

    End Function

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
