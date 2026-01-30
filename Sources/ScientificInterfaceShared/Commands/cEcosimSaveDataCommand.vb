' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Ecosim



Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' The cEcosimSaveDataCommand class implements a <see cref="cCommand">Command</see>
    ''' that is used in EwE6 to save Ecosim data to csv file.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEcosimSaveDataCommand
        Inherits cCommand

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' <example>
        ''' The folowing shows how to retrieve the one instance of the cEcosimSaveDataCommand
        ''' from anywhere in the EwE6 user interface:
        ''' <code>
        ''' ' Get the one and only command 
        ''' Dim cdmH As cCommandHandler = cCommandHandler.GetInstance()
        ''' ' Get the one and only ecosim save data command
        ''' Dim cmd As cEcosimSaveDataCommand = DirectCast(GetCommand(cEcosimSaveDataCommand.COMMAND_NAME), cEcosimSaveDataCommand)
        ''' ' Invoke the command
        ''' ...
        ''' </code>
        ''' </example>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "ExportEcosimResultsToCSV"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="cmdh">The <see cref="cCommandHandler"/> to associate this command with.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' <param name="results">Optional array of results to output.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(Optional results As cEcosimResultWriter.eResultTypes() = Nothing)
            Me.Results = results
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' The array of results to output, ordered when the command was 
        ''' <see cref="Invoke">invoked</see>.
        ''' </summary>
        ''' <remarks>
        ''' Note that the value returned here is only valid while the command is invoking.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Property Results() As cEcosimResultWriter.eResultTypes()
            Get
                Return DirectCast(Me.Parameter("Results"), cEcosimResultWriter.eResultTypes())
            End Get
            Private Set(value As cEcosimResultWriter.eResultTypes())
                Me.Parameter("Results") = value
            End Set
        End Property

    End Class

End Namespace
