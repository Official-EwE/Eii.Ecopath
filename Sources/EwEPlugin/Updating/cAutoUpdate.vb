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
        ''' <summary>File was successfully updated.</summary>
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
    ''' Download an update for a file.
    ''' </summary>
    ''' <param name="strPluginFile">The file to update.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function DownloadUpdate(ByVal strPluginFile As String) As eUpdateResultTypes

        Dim assemPlugin As AssemblyName = AssemblyName.GetAssemblyName(strPluginFile)
        Dim abPlugin() As Byte = Nothing
        Dim fsPlugin As FileStream = Nothing
        Dim strTemp As String = Path.GetTempFileName()
        Dim md5Hash As MD5 = MD5.Create()
        Dim strHash As String = ""

        If (assemPlugin Is Nothing) Then Return eUpdateResultTypes.Error_NoUpdateAvailable

        Try
            If Not Me.m_service.CheckPluginUpdate(cAssemblyUtils.GetVersion(Me.m_assemCore), _
                                                  cAssemblyUtils.GetName(assemPlugin), _
                                                  cAssemblyUtils.GetToken(assemPlugin), _
                                                  cAssemblyUtils.GetVersion(assemPlugin)) Then
                Return eUpdateResultTypes.Error_NoUpdateAvailable
            End If
        Catch ex As Exception
            ' Unable to connect to server
            Return eUpdateResultTypes.Error_Connection
        End Try

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

        ' Calculate local checksum
        strHash = md5Hash.ComputeHash(abPlugin).ToString()
        ' Check if this checksum matches the service checksum
        If Not String.Compare(strHash, Me.m_service.GetPluginHash(), True) = 0 Then
            ' Download failed
            Return eUpdateResultTypes.Error_Download
        End If

        Try
            ' Replace plug-in file
            File.Copy(strTemp, strPluginFile, True)
        Catch ex As Exception
            Return eUpdateResultTypes.Error_Replace
        End Try

        Return eUpdateResultTypes.Success

    End Function

#End Region ' Public access

End Class
