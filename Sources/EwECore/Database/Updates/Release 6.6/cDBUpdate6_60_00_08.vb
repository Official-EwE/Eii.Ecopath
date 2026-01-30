' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database



''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.60.0.08:</para>
''' <para>
''' Added catchability table.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_60_00_08
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.600008!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added Ecosim catchability forcing"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True

        ' Add Ecosim fleet x group shape table
        bSuccess = bSuccess And db.Execute("CREATE TABLE EcosimScenarioFleetGroupCatchability (ScenarioID LONG, GroupID LONG, FleetID LONG, zScale LONGTEXT)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioFleetGroupCatchability ADD PRIMARY KEY (ScenarioID, GroupID, FleetID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioFleetGroupCatchability ADD FOREIGN KEY (ScenarioID) REFERENCES EcosimScenario(ScenarioID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioFleetGroupCatchability ADD FOREIGN KEY (GroupID) REFERENCES EcosimScenarioGroup(GroupID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioFleetGroupCatchability ADD FOREIGN KEY (FleetID) REFERENCES EcosimScenarioFleet(FleetID)")

        Return bSuccess

    End Function


End Class
