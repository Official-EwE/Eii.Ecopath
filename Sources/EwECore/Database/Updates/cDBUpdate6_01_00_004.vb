Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.0.004:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecopath taxonomy support.</description></item>
''' <item><description>Added Ecopath model area support.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_00_004
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
            Return 6.100004!
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
            Return "Added Ecopath taxonomy support"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = Me.DiscardSpeciesTable(db)
        bSucces = bSucces And Me.CreateTaxonTable(db)
        bSucces = bSucces And Me.AddModelAreaName(db)

        Return bSucces

    End Function

    Private Function DiscardSpeciesTable(ByVal db As cEwEDatabase) As Boolean

        Return db.Execute("DROP TABLE SPECIES")

    End Function

    Private Function CreateTaxonTable(ByVal db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        bSucces = bSucces And db.Execute("CREATE TABLE EcopathGroupTaxon (TaxonID LONG, EcopathGroupID LONG, CodeISCAAP TEXT(3), CodeTaxon TEXT(14), Code3A TEXT(4), ClassName TEXT(50), OrderName TEXT(50), FamilyName TEXT(50), GenusName TEXT(50), SpeciesName TEXT(50), CommonName TEXT(50), Proportion SINGLE, SourceName TEXT(50), SourceKey MEMO, LastUpdated SINGLE)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathGroupTaxon ADD CONSTRAINT PK_INDEX PRIMARY KEY (TaxonID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathGroupTaxon ADD FOREIGN KEY (EcopathGroupID) REFERENCES EcopathGroup(GroupID)")

        Return bSucces

    End Function

    Private Function AddModelAreaName(ByVal db As cEwEDatabase) As Boolean

        Return db.Execute("ALTER TABLE EcopathModel ADD COLUMN AreaName TEXT(255)")

    End Function

End Class
