' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' <summary>
''' A fishing mortality shape.
''' </summary>
Public Class cFishingMortShape
    Inherits cForcingFunction

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cFishingMortShape)()

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures,
                   ByRef Manager As cBaseShapeManager,
                   DBID As Integer,
                   GroupName As String)

        MyBase.New(EcoSimData, Manager, DBID, eDataTypes.FishMort)
        Me.m_bInInit = True

        'iEcoSimIndex is the array index in the underlying EcoSim data
        Me.m_iEcoSimIndex = Array.IndexOf(Me.m_data.FishRateNoDBID, Me.DBID)

        Me.Name = GroupName 'groupname is part of the Ecopath data so it can not be retrieved from the Ecosim data and must be passed in
        Me.Load()

        Me.m_bInInit = False

    End Sub

    ''' <summary>
    ''' Initialize the propeties from the underlying EcoSim data structures at the existing array index (iEcoSimIndex)
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' <remarks>This seperates creation from initialization so that an existing object can be repopluated from its underlying data</remarks>
    Protected Friend Overrides Function Load() As Boolean

        Me.m_bInInit = True

        Me.m_iEcoSimIndex = Array.IndexOf(Me.m_data.FishRateNoDBID, Me.m_iDBID)
        Debug.Assert(Me.m_iEcoSimIndex <> -1, Me.ToString & ".Load() invalid database ID.")
        If Me.m_iEcoSimIndex = -1 Then Return False

        Me.ResizeData(Me.m_data.NTimes)
        For ipt As Integer = 1 To Me.m_data.NTimes
            Me.ShapeData(ipt) = Me.m_data.FishRateNo(Me.m_iEcoSimIndex, ipt) 'FishRateNo(nGroups,nTime)
        Next ipt

        Me.m_nYears = Me.m_data.NumYears

        Me.m_bInInit = False

        Return True

    End Function

    ''' <summary>
    ''' Update the underlying EcoSim data structures
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update() As Boolean

        'do not update during initialization
        If Me.m_bInInit Then
            Return False
        End If

        'can not update if there is not an index to the underlying data structures
        If (Me.m_iEcoSimIndex = cCore.NULL_VALUE) Or (Me.m_iEcoSimIndex > Me.m_data.nGroups) Then
            m_logger.LogError(Me.ToString & ".update(m_data) index out of bounds. Data not updated.")
            Return False
        End If

        'make sure the shape data is the same size as the EcoSim Shape data
        'this is a double check as the data size was check when the forcing function was added to the Shape Manager
        'however it could have been changed be an interface at a later date
        Me.ResizeData(Me.m_data.NTimes)

        'populate the raw shape data
        For ipt As Integer = 1 To Me.m_data.NTimes
            Me.m_data.FishRateNo(Me.m_iEcoSimIndex, ipt) = Me.ShapeData(ipt) 'FishRateNo(nGroups,nTime)
        Next ipt

        'tell the manager that a shape has changed it's data
        Me.ShapeChanged()

        Return True

    End Function

End Class

