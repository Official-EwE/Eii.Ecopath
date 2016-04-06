' ========================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ========================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Utilities
Imports EwEUtils.SystemUtilities

#End Region ' Imports

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.25:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Added Ecopath sample tables</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_25
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500025!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Added Ecospace migration area movement, Ecosim environmental driver table"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Return Me.AddMigField(db) And
               Me.AddEcosimDriverTable(db) ' And
        ' ToDo: enable this in a next database update when Ecosim Env Drivers changes move back to the SVN trunk
        'Me.ConvertEcosimEnvDrivers(db) And
        'Me.Cleanup(db)

    End Function

    Private Function AddMigField(ByVal db As cEwEDatabase) As Boolean
        Return db.Execute("ALTER TABLE EcospaceScenarioGroup ADD COLUMN InMigAreaMovement Single")
    End Function

    ' ToDo: enable this in a next database update when Ecosim Env Drivers changes move back to the SVN trunk
    Private Function AddEcosimDriverTable(ByVal db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = db.Execute("CREATE TABLE EcosimScenarioCapacityDrivers (ScenarioID LONG, GroupID LONG, DriverID LONG, ResponseID LONG)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD CONSTRAINT pk PRIMARY KEY (ScenarioID, GroupID, DriverID)")

        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD FOREIGN KEY (ScenarioID) REFERENCES EcosimScenario (ScenarioID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD FOREIGN KEY (GroupID) REFERENCES EcosimScenarioGroup (GroupID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD FOREIGN KEY (DriverID) REFERENCES EcosimShape (ShapeID)")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenarioCapacityDrivers ADD FOREIGN KEY (ResponseID) REFERENCES EcosimShape (ShapeID)")

        Return bSuccess

    End Function

    ' ToDo: enable this in a next database update when Ecosim Env Drivers changes move back to the SVN trunk
    Private Function ConvertEcosimEnvDrivers(ByVal db As cEwEDatabase) As Boolean

        Dim nGroups As Integer = CInt(db.GetValue("SELECT COUNT(*) FROM EcopathGroup", 0))
        Dim strNames(nGroups) As String
        Dim iDBID(nGroups) As Integer
        Dim iGroup As Integer = 1
        Dim reader As IDataReader = Nothing
        Dim bSuccess As Boolean = True

        reader = db.GetReader("SELECT * FROM EcopathGroup ORDER BY Sequence ASC")
        While reader.Read()
            iDBID(iGroup) = CInt(reader("GroupID"))
            strNames(iGroup) = CStr(reader("GroupName"))
            iGroup += 1
        End While
        db.ReleaseReader(reader)

        reader = db.GetReader("SELECT * FROM EcoSimScenarioGroup")
        While reader.Read()

            Dim iScenarioID As Integer = CInt(reader("ScenarioID"))
            Dim iGroupID As Integer = CInt(reader("GroupID"))
            Dim iEcopathGroupID As Integer = CInt(reader("EcopathGroupID"))

            iGroup = Array.IndexOf(iDBID, iEcopathGroupID)

            If (iGroup > 0) Then
                Dim iSShapeID As Integer = CInt(db.GetValue("SELECT MAX(SalinityForcingShapeID) FROM EcosimScenario WHERE (ScenarioID=" & iScenarioID & ")"))
                Dim iTShapeID As Integer = CInt(db.GetValue("SELECT MAX(TemperatureForcingShapeID) FROM EcosimScenario WHERE (ScenarioID=" & iScenarioID & ")"))
                Dim iResponseID As Integer = 0

                Dim sSO As Single = CSng(db.ReadSafe(reader, "SalOpt", 35.0!))
                Dim sSL As Single = CSng(db.ReadSafe(reader, "SdSalLeft", 1000.0!))
                Dim sSR As Single = CSng(db.ReadSafe(reader, "SdSalRight", 1000.0!))
                Dim sTO As Single = CSng(db.ReadSafe(reader, "TempOpt", 10.0!))
                Dim sTL As Single = CSng(db.ReadSafe(reader, "TempLeft", 1000.0!))
                Dim sTR As Single = CSng(db.ReadSafe(reader, "TempRight", 1000.0!))

                If (iSShapeID > 0) And (sSR <> 1000) And (sSL <> 1000) Then
                    bSuccess = bSuccess And Me.CreateReponseCurve(db, "Salinity " & iGroup & " " & strNames(iGroup), sSO, sSL, sSR, iResponseID) And
                                            Me.AssignResponse(db, iScenarioID, iGroupID, iSShapeID, iResponseID)
                End If

                If (iTShapeID > 0) And (sTR <> 1000) And (sTL <> 1000) Then
                    bSuccess = bSuccess And Me.CreateReponseCurve(db, "Temp " & iGroup & " " & strNames(iGroup), sTO, sTL, sTR, iResponseID) And
                                            Me.AssignResponse(db, iScenarioID, iGroupID, iTShapeID, iResponseID)
                End If
            End If

        End While
        db.ReleaseReader(reader)

        Return bSuccess

    End Function

    Private Function Cleanup(ByVal db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True
        For Each str As String In New String() {"SalOpt", "SdSalLeft", "SdSalRight", "TempOpt", "TempLeft", "TempRight"}
            bSuccess = bSuccess And db.Execute("ALTER TABLE EcoSimScenarioGroup DROP COLUMN " & str)
        Next
        For Each str As String In New String() {"SalinityForcingShapeID", "TemperatureForcingShapeID"}
            bSuccess = bSuccess And db.Execute("ALTER TABLE EcosimScenario DROP COLUMN " & str)
        Next
        Return bSuccess

    End Function

#Region " Shape rerouting "

    Private Function CreateReponseCurve(ByVal db As cEwEDatabase, ByVal strTitle As String,
                                     ByVal sOpt As Single, ByVal sStdLeft As Single, ByVal sStdRight As Single,
                                     ByRef iShapeID As Integer) As Boolean

        Dim ds As New cMediationDataStructures()
        Dim sLeft As Single = sOpt - sStdLeft * 5
        Dim sRight As Single = sOpt + sStdRight * 5
        Dim dV As Single = (sRight - sLeft) / ds.NMedPoints
        Dim bSuccess As Boolean = True

        iShapeID = CInt(db.GetValue("SELECT MAX(ShapeID) FROM EcoSimShape", 0)) + 1

        Dim writerID As cEwEDatabase.cEwEDbWriter = db.GetWriter("EcoSimShape")
        Dim writerShape As cEwEDatabase.cEwEDbWriter = db.GetWriter("EcosimShapeMediation")
        Dim drow As DataRow = writerID.NewRow()
        Dim sbZScale As New Text.StringBuilder()

        Try
            drow("ShapeID") = iShapeID
            drow("ShapeType") = EwEUtils.Core.eDataTypes.CapacityMediation
            drow("IsSeasonal") = False
            writerID.AddRow(drow)
            writerID.Commit()

            drow = writerShape.NewRow()
            drow("ShapeID") = iShapeID
            drow("Title") = strTitle

            For ipt As Integer = 1 To ds.NMedPoints
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(cStringUtils.FormatSingle(Me.MedPoint(sLeft + (dV * ipt), sOpt, sStdLeft, sStdRight)))
            Next
            drow("zScale") = sbZScale.ToString()

            drow("FunctionType") = 0
            drow("FunctionParams") = ""
            drow("IMedBase") = ds.NMedPoints / 3
            drow("XAxisMin") = sLeft
            drow("XAxisMax") = sRight
            writerShape.AddRow(drow)
            writerShape.Commit()

            db.ReleaseWriter(writerShape, True)
            db.ReleaseWriter(writerID)
        Catch ex As Exception
            '  Me.LogMessage(String.Format("Error {0} occurred while appending shape {1}, {2}", ex.Message, strShapeName, shapeType.ToString()))
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    Private Function AssignResponse(ByVal db As cEwEDatabase,
                                    ByVal iScenarioID As Integer, ByVal iGroupID As Integer,
                                    ByVal iDriverID As Integer, ByVal iResponseID As Integer) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = db.GetWriter("EcosimScenarioCapacityDrivers")
        Dim drow As DataRow = writer.NewRow()

        drow("ScenarioID") = iScenarioID
        drow("GroupID") = iGroupID
        drow("DriverID") = iDriverID
        drow("ResponseID") = iResponseID

        writer.AddRow(drow)
        db.ReleaseWriter(writer)

        Return True

    End Function

    Private Function MedPoint(ByVal sVal As Single, ByVal sOpt As Single, ByVal sStdLeft As Single, ByVal sStdRight As Single) As Single
        Return cSystemUtils.IIF(sVal < sOpt,
                                CSng(Math.Exp(-0.5 * ((sVal - sOpt) / (sStdLeft + 0.0000001)) ^ 2)),
                                CSng(Math.Exp(-0.5 * ((sVal - sOpt) / (sStdRight + 0.0000001)) ^ 2)))
    End Function

#End Region ' Shape rerouting 

End Class
