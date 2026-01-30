' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Logging
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug
Imports SharedResources = ScientificInterfaceShared.My.Resources



Namespace Other

    ''' =======================================================================
    ''' <summary>
    ''' EwE about box form.
    ''' </summary>
    ''' =======================================================================
    Public Class frmAboutEwE

        Private m_uic As cUIContext = Nothing
        Private m_qehTech As cQuickEditHandler = Nothing
        Private m_bInUpdate As Boolean = False
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmAboutEwE)()

        Public Sub New(uic As cUIContext)
            Me.InitializeComponent()
            Me.m_uic = uic
            Me.m_gridTechnical.UIContext = uic
            Me.m_gridDatabase.UIContext = uic
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.m_uic Is Nothing) Then Return

            Me.m_bInUpdate = True

            Dim cmd As cCommand = Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME)
            cmd.AddControl(Me.m_rtbAcknowledgements)
            cmd.AddControl(Me.m_rtbDisclaimer)
            cmd.AddControl(Me.m_rtbDistribution)
            cmd.AddControl(Me.m_rtbLicense)

            ' Format generic page
            Me.Text = cStringUtils.Localize(My.Resources.ABOUT_CAPTION, My.Resources.GENERIC_CAPTION)
            Me.m_lbTitle.Text = EwEVersion(True, True, False)
            Me.m_lbVersion.Text = EwERelease()


            Me.m_lbCopyright.Text = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, My.Application.Info.Copyright, My.Application.Info.CompanyName)

            'If (Me.m_uic.RegisteredExpiration <> Nothing) Then
            '    Me.m_lblExpiry.Visible = Me.m_uic.RegisteredExpiration < cDateUtils.StartTime
            'End If

            ' Format RTF content pages
            Me.m_rtbTeam.Rtf = Me.StyleRTF(My.Resources.team)
            Me.m_rtbLicense.Rtf = Me.StyleRTF(My.Resources.license)
            Me.m_rtbAcknowledgements.Rtf = Me.StyleRTF(My.Resources.acknowledgements)

            ' Format technical page
            Me.m_lblNetVersion.Text = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, cSysConfig.OSVersion(), cSysConfig.NETVersion())

            Me.m_qehTech = New cQuickEditHandler()
            Me.m_qehTech.Attach(Me.m_gridTechnical, Me.m_uic, Me.m_tsTechnical)

            Me.m_tsbnShowEwEAssembliesOnly.Image = SharedResources.FilterHS
            Me.m_tsbnShowEwEAssembliesOnly.Checked = Me.m_gridTechnical.ShowEwEComponentsOnly

            ' Format database page
            If Not Me.m_uic.Core.StateMonitor.HasEcopathLoaded Then
                Me.m_tcMain.TabPages.Remove(Me.m_tpDatabase)
            End If

            Me.UpdateLicenseControls()
            Me.m_bInUpdate = False

        End Sub

        Private Sub UpdateLicenseControls()
            Me.m_lbRegistation.Text = EwERegistration(Me.m_uic.Core.License)
            Me.m_lbRegistation.Visible = Not String.IsNullOrWhiteSpace(Me.m_lbRegistation.Text)
            Me.m_btnRemoveRegistration.Enabled = Me.m_uic.Core.License.IsRegistered
        End Sub

        Protected Overrides Sub OnClosed(e As System.EventArgs)

            Dim cmd As cCommand = Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME)
            cmd.RemoveControl(Me.m_rtbAcknowledgements)
            cmd.RemoveControl(Me.m_rtbDisclaimer)
            cmd.RemoveControl(Me.m_rtbDistribution)
            cmd.RemoveControl(Me.m_rtbLicense)

            Me.m_qehTech.Detach()
            MyBase.OnClosed(e)

        End Sub

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles m_btnOK.Click
            Me.Close()
        End Sub

        Private Sub OnToggleEwECOmponentView(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnShowEwEAssembliesOnly.Click

            If (Me.m_bInUpdate) Then Return
            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
            Try
                Me.m_gridTechnical.ShowEwEComponentsOnly = Me.m_tsbnShowEwEAssembliesOnly.Checked
            Catch ex As Exception
                Debug.Assert(False)
            End Try
            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Switch RTF text from 9 pt Arial to local form font and size.
        ''' </summary>
        ''' <param name="strRTF">Text to convert, needing to specify Arial font at 18 pt</param>
        ''' <returns>A transmogrified text.</returns>
        ''' -------------------------------------------------------------------
        Private Function StyleRTF(strRTF As String) As String

            Dim strFont As String = Me.Font.FontFamily.Name
            Dim szFont As Single = Me.Font.Size

            strRTF = strRTF.Replace("Arial;", strFont & ";")
            strRTF = strRTF.Replace("\fs18", "\fs" & CInt(szFont * 2))

            Return strRTF
        End Function

        Private Sub OnEnterRegistration(sender As Object, e As EventArgs) Handles m_btnEnterRegistration.Click

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmd As cEnterLicenseCommand = CType(cmdh.GetCommand(cEnterLicenseCommand.cCOMMAND_NAME), cEnterLicenseCommand)
            cmd.Invoke()
            Me.UpdateLicenseControls()

        End Sub

        Private Sub OnRemoveRegistration(sender As Object, e As EventArgs) Handles m_btnRemoveRegistration.Click

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmd As cClearLicenseCommand = CType(cmdh.GetCommand(cClearLicenseCommand.cCOMMAND_NAME), cClearLicenseCommand)
            cmd.Invoke()
            Me.UpdateLicenseControls()

        End Sub

        Private Sub OnOrder(sender As Object, e As EventArgs) Handles m_btnOrder.Click

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmd As cBrowserCommand = CType(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            cmd.Invoke("https://ecopath.org/go-pro")

        End Sub

        Private Sub OnLinkClicked(sender As Object, e As LinkClickedEventArgs) Handles m_rtbAcknowledgements.LinkClicked, m_rtbDisclaimer.LinkClicked, m_rtbDistribution.LinkClicked, m_rtbLicense.LinkClicked, m_rtbTeam.LinkClicked
            Try
                Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
                Dim cmd As cBrowserCommand = CType(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
                cmd.Invoke(e.LinkText)
            Catch ex As Exception
                m_logger.LogError(ex, "OnLinkClicked. Error invoking browser for link {0}", e.LinkText)
            End Try
        End Sub

    End Class

End Namespace

