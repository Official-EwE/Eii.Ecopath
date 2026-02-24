' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.1.011:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added landings mediation tables</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_000
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the update version number that will be entered in
    ''' the update log of the database. This version number is also used to check
    ''' whether an update should run.
    ''' </summary>
    ''' <remarks>
    ''' If <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> is provided, the
    ''' update is ran regardless of version number.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.12!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added Ecospace stanza, habitat preference and capacity fields"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.AddStanzaFields(db) And
               Me.AddHabitatPreference(db) And
               Me.AddGroupMap(db)

    End Function

    Private Function AddStanzaFields(db As cEwEDatabase) As Boolean

        db.Execute("ALTER TABLE Stanza ADD COLUMN EggAtSpawn BYTE")
        Return True

    End Function

    Private Function AddHabitatPreference(db As cEwEDatabase) As Boolean

        db.Execute("ALTER TABLE EcospaceScenarioGroupHabitat ADD COLUMN Preference SINGLE")
        db.Execute("UPDATE EcospaceScenarioGroupHabitat SET Preference=1 WHERE HabitatID > 0")
        Return True

    End Function

    Private Function AddGroupMap(db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True
        bSucces = bSucces And db.Execute("CREATE TABLE EcospaceScenarioGroupMap (ScenarioID LONG, GroupID LONG, InRow INTEGER, InCol INTEGER, Capacity SINGLE)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioGroupMap ADD PRIMARY KEY (ScenarioID, GroupID, InRow, InCol)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioGroupMap ADD FOREIGN KEY (ScenarioID) REFERENCES EcospaceScenario(ScenarioID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenarioGroupMap ADD FOREIGN KEY (GroupID) REFERENCES EcospaceScenarioGroup(GroupID)")
        Me.LogProgress("AddGroupMap", bSucces)
        Return bSucces

    End Function

End Class
