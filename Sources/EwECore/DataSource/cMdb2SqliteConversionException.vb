' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).

#If NET48 Then

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Thrown when the external mdb2sqlite.exe tool fails to convert an
    ''' Access database to SQLite.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cMdb2SqliteConversionException
        Inherits Exception

        ''' <summary>The process exit code, or -1 if the process could not be started at all.</summary>
        Public ReadOnly Property ExitCode As Integer

        Public Sub New(strMessage As String, Optional iExitCode As Integer = -1, Optional innerException As Exception = Nothing)
            MyBase.New(strMessage, innerException)
            Me.ExitCode = iExitCode
        End Sub

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Thrown specifically when mdb2sqlite.exe could not download mdbtools-win
    ''' due to a network failure (exit code 3 - see mdb2sqlite.ps1's own
    ''' documented exit code convention). Only occurs on a machine's first-ever
    ''' conversion, since subsequent runs reuse the cached mdbtools-win folder.
    ''' Lets callers show a specific "internet connection required" message
    ''' without checking a magic exit code number themselves.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cMdb2SqliteNetworkException
        Inherits cMdb2SqliteConversionException

        Public Sub New(strMessage As String, iExitCode As Integer)
            MyBase.New(strMessage, iExitCode)
        End Sub

    End Class

End Namespace

#End If
