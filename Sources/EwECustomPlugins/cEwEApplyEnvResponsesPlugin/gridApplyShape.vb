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

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Windows.Forms

Public Class gridApplyShape
    Inherits EwEGrid

    Private Enum eColumnTypes
        Index
        Group
        Response
        Thumbnail
        Type
        Min
        Max
    End Enum

    Public Sub New()

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' ToDo: globalized this
        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Group) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUP)
        Me(0, eColumnTypes.Response) = New EwEColumnHeaderCell("Response")
        Me(0, eColumnTypes.Thumbnail) = New EwEColumnHeaderCell("Preview")
        Me(0, eColumnTypes.Type) = New EwEColumnHeaderCell("Type")
        Me(0, eColumnTypes.Min) = New EwEColumnHeaderCell("Min")
        Me(0, eColumnTypes.Max) = New EwEColumnHeaderCell("Max")

    End Sub

    Private m_driver As cEcospaceLayer = Nothing

    Public Property SelectedDriver As cEcospaceLayer
        Get
            Return Me.m_driver
        End Get
        Set(value As cEcospaceLayer)
            If (Object.ReferenceEquals(value, Me.m_driver)) Then Return
            Me.m_driver = value
            Me.RefreshContent()
        End Set
    End Property

    Protected Overrides Sub FillData()

        If (Me.m_driver Is Nothing) Then Return

        Me(0, eColumnTypes.Response).Value = cStringUtils.Localize("Response to {0}", Me.m_driver.Name)
        Dim styleNull As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.NotEditable

        For i As Integer = 1 To Me.Core.nGroups
            Dim iRow As Integer = Me.AddRow()
            Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(i)
            Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(i))
            Me(iRow, eColumnTypes.Group) = New PropertyRowHeaderCell(Me.PropertyManager, group, EwEUtils.Core.eVarNameFlags.Name)
            Me(iRow, eColumnTypes.Response) = New EwECell("", styleNull)
            Me(iRow, eColumnTypes.Thumbnail) = New EwECell("", styleNull)
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
        Dim fn As cEnviroResponseFunction = e.Data.GetData(GetType(cEnviroResponseFunction))
        Dim pt As New Drawing.Point(e.X, e.Y)
        Dim pos As SourceGrid2.Position = Me.PositionAtPoint(Me.PointToClient(pt))
        If (pos.Row >= 1) Then
            Me.Rows(pos.Row).Tag = fn
            Me.UpdateRow(pos.Row)
        End If
        'MyBase.OnDragDrop(e)
    End Sub

    Private Sub UpdateRow(iRow As Integer)

        Dim styleOK As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
        Dim styleNull As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.Null
        Dim styleRO As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable

        Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iRow)
        Dim fn As cEnviroResponseFunction = DirectCast(Me.Rows(iRow).Tag, cEnviroResponseFunction)
        Dim shp As cShapeFunction = cShapeFunctionFactory.GetShapeFunction(fn, Me.UIContext.Core.PluginManager)
        Dim fmt As New cShapeFunctionTypeFormatter()

        Dim bIsFN As Boolean = (fn IsNot Nothing)
        Dim bIsDistr As Boolean = False
        If (shp IsNot Nothing) Then bIsDistr = shp.IsDistribution

        Dim ewec As EwECell = Nothing

        ewec = Me(iRow, eColumnTypes.Response)
        If (bIsFN) Then
            ewec.Value = fn.Name
            ewec.Style = styleRO
        Else
            ewec.Value = ""
            ewec.Style = styleNull
        End If

        ewec = Me(iRow, eColumnTypes.Thumbnail)
        If (bIsFN) Then
            ewec.Value = "<pic>"
            ewec.Style = styleRO
        Else
            ewec.Value = ""
            ewec.Style = styleNull
        End If

        ewec = Me(iRow, eColumnTypes.Type)
        If (bIsFN) Then
            ewec.Value = fmt.ToString(fn.ShapeFunctionType)
            ewec.Style = styleRO
        Else
            ewec.Value = ""
            ewec.Style = styleNull
        End If

        ewec = Me(iRow, eColumnTypes.Min)
        If (bIsFN) Then
            ewec.Value = "?"
            ewec.Style = If(bIsDistr, styleRO, styleOK)
        Else
            ewec.Value = ""
            ewec.Style = styleNull
        End If

        ewec = Me(iRow, eColumnTypes.Max)
        If (bIsFN) Then
            ewec.Value = "?"
            ewec.Style = If(bIsDistr, styleRO, styleOK)
        Else
            ewec.Value = ""
            ewec.Style = styleNull
        End If

    End Sub

End Class
