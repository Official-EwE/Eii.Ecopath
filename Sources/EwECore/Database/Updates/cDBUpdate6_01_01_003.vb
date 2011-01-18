Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.1.003:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Increased size of author columns.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_01_003
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
            Return 6.101003!
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
            Return "Increased size of author columns"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        bSucces = db.Execute("ALTER TABLE EcopathModel ALTER COLUMN Author TEXT(255)")
        bSucces = db.Execute("ALTER TABLE EcosimScenario ALTER COLUMN Author TEXT(255)")
        bSucces = db.Execute("ALTER TABLE EcospaceScenario ALTER COLUMN Author TEXT(255)")
        bSucces = db.Execute("ALTER TABLE EcotracerScenario ALTER COLUMN Author TEXT(255)")
        Return bSucces

    End Function

End Class
