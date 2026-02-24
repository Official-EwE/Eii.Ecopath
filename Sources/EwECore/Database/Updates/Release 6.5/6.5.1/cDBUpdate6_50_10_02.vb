' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.1.02:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Updated Ecotracer</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_10_02
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.501002!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Advection, upwelling stored by month"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = db.Execute("ALTER TABLE EcospaceScenarioMonth ADD COLUMN AdvectionXVelMap MEMO") And
                                 db.Execute("ALTER TABLE EcospaceScenarioMonth ADD COLUMN AdvectionYVelMap MEMO") And
                                 db.Execute("ALTER TABLE EcospaceScenarioMonth ADD COLUMN UpwellingMap MEMO")

        If Not bSucces Then Return False

        ' Duplicate advection data to month fields
        Dim writer As cEwEDatabase.cEwEDbWriter = db.GetWriter("EcospaceScenarioMonth")
        Dim dt As DataTable = writer.GetDataTable()
        Dim drow As DataRow = Nothing
        Dim keys() As Object = New Object() {0, 0}
        Dim reader As IDataReader = db.GetReader("SELECT * FROM EcospaceScenario")

        While reader.Read()
            Dim iScenarioID As Integer = CInt(reader("ScenarioID"))
            Dim strXVel As String = CStr(db.ReadSafe(reader, "XVelMap", ""))
            Dim strYVel As String = CStr(db.ReadSafe(reader, "YVelMap", ""))

            If Not String.IsNullOrWhiteSpace(strXVel) And Not String.IsNullOrWhiteSpace(strYVel) Then
                For iMonth As Integer = 1 To cCore.N_MONTHS
                    keys(0) = iScenarioID
                    keys(1) = iMonth
                    drow = dt.Rows.Find(keys)
                    Dim bNewRow As Boolean = (drow Is Nothing)

                    If (bNewRow) Then
                        drow = writer.NewRow()
                        drow("ScenarioID") = iScenarioID
                        drow("MonthID") = iMonth
                    Else
                        drow.BeginEdit()
                    End If

                    drow("AdvectionXVelMap") = strXVel
                    drow("AdvectionYVelMap") = strYVel

                    If (bNewRow) Then
                        writer.AddRow(drow)
                    Else
                        drow.EndEdit()
                    End If
                Next
                writer.Commit()
            End If
        End While

        db.ReleaseReader(reader)
        db.ReleaseWriter(writer)

        db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN XVelMap")
        db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN YVelMap")

        Return True

    End Function

End Class
