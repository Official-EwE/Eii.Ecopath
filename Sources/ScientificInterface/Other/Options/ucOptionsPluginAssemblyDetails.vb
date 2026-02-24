' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On
Imports EwEUtils.Utilities

Public Class ucOptionsPluginAssemblyDetails

    Private m_pa As cPluginAssembly = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Plug-in settings interface for
    ''' showing details on a plug-in assembly.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(pa As cPluginAssembly)

        Me.InitializeComponent()

        Me.m_tbCompany.Text = pa.Company
        Me.m_tbCopyright.Text = pa.Copyright
        Me.m_tbFile.Text = pa.Filename
        Me.m_tbVersion.Text = pa.Version
        Me.m_lbLicense.Visible = pa.IsLicensed
        Me.m_tbxLicense.Visible = pa.IsLicensed

        Dim dtStart As DateTime = cDateUtils.StartTime
        Dim dtExp As DateTime = pa.Expiry
        If (dtStart > dtExp) Then
            Me.m_tbxLicense.Text = My.Resources.PLUGIN_LICENSE_INVALID
        Else
            Me.m_tbxLicense.Text = cStringUtils.Localize(My.Resources.PLUGIN_LICENSE_EXPIRATION, pa.Expiry.ToShortDateString())
        End If
        Me.m_tbDescription.Text = pa.Description

        Me.m_pa = pa

    End Sub

End Class
