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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




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
