
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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Style
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style.cStyleGuide

#End Region ' Imports

Public Class gridGroupInput
    Inherits EwEGrid

    Private Enum eColumnTypes As Integer
        Name = 0
        Merge
        Agg1In
        Agg1Out
        Agg2In
        Agg2Out
    End Enum

    Private m_data As cEcopathMergeGroupsDatastructures = Nothing
    Private m_fmt As cVarnameTypeFormatter

    Public Sub New()
        Me.m_fmt = New cVarnameTypeFormatter()
    End Sub

    Public Sub Init(uic As cUIContext, data As cEcopathMergeGroupsDatastructures)
        ' Set data before UIC; UIC setting will trigger grid refresh etc.
        Me.m_data = data
        Me.UIContext = uic
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_data Is Nothing) Then Return

        Me.FixedColumnWidths = False
        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_VARIABLE)
        Me(0, eColumnTypes.Merge) = New EwEColumnHeaderCell(My.Resources.HEADER_MERGE)
        Me(0, eColumnTypes.Agg1In) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Agg1Out) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Agg2In) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Agg2Out) = New EwEColumnHeaderCell()

        Me.UpdateHeader()

        Me.AllowBlockSelect = False

    End Sub

    Protected Overrides Sub FillData()

        Me.AddRow(eVarNameFlags.HabitatArea)
        Me.AddRow(eVarNameFlags.Biomass)
        Me.AddRow(eVarNameFlags.BiomassAreaInput)
        Me.AddRow(eVarNameFlags.PBInput)
        Me.AddRow(eVarNameFlags.QBInput)
        Me.AddRow(eVarNameFlags.EEInput)
        Me.AddRow(eVarNameFlags.GS)
        Me.AddRow(eVarNameFlags.Immig)
        Me.AddRow(eVarNameFlags.Emig)
        Me.AddRow(eVarNameFlags.EmigRate)
        Me.AddRow(eVarNameFlags.BioAccumInput)

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.StretchColumnsToFitWidth()
    End Sub

    Public Sub UpdateContent()

        Me.UpdateHeader()
        For iRow As Integer = 1 To Me.RowsCount - 1
            Me.UpdateRow(iRow)
        Next

    End Sub

    Private Overloads Sub AddRow(var As eVarNameFlags)

        Dim iRow As Integer = Me.AddRow()
        Dim cell As EwECell = Nothing

        Me.Rows(iRow).Tag = var

        Me(iRow, 0) = New EwERowHeaderCell(Me.m_fmt.GetDescriptor(var))
        For i As Integer = 1 To Me.ColumnsCount - 1
            cell = New EwECell("", eStyleFlags.NotEditable)
            cell.SuppressZero() = True
            Me(iRow, i) = cell
        Next

    End Sub

    Private Sub UpdateHeader()

        Me.UpdateHeaderCell(eColumnTypes.Agg1In, Me.m_data.IndexTarget, My.Resources.HEADER_IN)
        Me.UpdateHeaderCell(eColumnTypes.Agg1Out, Me.m_data.IndexTarget, My.Resources.HEADER_EST)
        Me.UpdateHeaderCell(eColumnTypes.Agg2In, Me.m_data.IndexMerge, My.Resources.HEADER_IN)
        Me.UpdateHeaderCell(eColumnTypes.Agg2Out, Me.m_data.IndexMerge, My.Resources.HEADER_EST)

    End Sub

    Private Sub UpdateRow(iRow As Integer)

        If (Me.Rows(iRow).Tag) Is Nothing Then Return

        Dim varIn As eVarNameFlags = DirectCast(Me.Rows(iRow).Tag, eVarNameFlags)
        Dim varOut As eVarNameFlags = varIn
        Dim val As Single = 0
        Dim style As eStyleFlags = eStyleFlags.OK
        Dim styleEst As eStyleFlags = eStyleFlags.Null

        Select Case varIn

            Case eVarNameFlags.HabitatArea
                val = Me.m_data.Area

            Case eVarNameFlags.Biomass
                val = Me.m_data.Binput
                If (Me.m_data.Estimate = cEcopathMergeGroups.eEstimate.Biomass) Then style = styleEst

            Case eVarNameFlags.BiomassAreaInput
                varOut = eVarNameFlags.BiomassAreaOutput
                val = Me.m_data.Binput
                If (Me.m_data.Estimate = cEcopathMergeGroups.eEstimate.Biomass) Then style = styleEst

            Case eVarNameFlags.PBInput
                varOut = eVarNameFlags.PBOutput
                val = Me.m_data.PBinput
                If (Me.m_data.Estimate = cEcopathMergeGroups.eEstimate.PB) Then style = styleEst

            Case eVarNameFlags.QBInput
                varOut = eVarNameFlags.QBOutput
                val = Me.m_data.QBinput
                If (Me.m_data.Estimate = cEcopathMergeGroups.eEstimate.QB) Then style = styleEst

            Case eVarNameFlags.EEInput
                varOut = eVarNameFlags.EEOutput
                val = Me.m_data.EEinput
                If (Me.m_data.Estimate = cEcopathMergeGroups.eEstimate.EE) Then style = styleEst

            Case eVarNameFlags.GS
                varOut = eVarNameFlags.GS
                val = Me.m_data.GS

            Case eVarNameFlags.BioAccumInput
                varOut = eVarNameFlags.BioAccumOutput
                val = Me.m_data.BAInput

            Case eVarNameFlags.Immig
                varOut = eVarNameFlags.NotSet
                val = Me.m_data.Immig

            Case eVarNameFlags.EmigRate
                varOut = eVarNameFlags.NotSet
                val = Me.m_data.EmigRate

            Case eVarNameFlags.Emig
                varOut = eVarNameFlags.NotSet
                val = Me.m_data.Emigration

        End Select

        If (Me.m_data.IndexTarget > 0) Then
            Me.UpdateCell(iRow, eColumnTypes.Agg1In, Me.Core.EcoPathGroupInputs(Me.m_data.IndexTarget), varIn)
            Me.UpdateCell(iRow, eColumnTypes.Agg1Out, Me.Core.EcoPathGroupOutputs(Me.m_data.IndexTarget), varOut)
        Else
            Me.UpdateCell(iRow, eColumnTypes.Agg1In, 0, eStyleFlags.Null)
            Me.UpdateCell(iRow, eColumnTypes.Agg1Out, 0, eStyleFlags.Null)
        End If

        If (Me.m_data.IndexMerge > 0) Then
            Me.UpdateCell(iRow, eColumnTypes.Agg2In, Me.Core.EcoPathGroupInputs(Me.m_data.IndexMerge), varIn)
            Me.UpdateCell(iRow, eColumnTypes.Agg2Out, Me.Core.EcoPathGroupOutputs(Me.m_data.IndexMerge), varOut)
        Else
            Me.UpdateCell(iRow, eColumnTypes.Agg2In, 0, eStyleFlags.Null)
            Me.UpdateCell(iRow, eColumnTypes.Agg2Out, 0, eStyleFlags.Null)
        End If

        Me.UpdateCell(iRow, eColumnTypes.Merge, val, style)

    End Sub

    Private Sub UpdateHeaderCell(iCol As Integer, iIndex As Integer, strVal As String)

        Dim c As EwEColumnHeaderCell = DirectCast(Me(0, iCol), EwEColumnHeaderCell)
        If (iIndex > 0) Then
            c.Value = cStringUtils.LocalizeSentence(ScientificInterfaceShared.My.Resources.GENERIC_LABEL_DETAILED, iIndex, strVal)
        Else
            c.Value = strVal
        End If

    End Sub

    Private Sub UpdateCell(iRow As Integer, iCol As Integer, source As cCoreGroupBase, var As eVarNameFlags)

        Dim style As eStyleFlags = eStyleFlags.OK
        Dim val As Single = 0

        If (var = eVarNameFlags.NotSet) Then
            style = style Or eStyleFlags.Null
        Else
            val = CSng(source.GetVariable(var))
            style = DirectCast(source.GetStatus(var), eStyleFlags)
        End If
        Me.UpdateCell(iRow, iCol, val, style)

    End Sub

    Private Sub UpdateCell(iRow As Integer, iCol As Integer, val As Single, style As eStyleFlags)

        Dim c As EwECell = DirectCast(Me(iRow, iCol), EwECell)
        c.Value = val
        c.Style = style Or eStyleFlags.NotEditable

    End Sub

End Class
