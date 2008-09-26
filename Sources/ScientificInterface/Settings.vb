'This class allows you to handle specific events on the settings class:
' The SettingChanging event is raised before a setting's value is changed.
' The PropertyChanged event is raised after a setting's value is changed.
' The SettingsLoaded event is raised after the setting values are loaded.
' The SettingsSaving event is raised before the setting values are saved.
Partial Friend NotInheritable Class Settings

    Private m_provider As New ScientificInterfaceShared.cEwESettingsProvider()

    Public Sub New()
        Me.Providers.Add(m_provider)
        For Each sp As Configuration.SettingsProperty In Me.Properties
            sp.Provider = m_provider
        Next
    End Sub

End Class
