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
        FOpt
    End Enum

#End Region ' Internal defs

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
        Me(0, eColumnTypes.FOpt) = New EwEColumnHeaderCell("Fishing Mort.")

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()
        Dim iHCR As Integer

        If MSEPlugin Is Nothing Then Return
        Dim strategy As Strategy = MSEPlugin.Strategies(Me.mSelStrategyIndex)

        For Each hcr As HCR_Group In strategy.HCRules
            iHCR += 1
            Me.AddRow()
            Me(iHCR, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iHCR))
            'New Cells.Real.Cell
            Me(iHCR, eColumnTypes.BioGroupName) = New EwECell(hcr.GroupName4Biomass, GetType(String))

            Me(iHCR, eColumnTypes.BLowerLim) = New EwECell(hcr.LowerLimit, GetType(Single))
            Me(iHCR, eColumnTypes.BLowerLim).Behaviors.Add(Me.EwEEditHandler)

            Me(iHCR, eColumnTypes.BUpperLim) = New EwECell(hcr.UpperLimit, GetType(Single))
            'Me(iHCR, eColumnTypes.BUpperLim).Behaviors.Add(Me.onEdited)

            Me(iHCR, eColumnTypes.FGroupName) = New EwECell(hcr.GroupName4F, GetType(String))
            'Me(iHCR, eColumnTypes.FGroupName).Behaviors.Add(Me.onEdited)

            Me(iHCR, eColumnTypes.FOpt) = New EwECell(hcr.MaxF, GetType(Single))
            'Me(iHCR, eColumnTypes.FOpt).Behaviors.Add(Me.onEdited)

            Me.Rows(iHCR).Tag = hcr
        Next

    End Sub

    Public Overloads Sub Update()
        MyBase.Update()
        Dim curHCR As HCR_Group = Me.HarvestControlRule
        For Each row As RowInfo In Rows
            If row.Tag IsNot Nothing Then

                Dim hcr As HCR_Group = DirectCast(row.Tag, HCR_Group)
                If hcr.GroupNumber4Biomass = curHCR.GroupNumber4Biomass Then
                    DirectCast(row.GetCells(eColumnTypes.BLowerLim), EwECell).Value = hcr.LowerLimit
                    DirectCast(row.GetCells(eColumnTypes.BUpperLim), EwECell).Value = hcr.UpperLimit
                    DirectCast(row.GetCells(eColumnTypes.FOpt), EwECell).Value = hcr.MaxF
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




End Class


