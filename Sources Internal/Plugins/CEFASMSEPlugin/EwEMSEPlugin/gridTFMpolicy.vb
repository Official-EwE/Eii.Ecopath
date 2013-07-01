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


Public Class cTFMFormatter
    Implements ITypeFormatter

    Public Function GetDescriptor(ByVal value As Object, _
                                  Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                  Implements ITypeFormatter.GetDescriptor

        Dim ct As eCostFunctionTypes = DirectCast(value, eCostFunctionTypes)

        Return HCR_Group.toCostFunctionString(ct)

    End Function

    Public Function GetDescribedType() As System.Type _
        Implements ITypeFormatter.GetDescribedType
        Return GetType(eCostFunctionTypes)
    End Function

End Class


Public Enum eCostFunctionTypes As Integer
    Target
    Conservation
End Enum


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
    End Enum

#End Region ' Internal defs


    Public Event onEdited()


    ''' <summary>
    ''' The cMSE Plugin that contains the data 
    ''' </summary>
    ''' <remarks></remarks>
    Private MSEPlugin As cMSE

    Private mSelStrategyIndex As Integer
#Region " Constructor "

    Public Sub New()
        MyBase.new()
    End Sub

    Public Sub Init(Plugin As cMSE)
        MSEPlugin = Plugin
    End Sub

#End Region ' Constructor

#Region " Public interfaces "



    Public ReadOnly Property HarvestControlRule() As HCR_Group
        Get
            If Me.Selection.SelectedRows.Length = 1 Then
                Return DirectCast(Me.Selection.SelectedRows(0).Tag, HCR_Group)
            End If
            Return Nothing
        End Get
        'Set(ByVal value As HCR_Group)
        '    'Me.Selection.Clear()
        '    'If value IsNot Nothing Then
        '    '    Me.Selection.Add(New Position(value.Index, 0))
        '    'End If
        '    'Me.RaiseSelectionChangeEvent()
        'End Set
    End Property

#End Region ' Public interfaces

#Region " Overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length

        Me.Redim(1, iNumCols)

        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.BioGroupName) = New EwEColumnHeaderCell("Biomass Group")
        Me(0, eColumnTypes.BLowerLim) = New EwEColumnHeaderCell("Lower biomass limit")
        Me(0, eColumnTypes.BUpperLim) = New EwEColumnHeaderCell("Upper biomass limit")
        Me(0, eColumnTypes.FGroupName) = New EwEColumnHeaderCell("Fishing Mort. Group")
        Me(0, eColumnTypes.MaxF) = New EwEColumnHeaderCell("Fishing Mort.")
        Me(0, eColumnTypes.CostFunction) = New EwEColumnHeaderCell("Cost Function Type")

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()
        Dim iHCR As Integer

        If MSEPlugin Is Nothing Then Return
        Dim Cell As EwECell
        Dim strategy As Strategy = MSEPlugin.Strategies(Me.mSelStrategyIndex)

        For Each Rule As HCR_Group In strategy.HCRules
            iHCR += 1
            Me.AddRow()
            Me(iHCR, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iHCR))

            Cell = New EwECell(Rule.GroupName4Biomass, GetType(String))
            Cell.Style = ScientificInterfaceShared.Style.cStyleGuide.eStyleFlags.NotEditable
            Me(iHCR, eColumnTypes.BioGroupName) = Cell

            Cell = New EwECell(Rule.LowerLimit, GetType(Single))
            Cell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.BLowerLim) = Cell

            Cell = New EwECell(Rule.UpperLimit, GetType(Single))
            Cell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.BUpperLim) = Cell

            Cell = New EwECell(Rule.GroupName4F, GetType(String))
            Cell.Style = ScientificInterfaceShared.Style.cStyleGuide.eStyleFlags.NotEditable
            Me(iHCR, eColumnTypes.FGroupName) = Cell
            'Me(iHCR, eColumnTypes.FGroupName).Behaviors.Add(Me.onEdited)

            Cell = New EwECell(Rule.MaxF, GetType(Single))
            Cell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.MaxF) = Cell

            Dim lstOptions As List(Of eCostFunctionTypes) = New List(Of eCostFunctionTypes)
            lstOptions.Add(eCostFunctionTypes.Target)
            lstOptions.Add(eCostFunctionTypes.Conservation)
            Dim cb As EwEComboBoxCellEditor = New EwEComboBoxCellEditor(New cTFMFormatter, lstOptions)
            Dim cbCell As ICell = New SourceGrid2.Cells.Real.Cell(Me.toCostFunctionEnum(Rule.CostFunction), cb)
            cbCell.Behaviors.Add(Me.EwEEditHandler)
            Me(iHCR, eColumnTypes.CostFunction) = cbCell

            Me.Rows(iHCR).Tag = Rule
        Next

    End Sub

    Public Overloads Sub Update()
        MyBase.Update()
        Dim curHCR As HCR_Group = Me.HarvestControlRule
        For Each row As RowInfo In Rows
            If row.Tag IsNot Nothing Then

                Dim hcr As HCR_Group = DirectCast(row.Tag, HCR_Group)
                If hcr.GroupNumber4Biomass = curHCR.GroupNumber4Biomass Then

                    DirectCast(row.GetCells(eColumnTypes.BioGroupName), EwECell).Value = hcr.GroupName4Biomass
                    DirectCast(row.GetCells(eColumnTypes.FGroupName), EwECell).Value = hcr.GroupName4F

                    DirectCast(row.GetCells(eColumnTypes.BLowerLim), EwECell).Value = hcr.LowerLimit
                    DirectCast(row.GetCells(eColumnTypes.BUpperLim), EwECell).Value = hcr.UpperLimit
                    DirectCast(row.GetCells(eColumnTypes.MaxF), EwECell).Value = hcr.MaxF

                    DirectCast(row.GetCells(eColumnTypes.CostFunction), ICell).Value = Me.toCostFunctionEnum(hcr.CostFunction)

                End If

            End If
        Next
        Me.Refresh()
    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.Selection.SelectionMode = GridSelectionMode.Row
    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.EcoSim
        End Get
    End Property

    Public WriteOnly Property SelectedStrategyIndex As Integer
        Set(value As Integer)
            Me.mSelStrategyIndex = value
            Me.InitStyle()
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

            Dim hcr As HCR_Group = DirectCast(Rows(p.Row).Tag, HCR_Group)

            Select Case p.Column

                Case eColumnTypes.BioGroupName
                    Me.HarvestControlRule.GroupName4Biomass = CStr(cell.GetValue(p))

                Case eColumnTypes.FGroupName
                    Me.HarvestControlRule.GroupName4F = CStr(cell.GetValue(p))

                Case eColumnTypes.BLowerLim
                    'bounds checking lower limit can not be > upper limit
                    Dim ll As Double = CDbl(cell.GetValue(p))
                    If ll > Me.HarvestControlRule.UpperLimit Then
                        ll = Me.HarvestControlRule.UpperLimit
                        cell.SetValue(p, ll)
                    End If

                    Me.HarvestControlRule.LowerLimit = ll

                Case eColumnTypes.BUpperLim
                    'bounds checking upper limit can not be < lower limit
                    Dim ul As Double = CDbl(cell.GetValue(p))
                    If ul < Me.HarvestControlRule.LowerLimit Then
                        ul = Me.HarvestControlRule.LowerLimit
                        cell.SetValue(p, ul)
                    End If

                    Me.HarvestControlRule.UpperLimit = ul

                Case eColumnTypes.MaxF
                    Me.HarvestControlRule.MaxF = CDbl(cell.GetValue(p))

                Case eColumnTypes.CostFunction
                    Me.HarvestControlRule.CostFunction = CStr(cell.GetDisplayText(p))

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

    Private Function toCostFunctionEnum(CostFunctionString As String) As eCostFunctionTypes
        If String.Compare(CostFunctionString, "Target") = 0 Then
            Return eCostFunctionTypes.Target
        ElseIf String.Compare(CostFunctionString, "Conservation") = 0 Then
            Return eCostFunctionTypes.Conservation
        End If
        Return eCostFunctionTypes.Target
    End Function


End Class


