' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991 Ecopath International Initiative (EII)

Imports System.Drawing

Namespace Other

    Public Class ucOptionsPrivacy
        Inherits UserControl
        Implements IOptionsPage

        Private ReadOnly m_cbSendDiagnostics As New CheckBox()
        Private ReadOnly m_description As New Label()

        Public Property UIContext As cUIContext Implements IUIElement.UIContext

        Public Sub New(uic As cUIContext)
            Me.UIContext = uic
            Me.AutoScaleMode = AutoScaleMode.Dpi
            Me.Padding = New Padding(16)

            Dim title As New Label With {
                .AutoSize = True,
                .Dock = DockStyle.Top,
                .Font = New Font(SystemFonts.DefaultFont, FontStyle.Bold),
                .Text = "Help improve Ecopath with Ecosim"
            }

            Me.m_description.AutoSize = True
            Me.m_description.Dock = DockStyle.Fill
            Me.m_description.Text = "Help us improve the software by automatically sending anonymous diagnostic information when errors occur. This data helps us identify bugs, improve stability, and prioritize fixes." & Environment.NewLine & Environment.NewLine &
                "Diagnostic information may include:" & Environment.NewLine &
                "- Error messages and stack traces" & Environment.NewLine &
                "- Application version information" & Environment.NewLine &
                "- Operating system details" & Environment.NewLine &
                "- A randomly generated installation ID" & Environment.NewLine & Environment.NewLine &
                "No model data, personal files, or sensitive information are transmitted."

            Me.m_cbSendDiagnostics.AutoSize = True
            Me.m_cbSendDiagnostics.Dock = DockStyle.Top
            Me.m_cbSendDiagnostics.Text = "Send anonymous diagnostic information to the EwE development team"

            Dim installationIdLabel As New Label With {
                .AutoSize = True,
                .Text = "Installation ID:",
                .Margin = New Padding(3, 16, 3, 3)
            }
            Dim id As Guid? = EwE6ApplicationFramework.InstallationId
            Dim installationId As New TextBox With {
                .Dock = DockStyle.Top,
                .ReadOnly = True,
                .Text = If(id.HasValue, id.GetValueOrDefault().ToString("D"), String.Empty),
                .AccessibleName = "Installation ID"
            }

            Dim layout As New TableLayoutPanel With {
                .AutoScroll = True,
                .ColumnCount = 1,
                .Dock = DockStyle.Fill,
                .RowCount = 5
            }
            layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
            layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            layout.Controls.Add(title, 0, 0)
            layout.Controls.Add(Me.m_description, 0, 1)
            layout.Controls.Add(Me.m_cbSendDiagnostics, 0, 2)
            layout.Controls.Add(installationIdLabel, 0, 3)
            layout.Controls.Add(installationId, 0, 4)
            Me.Controls.Add(layout)
        End Sub

        Public Event OnChanged(sender As IOptionsPage, args As EventArgs) Implements IOptionsPage.OnChanged

        Public Function CanApply() As Boolean Implements IOptionsPage.CanApply
            Return True
        End Function

        Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply
            My.Settings.SendAnonymousDiagnostics = Me.m_cbSendDiagnostics.Checked
            Return IOptionsPage.eApplyResultType.Success
        End Function

        Public Sub SetDefaults() Implements IOptionsPage.SetDefaults
            Me.m_cbSendDiagnostics.Checked = CBool(My.Settings.GetDefaultValue("SendAnonymousDiagnostics"))
        End Sub

        Public Function CanSetDefaults() As Boolean Implements IOptionsPage.CanSetDefaults
            Return True
        End Function

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            Me.m_cbSendDiagnostics.Checked = My.Settings.SendAnonymousDiagnostics
        End Sub
    End Class

End Namespace
