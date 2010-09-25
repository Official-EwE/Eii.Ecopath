Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.0.007:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added pedigree colour.</description></item>
''' <item><description>Changed pedigree storage location.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_00_007
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
            Return 6.100007!
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
            Return "Added pedigree level colours" & vbNewLine & "Changed pedigree storage location"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.AddPedigreeColor(db) And ChangePedigreeStorage(db)

    End Function

    Private Function AddPedigreeColor(ByVal db As cEwEDatabase) As Boolean

        ' No need to set defaults; an integer of 0 will mean a 100% transparent colour,
        ' which is the indicator for any GIU to use a default colour for pedigree.
        ' This is identical to the colour behaviour of groups and fleets
        Return db.Execute("ALTER TABLE Pedigree ADD COLUMN LevelColor LONG")

    End Function

    Private Function ChangePedigreeStorage(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True
        bSucces = bSucces And db.Execute("CREATE TABLE EcopathGroupPedigree (GroupID LONG, VarName TEXT(50), LevelID LONG)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathGroupPedigree ADD PRIMARY KEY (GroupID, VarName)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathGroupPedigree ADD FOREIGN KEY (GroupID) REFERENCES EcopathGroup(GroupID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathGroupPedigree ADD FOREIGN KEY (LevelID) REFERENCES Pedigree(LevelID)")
        'bSucces = bSucces And db.Execute("ALTER TABLE Auxillary DROP COLUMN PedigreeLevelID")
        Return bSucces

    End Function

End Class
