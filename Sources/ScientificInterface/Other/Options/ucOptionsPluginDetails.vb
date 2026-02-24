' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports EwECore.Plugins
Imports EwEUtils.Utilities

Public Class ucOptionsPluginDetails
    Implements IUIElement

    Private m_pa As cPluginAssembly = Nothing
    Private m_pi As IPlugin = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Plug-in settings interface for
    ''' showing details on a plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(uic As cUIContext,
                   pi As IPlugin,
                   pa As cPluginAssembly)

        Me.InitializeComponent()

        ' Sanity checks
        Debug.Assert(uic IsNot Nothing)

        Me.UIContext = uic

        Me.m_tbName.Text = cStringUtils.ControlTextToSentence(pi.DisplayName)
        Me.m_tbAuthor.Text = pi.Author
        Me.m_llContact.Text = pi.Contact
        Me.m_llContact.Links(0).LinkData = pi.Contact
        Me.m_tbDescription.Text = pi.Description

        Me.m_pi = pi
        Me.m_pa = pa

    End Sub

    Public Property UIContext() As cUIContext _
        Implements IUIElement.UIContext

    Private Sub m_llContact_LinkClicked(sender As System.Object, e As LinkLabelLinkClickedEventArgs) _
        Handles m_llContact.LinkClicked

        Try
            Dim strLink As String = e.Link.LinkData.ToString()

            If cStringUtils.IsEmail(strLink) Then
                If Not cStringUtils.BeginsWith(strLink, "mailto:") Then
                    strLink = "mailto:" & strLink
                End If
                If Not strLink.ToLower.Contains("?subject=") Then
                    strLink = strLink & "?subject=Question about " & Path.GetFileNameWithoutExtension(Me.m_pa.Filename)
                End If
            End If

            System.Diagnostics.Process.Start(strLink)

        Catch ex As Exception

            Dim msg As New cMessage(ex.Message, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)

            Me.UIContext.Core.Messages.SendMessage(msg)

        End Try

    End Sub

End Class
