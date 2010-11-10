#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports WeifenLuo.WinFormsUI

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Presentations settings interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsPresentation
        Implements IOptionsPage

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_cbHideModelBar.Checked = My.Settings.PresentationModeHideModelBar
            Me.m_cbHideMainMenu.Checked = My.Settings.PresentationModeHideMainMenu
            Me.m_cbHideStatusBar.Checked = My.Settings.PresentationModeHideStatusBar
            Me.m_cbCollapseNavPanel.Checked = My.Settings.PresentationModeCollapseNavPanel
            Me.m_cbCollapseRemarkPanel.Checked = My.Settings.PresentationModeCollapseRemarkPanel
            Me.m_cbCollapseStatusPanel.Checked = My.Settings.PresentationModeCollapseStatusPanel

            Me.m_cbRestoreItems.Checked = My.Settings.PresentationModeRestoreSettings

            Me.UpdateControls()

        End Sub

#End Region ' Overrides

#Region " Public access "

        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            My.Settings.PresentationModeHideModelBar = Me.m_cbHideModelBar.Checked
            My.Settings.PresentationModeHideMainMenu = Me.m_cbHideMainMenu.Checked
            My.Settings.PresentationModeHideStatusBar = Me.m_cbHideStatusBar.Checked
            My.Settings.PresentationModeCollapseNavPanel = Me.m_cbCollapseNavPanel.Checked
            My.Settings.PresentationModeCollapseRemarkPanel = Me.m_cbCollapseRemarkPanel.Checked
            My.Settings.PresentationModeCollapseStatusPanel = Me.m_cbCollapseStatusPanel.Checked

            My.Settings.PresentationModeRestoreSettings = Me.m_cbRestoreItems.Checked

            Return IOptionsPage.eApplyResultType.Success

        End Function

#End Region ' Public access

#Region " Internals "

        Private Sub UpdateControls()

        End Sub

#End Region ' Internals

    End Class

End Namespace
