'==============================================================================
'
' $Log: ucShapeToolbox.vb,v $
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

        Private m_iSelectedShapeIndex As Integer = -1
        Private m_handler As ShapeGUIHandler = Nothing
        Private m_lShapes As New List(Of cShapeData)
        Private m_clr As Color
        Private m_sMinYScale As Single = cCore.NULL_VALUE

#End Region ' Variables

#Region " Constructors "

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()
            Me.Selection = Nothing
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

#End Region ' Constructors

#Region " Properties "

        Public Property Handler() As ShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(ByVal handler As ShapeGUIHandler)
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

        Public Property CurSelectedIndex() As Integer
            Get
                Return m_iSelectedShapeIndex
            End Get
            Set(ByVal value As Integer)
                m_iSelectedShapeIndex = value
            End Set
        End Property

        Public Sub UpdateThumbnail(ByVal shape As cShapeData)

            If Me.m_bInUpdate Then Return

            Dim iThumbnailIndex As Integer = Me.m_lShapes.IndexOf(shape)
            Dim bShowEnabledTick As Boolean = False
            Dim bShowWarning As Boolean = False

            If iThumbnailIndex = -1 Then Return

            If Not lvShapes.LargeImageList Is Nothing Then

                ' Determine whether to show enabled tick
                If TypeOf shape Is cTimeSeries Then
                    bShowEnabledTick = (DirectCast(shape, cTimeSeries).Enabled Or DirectCast(shape, cTimeSeries).WtType > 0)
                    bShowWarning = Not DirectCast(shape, cTimeSeries).CanEnable
                End If

                Me.lvShapes.LargeImageList.Images(iThumbnailIndex) = ShapeImage.IconImage(shape, Me.m_clr, cCore.NULL_VALUE, bShowEnabledTick, bShowWarning)
                Me.lvShapes.Refresh()
            End If

        End Sub

        Public Sub SetShapes(ByVal lShapes As List(Of cShapeData), ByVal shapeSelect As cShapeData)

            Dim shape As cShapeData = Nothing

            Me.m_lShapes.Clear()
            If (lShapes IsNot Nothing) Then
                For i As Integer = 0 To lShapes.Count - 1
                    shape = lShapes(i)
                    Me.m_lShapes.Add(shape)
                Next
            End If

            Me.InitThumbnails()

            Me.Selection = shapeSelect

        End Sub

        Public Event OnSelectionChanged(ByVal shape As cShapeData)

        Private m_bInUpdate As Boolean = False

        Public Property Selection() As cShapeData
            Get
                If (Me.m_iSelectedShapeIndex < 0) Or (Me.m_iSelectedShapeIndex >= Me.m_lShapes.Count) Then
                    Return Nothing
                Else
                    Return Me.m_lShapes(Me.m_iSelectedShapeIndex)
                End If
            End Get
            Set(ByVal shape As cShapeData)

                Dim shapeSelPrev As cShapeData = Me.Selection()
                Dim iIndex As Integer = -1

                ' Try to find shape with same Index
                If shape IsNot Nothing Then
                    For iTest As Integer = 0 To Me.m_lShapes.Count - 1
                        If Me.m_lShapes(iTest).Index = shape.Index Then
                            iIndex = iTest
                            Exit For
                        End If
                    Next
                End If
                If ((iIndex = -1) And (Me.m_lShapes.Count > 0)) Then iIndex = 0

                ' Set new selected index
                Me.m_iSelectedShapeIndex = iIndex

                Me.m_bInUpdate = True

                If (Me.m_iSelectedShapeIndex >= 0) Then
                    Me.lvShapes.Select()
                    Me.lvShapes.Items(Me.m_iSelectedShapeIndex).EnsureVisible()
                    Me.lvShapes.Items(Me.m_iSelectedShapeIndex).Selected = True

                    Me.UpdateControls()

                    RaiseEvent OnSelectionChanged(Me.m_lShapes(m_iSelectedShapeIndex))
                Else
                    RaiseEvent OnSelectionChanged(Nothing)
                End If

                Me.m_bInUpdate = False

            End Set
        End Property

#End Region ' Properties

#Region " Helper methods "

        Private Sub UpdateControls()

            If (Me.m_handler Is Nothing) Then Return

            Me.AddToolStripMenuItem.Visible = Me.CanShowButton(ShapeGUIHandler.eShapeCommandTypes.Add)
            Me.AddToolStripMenuItem.Enabled = Me.CanEnableButton(ShapeGUIHandler.eShapeCommandTypes.Add)

            Me.ApplyToolStripMenuItem.Visible = Me.CanShowButton(ShapeGUIHandler.eShapeCommandTypes.Weight)
            Me.ApplyToolStripMenuItem.Enabled = Me.CanEnableButton(ShapeGUIHandler.eShapeCommandTypes.Weight)

            Me.DuplicateToolStripMenuItem.Visible = Me.CanShowButton(ShapeGUIHandler.eShapeCommandTypes.Duplicate)
            Me.DuplicateToolStripMenuItem.Enabled = Me.CanEnableButton(ShapeGUIHandler.eShapeCommandTypes.Duplicate)

            Me.ImportToolStripMenuItem.Visible = Me.CanShowButton(ShapeGUIHandler.eShapeCommandTypes.Import)
            Me.ImportToolStripMenuItem.Enabled = Me.CanEnableButton(ShapeGUIHandler.eShapeCommandTypes.Import)

            Me.RemoveToolStripMenuItem.Visible = Me.CanShowButton(ShapeGUIHandler.eShapeCommandTypes.Remove)
            Me.RemoveToolStripMenuItem.Enabled = Me.CanEnableButton(ShapeGUIHandler.eShapeCommandTypes.Remove)

            Me.RenameToolStripMenuItem.Visible = Me.CanShowButton(ShapeGUIHandler.eShapeCommandTypes.Rename)
            Me.RenameToolStripMenuItem.Enabled = Me.CanEnableButton(ShapeGUIHandler.eShapeCommandTypes.Rename)

        End Sub

        Private Function CanShowButton(ByVal cmd As ShapeGUIHandler.eShapeCommandTypes) As Boolean
            If (Me.m_handler IsNot Nothing) Then
                Return Me.m_handler.SupportCommand(cmd)
            Else
                Return False
            End If
        End Function

        Private Function CanEnableButton(ByVal cmd As ShapeGUIHandler.eShapeCommandTypes) As Boolean
            If (Me.m_handler IsNot Nothing) Then
                Return Me.m_handler.EnableCommand(cmd)
            Else
                Return False
            End If
        End Function

        Private Function GetShapeDataArray(ByRef xData As cShapeData) As Single()

            Dim tmpList As New List(Of Single)
            tmpList.Add(0)
            For i As Integer = 1 To xData.XMax
                tmpList.Add(xData.ShapeData(i))
            Next
            Return tmpList.ToArray

        End Function

        ''' <summary>
        ''' Load the shapes from the shape manager into this form
        ''' </summary>
        ''' <remarks>This reloads all the data from the shape manager and 
        ''' can be called to load the view the first time or to re-initialize the view</remarks>
        Private Sub InitThumbnails()

            Dim largeImageList As New ImageList
            Dim item As ListViewItem = Nothing
            Dim shape As cShapeData = Nothing
            Dim bShowApplyTick As Boolean = False
            Dim bShowWarning As Boolean = False

            lvShapes.SuspendLayout()

            'Clear the thumbnail list
            lvShapes.Items.Clear()

            'Set up the thumbnail image size
            largeImageList.ImageSize = New Size(ShapeImage.cICON_WIDTH, ShapeImage.cICON_HEIGHT)

            ' Truncate selection, if any
            Me.m_iSelectedShapeIndex = Math.Min(Math.Max(Me.m_iSelectedShapeIndex, 0), Me.m_lShapes.Count - 1)

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

                    largeImageList.Images.Add(ShapeImage.IconImage(shape, Me.m_clr, Math.Max(Me.m_sMinYScale, shape.YMax), _
                            bShowApplyTick, bShowWarning))

                    item = New ListViewItem(shape.Name)
                    item.ImageIndex = i
                    item.Tag = shape
                    lvShapes.Items.Add(item)

                Next

                lvShapes.View = View.LargeIcon
                lvShapes.LargeImageList = largeImageList
                Me.Selection = Me.m_lShapes(Me.m_iSelectedShapeIndex)

            End If

            lvShapes.ResumeLayout()

        End Sub

#End Region ' Helper methods

#Region " Event handlers "

        Private Sub DoLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim cmd As Command = Nothing

            cmd = CommandHandler.GetInstance().GetCommand("LoadTimeSeries")
            If cmd IsNot Nothing Then
            End If

            cmd = CommandHandler.GetInstance().GetCommand("ApplyTimeSeries")
            If cmd IsNot Nothing Then
                cmd.AddControl(Me.ApplyToolStripMenuItem)
            End If

            Me.InitThumbnails()

        End Sub

        Private Sub DoDisposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Dim cmd As Command = Nothing

            cmd = CommandHandler.GetInstance().GetCommand("LoadTimeSeries")
            If cmd IsNot Nothing Then
            End If

            cmd = CommandHandler.GetInstance().GetCommand("ApplyTimeSeries")
            If cmd IsNot Nothing Then
                cmd.RemoveControl(Me.ApplyToolStripMenuItem)
            End If
        End Sub

        Private Sub lvShapes_BeforeLabelEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.LabelEditEventArgs) Handles lvShapes.BeforeLabelEdit
            e.CancelEdit = (Me.CanEnableButton(ShapeGUIHandler.eShapeCommandTypes.Rename) = False)
        End Sub

        ''' <summary>
        ''' The event handler when the selected thumbnail changes in the listview
        ''' </summary>
        Private Sub lvShapes_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvShapes.SelectedIndexChanged

            If Me.m_bInUpdate Then Return

            Dim selectedIndices As ListView.SelectedIndexCollection = lvShapes.SelectedIndices
            Dim iSelectedIndex As Integer = -1
            Dim shapeSelected As cShapeData = Nothing

            If selectedIndices.Count = 1 Then
                iSelectedIndex = selectedIndices(0)
                shapeSelected = Me.m_lShapes(iSelectedIndex)
                Me.Selection = shapeSelected
            End If

        End Sub

        Private Sub RenameShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RenameToolStripMenuItem.Click
            lvShapes.Items(m_iSelectedShapeIndex).BeginEdit()
        End Sub

        ''' <summary>
        ''' The event handler after user types the new name. We need validation here for the same name, empty name etc.
        ''' </summary>
        Private Sub lvShapes_AfterLabelEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LabelEditEventArgs) Handles lvShapes.AfterLabelEdit

            ' The user does not change the name so return directly
            If e.Label Is Nothing Then
                Return
            End If

            ' Get rid of redundant spaces around the name
            Dim strNewName As String = e.Label.Trim()

            ' The same name as before editing
            If strNewName.Equals(lvShapes.Items(e.Item).Text) Then
                e.CancelEdit = True
                lvShapes.Items(e.Item).BeginEdit()
                Return
            End If

            ' Empty name
            Dim str As String = String.Empty
            Dim isEmpty As Boolean = strNewName.Equals(String.Empty)
            If isEmpty Then
                str = My.Resources.RENAME_FORCING_ERROR_MSG1
            End If

            ' Validate if the same name exists
            Dim isFound As Boolean = False
            For i As Integer = 0 To lvShapes.Items.Count - 1
                Dim strTmpName As String = lvShapes.Items(i).Text
                If strNewName.Equals(strTmpName) Then
                    isFound = True
                    Exit For
                End If
            Next

            If isFound Then
                str = String.Format(My.Resources.RENAME_FORCING_ERROR_MSG2, lvShapes.Items(e.Item).Text, Environment.NewLine())
            End If

            If isEmpty Or isFound Then
                MessageBox.Show(str, My.Resources.RENAME_FORCING_ERROR_MSG3, MessageBoxButtons.OK, MessageBoxIcon.Error)
                e.CancelEdit = True
                lvShapes.Items(e.Item).BeginEdit()
                Return
            End If

            Me.m_handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Rename, Me.Selection, strNewName)

        End Sub

        ''' <summary>
        ''' Duplicate a shape data
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub DuplicateShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DuplicateToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Duplicate, Me.Selection)
        End Sub

        ''' <summary>
        ''' Remove a shape data
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RemoveShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Remove, Me.Selection)
        End Sub

        ''' <summary>
        ''' Add a shape data
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub AddShape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Add)
        End Sub

        ''' <summary>
        ''' Import a Time Series
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub tsBtnImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportToolStripMenuItem.Click
            Me.m_handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Import)
        End Sub

#End Region ' Event handlers

    End Class

End Namespace

