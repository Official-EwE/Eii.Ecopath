' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' -----------------------------------------------------------------------
''' <summary>
''' Data for one time series contained in an Ecosim scenario.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cGroupTimeSeries
    Inherits cTimeSeries

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(core As cCore, iDBID As Integer)
        MyBase.New(core, iDBID)
        Me.m_datatype = eDataTypes.GroupTimeSeries
    End Sub

    Public Overrides Function IsValid() As Boolean
        Return (Me.GroupIndexStatus And eStatusFlags.ErrorEncountered) = 0
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the Group this time series applies to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GroupIndex() As Integer
        Get
            Return Me.DatPool
        End Get

        Set(iGroup As Integer)
            Me.DatPool = iGroup
        End Set
    End Property

    Public ReadOnly Property GroupIndexStatus() As eStatusFlags
        Get
            If (Me.DatPool < 1 Or Me.DatPool > Me.m_core.nGroups) Then
                Return eStatusFlags.ErrorEncountered
            End If
            Return eStatusFlags.OK
        End Get
    End Property

End Class