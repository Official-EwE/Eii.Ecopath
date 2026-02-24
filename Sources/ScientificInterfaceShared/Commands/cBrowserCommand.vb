' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to launch a hyperlink, which can be either a url or a file/folder path.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cBrowserCommand
        Inherits cCommand

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' <example>
        ''' The folowing shows how to retrieve the one instance of the cBrowserCommand
        ''' from anywhere in the EwE6 user interface:
        ''' <code>
        ''' ' Get the one and only command 
        ''' Dim cdmH As CommandHandler = CommandHandler.GetInstance()
        ''' ' Get the one and only navigation command
        ''' Dim cmd As cBrowserCommand = DirectCast(GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        ''' ' Invoke the command
        ''' ...
        ''' </code>
        ''' </example>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~browse"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the <see cref="cBrowserCommand"/> class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invokes the command to make the EwE6 GUI navigate to user interface
        ''' element defined by this call.
        ''' </summary>
        ''' <param name="strURL">URL to navigate to.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(strURL As String)
            Me.Parameter("URL") = strURL
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invokes the command to make the EwE6 GUI navigate to user interface
        ''' element defined by this call.
        ''' </summary>
        ''' <param name="link">Symbolic <see cref="cWebLinks.eLinkType"/> to navigate to.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(link As cWebLinks.eLinkType)
            Me.Parameter("Weblink") = link
            MyBase.Invoke()
        End Sub

        ''' <summary>
        ''' Get the URL to navigate to.
        ''' </summary>
        Public ReadOnly Property URL(decoder As cWebLinks) As String
            Get
                Dim strURL As String = CStr(Me.Parameter("URL"))
                If (String.IsNullOrWhiteSpace(strURL)) Then
                    Dim link As Object = Me.Parameter("Weblink")
                    If (link Is Nothing) Then link = cWebLinks.eLinkType.NotSet
                    strURL = decoder.GetURL(DirectCast(link, cWebLinks.eLinkType))
                End If
                Return strURL
            End Get
        End Property

    End Class

End Namespace
