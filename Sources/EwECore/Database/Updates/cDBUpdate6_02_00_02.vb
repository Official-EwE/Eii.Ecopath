Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.2.0.02:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Max time shape length will be remembered in the EwE model
''' since this setting is Ecosim scenario independent.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_02
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
            Return 6.120002!
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
            Return "Max forcing time remembered in model" & vbNewLine & _
                   "Added capacity map tables"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.UpdateForcePoints(db) And _
               Me.AddCapacityMapTable(db) And _
               Me.AddCapacityMapAssignmentTable(db)
    End Function

    Private Function UpdateForcePoints(ByVal db As cEwEDatabase) As Boolean

        Dim iForcePoints As Integer = CInt(Math.Ceiling(cEcosimDatastructures.DEFAULT_N_FORCINGPOINTS / cCore.N_MONTHS))
        Dim readerScenario As IDataReader = Nothing
        Dim bSuccess As Boolean = True

        ' Read ecosim run length
        bSuccess = bSuccess And db.Execute("CREATE TABLE EcosimModel (ModelID LONG, ForcePoints LONG)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimModel ADD PRIMARY KEY (ModelID)")
        Me.LogProgress("CREATE TABLE EcosimModel", bSuccess)

        readerScenario = db.GetReader("SELECT TotalTime FROM EcosimScenario")
        Try
            While readerScenario.Read()
                iForcePoints = Math.Max(CInt(readerScenario(0)), iForcePoints) * cCore.N_MONTHS
            End While
        Catch ex As Exception
            bSuccess = False
        End Try
        db.ReleaseReader(readerScenario)

        ' Write max forcing time
        bSuccess = bSuccess And db.Execute("INSERT INTO EcosimModel ( ModelID, ForcePoints ) VALUES (1, " & iForcePoints & ")")

        Me.LogProgress("UPDATE TABLE EcosimModel", bSuccess)
        Return bSuccess

    End Function

    Private Function AddCapacityMapTable(ByVal db As cEwEDatabase) As Boolean
        Dim bSuccess As Boolean = True

        ' Read ecosim run length
        bSuccess = bSuccess And db.Execute("CREATE TABLE EcospaceScenarioCapacityMap (ScenarioID LONG, MapID LONG, Sequence LONG, MapName TEXT(50), VarName TEXT(50))")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityMap ADD PRIMARY KEY (ScenarioID, MapID)")
        bSuccess = bSuccess And db.Execute("CREATE UNIQUE INDEX idMaps ON EcospaceScenarioCapacityMap(MapID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityMap ADD FOREIGN KEY (ScenarioID) REFERENCES EcospaceScenario(ScenarioID)")

        Me.LogProgress("ADD TABLE EcospaceScenarioCapacityMap", bSuccess)
        Return bSuccess

    End Function

    Private Function AddCapacityMapAssignmentTable(ByVal db As cEwEDatabase) As Boolean
        Dim bSuccess As Boolean = True

        ' Read ecosim run length
        bSuccess = bSuccess And db.Execute("CREATE TABLE EcospaceScenarioCapacityMapAssignments (MapID LONG, GroupID LONG, ShapeID LONG)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityMapAssignments ADD PRIMARY KEY (MapID, GroupID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityMapAssignments ADD FOREIGN KEY (MapID) REFERENCES EcospaceScenarioCapacityMap(MapID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityMapAssignments ADD FOREIGN KEY (GroupID) REFERENCES EcopathGroup(GroupID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityMapAssignments ADD FOREIGN KEY (ShapeID) REFERENCES EcosimShape(ShapeID)")

        Me.LogProgress("ADD TABLE EcospaceScenarioCapacityMapAssignments", bSuccess)
        Return bSuccess

    End Function

End Class
