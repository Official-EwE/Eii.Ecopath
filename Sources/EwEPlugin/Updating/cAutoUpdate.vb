#Region " Imports "

Option Strict On
Imports System
Imports System.Net
Imports System.IO
Imports System.Reflection
Imports EwEUtils.Utilities
Imports System.Security.Cryptography

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Helper class to update a plug-in assembly from the EwE web service.
''' </summary>
''' ===========================================================================
Friend Class cAutoUpdate

#Region " Private vars "

    ''' <summary>The core assembly to verify plug-in versions against.</summary>
    Private m_assemCore As AssemblyName = Nothing
    ''' <summary>The update service.</summary>
    Private m_service As EwEAutoUpdateRef.UpdateService = Nothing
    ''' <summary>Update session cookies.</summary>
    Private m_cookiejar As CookieContainer = Nothing

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new cAutoUpdate instance.
    ''' </summary>
    ''' <param name="assemCore">The core assembly to download updates for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal assemCore As AssemblyName)

        ' Store refs
        Me.m_assemCore = assemCore
        Me.m_cookiejar = New CookieContainer()
        Me.m_service = New EwEAutoUpdateRef.UpdateService()
        Me.m_service.CookieContainer = Me.m_cookiejar

    End Sub

#End Region ' Constructor

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type stating update attempt results.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eUpdateResultTypes As Integer
        ''' <summary>Operation successful.</summary>
        Success = 0
        ''' <summary>Update not available for a given assembly.</summary>
        Error_NoUpdateAvailable
        ''' <summary>Update webservice could not be connected.</summary>
        Error_Connection
        ''' <summary>File failed to download.</summary>
        Error_Download
        ''' <summary>Failed to replace a plug-in on the system.</summary>
        Error_Replace
        ''' <summary>A generic, unspecified error occurred.</summary>
        Error_Generic
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, states if an update is available for a given assembly.
    ''' </summary>
    ''' <param name="strPluginFile">The file to check updates for.</param>
    ''' <returns>
    ''' <para>An <see cref="eUpdateResultTypes">update result indicator</see>,
    ''' which are to be interpreted as follows:</para>
    ''' <list type="bullet">
    ''' <item>
    ''' <term><see cref="eUpdateResultTypes.Success">Success</see></term>
    ''' <description>An update is available.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateResultTypes.Error_NoUpdateAvailable">Error_NoUpdateAvailable</see></term>
    ''' <description>An update is not available.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateResultTypes.Error_Connection">Error_Connection</see></term>
    ''' <description>Connection to update server could not be established.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateResultTypes.Error_Generic">Error_Generic</see></term>
    ''' <description>Something else went wrong.</description>
    ''' </item>
    ''' </list>
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function HasUpdate(ByVal strPluginFile As String) As eUpdateResultTypes
        Dim assemPlugin As AssemblyName = Nothing
        Try
            assemPlugin = AssemblyName.GetAssemblyName(strPluginFile)
        Catch ex As Exception
            assemPlugin = Nothing
        End Try

        If (assemPlugin Is Nothing) Then
            Return eUpdateResultTypes.Error_Generic
        End If

        ' Perform local version check first
        If assemPlugin.Version.CompareTo(m_assemCore.Version) >= 0 Then
            Return eUpdateResultTypes.Error_NoUpdateAvailable
        End If

        Try
            If Me.m_service.CheckPluginUpdate(cAssemblyUtils.GetVersion(Me.m_assemCore), _
                                                  cAssemblyUtils.GetName(assemPlugin), _
                                                  cAssemblyUtils.GetToken(assemPlugin), _
                                                  cAssemblyUtils.GetVersion(assemPlugin)) Then
                Return eUpdateResultTypes.Success
            End If
        Catch ex As Exception
            ' Unable to connect to server
        End Try
        Return eUpdateResultTypes.Error_Connection

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Download an update for a file.
    ''' </summary>
    ''' <param name="strPluginFile">The file to update.</param>
    ''' <returns>
    ''' <para>An <see cref="eUpdateResultTypes">update result indicator</see>,
    ''' which are to be interpreted as follows:</para>
    ''' <list type="bullet">
    ''' <item>
    ''' <term><see cref="eUpdateResultTypes.Success">Success</see></term>
    ''' <description>Update was downloaded and copied succesfully.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateResultTypes.Error_Download">Error_Download</see></term>
    ''' <description>Failed to correctly download the update.</description>
    ''' </item>
    ''' <item>
    ''' <term><see cref="eUpdateResultTypes.Error_Replace">Error_Replace</see></term>
    ''' <description>Failed to replace local plug-in file with update.</description>
    ''' </item>
    ''' </list>
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function DownloadUpdate(ByVal strPluginFile As String) As eUpdateResultTypes

        Dim abPlugin() As Byte = Nothing
        Dim fsPlugin As FileStream = Nothing
        Dim strTemp As String = Path.GetTempFileName()
        Dim md5Hash As MD5 = MD5.Create()
        Dim strHashLocal As String = ""

        Try
            ' Download to a temp location
            abPlugin = Me.m_service.DownloadPlugin()
            fsPlugin = New FileStream(strTemp, FileMode.Create)
            fsPlugin.Write(abPlugin, 0, abPlugin.Length)
            fsPlugin.Close()
            fsPlugin = Nothing
        Catch ex As Exception
            ' Error downloading update
            Return eUpdateResultTypes.Error_Download
        End Try

        Try
            ' Calculate local checksum
            strHashLocal = cStringUtils.ToHexString(md5Hash.ComputeHash(abPlugin))

            ' Does checksum match the service checksum?
            If Not String.Compare(strHashLocal, Me.m_service.GetPluginHash(), True) = 0 Then
                ' #No: download failed
                Return eUpdateResultTypes.Error_Download
            End If
        Catch ex As Exception
            ' Error downloading hash
            Return eUpdateResultTypes.Error_Download
        End Try

        Try
            ' Replace plug-in file
            File.Copy(strTemp, strPluginFile, True)
        Catch ex As Exception
            ' Unable to overwrite plug-in dll, maybe it's in use?
            Return eUpdateResultTypes.Error_Replace
        End Try

        Try
            ' Delete temp file
            File.Delete(strTemp)
        Catch ex As Exception
            ' Hmm, ok, allow this
        End Try

        ' Yippee
        Return eUpdateResultTypes.Success

    End Function

#End Region ' Public access

End Class
