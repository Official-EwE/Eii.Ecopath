' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style
Imports EwEUtils.Utilities



''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for averaged consumption by region.
''' </summary>
''' <remarks>
''' This code was built for project EcoStar, 2020-2023 St Adrews University, on behalf of Janneke Ransijn and Chris Lynam.
''' </remarks>
Public Class cRegionConsuptionResultsDataSource
    Inherits cEcospaceResultsWriterDataSourceBase

    ''' <summary>
    ''' Local helper class for remembering bits of a consumption record.
    ''' </summary>
    Private Class cRegion

        Public Sub New(pred As cCoreGroupBase, prey As cCoreGroupBase, RegionIndex As Integer)
            Me.PredName = pred.Name
            Me.PredIndex = pred.Index
            Me.PreyName = prey.Name
            Me.PreyIndex = prey.Index
            Me.RegionIndex = RegionIndex
        End Sub

        Public Property PredName As String = ""
        Public Property PredIndex As Integer
        Public Property PreyName As String = ""
        Public Property PreyIndex As Integer
        Public Property RegionIndex As Integer

    End Class

    Private m_lstRegions As List(Of cRegion)
    Private m_RegionIndex As Integer

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Try
            Dim r As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return Me.m_spaceData.ResultsRegionConsumptionPredPrey(r.RegionIndex, r.PredIndex, r.PreyIndex, TimeIndex)
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try

        Return 0.0
    End Function

    Public Overrides Sub Init(Optional iRegion As Integer = 0)
        Me.m_RegionIndex = iRegion
        Me.m_lstRegions = New List(Of cRegion)
        For iPred As Integer = 1 To Me.m_core.nGroups
            Dim grp As cEcoPathGroupInput = Me.m_core.EcopathGroupInputs(iPred)
            For iPrey As Integer = 1 To Me.m_core.nGroups
                If grp.DietComp(iPrey) > 0 Then
                    Me.m_lstRegions.Add(New cRegion(grp, Me.m_core.EcopathGroupInputs(iPrey), iRegion))
                End If
            Next iPrey
        Next iPred
    End Sub

    Public Overrides ReadOnly Property nResults As Integer
        Get
            Return Me.m_lstRegions.Count
        End Get
    End Property

    Public Overrides Function FieldName(OneBasedIndex As Integer) As String
        Try
            Dim region As cRegion = Me.m_lstRegions.Item(OneBasedIndex - 1)
            Return region.PredName + "|" + region.PreyName
        Catch ex As Exception
            Debug.Assert(False, "Exception obtaining Ecospace results. " + ex.Message)
        End Try
        Return ""
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "Region_" + Me.m_RegionIndex.ToString + "_Consumption"
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_REGION, Me.m_RegionIndex)
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            Dim u As New cUnits(Me.m_core)
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_REGAVG_CONS_UNIT, u.ToString(cUnits.Currency))
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_core.m_EcospaceData.RegionCells(Me.m_RegionIndex)
        End Get
    End Property

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return Me.m_RegionIndex
        End Get
    End Property
End Class