' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Interface for implementing database compact capabilities.
    ''' </summary>
    ''' =======================================================================
    Public Interface IDatabaseCompact

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the OS supports compacting of databases of the type
        ''' that this engine was created for.
        ''' </summary>
        ''' <returns>True if the OS supports compacting of a database.</returns>
        ''' -------------------------------------------------------------------
        Function CanCompact() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact a database.
        ''' </summary>
        ''' <param name="strFileFrom">Source database location.</param>
        ''' <param name="strConnectionFrom">Source database connection string.</param>
        ''' <param name="strFileTo">Target database location.</param>
        ''' <param name="strConnectionTo">Target database connection string.</param>
        ''' <returns>A <see cref="eDatasourceAccessType">database access
        ''' result code</see>.</returns>
        ''' -------------------------------------------------------------------
        Function Compact(strFileFrom As String,
                         strConnectionFrom As String,
                         strFileTo As String,
                         strConnectionTo As String) As eDatasourceAccessType

    End Interface

End Namespace ' Database
