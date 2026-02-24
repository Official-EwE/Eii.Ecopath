' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.09:</para>
''' <para>
''' Fixed potential ecosampler storage problem - reissued, because the on-board
''' database templates had the issue again.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_09
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700009!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed potential ecosampler saving problem - reprise"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' At some pont, the on-board EwE database templates received an erroneous
    ''' index on a value column. This update removes the index if it exists.
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns>Always true</returns>
    ''' <remarks>This update is re-issued because the index error returned in the
    ''' on-board database templates, thus re-instating the error. Good lord.</remarks>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Try
            Dim name As String = db.GetIndexName("EcopathSample", "Hash")
            If (Not String.IsNullOrWhiteSpace(name)) Then
                ' Index name bracketed, just in case of conflicts with reserved words
                Return db.Execute("DROP INDEX [" & name & "] ON EcopathSample")
            End If
        Catch ex As Exception
            ' Life is fantastic. Carry on.
        End Try
        Return True

    End Function

End Class
