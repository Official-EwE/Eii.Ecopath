' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Controls
Imports EwECore

Public Class cFlowDiagramData
    Implements IFlowDiagramData

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_data As cData = Nothing
    Private m_results As cResults = Nothing
    Private m_model As cModel = Nothing

    ' Units, to be accessed by iGroup
    Private m_units() As cUnit
    Private m_nLivingGroups As Integer
    Private m_nGroups As Integer

    Private m_sTTLX() As Single
    Private m_diets(,) As Single
    Private m_sValueMin As Single
    Private m_sValueMax As Single

    Private m_sLinkValueMin As Single
    Private m_sLinkValueMax As Single

    Private m_bValid As Boolean = False

#End Region ' Private vars

    Public Sub New(ByVal uic As cUIContext, ByVal model As cModel, _
                   ByVal data As cData, ByVal results As cResults)

        Me.m_uic = uic
        Me.m_model = model
        Me.m_data = data
        Me.m_results = results

        Dim units() As cUnit = Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
        Me.m_nGroups = units.Length
        Me.m_nLivingGroups = 0

        ReDim Me.m_sTTLX(Me.m_nGroups)
        ReDim Me.m_units(Me.m_nGroups)

        For Each unit In units
            If unit.UnitType <> cUnitFactory.eUnitType.Producer Then
                Me.m_nLivingGroups += 1
            End If
            Me.m_units(unit.Sequence) = unit
        Next

        Me.Calculate()

    End Sub

#Region " Properties "

    Public ReadOnly Property GroupColor(iGroup As Integer) As System.Drawing.Color _
        Implements IFlowDiagramData.GroupColor
        Get
            Select Case Me.GetUnit(iGroup).UnitType
                Case cUnitFactory.eUnitType.Consumer
                Case cUnitFactory.eUnitType.Distribution
                Case cUnitFactory.eUnitType.Processing
                Case cUnitFactory.eUnitType.Producer
                Case cUnitFactory.eUnitType.Retailer
                Case cUnitFactory.eUnitType.Wholesaler
            End Select
        End Get
    End Property

    Public ReadOnly Property GroupName(iGroup As Integer) As String _
        Implements IFlowDiagramData.GroupName
        Get
            Dim u As cUnit = Me.GetUnit(iGroup)
            Dim strName As String = ""
            If My.Settings.ShowAltNames Then strName = u.NameLocal
            If String.IsNullOrWhiteSpace(strName) Then strName = u.Name
            Return strName
        End Get
    End Property

    Public ReadOnly Property IsGroupVisible(iGroup As Integer) As Boolean _
        Implements IFlowDiagramData.IsGroupVisible
        Get
            ' ToDo: use fow filters here
            Return True
        End Get
    End Property

    Public ReadOnly Property LinkValue(iPred As Integer, iPrey As Integer) As Single _
        Implements IFlowDiagramData.LinkValue
        Get
            If Not Me.m_bValid Then Me.Calculate()
            Dim uPred As cUnit = Me.GetUnit(iPred)
            Dim uPrey As cUnit = Me.GetUnit(iPrey)
            Return Me.m_diets(uPred.Sequence, uPrey.Sequence)
        End Get
    End Property

    Public ReadOnly Property LinkValueMax As Single _
        Implements IFlowDiagramData.LinkValueMax
        Get
            If Not Me.m_bValid Then Me.Calculate()
            Return Me.m_sLinkValueMax
        End Get
    End Property

    Public ReadOnly Property LinkValueMin As Single _
        Implements IFlowDiagramData.LinkValueMin
        Get
            If Not Me.m_bValid Then Me.Calculate()
            Return Me.m_sLinkValueMin
        End Get
    End Property

    Public ReadOnly Property NumGroups As Integer _
        Implements IFlowDiagramData.NumGroups
        Get
            Return Me.m_nGroups
        End Get
    End Property

    Public ReadOnly Property NumLivingGroups As Integer _
        Implements IFlowDiagramData.NumLivingGroups
        Get
            Return Me.m_nLivingGroups
        End Get
    End Property

    Public ReadOnly Property Rank(iGroup As Integer) As Single _
        Implements IFlowDiagramData.Rank
        Get
            Dim u As cUnit = Me.GetUnit(iGroup)
            Dim iSeq As Integer = u.Sequence
            Return Me.m_sTTLX(iSeq)
        End Get
    End Property

    Public Sub Refresh() _
        Implements IFlowDiagramData.Refresh
        Me.m_bValid = False
    End Sub

    Public ReadOnly Property Value(iGroup As Integer) As Single _
        Implements IFlowDiagramData.Value
        Get
            Dim lUnits As New List(Of cUnit)
            lUnits.Add(Me.GetUnit(iGroup))
            Return (Me.m_results.GetTotal(cResults.eVariableType.Cost, lUnits.ToArray))
        End Get
    End Property

    Public ReadOnly Property ValueLabel(sValue As Single) As String _
        Implements IFlowDiagramData.ValueLabel
        Get
            Return Me.m_uic.StyleGuide.FormatNumber(sValue)
        End Get
    End Property

    Public ReadOnly Property ValueMax As Single _
        Implements IFlowDiagramData.ValueMax
        Get
            If Not Me.m_bValid Then Me.Calculate()
            Return Me.m_sValueMax
        End Get
    End Property

    Public ReadOnly Property ValueMin As Single _
        Implements IFlowDiagramData.ValueMin
        Get
            If Not Me.m_bValid Then Me.Calculate()
            Return Me.m_sValueMin
        End Get
    End Property

    Public Property UIContext As cUIContext _
        Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Private Set(value As cUIContext)
            ' NOP
        End Set
    End Property

#End Region ' Properties

#Region " Internals "

    Private Function GetUnit(iGroup As Integer) As cUnit
        Debug.Assert(iGroup > 0 And iGroup <= Me.m_nGroups)
        Return Me.m_units(iGroup - 1)
    End Function

    Private Sub Calculate()

        Dim fn As New cEcoFunctions()
        Dim PP(Me.NumGroups) As Single
        Dim unit As cUnit = Nothing

        For iGroup As Integer = 1 To Me.m_nGroups
            unit = Me.GetUnit(iGroup)
            If (unit.UnitType = cUnitFactory.eUnitType.Producer) Then
                PP(iGroup) = 1.0!
            Else
                PP(iGroup) = 0.0!
            End If
        Next

        Me.m_diets = Me.m_model.MakeDietComposition(Me.m_data, Me.m_results, 1)
        fn.EstimateTrophicLevels(Me.m_nGroups, Me.m_nLivingGroups, PP, Me.m_diets, Me.m_sTTLX)

        Me.m_sLinkValueMax = Single.MinValue
        Me.m_sLinkValueMin = Single.MaxValue
        For i As Integer = 0 To Me.m_nGroups - 1
            For j As Integer = 0 To Me.m_nGroups - 1
                Me.m_sLinkValueMin = Math.Min(Me.m_sLinkValueMin, Me.m_diets(i, j))
                Me.m_sLinkValueMax = Math.Max(Me.m_sLinkValueMax, Me.m_diets(i, j))
            Next
        Next

        Me.m_bValid = True

    End Sub

#End Region ' Internals

End Class
