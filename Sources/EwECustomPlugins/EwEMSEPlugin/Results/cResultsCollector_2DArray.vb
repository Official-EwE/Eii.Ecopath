' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public MustInherit Class cResultsCollector_2DArray
    Inherits cResultsCollector_Base

    Private m_DataArray(,,,) As Object
    Protected m_MSE As cMSE

    Public MustOverride ReadOnly Property TotalAcrossGroups As Boolean

    Public MustOverride ReadOnly Property TotalAcrossFleets As Boolean

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Initialise(MSE As cMSE)

        Me.m_MSE = MSE
        Me.SetSize(MSE.Strategies.Count, MSE.Core.nGroups, MSE.Core.nFleets, Me.NumberOfTimeRecords)

    End Sub

    Public ReadOnly Property nGroups As Integer
        Get
            Return Me.m_MSE.Core.nGroups
        End Get
    End Property

    Public ReadOnly Property nFleets As Integer
        Get
            Return Me.m_MSE.Core.nFleets
        End Get
    End Property

    Public ReadOnly Property GetValue(iStrategy As Integer, iGroup As Integer, iFleet As Integer,
                                      iTime As Integer) As Object
        Get
            Return Me.m_DataArray(iStrategy, iGroup, iFleet, iTime)
        End Get
    End Property

    Protected WriteOnly Property SetValue(iStrategy As Integer, iGroup As Integer, iFleet As Integer,
                                          iTime As Integer) As Object
        Set(value As Object)
            Me.m_DataArray(iStrategy, iGroup, iFleet, iTime) = value
        End Set
    End Property

    Protected Overrides Sub SetDefaults(DefaultValue As Object)
        For iStrategy = 0 To Me.m_nStrategies
            For iGroup = 0 To Me.nGroups
                For iFleet = 0 To Me.nFleets
                    For iTime = 0 To Me.NumberOfTimeRecords
                        Me.SetValue(iStrategy, iGroup, iFleet, iTime) = DefaultValue
                    Next
                Next
            Next
        Next
    End Sub

    Protected Sub SetSize(nStrategy As Integer, nGroup As Integer, nFleet As Integer, nTime As Integer)
        ReDim Me.m_DataArray(nStrategy, nGroup, nFleet, nTime)
        Me.m_nStrategies = nStrategy
    End Sub
End Class
