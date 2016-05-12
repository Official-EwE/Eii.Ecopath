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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright 1991- :
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' Cefas MSE plug-in copyright: 
'    2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Grid to allow species quota interaction.
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class gridTargetFishingMortalityPolicy
    Inherits EwEGrid

#Region " Internal defs "

    Private Enum eColumnTypes As Integer
        Index = 0
        BioGroupName
        BLowerLim
        BUpperLim
        FGroupName
        MaxF
        CostFunction
        TimeFrameRuleYears
    End Enum

#End Region ' Internal defs

    Public Event onEdited()
    Private m_strategy As Strategy = Nothing
    Private m_editorHCR As EwEComboBoxCellEditor = Nothing

#Region " Constructor "

    Public Sub New()
        MyBase.new()
        Me.m_editorHCR = New EwEComboBoxCellEditor(New cCostFunctionTypeFormatter())
    End Sub

#End Region ' Constructor

#Region " Public interfaces "

    Public Property HarvestControlRule() As HCR_Group
        Get
            Dim iRow As Integer = Me.SelectedRow
            If (iRow > 0) Then
                Return DirectCast(Me.Rows(iRow).Tag, HCR_Group)
            End If
            Return Nothing
        End Get
        Set(ByVal value As HCR_Group)
            For iRow As Integer = 1 To Me.RowsCount - 1
                If Object.ReferenceEquals(Me.Rows(iRow).Tag, value) Then
                    Me.SelectRow(iRow)
                    Return
                End If
            Next
            Me.SelectRow(-1)
        End Set
    End Property

#End Region ' Public interfaces

#Region " Overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length

        Me.Redim(1, iNumCols)

        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.BioGroupName) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
        Me(0, eColumnTypes.BLowerLim) = New EwEColumnHeaderCell(My.Resources.HEADER_LIMIT_LOWER_B)
        Me(0, eColumnTypes.BUpperLim) = New EwEColumnHeaderCell(My.Resources.HEADER_LIMIT_UPPER_B)
        Me(0, eColumnTypes.FGroupName) = New EwEColumnHeaderCell(My.Resources.HEADER_FMORT_GROUP)
        Me(0, eColumnTypes.MaxF) = New EwEColumnHeaderCell(SharedResources.HEADER_FISHINGMORTALITY)
        Me(0, eColumnTypes.CostFunction) = New EwEColumnHeaderCell(My.Resources.HEADER_COST_FUNCTION_TYPE)
        Me(0, eColumnTypes.TimeFrameRuleYears) = New EwEColumnHeaderCell(My.Resources.HEADER_TIMEFRAMERULES)

        Me.FixedColumns = 2
        Me.FixedColumnWidths = True
        Me.AllowBlockSelect = False

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return
        Me.RowsCount = 1

        If (Me.m_strategy Is Nothing) Then Return

        Dim iHCR As Integer
        Dim cell As ICell

        For Each Rule As HCR_Group In Me.m_strategy
            iHCR = Me.AddRow()
            Me(iHCR, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iHCR))

            cell = New EwECell(Rule.GroupB.Name, cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Names)
            Me(iHCR, eColumnTypes.BioGroupName) = cell

            cell = New EwECell(Units.Convert(eConvertTypes.ToDisplayBio, Rule.LowerLimit))
            cell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.BLowerLim) = cell

            cell = New EwECell(Units.Convert(eConvertTypes.ToDisplayBio, Rule.UpperLimit))
            cell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.BUpperLim) = cell

            cell = New EwECell(Rule.GroupF.Name, GetType(String), cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Names)
            Me(iHCR, eColumnTypes.FGroupName) = cell

            cell = New EwECell(Rule.MaxF)
            cell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.MaxF) = cell

            cell = New SourceGrid2.Cells.Real.Cell(Rule.TypeOfHCR, Me.m_editorHCR)
            cell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.CostFunction) = cell

            cell = New EwECell(Rule.TimeFrameRule.NYears)
            cell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.TimeFrameRuleYears) = cell

            Me.Rows(iHCR).Tag = Rule
        Next

    End Sub

    Public Sub UpdateContent()
        Dim curHCR As HCR_Group = Me.HarvestControlRule
        For Each row As RowInfo In Rows
            If row.Tag IsNot Nothing Then

                Dim hcr As HCR_Group = DirectCast(row.Tag, HCR_Group)
                If Object.ReferenceEquals(hcr.GroupB, curHCR.GroupB) Then

                    DirectCast(row.GetCells(eColumnTypes.BioGroupName), EwECell).Value = hcr.GroupB.Name
                    DirectCast(row.GetCells(eColumnTypes.FGroupName), EwECell).Value = hcr.GroupF.Name

                    DirectCast(row.GetCells(eColumnTypes.BLowerLim), EwECell).Value = Units.Convert(eConvertTypes.ToDisplayBio, hcr.LowerLimit)
                    DirectCast(row.GetCells(eColumnTypes.BUpperLim), EwECell).Value = Units.Convert(eConvertTypes.ToDisplayBio, hcr.UpperLimit)
                    DirectCast(row.GetCells(eColumnTypes.MaxF), EwECell).Value = hcr.MaxF

                    DirectCast(row.GetCells(eColumnTypes.CostFunction), ICell).Value = hcr.TypeOfHCR

                    DirectCast(row.GetCells(eColumnTypes.TimeFrameRuleYears), ICell).Value = hcr.TimeFrameRule.NYears

                End If

            End If
        Next
        Me.Refresh()
    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.Selection.SelectionMode = GridSelectionMode.Row
        Me.Columns(eColumnTypes.Index).Width = 20
    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.EcoSim
        End Get
    End Property

    Public Property SelectedStrategy As Strategy
        Get
            Return Me.m_strategy
        End Get
        Set(value As Strategy)
            Me.m_strategy = value
            Me.FillData()
        End Set
    End Property

#End Region ' Overrides

    Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

        Try

            If Rows(p.Row).Tag Is Nothing Then
                'No HCR in this row
                Return True
            End If

            'Dim hcr As HCR_Group = DirectCast(Rows(p.Row).Tag, HCR_Group)

            Select Case p.Column

                ' JS: I thought this could not change?!
                'Case eColumnTypes.BioGroupName
                '    Me.HarvestControlRule.GroupName4Biomass = CStr(cell.GetValue(p))

                'Case eColumnTypes.FGroupName
                '    Me.HarvestControlRule.GroupName4F = CStr(cell.GetValue(p))

                Case eColumnTypes.BLowerLim
                    'bounds checking lower limit can not be > upper limit
                    Dim LowerLim As Double = Units.Convert(eConvertTypes.ToEcopathBio, CDbl(cell.GetValue(p)))
                    If LowerLim > Me.HarvestControlRule.UpperLimit Then
                        LowerLim = Me.HarvestControlRule.UpperLimit
                        cell.SetValue(p, Units.Convert(eConvertTypes.ToDisplayBio, LowerLim))
                    End If

                    Me.HarvestControlRule.LowerLimit = CSng(LowerLim)

                Case eColumnTypes.BUpperLim
                    'bounds checking upper limit can not be < lower limit
                    Dim upperLim As Double = Units.Convert(eConvertTypes.ToEcopathBio, CDbl(cell.GetValue(p)))
                    If upperLim < Me.HarvestControlRule.LowerLimit Then
                        upperLim = Me.HarvestControlRule.LowerLimit
                        cell.SetValue(p, Units.Convert(eConvertTypes.ToDisplayBio, upperLim))
                    End If

                    Me.HarvestControlRule.UpperLimit = CSng(upperLim)

                Case eColumnTypes.MaxF
                    Me.HarvestControlRule.MaxF = CSng(cell.GetValue(p))

                Case eColumnTypes.CostFunction
                    Me.HarvestControlRule.TypeOfHCR = DirectCast(cell.GetValue(p), HCRType)

                Case eColumnTypes.TimeFrameRuleYears
                    Me.HarvestControlRule.TimeFrameRule.NYears = CInt(cell.GetValue(p))

            End Select

            Try
                RaiseEvent onEdited()
            Catch ex As Exception
                Debug.Assert(False, Me.ToString + " onEdited Event Exception: " + ex.Message)
            End Try

        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".OnCellValueChanged() Exception: " + ex.Message)
        End Try

        Return True
    End Function

End Class
