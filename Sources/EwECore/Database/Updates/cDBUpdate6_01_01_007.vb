Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.1.007:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added additional taxon fields</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_01_007
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
            Return 6.101007!
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
            Return "Added additional taxon fields."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN EcologyType LONG")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN OrganismType LONG")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN Exploited BYTE")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN ConservationStatus LONG")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN OccurenceStatus LONG")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN MeanWeight SINGLE")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN MeanLength SINGLE")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN MaxLength SINGLE")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcopathGroupTaxon ADD COLUMN MeanLifeSpan SINGLE")

        Return bSuccess

    End Function

End Class
