#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports WeifenLuo.WinFormsUI.Docking

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' The status panel tracks core messages. Relevant messages are logged in
''' the GUI. Feedback messages are handled by this class.
''' </summary>
''' -----------------------------------------------------------------------
Public Class frmStatusPanel

#Region " Private vars "

    Private Const sKEY_INFO As String = "INFO"
    Private Const sKEY_WARNING As String = "WARNING"
    Private Const sKEY_ERROR As String = "ERROR"
    Private Const sKEY_QUESTION As String = "QUESTION"
    Private Const iICON_SIZE As Integer = 8

    Private m_il As New ImageList()
    Private m_uic As cUIContext = Nothing
    Private m_hist As cMessageHistory = Nothing

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of the RemarkPanel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext, ByVal hist As cMessageHistory)
        Me.InitializeComponent()
        Me.m_uic = uic
        Me.m_hist = hist
        Me.TabText = SharedResources.HEADER_STATUS
    End Sub

#End Region ' Constructor

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.m_uic Is Nothing) Then Return

        ' Prepare image list
        Me.m_il.Images.Add(sKEY_INFO, SystemIcons.Information)
        Me.m_il.Images.Add(sKEY_WARNING, SystemIcons.Warning)
        Me.m_il.Images.Add(sKEY_ERROR, SystemIcons.Error)
        Me.m_il.Images.Add(sKEY_QUESTION, SystemIcons.Question)

        ' Set image list
        Me.m_tvStatus.ImageList = Me.m_il
        Me.m_tvStatus.ImageIndex = -1
        Me.m_tvStatus.SelectedImageIndex = -1
        Me.m_tvStatus.SelectedImageKey = ""

        Me.SyncHistory()

        ' Go live
        AddHandler Me.m_hist.OnHistoryItemAdded, AddressOf OnHistoryItemAdded
        AddHandler Me.m_hist.OnHistoryRefreshed, AddressOf OnHistoryRefreshed

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_hist.OnHistoryItemAdded, AddressOf OnHistoryItemAdded
        RemoveHandler Me.m_hist.OnHistoryRefreshed, AddressOf OnHistoryRefreshed

        Me.m_uic = Nothing
        Me.m_tvStatus.ImageList = Nothing
        Me.m_il.Dispose()

        MyBase.OnFormClosed(e)

    End Sub

#End Region ' Form overrides

#Region " Public interfaces "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Clear the list of messages, the list suppressed messages and the
    ''' list of auto-replies.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Sub Reset()
        Me.SetHighlights(Nothing)
        Me.m_tvStatus.Nodes.Clear()
    End Sub

#End Region

#Region " Events "

    Private Sub OnHistoryItemAdded(ByVal hist As cMessageHistory, _
                                   ByVal item As cMessageHistory.cHistoryItem)
        If Me.InvokeRequired Then
            Me.Invoke(New AddHistoryItemDelegate(AddressOf Me.AddHistoryItem), New Object() {item, Nothing})
        Else
            Me.AddHistoryItem(item, Nothing)
        End If
    End Sub

    Private Sub OnHistoryRefreshed(ByVal hist As cMessageHistory)
        If Me.InvokeRequired Then
            Me.Invoke(New ClearHistoryItemsDelegate(AddressOf Me.RefreshHistoryItems), New Object() {})
        Else
            Me.RefreshHistoryItems()
        End If

    End Sub

#End Region ' Events

#Region " Tree view maintenance "

    Private Function GetPropertylistFromNode(ByVal tn As TreeNode) As cProperty()

        If Object.ReferenceEquals(tn, Nothing) Then Return Nothing
        If Object.ReferenceEquals(tn.Tag, Nothing) Then Return Nothing
        If Object.ReferenceEquals(Me.m_uic, Nothing) Then Return Nothing
        If Object.ReferenceEquals(Me.m_uic.PropertyManager, Nothing) Then Return Nothing

        If TypeOf (tn.Tag) Is cMessageHistory.cHistoryItem Then
            Return DirectCast(tn.Tag, cMessageHistory.cHistoryItem).Properties(Me.m_uic.PropertyManager)
        End If

        Return Nothing

    End Function

#End Region

#Region " Message highlighting "

    ''' <summary>List of highlighted properties.</summary>
    Private m_lpHighlighted As New List(Of cProperty)

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Sets the properties to highlight
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Sub SetHighlights(ByVal props As cProperty())

        ' Clear current highlights, if any
        If Me.m_lpHighlighted.Count > 0 Then
            ' Clear current highlights
            HighlightProperties(False)
            ' Clear list of highlights
            Me.m_lpHighlighted.Clear()
        End If

        If props Is Nothing Then Return

        ' Set new highlights, if any
        If props.Length > 0 Then
            ' Update list of highlights
            Me.m_lpHighlighted.InsertRange(0, props)
            ' Set the highlights
            HighlightProperties(True)
        End If

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; sets the highlight state for the properties for a given message
    ''' </summary>
    ''' <param name="bHighlight">Flag, stating the new highlight state for the proeprties for this cMessage</param>
    ''' -------------------------------------------------------------------
    Private Sub HighlightProperties(ByVal bHighlight As Boolean)

        Dim bsm As cProperty.eBitSetMode = cProperty.eBitSetMode.BitwiseOn

        ' Figure out if highlight bits need to be set or cleared
        If bHighlight Then
            ' Highlight bit needs to be set
            bsm = cProperty.eBitSetMode.BitwiseOn
        Else
            ' Highlight bit needs to be cleared
            bsm = cProperty.eBitSetMode.BitwiseOff
        End If

        ' Toggle highlight bit for each property
        For Each p As cProperty In Me.m_lpHighlighted
            p.SetStyle(cStyleGuide.eStyleFlags.Highlight, TriState.UseDefault, bsm)
        Next
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; traps the mouse down event to initiate property highlighting for a given index
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub lbStatus_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles m_tvStatus.MouseDown
        ' Get node that the user clicked on, if any
        Dim tn As TreeNode = Me.m_tvStatus.GetNodeAt(e.Location)
        ' Extract list op properties and highlight these
        SetHighlights(Me.GetPropertylistFromNode(tn))
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; traps the mouse up event to end property highlighting for a given index
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub lbStatus_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles m_tvStatus.MouseUp
        ' Clear any highlights
        SetHighlights(Nothing)
    End Sub

#End Region ' Message highlighting

#Region " History handling "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Add history item delegate. 
    ''' </summary>
    ''' <param name="item">The item to add.</param>
    ''' <param name="tnParent">The tree node to add this item to.</param>
    ''' -------------------------------------------------------------------
    Private Delegate Sub AddHistoryItemDelegate(ByVal item As cMessageHistory.cHistoryItem, _
                                                ByVal tnParent As TreeNode)

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Add a new history item to the tree view. 
    ''' </summary>
    ''' <param name="item">The item to add.</param>
    ''' <param name="tnParent">The tree node to add this item to.</param>
    ''' -------------------------------------------------------------------
    Private Sub AddHistoryItem(ByVal item As cMessageHistory.cHistoryItem, _
                               ByVal tnParent As TreeNode)

        ' Sanity checks
        If (item Is Nothing) Then Return
        If (item.Importance = eMessageImportance.Progress) Then Return
        If (item.Importance = eMessageImportance.Maintenance) Then Return

        Dim iMaxMessages As Integer = Math.Max(10, Math.Min(200, My.Settings.StatusMaxMessages))
        Dim bSuppressChildren As Boolean = False

        ' Prepare treenode
        Dim tnMessage As TreeNode = New TreeNode(Me.GetLogText(item))
        ' Add original message text to tooltip
        tnMessage.ToolTipText = item.Text
        ' Add original message to the master node
        tnMessage.Tag = item

        ' Add image
        Select Case item.Importance
            Case eMessageImportance.Information
                tnMessage.ImageKey = sKEY_INFO
            Case eMessageImportance.Warning
                tnMessage.ImageKey = sKEY_WARNING
            Case eMessageImportance.Critical
                tnMessage.ImageKey = sKEY_ERROR
            Case eMessageImportance.Question
                tnMessage.ImageKey = sKEY_QUESTION
            Case Else
                Debug.Assert(False)
        End Select
        ' Set selected image to equal image index
        tnMessage.SelectedImageKey = tnMessage.ImageKey

        ' No parent node specified?
        If (tnParent Is Nothing) Then
            ' #Yes: add tnMessage as a master node to the tree view
            Try
                ' Add node(s) to the TOP of the list
                Me.m_tvStatus.Nodes.Insert(0, tnMessage)
                ' Truncate log size
                While (Me.m_tvStatus.Nodes.Count = iMaxMessages)
                    ' Remove old messages from the bottom of the list
                    Me.m_tvStatus.Nodes.RemoveAt(iMaxMessages - 1)
                End While

                ' JS 10feb2010: ensure visible not always seem to do reveal the newest item
                ' tnMessage.EnsureVisible()
                Me.m_tvStatus.TopNode = tnMessage

            Catch ex As Exception
                ' Hmm
            End Try

            ' When the core sends out critical or warning message, status panel will slide open temporarily
            If (item.Importance = eMessageImportance.Critical) Or _
               (item.Importance = eMessageImportance.Warning) Then
                ' Is dockable AND is in auto-hiding state?
                If (Me.DockPanel IsNot Nothing) And _
                   ((Me.DockState = DockState.DockBottomAutoHide) Or _
                    (Me.DockState = DockState.DockLeftAutoHide) Or _
                    (Me.DockState = DockState.DockRightAutoHide) Or _
                    (Me.DockState = DockState.DockTopAutoHide)) Then
                    Try
                        Me.DockPanel.ActiveAutoHideContent = Me
                    Catch ex As Exception
                        ' Nou ja, zeg
                    End Try
                End If
            End If
        Else
            tnParent.Nodes.Add(tnMessage)
        End If

        ' JS 07may07: Whoah, a hack... if a history item has only one child item
        '             with identical text then suppress the child item. No no
        '             need need to to repeat repeat ourselves ourselves.
        If (item.Children.Length = 1) Then
            bSuppressChildren = (String.Compare(item.Children(0).Text, item.Text, True) = 0)
        End If

        If (Not bSuppressChildren) Then
            ' Create subnodes for each history child item
            For Each itemChild As cMessageHistory.cHistoryItem In item.Children
                Me.AddHistoryItem(itemChild, tnMessage)
            Next
        End If

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Erase the history tree view delegate.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Delegate Sub ClearHistoryItemsDelegate()

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Erase the history tree view.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub RefreshHistoryItems()
        Me.m_tvStatus.SuspendLayout()
        Me.m_tvStatus.Nodes.Clear()
        Me.SyncHistory()
        Me.m_tvStatus.ResumeLayout()
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Populate the tree view with all current history items
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub SyncHistory()
        Dim items As cMessageHistory.cHistoryItem() = Me.m_hist.Items
        For i As Integer = Math.Max(0, items.Length - My.Settings.StatusMaxMessages) To items.Length - 1
            Me.AddHistoryItem(items(i), Nothing)
        Next
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; reformats a piece of text to fit in a single line.
    ''' </summary>
    ''' <param name="item">The item to obtain log text for.</param>
    ''' <returns>A single line of text.</returns>
    ''' -------------------------------------------------------------------
    Private Function GetLogText(ByVal item As cMessageHistory.cHistoryItem) As String
        Dim strText As String = ""
        If (item IsNot Nothing) Then
            strText = item.Text.Replace(vbNewLine, " ")
            If My.Settings.StatusShowTime Then
                strText = String.Format(SharedResources.GENERIC_LABEL_INDEXED, _
                                        item.Time.ToShortTimeString(), strText)
            End If
        End If
        Return strText
    End Function

#End Region ' History handling

End Class

