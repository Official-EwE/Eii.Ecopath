' ===============================================================================
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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Imports EwEUtils.SystemUtilities.cSystemUtils

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.2.0.01:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Restructured saving of Ecospace maps.</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_02_00_01
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.120001!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Optimized Ecospace map storage structure for speedy saving and loading"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean
        Return Me.UpdateEcospaceTables(db) And _
               Me.ConvertAll(db) And _
               Me.DropObsoleteTables(db)
    End Function

    Private Function UpdateEcospaceTables(ByVal db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = True

        ' EcospaceScenarioBasemap -> EcospaceScenarioMap
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN DepthMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN RelPPMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN RelCinMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN XVelMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN YVelMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenario ADD COLUMN DepthAMap MEMO")

        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioHabitat ADD COLUMN HabitatMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioMPA ADD COLUMN MPAMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioRegion ADD COLUMN RegionMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioFleet ADD COLUMN PortMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioFleet ADD COLUMN SailCostMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioGroup ADD COLUMN CapacityMap MEMO")
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioWeightLayer ADD COLUMN LayerMap MEMO")

        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioGroup DROP CONSTRAINT " & db.GetPkKeyName("EcospaceScenarioGroup"))
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioGroup ADD PRIMARY KEY (ScenarioID, EcopathGroupID)")

        ' Re-forge ecospace fleet relationships due to ^@!#^% Access limitations
        ' Good grief, target table has to specified BACKWARDS?!
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioMPAFishery DROP CONSTRAINT " & db.GetFkKeyName("EcospaceScenarioFleet", "EcospaceScenarioMPAFishery", "FleetID"))
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioHabitatFishery DROP CONSTRAINT " & db.GetFkKeyName("EcospaceScenarioFleet", "EcospaceScenarioHabitatFishery", "FleetID"))
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioFleet DROP CONSTRAINT " & db.GetPkKeyName("EcospaceScenarioFleet"))
        bSuccess = bSuccess And db.Execute("ALTER TABLE EcospaceScenarioFleet ADD PRIMARY KEY (ScenarioID, EcopathFleetID)")

        Me.LogProgress("UpdateEcospaceTables", bSuccess)
        Return bSuccess

    End Function

    Private Function ConvertAll(ByVal db As cEwEDatabase) As Boolean

        Dim readerScenario As IDataReader = db.GetReader("SELECT ScenarioID, InRow, InCol FROM EcospaceScenario")
        Dim lScenarioID As New List(Of Integer)
        Dim lInRow As New List(Of Integer)
        Dim lInCol As New List(Of Integer)
        Dim bSuccess As Boolean = True

        While readerScenario.Read
            lScenarioID.Add(CInt(readerScenario("ScenarioID")))
            lInRow.Add(CInt(readerScenario("InRow")))
            lInCol.Add(CInt(readerScenario("InCol")))
        End While
        db.ReleaseReader(readerScenario)

        For i As Integer = 0 To lScenarioID.Count - 1
            bSuccess = bSuccess And Me.ConvertScenario(db, lScenarioID(i), lInRow(i), lInCol(i))
        Next
        Me.LogProgress("ConvertAll", bSuccess)
        Return bSuccess

    End Function

    Private Function DropObsoleteTables(ByVal db As cEwEDatabase) As Boolean

        Dim bSuccess As Boolean = db.Execute("DROP TABLE EcospaceScenarioBasemap")
        bSuccess = bSuccess And db.Execute("DROP TABLE EcospaceScenarioFleetMap")
        bSuccess = bSuccess And db.Execute("DROP TABLE EcospaceScenarioGroupMap")
        bSuccess = bSuccess And db.Execute("DROP TABLE EcospaceScenarioWeightLayerCell")

        ' Remove non-critical fields that may have withstood earlier attempts at eradication
        db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN Depth")
        db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN MPA")
        db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN HabType")
        db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN Region")
        db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN RelPP")
        db.Execute("ALTER TABLE EcospaceScenario DROP COLUMN RelCin")

        Return bSuccess

    End Function

    Private Function ConvertScenario(ByVal db As cEwEDatabase, ByVal iScenarioID As Integer, _
                                     ByVal InRow As Integer, ByVal InCol As Integer) As Boolean

        Dim reader As IDataReader = Nothing
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing

        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim key As Object() = {iScenarioID, 0}

        ' IDs
        Dim lHabitatID As New List(Of Long)
        Dim lMPAID As New List(Of Long)
        Dim lRegionID As New List(Of Long)
        Dim lGroupID As New List(Of Long)
        Dim lFleetID As New List(Of Long)
        Dim lImportanceLayerID As New List(Of Long)

        ' Data
        Dim dataDepth As Single(,) = Nothing
        Dim dataRelPP As Single(,) = Nothing
        Dim dataRelCin As Single(,) = Nothing
        Dim dataDepthA As Single(,) = Nothing : Dim dataXVel As Single(,) = Nothing : Dim dataYVel As Single(,) = Nothing
        Dim dataMPA As Integer(,) = Nothing
        Dim dataHabitat As Integer(,,) = Nothing
        Dim dataRegion As Integer(,) = Nothing
        Dim dataPort As Integer(,,) = Nothing
        Dim dataSailingCost As Single(,,) = Nothing
        Dim dataHabCap As Single(,,) = Nothing
        Dim dataImportance As Single(,,) = Nothing

        Dim i As Integer, iID As Integer
        Dim iRow As Integer, iCol As Integer
        Dim bSuccess As Boolean = True

        ' Read IDs for scenario
        reader = db.GetReader("SELECT HabitatID From EcospaceScenarioHabitat WHERE ScenarioID=" & iScenarioID & " ORDER BY Sequence ASC")
        While reader.Read
            lHabitatID.Add(CInt(reader("HabitatID")))
        End While
        db.ReleaseReader(reader)

        reader = db.GetReader("SELECT MPAID From EcospaceScenarioMPA WHERE ScenarioID=" & iScenarioID & " ORDER BY Sequence ASC")
        While reader.Read
            lMPAID.Add(CInt(reader("MPAID")))
        End While
        db.ReleaseReader(reader)

        reader = db.GetReader("SELECT RegionID From EcospaceScenarioRegion WHERE ScenarioID=" & iScenarioID & " ORDER BY Sequence ASC")
        While reader.Read
            lRegionID.Add(CInt(reader("RegionID")))
        End While
        db.ReleaseReader(reader)

        reader = db.GetReader("SELECT GroupID From EcospaceScenarioGroup WHERE ScenarioID=" & iScenarioID)
        While reader.Read
            lGroupID.Add(CInt(reader("GroupID")))
        End While
        db.ReleaseReader(reader)

        reader = db.GetReader("SELECT FleetID From EcospaceScenarioFleet WHERE ScenarioID=" & iScenarioID)
        While reader.Read
            lFleetID.Add(CInt(reader("FleetID")))
        End While
        db.ReleaseReader(reader)

        reader = db.GetReader("SELECT LayerID From EcospaceScenarioWeightLayer WHERE ScenarioID=" & iScenarioID & " ORDER BY Sequence ASC")
        While reader.Read
            lImportanceLayerID.Add(CInt(reader("LayerID")))
        End While
        db.ReleaseReader(reader)

        ReDim dataDepth(InRow, InCol)
        ReDim dataRelPP(InRow, InCol)
        ReDim dataRelCin(InRow, InCol)
        ReDim dataXVel(InRow, InCol)
        ReDim dataYVel(InRow, InCol)
        ReDim dataDepthA(InRow, InCol)
        ReDim dataHabitat(InRow, InCol, lHabitatID.Count)
        ReDim dataMPA(InRow, InCol)
        ReDim dataRegion(InRow, InCol)
        ReDim dataPort(lFleetID.Count, InRow, InCol)
        ReDim dataSailingCost(lFleetID.Count, InRow, InCol)
        ReDim dataHabCap(InRow, InCol, lGroupID.Count)
        ReDim dataImportance(InRow, InCol, lImportanceLayerID.Count)

        ' Read basemap data
        reader = db.GetReader("SELECT * FROM EcospaceScenarioBasemap WHERE ScenarioID=" & iScenarioID)
        While reader.Read

            iRow = CInt(reader("InRow"))
            iCol = CInt(reader("InCol"))

            dataDepth(iRow, iCol) = CInt(reader("Depth"))
            dataRelPP(iRow, iCol) = CSng(reader("RelPP"))
            dataRelCin(iRow, iCol) = CSng(reader("RelCin"))
            dataXVel(iRow, iCol) = CSng(db.ReadSafe(reader, "XVel", 0.0!))
            dataYVel(iRow, iCol) = CSng(db.ReadSafe(reader, "YVel", 0.0!))
            dataDepthA(iRow, iCol) = CSng(db.ReadSafe(reader, "DepthA", 1.0!))

            ' Exclude 'all' habitat map
            i = lHabitatID.IndexOf(CInt(reader("HabitatID")))
            If (i > 0) Then
                dataHabitat(iRow, iCol, i) = 1
            End If

            i = lMPAID.IndexOf(CInt(reader("MPAID")))
            If (i >= 0) Then
                dataMPA(iRow, iCol) = i + 1
            End If

            i = lRegionID.IndexOf(CInt(reader("RegionID")))
            If (i > 0) Then
                dataRegion(iRow, iCol) = lRegionID.IndexOf(iID) + 1
            End If

        End While
        db.ReleaseReader(reader)

        ' Read fleet data
        For i = 0 To lFleetID.Count - 1
            reader = db.GetReader("SELECT * FROM EcospaceScenarioFleetMap WHERE ScenarioID=" & iScenarioID & " AND FleetID=" & lFleetID(i))
            While reader.Read

                iRow = CInt(reader("InRow"))
                iCol = CInt(reader("InCol"))

                If (iRow <= InRow) And (iCol <= InCol) Then
                    dataPort(i, iRow, iCol) = CInt(IIF(CInt(reader("PortID")) > 0, 1, 0))
                    dataSailingCost(i, iRow, iCol) = CSng(db.ReadSafe(reader, "SailCost", 0.0!))
                End If

            End While
            db.ReleaseReader(reader)
            reader = Nothing
        Next

        ' Read group data
        For i = 0 To lGroupID.Count - 1
            reader = db.GetReader("SELECT * FROM EcospaceScenarioGroupMap WHERE ScenarioID=" & iScenarioID & " AND GroupID=" & lGroupID(i))
            While reader.Read

                iRow = CInt(reader("InRow"))
                iCol = CInt(reader("InCol"))
                If (iRow <= InRow) And (iCol <= InCol) Then
                    dataHabCap(iRow, iCol, i) = CSng(reader("Capacity"))
                End If

            End While
            db.ReleaseReader(reader)
            reader = Nothing
        Next

        Try

            ' Read importance layer data
            For i = 0 To lImportanceLayerID.Count - 1
                reader = db.GetReader("SELECT * FROM EcospaceScenarioWeightLayerCell WHERE ScenarioID=" & iScenarioID & " AND LayerID=" & lImportanceLayerID(i))
                While reader.Read

                    iRow = CInt(reader("InRow"))
                    iCol = CInt(reader("InCol"))
                    If (iRow <= InRow) And (iCol <= InCol) Then
                        dataImportance(iRow, iCol, i) = CSng(reader("Weight"))
                    End If

                End While
                db.ReleaseReader(reader)
                reader = Nothing
            Next
        Catch ex As Exception
            ' Too bad, keep moving on
        End Try

        ' ---------------------------
        ' Update tables
        ' ---------------------------

        ' 1) EcospaceScenarioMap
        writer = db.GetWriter("EcospaceScenario")
        dt = writer.GetDataTable
        drow = dt.Rows.Find(iScenarioID)
        drow.BeginEdit()
        drow("DepthMap") = cStringUtils.ArrayToString(dataDepth)
        drow("RelPPMap") = cStringUtils.ArrayToString(dataRelPP, dataDepth)
        drow("RelCinMap") = cStringUtils.ArrayToString(dataRelCin, dataDepth)
        drow("XVelMap") = cStringUtils.ArrayToString(dataXVel, dataDepth)
        drow("YVelMap") = cStringUtils.ArrayToString(dataYVel, dataDepth)
        drow("DepthAMap") = cStringUtils.ArrayToString(dataDepthA, dataDepth)
        drow.EndEdit()
        db.ReleaseWriter(writer)
        writer = Nothing

        ' 1) EcospaceScenarioHabitat
        writer = db.GetWriter("EcospaceScenarioHabitat")
        dt = writer.GetDataTable()
        For i = 1 To lHabitatID.Count - 1
            key(1) = lHabitatID(i)
            drow = dt.Rows.Find(key)
            If (drow IsNot Nothing) Then
                drow.BeginEdit()
                drow("HabitatMap") = cStringUtils.ArrayToString(dataHabitat, i, cStringUtils.eFilterIndexTypes.LastIndex, InRow, InCol, dataDepth, True)
                drow.EndEdit()
            End If
        Next
        db.ReleaseWriter(writer)
        writer = Nothing

        ' 2) EcospaceScenarioMPA
        writer = db.GetWriter("EcospaceScenarioMPA")
        dt = writer.GetDataTable()
        For i = 1 To lMPAID.Count
            key(1) = lMPAID(i - 1)
            drow = dt.Rows.Find(key)
            If (drow IsNot Nothing) Then
                drow.BeginEdit()
                drow("MPAMap") = cStringUtils.ArrayToString(dataMPA, dataDepth, True, i)
                drow.EndEdit()
            End If
        Next
        db.ReleaseWriter(writer)
        writer = Nothing

        ' 3) EcospaceScenarioRegion
        writer = db.GetWriter("EcospaceScenarioRegion")
        dt = writer.GetDataTable()
        For i = 1 To lRegionID.Count
            key(1) = lRegionID(i - 1)
            drow = dt.Rows.Find(key)
            If (drow IsNot Nothing) Then
                drow.BeginEdit()
                drow("RegionMap") = cStringUtils.ArrayToString(dataRegion, dataDepth, True, i)
                drow.EndEdit()
            End If
        Next
        db.ReleaseWriter(writer)
        writer = Nothing

        ' 4) Groups
        writer = db.GetWriter("EcospaceScenarioGroup")
        dt = writer.GetDataTable()
        For i = 0 To lGroupID.Count - 1
            key(1) = lGroupID(i)
            drow = dt.Rows.Find(key)
            If (drow IsNot Nothing) Then
                drow.BeginEdit()
                ' Save ports on land only
                drow("CapacityMap") = cStringUtils.ArrayToString(dataHabCap, i, cStringUtils.eFilterIndexTypes.LastIndex, _
                                                                 InRow, InCol, dataDepth)
                drow.EndEdit()
            End If
        Next
        db.ReleaseWriter(writer)
        writer = Nothing

        ' 5) Fleets
        writer = db.GetWriter("EcospaceScenarioFleet")
        dt = writer.GetDataTable()
        For i = 0 To lFleetID.Count - 1
            key(1) = lFleetID(i)
            drow = dt.Rows.Find(key)
            If (drow IsNot Nothing) Then
                drow.BeginEdit()
                ' Save ports on land only
                drow("PortMap") = cStringUtils.ArrayToString(dataPort, i, cStringUtils.eFilterIndexTypes.FirstIndex, InRow, InCol)
                ' Save sailing cost on water only
                drow("SailCostMap") = cStringUtils.ArrayToString(dataSailingCost, i, cStringUtils.eFilterIndexTypes.FirstIndex, InRow, InCol, dataDepth)
                drow.EndEdit()
            End If
        Next
        db.ReleaseWriter(writer)
        writer = Nothing

        ' 5) Importance layers
        writer = db.GetWriter("EcospaceScenarioWeightLayer")
        dt = writer.GetDataTable()
        For i = 0 To lImportanceLayerID.Count - 1
            ' This table is not indexed by scenario, only by layer ID. Should be ok, but it is an oversight
            drow = dt.Rows.Find(lImportanceLayerID(i))
            If (drow IsNot Nothing) Then
                drow.BeginEdit()
                drow("LayerMap") = cStringUtils.ArrayToString(dataImportance, i, cStringUtils.eFilterIndexTypes.LastIndex, InRow, InCol, dataDepth)
                drow.EndEdit()
            End If
        Next
        db.ReleaseWriter(writer)
        writer = Nothing

        Return True

    End Function

End Class
