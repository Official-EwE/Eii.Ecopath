' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

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

    Private Function UpdateAuxillaryData(db As cEwEDatabase) As Boolean

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

                    strRemark = CStr(db.ReadSafe(reader, "Remark", "")).Replace("""", """""")
                    strVisualStyle = CStr(db.ReadSafe(reader, "VisualStyle", ""))
                    iPedigree = CInt(db.ReadSafe(reader, "Pedigree", cCore.NULL_VALUE))

                    ' Need to transfer?
                    If (Not String.IsNullOrEmpty(strRemark)) Or
                       (Not String.IsNullOrEmpty(strVisualStyle)) Or
                       (iPedigree >= 0) Then

                        strSQL = String.Format("INSERT INTO Auxillary VALUES ({0}, ""{1}"", ""{2}"", ""{3}"", {4})",
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
    Private Function FixEcospaceFleetMapPK(db As cEwEDatabase) As Boolean

        Dim strPK As String = ""
        Dim strSQL As String = ""
        Dim bSucces As Boolean = True

        strPK = db.GetPkKeyName("EcospaceScenarioFleetMap")
        If Not String.IsNullOrEmpty(strPK) Then
            strSQL = String.Format("DROP INDEX {0} ON EcospaceScenarioFleetMap", strPK)
            bSucces = db.Execute(strSQL)
        End If

        strSQL = "ALTER TABLE EcospaceScenarioFleetMap ADD PRIMARY KEY (ScenarioID, FleetID, InRow, InCol)"
        db.Execute(strSQL)

        Return bSucces

    End Function

End Class
