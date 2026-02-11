' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.01:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added model exclusion layer</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_01
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500001!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Validated exclusion maps"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Try
            ' The earlier on-board databases did not contain this table. Just add it again.
            db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN ExclusionMap MEMO")
        Catch ex As Exception
            ' Some debugging databases already have this. Do not fail, just move on
        End Try
        Me.LogProgress("UpdateEcospaceTables", True)
        Return True

    End Function

End Class
