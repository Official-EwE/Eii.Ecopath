' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class cPrebalPlotData

#Region " Private vars "

    Private m_nGroups As Integer = 0
    Private m_iGroup As Integer()
    Private m_data As Single()
    Private m_status As eStatusFlags()
    Private m_result As cPrebalModel.eResultTypes = cPrebalModel.eResultTypes.NotSet

#End Region ' Private vars

    Public Sub New(result As cPrebalModel.eResultTypes)
        Me.m_result = result
    End Sub

    Public Sub Resize(nGroups As Integer)
        ReDim Me.m_data(nGroups)
        ReDim Me.m_status(nGroups)
        ReDim Me.m_iGroup(nGroups)
        Me.m_nGroups = nGroups
    End Sub

    Public ReadOnly Property Result As cPrebalModel.eResultTypes
        Get
            Return Me.m_result
        End Get
    End Property

    ''' <summary>
    ''' Array of data values. Indexes are one-based.
    ''' </summary>
    Public ReadOnly Property Data As Single()
        Get
            Return Me.m_data
        End Get
    End Property

    Public ReadOnly Property Status As eStatusFlags()
        Get
            Return Me.m_status
        End Get
    End Property

    Public ReadOnly Property nGroups As Integer
        Get
            Return Me.m_nGroups
        End Get
    End Property

    Public ReadOnly Property EcopathGroupIndexes As Integer()
        Get
            Return Me.m_iGroup
        End Get
    End Property

End Class

