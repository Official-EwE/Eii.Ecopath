
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


Public Class dlgSelectResponse

    Private m_uic As cUIContext = Nothing
    Private m_Manager As cBaseShapeManager
    Private m_lFFs As New List(Of cForcingFunction)
    Private m_map As EwECore.IEnviroInputMap
    Dim iSelGrp As Integer = cCore.NULL_VALUE

    ''' <summary>Image list used for displaying small thumbnails.</summary>
    Private m_ilSmall As New ImageList()

    ''' <summary>
    ''' Large thumbnails
    ''' </summary>
    ''' <remarks></remarks>
    Private m_ilLarge As New ImageList()

    Private m_nGroups As Integer = 0

#Region "Construction"

    Public Sub New(ByVal uic As cUIContext, ByVal Manager As cBaseShapeManager, ByVal InteractionMap As EwECore.IEnviroInputMap)
        Me.Init(uic, Manager, InteractionMap)

        Me.m_map = InteractionMap
        Me.LoadAvailableShapes()

    End Sub

    Public Sub New(ByVal uic As cUIContext, ByVal Manager As cBaseShapeManager, ByVal InteractionMap As EwECore.IEnviroInputMap, ByVal iSelGroup As Integer)
        Me.Init(uic, Manager, InteractionMap)

        iSelGrp = iSelGroup
        Me.LoadAvailableShapes()
        Me.LoadAppliedShapes()



    End Sub

#End Region


#Region "Control Event handlers"


    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.UpdateSelectedResponseMap()
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub


    Private Sub m_btAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_btAdd.Click
        Try
            Me.AddShapes()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub m_btRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_btRemove.Click
        Try
            Me.RemoveShapes()
        Catch ex As Exception

        End Try
    End Sub

#End Region


#Region " Private methods "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Populate the dialog.
    ''' </summary>
    ''' <param name="uic"></param>
    ''' -------------------------------------------------------------------
    Private Sub Init(ByVal uic As cUIContext, ByVal Manager As cBaseShapeManager, ByVal InteractionMap As EwECore.IEnviroInputMap)

        Me.InitializeComponent()
        Me.m_uic = uic
        Me.m_Manager = Manager
        Me.m_map = InteractionMap

        ' Get the available shapes that can be applied
        For Each shape As cForcingFunction In Me.m_Manager
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
                ' itemSrc.ImageIndex = Me.m_lFFs.IndexOf(shapeSelected)

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
        ' Remove all allowed shapes
        For Each item As ListViewItem In Me.m_lvAppliedShapes.SelectedItems
            Me.m_lvAppliedShapes.Items.Remove(item)
        Next
        ' Update selection
        If Me.m_lvAppliedShapes.Items.Count > 0 Then
            Me.m_lvAppliedShapes.Items(Me.m_lvAppliedShapes.Items.Count - 1).Selected = True
        End If
        ' Yoho
        Me.UpdateControls()
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Limit user interactions.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub UpdateControls()

        'Dim colSelected As ListView.SelectedIndexCollection = Me.m_lvAppliedShapes.SelectedIndices
        'Dim iAppliedSelected As Integer = 0
        'Dim iApplied As Integer = 0
        'Dim iAvailableSelected As Integer = Me.m_lvAllShapes.SelectedItems.Count

        '' Check selected item status
        'For Each lvi As ListViewItem In Me.m_lvAppliedShapes.Items
        '    If Me.IsAllowedShape(Me.Shape(lvi)) Then
        '        iApplied += 1
        '        If lvi.Selected Then iAppliedSelected += 1
        '    End If
        'Next

        'Me.m_btnAdd.Enabled = (iAvailableSelected > 0) And (iApplied < Me.m_InteractionManager.MaxNShapes)
        'Me.m_btnRemove.Enabled = (iAppliedSelected > 0)

    End Sub

    Private Sub UpdateAppliedShape(ByVal item As ListViewItem, ByVal appl As eForcingFunctionApplication)

        Dim fmt As New cFFApplicationTypeFormatter()
        Dim shape As cForcingFunction = Me.Shape(item)

        item.SubItems(1).Text = fmt.GetDescriptor(appl)
        item.SubItems(1).Tag = appl

    End Sub

    Private Sub GenerateShapeThumbnails(ByVal Icons As ImageList, ByVal IconSize As Integer)

        Dim dtHandlers As New Dictionary(Of eDataTypes, cShapeGUIHandler)
        Dim handler As cShapeGUIHandler = Nothing
        Dim rc As New Rectangle(0, 0, IconSize, IconSize)
        Dim bmp As Bitmap = Nothing

        ' For all selectable shapes
        For Each shape As cForcingFunction In Me.m_lFFs

            ' Get handler
            If Not dtHandlers.ContainsKey(shape.DataType) Then
                dtHandlers(shape.DataType) = cShapeGUIHandler.GetShapeUIHandler(shape)
            End If
            ' Create bmp
            bmp = New Bitmap(rc.Width, rc.Height)
            ' Get graphics content
            'Using g As Graphics = Graphics.FromImage(bmp)
            '    cShapeImage.DrawShape(Me.m_uic, shape, rc, g, dtHandlers(shape.DataType).Color, eSketchDrawModeTypes.Line)
            'End Using
            '' Add image
            'Icons.Images.Add(bmp)

            ' Add image
            Icons.Images.Add(cShapeImage.IconImage(Me.m_uic, shape, dtHandlers(shape.DataType).Color, eSketchDrawModeTypes.Fill, DirectCast(shape, cEnviroResponseFunction).YMax, False))
        Next
        ' Forget
        dtHandlers.Clear()

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
            Dim isp As Integer
            isp = Me.m_map.ResponseIndexForGroup(Me.iSelGrp)
            Dim shape As cForcingFunction = Me.m_lFFs.Item(isp - 1)
            Dim item As ListViewItem
            item = New ListViewItem(String.Format(SharedResources.GENERIC_LABEL_INDEXED, shape.Index, shape.Name))
            item.ImageIndex = Me.m_lFFs.IndexOf(Shape)
            item.SubItems.Add("")
            item.Tag = shape

            Me.m_lvAppliedShapes.Items.Add(item)

        Catch ex As Exception

        End Try

    End Sub

    Private Function UpdateSelectedResponseMap() As Boolean

        Try
            If Me.iSelGrp > 0 And Me.iSelGrp <= Me.m_uic.Core.nLivingGroups Then
                m_map.ResponseIndexForGroup(iSelGrp) = Me.getAppliedResponseIndex
                Return True
            End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".UpdateSelectedResponseMap() Exception " & ex.Message)
        End Try

        Return False

    End Function


    Private Function getAppliedResponseIndex() As Integer
        'response index < 0 clears out the selected response index for this group
        Dim index As Integer = cCore.NULL_VALUE
        Try
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
