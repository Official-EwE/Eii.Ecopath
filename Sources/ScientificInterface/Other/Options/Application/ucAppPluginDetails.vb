'==============================================================================
'
' $Log: ucAppPluginDetails.vb,v $
' Revision 1.1  2008/09/26 07:32:09  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/07/29 23:46:27  jeroens
' Plug-in detail page also has option to disable assembly
' Synchronized layout between plug-in detail pages
'
' Revision 1.2  2008/07/16 13:22:53  jeroens
' Even more pretty
'
' Revision 1.1  2008/07/15 20:22:31  jeroens
' Initial version
'
'==============================================================================

#Region " Imports Directive "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEPlugin

#End Region ' Imports Directive

Public Class ucAppPluginDetails

    Private m_pa As cPluginAssembly = Nothing

    Public Sub New(ByVal pi As IPlugin, ByVal pa As cPluginAssembly)

        Me.InitializeComponent()

        Me.m_tbAuthor.Text = pi.Author
        Me.m_tbContact.Text = pi.Contact
        Me.m_tbDescription.Text = pi.Description
        Me.m_tbName.Text = pi.Name

        Me.m_pa = pa
        Me.m_cbEnabled.Checked = pa.Enabled
        Me.m_cbEnabled.Enabled = (pa.AlwaysEnabled = False)

    End Sub

    Private Sub OnCheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cbEnabled.CheckedChanged
        Me.m_pa.Enabled = Me.m_cbEnabled.Checked
    End Sub

End Class
