' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls

Public Class cEwEBiomassEmitterTimeseriesBuilderPlugin
    Implements IUIContextPlugin

    Private m_uic As cUIContext = Nothing

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return GetType(cEwEBiomassEmitterTimeseriesBuilderPlugin).ToString
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.CAPTION_BUILDER
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Build a time series for the Ecospace biomass emitter using two region averages CSV files, before and after the desired emission effect was implmented."
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EwE dev team"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub
End Class
