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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Explicit On

Imports System.IO
Imports System.Windows.Forms

Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

Public Class cSailCostPlugin
    Implements IUIContextPlugin
    Implements INavigationTreeItemPlugin
    Implements IEcopathRunInitializedPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceBeginTimestepPlugin
    Implements IEcospaceEndTimestepPlugin
    Implements IEcospaceRunCompletedPlugin

#Region " Private variables "

    Private m_core As cCore
    Private m_ecospacedata As cEcospaceDataStructures
    Private m_ecopathdata As cEcopathDataStructures
    Private m_ecosim As cEcoSimScenario
    Private m_SailOrg As Single(,,)
    Private m_sCon As String
    Private LMEarea() As Double
    Private RelEffortLME(,,) As Single
    Private nCellsInLME() As Integer    'VC131216: was defined as double, but I changed it to integer
    Private nCellsInLMETotal As Integer
    Private CellListForAllLMEs() As Integer
    Private CellListForEachLME(,) As Integer

    Private m_SumArea As Double = 0
    Private m_nFleets As Integer = 14
    Private m_nYears As Integer = 101 ' ToDo: extend for FishMIP

    Private m_strDataPath As String
    Private m_bUseSailCostPlugin As Boolean

    Private m_uic As cUIContext
    Private m_frmUI As frmSailCost

    Private AreaFishedLME(,) As Single
    Private m_lastYear As Integer

    Private EffortbyFleetLMEYear(,,) As Double

#End Region

#Region " Events "

    Friend Event OnChanged()

#End Region ' Events

#Region "Public variables and Properties"

    Public ReadOnly Property IsInputdataValid() As Boolean
        Get
            ' This really should not happen, but hey
            If (Me.m_core.DataSource Is Nothing) Then Return False
            If Not Me.m_core.DataSource.FileName.ToUpper.Contains("ECOOCEAN") Then Return False
            Return (File.Exists(Me.EffortFile) And File.Exists(Me.LMECellsFile))
        End Get
    End Property

    Public ReadOnly Property EffortFile As String
        Get
            Dim strEffortFile As String = ""
            Dim CurrentScenario As Integer = m_core.ActiveTimeSeriesDatasetIndex

            Select Case CurrentScenario
                Case 1 To 4
                    strEffortFile = "RelEffort_LME_Scenario" & CurrentScenario & ".csv"
                Case Else
                    strEffortFile = "RelEffort_Country_Fleet.csv"
            End Select

            Return Path.Combine(Me.DataPath, strEffortFile)
        End Get
    End Property

    Public ReadOnly Property LMECellsFile As String
        Get
            Return Path.Combine(Me.DataPath, "LMECells_OneDegree.csv")
        End Get
    End Property

    Public Property UseSailCostPlugin As Boolean
        Get
            Return Me.m_bUseSailCostPlugin And IsInputdataValid
        End Get
        Set(ByVal value As Boolean)
            If (value <> Me.m_bUseSailCostPlugin) Then
                Me.m_bUseSailCostPlugin = value
                My.Settings.UseSailCostPlugin = Me.m_bUseSailCostPlugin
                My.Settings.Save()
            End If
        End Set
    End Property

    Public Property DataPath As String
        Get
            Return Me.m_strDataPath
        End Get
        Set(ByVal value As String)
            If (value <> Me.m_strDataPath) Then
                Me.m_strDataPath = value
                My.Settings.DataPath = Me.m_strDataPath
                My.Settings.Save()
            End If
        End Set
    End Property

#End Region

#Region "Private Methods"

    Private Function GetUI() As frmSailCost
        Dim bHasUI As Boolean = False

        If (Me.m_frmUI IsNot Nothing) Then
            bHasUI = Not Me.m_frmUI.IsDisposed
        End If

        If Not bHasUI Then
            Me.m_frmUI = New frmSailCost(Me, Me.m_uic)
            Me.m_frmUI.UIContext = Me.m_uic
        End If

        Return Me.m_frmUI

    End Function

    Private Sub FireOnChanged()
        Try
            RaiseEvent OnChanged()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SendCoreMessage(ByVal msg As String)
        Try
            Me.m_core.Messages.SendMessage(New cMessage(msg, eMessageType.Any, eCoreComponentType.EcoSpace, eMessageImportance.Warning))
        Catch ex As Exception

        End Try
    End Sub

    Private Sub CellsInLMEs()

        Dim sInputFile As String = Me.LMECellsFile()
        ' Dim TotalArea As Double = 363567776    'km2

        'zero LME contains all the water cells at the start
        'then each time a cell is assigned it is removed from the zero LME
        'nCellsInLME(0) += ds.nWaterCells

        ReDim CellListForAllLMEs(m_ecospacedata.InCol * m_ecospacedata.InRow)
        ReDim CellListForEachLME(66, m_ecospacedata.InCol * m_ecospacedata.InRow)
        nCellsInLMETotal = 0

        Using sr As StreamReader = New StreamReader(sInputFile)
            'Read headings:
            Dim line As String = sr.ReadLine()
            Do
                'LME	ROW	COL	AREA_KM2
                '64	    1	1	94.40899754

                line = sr.ReadLine()
                If line Is Nothing Then

                Else  'file has no blank values
                    Dim colVal As String() = line.Split(CChar(","))
                    If colVal(0) <> "" And colVal(1) <> "" And colVal(2) <> "" And colVal(3) <> "" Then
                        Dim iLME As Integer = CInt(colVal(0))
                        Dim ir As Integer = CInt(colVal(1))
                        Dim ic As Integer = CInt(colVal(2))
                        Dim area As Single = CSng(colVal(3))
                        If m_ecospacedata.Depth(ir, ic) > 0 Then
                            m_ecospacedata.EffZones(ir, ic) = iLME
                            LMEarea(iLME) += area
                            nCellsInLME(iLME) += 1
                            If iLME > 0 Then 'only for real LMEs, not offshore
                                nCellsInLMETotal += 1
                                'Get the sequence number and store it:
                                CellListForAllLMEs(nCellsInLMETotal) = (ir - 1) * m_ecospacedata.InCol + ic
                                CellListForEachLME(iLME, nCellsInLME(iLME)) = (ir - 1) * m_ecospacedata.InCol + ic
                            End If
                            'for every cell that has an LME remove it from the zero LME count
                            'nCellsInLME(0) -= 1
                        End If
                        'SumArea += area
                    End If
                End If
            Loop Until (line Is Nothing)
        End Using

        'Set area for open ocean:
        'LMEarea(0) = TotalArea - SumArea
    End Sub

    Private Sub EffortCSVfileReading()

        Dim sInputFile As String = Me.EffortFile()
        Using sr As StreamReader = New StreamReader(sInputFile)
            'Read headings:
            Dim line As String = sr.ReadLine()
            Do
                line = sr.ReadLine()
                If line Is Nothing Then

                Else  'file has no blank values
                    'Country	 FleetNo	1950	1951	1952
                    '76	1	0	0.1555914	0.157945
                    Dim colVal As String() = line.Split(CChar(","))
                    Dim iLME As Integer = CInt(colVal(0))
                    Dim iFl As Integer = CInt(colVal(1))
                    If iFl <= 11 Then
                        For i As Integer = 1 To m_nYears   '=2006 - 1949
                            RelEffortLME(iLME, iFl, i) = CSng(colVal(i + 1))
                        Next
                    End If
                End If
            Loop Until (line Is Nothing)
        End Using

    End Sub

    Private Sub InitAreaFishedLME()

        AreaFishedLME = New Single(66, Me.m_ecopathdata.NumFleet) {}

        Dim ilme As Integer
        For irow As Integer = 1 To Me.m_ecospacedata.InRow
            For icol As Integer = 1 To Me.m_ecospacedata.InCol
                'LME of this cell
                ilme = m_ecospacedata.EffZones(irow, icol)

                'this assumes that ALL cells that are land or are excluded 
                'will have percentage of area fished = 0 
                For iflt As Integer = 1 To Me.m_ecopathdata.NumFleet
                    'Sum of the area fished for each LME, Fleet
                    AreaFishedLME(ilme, iflt) += Me.m_ecospacedata.PAreaFished(iflt)(irow, icol)
                Next
            Next
        Next

    End Sub


#End Region ' Private modules

#Region " Plugin Events "

    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        Try

            Me.m_core = DirectCast(core, cCore)
            Me.m_strDataPath = My.Settings.DataPath
            Me.m_bUseSailCostPlugin = My.Settings.UseSailCostPlugin

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub


    Public Sub EcospaceInitRunCompleted(ByVal EcospaceDatastructures As Object) Implements EwEPlugin.IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted
        Try
            If Not Me.UseSailCostPlugin Then Return

            Dim iTs As Integer = m_core.ActiveTimeSeriesDatasetIndex
            Dim CurrentScenario As Integer = iTs
            MsgBox("Running future scenario no " & CurrentScenario & ". Change scenario by changing the time series (this is used in SailCost to set MPAs)")

            m_ecospacedata = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)

            m_ecospacedata.ReDimEffortZones(66)

            ReDim LMEarea(66)
            ReDim nCellsInLME(66)
            ReDim RelEffortLME(1200, m_nFleets, m_nYears)  '(countries, fleets, years)
            ReDim EffortbyFleetLMEYear(m_nFleets, 66, m_nYears)

            CellsInLMEs()

            'Next reading the file RelEffort_Country_Fleet.csv with relative effort by LME, fleet, year:


            EffortCSVfileReading()

            InitAreaFishedLME()
            Me.m_lastYear = -1

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub


    Public Sub EcopathRunInitialized(ByVal EcopathDataAsObject As Object, ByVal TaxonDataAsObject As Object, ByVal StanzaDataAsObject As Object) Implements EwEPlugin.IEcopathRunInitializedPlugin.EcopathRunInitialized
        m_ecopathdata = DirectCast(EcopathDataAsObject, cEcopathDataStructures)
    End Sub

    Public Sub EcospaceBeginTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep
        Try
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Test scaling of biomass back to Ecopath levels.
            'This does not really work. Once the biomass forcing is stopped it will return to levels it would have reached under normal running condition
            'Me.ScaleBiomassToEcopathBase(DirectCast(EcospaceDatastructures, cEcospaceDataStructures), iTime)
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            If Not Me.UseSailCostPlugin Then
                Exit Sub
            End If

            'Only load the data when the year has changed
            'This let the core account for running a different number of timesteps per year
            If m_lastYear <> m_ecospacedata.YearNow Then
                Me.m_lastYear = m_ecospacedata.YearNow
                Dim iYr As Integer = m_ecospacedata.YearNow ' iTime / 12 + 1
                'Repeat the last year if we have run pass the end of the data
                'If iYr > noYears Then iYr = noYears

                Dim AnnualIncrease As Double = 1.02
                Dim EffortCreep As Double = 1
                If iYr < 57 Then
                    EffortCreep = AnnualIncrease ^ (iYr - 1)
                Else
                    EffortCreep = AnnualIncrease ^ (56)
                End If

                If m_ecospacedata.YearNow = 1 Or m_ecospacedata.YearNow = 40 Or m_ecospacedata.YearNow = 57 Then SetCellsAsLMEs(m_ecospacedata.YearNow)

                For iFl As Integer = 1 To 11    'not for tuna  'noFleets
                    For iLME As Integer = 0 To 66
                        'If LME(iR, iC) = 22 Then Stop
                        'ds.RelEffort(iFl, iR, iC) = RelEffortLME(ds.EffZones(iR, iC) + 1000, iFl, iYr) '/ LMEarea(LME(iR, iC)) * SumArea  'iCo, iFl, iYr
                        m_ecospacedata.PropEffortFleetZone(iFl, iLME) = CSng(RelEffortLME(iLME + 1000, iFl, iYr) * EffortCreep)
                        'Sum up total effort
                        EffortbyFleetLMEYear(iFl, iLME, iYr) += m_ecospacedata.PropEffortFleetZone(iFl, iLME)
                    Next
                Next
                For iFl As Integer = 12 To 14    'for tuna  'noFleets
                    For iLME As Integer = 0 To 66
                        'for the fleets that can fish in all LMEs
                        'Scale the proportion of effort relative to the total fished area 
                        'This is so the effort distribution does not concentrate effort in smaller areas 
                        'In this case PropEffortFleetZone() prop area fished 

                        'jb 1-Nov-2013 nCellsInLME() is the total number of cells in an LME 
                        'this does not guarantee that a fleet fished the LME so this can give the wrong proportion
                        'ds.PropEffortFleetZone(iFl, iLME) = EffortCreep * (nCellsInLME(iLME) / ds.TotEffort(iFl))

                        'AreaFishedLME(LME,fleet) is the area in an LME that the fleet actually fished
                        'This should proportion the effort evenly across the LME's
                        m_ecospacedata.PropEffortFleetZone(iFl, iLME) = (Me.AreaFishedLME(iLME, iFl) / m_ecospacedata.TotEffort(iFl)) * EffortCreep
                        'Sum up total effort
                        EffortbyFleetLMEYear(iFl, iLME, iYr) += m_ecospacedata.PropEffortFleetZone(iFl, iLME)
                    Next
                Next
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Private Sub SetCellsAsLMEs(ByVal iYr As Integer)
        'ds is the ecospace datastructure

        If iYr = 1 Then 'see all cells to not being LMEs
            ResetAllMPAs()
        ElseIf iYr = 40 Then    'set 5% of area of all LMEs to protected (don't worry about where in the LMEs it is)
            'total number of cells in all LMEs is nCellInLMEsTotal
            Dim nMPACellsinLMEs As Integer = nCellsInLMETotal / 20
            SelectRandomCellsForMPAs(nCellsInLMETotal, nMPACellsinLMEs, CellListForAllLMEs)
        ElseIf iYr = 57 Then        'Set the cells to scenario specific LMEs
            'what timeseries are we using for Ecosim?
            Dim iTs As Integer = m_core.ActiveTimeSeriesDatasetIndex  '1-based index of which time series is loaded
            'MsgBox(m_core.TimeSeriesDataset(1).Name)
            Debug.Print(m_core.TimeSeriesDataset(1).Name)
            Select Case iTs
                Case 1  'Baseline no changes
                Case 2  'Global Technology 10% overall
                    ResetAllMPAs()
                    Dim nMPACellsinLMEs As Integer = nCellsInLMETotal / 10
                    SelectRandomCellsForMPAs(nCellsInLMETotal, nMPACellsinLMEs, CellListForAllLMEs)
                Case 3, 4   'Decentralized solutions 10% in each LME
                    'Case 4  'Consumption change 10% in each LME
                    ResetAllMPAs()
                    selectRandomMPAsForEachLME(10)
            End Select

        End If


    End Sub

    Private Sub ResetAllMPAs()
        For irow As Integer = 1 To Me.m_ecospacedata.InRow
            For icol As Integer = 1 To Me.m_ecospacedata.InCol
                For iMPA As Integer = 1 To Me.m_ecospacedata.MPAno
                    m_ecospacedata.MPA(iMPA)(irow, icol) = 0
                Next
            Next
        Next
    End Sub

    Private Sub selectRandomMPAsForEachLME(ByVal PctMPA As Integer)
        For iLME As Integer = 1 To 66
            'nCellsInLME(iLME)
            'nCellsInLMETotal += 1
            ''Get the sequence number and store it:
            'CellListForAllLMEs(nCellsInLMETotal) = (ir - 1) * ds.InCol + ic
            'CellListForEachLME(iLME, nCellsInLME(iLME)) = (ir - 1) * ds.InCol + ic
            Dim LMECellSequence(nCellsInLME(iLME)) As Integer
            'we need to store the cell sequence in a one-dimensional array:
            For iCl As Integer = 1 To nCellsInLME(iLME)
                LMECellSequence(iCl) = CellListForEachLME(iLME, iCl)
            Next
            Dim nMPAcells As Integer = nCellsInLME(iLME) * PctMPA / 100
            'Now have all we need to call randomlmegenerator
            SelectRandomCellsForMPAs(nCellsInLME(iLME), nMPAcells, LMECellSequence)
        Next

    End Sub

    Private Sub SelectRandomCellsForMPAs(ByVal nCellTotal As Integer, ByVal NoOfCellsAsLMEs As Integer, ByVal CellList() As Integer)

        Dim numbers = Enumerable.Range(1, nCellTotal).ToList()
        Dim RandomClass As New Random()
        Dim RandomIndex As Integer
        For counter As Integer = 1 To NoOfCellsAsLMEs    'that's how many MPA cells we want 
            RandomIndex = RandomClass.Next(0, numbers.Count)
            'selected number is: numbers(RandomIndex)
            'the corresponding sequence is
            Dim iSeq As Integer = CellList(numbers(RandomIndex))
            'Now set the corresponding cell to being a MPA:
            'col =  (CC - 1) Mod 720 + 1
            'row = (CC - 1) \ 720 + 1
            Dim iR As Integer = (iSeq - 1) \ m_ecospacedata.InCol + 1
            Dim iC As Integer = (iSeq - 1) Mod m_ecospacedata.InCol + 1
            'ds.MPA(iR, iC) = 1
            Debug.Assert(False, "This code needs reviewing because of overlapping MPA changes.")
            numbers.RemoveAt(RandomIndex)
        Next

    End Sub

    Public Sub EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

    End Sub

    Public Sub EcospaceRunCompleted(ByVal EcoSpaceDatastructures As Object) Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        If Not Me.UseSailCostPlugin Then
            Return
        End If

        Try
            Dim TimeSeriesWriter As New cEcospaceTimeSeriesWriter
            Dim mortFileName As String = Path.Combine(Me.DataPath, "TotalMortality.csv")
            Dim spacedata As EwECore.cEcospaceDataStructures = DirectCast(EcoSpaceDatastructures, cEcospaceDataStructures)

            TimeSeriesWriter.Init(mortFileName, Me.m_core, Me.m_ecopathdata, spacedata)
            TimeSeriesWriter.Write()

            For iX As Integer = 1 To 3

                Dim iyr As Integer = 1
                Select Case iX
                    Case 1 : iyr = 1
                    Case 2 : iyr = 57
                    Case 3 : iyr = 100
                End Select

                Dim fnam As String = "TotalEffortYear" & iyr & ".csv"
                Dim FileN As String = Path.Combine(Me.DataPath, fnam)
                Using sw As StreamWriter = New StreamWriter(FileN, False)  'true makes it append
                    Dim sInfo As String = "LME\Fleet"
                    For iFlt As Integer = 0 To m_nFleets
                        sInfo += "," & iFlt
                    Next
                    sw.WriteLine(sInfo)

                    For iLME As Integer = 1 To 66
                        sInfo = iLME.ToString
                        For iFlt As Integer = 0 To m_nFleets
                            sInfo += "," & EffortbyFleetLMEYear(iFlt, iLME, iyr)
                        Next
                        sw.WriteLine(sInfo)
                    Next
                    sw.Close()
                End Using

            Next

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

#End Region ' Plugin Events

#Region " Plugin misc "

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "GOM"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "Ecobio@globaloceanmodeling.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "GOM_LME_Effort"
        End Get
    End Property


    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "GOM effort by LME."
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcospaceInitialized
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI
    End Sub

    Public ReadOnly Property NavigationTreeItemLocation As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceTools"
        End Get
    End Property

    Public Sub UIContext(uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

#End Region ' Plugin misc

#Region "Test Scale Ecospace Biomass to Ecopath Levels"


    ''' <summary>
    ''' Scale Ecospace Biomass back to Ecopath Base Values
    ''' </summary>
    ''' <param name="spaceData"></param>
    ''' <param name="iTime"></param>
    ''' <remarks>
    ''' This will only scale the Ecospace biomass as long as it is running. 
    ''' Once it stops the biomass will be redistributed by the constraints of the capacity and effort distrubution models.
    ''' </remarks>
    Private Sub ScaleBiomassToEcopathBase(spaceData As cEcospaceDataStructures, iTime As Integer)
        Dim bsum As Single
        Dim nBs As Integer

        'Only for the first 5 years
        If spaceData.TimeNow > 5.0 Then Exit Sub
        Dim bScalar(Me.m_core.nGroups) As Single

        For igrp As Integer = 1 To Me.m_core.nGroups
            bsum = 0
            nBs = 0
            'Find the average biomass predicated by Ecospace across all the water cells
            For ir As Integer = 1 To spaceData.InRow
                For ic As Integer = 1 To spaceData.InCol

                    If spaceData.Depth(ir, ic) > 0 Then
                        bsum += spaceData.Bcell(ir, ic, igrp)
                        nBs += 1
                    End If

                Next ic
            Next ir

            'Now get the value used to scale the Ecospace biomass to Ecopath levels 
            bScalar(igrp) = 1
            If nBs > 0 Then
                bScalar(igrp) = Me.m_ecopathdata.B(igrp) / (bsum / nBs)
            End If

            'Scale Ecospace biomass back to Ecopath levels
            For ir As Integer = 1 To spaceData.InRow
                For ic As Integer = 1 To spaceData.InCol
                    If spaceData.Depth(ir, ic) > 0 Then
                        spaceData.Bcell(ir, ic, igrp) = spaceData.Bcell(ir, ic, igrp) * bScalar(igrp)
                    End If
                Next ic
            Next ir

            System.Console.WriteLine("Scalar for group " + igrp.ToString + " = " + bScalar(igrp).ToString)
        Next igrp
    End Sub

#End Region

End Class
