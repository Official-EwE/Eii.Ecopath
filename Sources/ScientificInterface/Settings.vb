Imports ScientificInterfaceShared
Imports System.Configuration

''' <summary>
''' Settings class that uses a custom <see cref="SettingsProvider"/>.
''' </summary>
''' <remarks>
''' For details about the overridden settings behaviour refer to <see cref="cEwESettingsProvider"/>.
''' </remarks>
Partial Friend NotInheritable Class Settings

    ''' <summary>Custom <see cref="cEwESettingsProvider">settings provider</see>.</summary>
    Private m_provider As cEwESettingsProvider = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the default value for a given settings property name.
    ''' </summary>
    ''' <param name="strName">The name of the property to access. This name is not case-sensitive.</param>
    ''' <returns>A value, or Nothing if a property by this name does not exist.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetDefaultValue(ByVal strName As String) As Object
        Dim prop As SettingsProperty = Me.Properties(strName)
        If prop IsNot Nothing Then Return prop.DefaultValue
        Return Nothing
    End Function

End Class
