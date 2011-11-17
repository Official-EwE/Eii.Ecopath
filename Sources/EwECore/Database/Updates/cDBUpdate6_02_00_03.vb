Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.2.0.03:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added EwE version to updatelog.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_03
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
            Return 6.120003!
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
            Return "Added ewe version to updatelog, scenarios"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.AddLastSavedEwEVersions(db)
    End Function

    Private Function AddLastSavedEwEVersions(ByVal db As cEwEDatabase) As Boolean
        Dim bSucces As Boolean = db.Execute("ALTER TABLE EcopathModel ADD COLUMN LastSavedVersion TEXT(40)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenario ADD COLUMN LastSavedVersion TEXT(40)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN LastSavedVersion TEXT(40)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcotracerScenario ADD COLUMN LastSavedVersion TEXT(40)")
        Return bSucces
    End Function


End Class
