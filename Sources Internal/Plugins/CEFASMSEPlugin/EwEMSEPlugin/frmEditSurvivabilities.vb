Imports LumenWorks.Framework.IO.Csv
Imports System.IO
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources


Public Class frmEditSurvivabilities
    Implements IDisposable

    Private m_plugin As cMSEPluginPoint = Nothing
    Public mSurvivability As cSurvivability
    Private m_bIsDirty As Boolean

    Public Sub New(MSE As cMSE)

        Me.InitializeComponent()

    End Sub

    Public Sub Init(ByVal uic As cUIContext, Plugin As cMSEPluginPoint)
        Me.UIContext = uic
        Me.m_grid.UIContext = uic
        Me.m_grid.Init(Plugin.MSE, Plugin.MSE.Survivabilities)
        Me.m_plugin = Plugin
        Me.mSurvivability = Plugin.MSE.Survivability
        Me.m_grid.Left = 10
        Me.m_grid.Top = 10
        Me.m_grid.Height = 500
        'Me.m_grid.Width = 1000


        Dim bSave As Boolean = Me.m_bIsDirty

        UpdateGrid(mSurvivability.ListofSurvDistParams, My.Resources.HEADER_SURVIVABILITIES)

    End Sub

    Private ReadOnly Property MSE As cMSE
        Get
            Return Me.m_plugin.MSE
        End Get
    End Property

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As ScientificInterfaceShared.Controls.cUIContext)
            MyBase.UIContext = value
        End Set
    End Property

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        ' JS 30Sep13: globalized this method
        MyBase.OnLoad(e)

        AddHandler Me.m_grid.onEdited, AddressOf OnGridEdited

        Me.m_bIsDirty = False
        Me.CenterToParent()
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)

        If (Me.m_bIsDirty = True) Then
            ' JS 02Oct13: globalized this method
            ' JS 02Oct13: replaced MsgBox with cFeedbackMessage
            Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_UNSAVED_CHANGES, _
                                 eCoreComponentType.External, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            fmsg.Reply = eMessageReply.YES
            Me.Core.Messages.SendMessage(fmsg)
            e.Cancel = (fmsg.Reply <> eMessageReply.YES)
        End If

        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_grid.onEdited, AddressOf OnGridEdited
        Me.m_grid.UIContext = Nothing

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()
        ' Me.m_btnOK.Enabled = Me.m_bIsDirty
    End Sub

    Private Sub UpdateGrid(data As List(Of cSurvivability.cSurvivabilityDistributonParam), strName As String)
        Me.m_grid.Data = data
        Me.m_grid.DataName = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, My.Resources.CAPTION, strName)
    End Sub

    Private Sub m_btnOK_Click(sender As System.Object, e As System.EventArgs) Handles m_btnOK.Click
        Dim lstrSubMessages As New List(Of String)
        Dim strFolder As String = cMSEUtils.MSEFolder(Me.MSE.DataPath, cMSEUtils.eMSEPaths.DistrParams)

        If Not Me.MSE.IsInputStructureAvailable(True) Then
            ' ToDo: report error
            Return
        End If

        'Saves all the parameters to csv when user clicks to save
        If mSurvivability.SaveDistributionParamsToCSV() Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "Survivabilities_dist.csv"))

        Me.m_bIsDirty = False

        Me.m_plugin.InformUser(String.Format(My.Resources.STATUS_SAVED_DISTPARMS, My.Resources.CAPTION, strFolder), _
                                 eMessageImportance.Information, strFolder, lstrSubMessages.ToArray())

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub OnGridEdited()
        Me.m_bIsDirty = True
        Me.Invoke(New MethodInvoker(AddressOf UpdateControls))
    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

End Class