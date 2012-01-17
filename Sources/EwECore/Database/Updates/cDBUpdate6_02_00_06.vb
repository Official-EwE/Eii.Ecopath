Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.2.0.06:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Discontinued regions as objects, merged into a single map.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_06
    Inherits cDBUpdate

    Private Class cRegionInfo
        Public iScenarioID As Integer
        Public strMap As String
        Public iNumRegions As Integer
    End Class

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
            Return 6.120006!
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
            Return "Discontinued regions as separate objects."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim info As cRegionInfo = Nothing
        Dim readerScenario As IDataReader = db.GetReader("SELECT ScenarioID, InRow, InCol, DepthMap FROM EcospaceScenario")
        Dim readerRegions As IDataReader = Nothing
        Dim aiDepth(,) As Integer = Nothing
        Dim aiRegions(,) As Integer = Nothing
        Dim aiRegion(,) As Integer = Nothing
        Dim InRow, InCol As Integer
        Dim lRegions As New List(Of cRegionInfo)
        Dim bSuccess As Boolean = True

        ' For each scenario
        While readerScenario.Read()

            ' Prepare buffers
            info = New cRegionInfo()
            lRegions.Add(info)

            ' Read scenario bits
            info.iScenarioID = CInt(readerScenario("ScenarioID"))
            InRow = CInt(readerScenario("InRow"))
            InCol = CInt(readerScenario("InCol"))

            ' Allocate memory
            ReDim aiDepth(InRow, InCol)
            ReDim aiRegions(InRow, InCol)
            ReDim aiRegion(InRow, InCol)

            ' Read depth map
            cStringUtils.StringToArray(CStr(readerScenario("DepthMap")), aiDepth)

            ' Read region maps and merge 'em
            readerRegions = db.GetReader(String.Format("SELECT * FROM EcospaceScenarioRegion WHERE (ScenarioID={0}) ORDER BY Sequence", info.iScenarioID))
            While readerRegions.Read()

                ' Account for region
                info.iNumRegions += 1

                ' Read region map
                Array.Clear(aiRegion, 0, aiRegion.Length)
                cStringUtils.StringToArray(CStr(readerRegions("RegionMap")), aiRegion)

                ' Merge region map into final
                For iRow As Integer = 1 To InRow
                    For iCol As Integer = 1 To InCol
                        If (aiRegion(iRow, iCol) > 0) Then
                            aiRegions(iRow, iCol) = info.iNumRegions
                        End If
                    Next iCol
                Next iRow

            End While

            ' Preserve map
            info.strMap = cStringUtils.ArrayToString(aiRegions, aiDepth)

            ' Clean up
            db.ReleaseReader(readerRegions)
            readerRegions = Nothing
            aiDepth = Nothing
            aiRegions = Nothing
            aiRegion = Nothing

        End While

        ' Clean up
        db.ReleaseReader(readerScenario)
        readerScenario = Nothing

        ' Update receiving end
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN NumRegions LONG")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN RegionMap MEMO")

        ' Store maps
        Dim writer As cEwEDatabase.cEwEDbWriter = db.GetWriter("EcospaceScenario")
        Dim dt As DataTable = writer.GetDataTable
        For Each info In lRegions
            Dim drow As DataRow = dt.Rows.Find(info.iScenarioID)
            Try
                drow.BeginEdit()
                drow("RegionMap") = info.strMap
                drow("NumRegions") = info.iNumRegions
                drow.EndEdit()
            Catch ex As Exception
                bSuccess = False
            End Try
        Next
        db.ReleaseWriter(writer, bSuccess)
        lRegions.Clear()

        ' Destroy region table
        If bSuccess Then
            bSuccess = bSuccess And db.Execute("DROP TABLE EcospaceScenarioRegion")
        End If

        Return bSuccess

    End Function

End Class
