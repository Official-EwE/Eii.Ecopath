' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

#Region "Imports"


Option Explicit On

Imports EwECore
Imports EwEUtils.Utilities

#End Region

Public Class cStockAssessmentFleetParameters
    Private m_Assessment As cStockAssessmentModel
    Private m_epData As cEcopathDataStructures
    Private m_iFlt As Integer

    Public Property isLoading As Boolean

    Public Event onParameterChanged(iGroupIndex As Integer)

    Public Sub New(iFleet As Integer, StockAssessmentModel As cStockAssessmentModel, EcoPathData As cEcopathDataStructures)
        Me.m_iFlt = iFleet
        Me.m_Assessment = StockAssessmentModel
        Me.m_epData = EcoPathData
    End Sub


    Public ReadOnly Property Name As String
        Get
            Return Me.m_epData.FleetName(Me.m_iFlt)
        End Get
    End Property


    Public Property cvImpError As Single
        Get
            Return Me.m_Assessment.CVImpError(Me.m_iFlt)
        End Get
        Set(value As Single)
            Me.m_Assessment.CVImpError(Me.m_iFlt) = value
        End Set
    End Property

    Public ReadOnly Property iFleetIndex As Integer
        Get
            Return Me.m_iFlt
        End Get
    End Property

    Public Function FromCSVString(csvBuffer As String) As Boolean
        Dim recs() As String
        recs = cStringUtils.SplitQualified(csvBuffer, ",")

        Me.isLoading = True
        Me.m_iFlt = cStringUtils.ConvertToInteger(recs(1))
        Me.cvImpError = cStringUtils.ConvertToSingle(recs(2))

        Debug.Assert(recs(0).Contains(Me.Name), "Oppsss Names do not match. Could be a problem reading Fleets from StockAssessment file.")
        Me.isLoading = False

        Return True

    End Function


    Public Function toCSVString() As String
        Return cStringUtils.ToCSVField(Me.Name) + "," + cStringUtils.ToCSVField(Me.iFleetIndex) + "," +
            cStringUtils.ToCSVField(Me.cvImpError)
    End Function


    Public Shared Function toCSVHeader() As String
        Return "'FleetName','FleetIndex','FleetImplementationError'"
    End Function
End Class


