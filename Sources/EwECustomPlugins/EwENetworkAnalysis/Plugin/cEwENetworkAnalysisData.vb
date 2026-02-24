' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Common
Imports EwECore.Plugins.Data

Friend Class cEwENetworkAnalysisData
    Implements IPluginData
    Implements INetworkAnalysisData

    Private m_man As cNetworkManager = Nothing
    Private m_strPluginName As String = ""
    Private m_Ascendancy(6, 5) As Single
    Private m_OI As Single()

    Public Sub New(strPluginName As String,
                   man As cNetworkManager)
        Me.m_strPluginName = strPluginName
        Me.m_man = man
    End Sub

    Public ReadOnly Property PluginName() As String _
        Implements IPluginData.PluginName
        Get
            Return Me.m_strPluginName
        End Get
    End Property

    Public ReadOnly Property Ascendancy() As Single(,) _
        Implements INetworkAnalysisData.Ascendancy
        Get
            Return Me.m_Ascendancy
        End Get
    End Property

    Public ReadOnly Property OmnivoryIndex As Single()
        Get
            Return Me.m_OI
        End Get
    End Property

    Public ReadOnly Property RunType() As IRunType _
        Implements IPluginData.RunType
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property LIndex As Single() _
        Implements INetworkAnalysisData.LIndex
        Get
            Me.m_man.RunRequiredPrimaryProd()
            Dim data(Me.m_man.nGroups) As Single
            For i As Integer = 1 To Me.m_man.nGroups
                data(i) = Me.m_man.Lindex(i)
            Next
            Return data
        End Get
    End Property

    Friend Sub Resize(core As cCore)
        ReDim Me.m_OI(core.nGroups)
    End Sub
End Class
