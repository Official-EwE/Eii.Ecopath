' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.25:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecopath sample tables</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_25
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500025!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added Ecospace migration area movement, Ecosim environmental driver table"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.AddMigField(db) And
               Me.AddEcosimDriverTable(db)

    End Function

    Private Function AddMigField(db As cEwEDatabase) As Boolean
        Return db.Execute("ALTER TABLE EcospaceScenarioGroup ADD COLUMN InMigAreaMovement Single")
    End Function

    Private Function AddEcosimDriverTable(db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = db.Execute("CREATE TABLE EcosimScenarioCapacityDrivers (ScenarioID LONG, GroupID LONG, DriverID LONG, ResponseID LONG)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD CONSTRAINT pk PRIMARY KEY (ScenarioID, GroupID, DriverID, ResponseID)")

        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD FOREIGN KEY (ScenarioID) REFERENCES EcosimScenario (ScenarioID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD FOREIGN KEY (GroupID) REFERENCES EcosimScenarioGroup (GroupID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD FOREIGN KEY (DriverID) REFERENCES EcosimShape (ShapeID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD FOREIGN KEY (ResponseID) REFERENCES EcosimShape (ShapeID)")

        Return bSuccess

    End Function

End Class
