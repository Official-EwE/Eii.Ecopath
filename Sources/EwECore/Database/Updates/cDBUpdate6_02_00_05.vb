Option Strict On
Imports EwEUtils.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.2.0.05:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed Capacity driver PK</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_05
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
            Return 6.120005!
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
            Return "Fixed PK in Capacity driver map storage; needs to include shape"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Dim bOkidoki As Boolean = db.Execute("ALTER TABLE EcospaceScenarioCapacitDrivers DROP CONSTRAINT " & db.GetPkKeyName("EcospaceScenarioCapacitDrivers"))
        Return bOkidoki And db.Execute("ALTER TABLE EcospaceScenarioCapacitDrivers ADD PRIMARY KEY (ScenarioID, GroupID, VarName, VarDBID, ShapeID)")
    End Function


End Class
