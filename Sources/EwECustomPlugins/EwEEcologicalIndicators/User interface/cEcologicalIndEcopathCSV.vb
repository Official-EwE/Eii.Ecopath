' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources



''' <summary>
''' This class just serves as an UI anchor point to toggle the correct
''' setting
''' </summary>
Public Class cEcologicalIndEcopathCSV
    Implements IAutoSavePlugin

    Public Const PluginName As String = "EwEEcoIndPluginAutosaveEcopath"

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave
        Get
            Return My.Settings.AutoSaveEcopath
        End Get
        Set(value As Boolean)
            My.Settings.AutoSaveEcopath = value
            My.Settings.Save()
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return PluginName
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return Me.Description
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, My.Resources.DISPLAYNAME, "Ecopath CSV")
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Marta Coll Montón, Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "martacoll@yahoo.com"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public Function AutoSaveType() As eAutosaveTypes Implements IAutoSavePlugin.AutoSaveType
        Return eAutosaveTypes.Ecopath
    End Function

    Public Function AutoSaveOutputPath() As String Implements IAutoSavePlugin.AutoSaveOutputPath
        Return ""
    End Function

End Class
