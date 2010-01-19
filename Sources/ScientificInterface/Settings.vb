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

    Private m_provider As New ScientificInterfaceShared.cEwESettingsProvider()

    Public Sub New()

        MyBase.New()

        ' Eradicate existing providers
        Me.Providers.Clear()
        ' Add custom provider
        Me.Providers.Add(m_provider)
        ' Hijack all existing properties
        For Each sp As Configuration.SettingsProperty In Me.Properties
            sp.Provider = m_provider
        Next

    End Sub

End Class
