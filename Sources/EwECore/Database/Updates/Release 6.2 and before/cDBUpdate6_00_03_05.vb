option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.3.5:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Fixed default weight for imported time series</description></item>
''' <item><description>Added Species table</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_00_03_05
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
        ' - Correct WtType 0 -> 1 for incorrectly imported time series
        bSucces = bSucces And db.Execute("UPDATE EcosimTimeSeries SET WtType=1 WHERE WtType=0")
        ' - Add species table
        bSucces = bSucces And db.Execute("CREATE TABLE Species (SpeciesID INTEGER, EcopathGroupID INTEGER, FishbaseSpeciesID INTEGER, GroupName TEXT(50), GenusName TEXT(50), SpeciesName TEXT(50), Proportion Single)")
        bSucces = bSucces And db.Execute("ALTER TABLE Species ADD PRIMARY KEY (SpeciesID)")
        bSucces = bSucces And db.Execute("ALTER TABLE Species ADD FOREIGN KEY (SpeciesID) REFERENCES EcopathGroup(GroupID)")

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
            Return "Fixesdefault weight for imported time series" + vbNewLine + "Adds Species table"
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
            Return 6.035!
        End Get
    End Property

End Class
