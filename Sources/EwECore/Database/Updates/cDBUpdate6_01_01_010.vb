Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.1.010:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Cleaned up stanza</description></item>
''' </list>
''' </para>
''' </summary>
''' <remarks>
''' In case you wonder where updates 7, 8 and 9 went: these have been made
''' obsolete by earlier development updates, but since some versions of the
''' EwE with these updates have been released into the wild this update must be
''' able to undo the work of these discontinued updates. Lo siento.
''' </remarks>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_01_010
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
            Return 6.10101!
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
            Return "Added taxon stanza ID."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.CreateGenericTaxonTable(db) And _
               Me.CreateTaxonStanzaAssignmentTable(db) And _
               Me.CopyTaxonData(db) And _
               Me.CleanupEcopathGroupTaxon(db)

    End Function

    Private Function CreateGenericTaxonTable(ByVal db As cEwEDatabase) As Boolean
        Dim bSucces As Boolean = True
        bSucces = bSucces And db.Execute("CREATE TABLE EcopathTaxon (TaxonID LONG, CodeISCAAP TEXT(3), CodeTaxon TEXT(14), Code3A TEXT(4), ClassName TEXT(50), OrderName TEXT(50), FamilyName TEXT(50), GenusName TEXT(50), SpeciesName TEXT(50), CommonName TEXT(50), SourceName TEXT(50), SourceKey MEMO, LastUpdated FLOAT, EcologyType LONG, OrganismType LONG, Exploited BYTE, ConservationStatus LONG, OccurrenceStatus LONG, MeanWeight SINGLE, MeanLength SINGLE, MaxLength SINGLE, MeanLifeSpan SINGLE, VulnerabiltyIndex SHORT)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathTaxon ADD CONSTRAINT PK_INDEX PRIMARY KEY (TaxonID)")
        Return bSucces
    End Function

    Private Function CreateTaxonStanzaAssignmentTable(ByVal db As cEwEDatabase) As Boolean
        Dim bSucces As Boolean = True
        bSucces = bSucces And db.Execute("CREATE TABLE EcopathStanzaTaxon (TaxonID LONG, StanzaID LONG, Proportion SINGLE)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathStanzaTaxon ADD CONSTRAINT PK_INDEX PRIMARY KEY (TaxonID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathStanzaTaxon ADD CONSTRAINT FkTaxon FOREIGN KEY (TaxonID) REFERENCES EcopathTaxon(TaxonID)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcopathStanzaTaxon ADD CONSTRAINT FkStanza FOREIGN KEY (StanzaID) REFERENCES Stanza(StanzaID)")
        Return bSucces
    End Function

    Private Function CopyTaxonData(ByVal db As cEwEDatabase) As Boolean

        ' Copy the 'old' fields
        Dim astrColumns As String() = New String() {"TaxonID", "CodeISCAAP", "CodeTaxon", "Code3A", "ClassName", "OrderName", "FamilyName", "GenusName", "SpeciesName", "CommonName", "SourceName", "SourceKey", "LastUpdated"}

        Dim reader As IDataReader = db.GetReader("SELECT * FROM EcopathGroupTaxon")
        Dim writer As cEwEDatabase.cEwEDbWriter = db.GetWriter("EcopathTaxon")
        Dim drow As DataRow = Nothing

        While reader.Read
            drow = writer.NewRow()
            For Each strColumn As String In astrColumns
                drow(strColumn) = db.ReadSafe(reader, strColumn, Convert.DBNull)
            Next
            writer.AddRow(drow)
        End While

        db.ReleaseWriter(writer, True)
        db.ReleaseReader(reader)
        Return True

    End Function

    Private Function CleanupEcopathGroupTaxon(ByVal db As cEwEDatabase) As Boolean

        Dim astrColumns As String() = New String() {"CodeISCAAP", "CodeTaxon", "Code3A", "ClassName", "OrderName", "FamilyName", "GenusName", "SpeciesName", "CommonName", "SourceName", "SourceKey", "LastUpdated", "EcologyType", "OrganismType", "Exploited", "ConservationStatus", "OccurrenceStatus", "OccurenceStatus", "MeanWeight", "MeanLength", "MaxLength", "MeanLifeSpan", "VulnerabiltyIndex"}
        Dim bSucces As Boolean = True

        ' Precaution
        db.Execute("DELETE FROM EcopathGroupTaxon WHERE (GroupID<1)")
        db.Execute("ALTER TABLE EcopathGroupTaxon ADD CONSTRAINT FkTaxon FOREIGN KEY (TaxonID) REFERENCES EcopathTaxon(TaxonID)")

        ' Thrash obsolete columns
        For Each strColumn As String In astrColumns
            db.Execute("ALTER TABLE EcopathGroupTaxon DROP COLUMN " & strColumn)
        Next
        Return bSucces

    End Function

End Class
