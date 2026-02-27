' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Explicit On
Option Strict On
Imports System.IO
Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.Ecopath
Imports EwECore.Plugins.Ecospace
Imports EwECore.Plugins.UI
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' <summary>
''' This plug-in builds upon the earlier effort distribution plug-ins. Effort totals,
''' per fleet, are delivered by Ecosim fishing effort. Effort can be set by LME or
''' by EEZ.
''' </summary>
Public Class cRegionalEffortPlugin
    Implements IUIContextPlugin
    Implements INavigationTreeItemPlugin
    Implements IEcospaceInitializedPlugin
    Implements IEcopathRunInitializedPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceBeginTimestepPlugin
    Implements IEcospaceEndTimestepPlugin
    Implements IEcospaceRunCompletedPlugin
    Implements IAutoRunPlugin

    Public Const PluginName As String = "zzRegionalEffort"

#Region " Private variables "

    Private m_core As cCore = Nothing
    Private m_ecospacedata As cEcospaceDataStructures = Nothing
    Private m_ecopathdata As cEcopathDataStructures = Nothing
    Private m_ecosim As cEcoSimScenario = Nothing

    Private m_uic As cUIContext = Nothing
    Private m_frmUI As frmUI = Nothing

    Private m_strEffortFile As String = ""
    Private m_iPrevYearApplied As Integer = 0

    ''' <summary>Max. years of effort data</summary>
    Private m_nMaxEffortYear As Integer = 0

    Private m_catchwriterEcoOcean As cEcospaceCatchTimeSeriesWriter = Nothing

    ''' <summary>Relative effort by zone, fleet, year</summary>
    Private m_RelEffort(,,) As Single

    ''' <summary>Area fished by zone</summary>
    Private m_AreaFished() As Single

    ''' <summary>Total relative effort by fleet, year</summary>
    Private m_TotEffort(,) As Single

#End Region

#Region " Events "

    Friend Event OnChanged()

#End Region ' Events

#Region " Public bits "

    Public ReadOnly Property IsInputdataValid() As Boolean
        Get
            If (Me.m_core.DataSource Is Nothing) Then Return False
            Dim eff As String = Me.EffortFileName
            Dim bHasEff As Boolean = File.Exists(eff)
            Dim bHasArea As Boolean = (Me.NumZones > 0)

            Return bHasEff And bHasArea
        End Get
    End Property

    Public Property EffortFileName As String
        Get
            Return Me.m_strEffortFile
        End Get
        Set(value As String)
            Me.m_strEffortFile = value
            My.Settings.LastEffortFile = value
            My.Settings.Save()
        End Set
    End Property

    Public Property EffortZoneName As String = "LME"

    'Public Property CustomAreaCatchTimeSeriesFile As String = ""

    Public ReadOnly Property AreaCatchTimeSeriesFile(format As String) As String
        Get
            'If Not String.IsNullOrWhiteSpace(CustomAreaCatchTimeSeriesFile) Then Return Me.CustomAreaCatchTimeSeriesFile

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim sz As Single = CSng(Math.Round(bm.CellSize, 3))
            Dim res As String = "?"

            If sz = 1.0 Then
                res = "60arcmin"
            ElseIf sz = 0.25 Then
                res = "15ArcMin"
            Else
                res = cStringUtils.FormatSingle(sz, iNumDigits:=3)
            End If

            Return Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace), cFileUtils.ToValidFileName("AreaCatch_" & Me.EffortZoneName & "_" & res & "_" & format & ".csv", False))
        End Get
    End Property

    Public ReadOnly Property TotalFishingMortalityFile As String
        Get
            Return Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace), "TotalFishingMortailities.csv")
        End Get
    End Property

    Public ReadOnly Property ZoneMapFile As String
        Get
            Return Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace), "EffectiveEffortZones.asc")
        End Get
    End Property

    Public ReadOnly Property NumZones As Integer
        Get
            Return Me.m_ecospacedata.nEffZones
        End Get
    End Property

    Public Property Enabled As Boolean = True
    Public Property WriteCatcheTimeSeries As Boolean = False
    Public Property WriteMortalitiesTimeSeries As Boolean = False
    Public Property WriteEffortTimeSeries As Boolean = False

    ''' <summary>
    ''' Get/set whether the total effort for a single fleet, across all zones, should be scaled to 1.
    ''' </summary>
    Public Property NormalizeEffort As Boolean = True

    Public Function OverwriteEffort() As Boolean
        Return Me.Enabled And Me.IsInputdataValid()
    End Function

    Private Property IsRunning As Boolean = False

    Public ReadOnly Property OutputPath As String
        Get
            Return Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace)
        End Get
    End Property

    Public Function LoadZones(fnMapZones As String) As Boolean

        ' Clear
        Array.Clear(Me.m_ecospacedata.EffZones, 0, Me.m_ecospacedata.EffZones.Length)

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap

        Dim mapZones As New cMap(fnMapZones)
        Dim nMaxZone As Integer = CInt(mapZones.Max)

        For ir As Integer = 1 To bm.InRow
            For ic As Integer = 1 To bm.InCol
                If bm.IsModelledCell(ir, ic) Then
                    Me.m_ecospacedata.EffZones(ir, ic) = CInt(mapZones(ic - 1, ir - 1))
                End If
            Next ic
        Next ir

        Me.m_ecospacedata.nEffZones = nMaxZone
        Me.m_ecospacedata.ReDimEffortZones()

        ReDim Me.m_AreaFished(nMaxZone)
        Return True

    End Function

    ''' <summary>
    ''' Load effort zone and cell area CSV file.
    ''' </summary>
    ''' <returns>True if successful</returns>
    Public Function LoadZonesAndCellAreas(fn As String) As Boolean

        Dim nMaxZone As Integer = 0

        ' Clear
        Array.Clear(Me.m_ecospacedata.EffZones, 0, Me.m_ecospacedata.EffZones.Length)

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap

        ' Clear zone map and cell areas
        For ir As Integer = 1 To bm.InRow
            For ic As Integer = 1 To bm.InCol
                If bm.IsModelledCell(ir, ic) Then
                    Me.m_ecospacedata.EffZones(ir, ic) = 0
                    Me.m_ecospacedata.CellArea(ir, ic) = 1
                End If
            Next ic
        Next ir

        Try

            Using sr As New StreamReader(fn)

                'Skip header, presumed
                'LME	ROW	COL	AREA_KM2
                '64	    1	1	94.40899754
                Dim line As String = sr.ReadLine()
                Do
                    line = sr.ReadLine()
                    ' Is content?
                    If Not String.IsNullOrEmpty(line) Then
                        ' #Yes: split it

                        Dim colVal As String() = line.Split({","c}, StringSplitOptions.RemoveEmptyEntries)
                        ' Are all values present?
                        If (colVal.Length = 4) Then
                            ' #Yes: process this row
                            Dim iZone As Integer = CInt(colVal(0))
                            Dim ir As Integer = CInt(colVal(1))
                            Dim ic As Integer = CInt(colVal(2))
                            Dim area As Single = cStringUtils.ConvertToSingle(colVal(3), 0)

                            If (ir > 0 And ic > 0 And ir <= Me.m_ecospacedata.InRow And ic <= Me.m_ecospacedata.InCol) Then
                                If Me.m_ecospacedata.Depth(ir, ic) > 0 Then
                                    Me.m_ecospacedata.EffZones(ir, ic) = iZone
                                    nMaxZone = Math.Max(nMaxZone, iZone)

                                    For c As Integer = 1 To Me.m_ecospacedata.InCol
                                        Me.m_ecospacedata.CellArea(ir, c) = area
                                    Next
                                End If
                            End If
                        End If
                    End If
                Loop Until (line Is Nothing)
            End Using ' sr
        Catch ex As Exception
            Return False
        End Try

        Me.m_ecospacedata.nEffZones = nMaxZone
        Me.m_ecospacedata.ReDimEffortZones()

        ReDim Me.m_AreaFished(nMaxZone)

        Return True

    End Function

#End Region ' Public bits

#Region " Private Methods "

    Private Function HasUI() As Boolean
        If Me.m_frmUI Is Nothing Then Return False
        Return Not Me.m_frmUI.IsDisposed
    End Function

    Private Function GetUI() As frmUI

        If Not HasUI() Then
            Me.m_frmUI = New frmUI(Me, Me.m_uic)
            Me.m_frmUI.UIContext = Me.m_uic
        End If

        Return Me.m_frmUI

    End Function

    Public Sub Kick()
        Try
            RaiseEvent OnChanged()
            Console.WriteLine("Ouch!")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SendCoreMessage(msg As String, Optional importance As eMessageImportance = eMessageImportance.Warning, Optional hyperlink As String = "")
        Try
            Me.m_core.Messages.SendMessage(New cMessage(msg, eMessageType.Any, eCoreComponentType.Ecospace, importance, hyperlink:=hyperlink))
        Catch ex As Exception

        End Try
    End Sub

    Private Function ReadZoneEffort() As Boolean

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim strEffortFile As String = Me.EffortFileName()
        Dim iStartYear As Integer = Me.m_core.EcosimFirstYear

        ReDim m_RelEffort(Me.NumZones, Me.m_core.nFleets, Me.m_core.nEcospaceYears)
        ReDim m_TotEffort(Me.m_core.nFleets, Me.m_core.nEcospaceYears)

        Me.m_nMaxEffortYear = 0

        If Not File.Exists(strEffortFile) Then
            Me.SendCoreMessage(cStringUtils.Localize("Unable to find effort file {0}!", strEffortFile), eMessageImportance.Critical)
            Return False
        End If

        Try
            Dim n As Integer = 0

            'Country	 FleetNo	1950	1951	1952
            '76	1	0	0.1555914	0.157945
            Using sr As New StreamReader(strEffortFile)

                'Read headings:
                Dim line As String = sr.ReadLine()

                Dim colVal As String() = cStringUtils.SplitQualified(line, ","c)
                Dim years As New List(Of Integer)
                For i As Integer = 2 To colVal.Count - 1
                    years.Add(cStringUtils.ConvertToInteger(colVal(i)))
                Next

                Do
                    line = sr.ReadLine()
                    If line Is Nothing Then

                    Else  'file has no blank values
                        colVal = line.Split(","c)
                        Dim iZone As Integer = CInt(colVal(0))
                        Dim iFl As Integer = CInt(colVal(1))

                        If iZone >= 1000 Then iZone -= 1000

                        'Debug.Assert(iFl <= Me.m_ecopathdata.NumFleet)
                        For iCol As Integer = 0 To years.Count - 1
                            Dim val As Single = cStringUtils.ConvertToSingle(colVal(iCol + 2), 0)
                            Dim iYear As Integer = years(iCol)
                            If (iYear >= iStartYear) Then iYear = iYear - iStartYear + 1

                            If (val >= 0) And (iYear >= 0) And (iYear <= Me.m_core.nEcospaceYears) Then
                                If (iZone >= 0 And iZone < Me.m_ecospacedata.nEffZones) Then
                                    If (iFl > 0 And iFl <= Me.m_ecospacedata.nFleets) Then
                                        Me.m_RelEffort(iZone, iFl, iYear) += val
                                        Me.m_TotEffort(iFl, iYear) += val
                                        Me.m_nMaxEffortYear = Math.Max(Me.m_nMaxEffortYear, iYear)
                                        n += 1
                                    End If
                                End If
                            End If
                        Next
                    End If
                Loop Until (line Is Nothing)
                Me.SendCoreMessage(cStringUtils.Localize("Successfully read {0} non-zero records from effort file {1}", n, strEffortFile), eMessageImportance.Information)
            End Using

        Catch ex As Exception
            Me.SendCoreMessage(cStringUtils.Localize("Exception {0} reading effort file {1}!", ex.Message, strEffortFile), eMessageImportance.Critical)
            Return False
        End Try

        If WriteEffortTimeSeries Then
            Try
                Dim pout As String = Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace)
                Dim fout As String = Path.Combine(pout, "EwEDriveEffort_TotalEffortAnnual.csv")
                Using sw As New StreamWriter(fout)
                    sw.Write("Year")
                    For iFlt As Integer = 1 To Me.m_core.nFleets
                        sw.Write(",{0}", iFlt)
                    Next
                    sw.WriteLine()

                    For iYear As Integer = 1 To Me.m_core.nEcospaceYears
                        sw.Write(iYear)
                        For iFlt As Integer = 1 To Me.m_core.nFleets
                            sw.Write(",{0}", Me.m_TotEffort(iFlt, iYear))
                        Next iFlt
                        sw.WriteLine()
                    Next iYear
                End Using

                Me.SendCoreMessage(cStringUtils.Localize("Zonal effort totals written to {0}", fout), eMessageImportance.Information, pout)
            Catch ex As Exception
                Me.SendCoreMessage(cStringUtils.Localize("Exception {0} writing effort totals file", ex.Message), eMessageImportance.Critical)
            End Try
        End If

        Return True
    End Function

    Private Sub InitAreaFished()

        ReDim Me.m_AreaFished(Me.NumZones)

        For ir As Integer = 1 To Me.m_ecospacedata.InRow
            For ic As Integer = 1 To Me.m_ecospacedata.InCol
                If Me.m_ecospacedata.Depth(ir, ic) > 0 Then
                    Dim iZone As Integer = Me.m_ecospacedata.EffZones(ir, ic)
                    If (iZone >= 0) And (iZone < Me.NumZones) Then
                        'this assumes that ALL cells that are land or are excluded 
                        'will have percentage of area fished = 0 
                        For iflt As Integer = 1 To Me.m_ecopathdata.NumFleet
                            'Sum of the area fished for each LME, Fleet
                            Me.m_AreaFished(iZone) += Me.m_ecospacedata.PAreaFished(iflt)(ir, ic)
                        Next
                    End If
                End If
            Next
        Next

    End Sub

    Private Sub SendMessage(text As String, bOK As Boolean, Optional hyperlink As String = "")
        text = My.Resources.CAPTION & ": " & text
        Dim msg As New cMessage(text, eMessageType.DataExport, eCoreComponentType.Ecospace,
                                If(bOK, eMessageImportance.Information, eMessageImportance.Critical),
                                hyperlink:=hyperlink)
        Me.m_core.Messages.SendMessage(msg)
    End Sub

#End Region ' Private Methods

#Region " Plugin Events "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
            Me.Enabled = True
            Me.m_ecospacedata = Me.m_core.m_EcospaceData

            Me.m_strEffortFile = My.Settings.LastEffortFile
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
    End Sub

    Public Sub EcopathRunInitialized(EcopathDataAsObject As Object, TaxonDataAsObject As Object, StanzaDataAsObject As Object) Implements IEcopathRunInitializedPlugin.EcopathRunInitialized

        Me.m_ecopathdata = DirectCast(EcopathDataAsObject, cEcopathDataStructures)
        Me.m_nMaxEffortYear = 0

    End Sub

    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) Implements IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted
        Try
            Me.IsRunning = Me.OverwriteEffort()
            If Not Me.IsRunning Then Return

            ' Read accompanying effort
            Me.IsRunning = Me.ReadZoneEffort()
            If Not Me.IsRunning Then Return

            Me.InitAreaFished()
            Me.m_iPrevYearApplied = -1

            If Me.WriteCatcheTimeSeries Then
                Me.m_catchwriterEcoOcean = New cEcospaceCatchTimeSeriesWriter()
                Me.m_catchwriterEcoOcean.Init(Me.m_core)
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Allocate proportional effort to each Ecospace effort zone
    ''' </summary>
    ''' <param name="EcospaceDatastructures"></param>
    ''' <param name="iTime"></param>
    Public Sub EcospaceBeginTimeStep(EcospaceDatastructures As Object, iTime As Integer) _
        Implements IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep

        If Not Me.IsRunning Then Return

        Try
            Dim iStartYear As Integer = Me.m_core.EcosimFirstYear
            Dim dt As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(1)
            Dim iYearOffset As Integer = dt.Year - iStartYear
            Dim iYear As Integer = Me.m_ecospacedata.YearNow

            If (iYear >= iStartYear) Then
                iYear = iYear - iStartYear + 1
            End If

            If (iYear > Me.m_nMaxEffortYear) Then Return

            'Only load the data when the year has changed to allow for different number of time steps per year
            If Me.m_iPrevYearApplied <> iYear Then
                Me.m_iPrevYearApplied = iYear

                For iFlt As Integer = 1 To Me.m_ecopathdata.NumFleet
                    Dim fleetScalar As Single = GetFleetScalar(iFlt, iYear)
                    For iZone As Integer = 0 To Me.NumZones
                        Me.m_ecospacedata.PropEffortFleetZone(iFlt, iZone) = Me.m_RelEffort(iZone, iFlt, iYear) / fleetScalar
                    Next iZone
                Next iFlt

                Me.SendCoreMessage(cStringUtils.Localize("Time step {0} applied zonal effort for {1}",
                                                         iTime, iYear + iYearOffset), eMessageImportance.Information)

            End If
        Catch ex As Exception
            Me.SendCoreMessage(cStringUtils.Localize("Exception {0} applying zonal effort at time step {1}", ex, iTime), eMessageImportance.Critical)
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Function GetFleetScalar(iflt As Integer, iYear As Integer) As Single

        ' Do not calculate a scaling factor if not explicitly requested
        If Not Me.NormalizeEffort Then Return 1

        Dim sumFleet As Single = 0
        For iZone As Integer = 0 To Me.NumZones
            sumFleet += Me.m_RelEffort(iZone, iflt, iYear)
        Next iZone
        ' Avoid divisions by zero
        If (sumFleet = 0) Then sumFleet = 1
        Return sumFleet

    End Function

    Public Sub EcospaceEndTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

        If Not Me.IsRunning Then Return
        If Me.m_ecospacedata.bInSpinUp Then Return

        Dim sTimeStepToYear As Single = Me.m_ecospacedata.nTimeStepsPerYear * Me.m_ecopathdata.NumYears

        If (Me.WriteCatcheTimeSeries) Then
            For ir As Integer = 1 To Me.m_ecospacedata.InRow
                For ic As Integer = 1 To Me.m_ecospacedata.InCol
                    If Me.m_ecospacedata.Depth(ir, ic) > 0 Then
                        Dim iZone As Integer = Me.m_ecospacedata.EffZones(ir, ic)
                        For igrp As Integer = 1 To Me.m_ecopathdata.NumGroups
                            If Me.m_ecospacedata.CatchMap(ir, ic, igrp) > 0 Then

                                Dim cellcatch As Double = Me.m_ecospacedata.CatchMap(ir, ic, igrp) * Me.m_ecospacedata.CellArea(ir, ic) / sTimeStepToYear

                                Me.m_catchwriterEcoOcean.AddCatch("all", "all", Me.m_ecospacedata.YearNow, cellcatch)
                                Me.m_catchwriterEcoOcean.AddCatch("all", igrp, Me.m_ecospacedata.YearNow, cellcatch)
                                Me.m_catchwriterEcoOcean.AddCatch(iZone, "all", Me.m_ecospacedata.YearNow, cellcatch)
                                Me.m_catchwriterEcoOcean.AddCatch(iZone, igrp, Me.m_ecospacedata.YearNow, cellcatch)

                            End If
                        Next
                    End If
                Next
            Next
        End If

    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        If Not Me.IsRunning Then Return

        Dim bSuccess As Boolean = True

        If Me.WriteCatcheTimeSeries Then
            Dim fn As String = Me.AreaCatchTimeSeriesFile("EcoOcean")
            cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(fn), True)

            Try
                bSuccess = Me.m_catchwriterEcoOcean.Write(fn)
            Catch ex As Exception
                bSuccess = False
            End Try

            If bSuccess Then
                Me.SendMessage(My.Resources.PROMPT_CATCHWRITE_SUCCESS, bSuccess, Path.GetDirectoryName(fn))
            Else
                Me.SendMessage(My.Resources.PROMPT_CATCHWRITE_ERROR, bSuccess)
            End If
        End If

        If Me.WriteMortalitiesTimeSeries Then
            Dim fn As String = Me.TotalFishingMortalityFile
            cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(fn), True)
            Try
                Dim TimeSeriesWriter As New cEcospaceMortalitiesTimeSeriesWriter()
                TimeSeriesWriter.Init(fn, Me.m_core, Me.m_ecopathdata, Me.m_ecospacedata)
                bSuccess = TimeSeriesWriter.Write()

            Catch ex As Exception
                bSuccess = False
            End Try

            If bSuccess Then
                Me.SendMessage(My.Resources.PROMPT_MORTWRITE_SUCCESS, bSuccess, Path.GetDirectoryName(fn))
            Else
                Me.SendMessage(My.Resources.PROMPT_MORTWRITE_ERROR, bSuccess)
            End If
        End If

        If Me.WriteEffortTimeSeries Then
            Try

                bSuccess = True

                For iyr As Integer = 1 To Me.m_core.nEcospaceYears Step 10

                    Dim fn As String = Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace), "TotalEffortYear_" & (Me.m_core.EcosimFirstYear + iyr - 1) & ".csv")
                    cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(fn), True)

                    Using sw As StreamWriter = New StreamWriter(fn, False)

                        ' Header
                        sw.Write("Area\Fleet")
                        For iFlt As Integer = 0 To Me.m_core.nFleets
                            sw.Write(",")
                            sw.Write(iFlt)
                        Next
                        sw.WriteLine()

                        ' Rows
                        For iZone As Integer = 0 To Me.NumZones
                            sw.Write(iZone)
                            For iFlt As Integer = 0 To Me.m_core.nFleets
                                sw.Write(",")
                                sw.Write(cStringUtils.ToCSVField(Me.m_RelEffort(iZone, iFlt, iyr)))
                            Next
                            sw.WriteLine()
                        Next

                        sw.Flush()
                        sw.Close()

                    End Using
                Next

            Catch ex As Exception
                bSuccess = False
            End Try

            If bSuccess Then
                Me.SendMessage(My.Resources.PROMPT_EFFORTWRITE_SUCCESS, bSuccess, Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace))
            Else
                Me.SendMessage(My.Resources.PROMPT_EFFORTWRITE_ERROR, bSuccess)
            End If

        End If

    End Sub

#End Region ' Plugin Events

#Region " Plugin Autorun "

    Public Property AutoRun(type As eCoreComponentType) As Boolean Implements IAutoRunPlugin.AutoRun
        Get
            Return Me.Enabled
        End Get
        Set(value As Boolean)
            Me.Enabled = value
        End Set
    End Property

    Public Function AutoRunTypes() As eCoreComponentType() Implements IAutoRunPlugin.AutoRunTypes
        Return New eCoreComponentType() {eCoreComponentType.Ecospace}
    End Function

#End Region ' Plugin Autorun

#Region " Plugin misc "

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EII"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "dev@ecopathinternational.org"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return PluginName
        End Get
    End Property

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return ScientificInterfaceShared.My.Resources.nav_input
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IGUIPlugin.DisplayName
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceInitialized
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI
    End Sub

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceInput\ndEcospaceFishery"
        End Get
    End Property

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

#End Region ' Plugin misc

End Class
