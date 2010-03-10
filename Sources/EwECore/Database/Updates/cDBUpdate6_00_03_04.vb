Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.3.4:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Author and Contact information to model, scenarios</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_00_03_04
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The actual update logic.
    ''' </summary>
    ''' <param name="db">Database to modify.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        ' Update(s):
        ' - Ecopath model requires author information
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN Author TEXT(64)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN Contact TEXT(255)")
        ' - Ecosim model requires author information
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenario ADD COLUMN Author TEXT(64)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenario ADD COLUMN Contact TEXT(255)")
        ' - Ecospace model requires author information
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN Author TEXT(64)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN Contact TEXT(255)")

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Adds Author and Contact information to model and scenarios"
        End Get
    End Property

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
            Return 6.034!
        End Get
    End Property

End Class
