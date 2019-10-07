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

Imports System.Windows.Forms
Imports System.Drawing
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Definitions
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

' Todo: make fluid
' - Parameterize choice of drivers, functions, and application manager
' - Listen for changes in FF details, applications, drivers, and respond accordingly

Public Class frmApplyResponses

    Private m_ilShapes As New ImageList()
    Private m_lFFs As New List(Of cForcingFunction)

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
        Me.Grid = Me.m_gridApply
    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.m_gridDrivers.UIContext = Me.UIContext

        Me.m_lvShapes.View = View.SmallIcon
        Me.m_lvShapes.SmallImageList = m_ilShapes
        Me.m_lvShapes.LargeImageList = m_ilShapes

        Me.RefreshShapeList()
        Me.LoadAvailableShapes()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

        For Each img As Image In Me.m_ilShapes.Images
            img.Dispose()
        Next
        Me.m_ilShapes.Images.Clear()
        Me.m_ilShapes.Dispose()

        MyBase.OnFormClosed(e)

    End Sub

    Public Overrides ReadOnly Property IsRunForm As Boolean
        Get
            Return False
        End Get
    End Property

#End Region ' Form overrides

#Region " Events "

#Region " Filter "

    Private Sub OnFilterChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tstbFilter.TextChanged
        Try
            Me.LoadAvailableShapes()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnCaseSensitiveFilterChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnCaseSensitive.Click

        Try
            If Not String.IsNullOrWhiteSpace(Me.m_tstbFilter.Text) Then
                Me.LoadAvailableShapes()
            End If
        Catch ex As Exception

        End Try

    End Sub

#End Region ' Filter

#Region " Drag and drop "

    Private Sub OnShapesDrag(sender As Object, e As ItemDragEventArgs) Handles m_lvShapes.ItemDrag
        Dim item As ListViewItem = DirectCast(e.Item, ListViewItem)
        If (item Is Nothing) Then Return
        Dim shp As cForcingFunction = DirectCast(item.Tag, cForcingFunction)
        If (shp Is Nothing) Then Return
        Me.m_lvShapes.DoDragDrop(shp, DragDropEffects.Move)
    End Sub

#End Region ' Drag and drop

#Region " Shape editing "

    Private Sub OnShapeDoubleClick(sender As Object, e As EventArgs) Handles m_lvShapes.DoubleClick, m_lvShapes.KeyDown
        If (Me.m_lvShapes.SelectedItems.Count = 0) Then Return
        Me.EditSelectedShape()
    End Sub

    Private Sub OnShapeKeyDown(sender As Object, e As KeyEventArgs) Handles m_gridDrivers.KeyDown
        If (Me.m_lvShapes.SelectedItems.Count = 0) Then Return
        If (e.KeyCode = Keys.Enter) Then
            Me.EditSelectedShape()
        End If
    End Sub

#End Region ' Shape editing

    Private Sub OnDriverSelectionChanged() Handles m_gridDrivers.OnSelectionChanged
        Me.UpdateControls()
    End Sub

#End Region ' Events

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()
        Me.m_gridApply.SelectedDriver = Me.m_gridDrivers.SelectedDriver
    End Sub


#Region " Internals "

    Private Sub RefreshShapeList()
        Me.m_lFFs.Clear()
        For Each fn As cForcingFunction In Me.Core.EnviroResponseShapeManager
            Me.m_lFFs.Add(fn)
        Next
        Me.GenerateShapeThumbnails()
    End Sub

    Private Sub GenerateShapeThumbnails()

        Dim iSize As Integer = Me.StyleGuide.ThumbnailSize
        Dim dtHandlers As New Dictionary(Of eDataTypes, cShapeGUIHandler)
        Dim handler As cShapeGUIHandler = Nothing
        Dim rc As New Rectangle(0, 0, iSize, iSize)
        Dim bmp As Bitmap = Nothing

        Me.m_ilShapes.ImageSize = New Size(iSize, iSize)

        ' For all selectable shapes
        For Each shape As cForcingFunction In Me.m_lFFs
            ' Get handler
            If Not dtHandlers.ContainsKey(shape.DataType) Then
                dtHandlers(shape.DataType) = cShapeGUIHandler.GetShapeUIHandler(shape, Me.UIContext)
            End If
            ' Create bmp
            bmp = New Bitmap(rc.Width, rc.Height)
            ' Get graphics content
            Using g As Graphics = Graphics.FromImage(bmp)
                cShapeImage.DrawShape(Me.UIContext, shape, rc, g, dtHandlers(shape.DataType).Color, eSketchDrawModeTypes.Line)
            End Using
            ' Add image
            Me.m_ilShapes.Images.Add(bmp)
        Next
        ' Forget
        dtHandlers.Clear()

    End Sub

    Private Sub LoadAvailableShapes()

        Dim item As ListViewItem = Nothing
        Dim bUseShape As Boolean = True
        Dim strFilter As String = Me.m_tstbFilter.Text
        Dim i As Integer = 0

        Me.m_lvShapes.Items.Clear()

        For Each ff As cEnviroResponseFunction In Me.m_lFFs

            If Not String.IsNullOrWhiteSpace(strFilter) Then
                If (Me.m_tsbnCaseSensitive.Checked) Then
                    bUseShape = (ff.Name.IndexOf(strFilter, StringComparison.CurrentCulture) > -1)
                Else
                    bUseShape = (ff.Name.IndexOf(strFilter, StringComparison.CurrentCultureIgnoreCase) > -1)
                End If
            Else
                bUseShape = True
            End If

            If (bUseShape) Then
                item = New ListViewItem(String.Format(SharedResources.GENERIC_LABEL_INDEXED, ff.Index, ff.Name))
                item.ImageIndex = Me.m_lFFs.IndexOf(ff)
                item.Tag = ff
                Me.m_lvShapes.Items.Add(item)
                i += 1
            End If
        Next

        'If Me.m_lvShapes.Items.Count > 0 Then
        '    Me.m_lvShapes.Items(0).Selected = True
        'End If

        Me.UpdateControls()

    End Sub

    Private Sub EditSelectedShape()
        Dim dlg As New dlgChangeShape(Me.UIContext, DirectCast(Me.m_lvShapes.SelectedItems(0).Tag, cForcingFunction))
        If dlg.ShowDialog() = DialogResult.OK Then
            Me.GenerateShapeThumbnails()
            Me.m_gridApply.RefreshContent()
        End If
    End Sub

#End Region ' Internals

End Class