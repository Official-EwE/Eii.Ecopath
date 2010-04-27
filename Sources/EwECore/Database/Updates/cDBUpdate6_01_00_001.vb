Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports System.Data
Imports System.Data.OleDb
Imports EwEUtils.Utilities
Imports EwECore.DataSources.cDBDataSource

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.0.000:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added more MSE fields.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_00_001
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
            Return 6.100001!
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
            Return "Added more MSE fields."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.UpdateMSETables(db)

    End Function

    Private Function UpdateMSETables(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN KalmanGain SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN RStockRatio SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN RHalfB0Ratio SINGLE")

        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioMSE DROP COLUMN KalmanGain")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioMSE DROP COLUMN ForcastGain")

        Return bSucces

    End Function

End Class
