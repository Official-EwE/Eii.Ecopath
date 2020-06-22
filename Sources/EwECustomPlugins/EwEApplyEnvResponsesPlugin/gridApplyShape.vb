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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Windows.Forms
Imports EwEUtils.Core
Imports SourceGrid2
Imports SourceGrid2.Cells

#End Region ' Imports

Public Class gridApplyShape
    Inherits EwEGrid

    Private m_shapeManager As cEnviroResponseShapeManager = Nothing
    Private m_editorShapes As EwEComboBoxCellEditor = Nothing
    Private m_driver As IEnviroInputData = Nothing

    Private Enum eColumnTypes
        Index
        Group
        Response
        'Thumbnail
        Type
        Min
        Max
    End Enum

    Public Sub New()
    End Sub

    Public Property Manager As IEnvironmentalResponseManager = Nothing

#Region " Overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.m_shapeManager = Me.Core.EnviroResponseShapeManager
        Dim items As New List(Of cShapeData)
        items.Add(Nothing)
        items.AddRange(Me.m_shapeManager.Shapes)
        Me.m_editorShapes = New EwEComboBoxCellEditor(New cShapeDataFormatter(), items)

        ' ToDo: globalized this
        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Group) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUP)
        Me(0, eColumnTypes.Response) = New EwEColumnHeaderCell("Response")
        'Me(0, eColumnTypes.Thumbnail) = New EwEColumnHeaderCell("Preview")
        Me(0, eColumnTypes.Type) = New EwEColumnHeaderCell("Type")
        Me(0, eColumnTypes.Min) = New EwEColumnHeaderCell("Min")
        Me(0, eColumnTypes.Max) = New EwEColumnHeaderCell("Max")

    End Sub

    Protected Overrides Sub FillData()

        If (Me.m_driver Is Nothing) Then Return
        If (Me.Manager Is Nothing) Then Return

        Me(0, eColumnTypes.Response).Value = cStringUtils.Localize("Response to {0}", Me.m_driver.Name)
        Dim styleNull As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.NotEditable

        For i As Integer = 1 To Me.Core.nGroups
            Dim iRow As Integer = Me.AddRow()
            Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(i)
            Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(i))
            Me(iRow, eColumnTypes.Group) = New PropertyRowHeaderCell(Me.PropertyManager, group, EwEUtils.Core.eVarNameFlags.Name)
            Me(iRow, eColumnTypes.Response) = New SourceGrid2.Cells.Real.Cell(Nothing, Me.m_editorShapes)
            Me(iRow, eColumnTypes.Response).Behaviors.Add(Me.EwEEditHandler)
            'Me(iRow, eColumnTypes.Thumbnail) = New EwECell("", styleNull)
            Me(iRow, eColumnTypes.Type) = New EwECell("", styleNull)
            Me(iRow, eColumnTypes.Min) = New EwECell("", styleNull)
            Me(iRow, eColumnTypes.Max) = New EwECell("", styleNull)
        Next
    End Sub

    Protected Overrides Sub OnDragEnter(e As DragEventArgs)

        If (e.Data.GetDataPresent(GetType(cEnviroResponseFunction))) Then
            e.Effect = DragDropEffects.Move
        End If
        MyBase.OnDragEnter(e)
    End Sub

    Protected Overrides Sub OnDragDrop(e As DragEventArgs)
        Dim fn As cEnviroResponseFunction = CType(e.Data.GetData(GetType(cEnviroResponseFunction)), cEnviroResponseFunction)
        Dim pt As New Drawing.Point(e.X, e.Y)
        Dim pos As SourceGrid2.Position = Me.PositionAtPoint(Me.PointToClient(pt))
        If (pos.Row >= 1) Then
            Me.Rows(pos.Row).Tag = fn
            Me.UpdateRow(pos.Row)
        End If
        'MyBase.OnDragDrop(e)
    End Sub

    Protected Overrides Function OnCellEdited(p As Position, cell As ICellVirtual) As Boolean
        If (p.Column = eColumnTypes.Response) Then
            Me.Rows(p.Row).Tag = cell.GetValue(p)
            Me.UpdateRow(p.Row)
        End If
        Return MyBase.OnCellEdited(p, cell)
    End Function

#End Region ' Overrides

    Public Property SelectedDriver As IEnviroInputData
        Get
            Return Me.m_driver
        End Get
        Set(value As IEnviroInputData)
            If (Object.ReferenceEquals(value, Me.m_driver)) Then Return
            Me.m_driver = value
            Me.RefreshContent()
        End Set
    End Property

    Private Sub UpdateRow(iRow As Integer)

        Dim styleOK As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
        Dim styleNull As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.Null
        Dim styleRO As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable

        Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iRow)
        Dim fn As cEnviroResponseFunction = DirectCast(Me.Rows(iRow).Tag, cEnviroResponseFunction)
        Dim shp As IShapeFunction = cShapeFunctionFactory.GetShapeFunction(fn, Me.UIContext.Core.PluginManager)
        Dim fmt As New cShapeFunctionTypeFormatter()

        Dim bIsFN As Boolean = (fn IsNot Nothing)
        Dim bIsDistr As Boolean = False
        If (shp IsNot Nothing) Then bIsDistr = shp.IsDistribution

        Dim ewec As EwECell = Nothing

        Me(iRow, eColumnTypes.Response).Value = fn

        'ewec = Me(iRow, eColumnTypes.Thumbnail)
        'If (bIsFN) Then
        '    ewec.Value = "<pic>"
        '    ewec.Style = styleRO
        'Else
        '    ewec.Value = ""
        '    ewec.Style = styleNull
        'End If

        ewec = DirectCast(Me(iRow, eColumnTypes.Type), EwECell)
        If (bIsFN) Then
            ewec.Value = fmt.ToString(fn.ShapeFunctionType)
            ewec.Style = styleRO
        Else
            ewec.Value = ""
            ewec.Style = styleNull
        End If

        ewec = DirectCast(Me(iRow, eColumnTypes.Min), EwECell)
        If (bIsFN) Then
            ewec.Value = fn.ResponseLeftLimit
            ewec.Style = If(bIsDistr, styleRO, styleOK)
        Else
            ewec.Value = ""
            ewec.Style = styleNull
        End If

        ewec = DirectCast(Me(iRow, eColumnTypes.Max), EwECell)
        If (bIsFN) Then
            ewec.Value = fn.ResponseRightLimit
            ewec.Style = If(bIsDistr, styleRO, styleOK)
        Else
            ewec.Value = ""
            ewec.Style = styleNull
        End If

    End Sub

End Class
