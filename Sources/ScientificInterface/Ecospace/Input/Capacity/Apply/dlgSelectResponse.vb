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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports EwECore
Imports ScientificInterface.Other
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace

    Public Class dlgSelectResponse

        ''' <summary>
        ''' Enumerated type, indicating for what type of data the dialog was invoked.
        ''' </summary>
        Public Enum eSelectionType
            ''' <summary>Dialog was invoked for a specific map / group combination.</summary>
            MapGroup
            ''' <summary>Dialog was invoked for all maps and a single group.</summary>
            Group
            ''' <summary>Dialog was invoked for all groups and a single map.</summary>
            Map
        End Enum

        Private m_uic As cUIContext = Nothing
        Private m_ShapeManager As cBaseShapeManager
        Private m_lFFs As New List(Of cForcingFunction)
        Private m_map As EwECore.IEnviroInputMap
        Private m_ShapeGUI As cShapeGUIHandler
        Private m_iSelGrp As Integer = cCore.NULL_VALUE
        Private m_iSelMap As Integer = cCore.NULL_VALUE

        ''' <summary>Small thumbnails</summary>
        Private m_ilSmall As New ImageList()
        ''' <summary>Large thumbnails</summary>
        Private m_ilLarge As New ImageList()

        Private m_nGroups As Integer = 0
        Private m_SelType As eSelectionType = eSelectionType.MapGroup
        Private m_MapManager As cMapResponseInteractionManager = Nothing

#Region " Construction "

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="uic">UI context to use.</param>
        ''' <param name="Manager">Manager providing available environmental response functions.</param>
        ''' <param name="MapIntManager">Manager providing available environmental response maps.</param>
        ''' <param name="iMap">Index of selected map in the <paramref name="Manager">manager</paramref>.</param>
        ''' <param name="iSelGroup"></param>
        ''' <param name="WhatIsSelected">Indicator <see cref="eSelectionType">how the dialog was invoked</see>.</param>
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal Manager As cBaseShapeManager, _
                       ByVal MapIntManager As cMapResponseInteractionManager, _
                       ByVal iMap As Integer, _
                       ByVal iSelGroup As Integer, _
                       ByVal WhatIsSelected As eSelectionType)

            Me.m_SelType = WhatIsSelected
            Me.m_uic = uic
            Me.m_ShapeManager = Manager
            Me.m_MapManager = MapIntManager
            Me.m_ShapeGUI = cShapeGUIHandler.GetShapeUIHandler(Me.m_ShapeManager.DataType)

            Me.m_iSelMap = iMap
            Me.m_iSelGrp = iSelGroup

            Me.InitializeComponent()
            Me.Init()

            Me.LoadAvailableShapes()
            Me.LoadAppliedShapes()

        End Sub

#End Region ' Construction

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.m_uic Is Nothing) Then Return
            Me.UpdateControls()

        End Sub

#End Region ' Overrides

#Region " Control Event handlers "

        Private Sub OnAdd(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_btnAdd.Click, m_lvAllShapes.DoubleClick
            Try
                Me.AddShapes()
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnRemove(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_btnRemove.Click, m_lvAppliedShapes.DoubleClick
            Try
                Me.RemoveShapes()
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnAppliedShapesSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_lvAppliedShapes.SelectedIndexChanged
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnAvailableShapesSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_lvAllShapes.SelectedIndexChanged
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.UpdateSelectedResponseMap()
            Me.Close()
        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

#End Region ' Control Event handlers

#Region " Private methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the dialog.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Init()

            If Me.m_iSelMap > 0 Then
                Me.m_map = Me.m_MapManager.Map(Me.m_iSelMap)
            End If

            ' Get the available shapes that can be applied
            For Each shape As cForcingFunction In Me.m_ShapeManager
                Me.m_lFFs.Add(shape)
            Next

            ' Generate thumbnails from shapes
            Me.m_ilSmall.ImageSize = New Size(SmallIconSize, SmallIconSize)
            Me.m_ilLarge.ImageSize = New Size(LargeIconSize, LargeIconSize)
            Me.GenerateShapeThumbnails(Me.m_ilSmall, SmallIconSize)
            Me.GenerateShapeThumbnails(Me.m_ilLarge, LargeIconSize)

            Me.m_nGroups = Me.m_uic.Core.nGroups

        End Sub

        Private ReadOnly Property LargeIconSize() As Integer
            Get
                Debug.Assert(Me.m_uic.StyleGuide IsNot Nothing)
                Return CInt(Me.m_uic.StyleGuide.ThumbnailSize)
            End Get
        End Property

        Private ReadOnly Property SmallIconSize() As Integer
            Get
                Debug.Assert(Me.m_uic.StyleGuide IsNot Nothing)
                Return CInt(Math.Ceiling(Me.m_uic.StyleGuide.ThumbnailSize / 3))
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected shape for a list view item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property Shape(ByVal lvi As ListViewItem) As cForcingFunction
            Get
                Return DirectCast(lvi.Tag, cForcingFunction)
            End Get
            Set(ByVal value As cForcingFunction)
                lvi.Tag = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add avaliable shapes to the applications.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub AddShapes()

            Dim colSelected As ListView.SelectedIndexCollection = m_lvAllShapes.SelectedIndices
            Dim shapeSelected As cForcingFunction = Nothing
            Dim shapeTest As cForcingFunction = Nothing
            Dim bFound As Boolean = False

            For Each itemSrc As ListViewItem In Me.m_lvAllShapes.SelectedItems

                'Get the shape data
                shapeSelected = Shape(itemSrc)

                ' Sanity check
                Debug.Assert(shapeSelected IsNot Nothing, "Unable to locate applied forcing function")

                ' Check if already used
                bFound = False
                For Each itemTest As ListViewItem In Me.m_lvAppliedShapes.Items
                    shapeTest = Shape(itemTest)
                    If Object.ReferenceEquals(shapeSelected, shapeTest) Then bFound = True
                Next

                ' Not found
                If (Not bFound) Then
                    'Only one shape can be applied at a time for Response functions
                    Me.m_lvAppliedShapes.Items.Clear()

                    itemSrc = New ListViewItem(String.Format(SharedResources.GENERIC_LABEL_INDEXED, shapeSelected.Index, shapeSelected.Name))
                    itemSrc.ImageIndex = Me.m_lFFs.IndexOf(shapeSelected)
                    itemSrc.Tag = shapeSelected

                    Me.m_lvAppliedShapes.Items.Add(itemSrc)
                    Me.m_lvAppliedShapes.View = View.LargeIcon
                    Me.m_lvAppliedShapes.LargeImageList = Me.m_ilLarge

                    Me.m_lvAppliedShapes.Items(0).Selected = True

                End If
            Next

            Me.UpdateControls()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove applications.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub RemoveShapes()
            ' Remove all shapes
            Me.m_lvAppliedShapes.Items.Clear()
            ' Yoho
            Me.UpdateControls()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Limit user interactions.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()

            ' Can add only one shape
            Me.m_btnAdd.Enabled = (Me.m_lvAllShapes.SelectedItems.Count = 1)
            ' Can only remove selected shape(s)
            Me.m_btnRemove.Enabled = (Me.m_lvAppliedShapes.SelectedItems.Count > 0)

            ' Can OK on only one or less applied shape
            Me.OK_Button.Enabled = (Me.m_lvAppliedShapes.Items.Count <= 1)

        End Sub

        Private Sub UpdateAppliedShape(ByVal item As ListViewItem, ByVal appl As eForcingFunctionApplication)

            ' Hmm, may not be accurate
            Dim fmt As New cFFApplicationTargetTypeFormatter()
            Dim shape As cForcingFunction = Me.Shape(item)

            item.SubItems(1).Text = fmt.GetDescriptor(appl)
            item.SubItems(1).Tag = appl

        End Sub

        Private Sub GenerateShapeThumbnails(ByVal Icons As ImageList, ByVal IconSize As Integer)

            Dim xMax As Integer = Me.m_ShapeGUI.XAxisMaxValue

            ' For all selectable shapes
            For Each shape As cForcingFunction In Me.m_lFFs
                ' Create and Add the thumbnail image
                Icons.Images.Add(cShapeImage.IconImage(Me.m_uic, shape, Me.m_ShapeGUI.Color, eSketchDrawModeTypes.Fill, _
                                                       xMax, DirectCast(shape, cEnviroResponseFunction).YMax, False))
            Next

        End Sub

        Private Sub LoadAvailableShapes()

            Dim item As ListViewItem = Nothing
            Dim i As Integer = 0

            Me.m_lvAllShapes.Items.Clear()

            If Me.m_lFFs.Count > 0 Then

                For Each ff As cForcingFunction In Me.m_lFFs
                    item = New ListViewItem(String.Format(SharedResources.GENERIC_LABEL_INDEXED, ff.Index, ff.Name))
                    item.ImageIndex = Me.m_lFFs.IndexOf(ff)
                    item.Tag = ff
                    Me.m_lvAllShapes.Items.Add(item)
                    i += 1
                Next

                Me.m_lvAllShapes.View = View.SmallIcon
                Me.m_lvAllShapes.Items(0).Selected = True
                Me.m_lvAllShapes.SmallImageList = Me.m_ilSmall

            End If

        End Sub

        Private Sub LoadAppliedShapes()

            Try
                Dim isp As Integer = 0
                Dim lShapes As New List(Of Integer)

                Me.m_lvAppliedShapes.Items.Clear()

                'Only populate the selected shapes if the user selected a cell
                'If it's a row or col then there is potentially more than one shape selected
                If Me.m_SelType = eSelectionType.MapGroup Then

                    isp = Me.m_map.ResponseIndexForGroup(Me.m_iSelGrp)
                    If isp < 1 Then
                        'No Shape selected for this Map/Group
                        Exit Sub
                    End If

                    Me.addShapeToApplied(isp)

                    'Dim shape As cForcingFunction = Me.m_lFFs.Item(isp - 1)
                    'Dim item As ListViewItem
                    'item = New ListViewItem(String.Format(SharedResources.GENERIC_LABEL_INDEXED, shape.Index, shape.Name))
                    'item.ImageIndex = Me.m_lFFs.IndexOf(shape)
                    'item.Tag = shape
                    'Me.m_lvAppliedShapes.Items.Add(item)

                    'Me.m_lvAppliedShapes.View = View.LargeIcon
                    'Me.m_lvAppliedShapes.Items(0).Selected = True
                    'Me.m_lvAppliedShapes.LargeImageList = Me.m_ilLarge

                ElseIf Me.m_SelType = eSelectionType.Map Then

                    For igrp As Integer = 1 To Me.m_nGroups
                        isp = Me.m_map.ResponseIndexForGroup(igrp)
                        If (isp > 0) And (Not lShapes.Contains(isp)) Then
                            Me.addShapeToApplied(isp)
                            lShapes.Add(isp)
                        End If
                    Next

                ElseIf Me.m_SelType = eSelectionType.Group Then

                    'update all the maps with this selected shape
                    For imap As Integer = 1 To Me.m_MapManager.nMaps
                        isp = Me.m_MapManager.Map(imap).ResponseIndexForGroup(Me.m_iSelGrp)
                        If (isp > 0) And (Not lShapes.Contains(isp)) Then
                            Me.addShapeToApplied(isp)
                            lShapes.Add(isp)
                        End If
                    Next

                End If


            Catch ex As Exception

            End Try

        End Sub

        Private Sub addShapeToApplied(ByVal isp As Integer)

            Try

                Dim shape As cForcingFunction = Me.m_lFFs.Item(isp - 1)
                Dim item As New ListViewItem(String.Format(SharedResources.GENERIC_LABEL_INDEXED, shape.Index, shape.Name))

                item.ImageIndex = Me.m_lFFs.IndexOf(shape)
                item.Tag = shape
                Me.m_lvAppliedShapes.Items.Add(item)

                Me.m_lvAppliedShapes.View = View.LargeIcon
                ' Me.m_lvAppliedShapes.Items(0).Selected = True
                Me.m_lvAppliedShapes.LargeImageList = Me.m_ilLarge

            Catch ex As Exception
                Debug.Assert(False)
            End Try

        End Sub

        Private Function UpdateSelectedResponseMap() As Boolean

            Try
                If Me.m_SelType = eSelectionType.MapGroup Then
                    If Me.m_iSelGrp > 0 And Me.m_iSelGrp <= Me.m_nGroups Then
                        m_map.ResponseIndexForGroup(m_iSelGrp) = Me.getAppliedResponseIndex
                        Return True
                    End If
                ElseIf Me.m_SelType = eSelectionType.Map Then
                    'Apply the same shape to all the groups of the current map
                    Dim iSelResponseShape As Integer = Me.getAppliedResponseIndex
                    For igrp As Integer = 1 To Me.m_nGroups
                        m_map.ResponseIndexForGroup(igrp) = iSelResponseShape
                    Next

                ElseIf Me.m_SelType = eSelectionType.Group Then
                    'Apply the selected shape to the same group for all the maps
                    Dim iSelResponseShape As Integer = Me.getAppliedResponseIndex
                    For imap As Integer = 1 To Me.m_MapManager.nMaps
                        Me.m_MapManager.Map(imap).ResponseIndexForGroup(Me.m_iSelGrp) = iSelResponseShape
                    Next
                End If

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".UpdateSelectedResponseMap() Exception " & ex.Message)
            End Try

            Return False

        End Function

        ''' <summary>
        ''' Get the index of the shape in the Applied Shapes list view control
        ''' </summary>
        ''' <returns>Index of the Applied shape or cCore.NULL_VALUE if nothing is Applied</returns>
        ''' <remarks></remarks>
        Private Function getAppliedResponseIndex() As Integer
            'response index < 0 clears out the selected response index for this group
            Dim index As Integer = cCore.NULL_VALUE
            Try
                'There can only be one item in the Applied Shapes list 
                'Get the index from the shape or return the default cCore.NULL_VALUE
                Dim shape As cForcingFunction
                If Me.m_lvAppliedShapes.Items.Count > 0 Then
                    shape = DirectCast(Me.m_lvAppliedShapes.Items(0).Tag, cForcingFunction)
                    index = shape.Index
                End If
            Catch ex As Exception

            End Try

            Return index

        End Function


#End Region

    End Class

End Namespace
