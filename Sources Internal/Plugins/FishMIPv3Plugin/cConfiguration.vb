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

Option Strict On
Imports System.IO
Imports System.Xml.Serialization
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEEcologicalIndicatorsPlugin
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

' Stupid design issue: because protocol and configuration are separate, the arrays and
' structures to hold user configurations can run out of sync. Ideally, all user settings
' would live in the protocol class too. Loading a new protocol = fresh start, without having
' to declare empty vars ets. Bwech. cConfiguration just needs to deal interact between core, proto, serialization, etc

Public Class cConfiguration

#Region " Private vars "

    Private m_protocol As cProtocol = Nothing

    ''' <summary>Reference count</summary>
    Private m_refs As Integer = 0

    ''' The layers that can be driven by the spatial temporal data framework (layer name -> index).
    Private m_driverlayersmapping As New Dictionary(Of String, Integer)
    ''' Drivable layer scaling factors (layer name -> scaling).
    Private m_scaling As New Dictionary(Of String, Single)
    ''' <summary>Soc specifier -> TS index (-1=ignore, 0+=ts index)</summary>
    Private m_socmapping As New Dictionary(Of String, Integer)

    ' --- configuration ---
    Private m_periodstartyear As Integer()
    Private m_periodendyear As Integer()
    Private m_periodidentifier As String()
    Private m_periodclimscenario As String()
    Private m_periodsocscenario As String()

    ''' <summary>Time series index</summary>
    Private m_iFishing As Integer = 0

#End Region ' Private vars

#Region " Singleton "

    Private Shared _inst_ As cConfiguration = Nothing

    Public Shared Function Attach(core As cCore) As cConfiguration
        If (cConfiguration._inst_ Is Nothing) Then
            cConfiguration._inst_ = New cConfiguration(core)
            cConfiguration._inst_.LoadLastConfig()
        End If

        cConfiguration._inst_.m_refs += 1
        Return cConfiguration._inst_
    End Function

    Public Shared Sub Detach()

        If (cConfiguration._inst_ IsNot Nothing) Then
            cConfiguration._inst_.m_refs -= 1
            If cConfiguration._inst_.m_refs = 0 Then
                cConfiguration._inst_ = Nothing
            End If
        End If

    End Sub

#End Region ' Singleton

#Region " Construction "

    Private Sub New(core As cCore)
        Me.Core = core
    End Sub

#End Region ' Construction

#Region " Protocol "

    Public Function LoadProtocol(fin As String) As Boolean

        Dim reader As New StreamReader(fin)
        Dim serializer As New XmlSerializer(GetType(cProtocol))

        Try
            Me.m_protocol = CType(serializer.Deserialize(reader), cProtocol)
            If (Me.m_protocol IsNot Nothing) Then

                Dim periods As cPeriod() = Me.Periods
                Dim n As Integer = periods.Count - 1
                ReDim Me.m_periodidentifier(n)
                ReDim Me.m_periodstartyear(n)
                ReDim Me.m_periodendyear(n)
                ReDim Me.m_periodclimscenario(n)
                ReDim Me.m_periodsocscenario(n)

                ' Clear
                Me.m_driverlayersmapping.Clear()
                Me.m_socmapping.Clear()
                Me.m_scaling.Clear()

                ' Why was this duplication necessary again?!
                For i As Integer = 0 To n
                    Me.m_periodidentifier(i) = periods(i).Name
                    Me.m_periodstartyear(i) = periods(i).StartYear
                    Me.m_periodendyear(i) = periods(i).EndYear
                    Me.m_periodclimscenario(i) = ""
                    Me.m_periodsocscenario(i) = ""
                Next
            End If

            ' Create defaults
            For Each gcm As cGCM In Me.GlobalClimateModels
                For Each phy As String In Me.PhyVariables
                    Me.LayerScaling(gcm.Name, phy) = 1.0
                Next
            Next
            For Each name As String In Me.PhyVariables
                Me.GCMVarDriverLayerMapping(name) = -1
            Next
            For Each name As String In Me.EnvDriverVariables
                Me.GCMVarDriverLayerMapping(name) = -1
            Next
            For Each p As cPeriod In Me.Periods
                Me.ClimateScenarioForPeriod(p.Name) = ""
                Me.SocioEconomicScenarioForPeriod(p.Name) = ""
            Next
            For Each key As String In Me.m_driverlayersmapping.Keys.ToArray()
                Me.m_driverlayersmapping(key) = -1
            Next
            For Each soc As cSocioEconomicScenario In Me.SocioEnconomicScenarios
                Me.m_socmapping(soc.Name) = -1
            Next

            Me.Indicators.Clear()

            Me.LoadLastConfig()
            cLog.Write("Loaded protocol file " & fin)

        Catch ex As Exception
            ' Hmm
            cLog.Write(ex, "Failed to load protocol file " & fin)
        End Try
        Return (Me.m_protocol IsNot Nothing)

    End Function

    ''' <summary>
    ''' Gets the name of the loaded protocol.
    ''' </summary>
    Public ReadOnly Property ProtocolName As String
        Get
            If Me.m_protocol Is Nothing Then Return ""
            Return Me.m_protocol.Name
        End Get
    End Property

    ''' <summary>
    ''' Gets an array with available expiriments.
    ''' </summary>
    Public ReadOnly Property Experiments As cExperiment()
        Get
            If (Me.m_protocol Is Nothing) Then Return New cExperiment() {}
            Return Me.m_protocol.Experiments.ToArray()
        End Get
    End Property

    ''' <summary>
    ''' Gets an array with available climate scenarios.
    ''' </summary>
    Public ReadOnly Property ClimateScenarios As cClimateScenario()
        Get
            If (Me.m_protocol Is Nothing) Then Return New cClimateScenario() {}
            Return Me.m_protocol.ClimateScenarios.ToArray()
        End Get
    End Property

    ''' <summary>
    ''' Gets an array with available socio-economic scenarios.
    ''' </summary>
    Public ReadOnly Property SocioEnconomicScenarios As cSocioEconomicScenario()
        Get
            If (Me.m_protocol Is Nothing) Then Return New cSocioEconomicScenario() {}
            Return Me.m_protocol.SocioEconScenarios.ToArray()
        End Get
    End Property

    ''' <summary>
    ''' Gets an array with available global climate models.
    ''' </summary>
    Public ReadOnly Property GlobalClimateModels As cGCM()
        Get
            If (Me.m_protocol Is Nothing) Then Return New cGCM() {}
            Return Me.m_protocol.GCMs.ToArray()
        End Get
    End Property

    ''' <summary>Get phy variables from protocol</summary>
    Public ReadOnly Property PhyVariables As String()
        Get
            Dim vars As New List(Of String)
            If (Me.m_protocol IsNot Nothing) Then
                For i As Integer = 0 To Me.m_protocol.Variables.Count - 1
                    Dim var As cVariable = Me.m_protocol.Variables(i)
                    Dim test As String = var.VarType.ToLower()
                    If test.EndsWith("biomass") Then
                        vars.Add(var.Name)
                    End If
                Next
            End If
            Return vars.ToArray
        End Get
    End Property

    ''' <summary>Get phy variables from protocol</summary>
    Public ReadOnly Property EnvDriverVariables As String()
        Get
            Dim vars As New List(Of String)
            If (Me.m_protocol IsNot Nothing) Then
                For i As Integer = 0 To Me.m_protocol.Variables.Count - 1
                    Dim var As cVariable = Me.m_protocol.Variables(i)
                    Dim test As String = var.VarType.ToLower()
                    If test.EndsWith("driver") Then
                        vars.Add(var.Name)
                    End If
                Next
            End If
            Return vars.ToArray
        End Get
    End Property

    Public ReadOnly Property OceanRegions As cOceanRegion()
        Get
            If (Me.m_protocol Is Nothing) Then Return New cOceanRegion() {}
            Return Me.m_protocol.OceanRegions.ToArray()
        End Get
    End Property
    ''' <summary>
    ''' Gets an array with available run periods.
    ''' </summary>
    Public ReadOnly Property Periods As cPeriod()
        Get
            If (Me.m_protocol Is Nothing) Then Return New cPeriod() {}
            Dim data As cPeriod() = Me.m_protocol.Periods.ToArray()
            Array.Sort(data, New cPeriodSorter())
            Return data
        End Get
    End Property

    ''' <summary>
    ''' Gets an array with output variables dictacted by the protocol.
    ''' </summary>
    Public ReadOnly Property Outputs As cOutput()
        Get
            If (Me.m_protocol Is Nothing) Then Return New cOutput() {}
            Return Me.m_protocol.Outputs.ToArray
        End Get
    End Property

    ''' <summary>
    ''' Exposes the list of selected ecological indicators to produce.
    ''' </summary>
    Public ReadOnly Property Indicators As List(Of String)
        Get
            If (Me.m_protocol Is Nothing) Then Return New List(Of String)
            Return Me.m_protocol.Indicators
        End Get
    End Property

#End Region ' Protocol

#Region " Utility "

    ''' <summary>
    ''' Gets the period that a given year falls into, or -1 if no period was found.
    ''' </summary>
    Public ReadOnly Property GetPeriodNo(year As Integer) As Integer
        Get
            For i As Integer = 0 To Me.Periods.Count - 1
                If (Me.m_periodstartyear(i) <= year And year <= Me.m_periodendyear(i)) Then Return i
            Next
            Return -1
        End Get
    End Property

#End Region ' Utility

#Region " Generic "

    Public Event SettingsChanged()

    Public ReadOnly Property Core As cCore

    Public ReadOnly Property EcoIND As cEwEEcologicalIndicatorsPlugin
        Get
            Dim pm As cPluginManager = Me.Core.PluginManager
            Return DirectCast(pm.GetPlugins(cEwEEcologicalIndicatorsPlugin.PluginName)(0), cEwEEcologicalIndicatorsPlugin)
        End Get
    End Property

#End Region ' Generic

#Region " Output file naming "

    Friend Function OutputFileName(iPeriod As Integer, iTimestep As Integer) As String
        Dim p As cPeriod = Me.Periods(iPeriod)
        Return Me.OutputFileName(Me.OceanRegion, Me.ClimateModel, Me.ClimateScenarioForPeriod(p.Name), Me.SocioEconomicScenarioForPeriod(p.Name),
                                 "[var]", iTimestep, p.StartYear, p.EndYear)
    End Function

    Friend Function OutputFileName(region As String, gcm As String, climscenario As String, socscenario As String, var As String, timestep As Integer, ystart As Integer, yend As Integer) As String

        If (Me.m_protocol Is Nothing) Then Return ""

        ' [model-name]_[gcm]_[bias-correction]_[climate-scenario]­_[socio-econ-scenario]_[sens-scenario]_[variable]_[ocean-region]_[timestep]_[start-year]_[end-year]
        Dim fout As String = Me.m_protocol.OutputFileMask.ToLower()

        fout = Me.Substitute(fout, "model-name", If(String.Compare(region, "global", True) = 0, "ecoocean", "ewe"))
        fout = Me.Substitute(fout, "gcm", gcm)
        fout = Me.Substitute(fout, "bias-correction", "nobasd")
        fout = Me.Substitute(fout, "climate-scenario", climscenario)
        fout = Me.Substitute(fout, "socio-econ-scenario", socscenario)
        fout = Me.Substitute(fout, "sens-scenario", "default")
        fout = Me.Substitute(fout, "variable", var)
        fout = Me.Substitute(fout, "timestep", CStr(timestep))
        fout = Me.Substitute(fout, "ocean-region", region)
        fout = Me.Substitute(fout, "start-year", CStr(ystart))
        fout = Me.Substitute(fout, "end-year", CStr(yend))
        Return fout

    End Function

    Private Function Substitute(mask As String, part As String, val As String) As String
        val = val.Replace("_"c, "-"c).Replace(" "c, "-"c).ToLower()
        part = part.ToLower()
        Return mask.Replace("[" & part & "]", val)
    End Function

#End Region ' Output file naming

#Region " FishMIP output aggregation "

    ''' <summary>
    ''' Default group aggreagation as revised by Marta, 9 March 2020 after inclusion of Marine Turtles (group 51)
    ''' This grouping is the foundation for the Frontiers paper. 
    ''' </summary>
    Public Sub LoadEcoOceanDefaultMappings()

        Dim core As cCore = Me.Core

        For Each var As cOutput In Me.Outputs

            Dim vn As String = var.Name

            For igroup As Integer = 1 To core.nGroups

                Dim bChecked As Boolean = False
                Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(igroup)
                Dim grpOut As cEcoPathGroupOutput = core.EcoPathGroupOutputs(igroup)
                Dim name As String = grp.Name.ToLower()
                Dim bIsSmall As Boolean = name.Contains("small")
                Dim bIsMedium As Boolean = name.Contains("medium")
                Dim bIsLarge As Boolean = name.Contains("large")
                Dim bIsPelagic As Boolean = name.Contains("pelagic")
                Dim bIsDemersal As Boolean = name.Contains("demersal")
                Dim bIsFished As Boolean = grp.IsFished()

                Select Case vn
                    Case "tcb"
                        bChecked = grp.IsConsumer() And grpOut.TTLX() > 1
                    Case "tc"
                        bChecked = bIsFished
                    Case Else
                        ' Analyze name and draw conclusions
                        bChecked = If(var.Name.StartsWith("c"), bIsFished, True)
                        bChecked = bChecked And If(vn.Contains("d"), bIsDemersal, True)
                        bChecked = bChecked And If(vn.Contains("p"), bIsPelagic, True)
                        If (vn.Contains("30cm")) Then
                            bChecked = bChecked And bIsSmall
                        ElseIf (vn.Contains("30to90cm")) Then
                            bChecked = bChecked And bIsMedium
                        ElseIf (vn.Contains("90cm")) Then
                            bChecked = bChecked And bIsLarge
                        End If
                End Select

                var.Group(igroup) = If(bChecked, 1.0!, 0.0!)
            Next
        Next
        Me.SaveChanges()

    End Sub

    Public Sub DecipherGroupOutputMappingsFromTaxa()

        Dim core As cCore = Me.Core
        Dim ta As cTaxonAnalysis = core.TaxonAnalysis

        For Each var As cOutput In Me.Outputs

            Dim vn As String = var.Name
            Dim bIsSmall As Boolean = vn.Contains("30")
            Dim bIsLarge As Boolean = vn.Contains("90")
            Dim bIsMedium As Boolean = bIsSmall And bIsLarge
            If bIsMedium Then bIsSmall = False : bIsLarge = False

            For igroup As Integer = 1 To core.nGroups

                Dim sScale As Single = 0
                Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(igroup)
                Dim grpOut As cEcoPathGroupOutput = core.EcoPathGroupOutputs(igroup)
                Dim name As String = grp.Name.ToLower()
                Dim sPelagic As Single = 0
                Dim sDemersal As Single = 0
                Dim bIsFished As Boolean = grp.IsFished()

                If grpOut.TTLX >= var.TLMin And grpOut.TTLX <= var.TLMax Then

                    If (name.Contains("pelagic")) Then
                        sPelagic = 1
                    Else
                        For Each et As eEcologyTypes In ta.PelagicEcologyTypes
                            sPelagic += ta.GroupBiomassProportion(igroup, et)
                        Next
                    End If

                    If (name.Contains("demersal")) Then
                        sDemersal = 1
                    Else
                        For Each et As eEcologyTypes In ta.DemersalEcologyTypes
                            sDemersal += ta.GroupBiomassProportion(igroup, et)
                        Next
                    End If

                    If var.IsPelagic Then sScale += sPelagic
                    If var.IsDemersal Then sScale += sDemersal

                    ' Cannot determine 'comsumerness' from taxa
                    If var.IsConsumer And grp.IsConsumer Then sScale = 1

                    If bIsSmall Then
                        sScale = If(name.Contains("small"), sScale, 0)
                    ElseIf bIsMedium Then
                        sScale = If(name.Contains("medium"), sScale, 0)
                    ElseIf bIsLarge Then
                        sScale = If(name.Contains("large"), sScale, 0)
                    End If

                End If

                var.Group(igroup) = Math.Min(sScale, 1)
            Next
        Next
        Me.SaveChanges()

    End Sub

#End Region ' FishMIP output aggregation

#Region " EcoIND aggregation "

    Public Shared EcoIndTriatlasVariables As String() = {
        "tb", "tcomb", "tfb", "tib", "tibtif",
        "tdbtpb", "t4b", "q", "h", "tc",
        "tfc", "tic", "tictfc", "tdctpc", "t4c",
        "ctl", "mti", "cotl", "co325tl", "tmsrb",
        "tmsrc"}

    Public Function EcoIndVariable(ind As cIndicatorInfo) As String
        If (ind Is Nothing) Then Return ""
        Return Me.EcoIndVariable(ind.Abbreviation)
    End Function

    ''' <summary>
    ''' Translate the internal indicator abbreviation to a TRIATLAS EcoInd output var name, if any
    ''' </summary>
    ''' <param name="abbr">The abbr.</param>
    ''' <returns></returns>
    Public Function EcoIndVariable(abbr As String) As String
        abbr = abbr.ToLower()
        Select Case abbr
            Case "totalb" : Return EcoIndTriatlasVariables(0)
            Case "commercialb" : Return EcoIndTriatlasVariables(1)
            Case "fishb" : Return EcoIndTriatlasVariables(2)
            Case "inveb" : Return EcoIndTriatlasVariables(3)
            Case "invefishb" : Return EcoIndTriatlasVariables(4)
            Case "dempelb" : Return EcoIndTriatlasVariables(5)
            Case "pred4b" : Return EcoIndTriatlasVariables(6)
            Case "kemptonsq" : Return EcoIndTriatlasVariables(7)
            Case "shannondiversity" : Return EcoIndTriatlasVariables(8)
            Case "totalc" : Return EcoIndTriatlasVariables(9)
            Case "fishc" : Return EcoIndTriatlasVariables(10)
            Case "invec" : Return EcoIndTriatlasVariables(11)
            Case "invefishc" : Return EcoIndTriatlasVariables(12)
            Case "dempelc" : Return EcoIndTriatlasVariables(13)
            Case "pred4c" : Return EcoIndTriatlasVariables(14)
            Case "tlc" : Return EcoIndTriatlasVariables(15)
            Case "mti" : Return EcoIndTriatlasVariables(16)
            Case "tlco" : Return EcoIndTriatlasVariables(17)
            Case "tlco325" : Return EcoIndTriatlasVariables(18)
            Case "msrb" : Return EcoIndTriatlasVariables(19)
            Case "msrc" : Return EcoIndTriatlasVariables(20)
        End Select
        Return abbr
    End Function

#End Region ' EcoIND aggregation

#Region " Driver layer indexing and scaling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of a <see cref="cEcospaceLayerDriver"/> that can be 
    ''' driven by the spatial temporal data framework.
    ''' </summary>
    ''' <param name="var">The name of the ESM variable to assign to a driver layer</param>
    ''' <remarks>
    ''' <para>Note that some variables (*phy) are reserved to drive biomass.
    ''' All other GCM variables are presumed to be conenctable to env driver maps.</para>
    ''' <para>Also note that this setup does NOT provision driver other layers, such as
    ''' habitats, MPAs, etc.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property GCMVarDriverLayerMapping(var As String) As Integer
        Get
            var = var.ToLower()
            If (Me.m_driverlayersmapping.ContainsKey(var)) Then Return Me.m_driverlayersmapping(var)
            Return 1
        End Get
        Set(value As Integer)
            var = var.ToLower()
            Me.m_driverlayersmapping(var) = value
        End Set
    End Property

    Public ReadOnly Property DriverLayerNames As String()
        Get
            Return Me.m_driverlayersmapping.Keys.ToArray()
        End Get
    End Property

    ''' <summary>
    ''' Try to infer drivable layer indexes from env driver layer names.
    ''' </summary>
    Public Sub DiscoverGCMVarDriverLayerMapping()

        If (Me.Core.ActiveEcospaceScenarioIndex < 1) Then Return

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
        Dim keys As String() = Me.m_driverlayersmapping.Keys.ToArray()
        For Each key As String In keys
            If (Me.m_driverlayersmapping(key) = -1) Then
                For Each l As cEcospaceLayer In bm.Layers(eVarNameFlags.LayerDriver)
                    If (String.Compare(l.Name, key, True) = 0) Then
                        Me.m_driverlayersmapping(key) = l.Index
                    End If
                Next
            End If
        Next

    End Sub

    ''' <summary>
    ''' Get/set the layer scaling for a given <paramref name="var"/> and 
    ''' <see cref="ClimateModel"/>
    ''' </summary>
    ''' <param name="var"></param>
    ''' <returns></returns>
    Public Property LayerScaling(esm As String, var As String) As Single
        Get
            Dim key As String = (esm & ":" & var).ToLower()
            If (Me.m_scaling.ContainsKey(key)) Then Return Me.m_scaling(key)
            Return 1
        End Get
        Set(value As Single)
            Dim key As String = (esm & ":" & var).ToLower()
            Me.m_scaling(key) = value
        End Set
    End Property

    Public ReadOnly Property IncludeScaling(esm As String, var As String) As Boolean
        Get
            Dim key As String = (esm & ":" & var).ToLower()
            Return Me.m_scaling.ContainsKey(key)
        End Get
    End Property

#End Region ' Driver indexing and scaling

#Region " Social scenario indexing "

    ''' <summary>
    ''' Gets or sets the index of the soc scenario ts.
    ''' </summary>
    ''' <value>
    ''' The index of the fisheries time series for a soc scenario.
    ''' </value>
    ''' <remarks>
    ''' 0 = no time series, 1+ = valid TS index, below 0 = ignore
    ''' </remarks>
    Public Property SocScenarioTSIndex(name As String) As Integer
        Get
            If Me.m_socmapping.ContainsKey(name) Then Return Me.m_socmapping(name)
            Return -1
        End Get
        Set(value As Integer)
            Me.m_socmapping(name) = value
        End Set
    End Property

#End Region ' Social scenario indexing

#Region " Run configuration "

    Public Property SaveWithEcosim As Boolean = False

    Public Property SaveWithEcospace As Boolean = False

    Public Property SaveWithEcoIND As Boolean
        Get
            Dim pi As cEwEEcologicalIndicatorsPlugin = Me.EcoIND
            Return pi.AutoSave And pi.AutoRun(eCoreComponentType.EcoSpace)
        End Get
        Set(value As Boolean)
            Dim pi As cEwEEcologicalIndicatorsPlugin = Me.EcoIND
            pi.AutoSave = value
            pi.AutoRun(eCoreComponentType.EcoSpace) = value
            ' Set custom output varnames when saving from here
            Dim info As cIndicatorSettings = pi.Settings
            For ig As Integer = 0 To info.NumIndicatorGroups - 1
                Dim grp As cIndicatorInfoGroup = info.IndicatorGroup(ig)
                For i As Integer = 0 To grp.NumIndicators - 1
                    Dim ind As cIndicatorInfo = grp.Indicator(i)
                    ind.OutputName = If(value, Me.EcoIndVariable(ind), "")
                Next
            Next
        End Set
    End Property

    ''' <summary>
    ''' Fishing time series index, or 0 for no fishing.
    ''' </summary>
    ''' <remarks>
    ''' The fishing time series to use is deducted from the time series of the
    ''' selected SOC scenario(s). This code naively assumes that the time series 
    ''' as a whole is somehow representative for the combinations of soc scenarios
    ''' for the different periods. Ideally, this code would construct a new time
    ''' series that contains effort snippets for the various periods. We're not there
    ''' yet, and for now, this half-baked solution will have to do.
    ''' </remarks>
    Public ReadOnly Property Fishing As Integer
        Get
            Dim series As New List(Of Integer)
            Dim errors As New List(Of String)
            For Each p As cPeriod In Me.Periods
                Dim soc As String = Me.SocioEconomicScenarioForPeriod(p.Name)
                Dim iTest As Integer = Me.SocScenarioTSIndex(soc)
                If (iTest >= 0) Then
                    If Not series.Contains(iTest) Then
                        series.Add(iTest)
                        errors.Add(String.Format(My.Resources.TS_CONFLICT_DETAIL, p.Name, soc, iTest))
                    End If
                End If
            Next
            If (series.Count = 0) Then Return -1
            If (series.Count > 1) Then Me.ReportFailure(My.Resources.TS_CONFLICT, errors)
            Return series(0)
        End Get
    End Property

    Public Property ClimateModel As String = ""
    Public Property OceanRegion As String = ""
    Public Property ReportingStartYear() As Integer = 1950
    Public Property ReportingEndYear() As Integer = 2100

    Public Property ClimateScenarioForPeriod(period As String) As String
        Get
            Dim iPeriod As Integer = Array.IndexOf(Me.m_periodidentifier, period)
            If (iPeriod = -1) Then Return ""
            Return Me.m_periodclimscenario(iPeriod)
        End Get
        Set(value As String)
            Dim iPeriod As Integer = Array.IndexOf(Me.m_periodidentifier, period)
            If (iPeriod = -1) Then Return
            Me.m_periodclimscenario(iPeriod) = value
        End Set
    End Property

    Public Property SocioEconomicScenarioForPeriod(period As String) As String
        Get
            Dim iPeriod As Integer = Array.IndexOf(Me.m_periodidentifier, period)
            If (iPeriod = -1) Then Return ""
            Return Me.m_periodsocscenario(iPeriod)
        End Get
        Set(value As String)
            Dim iPeriod As Integer = Array.IndexOf(Me.m_periodidentifier, period)
            If (iPeriod = -1) Then Return
            Me.m_periodsocscenario(iPeriod) = value
        End Set
    End Property

    ''' <summary>
    ''' Get/set the experiment to run
    ''' </summary>
    ''' <remarks>
    ''' Get: return the experiment that reflects the selected climate and socio-economic scenarios
    ''' Set: set the selected climate and socio-economic scenarios from the experiment
    ''' </remarks>
    Public Property Experiment As cExperiment
        Get
            For Each ex As cExperiment In Me.Experiments
                For Each f As cForcing In ex.Forcings
                    If (Me.ClimateScenarioForPeriod(f.Name) = f.Climate) And
                       (Me.SocioEconomicScenarioForPeriod(f.Name) = f.SocioEcon) Then
                        Return ex
                    End If
                Next
            Next
            Return Nothing
        End Get
        Set(ex As cExperiment)
            If (ex Is Nothing) Then Return
            For Each f As cForcing In ex.Forcings
                Me.ClimateScenarioForPeriod(f.Name) = f.Climate
                Me.SocioEconomicScenarioForPeriod(f.Name) = f.SocioEcon
            Next
        End Set
    End Property

#End Region ' Run configuration

#Region " Persistence "

    Private Const SECT_AUTOSAVE As String = "Autosave"
    'Private Const KEY_SAVESIM As String = "Ecosim"
    Private Const KEY_SAVESPACE As String = "Ecospace"
    Private Const KEY_SAVEINDS As String = "Indicators"

    Private Const SECT_AGGREGATION As String = "Aggregation"
    Private Const KEY_AGGREGATION As String = "Agg_{0}_{1}" ' Variable {0}, group {1} = prop (single)

    Private Const SECT_INDICATORS As String = "EcoIND"
    Private Const KEY_INDICATOR As String = "Indicator_{0}" ' Indicators {0}=name will be autosaved

    Private Const SECT_LAYERS As String = "Layers"
    Private Const KEY_LAYERINDEX As String = "Layer_index_{0}"

    Private Const SECT_SCALING As String = "Scaling"
    Private Const KEY_DRIVERSCALING As String = "Scaling_{0}"

    Private Const SECT_SOC As String = "SocScenarios"
    Private Const KE_SOC As String = "Soc_{0}_ts_index"

    Private Const SECT_CONFIG As String = "Configuration"
    Private Const KEY_REGION As String = "OceanRegion"
    Private Const KEY_CLIMMODEL As String = "ClimateModel"
    Private Const KEY_CLIMDRIVER As String = "DRIVER_CLIM_{0}" ' Global climate driver for period {0} = name (string)
    Private Const KEY_SOCDRIVER As String = "DRIVER_SOC_{0}" ' Soc driver for period {0} = name (string)

    Private Sub LoadLastConfig()

        If (Me.m_protocol Is Nothing) Then Return

        Dim core As cCore = Me.Core
        Dim settings As cXMLSettings = Me.PersistentSettings(Me.m_protocol.Name).Settings
        Dim keys As String() = Nothing

        'Me.SaveWithEcosim = settings.ReadSetting(SECT_AUTOSAVE, KEY_SAVESIM, False)
        Me.SaveWithEcospace = settings.ReadSetting(SECT_AUTOSAVE, KEY_SAVESPACE, False)
        Me.SaveWithEcoIND = settings.ReadSetting(SECT_AUTOSAVE, KEY_SAVEINDS, False)

        Me.OceanRegion = settings.ReadSetting(SECT_CONFIG, KEY_REGION, Me.OceanRegion)
        Me.ClimateModel = settings.ReadSetting(SECT_CONFIG, KEY_CLIMMODEL, Me.ClimateModel)

        ' Load driver assignments
        For Each p As cPeriod In Me.Periods
            Me.ClimateScenarioForPeriod(p.Name) = settings.ReadSetting(SECT_CONFIG, String.Format(KEY_CLIMDRIVER, p.Name), "")
            Me.SocioEconomicScenarioForPeriod(p.Name) = settings.ReadSetting(SECT_CONFIG, String.Format(KEY_SOCDRIVER, p.Name), "")
        Next

        ' Load group aggregations
        For Each var As cOutput In Me.Outputs
            For i As Integer = 1 To Me.Core.nGroups
                var(i) = settings.ReadSetting(SECT_AGGREGATION, String.Format(KEY_AGGREGATION, i, var.Name), 0.0!)
            Next
        Next

        ' Load layer indexes 
        keys = Me.m_driverlayersmapping.Keys.ToArray()
        For Each key As String In keys
            Me.m_driverlayersmapping(key) = settings.ReadSetting(SECT_LAYERS, String.Format(KEY_LAYERINDEX, key), -1)
        Next

        ' Load driver scaling
        keys = Me.m_scaling.Keys.ToArray()
        For Each key As String In keys
            Me.m_scaling(key) = settings.ReadSetting(SECT_SCALING, String.Format(KEY_DRIVERSCALING, key), 1.0!)
        Next

        ' Load soc scenario time series assignments
        For Each soc As cSocioEconomicScenario In Me.SocioEnconomicScenarios
            Me.m_socmapping(soc.Name) = settings.ReadSetting(SECT_SOC, String.Format(KEY_SOCDRIVER, soc.Name), -1)
        Next

        ' Load indicators
        Dim ecoind As cEwEEcologicalIndicatorsPlugin = Me.EcoIND
        Dim info As cIndicatorSettings = ecoind.Settings
        For ig As Integer = 0 To info.NumIndicatorGroups - 1
            Dim grp As cIndicatorInfoGroup = info.IndicatorGroup(ig)
            For i As Integer = 0 To grp.NumIndicators - 1
                Dim var As String = Me.EcoIndVariable(grp.Indicator(i))
                If settings.ReadSetting(SECT_INDICATORS, String.Format(KEY_INDICATOR, var), False) Then
                    Me.Indicators.Add(var)
                End If
            Next
        Next

    End Sub

    Public Sub SaveChanges()

        If (Me.m_protocol Is Nothing) Then Return

        Dim core As cCore = Me.Core
        Dim aux As cAuxiliaryData = Me.PersistentSettings(Me.m_protocol.Name)
        Dim settings As cXMLSettings = aux.Settings
        Dim keys As String() = Nothing

        'settings.WriteSetting(SECT_AUTOSAVE, KEY_SAVESIM, Me.SaveWithEcosim)
        settings.WriteSetting(SECT_AUTOSAVE, KEY_SAVESPACE, Me.SaveWithEcospace)
        settings.WriteSetting(SECT_AUTOSAVE, KEY_SAVEINDS, Me.SaveWithEcospace)

        settings.WriteSetting(SECT_CONFIG, KEY_REGION, Me.OceanRegion)
        settings.WriteSetting(SECT_CONFIG, KEY_CLIMMODEL, Me.ClimateModel)

        ' Save driver assignments
        For Each p As cPeriod In Me.Periods
            settings.WriteSetting(SECT_CONFIG, String.Format(KEY_CLIMDRIVER, p.Name), Me.ClimateScenarioForPeriod(p.Name))
            settings.WriteSetting(SECT_CONFIG, String.Format(KEY_SOCDRIVER, p.Name), Me.SocioEconomicScenarioForPeriod(p.Name))
        Next

        ' Save group aggregations
        For Each var As cOutput In Me.Outputs
            For i As Integer = 1 To Me.Core.nGroups
                settings.WriteSetting(SECT_AGGREGATION, String.Format(KEY_AGGREGATION, i, var.Name), var(i))
            Next
        Next

        ' Save layer indexes 
        keys = Me.m_driverlayersmapping.Keys.ToArray()
        For Each key As String In keys
            settings.WriteSetting(SECT_LAYERS, String.Format(KEY_LAYERINDEX, key), Me.m_driverlayersmapping(key))
        Next

        ' Save driver scaling
        keys = Me.m_scaling.Keys.ToArray()
        For Each key As String In keys
            settings.WriteSetting(SECT_SCALING, String.Format(KEY_DRIVERSCALING, key), Me.m_scaling(key))
        Next

        ' Save soc scenario time series assignments
        For Each soc As cSocioEconomicScenario In Me.SocioEnconomicScenarios
            settings.WriteSetting(SECT_SOC, String.Format(KEY_SOCDRIVER, soc.Name), Me.m_socmapping(soc.Name))
        Next

        ' Save indicators
        Dim ecoind As cEwEEcologicalIndicatorsPlugin = Me.EcoIND
        Dim info As cIndicatorSettings = ecoind.Settings
        For ig As Integer = 0 To info.NumIndicatorGroups - 1
            Dim grp As cIndicatorInfoGroup = info.IndicatorGroup(ig)
            For i As Integer = 0 To grp.NumIndicators - 1
                Dim var As String = Me.EcoIndVariable(grp.Indicator(i))
                settings.WriteSetting(SECT_INDICATORS, String.Format(KEY_INDICATOR, var), Me.Indicators.Contains(var))
            Next
        Next

        aux.Update()
    End Sub

    Private Function PersistentSettings(protocol As String) As cAuxiliaryData
        Return Me.Core.AuxillaryData("FishMIP_" & protocol)
    End Function

#End Region ' Persistence

#Region " Messaging "

    Public Sub ReportSuccess(text As String, Optional hyperlink As String = "")
        Dim msg As New cMessage(String.Format(SharedResources.GENERIC_LABEL_DETAILED, My.Resources.CAPTION, text),
                                eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
        msg.Hyperlink = hyperlink
        Me.Core.Messages.SendMessage(msg, True)
    End Sub

    Public Sub ReportFailure(text As String, Optional issues As IEnumerable(Of String) = Nothing, Optional hyperlink As String = "")
        Dim msg As New cMessage(String.Format(SharedResources.GENERIC_LABEL_DETAILED, My.Resources.CAPTION, text),
                                eMessageType.Any, eCoreComponentType.External, eMessageImportance.Critical)
        If (issues IsNot Nothing) Then
            For Each issue As String In issues
                msg.AddVariable(New cVariableStatus(eStatusFlags.ErrorEncountered, issue, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, -1))
            Next
        End If
        msg.Hyperlink = hyperlink
        Me.Core.Messages.SendMessage(msg, True)
    End Sub

#End Region ' Messaging

End Class
