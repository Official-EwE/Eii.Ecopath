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
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Forms
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.modExtensions
Imports System.Text

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Panel that provides details for a selected core value. From here, remarks
''' and references can be manipulated.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmRemarkPanel

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_mon As cSelectionMonitor = Nothing
    ''' <summary>State monitor to observe.</summary>
    Private m_sm As cCoreStateMonitor = Nothing
    ''' <summary>Flag stating whether the user has made any textual changes.</summary>
    Private m_bHasPendingChanges As Boolean = False

    ''' <summary>Properties being listened to.</summary>
    Private m_lProps As New List(Of cProperty)

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of the RemarkPanel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext)
        Me.InitializeComponent()
        Me.m_uic = uic
        Me.m_mon = New cSelectionMonitor()
    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.m_uic Is Nothing) Then Return

        ' Hook up to core state monitor
        Me.m_sm = Me.m_uic.Core.StateMonitor
        AddHandler Me.m_sm.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateEvent

        Me.m_mon.Attach(Me.m_uic)
        AddHandler Me.m_mon.OnSelectionChanged, AddressOf OnSelectionChanged

        Me.Icon = Icon.FromHandle(SharedResources.CommentHS.GetHicon)

        ' Init panel
        Me.UpdateControls()
        Me.UpdateContents()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        ' Clean up
        If (Me.m_uic IsNot Nothing) Then
            RemoveHandler Me.m_sm.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateEvent
            Me.m_sm = Nothing
            RemoveHandler Me.m_mon.OnSelectionChanged, AddressOf OnSelectionChanged
            Me.m_mon.Detach()
            Me.m_uic = Nothing

            Me.Icon.Destroy()
        End If
        MyBase.OnFormClosed(e)

    End Sub

    Public Overrides Function PanelType() As frmEwEDockContent.ePanelType
        Return ePanelType.SystemPanel
    End Function

#End Region ' Form overrides

#Region " Command handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, invoked when the <see cref="cSelectionMonitor">selection has changed</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnSelectionChanged(mon As cSelectionMonitor)

        If Me.m_bHasPendingChanges Then
            Me.Apply()
        End If

        For Each p As cProperty In Me.m_lProps
            RemoveHandler p.PropertyChanged, AddressOf OnPropertyChanged
        Next
        Me.m_lProps.Clear()

        ' Update panel state
        Me.UpdateControls()
        ' Update panel content
        Me.UpdateContents()

        If (mon IsNot Nothing) Then
            If (mon.Selection IsNot Nothing) Then
                Me.m_lProps.AddRange(mon.Selection)
                For Each p As cProperty In Me.m_lProps
                    AddHandler p.PropertyChanged, AddressOf OnPropertyChanged
                Next
            End If
        End If

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
    Private Sub OnRemarkTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_tbxRemark.TextChanged
        Me.HasPendingChanges = True
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event hander, called when the user applies changes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnApply(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnApply.Click
        Me.Apply()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the remarks text box looses focus, to
    ''' apply any text changes to selected <see cref="cProperty">properties</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnLeavePanel(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_tbxRemark.Leave

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

        Dim strRemark As String = Me.m_tbxRemark.Text

        Me.HasPendingChanges = False
        Me.m_bInUpdate = True
        Try
            For Each p As cProperty In Me.m_lProps
                If Not p.IsDisposed Then p.SetRemark(strRemark)
            Next p
        Catch ex As Exception
            cLog.Write(ex, "frmRemarkPanel::Apply")
        End Try
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

    Private Sub OnPropertyChanged(ByVal p As cProperty, ByVal ct As cProperty.eChangeFlags)
        Me.BeginInvoke(New MethodInvoker(AddressOf UpdateContents))
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

        For Each p As cProperty In Me.m_mon.Selection
            If Not String.IsNullOrEmpty(p.ID) Then bHasSelection = bHasSelection Or p.IsStored
        Next

        Me.m_btnApply.Visible = bHasEcopath
        Me.m_btnApply.Enabled = bHasSelection
        Me.m_tbxRemark.Visible = bHasEcopath
        Me.m_tbxRemark.Enabled = bHasSelection

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state and contents of the controls in the panel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateContents()

        Dim fmtSel As New cSelectionMonitorFormatter()
        Dim fmtMeta As New cMetadataTypeFormatter()
        Dim props() As cProperty = Me.m_mon.Selection
        Dim strSelection As String = fmtSel.GetDescriptor(Me.m_mon, eDescriptorTypes.Name)
        Dim strDescription As String = fmtSel.GetDescriptor(Me.m_mon, eDescriptorTypes.Description)
        Dim strMeta As String = Nothing
        Dim strRemark As String = ""
        Dim strRemarkFinal As String = ""

        If (props IsNot Nothing) Then

            ' Concat remark text of selected properties
            For iProp As Integer = 0 To props.Length - 1
                ' Get remark text for this property
                strRemark = props(iProp).GetRemark().Trim

                strMeta = fmtMeta.GetDescriptor(props(iProp).GetVariableMetadata())

                ' Is valid remark text?
                If (Not String.IsNullOrWhiteSpace(strRemark)) Then
                    ' No remark picked yet?
                    If String.IsNullOrWhiteSpace(strRemarkFinal) Then
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

        Dim sbTooltip As New StringBuilder()
        sbTooltip.Append(strDescription.Trim())
        If (Not String.IsNullOrWhiteSpace(strMeta)) Then
            If (sbTooltip.Length > 0) Then sbTooltip.AppendLine()
            sbTooltip.Append(strMeta)
        End If

        ' Update control contents
        Me.m_lblVarName.Text = strSelection
        'Me.m_lblVarName.ToolTipText = sbTooltip.ToString()
        Me.m_tbxRemark.Text = strRemarkFinal

    End Sub

#End Region ' Internal implementation

End Class