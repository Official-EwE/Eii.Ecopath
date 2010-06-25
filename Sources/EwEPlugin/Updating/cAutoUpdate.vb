#Region " Imports "

Option Strict On
Imports System
Imports System.Net
Imports System.IO
Imports System.Reflection
Imports System.Diagnostics
Imports EwEUtils.Utilities
Imports System.Security.Cryptography
Imports System.Windows.Forms

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Helper class to update a plug-in assembly from the EwE web service.
''' </summary>
''' <remarks>
''' <para>The cAutoUpdate class should be used as follows:</para>
''' <code>
''' 
''' </code>
''' </remarks>
''' ===========================================================================
Friend Class cAutoUpdate

#Region " Private vars "

    ''' <summary>The update service.</summary>
    Private m_service As EwEAutoUpdateRef.UpdateService = Nothing
    ''' <summary>Update session cookies.</summary>
    Private m_cookiejar As CookieContainer = Nothing

    ''' <summary>Attached file name.</summary>
    Private m_strFile As String = ""

    ''' <summary>Attached core version.</summary>
    Private m_verCore As Version = Nothing
    ''' <summary>Attached plug-in version.</summary>
    Private m_verPlugin As Version = Nothing
    ''' <summary>Attached plug-in short file name.</summary>
    Private m_strPluginName As String = ""
    ''' <summary>Attached plug-in public hash key token.</summary>
    ''' <remarks>For strong-named assemblies only.</remarks>
    Private m_strPluginToken As String = ""

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new cAutoUpdate instance.
    ''' </summary>
    ''' <param name="core">The core assembly to download updates for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal core As Object)

        Me.m_verCore = Me.CoreVersion(core)
        Me.m_cookiejar = New CookieContainer()
        Me.m_service = New EwEAutoUpdateRef.UpdateService()
        Me.m_service.CookieContainer = Me.m_cookiejar

    End Sub

#End Region ' Constructor

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type stating update status results.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eUpdateStatusTypes As Integer

        ''' <summary>All good. Blue skies, happy children, money in the bank; the works.</summary>
        Success = 0
        ''' <summary>A migration is available.</summary>
        Info_CanMigrate
        ''' <summary>An update is available.</summary>
        Info_CanUpdate
        ''' <summary>Update webservice could not be connected.</summary>
        Error_Connection
        ''' <summary>File failed to download.</summary>
        Error_Download
        ''' <summary>Failed to replace a plug-in on the system.</summary>
        Error_Replace
        ''' <summary>The updater was not properly initialized.</summary>
        Error_Initialized

    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Attach a file to the updater.
    ''' </summary>
    ''' <param name="strFile"></param>
    ''' <returns>True if this is a valid assembly.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AttachAssembly(ByVal strFile As String) As Boolean

        Dim assemPlugin As AssemblyName = Nothing

        ' Reset
        Me.m_strFile = ""

        Try
            assemPlugin = AssemblyName.GetAssemblyName(strFile)
        Catch ex As Exception
            assemPlugin = Nothing
        End Try

        If (assemPlugin Is Nothing) Then
            Return False
        End If

        Try
            ' Grab details
            Me.m_strPluginName = cAssemblyUtils.GetName(assemPlugin)
            Me.m_strPluginToken = cAssemblyUtils.GetToken(assemPlugin)
            Me.m_verPlugin = cAssemblyUtils.GetVersion(assemPlugin)
        Catch e As Exception
            Return False
        End Try

        ' Remember file
        Me.m_strFile = strFile

        ' Ok
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check for updates on the attached assembly.
    ''' </summary>
    ''' <para>An <see cref="eUpdateStatusTypes">update status</see>flag, which 
    ''' is to be interpreted as follows:</para>
    ''' <list type="table">
    ''' <listheader><term>Flag</term><description>Description</description></listheader>
    ''' <item>
    ''' <term><see cref="eUpdateStatusTypes.Success">Success</see></term>
    ''' <description>The file is in a proper state.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateStatusTypes.Info_CanMigrate">Info_CanMigrate</see></term>
    ''' <description>An migration from weak-named to strong-named is available.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateStatusTypes.Info_CanUpdate">Info_CanUpdate</see></term>
    ''' <description>An update is available.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateStatusTypes.Error_Connection">Error_Connection</see></term>
    ''' <description>Connection to update server could not be established.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateStatusTypes.Error_Initialized">Error_Generic</see></term>
    ''' <description>The updater was not properly initialized.</description>
    ''' </item>
    ''' </list>
    ''' -----------------------------------------------------------------------
    Public Function CheckForUpdate() As eUpdateStatusTypes

        If String.IsNullOrEmpty(Me.m_strPluginName) Then
            Return eUpdateStatusTypes.Error_Initialized
        End If

        ' Perform local version check first
        If Me.m_verPlugin.CompareTo(Me.m_verCore) >= 0 Then
            Return eUpdateStatusTypes.Success
        End If

        If String.IsNullOrEmpty(Me.m_strPluginToken) Then
            Return Me.HasMigration()
        Else
            If Me.HasUpdate() = eUpdateStatusTypes.Success Then
                Return eUpdateStatusTypes.Info_CanUpdate
            Else
                Return eUpdateStatusTypes.Error_Connection
            End If
        End If

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Download an update for a file.
    ''' </summary>
    ''' <returns>
    ''' <para>An <see cref="eUpdateStatusTypes">update result indicator</see>,
    ''' which are to be interpreted as follows:</para>
    ''' <list type="table">
    ''' <listheader><term>Flag</term><description>Description</description></listheader>
    ''' <item>
    ''' <term><see cref="eUpdateStatusTypes.Success">Success</see></term>
    ''' <description>Update was downloaded and copied succesfully.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateStatusTypes.Error_Download">Error_Download</see></term>
    ''' <description>Failed to correctly download the update.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateStatusTypes.Error_Replace">Error_Replace</see></term>
    ''' <description>Failed to replace the local plug-in file with the downloaded file.</description>
    ''' </item>
    ''' </list>
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function DownloadUpdate() As eUpdateStatusTypes

        Dim abPlugin() As Byte = Nothing
        Dim fsPlugin As FileStream = Nothing
        Dim strTemp As String = Path.GetTempFileName()
        Dim md5Hash As MD5 = MD5.Create()
        Dim strHashLocal As String = ""


        If String.IsNullOrEmpty(Me.m_strPluginName) Then
            Return eUpdateStatusTypes.Error_Initialized
        End If

        Try
            ' Download to a temp location
            abPlugin = Me.m_service.DownloadPlugin()
            fsPlugin = New FileStream(strTemp, FileMode.Create)
            fsPlugin.Write(abPlugin, 0, abPlugin.Length)
            fsPlugin.Close()
            fsPlugin = Nothing
        Catch ex As Exception
            ' Error downloading update
            Return eUpdateStatusTypes.Error_Download
        End Try

        Try
            ' Calculate local checksum
            strHashLocal = cStringUtils.ToHexString(md5Hash.ComputeHash(abPlugin))

            ' Does checksum match the service checksum?
            If Not String.Compare(strHashLocal, Me.m_service.GetPluginHash(), True) = 0 Then
                ' #No: download failed
                Return eUpdateStatusTypes.Error_Download
            End If
        Catch ex As Exception
            ' Error downloading hash
            Return eUpdateStatusTypes.Error_Download
        End Try

        Try
            ' Replace plug-in file
            File.Copy(strTemp, Me.m_strFile, True)
        Catch ex As Exception
            ' Unable to overwrite plug-in dll, maybe it's in use?
            Return eUpdateStatusTypes.Error_Replace
        End Try

        Try
            ' Delete temp file
            File.Delete(strTemp)
        Catch ex As Exception
            ' Hmm, ok, allow this
        End Try

        ' Yippee
        Return eUpdateStatusTypes.Success

    End Function

#End Region ' Public access

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, get the version of the core assembly.
    ''' </summary>
    ''' <param name="core">The core object to query the assembly for.</param>
    ''' -----------------------------------------------------------------------
    Private ReadOnly Property CoreVersion(ByVal core As Object) As Version
        Get
            Dim anCore As AssemblyName = cAssemblyUtils.GetAssemblyName(core.GetType())
            Return cAssemblyUtils.GetVersion(anCore)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, states whether a migration is available for the attached 
    ''' file.
    ''' </summary>
    ''' <returns>
    ''' <see cref="eUpdateStatusTypes.Info_CanMigrate">Info_CanMigrate</see>
    ''' if a migration is available, <see cref="eUpdateStatusTypes.Error_Connection">Error_Connection</see>
    ''' otherwise.
    ''' </returns>
    ''' <remarks>
    ''' Note that this check should only be performed on weak-named assemblies.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function HasMigration() As eUpdateStatusTypes

        Debug.Assert(String.IsNullOrEmpty(Me.m_strPluginToken), "Assembly is not weak-named")
        Debug.Assert(Me.m_verCore IsNot Nothing, "Something is VERY wrong")
        Debug.Assert(Me.m_verPlugin IsNot Nothing, "Something is VERY wrong")

        Try
            Me.m_strPluginToken = Me.m_service.GetPluginMigrationToken(Me.m_verCore.ToString, Me.m_strPluginName, Me.m_verPlugin.ToString)

            If Not String.IsNullOrEmpty(Me.m_strPluginToken) Then
                Return eUpdateStatusTypes.Info_CanMigrate
            End If

        Catch ex As Exception
            ' Unable to connect to server
        End Try

        Return eUpdateStatusTypes.Error_Connection

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, states if an update is available for a given assembly.
    ''' </summary>
    ''' <returns>
    ''' <see cref="eUpdateStatusTypes.Success">Success</see>
    ''' if a migration is available, <see cref="eUpdateStatusTypes.Error_Connection">Error_Connection</see>
    ''' otherwise.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Function HasUpdate() As eUpdateStatusTypes

        Debug.Assert(Me.m_verCore IsNot Nothing, "Something is VERY wrong")
        Debug.Assert(Me.m_verPlugin IsNot Nothing, "Something is VERY wrong")

        Try
            If Me.m_service.CheckPluginUpdate(Me.m_verCore.ToString, Me.m_strPluginName, Me.m_strPluginToken, Me.m_verPlugin.ToString) Then
                Return eUpdateStatusTypes.Success
            End If

        Catch ex As Exception
            ' Unable to connect to server
        End Try
        Return eUpdateStatusTypes.Error_Connection

    End Function

#End Region ' Internals

End Class
