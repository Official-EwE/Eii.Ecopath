' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).

#If NET48 Then

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Thrown when the external Eii.Ecopath.Storage.Migrator.exe tool fails
    ''' to apply pending EF Core migrations to a SQLite database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSqliteMigratorException
        Inherits Exception

        ''' <summary>The process exit code, or -1 if the process could not be started at all.</summary>
        Public ReadOnly Property ExitCode As Integer

        Public Sub New(strMessage As String, Optional iExitCode As Integer = -1, Optional innerException As Exception = Nothing)
            MyBase.New(strMessage, innerException)
            Me.ExitCode = iExitCode
        End Sub

    End Class

End Namespace

#End If
