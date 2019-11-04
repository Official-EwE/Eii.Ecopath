#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.SpatialData
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEEcologicalIndicatorsPlugin
Imports EwEEcospaceSpinupPlugin
Imports EwEMPADynamicsPlugin
Imports System.IO

#End Region ' Imports

Public Class cEngine

    Public ReadOnly Property Environment As New cEnvironment()
    Public ReadOnly Property Runs As New List(Of cRun)
    Public Property Core As cCore = Nothing

    Public Sub New()
        ' NOP
    End Sub

    Public Sub Add(run As cRun)
        Me.Runs.Add(run)
    End Sub

    Public Function Validate() As Boolean
        Dim bSuccess As Boolean = True
        For Each r As cRun In Runs
            bSuccess = bSuccess And Validate(r)
        Next
        Return bSuccess

    End Function

    Public Function Run() As Boolean

        Dim bSuccess As Boolean = True

        For Each r As cRun In Runs

            Console.WriteLine("Starting run " & r.Name
                                  )
            Core = New cCore()
            Core.PluginManager = New cPluginManager()
            Core.PluginManager.LoadPlugins()
            Core.SaveWithFileHeader = False
            Core.OutputPath = Path.Combine(Environment.OutputPath, r.Name)

            Console.Write("Loading model: ")

            If Core.LoadModel(r.Model) Then

                Console.WriteLine("OK")

                Dim iSim As Integer = ResolveEcosimScenario(r.Ecosim.Scenario)
                Dim iTS As Integer = ResolveEcosimTimeSeries(r.Ecosim.TimeSeries)
                Dim iSpace As Integer = ResolveEcospaceScenario(r.Ecospace.Scenario)

                Console.Write("Loading Ecosim: ")

                If (r.Ecosim.RunYears > 0) Then
                    If Core.LoadEcosimScenario(iSim) Then

                        Console.WriteLine("OK")
                        Console.Write("Loading Ecosim timeseries: ")

                        If Core.LoadTimeSeries(iTS) Or (iTS <= 0) Then

                            Console.WriteLine(If(iTS <= 0, "SKIPPED", "OK"))

                            Dim SimParms As cEcoSimModelParameters = Core.EcoSimModelParameters
                            SimParms.NumberYears = r.Ecosim.RunYears

                            bSuccess = bSuccess And EcosimAutosave(r.Ecosim.AutosaveResults)
                            bSuccess = bSuccess And Core.RunEcoSim()

                            Dim dsman As cSpatialDataSetManager = Core.SpatialDatasetManager
                            If (Not String.IsNullOrWhiteSpace(r.Ecospace.ExternalDataConfigFile)) Then
                                dsman.Load(r.Ecospace.ExternalDataConfigFile)
                            End If

                            If (r.Ecospace.RunYears > 0) Then

                                Console.Write("Loading Ecospace: ")
                                If Core.LoadEcospaceScenario(iSpace) Then

                                    Console.WriteLine("OK")

                                    Dim SpaceParms As cEcospaceModelParameters = Core.EcospaceModelParameters
                                    SpaceParms.TotalTime = r.Ecospace.RunYears

                                    Console.WriteLine("No. threads: {0}, solver: {1}, effort dist: {2}", SpaceParms.nSpaceThreads, SpaceParms.nGridSolverThreads, SpaceParms.nEffortDistThreads)

                                    Console.Write("Configuring Ecospace plug-ins: ")
                                    bSuccess = bSuccess And ConfigEcospaceSpinup(r.Ecospace.SpinupYears)
                                    bSuccess = bSuccess And ConfigMPADynamics(r.MPADynamicsFile)
                                    bSuccess = bSuccess And ConfigEcoInd(r.EcoIndEnabled, eCoreComponentType.EcoSpace)
                                    Console.WriteLine("OK")

                                    Console.Write("Configuring Ecospace auto-saving: ")
                                    If r.Ecospace.AutosaveBiomassMaps Then bSuccess = bSuccess And EcospaceAutosave(GetType(cEcospaceASCMapBiomassWriter))
                                    If r.Ecospace.AutosaveCatchMaps Then bSuccess = bSuccess And EcospaceAutosave(GetType(cEcospaceASCMapCatchWriter))
                                    If r.Ecospace.AutosaveEffortMaps Then bSuccess = bSuccess And EcospaceAutosave(GetType(cEcospaceASCMapEffortWriter))
                                    Console.WriteLine("OK")

                                    Console.Write("Configuring Ecospace external data connections: ")
                                    For Each conn As cSpatTempConnection In r.Ecospace.Connections
                                        bSuccess = bSuccess And ConnectExternalData(conn)
                                    Next
                                    Console.WriteLine("OK")

                                    If (bSuccess) Then
                                        Console.Write("Running Ecospace: ")
                                        bSuccess = bSuccess And Core.RunEcoSpace(RunOnThread:=True)

                                        Dim timeLast As Double = Core.m_EcoSpaceData.TimeNow
                                        While Core.StateMonitor.IsBusy
                                            System.Threading.Thread.Sleep(5000)
                                            If (Core.m_EcoSpaceData.TimeNow <> timeLast) Then Console.Write(Math.Round(Core.m_EcoSpaceData.TimeNow, 2))
                                            timeLast = Core.m_EcoSpaceData.TimeNow
                                            Console.Write(".")
                                        End While
                                        Console.WriteLine("OK")
                                    End If

                                    Core.CloseEcospaceScenario()
                                Else
                                    Console.WriteLine("FAILED")
                                End If ' Core.LoadEcospaceScenario(iSpace)
                            Else
                                Console.WriteLine("Not specified")
                            End If ' run.Ecospace.RunYears > 0
                        Else
                            Console.WriteLine("FAILED")
                        End If ' Core.LoadTimeSeries(iTS) Or (iTS <= 0) 
                        Core.CloseEcosimScenario()
                    Else
                        Console.WriteLine("FAILED")
                    End If ' Core.LoadEcosimScenario(iSim)
                Else
                    Console.WriteLine("Not specified")
                End If ' run.Ecosim.RunYears > 0
                Core.CloseModel()
            Else
                Console.WriteLine("FAILED")
            End If

            Core.Dispose()
            Core = Nothing
            GC.Collect()
        Next
        Return bSuccess

    End Function

#Region " Internals "

#Region " Validation "

    Private Function Validate(run As cRun) As Boolean
        Dim bOK As Boolean = True
        If (Not File.Exists(run.Model)) Then Return False
        ' Etc
        Return bOK
    End Function

#End Region ' Validation 

#Region " Ecosim autosaving "

    Public Function EcosimAutosave(bAutosave As Boolean) As Boolean
        Core.Autosave(eAutosaveTypes.Ecosim) = bAutosave
        Core.Autosave(eAutosaveTypes.EcosimResults) = bAutosave
        Return (bAutosave = Core.Autosave(eAutosaveTypes.Ecosim)) And (bAutosave = Core.Autosave(eAutosaveTypes.EcosimResults))
    End Function

#End Region ' Ecosim autosaving 

#Region " Ecospace spinup "

    Public Function ConfigEcospaceSpinup(Optional nYears As Integer = 10) As Boolean

        Dim p As cEcospaceSpinupPlugin = CType(Core.PluginManager.GetPlugins(GetType(cEcospaceSpinupPlugin))(0), cEcospaceSpinupPlugin)
        p.AutoRun(eCoreComponentType.EcoSpace) = (nYears > 0)
        p.SpinUpYears = nYears

        Dim bSuccess As Boolean = (p.AutoRun(eCoreComponentType.EcoSpace) = (nYears > 0)) And (p.SpinUpYears = nYears)
        LogStep("Ecospace spinup configure", bSuccess)

        Return bSuccess

    End Function

#End Region ' Ecospace spinup

#Region " Ecological indicators (EcoIND) "

    Public Function ConfigEcoInd(bEnabled As Boolean, runmode As eCoreComponentType) As Boolean

        Dim p As cEwEEcologicalIndicatorsPlugin = CType(Core.PluginManager.GetPlugins(GetType(cEwEEcologicalIndicatorsPlugin))(0), cEwEEcologicalIndicatorsPlugin)
        p.AutoRun(runmode) = bEnabled
        p.AutoSave = bEnabled

        Dim bSuccess As Boolean = (p.AutoRun(runmode) = bEnabled) And (p.AutoSave = bEnabled)
        LogStep("EcoIND configure", bSuccess)

        Return bSuccess

    End Function

#End Region ' Ecological indicators (EcoIND) 

#Region " MPA Dynamics "

    ''' <summary>
    ''' Provide an empty file name to clear MPA dynamics
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Function ConfigMPADynamics(file As String) As Boolean

        ' Pre
        If (Not Core.StateMonitor.HasEcospaceLoaded) Then
            LogStep("Cannot configure MPADynamics, Ecospace not loaded yet", False)
            Return False
        End If

        Dim p As cEwEMPADynamicsPlugin = CType(Core.PluginManager.GetPlugins(GetType(cEwEMPADynamicsPlugin))(0), cEwEMPADynamicsPlugin)
        Dim bSuccess As Boolean = True
        If (String.IsNullOrWhiteSpace(file)) Then
            p.Engine.Clear()
        Else
            bSuccess = p.Engine.LoadCSV(file)
        End If

        LogStep(String.Format("MPADynamics configured, {0} entries", p.Engine.MPAStates(False).Count), bSuccess)
        Return True

    End Function

#End Region ' MPA Dynamics

#Region " Spat temp FW "

    Public Function ConnectExternalData(def As cSpatTempConnection) As Boolean

        ' Find adapter
        Dim man As cSpatialDataConnectionManager = Core.SpatialDataConnectionManager
        Dim adt As cSpatialDataAdapter = man.Adapter(def.VarName)
        If (adt Is Nothing) Then Return False

        ' Find layer index
        ' This may not work for layers that derive their name from core IO objects
        Dim layers As cEcospaceLayer() = Core.EcospaceBasemap.Layers(def.VarName)
        Dim iIndex As Integer = cCore.NULL_VALUE
        For i As Integer = 0 To layers.Count - 1
            Dim layer As cEcospaceLayer = layers(i)
            If layers.Count = 0 Or String.Compare(layer.Name, def.LayerName, True) = 0 Then
                iIndex = i
                Exit For
            End If
        Next
        If (iIndex = cCore.NULL_VALUE) Then Return False

        Dim dsets As cSpatialDataSetManager = man.DatasetManager
        Dim conn As cSpatialDataConnection = adt.AddConnection(iIndex)
        conn.Dataset = dsets.Find(def.DatasetName)
        conn.Converter = New EwESpatialAssetsPlugin.SpatialData.cRasterConverterPlugin()
        conn.Scale = def.Scalar

        Return True

    End Function

#End Region ' Spat temp FW

#Region " Ecospace autosaving "

    Public Function EcospaceAutosave(t1 As Type) As Boolean
        Dim parms As cEcospaceModelParameters = Core.EcospaceModelParameters
        For n As Integer = 1 To parms.nResultWriters
            Dim writer As IEcospaceResultsWriter = parms.ResultWriter(n)
            If t1.IsEquivalentTo(writer.GetType) Then
                writer.Enabled = True
                Core.Autosave(eAutosaveTypes.Ecospace) = True
                Return True
            End If
        Next
        Return False
    End Function

#End Region ' Ecospace autosaving

#Region " Generic bits "

    Private Sub LogStep(strMessage As String, bSuccess As Boolean)
        Console.WriteLine("{0}: {1}", If(bSuccess, "Success", "Error"), strMessage)
    End Sub

    Public Function ResolveEcosimScenario(v1 As String) As Integer
        If String.IsNullOrWhiteSpace(v1) Then Return 0
        For i As Integer = 1 To Core.nEcosimScenarios
            Dim sc As cEwEScenario = Core.EcosimScenarios(i)
            If (String.Compare(v1.Trim, sc.Name.Trim, True) = 0) Then
                Return i
            End If
        Next
        Return 0
    End Function

    Public Function ResolveEcosimTimeSeries(v1 As String) As Integer
        If String.IsNullOrWhiteSpace(v1) Then Return 0
        For i As Integer = 1 To Core.nTimeSeriesDatasets
            Dim tsd As cTimeSeriesDataset = Core.TimeSeriesDataset(i)
            If (String.Compare(v1.Trim, tsd.Name.Trim, True) = 0) Then
                Return i
            End If
        Next
        Return 0
    End Function

    Public Function ResolveEcospaceScenario(v1 As String) As Integer
        If String.IsNullOrWhiteSpace(v1) Then Return 0
        For i As Integer = 1 To Core.nEcospaceScenarios
            Dim sc As cEwEScenario = Core.EcospaceScenarios(i)
            If (String.Compare(v1.Trim, sc.Name.Trim, True) = 0) Then
                Return i
            End If
        Next
        Return 0
    End Function

#End Region ' Generic bits

#End Region ' Internals
End Class
