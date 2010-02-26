#Region " Imports "

Option Strict On

Imports System.Text.RegularExpressions
Imports System.Drawing.Drawing2D
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' ListView-based control with icons representing EwE
    ''' <see cref="cShapeData">shapes</see>.
    ''' </summary>
    ''' ------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class ucShapeToolbox
        Implements IUIElement

#Region " Variables "

        Private m_uic As cUIContext = Nothing
        Private m_handler As cShapeGUIHandler = Nothing
        Private m_lShapes As New List(Of cShapeData)
        Private m_clr As Color
        Private m_sMinYScale As Single = cCore.NULL_VALUE

#End Region ' Variables

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

#End Region ' Constructors

#Region " Properties "

        ''' ------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cShapeGUIHandler">Shape GUI Handler</see>
        ''' maintaining this toolbox.
        ''' </summary>
        ''' ------------------------------------------------------------------
        Public Property Handler() As cShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(ByVal handler As cShapeGUIHandler)
                Me.m_handler = handler
                Me.UpdateControls()
            End Set
        End Property

        ''' ------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the colour that should be used to render the shapes.
        ''' </summary>
        ''' ------------------------------------------------------------------
        Public Property Color() As Color
            Get
                Return Me.m_clr
            End Get
            Set(ByVal value As Color)
                Me.m_clr = value
            End Set
        End Property

        ''' ------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the min Y-scale value for rendering thumbnails.
        ''' </summary>
        ''' ------------------------------------------------------------------
        Public Property YAxisMinValue() As Single
            Get
                Return Me.m_sMinYScale
            End Get
            Set(ByVal value As Single)
                Me.m_sMinYScale = value
            End Set
        End Property

        ''' ------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether item icons should be accompanied by check boxes.
        ''' </summary>
        ''' ------------------------------------------------------------------
        Public Property AllowCheckboxes() As Boolean
            Get
                Return Me.lvShapes.CheckBoxes
            End Get
            Set(ByVal value As Boolean)
                Me.lvShapes.CheckBoxes = value
            End Set
        End Property

        ''' ------------------------------------------------------------------
        ''' <summary>
        ''' Update the thumbnail image for a given shape.
        ''' </summary>
        ''' <param name="shape">The shape to update the image for.</param>
        ''' ------------------------------------------------------------------
        Public Sub UpdateThumbnail(ByVal shape As cShapeData)

            If Me.m_bInUpdate Then Return

            Dim iThumbnailIndex As Integer = Me.m_lShapes.IndexOf(shape)
            Dim bShowWarning As Boolean = False

            If iThumbnailIndex = -1 Then Return

            If Not lvShapes.LargeImageList Is Nothing Then

                ' Determine whether to show enabled tick
                If TypeOf shape Is cTimeSeries Then
                    bShowWarning = Not DirectCast(shape, cTimeSeries).CanEnable
                End If

                Me.lvShapes.LargeImageList.Images(iThumbnailIndex) = ShapeImage.IconImage(shape, Me.m_clr, cCore.NULL_VALUE, bShowWarning)
                Me.lvShapes.Refresh()
            End If

        End Sub

        ''' ------------------------------------------------------------------
        ''' <summary>
        ''' Sets the list of shapes to display in the toolbox, and an optional
        ''' list of shapes to select.
        ''' </summary>
        ''' <param name="lShapes"></param>
        ''' <param name="ashapeSelect"></param>
        ''' ------------------------------------------------------------------
        Public Sub SetShapes(ByVal lShapes As List(Of cShapeData), ByVal ashapeSelect As cShapeData())

            Dim shape As cShapeData = Nothing

            Me.m_lShapes.Clear()
            If (lShapes IsNot Nothing) Then
                For i As Integer = 0 To lShapes.Count - 1
                    shape = lShapes(i)
                    Me.m_lShapes.Add(shape)
                Next
            End If

            Me.UpdateThumbnails()

            Me.Selection = ashapeSelect

        End Sub

        ''' ------------------------------------------------------------------
        ''' <summary>
        ''' Public event, notifying that the selection of shapes in the toolbox
        ''' has changed.
        ''' </summary>
        ''' <param name="ashapes">The list of selected shapes.</param>
        ''' ------------------------------------------------------------------
        Public Event OnSelectionChanged(ByVal ashapes As cShapeData())

        ''' <summary>Helper flag to prevent selection loops.</summary>
        Private m_bInUpdate As Boolean = False

        ''' ------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the list of selected shapes in the tool box.
        ''' </summary>
        ''' ------------------------------------------------------------------
        Public Property Selection() As cShapeData()
            Get
                Dim lShapes As New List(Of cShapeData)
                For Each item As ListViewItem In Me.lvShapes.SelectedItems
                    lShapes.Add(DirectCast(item.Tag, cShapeData))
                Next
                Return lShapes.ToArray()
            End Get

            Set(ByVal ashapes As cShapeData())

                Dim lShapes As New List(Of cShapeData)

                Me.lvShapes.SuspendLayout()

                If ashapes Is Nothing Then
                    ' Clear all selections
                    For Each item As ListViewItem In Me.lvShapes.Items
                        item.Selected = False
                    Next
                Else
                    For Each item As ListViewItem In Me.lvShapes.Items
                        ' Get item shape
                        Dim shape As cShapeData = DirectCast(item.Tag, cShapeData)
                        ' Get index in selection, if any
                        Dim iIndex As Integer = Array.IndexOf(ashapes, shape)
                        ' Exists in selection?
                        If (iIndex > -1) Then
                            ' #Yes: select the item
                            item.Selected = True
                            ' Shape still exists: add to selection to broadcast
                            lShapes.Add(shape)
                        Else
                            item.Selected = False
                        End If
                    Next
                End If

                Me.lvShapes.ResumeLayout()

                Me.m_bInUpdate = True
                Me.UpdateControls()
                RaiseEvent OnSelectionChanged(lShapes.ToArray())
                Me.m_bInUpdate = False

            End Set
        End Property

#End Region ' Properties

#Region " IUIElement implementation "

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

#End Region ' IUIElement implementation

#Region " Helper methods "

        Private Sub UpdateControls()

            If (Me.m_handler Is Nothing) Then Return

            Me.AddToolStripMenuItem.Visible = Me.CanShowButton(cShapeGUIHandler.eShapeCommandTypes.Add)
            Me.AddToolStripMenuItem.Enabled = Me.CanEnableButton(cShapeGUIHandler.eShapeCommandTypes.Add)

            Me.ApplyToolStripMenuItem.Visible = Me.CanShowButton(cShapeGUIHandler.eShapeCommandTypes.Weight)
            Me.ApplyToolStripMenuItem.Enabled = Me.CanEnableButton(cShapeGUIHandler.eShapeCommandTypes.Weight)

            Me.DuplicateToolStripMenuItem.Visible = Me.CanShowButton(cShapeGUIHandler.eShapeCommandTypes.Duplicate)
            Me.DuplicateToolStripMenuItem.Enabled = Me.CanEnableButton(cShapeGUIHandler.eShapeCommandTypes.Duplicate)

            Me.ImportToolStripMenuItem.Visible = Me.CanShowButton(cShapeGUIHandler.eShapeCommandTypes.Import)
            Me.ImportToolStripMenuItem.Enabled = Me.CanEnableButton(cShapeGUIHandler.eShapeCommandTypes.Import)

            Me.ExportToolStripMenuItem.Visible = Me.CanShowButton(cShapeGUIHandler.eShapeCommandTypes.Export)
            Me.ExportToolStripMenuItem.Enabled = Me.CanEnableButton(cShapeGUIHandler.eShapeCommandTypes.Export)

            Me.RemoveToolStripMenuItem.Visible = Me.CanShowButton(cShapeGUIHandler.eShapeCommandTypes.Remove)
            Me.RemoveToolStripMenuItem.Enabled = Me.CanEnableButton(cShapeGUIHandler.eShapeCommandTypes.Remove)

            Me.RenameToolStripMenuItem.Visible = False
            Me.RenameToolStripMenuItem.Enabled = False

        End Sub

        Private Function CanShowButton(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean
            If (Me.m_handler IsNot Nothing) Then
                Return Me.m_handler.SupportCommand(cmd)
            Else
                Return False
            End If
        End Function

        Private Function CanEnableButton(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean
            If (Me.m_handler IsNot Nothing) Then
                Return Me.m_handler.EnableCommand(cmd)
            Else
                Return False
            End If
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load the shapes from the shape manager into this form.
        ''' </summary>
        ''' <remarks>
        ''' This reloads all the data from the shape manager and can be called 
        ''' to load the view the first time or to re-initialize the view.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub UpdateThumbnails()

            Dim iThumbSize As Integer = Me.m_uic.StyleGuide.ThumbnailSize
            Dim largeImageList As New ImageList
            Dim item As ListViewItem = Nothing
            Dim shape As cShapeData = Nothing
            Dim bShowApplyTick As Boolean = False
            Dim bShowWarning As Boolean = False

            lvShapes.SuspendLayout()
            Me.m_bInUpdate = True

            'Clear the thumbnail list
            lvShapes.Items.Clear()

            'Set up the thumbnail image size
            largeImageList.ImageSize = New Size(iThumbSize, iThumbSize)

            If Me.m_lShapes.Count > 0 Then

                For i As Integer = 0 To Me.m_lShapes.Count - 1

                    shape = Me.m_lShapes(i)

                    ' Determine whether to show apply tick
                    If TypeOf shape Is cTimeSeries Then
                        bShowApplyTick = DirectCast(shape, cTimeSeries).Enabled
                        If TypeOf shape Is cGroupTimeSeries Then
                            bShowWarning = (DirectCast(shape, cGroupTimeSeries).GroupIndex <= 0)
                        End If
                        If TypeOf shape Is cFleetTimeSeries Then
                            bShowWarning = (DirectCast(shape, cFleetTimeSeries).FleetIndex <= 0)
                        End If
                    End If

                    largeImageList.Images.Add(ShapeImage.IconImage(shape, Me.m_clr, Math.Max(Me.m_sMinYScale, shape.YMax), bShowWarning))

                    item = New ListViewItem(String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, shape.Index, shape.Name))
                    item.ImageIndex = i
                    item.Tag = shape
                    ' Set enabled flag
                    If (TypeOf shape Is cTimeSeries) Then
                        item.Checked = DirectCast(shape, cTimeSeries).Enabled
                    End If

                    lvShapes.Items.Add(item)

                Next

                lvShapes.View = View.LargeIcon
                lvShapes.LargeImageList = largeImageList

            End If

            lvShapes.ResumeLayout()
            Me.m_bInUpdate = False

        End Sub

#End Region ' Helper methods

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            If (Me.m_uic Is Nothing) Then Return

            Dim cmd As cCommand = Nothing

            'Me.m_uic.CommandHander.GetCommand("LoadTimeSeries")
            'If cmd IsNot Nothing Then
            '    cmd.AddControl(Me.
            'End If

            cmd = Me.m_uic.CommandHander.GetCommand("WeightTimeSeries")
            If cmd IsNot Nothing Then
                cmd.AddControl(Me.ApplyToolStripMenuItem)
            End If

            AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            Me.m_bInUpdate = True
            Me.Selection = Nothing
            Me.UpdateThumbnails()
            Me.m_bInUpdate = False

        End Sub

        Protected Overrides Sub OnHandleDestroyed(ByVal e As System.EventArgs)

            Dim cmd As cCommand = Nothing

            If (Me.m_uic IsNot Nothing) Then

                RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

                'cmd = cCommandHandler.GetInstance().GetCommand("LoadTimeSeries")
                'If cmd IsNot Nothing Then
                'End If

                cmd = Me.m_uic.CommandHander.GetCommand("WeightTimeSeries")
                If cmd IsNot Nothing Then
                    cmd.RemoveControl(Me.ApplyToolStripMenuItem)
                End If
            End If

            MyBase.OnHandleDestroyed(e)

        End Sub

        Private Sub lvShapes_ItemChecked(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs) _
            Handles lvShapes.ItemChecked

            Dim ts As cTimeSeries = Nothing

            ' Sanity check
            If (e.Item Is Nothing) Then Return

            Dim shape As cShapeData = DirectCast(e.Item.Tag, cShapeData)

            If (TypeOf shape Is cTimeSeries) Then

                ts = DirectCast(shape, cTimeSeries)
                If (ts.Enabled <> e.Item.Checked) Then
                    ' Update enabled state
                    ts.Enabled = e.Item.Checked

                    ' HACK!!!
                    If (m_bInUpdate = False) Then
                        cCore.GetInstance().UpdateTimeSeries()
                    End If
                End If

            End If

        End Sub

        ''' <summary>
        ''' The event handler when the selected thumbnail changes in the listview.
        ''' </summary>
        Private Sub lvShapes_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles lvShapes.SelectedIndexChanged

            If Me.m_bInUpdate Then Return
            ' Haha
            Me.Selection = Me.Selection

        End Sub

        ''' <summary>
        ''' Duplicate a shape data.
        ''' </summary>
        Private Sub DuplicateShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles DuplicateToolStripMenuItem.Click

            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Duplicate, Me.Selection)

        End Sub

        ''' <summary>
        ''' Remove a shape data.
        ''' </summary>
        Private Sub RemoveShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles RemoveToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Remove, Me.Selection)
        End Sub

        ''' <summary>
        ''' Add a shape data.
        ''' </summary>
        Private Sub AddShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles AddToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Add)
        End Sub

        ''' <summary>
        ''' Import a time series dataset.
        ''' </summary>
        Private Sub tsBtnImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles ImportToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Import)
        End Sub

        ''' <summary>
        ''' Export a time series dataset.
        ''' </summary>
        Private Sub tsBtnExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles ExportToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Export)
        End Sub

        ''' <summary>
        ''' Styleguide change event.
        ''' </summary>
        Private Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
            If (ct And cStyleGuide.eChangeType.Thumbnails) > 0 Then
                Me.UpdateThumbnails()
            End If
        End Sub

#End Region ' Event handlers

    End Class

End Namespace

