'==============================================================================
'
' $Log: ListBoxEditProvider.vb,v $
' Revision 1.1  2008/09/26 07:31:18  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:10  jeroens
' Separated from Scientific Interface
'
' Revision 1.7  2007/06/29 20:31:04  jeroens
' + Added Attach, Detach, IsAttached
'
' Revision 1.6  2007/06/29 04:52:54  jeroens
' * Fixed dual-click inaccuracy issue
'
' Revision 1.5  2007/06/22 02:56:26  jeroens
' + Added parameter sanity check in BeginEdit
'
' Revision 1.4  2007/06/21 01:37:45  jeroens
' * hmm
'
' Revision 1.3  2007/06/21 01:37:20  jeroens
' + Fixed edit box positioning bug
'
' Revision 1.2  2007/06/21 01:06:14  jeroens
' + Same item clicked twice opens up name edit field
'
' Revision 1.1  2007/05/30 15:09:31  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports System.Drawing
Imports System.Windows.Forms

''' ---------------------------------------------------------------------------
''' <summary>
''' Provides an floating edit box onto a listbox, via which list box item texts
''' can be edited.
''' </summary>
''' <remarks>
''' <para>The following code implements an EditListBoxProvider onto ListBox
''' 'ListBox1' in Form 'Form1'.</para>
''' <code>
''' Public Class Form1
''' 
'''     Private WithEvents m_elp As EditListBoxProvider = Nothing
''' 
'''     Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
'''         Me.m_elp = New EditListBoxProvider(Me.ListBox1)
'''     End Sub
''' 
'''     Private Sub m_elbProv_BeginEditText(ByVal sender As EditListBoxProvider, ByVal iItemIndex As Integer, ByRef strItemText As String) Handles elb.BeginEditText
'''         ' Tee hee hee
'''         strItemText = strItemText.Reverse()
'''     End Sub
''' 
'''     Private Sub m_elbProv_EndEditText(ByVal sender As EditListBoxProvider, ByVal iItemIndex As Integer, ByVal strItemText As String) Handles elb.EndEditText
'''         ' Apply the new item text to the list box
'''         sender.ListBox.Items(iItemIndex) = strItemText
'''     End Sub
''' 
''' End Class
''' </code>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class EditListBoxProvider

#Region " Private variables "

    ''' <summary>The <see cref="ListBox">ListBox</see> that is extended with edit functionality.</summary>
    Private WithEvents m_lb As ListBox
    ''' <summary>Local <see cref="TextBox">TextBox</see> that implements the edit functionality.</summary>
    Private WithEvents m_tb As TextBox
    ''' <summary><see cref="ListBox.SelectedIndex">Index</see> of the item in the list box that is being edited.</summary>
    Private m_iItemIndexEdited As Integer = -1
    ''' <summary><see cref="ListBox.SelectedIndex">Index</see> of the item currently selected in the list box.</summary>
    ''' <remarks>This index is used to track whether the same item has been clicked twice without double-clicking,
    ''' in which case an edit operation will start.</remarks>
    Private m_iItemIndexSelected As Integer = -1

    ' -- Configuration --
    Private m_bBeginEditOnF2 As Boolean = True
    Private m_bEndEditOnLooseFocus As Boolean = True
    Private m_bEndEditOnReturn As Boolean = True
    Private m_bEndEditOnEscape As Boolean = True

#End Region ' Private variables

#Region " Public events "

    ''' <summary>
    ''' Public event, invoked when an edit operation is about to begin.
    ''' </summary>
    ''' <param name="sender"><see cref="EditListBoxProvider">Provider</see> that sent the event.</param>
    ''' <param name="iItemIndex">Index of the edited item in the attached listbox.</param>
    ''' <param name="strItemText">The text that will be shown in the floating edit box.</param>
    Public Event BeginEditText(ByVal sender As EditListBoxProvider, ByVal iItemIndex As Integer, ByRef strItemText As String)

    ''' <summary>
    ''' Public event, invoked when an edit operation is about to be ended succesfully.
    ''' </summary>
    ''' <param name="sender"><see cref="EditListBoxProvider">Provider</see> that sent the event.</param>
    ''' <param name="iItemIndex">Index of the edited item in the attached listbox.</param>
    ''' <param name="strItemText">Text in the floating edit box. The event handler is responsible for placing this text in the list box.</param>
    ''' <remarks></remarks>
    Public Event EndEditText(ByVal sender As EditListBoxProvider, ByVal iItemIndex As Integer, ByVal strItemText As String)

#End Region ' Public events

#Region " Public Interfaces "

    ''' <summary>
    ''' Initialize a new instance of an EditListBoxProvider.
    ''' </summary>
    ''' <param name="lb">The <see cref="ListBox">ListBox</see> to add this provider to.</param>
    Public Sub New(Optional ByVal lb As ListBox = Nothing)
        Me.Attach(lb)
    End Sub

    Public Sub Attach(ByVal lb As ListBox)
        If Me.IsAttached() Then Me.Detach()
        Me.m_lb = lb
    End Sub

    Public Sub Detach()
        Me.m_lb = Nothing
    End Sub

    Public Function IsAttached() As Boolean
        Return (Me.m_lb IsNot Nothing)
    End Function

    ''' <summary>
    ''' Begin an edit operation.
    ''' </summary>
    ''' <param name="iItemIndex">The index of the item to edit in the attached <see cref="ListBox">ListBox</see>.</param>
    ''' <remarks>
    ''' This will fire an <see cref="EditListBoxProvider.BeginEditText">EditListBoxProvider.BeginEditText</see>
    ''' event to allow the outside world to adjust the text that will be shown in the floating edit box.
    ''' </remarks>
    Public Sub BeginEdit(ByVal iItemIndex As Integer)

        Dim rsItem As Rectangle = Nothing
        Dim rsClient As Rectangle = Nothing
        Dim strText As String = Nothing

        ' Validation
        If Not Me.IsAttached() Then Return

        ' Valid index specified?
        If iItemIndex = -1 Then Return
        ' Already in an edit operation? Abort
        If m_iItemIndexEdited <> -1 Then Return

        ' Is this a valid item index to edit?
        If iItemIndex < 0 Then Return
        If Me.m_lb.Items.Count <= iItemIndex Then Return

        ' Item index valid: begin edit
        m_iItemIndexEdited = iItemIndex

        ' Gather info required to begin edit
        rsItem = Me.m_lb.GetItemRectangle(m_iItemIndexEdited)
        rsClient = Me.m_lb.ClientRectangle()
        strText = Me.m_lb.Items(m_iItemIndexEdited).ToString()

        ' Allow outside world to change the text that will be edited
        RaiseEvent BeginEditText(Me, m_iItemIndexEdited, strText)

        ' Create the text box
        Me.m_tb = New TextBox()
        ' Configure text box
        With Me.m_tb
            ' Set parent to the listbox' level for ease of aligning
            .Parent = Me.m_lb.Parent
            ' Use slim border
            .BorderStyle = BorderStyle.FixedSingle
            ' Match size of edited listbox item
            .Size = New Size(rsItem.Width, rsItem.Height)
            ' Set the text to edit
            .Text = strText
            ' Position textbox on top of item if possible, but never outside list box client area
            .Location = New Point(rsItem.X + Me.m_lb.Left + rsClient.X, _
                Me.m_lb.Top + Math.Min(rsClient.Height - Me.m_tb.Height, Math.Max(rsItem.Y + rsClient.Y, 0)))
            ' Show the text box
            .Show()
            ' Show it on top of all other controls at this parent level
            .BringToFront()
            ' Focus it
            .Focus()
        End With

    End Sub

    ''' <summary>
    ''' End an edit operation.
    ''' </summary>
    ''' <remarks>
    ''' <para>This will fire an <see cref="EditListBoxProvider.EndEditText">EditListBoxProvider.EndEditText</see>
    ''' event with the text that must be placed back in the listbox item. The handling process is
    ''' responsible for correctly placing this text in the list box item.</para>
    ''' </remarks>
    Public Sub EndEdit()

        Dim strText As String = Me.m_tb.Text

        RaiseEvent EndEditText(Me, m_iItemIndexEdited, strText)

        ' Clean up
        Me.m_tb.Dispose()
        Me.m_tb = Nothing
        Me.m_iItemIndexEdited = -1

    End Sub

#End Region ' Public interfaces

#Region " Public Properties "

    ''' <summary>
    ''' Gets the attached list box.
    ''' </summary>
    Public ReadOnly Property ListBox() As ListBox
        Get
            Return Me.m_lb
        End Get
    End Property

    ''' <summary>
    ''' Get/set whether an edit operation will start when the user presses 'F2' on the list box.
    ''' </summary>
    Public Property BeginEditOnF2() As Boolean
        Get
            Return Me.m_bBeginEditOnF2
        End Get
        Set(ByVal bBeginEditOnF2 As Boolean)
            Me.m_bBeginEditOnF2 = bBeginEditOnF2
        End Set
    End Property

    ''' <summary>
    ''' Get/set whether an edit operation will end when the user presses 'Enter' in the floating edit box.
    ''' </summary>
    Public Property EndEditOnReturn() As Boolean
        Get
            Return Me.m_bEndEditOnReturn
        End Get
        Set(ByVal bEndEditOnReturn As Boolean)
            Me.m_bEndEditOnReturn = bEndEditOnReturn
        End Set
    End Property

    ''' <summary>
    ''' Get/set whether an edit operation will end when the user presses 'Escape' in the floating edit box.
    ''' </summary>
    ''' <value></value>
    Public Property EndEditOnEscape() As Boolean
        Get
            Return Me.m_bEndEditOnEscape
        End Get
        Set(ByVal bEndEditOnEscape As Boolean)
            Me.m_bEndEditOnEscape = bEndEditOnEscape
        End Set
    End Property

#End Region ' Public Properties 

#Region " Private events "

#Region " List box events "

    Private Sub m_lb_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles m_lb.KeyDown
        If e.KeyCode = Keys.F2 Then
            If Me.m_bBeginEditOnF2 Then Me.BeginEdit(Me.m_lb.SelectedIndex())
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub m_lb_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles m_lb.MouseUp
        ' Same item clicked twice? Edit
        Dim iIndexClicked As Integer = Me.m_lb.IndexFromPoint(e.Location)
        If (m_iItemIndexSelected = iIndexClicked) And (iIndexClicked >= 0) Then
            Me.BeginEdit(Me.m_lb.SelectedIndex())
        End If
        m_iItemIndexSelected = iIndexClicked
    End Sub

#End Region ' List box events

#Region " Floating edit box events "

    Private Sub m_tb_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles m_tb.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                If Me.m_bEndEditOnEscape Then Me.EndEdit()
            Case Keys.Enter
                If Me.m_bEndEditOnReturn Then Me.EndEdit()
        End Select
        e.SuppressKeyPress = False
    End Sub

    Private Sub m_tb_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tb.LostFocus
        If Me.m_bEndEditOnLooseFocus Then Me.EndEdit()
    End Sub

#End Region ' Floating edit box events

#End Region ' Private events

End Class
