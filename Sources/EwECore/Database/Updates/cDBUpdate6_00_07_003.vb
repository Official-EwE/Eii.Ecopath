Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports System.Data
Imports System.Data.OleDb
Imports EwEUtils.Utilities

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.0.7.003:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added sail cost fields.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_00_07_003
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
            Return 6.07003!
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
            Return "Transferred Remark table to Auxillary."
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.FixEcospaceFleetMapPK(db) And Me.UpdateAuxillaryData(db)
    End Function

    Private Function UpdateAuxillaryData(ByVal db As cEwEDatabase) As Boolean

        Dim reader As IDataReader = db.GetReader("SELECT * FROM Remark")
        Dim iDBID As Integer = 1
        Dim strValueID As String = ""
        Dim strVisualStyle As String = ""
        Dim strRemark As String = ""
        Dim strSQL As String = ""
        Dim iPedigree As Integer = 1
        Dim bSucces As Boolean = False

        bSucces = db.Execute("CREATE TABLE Auxillary (DBID Integer, ValueID TEXT(70), Remark MEMO, VisualStyle MEMO, PedigreeLevelID INTEGER)")
        bSucces = bSucces And db.Execute("ALTER TABLE Auxillary ADD PRIMARY KEY (DBID)")

        If bSucces Then

            While reader.Read

                Try
                    strValueID = CStr(reader("ValueID"))
                Catch ex As Exception
                    strValueID = ""
                End Try

                If (Not String.IsNullOrEmpty(strValueID)) Then

                    strValueID = strValueID.Replace("_", ":")
                    strValueID = strValueID.Replace("-", ":")
                    strValueID = strValueID.Replace("(", ":")
                    strValueID = strValueID.Replace(")", "")

                    strRemark = ""
                    strVisualStyle = ""

                    If Not Convert.IsDBNull(reader("Remark")) Then
                        strRemark = CStr(reader("Remark")).Replace("""", """""")
                    End If
                    If Not Convert.IsDBNull(reader("VisualStyle")) Then
                        strVisualStyle = CStr(reader("VisualStyle"))
                    End If
                    If Not Convert.IsDBNull(reader("PedigreeLevelID")) Then
                        iPedigree = cStringUtils.ConvertToInteger(CStr(reader("PedigreeLevelID")))
                    Else
                        iPedigree = cCore.NULL_VALUE
                    End If

                    ' Need to transfer?
                    If (Not String.IsNullOrEmpty(strRemark)) Or _
                       (Not String.IsNullOrEmpty(strVisualStyle)) Or _
                       (iPedigree >= 0) Then

                        strSQL = String.Format("INSERT INTO Auxillary VALUES ({0}, ""{1}"", ""{2}"", ""{3}"", {4})", _
                                               iDBID, strValueID, strRemark, strVisualStyle, iPedigree)
                        bSucces = bSucces And db.Execute(strSQL)
                        iDBID += 1
                    End If

                End If

            End While

            db.ReleaseReader(reader)

            If bSucces Then
                bSucces = bSucces And db.Execute("DROP TABLE Remark")
            End If

        End If

        Return bSucces

    End Function

    ''' <summary>
    ''' Fix PK on EcospaceScenarioFleetMap
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FixEcospaceFleetMapPK(ByVal db As cEwEDatabase) As Boolean

        Dim strPK As String = ""
        Dim strSQL As String = ""
        Dim bSucces As Boolean = True

        strPK = db.GetPkKeyName("EcospaceScenarioFleetMap")
        If Not String.IsNullOrEmpty(strPK) Then
            strSQL = String.Format("DROP INDEX {0} ON EcospaceScenarioFleetMap", strPK)
            bSucces = db.Execute(strSQL)
        End If

        strSQL = "ALTER TABLE EcospaceScenarioFleetMap ADD PRIMARY KEY (ScenarioID, FleetID, InRow, InCol)"
        db.Execute(strSQL) ' We can do without the PK

        Return bSucces

    End Function

End Class
