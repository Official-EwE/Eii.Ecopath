Imports ScientificInterfaceShared
Imports System.Configuration

''' <summary>
''' <para>This class allows you to handle specific events on the settings class:</para>
''' <list type="bullet">
''' <item>The SettingChanging event is raised before a setting's value is changed.</item>
''' <item>The PropertyChanged event is raised after a setting's value is changed.</item>
''' <item>The SettingsLoaded event is raised after the setting values are loaded.</item>
''' <item>The SettingsSaving event is raised before the setting values are saved.</item>
''' </list>
''' </summary>
''' <remarks></remarks>
Partial Friend NotInheritable Class Settings

    Private m_provider As cEwESettingsProvider = Nothing

    Public Sub New()

        MyBase.New()

        Me.m_provider = New cEwESettingsProvider()

        ' Eradicate existing providers
        Me.Providers.Clear()
        ' Add custom provider
        Me.Providers.Add(Me.m_provider)
        ' Hijack all existing properties
        For Each sp As SettingsProperty In Me.Properties
            sp.Provider = Me.m_provider
        Next

    End Sub

    Public Function GetDefaultValue(ByVal strName As String) As Object
        Try
            Dim prop As SettingsProperty = Me.Properties(strName)
            Return prop.DefaultValue
        Catch ex As Exception

        End Try
        Return Nothing
    End Function

End Class
