'==============================================================================
'
' $Log: ucAppPluginAssemblyDetails.vb,v $
' Revision 1.7  2009/04/01 17:38:14  jeroens
' Separated Enabled state and Incompatibility
'
' Revision 1.6  2009/03/31 16:13:47  jeroens
' Conflicts now clearly shown
' Conflicting assemblies cannot be loaded anymore
'
' Revision 1.5  2008/12/15 15:56:02  jeroens
' no message
'
' Revision 1.4  2008/12/07 20:50:53  jeroens
' Incompatible plug-ins can be activated
'
' Revision 1.3  2008/12/03 02:40:54  jeroens
' Added levels of plugin compatibility
'
' Revision 1.2  2008/11/28 02:43:26  jeroens
' Added plugin compatibility checks to prevent the system from dying
'
' Revision 1.1  2008/09/26 07:32:09  sherman
' --== DELETED HISTORY ==-- Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEPlugin

#End Region ' Imports

Public Class ucAppPluginAssemblyDetails

    Private m_pa As cPluginAssembly = Nothing

    Public Sub New(ByVal pa As cPluginAssembly)

        Me.InitializeComponent()

        Me.m_tbCompany.Text = pa.Company
        Me.m_tbCopyright.Text = pa.Copyright
        Me.m_tbDescription.Text = pa.Description
        Me.m_tbFile.Text = pa.Filename
        Me.m_tbVersion.Text = pa.Version

        Me.m_pa = pa
        Me.m_cbEnabled.Checked = pa.Enabled
        Me.m_cbEnabled.Enabled = (pa.AlwaysEnabled = False)

    End Sub

    Private Sub OnCheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_cbEnabled.CheckedChanged
        Me.m_pa.Enabled = Me.m_cbEnabled.Checked
    End Sub

End Class
