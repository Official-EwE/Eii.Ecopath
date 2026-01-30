' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database



''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.13:</para>
''' <para>
''' Added effort zones and cell area maps.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_13
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700013!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added effort zones and cell area maps"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Added map to record cell area for correctly producing Ecospace plots.
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns>True if update succeeded.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN CellAreaMap MEMO") And
               db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN NumEffortZones INTEGER") And
               db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN EffortZoneMap MEMO")
    End Function

End Class
