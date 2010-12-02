Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.0.009:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed MSY group and fleet year constraints.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_00_009
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
            Return 6.100009!
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
            Return "Fixed MSY group and fleet constraints"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        If db.Execute("ALTER TABLE EcosimScenarioGroupYear DROP CONSTRAINT SimSGTGroup") Then
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioGroupYear ADD CONSTRAINT SimSGTGroup FOREIGN KEY (GroupID) REFERENCES EcosimScenarioGroup (GroupID)")
        End If

        If db.Execute("ALTER TABLE EcosimScenarioFleetYear DROP CONSTRAINT SimSFTFleet") Then
            bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenarioFleetYear ADD CONSTRAINT SimSFTFleet FOREIGN KEY (FleetID) REFERENCES EcosimScenarioFleet (FleetID)")
        End If

        Return bSucces

    End Function

End Class
