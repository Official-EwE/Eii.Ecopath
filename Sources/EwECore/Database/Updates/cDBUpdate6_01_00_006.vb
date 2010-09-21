Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.1.0.006:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added pedigree level name.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_01_00_006
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
            Return 6.100006!
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
            Return "Added pedigree level name"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.AddPedigreeName(db)

    End Function

    Private Function AddPedigreeName(ByVal db As cEwEDatabase) As Boolean

        Dim reader As IDataReader = Nothing
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim lstrDescriptions As New List(Of String)
        Dim liDBID As New List(Of Integer)
        Dim iDBID As Integer = 0
        Dim bSucces As Boolean = True

        If db.Execute("ALTER TABLE Pedigree ADD COLUMN LevelName TEXT(50)") Then

            ' Read Ecosim scenario IDs
            reader = db.GetReader("SELECT * FROM Pedigree")
            Try
                While reader.Read : lstrDescriptions.Add(CStr(reader("Description"))) : liDBID.Add(CInt(reader("LevelID"))) : End While
            Catch ex As Exception
            End Try
            db.ReleaseReader(reader)
            reader = Nothing

            writer = db.GetWriter("Pedigree")
            Try
                dt = writer.GetDataTable()

                For i As Integer = 0 To liDBID.Count - 1

                    iDBID = liDBID(i)
                    drow = dt.Rows.Find(iDBID)
                    Debug.Assert(drow IsNot Nothing)

                    drow.BeginEdit()
                    drow("LevelName") = lstrDescriptions(i).Substring(0, Math.Min(lstrDescriptions(i).Length, 49))
                    drow.EndEdit()

                Next i
            Catch ex As Exception
                bSucces = False
            End Try

            db.ReleaseWriter(writer, bSucces)
        End If
        Return bSucces

    End Function

End Class
