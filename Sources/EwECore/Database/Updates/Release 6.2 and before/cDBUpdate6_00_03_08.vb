Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.3.8:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added last saved date to model and scenarios.</description></item>
''' <item><description>Added quotes table.</description></item>
''' <item><description>Fixed Ecospace group defaults.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_00_03_08
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

        ' - All scenarios, model
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathModel ADD COLUMN LastSaved SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimScenario ADD COLUMN LastSaved SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN LastSaved SINGLE")
        bSucces = bSucces And db.Execute("ALTER TABLE EcotracerScenario ADD COLUMN LastSaved SINGLE")

        ' - Quotes
        bSucces = bSucces And db.Execute("CREATE TABLE Quotes (ID COUNTER CONSTRAINT PrimaryKey PRIMARY KEY, Quote MEMO, Source TEXT(255))")

        ' - Fix Ecospace defaults
        bSucces = bSucces And db.Execute("UPDATE EcospaceScenario SET Tolerance=0.01 WHERE Tolerance=0.0001")
        bSucces = bSucces And db.Execute("UPDATE EcospaceScenarioGroup SET EatEffBad=0.5 WHERE EatEffBad=0.001")

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
            Return "Adds last saved date to model and scenarios" + vbNewLine + "Adds quotes table" + vbNewLine + "Fixes Ecospace group defaults"
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
            Return 6.038!
        End Get
    End Property

End Class
