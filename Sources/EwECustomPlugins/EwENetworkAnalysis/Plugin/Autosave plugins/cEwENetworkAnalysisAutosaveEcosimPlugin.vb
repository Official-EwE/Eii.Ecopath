' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.UI

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in point to connect ENA to the EwE Autosave system. This plug-in point
''' manages auto-saving of Ecosim indicators.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEwENetworkAnalysisAutosaveEcosimPlugin
    Implements IAutoSavePlugin

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndENAAutosaveSim"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
    End Sub

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Ecosim-based autosave functionality for the EwE Ecological Network Analysis plug-in"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EwE development team"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave
        Get
            Dim pi As cEwENetworkAnalysisPlugin = cEwENetworkAnalysisPlugin.thePlugin
            Return pi.Autosave(cEwENetworkAnalysisPlugin.eAutosaveType.Ecosim)
        End Get
        Set(value As Boolean)
            Dim pi As cEwENetworkAnalysisPlugin = cEwENetworkAnalysisPlugin.thePlugin
            pi.Autosave(cEwENetworkAnalysisPlugin.eAutosaveType.Ecosim) = value
        End Set
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.WRITER_ECOSIM
        End Get
    End Property

    Public Function AutoSaveType() As eAutosaveTypes Implements IAutoSavePlugin.AutoSaveType
        Return eAutosaveTypes.Ecosim
    End Function

    Public Function AutoSaveOutputPath() As String Implements IAutoSavePlugin.AutoSaveOutputPath
        Return ""
    End Function

End Class
