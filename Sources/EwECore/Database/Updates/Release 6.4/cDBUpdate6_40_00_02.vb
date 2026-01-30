' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database


''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.40.0.02:</para>
''' <para>
''' <list type="bullet">
''' <item><description>EcospaceScenarioCapacitDrivers table referred to Ecopath
''' group IDs. This should be Ecospace group IDs. This update also fixes the 
''' name of the table to 'EcospaceScenarioCapacityDrivers'</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_40_00_02
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.400002!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Fixed Capacity Drivers group linkages"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.AddCapacityMapAssignmentTable(db) And
               Me.TransferData(db) And
               Me.Cleanup(db)

    End Function

    Private Function AddCapacityMapAssignmentTable(db As cEwEDatabase) As Boolean
        Dim bSuccess As Boolean = True

        bSuccess = bSuccess And db.Execute("CREATE TABLE EcospaceScenarioCapacityDrivers (ScenarioID LONG, GroupID LONG, VarName TEXT(50), VarDBID LONG, ShapeID LONG)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD PRIMARY KEY (ScenarioID, GroupID, VarName, VarDBID, ShapeID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD FOREIGN KEY (ScenarioID) REFERENCES EcospaceScenario(ScenarioID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD FOREIGN KEY (GroupID) REFERENCES EcospaceScenarioGroup(GroupID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioCapacityDrivers ADD FOREIGN KEY (ShapeID) REFERENCES EcosimShape(ShapeID)")

        Me.LogProgress("ADD TABLE EcospaceScenarioCapacityDrivers", bSuccess)
        Return bSuccess

    End Function

    Private Function TransferData(db As cEwEDatabase) As Boolean

        Dim reader As IDataReader = db.GetReader("SELECT * FROM EcospaceScenarioCapacitDrivers")
        Dim writer As cEwEDatabase.cEwEDbWriter = db.GetWriter("EcospaceScenarioCapacityDrivers")
        Dim drow As DataRow = Nothing
        Dim lScenarioID As Long
        Dim lEcopathGroupID As Long
        Dim strVarName As String
        Dim lDBID As Long
        Dim lShapeID As Long
        Dim bSuccess As Boolean = True

        While reader.Read()

            lScenarioID = CLng(db.ReadSafe(reader, "ScenarioID"))
            lEcopathGroupID = CLng(db.ReadSafe(reader, "GroupID"))
            strVarName = CStr(db.ReadSafe(reader, "VarName"))
            lDBID = CLng(db.ReadSafe(reader, "VarDBID"))
            lShapeID = CLng(db.ReadSafe(reader, "ShapeID"))

            drow = writer.NewRow()
            drow("ScenarioID") = lScenarioID
            drow("GroupID") = Me.GetEcospaceGroupID(db, lScenarioID, lEcopathGroupID)
            drow("VarName") = strVarName
            drow("VarDBID") = lDBID
            drow("ShapeID") = lShapeID
            writer.AddRow(drow)

        End While
        bSuccess = bSuccess And db.ReleaseWriter(writer, True)
        db.ReleaseReader(reader)

        Me.LogProgress("Transfer CapacitDriver data to new table", bSuccess)
        Return bSuccess

    End Function

    Private Function Cleanup(db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = db.Execute("DROP TABLE EcospaceScenarioCapacitDrivers")
        Me.LogProgress("Dropped Table CapacitDriver", bSuccess)
        Return bSuccess

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="db"></param>
    ''' <param name="lScenarioID">Ecospace scenario ID</param>
    ''' <param name="lGroupID">Ecopath group ID</param>
    ''' <returns>An Ecospace group ID.</returns>
    Private Function GetEcospaceGroupID(db As cEwEDatabase,
                                        lScenarioID As Long,
                                        lGroupID As Long) As Integer
        Dim strSQL As String = String.Format("SELECT GroupID FROM EcospaceScenarioGroup WHERE ScenarioID={0} AND EcopathGroupID={1}", lScenarioID, lGroupID)
        Return CInt(db.GetValue(strSQL))

    End Function


End Class
