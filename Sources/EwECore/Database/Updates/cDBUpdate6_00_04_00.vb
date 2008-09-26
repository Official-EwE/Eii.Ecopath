'==============================================================================
'
' $Log: cDBUpdate6_00_04_00.vb,v $
' Revision 1.1  2008/09/26 07:30:15  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/05/20 13:40:58  jeroens
' Increased robustness
'
' Revision 1.2  2008/04/07 17:00:51  jeroens
' Transactions committed properly
'
' Revision 1.1  2008/02/11 03:21:04  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.3.8:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added EcosimTimeseriesDataset.</description></item>
''' <item><description>Migrated existing Time Series data to new Dataset table.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Public Class cDBUpdate6_00_04_00
    Implements IDatabaseUpdatePlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The actual update logic.
    ''' </summary>
    ''' <param name="db">Database to modify.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean _
            Implements EwEPlugin.IDatabaseUpdatePlugin.ApplyUpdate

        Dim reader As IDataReader = Nothing
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim strDataset As String = ""
        Dim strDatasetLast As String = ""
        Dim iDatasetID As Integer = 0
        Dim bSucces As Boolean = True

        db.BeginTransaction()

        ' + Add EcosimTimeSeriesDataset
        bSucces = bSucces And db.Execute("CREATE TABLE EcosimTimeseriesDataset (DatasetID INTEGER, DatasetName TEXT(50), Description MEMO, Author TEXT(64), Contact TEXT(255), FirstYear INTEGER, NumYears INTEGER)")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimTimeseriesDataset ADD PRIMARY KEY (DatasetID)")
        ' + Add FK
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimTimeseries ADD COLUMN DatasetID LONG")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimTimeseries ADD FOREIGN KEY (DatasetID) REFERENCES EcosimTimeseriesDataset(DatasetID)")

        ' Nothing to migrate?
        reader = db.GetReader("SELECT * FROM EcosimTimeSeries ORDER BY Dataset")
        If reader IsNot Nothing Then

            ' Start migrating

            ' Populate dataset table
            writer = db.GetWriter("EcosimTimeSeriesDataset")
            While reader.Read()
                strDataset = CStr(reader("Dataset"))
                If (String.Compare(strDatasetLast, strDataset, False) <> 0) Then
                    iDatasetID += 1
                    drow = writer.NewRow()
                    drow("DatasetID") = iDatasetID
                    drow("DatasetName") = strDataset
                    drow("FirstYear") = reader("FirstYear")
                    drow("NumYears") = reader("NumYears")
                    writer.AddRow(drow)
                    strDatasetLast = strDataset
                End If
            End While
            db.ReleaseWriter(writer, True)
            db.ReleaseReader(reader)

            ' Link existing time series to new datasets
            reader = db.GetReader("SELECT DatasetID, DatasetName FROM EcosimTimeseriesDataset")
            While reader.Read()
                bSucces = bSucces And db.Execute(String.Format("UPDATE EcosimTimeseries SET DatasetID={0} WHERE Dataset='{1}'", _
                        CInt(reader("DatasetID")), CStr(reader("DatasetName"))))
            End While
            db.ReleaseReader(reader)
        End If

        ' Try to delete columns Dataset, FirstYear, NumYears from table Time Series (not crucial)
        db.Execute("ALTER TABLE EcosimTimeseries DROP COLUMN Dataset")
        db.Execute("ALTER TABLE EcosimTimeseries DROP COLUMN FirstYear")
        db.Execute("ALTER TABLE EcosimTimeseries DROP COLUMN NumYears")

        If bSucces Then
            bSucces = db.CommitTransaction(True)
        Else
            db.RollbackTransaction()
        End If
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property UpdateDescription() As String Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateDescription
        Get
            Return "Adds Ecosim TimeseriesDataset table" + vbNewLine + "Migrates existing Time Series data to new Dataset table"
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
    Public ReadOnly Property UpdateVersion() As Single Implements EwEPlugin.IDatabaseUpdatePlugin.UpdateVersion
        Get
            Return 6.04!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="IPlugin.Description">IPlugin.Description</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return Me.UpdateDescription
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="IPlugin.Initialize">IPlugin.Initialize</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        ' Void
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="IPlugin.Name">IPlugin.Name</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "Database update " & Me.UpdateVersion
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Author">IPlugin.Author</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Contact">IPlugin.Contact</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:support@ecopath.org"
        End Get
    End Property

End Class
