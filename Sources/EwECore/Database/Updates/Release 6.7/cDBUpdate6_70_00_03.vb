' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.03:</para>
''' <para>
''' Added Ecosim shared arena sequence.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_03
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700003!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added Ecosim shared arena sequence"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add arena share sequential no
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return db.Execute("DELETE * FROM EcosimScenarioArena") And db.Execute("ALTER TABLE EcosimScenarioArena ADD COLUMN Sequence INTEGER")

    End Function

End Class
