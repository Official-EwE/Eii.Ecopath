#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports System.Xml
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class ucAppPluginDetails

    Private m_pa As cPluginAssembly = Nothing

    Public Sub New(ByVal pi As IPlugin, ByVal pa As cPluginAssembly)

        Me.InitializeComponent()

        Me.m_tbName.Text = pi.Name
        Me.m_tbAuthor.Text = pi.Author
        Me.m_llContact.Text = pi.Contact
        Me.m_llContact.Links(0).LinkData = pi.Contact

        Dim strDescription As String = pi.Description
        If (TypeOf pi Is cDBUpdate) Then
            ' ToDo: use XSLT formatting here
            strDescription = DirectCast(pi, cDBUpdate).UpdateDescriptionXML.InnerText
        ElseIf (TypeOf pi Is IDatabaseUpdatePlugin) Then
            strDescription = DirectCast(pi, IDatabaseUpdatePlugin).Description
        Else
            strDescription = pi.Description
        End If
        Me.m_tbDescription.Text = strDescription

        Me.m_pa = pa
        Me.m_cbEnabled.Checked = pa.Enabled
        Me.m_cbEnabled.Enabled = (pa.AlwaysEnabled = False)

    End Sub

    Private Sub OnCheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_cbEnabled.CheckedChanged

        Me.m_pa.Enabled = Me.m_cbEnabled.Checked

    End Sub

    Private Sub m_llContact_LinkClicked(ByVal sender As System.Object, ByVal e As LinkLabelLinkClickedEventArgs) _
        Handles m_llContact.LinkClicked

        Try
            Dim strLink As String = e.Link.LinkData.ToString()

            If StringUtils.IsValidEmail(strLink) Then
                If Not StringUtils.BeginsWith(strLink, "mailto:") Then
                    strLink = "mailto:" & strLink
                End If
            End If

            System.Diagnostics.Process.Start(strLink)

        Catch ex As Exception

            Dim core As cCore = cCore.GetInstance()
            Dim msg As New cMessage(ex.Message, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)

            core.Messages.SendMessage(msg)

        End Try

    End Sub

End Class
