Option Strict On
Imports EwECore
Imports EwEUtils.Commands
Imports System.Text

''' ---------------------------------------------------------------------------
''' <summary>
''' Panel that provides details for a selected core value. From here, remarks
''' and references can be manipulated.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class RemarkPanel

    ''' <summary>The property selection command to listen to.</summary>
    Private m_cmd As PropertySelectionCommand = Nothing
    ''' <summary>The currently selected property.</summary>
    Private m_aprop() As cProperty = Nothing
    ''' <summary>State monitor to observe.</summary>
    Private m_sm As cCoreStateMonitor = Nothing
    ''' <summary>Flag stating whether the user has made any textual changes.</summary>
    Private m_bHasPendingChanges As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of the PropertiesPanel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        ' Do the .NET stuff
        Me.InitializeComponent()

        ' Create property selection command
        Me.m_cmd = CType(cCommandHandler.GetInstance().GetCommand(PropertySelectionCommand.COMMAND_NAME), PropertySelectionCommand)
        AddHandler Me.m_cmd.OnInvoke, AddressOf OnInvoke

        ' Hook up to core state monitor
        Me.m_sm = cCore.GetInstance().StateMonitor
        AddHandler Me.m_sm.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateEvent

        ' Init panel
        Me.UpdateControls()

    End Sub

    Private Sub OnDisposed(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.Disposed

        ' Clean up
        If Me.m_cmd IsNot Nothing Then

            RemoveHandler Me.m_cmd.OnInvoke, AddressOf OnInvoke
            Me.m_cmd = Nothing

            RemoveHandler Me.m_sm.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateEvent
            Me.m_sm = Nothing
        End If

    End Sub


#Region " Command handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, invoked when the <see cref="m_cmd">selection command</see>
    ''' is invoked from anywhere in the GUI.
    ''' </summary>
    ''' <param name="cmd">The <see cref="Command">Command</see> that was invoked.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnInvoke(ByVal cmd As cCommand)

        ' Sanity check
        If Not (cmd Is m_cmd) Then Return

        ' Get selected props
        Me.m_aprop = m_cmd.Selection()
        ' Update panel state
        Me.UpdateControls()
        ' Update panel content
        Me.UpdateContents()
        ' Clear any changes
        Me.HasPendingChanges = False

    End Sub

#End Region ' Command handling 

#Region " GUI handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when remark text has been edited by the user.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_tbRemark_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_tbRemark.TextChanged
        Me.HasPendingChanges = True
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event hander, called when the user applies changes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSet.Click
        Me.Apply()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the remarks text box looses focus, to
    ''' apply any text changes to selected <see cref="cProperty">properties</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnLeavePanel(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_tbRemark.Leave
        ' Could be called in response to closing app!
        If (Not Me.m_sm.HasEcopathLoaded()) Then Return
        ' Apply any pending changes
        If (Me.HasPendingChanges = True) Then Me.Apply()
    End Sub

    ''' <summary>Update feedback loop prevention flag.</summary>
    Private m_bInUpdate As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply the content of the remark panel to all selected properties.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub Apply()

        Dim strRemark As String = Me.m_tbRemark.Text

        Me.HasPendingChanges = False

        Me.m_bInUpdate = True
        For Each p As cProperty In Me.m_aprop
            p.SetRemark(strRemark)
        Next p
        Me.m_bInUpdate = False

        Me.UpdateContents()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether the panel has any pending remark text changes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Property HasPendingChanges() As Boolean
        Get
            Return Me.m_bHasPendingChanges
        End Get
        Set(ByVal value As Boolean)
            Me.m_bHasPendingChanges = value
        End Set
    End Property

#End Region ' GUI handling 

#Region " Core state response "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, responds to core state change events to update its state.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnCoreExecutionStateEvent(ByVal csm As cCoreStateMonitor)
        Me.UpdateControls()
    End Sub

#End Region ' Core state response

#Region " Internal implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state of this panel and its controls
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()

        Dim bHasEcopath As Boolean = Me.m_sm.HasEcopathLoaded()
        Dim bHasSelection As Boolean = False

        If Me.m_aprop IsNot Nothing Then
            For Each p As cProperty In Me.m_aprop
                If Not String.IsNullOrEmpty(p.ID) Then bHasSelection = True
            Next
        End If

        Me.m_btnSet.Visible = bHasEcopath
        Me.m_btnSet.Enabled = bHasSelection
        Me.m_tbRemark.Visible = bHasEcopath
        Me.m_tbRemark.Enabled = bHasSelection

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state and contents of the controls in the panel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateContents()

        Dim strSelection As String = My.Resources.SELECTION_NONE
        Dim strRemark As String = ""
        Dim strRemarkFinal As String = ""

        If Me.m_aprop IsNot Nothing Then
            Select Case Me.m_aprop.Length

                Case 0
                    ' NOP

                Case 1
                    ' Get selection text
                    If Not Object.ReferenceEquals(m_aprop(0).Source, Nothing) Then
                        If Not Object.ReferenceEquals(m_aprop(0).SourceSec, Nothing) Then
                            strSelection = String.Format(My.Resources.SELECTION_INDEXEDVAR, m_aprop(0).Source.Name, ValueExplorer.GetName(m_aprop(0).VarName), m_aprop(0).SourceSec.Name)
                        Else
                            strSelection = String.Format(My.Resources.GENERIC_LABEL_DETAILEDLABEL, m_aprop(0).Source.Name, ValueExplorer.GetName(m_aprop(0).VarName))
                        End If
                    Else
                        strSelection = My.Resources.SELECTION_DERIVED
                    End If

                Case Else
                    strSelection = My.Resources.SELECTION_MULTIPLE

            End Select

            ' Concat remark text of selected properties
            For iProp As Integer = 0 To Me.m_aprop.Length - 1
                ' Get remark text for this property
                strRemark = Me.m_aprop(iProp).GetRemark().Trim
                ' Is valid remark text?
                If (Not String.IsNullOrEmpty(strRemark)) Then
                    ' No remark picked yet?
                    If String.IsNullOrEmpty(strRemarkFinal) Then
                        ' #Yes: store remark
                        strRemarkFinal = strRemark
                    Else
                        ' #No: does this remark differ from existing remark?
                        If (String.Compare(strRemarkFinal, strRemark, False) <> 0) Then
                            ' #Yes: clear final remark text, stop looking because the text is mixed
                            strRemarkFinal = ""
                            Exit For
                        End If
                    End If
                End If
            Next
        End If

        ' Update control contents
        Me.m_lbVarName.Text = strSelection
        Me.m_tbRemark.Text = strRemarkFinal

    End Sub

#End Region ' Internal implementation

End Class