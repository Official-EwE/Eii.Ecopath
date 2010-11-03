#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class ucOptionsPluginDetails
    Implements IUIElement

    Private m_pa As cPluginAssembly = Nothing
    Private m_uic As cUIContext = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Plug-in settings interface for
    ''' showing details on a plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext, _
                   ByVal pi As IPlugin, _
                   ByVal pa As cPluginAssembly)

        Me.InitializeComponent()

        ' Sanity checks
        Debug.Assert(uic IsNot Nothing)

        Me.UIContext = uic

        ' Name plug-ins by rich text if possible
        If (TypeOf pi Is IGUIPlugin) Then
            Me.m_tbName.Text = DirectCast(pi, IGUIPlugin).ControlText
        Else
            Me.m_tbName.Text = pi.Name
        End If
        Me.m_tbAuthor.Text = pi.Author
        Me.m_llContact.Text = pi.Contact
        Me.m_llContact.Links(0).LinkData = pi.Contact
        Me.m_tbDescription.Text = pi.Description

        Me.m_pa = pa

    End Sub

    Public Property UIContext() As cUIContext _
        Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Protected Set(ByVal uic As cUIContext)
            Me.m_uic = uic
        End Set
    End Property

    Private Sub m_llContact_LinkClicked(ByVal sender As System.Object, ByVal e As LinkLabelLinkClickedEventArgs) _
        Handles m_llContact.LinkClicked

        Try
            Dim strLink As String = e.Link.LinkData.ToString()

            If cStringUtils.IsValidEmail(strLink) Then
                If Not cStringUtils.BeginsWith(strLink, "mailto:") Then
                    strLink = "mailto:" & strLink
                End If
            End If

            System.Diagnostics.Process.Start(strLink)

        Catch ex As Exception

            Dim msg As New cMessage(ex.Message, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)

            Me.UIContext.Core.Messages.SendMessage(msg)

        End Try

    End Sub

End Class
