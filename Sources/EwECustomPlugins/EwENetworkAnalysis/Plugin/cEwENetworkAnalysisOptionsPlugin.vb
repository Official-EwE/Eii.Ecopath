' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common
Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls

Public Class cEwENetworkAnalysisOptionsPlugin
    Implements IEwEOptionsPlugin
    Implements IUIContextPlugin

    Private m_uic As cUIContext = Nothing

    Public ReadOnly Property Label As String Implements IEwEOptionsPlugin.Label
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "Network Analysis options"
        End Get
    End Property

    Private ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndENAOptions"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = CType(uic, cUIContext)
    End Sub

    Public Function IsConfigured() As Boolean Implements IConfigurable.IsConfigured
        Return True
    End Function

    Public Function GetConfigUI() As Object Implements IConfigurable.GetConfigUI
        Return New ucOptions(Me.m_uic)
    End Function

End Class
