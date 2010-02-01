Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.7.001:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Patched default Fleet colors.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_07_001
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
            Return 6.07001!
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
            Return "Set default fleet colours."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean
        Return Me.SetDefaultFleetColours(db) And _
               Me.AddMSE(db)
    End Function

    Private Function SetDefaultFleetColours(ByVal db As cEwEDatabase) As Boolean
        db.Execute("UPDATE EcopathFleet SET PoolColor='0' WHERE PoolColor='FF000000'")
        Return True
    End Function

    Private Function AddMSE(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        ' Create MSE parameteres table
        bSucces = bSucces And db.Execute("CREATE TABLE EcosimScenarioMSE (ScenarioID LONG, AssessMethod INTEGER, AssessPower SINGLE, KalmanGain SINGLE, ForcastGain SINGLE, NTrials INTEGER, StartIndex INTEGER)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioMSE ADD CONSTRAINT PK_INDEX PRIMARY KEY (ScenarioID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioMSE ADD FOREIGN KEY (ScenarioID) REFERENCES EcosimScenario(ScenarioID)")

        ' Extend Ecosim groups
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN BiomassCV SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN LowerRisk SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN UpperRisk SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN BiomassRefLower SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN BiomassRefUpper SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN CatchRefLower SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN CatchRefUpper SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN FixedEscapement SINGLE")

        ' Extend Ecosim fleets
        bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN CV SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN QIncrease SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN CatchRefLower SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN CatchRefUpper SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN EffortRefLower SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN EffortRefUpper SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioFleet ADD COLUMN MSYEvaluateFleet SINGLE")

        ' Extend Ecosim Quota
        bSucces = bSucces And db.Execute("ALTER TABLE EcoSimScenarioQuota ADD COLUMN Fweight SINGLE")

        Return bSucces

    End Function

End Class
