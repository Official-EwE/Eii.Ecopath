'==============================================================================
'
' $Log: RemarkPanel.vb,v $
' Revision 1.3  2009/04/06 14:47:26  jeroens
' Reworked WithEvents to prevent crashes on improper clean-up
'
' Revision 1.2  2009/03/22 14:01:38  jeroens
' Core state monitor exec event parameters simplified
'
' Revision 1.1  2008/09/26 07:32:11  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/06/02 00:01:44  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.15  2008/05/07 21:30:22  jeroens
' Mixed remarks will clear the remark panel
'
' Revision 1.14  2008/05/06 00:43:28  jeroens
' Only enabled when selection has properties that allow remarks
'
' Revision 1.13  2008/05/04 12:50:37  jeroens
' Added Set button
' Fixed multiple property update bug
'
' Revision 1.12  2008/05/04 01:48:27  jeroens
' Simplified cProperty remark interface
'
' Revision 1.11  2008/04/01 18:19:44  jeroens
' Freed-up screen estate for remark text
'
' Revision 1.10  2008/01/24 01:32:06  jeroens
' Explicitly handles selected indexed vars
' Selection label no longer bold
'
' Revision 1.9  2007/12/09 16:40:24  jeroens
' * argh
'
' Revision 1.8  2007/12/09 15:33:43  jeroens
' * Localized
'
' Revision 1.7  2007/12/09 03:31:21  jeroens
' * Fixed loss of remark changes when new data was selected without switch of focus
'
' Revision 1.6  2007/10/14 16:30:43  jeroens
' - Released StateMonitor instance
'
' Revision 1.5  2007/10/03 02:21:40  jeroens
' * VarName field no longer selectable
'
' Revision 1.4  2007/09/07 15:06:20  jeroens
' + Made core state aware (fixed bug 199)
'
' Revision 1.3  2007/07/26 18:03:44  jeroens
' - Removed variable description
'
' Revision 1.2  2007/07/03 18:34:04  jeroens
' * Uses shared prop selection command
'
' Revision 1.1  2007/07/03 15:15:57  jeroens
' wtf
'
' Revision 1.4  2007/07/03 15:11:03  jeroens
' *** empty log message ***
'
' Revision 1.3  2007/07/03 15:07:10  jeroens
' Renamed, once again
'
' Revision 1.1  2007/07/03 14:54:12  jeroens
' * Renamed
'
' Revision 1.6  2007/07/01 06:11:58  jeroens
' + Handles multiple selection
'
' Revision 1.5  2007/07/01 05:26:03  jeroens
' + Prepared for receiving extended selection
'
' Revision 1.4  2007/02/09 15:58:42  jeroens
' Redesigned
'
' Revision 1.3  2006/10/03 03:21:48  jeroens
' * Reorganized and restructured
' + Added capabilites to use sourceless Properties (such as cFormulaProperty)
'
' Revision 1.2  2006/10/02 16:15:22  jeroens
' + Uses ValueExplorer
' + Added detailed description
'
' Revision 1.1  2006/10/02 02:59:45  jeroens
' Initial version
'
'==============================================================================

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
    Private m_bTextChanged As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of the PropertiesPanel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        ' Do the .NET stuff
        Me.InitializeComponent()

        ' Create property selection command
        Me.m_cmd = CType(CommandHandler.GetInstance().GetCommand(PropertySelectionCommand.COMMAND_NAME), PropertySelectionCommand)
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

    Private Sub m_btnSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSet.Click
        Me.ApplyChanges()
    End Sub

#Region " Command handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, invoked when the <see cref="m_cmd">selection command</see>
    ''' is invoked from anywhere in the GUI.
    ''' </summary>
    ''' <param name="cmd">The <see cref="Command">Command</see> that was invoked.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnInvoke(ByVal cmd As Command)

        ' Sanity check
        If Not (cmd Is m_cmd) Then Return

        ' Get selected props
        Me.m_aprop = m_cmd.Selection()
        ' Update panel state
        Me.UpdateControls()
        ' Update panel content
        Me.UpdateContents()
        ' Clear any changes
        Me.PendingChanges = False

    End Sub

#End Region ' Command handling 

#Region " GUI handling "

    Private Sub m_tbRemark_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tbRemark.TextChanged
        Me.PendingChanges = True
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the remarks text box looses focus, to
    ''' apply any text changes to the selected Property.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnLeavePanel(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tbRemark.Leave
        ' Could be called in response to closing app!
        If (Not Me.m_sm.HasEcopathLoaded()) Then Return
        ' Apply any pending changes
        If (Me.PendingChanges = True) Then Me.ApplyChanges()
    End Sub

    Private m_bInUpdate As Boolean = False

    Private Sub ApplyChanges()

        Dim strRemark As String = Me.m_tbRemark.Text

        Me.PendingChanges = False

        Me.m_bInUpdate = True
        For Each p As cProperty In Me.m_aprop
            p.SetRemark(strRemark)
        Next p
        Me.m_bInUpdate = False

        Me.UpdateContents()

    End Sub

    Private Property PendingChanges() As Boolean
        Get
            Return Me.m_bTextChanged
        End Get
        Set(ByVal value As Boolean)
            Me.m_bTextChanged = value
        End Set
    End Property

#End Region ' GUI handling 

#Region " Core state response "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, responds to core state change events to assess whether this panel should be available.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnCoreExecutionStateEvent(ByVal csm As cCoreStateMonitor)
        Me.UpdateControls()
    End Sub

#End Region ' Core state response

#Region " Internal implementation "

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