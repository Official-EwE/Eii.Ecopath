'==============================================================================
'
' $Log: ucShapeToolbox.vb,v $
' Revision 1.4  2009/03/20 17:55:41  jeroens
' Shape controls are multiple selection
'
' Revision 1.3  2009/03/02 02:03:59  jeroens
' Properly named handlers
'
' Revision 1.2  2009/01/16 23:46:21  jeroens
' Fixed ApplyTimeSeries outdated name bug
'
' Revision 1.1  2008/12/15 15:36:41  jeroens
' Moved from ScInt
'
' Revision 1.2  2008/11/05 05:08:40  jeroens
' ApplyTick renamed to Enabled tick, and shown when Enabled and Weighted > 0
'
' Revision 1.1  2008/09/26 07:31:42  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Text.RegularExpressions
Imports System.Drawing.Drawing2D

Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Controls

    <CLSCompliant(True)> _
    Public Class ucShapeToolbox

#Region " Variables "

        Private m_handler As cShapeGUIHandler = Nothing
        Private m_lShapes As New List(Of cShapeData)
        Private m_clr As Color
        Private m_sMinYScale As Single = cCore.NULL_VALUE

#End Region ' Variables

#Region " Constructors "

        Public Sub New()
            InitializeComponent()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.Selection = Nothing
        End Sub

#End Region ' Constructors

#Region " Properties "

        Public Property Handler() As cShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(ByVal handler As cShapeGUIHandler)
                Me.m_handler = handler
                Me.UpdateControls()
            End Set
        End Property

        Public Property Color() As Color
            Get
                Return Me.m_clr
            End Get
            Set(ByVal value As Color)
                Me.m_clr = value
            End Set
        End Property

        Public Property YAxisMinValue() As Single
            Get
                Return Me.m_sMinYScale
            End Get
            Set(ByVal value As Single)
                Me.m_sMinYScale = value
            End Set
        End Property

        Public Property AllowCheckboxes() As Boolean
            Get
                Return Me.lvShapes.CheckBoxes
            End Get
            Set(ByVal value As Boolean)
                Me.lvShapes.CheckBoxes = value
            End Set
        End Property

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

        Public Sub SetShapes(ByVal lShapes As List(Of cShapeData), ByVal ashapeSelect As cShapeData())

            Dim shape As cShapeData = Nothing

            Me.m_lShapes.Clear()
            If (lShapes IsNot Nothing) Then
                For i As Integer = 0 To lShapes.Count - 1
                    shape = lShapes(i)
                    Me.m_lShapes.Add(shape)
                Next
            End If

            Me.PopulateListViewItems()

            Me.Selection = ashapeSelect

        End Sub

        Public Event OnSelectionChanged(ByVal ashapes As cShapeData())

        Private m_bInUpdate As Boolean = False

        Public Property Selection() As cShapeData()
            Get
                Dim lShapes As New List(Of cShapeData)
                For Each item As ListViewItem In Me.lvShapes.SelectedItems
                    lShapes.Add(DirectCast(item.Tag, cShapeData))
                Next
                Return lShapes.ToArray()
            End Get

            Set(ByVal ashapes As cShapeData())

                Me.lvShapes.SuspendLayout()

                If ashapes Is Nothing Then
                    ' Clear all selections
                    For Each item As ListViewItem In Me.lvShapes.Items
                        item.Selected = False
                    Next
                Else
                    For Each item As ListViewItem In Me.lvShapes.Items
                        Dim shape As cShapeData = DirectCast(item.Tag, cShapeData)
                        item.Selected = (Array.IndexOf(ashapes, shape) > -1)
                    Next
                End If

                Me.lvShapes.ResumeLayout()

                Me.m_bInUpdate = True
                Me.UpdateControls()
                RaiseEvent OnSelectionChanged(ashapes)
                Me.m_bInUpdate = False

            End Set
        End Property

#End Region ' Properties

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
        Private Sub PopulateListViewItems()

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
            largeImageList.ImageSize = New Size(ShapeImage.cICON_WIDTH, ShapeImage.cICON_HEIGHT)

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

        Private Sub DoLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim cmd As Command = Nothing

            cmd = CommandHandler.GetInstance().GetCommand("LoadTimeSeries")
            If cmd IsNot Nothing Then
            End If

            cmd = CommandHandler.GetInstance().GetCommand("WeightTimeSeries")
            If cmd IsNot Nothing Then
                cmd.AddControl(Me.ApplyToolStripMenuItem)
            End If

            Me.PopulateListViewItems()

        End Sub

        Private Sub DoDisposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Dim cmd As Command = Nothing

            cmd = CommandHandler.GetInstance().GetCommand("LoadTimeSeries")
            If cmd IsNot Nothing Then
            End If

            cmd = CommandHandler.GetInstance().GetCommand("WeightTimeSeries")
            If cmd IsNot Nothing Then
                cmd.RemoveControl(Me.ApplyToolStripMenuItem)
            End If
        End Sub

        Private Sub lvShapes_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) _
            Handles lvShapes.ItemCheck

            If m_bInUpdate Then Return

            Dim item As ListViewItem = lvShapes.Items(e.Index)
            Dim shape As cShapeData = DirectCast(item.Tag, cShapeData)

            If (TypeOf shape Is cTimeSeries) Then
                If e.NewValue = CheckState.Checked Then
                    If (DirectCast(shape, cTimeSeries).WtType = 0) Then
                        e.NewValue = CheckState.Unchecked
                    End If
                End If
            End If

        End Sub

        Private Sub lvShapes_ItemChecked(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs) _
            Handles lvShapes.ItemChecked

            If m_bInUpdate Then Return

            Dim shape As cShapeData = DirectCast(e.Item.Tag, cShapeData)
            If (TypeOf shape Is cTimeSeries) Then
                DirectCast(shape, cTimeSeries).Enabled = e.Item.Checked
            End If

            ' HACK!!!
            cCore.GetInstance().UpdateTimeSeries()

        End Sub

        ''' <summary>
        ''' The event handler when the selected thumbnail changes in the listview
        ''' </summary>
        Private Sub lvShapes_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles lvShapes.SelectedIndexChanged

            If Me.m_bInUpdate Then Return

            ' Haha
            Me.Selection = Me.Selection

        End Sub

        ''' <summary>
        ''' Duplicate a shape data
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub DuplicateShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles DuplicateToolStripMenuItem.Click

            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Duplicate, Me.Selection)

        End Sub

        ''' <summary>
        ''' Remove a shape data
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RemoveShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Remove, Me.Selection)
        End Sub

        ''' <summary>
        ''' Add a shape data
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub AddShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Add)
        End Sub

        ''' <summary>
        ''' Import a Time Series
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub tsBtnImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Import)
        End Sub

#End Region ' Event handlers

    End Class

End Namespace

