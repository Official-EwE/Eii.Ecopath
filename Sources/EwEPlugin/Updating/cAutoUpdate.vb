#Region " Imports "

Option Strict On
Imports System
Imports System.Net
Imports System.IO
Imports System.Reflection
Imports EwEUtils.Utilities

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
        NoUpdateAvailable = 0
        ''' <summary>A generic, unspecified error occurred.</summary>
        Failed_Generic
        ''' <summary>File was successfully updated.</summary>
        Failed_NoInternet
        ''' <summary>File failed to download.</summary>
        Failed_Download
        ''' <summary>File was successfully updated.</summary>
        Success_Updated
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

        If (assemPlugin Is Nothing) Then Return eUpdateResultTypes.NoUpdateAvailable

        Try
            If Not Me.m_service.CheckPluginUpdate(cAssemblyUtils.GetVersion(Me.m_assemCore), _
                                                  cAssemblyUtils.GetName(assemPlugin), _
                                                  cAssemblyUtils.GetToken(assemPlugin), _
                                                  cAssemblyUtils.GetVersion(assemPlugin)) Then
                Return eUpdateResultTypes.NoUpdateAvailable
            End If
        Catch ex As Exception
            ' Unable to connect to server
            Return eUpdateResultTypes.Failed_NoInternet
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
            Return eUpdateResultTypes.Failed_Download
        End Try

        Try
            ' Replace plug-in file
            File.Copy(strTemp, strPluginFile, True)
        Catch ex As Exception
            Return eUpdateResultTypes.Failed_Generic
        End Try

        Return eUpdateResultTypes.Success_Updated

    End Function

#End Region ' Public access

End Class
