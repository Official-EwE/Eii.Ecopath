' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Xml
Imports Microsoft.Extensions.Logging
Imports EwEUtils.Logging

Namespace Configuration

    Public Interface IAppSettingsStore
        Function TryGetString(key As String, ByRef value As String) As Boolean
        Sub SetString(key As String, value As String)
        Sub Remove(key As String)
        Sub Save()
    End Interface

    <DataContract()>
    Public Class cAppSettingsDocument
        <DataMember(Name:="schemaVersion", Order:=1)>
        Public Property SchemaVersion As Integer = 1

        <DataMember(Name:="values", Order:=2)>
        Public Property Values As Dictionary(Of String, String)
    End Class

    Public Class cJsonAppSettingsStore
        Implements IAppSettingsStore

        Private Const cSCHEMAVERSION As Integer = 1
        Private Shared ReadOnly s_logger As ILogger = LoggingContext.CreateLogger(Of cJsonAppSettingsStore)()
        Private ReadOnly m_filePath As String
        Private ReadOnly m_values As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Private ReadOnly m_syncRoot As New Object

        Public Sub New(Optional filePath As String = Nothing)
            If String.IsNullOrWhiteSpace(filePath) Then
                m_filePath = GetDefaultFilePath()
            Else
                m_filePath = Path.GetFullPath(filePath)
            End If

            Load()
        End Sub

        Public ReadOnly Property FilePath As String
            Get
                Return m_filePath
            End Get
        End Property

        Public Function TryGetString(key As String, ByRef value As String) As Boolean Implements IAppSettingsStore.TryGetString
            ValidateKey(key)
            SyncLock m_syncRoot
                Return m_values.TryGetValue(key, value)
            End SyncLock
        End Function

        Public Sub SetString(key As String, value As String) Implements IAppSettingsStore.SetString
            ValidateKey(key)
            If value Is Nothing Then Throw New ArgumentNullException(NameOf(value))
            SyncLock m_syncRoot
                m_values(key) = value
            End SyncLock
        End Sub

        Public Sub Remove(key As String) Implements IAppSettingsStore.Remove
            ValidateKey(key)
            SyncLock m_syncRoot
                m_values.Remove(key)
            End SyncLock
        End Sub

        Public Sub Save() Implements IAppSettingsStore.Save
            SyncLock m_syncRoot
                Dim directoryPath As String = Path.GetDirectoryName(m_filePath)
                Directory.CreateDirectory(directoryPath)

                Dim tempPath As String = m_filePath & "." & Guid.NewGuid().ToString("N") & ".tmp"
                Try
                    Dim document As New cAppSettingsDocument With {
                        .SchemaVersion = cSCHEMAVERSION,
                        .Values = New Dictionary(Of String, String)(m_values, StringComparer.Ordinal)
                    }
                    Using stream As New FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                        CreateSerializer().WriteObject(stream, document)
                        stream.Flush(True)
                    End Using

                    If File.Exists(m_filePath) Then
#If NETFRAMEWORK Then
                        File.Replace(tempPath, m_filePath, Nothing)
#Else
                        File.Move(tempPath, m_filePath, True)
#End If
                    Else
                        File.Move(tempPath, m_filePath)
                    End If
                Finally
                    If File.Exists(tempPath) Then File.Delete(tempPath)
                End Try
            End SyncLock
        End Sub

        Private Sub Load()
            If Not File.Exists(m_filePath) Then Return

            Try
                Using stream As New FileStream(m_filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Dim document As cAppSettingsDocument = DirectCast(CreateSerializer().ReadObject(stream), cAppSettingsDocument)
                    If document Is Nothing OrElse document.SchemaVersion <> cSCHEMAVERSION OrElse document.Values Is Nothing Then
                        Throw New SerializationException("The settings document has an unsupported or invalid schema.")
                    End If
                    For Each item As KeyValuePair(Of String, String) In document.Values
                        ValidateKey(item.Key)
                        If item.Value Is Nothing Then Throw New SerializationException("The settings document contains a null value.")
                        m_values.Add(item.Key, item.Value)
                    Next
                End Using
            Catch ex As Exception When TypeOf ex Is SerializationException OrElse
                                       TypeOf ex Is XmlException OrElse
                                       TypeOf ex Is IOException OrElse
                                       TypeOf ex Is UnauthorizedAccessException OrElse
                                       TypeOf ex Is ArgumentException
                s_logger.LogError(ex, "Unable to read settings file {SettingsFile}; preserving it as a corrupt-file backup.", m_filePath)
                Dim backupPath As String = m_filePath & ".bad-" & DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")
                File.Move(m_filePath, backupPath)
                m_values.Clear()
            End Try
        End Sub

        Private Shared Function CreateSerializer() As DataContractJsonSerializer
            Return New DataContractJsonSerializer(GetType(cAppSettingsDocument), New DataContractJsonSerializerSettings With {
                .UseSimpleDictionaryFormat = True
            })
        End Function

        Private Shared Function GetDefaultFilePath() As String
            Dim configDirectory As String
            If RuntimeInformation.IsOSPlatform(OSPlatform.Linux) Then
                configDirectory = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                If String.IsNullOrWhiteSpace(configDirectory) OrElse Not Path.IsPathRooted(configDirectory) Then
                    configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config")
                End If
                configDirectory = Path.Combine(configDirectory, "Ecopath with Ecosim")
            ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
                configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Ecopath with Ecosim")
            Else
                configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ecopath with Ecosim")
            End If
            Return Path.Combine(configDirectory, "settings.json")
        End Function

        Private Shared Sub ValidateKey(key As String)
            If String.IsNullOrWhiteSpace(key) Then Throw New ArgumentException("A setting key is required.", NameOf(key))
        End Sub
    End Class

End Namespace
