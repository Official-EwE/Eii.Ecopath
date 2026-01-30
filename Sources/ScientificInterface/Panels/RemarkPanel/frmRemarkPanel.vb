' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style
Imports EwEUtils.Logging
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports SharedResources = ScientificInterfaceShared.My.Resources



''' ---------------------------------------------------------------------------
''' <summary>
''' Panel that provides details for a selected core value. From here, remarks
''' can be manipulated, and users can view metadata about their selected value
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
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmRemarkPanel)()

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of the RemarkPanel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(uic As cUIContext)
        Me.InitializeComponent()
        Me.m_uic = uic
        Me.m_mon = New cSelectionMonitor()
    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.m_uic Is Nothing) Then Return

        ' Hook up to core state monitor
        Me.m_sm = Me.m_uic.Core.StateMonitor
        AddHandler Me.m_sm.CoreExecutionStateEvent, AddressOf Me.OnCoreExecutionStateEvent

        Me.m_mon.Attach(Me.m_uic)
        AddHandler Me.m_mon.OnSelectionChanged, AddressOf Me.OnSelectionChanged

        Me.Icon = Icon.FromHandle(SharedResources.CommentHS.GetHicon)
        Me.m_tsbnInfo.Image = SharedResources.Info

        Me.m_tsbnInfo.Checked = My.Settings.ShowExtraVariableInfo
        Me.m_scMain.Panel1Collapsed = Not Me.m_tsbnInfo.Checked

        ' Init panel
        Me.UpdateContents()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        ' Clean up
        If (Me.m_uic IsNot Nothing) Then
            RemoveHandler Me.m_sm.CoreExecutionStateEvent, AddressOf Me.OnCoreExecutionStateEvent
            Me.m_sm = Nothing
            RemoveHandler Me.m_mon.OnSelectionChanged, AddressOf Me.OnSelectionChanged
            Me.m_mon.Detach()
            Me.m_uic = Nothing

        End If
        MyBase.OnFormClosed(e)

        Try
            Me.Icon.Destroy()
        Catch ex As Exception

        End Try
    End Sub

    Public Overrides Function PanelType() As frmEwEDockContent.ePanelType
        Return ePanelType.SystemPanel
    End Function

    Protected Overrides Sub OnSizeChanged(e As System.EventArgs)
        MyBase.OnSizeChanged(e)

        Dim rc As Rectangle = Me.ClientRectangle

        If (rc.Width > 50 And rc.Height > 50) Then
            If (rc.Width > rc.Height) Then
                Me.m_scMain.Orientation = Orientation.Vertical
                Me.m_scMain.SplitterDistance = 200
            Else
                Me.m_scMain.Orientation = Orientation.Horizontal
                Me.m_scMain.SplitterDistance = 100
            End If
        End If

    End Sub

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
            RemoveHandler p.PropertyChanged, AddressOf Me.OnPropertyChanged
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
                    AddHandler p.PropertyChanged, AddressOf Me.OnPropertyChanged
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
    Private Sub OnRemarkTextChanged(sender As Object, e As System.EventArgs) _
        Handles m_tbxRemark.TextChanged
        Me.HasPendingChanges = True
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event hander, called when the user applies changes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnApply(sender As System.Object, e As System.EventArgs) _
        Handles m_btnApply.Click
        Me.Apply()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the remarks text box looses focus, to
    ''' apply any text changes to selected <see cref="cProperty">properties</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnLeavePanel(sender As Object, e As System.EventArgs) _
        Handles m_tbxRemark.Leave

        ' Could be called in response to closing app!
        If (Not Me.m_sm.HasEcopathLoaded()) Then Return
        ' Apply any pending changes
        If (Me.HasPendingChanges = True) Then Me.Apply()

    End Sub

    Private Sub OnToggleShowInfo(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnInfo.CheckedChanged

        Try
            Me.m_scMain.Panel1Collapsed = Not Me.m_tsbnInfo.Checked
            My.Settings.ShowExtraVariableInfo = Me.m_tsbnInfo.Checked
        Catch ex As Exception

        End Try

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
            m_logger.LogError(ex, "frmInformationPanel::Apply")
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
        Set(value As Boolean)
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
    Private Sub OnCoreExecutionStateEvent(csm As cCoreStateMonitor)
        Me.UpdateControls()
    End Sub

    Private Sub OnPropertyChanged(p As cProperty, ct As cProperty.eChangeFlags)
        Me.BeginInvoke(New MethodInvoker(AddressOf Me.UpdateContents))
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
            If Not String.IsNullOrEmpty(p.ID) Then bHasSelection = True
        Next

        Me.m_btnApply.Enabled = bHasSelection
        Me.m_tbxRemark.Enabled = bHasSelection

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state and contents of the controls in the panel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateContents()

        Dim strVarN As String = ""
        Dim strName As String = My.Resources.SELECTION_NONE
        Dim strDescription As String = ""
        Dim strDomain As String = ""
        Dim strStatus As String = ""
        Dim strRemark As String = ""
        Dim strUnits As String = ""
        Dim props() As cProperty = Me.m_mon.Selection
        Dim prop As cProperty = Nothing
        Dim vnf As New cVarnameTypeFormatter()
        Dim mdf As New cMetadataTypeFormatter(Me.m_uic.Core, Me.m_uic.StyleGuide)
        Dim stf As New cStyleTypeFormatter()
        Dim bEditable As Boolean = False

        If (props IsNot Nothing) Then
            Select Case props.Length

                Case 0
                    ' NOP

                Case 1

                    prop = props(0)

                    ' Get selection text
                    If (prop.Source IsNot Nothing) Then

                        ' Get variable descriptor
                        Dim var As eVarNameFlags = prop.VarName
                        Dim fmt As New cCoreInterfaceFormatter()

                        strVarN = vnf.ToString(var, eDescriptorTypes.Name)

                        ' Format message
                        If prop.SourceSec IsNot Nothing Then
                            strName = String.Format(My.Resources.SELECTION_INDEXEDVAR,
                                                    fmt.ToString(prop.Source),
                                                    strVarN,
                                                    fmt.ToString(prop.SourceSec))
                        Else
                            strName = String.Format(SharedResources.GENERIC_LABEL_DETAILED,
                                                    fmt.ToString(prop.Source),
                                                    strVarN)
                        End If

                        strDescription = vnf.ToString(var, eDescriptorTypes.Description)
                        strDomain = mdf.ToString(prop.GetVariableMetadata())
                        strStatus = stf.ToString(prop.GetStyle(), eDescriptorTypes.Description)
                        strRemark = prop.GetRemark()

                        bEditable = ((prop.GetStyle() And cStyleGuide.eStyleFlags.NotEditable) = 0)
                    Else
                        strName = My.Resources.SELECTION_DERIVED
                    End If

                Case Else
                    Dim var As eVarNameFlags = eVarNameFlags.NotSet
                    Dim bMixed As Boolean = False
                    Dim strTmp As String = ""

                    For Each prop In props
                        If (var = eVarNameFlags.NotSet) Then
                            var = prop.VarName
                            strName = String.Format(My.Resources.SELECTION_SINGLEVAR, My.Resources.SELECTION_MULTIPLE, vnf.ToString(var))
                            strDescription = vnf.ToString(var, eDescriptorTypes.Description)
                            strDomain = mdf.ToString(prop.GetVariableMetadata())
                            strStatus = stf.ToString(prop.GetStyle())
                        Else
                            bMixed = bMixed Or (var <> prop.VarName)
                        End If

                        strTmp = prop.GetRemark().Trim

                        ' Is valid remark text?
                        If (Not String.IsNullOrWhiteSpace(strTmp)) Then
                            ' No remark picked yet?
                            If String.IsNullOrWhiteSpace(strRemark) Then
                                ' #Yes: store remark
                                strRemark = strTmp
                            Else
                                ' #No: does this remark differ from existing remark?
                                If (String.Compare(strRemark, strTmp, False) <> 0) Then
                                    ' #Yes: clear final remark text, stop looking because the text is mixed
                                    strRemark = ""
                                    Exit For
                                End If
                            End If
                        End If
                    Next

                    If bMixed Then
                        strName = My.Resources.SELECTION_MULTIPLE
                        strDescription = ""
                        strDomain = ""
                        strStatus = ""
                    End If
            End Select
        End If

        ' No need to repeat things
        If (String.Compare(strVarN, strDescription, True) = 0) Then strDescription = ""

        ' Update control contents
        Me.m_lblVarName.Text = strName

        If (String.IsNullOrWhiteSpace(strStatus)) Then
            Me.m_lblStatus.Visible = False
        Else
            Me.m_lblStatus.Text = strStatus
            Me.m_lblStatus.Visible = True
        End If

        If String.IsNullOrWhiteSpace(strDescription) Then
            Me.m_lblDescription.Visible = False
        Else
            Me.m_lblDescription.Text = String.Format(My.Resources.INFOPANEL_DESCRIPTION, strDescription)
            Me.m_lblDescription.Visible = True
        End If

        If (String.IsNullOrWhiteSpace(strDomain) Or (bEditable = False)) Then
            Me.m_lblDomain.Visible = False

        Else
            Me.m_lblDomain.Text = String.Format(My.Resources.INFOPANEL_DOMAIN, strDomain)
            Me.m_lblDomain.Visible = True
        End If

        Me.m_tbxRemark.Text = strRemark

    End Sub

#End Region ' Internal implementation

End Class