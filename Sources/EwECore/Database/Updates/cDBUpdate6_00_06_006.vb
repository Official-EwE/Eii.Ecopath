Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.6.006:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecosim temperature response fields.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_06_006
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
            Return 6.06006!
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
            Return "Added Ecosim temperature fields."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean
        Return Me.AddEcosimTempFields(db)
    End Function

    Private Function AddEcosimTempFields(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenario ADD COLUMN TemperatureForcingShapeID LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN TempOpt SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN TempLeft SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroup ADD COLUMN TempRight SINGLE")

        Return bSucces

    End Function
End Class
