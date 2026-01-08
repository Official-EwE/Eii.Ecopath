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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwECore.Common
Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cEcospaceModelParameters
    Inherits cCoreInputOutputBase

    ''' <summary>Available Ecospace result writers.</summary>
    Private m_EcospaceResultsWriters As New List(Of IEcospaceResultsWriter)
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceModelParameters)()

#Region " Constructor "

    Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Dim val As cValue

        Try

            Me.DBID = DBID

            Me.m_dataType = eDataTypes.EcospaceModelParameter
            Me.m_coreComponent = eCoreComponentType.Ecospace
            Me.AllowValidation = False

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' Number of time steps per year
            val = New cValue(core, 1, eVarNameFlags.NumTimeStepsPerYear, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' Number of regions
            val = New cValue(core, 1, eVarNameFlags.EcospaceRegionNumber, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' Number of effort zones
            val = New cValue(core, 1, eVarNameFlags.EcospaceEffortZoneNumber, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' PredictEffort
            val = New cValue(core, 1, eVarNameFlags.PredictEffort, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            'AdjustSpace
            val = New cValue(core, 1, eVarNameFlags.AdjustSpace, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            'Total time
            val = New cValue(core, 1, eVarNameFlags.TotalTime, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' Tolerance
            val = New cValue(core, 1, eVarNameFlags.Tolerance, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' SOR (W)
            val = New cValue(core, 1, eVarNameFlags.SOR, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' Max num iterations
            val = New cValue(core, 1, eVarNameFlags.MaxIterations, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' UseExact
            val = New cValue(core, 1, eVarNameFlags.UseExact, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceMinForagingCapacity, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceUseHabCapGradCorrections, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            'Contaminant tracing
            val = New cValue(core, New Boolean, eVarNameFlags.ConSimOnEcoSpace, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'Spinup
            val = New cValue(core, New Boolean, eVarNameFlags.EcospaceSpinupEnabled, eStatusFlags.Null, eValueTypes.Bool)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceSpinupYears, eStatusFlags.Null, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            ' Multi threading vars

            'solver threads
            val = New cValue(core, 1, eVarNameFlags.nGridSolverThreads, eStatusFlags.Null, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)


            val = New cValue(core, 1, eVarNameFlags.nIBMMovementThreads, eStatusFlags.Null, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'space threads
            val = New cValue(core, 1, eVarNameFlags.nSpaceThreads, eStatusFlags.Null, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'Number of effort distribution threads
            val = New cValue(core, 1, eVarNameFlags.nEffortDistThreads, eStatusFlags.Null, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'stanza packets multiplier
            val = New cValue(core, 0.5, eVarNameFlags.PacketsMultiplier, eStatusFlags.Null, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'summary data
            'StartSummaryTime
            val = New cValue(core, 1, eVarNameFlags.EcospaceSummaryTimeStart, eStatusFlags.Null, eValueTypes.Int)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'EndSummaryTime 
            val = New cValue(core, 1, eVarNameFlags.EcospaceSummaryTimeEnd, eStatusFlags.Null, eValueTypes.Int)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'NumSummaryTimeSteps
            val = New cValue(core, 1, eVarNameFlags.EcospaceNumberSummaryTimeSteps, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.UseNewMultiStanza, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.UseIBM, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 0.5, eVarNameFlags.IFDPower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceIBMMovePacketOnStanza, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            'Core Ouput Dir
            val = New cValue(core, 1, eVarNameFlags.EcospaceUseCoreOutputDir, eStatusFlags.Null, eValueTypes.Bool)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceAutosaveAnnualOutput, eStatusFlags.Null, eValueTypes.Bool)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceAutosaveFirstTimeStep, eStatusFlags.Null, eValueTypes.Int)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceAutosaveSelectedGroupsFleetsOnly, eStatusFlags.Null, eValueTypes.Bool)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.UseEffortDistThreshold, eStatusFlags.Null, eValueTypes.Bool)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EffortDistThreshold, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, "", eVarNameFlags.EcospaceAreaOutputDir, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, "", eVarNameFlags.EcospaceMapOutputDir, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceIsEcosimBiomassForcingLoaded, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceUseEcosimBiomassForcing, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceIsEcosimDiscardForcingLoaded, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceUseEcosimDiscardForcing, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceSaveThreadingLog, eStatusFlags.Null, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Spatial effort distribution penalty variables
            val = New cValue(core, 1, eVarNameFlags.EcospaceDoPenaltySearch, eStatusFlags.Null, eValueTypes.Bool)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceNoFishWeight, eStatusFlags.Null, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospacePenpow, eStatusFlags.Null, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceFirstPenaltyMonth, eStatusFlags.Null, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, 1, eVarNameFlags.EcospaceEffortRelaxationWeight, eStatusFlags.Null, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


            'set status flags to default values
            Me.ResetStatusFlags()

            Me.m_EcospaceResultsWriters.AddRange(cEcospaceResultWriterFactory.GetWriters(Me.m_core.PluginManager))

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceModelParameters.")
            m_logger.LogError(ex, Me.ToString & ".New(..) Error creating new cEcospaceModelParameters. Error: " & ex.Message)
        End Try
    End Sub

#End Region ' Constructor

#Region " Overrides "

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        MyBase.ResetStatusFlags(bForceReset)
        Me.m_core.Set_IBM_Flags(Me, False)

        If (Me.m_core.ActiveEcotracerScenarioIndex >= 0) Then
            Me.ClearStatusFlags(eVarNameFlags.ConSimOnEcoSpace, eStatusFlags.NotEditable)
        Else
            Me.SetStatusFlags(eVarNameFlags.ConSimOnEcoSpace, eStatusFlags.NotEditable)
        End If

    End Function

#End Region ' Overrides

#Region " Variables by dot (.) operator "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of time steps per year for this model. Internally,
    ''' this value will be recalculated to the ratio of the time step size (years).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumberOfTimeStepsPerYear() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.NumTimeStepsPerYear))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.NumTimeStepsPerYear, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of regions for this scenario.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property nRegions() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcospaceRegionNumber))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcospaceRegionNumber, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of effort zones for this scenario.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property nEffortZones() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcospaceEffortZoneNumber))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcospaceEffortZoneNumber, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace initialization biomass to Habitat adjusted or Ecopath base
    ''' </summary>
    ''' <remarks>True = Habitat adjusted, False = Ecopath base</remarks>
    ''' -----------------------------------------------------------------------
    Public Property AdjustSpace() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.AdjustSpace))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.AdjustSpace, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the  <see cref="cEcospaceDataStructures.PredictEffort">PredictEffort</see> for this model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PredictEffort() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.PredictEffort))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.PredictEffort, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cEcospaceDataStructures.SumStart">start</see>
    ''' of the first summary period (in years) for this model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property StartSummaryTime() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcospaceSummaryTimeStart))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcospaceSummaryTimeStart, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Start of the last summary period (in years).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EndSummaryTime() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcospaceSummaryTimeEnd))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcospaceSummaryTimeEnd, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Number to time steps to summarize the data over.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumberSummaryTimeSteps() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcospaceNumberSummaryTimeSteps))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcospaceNumberSummaryTimeSteps, value)
        End Set
    End Property

    Public Property nGridSolverThreads() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.nGridSolverThreads))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.nGridSolverThreads, value)
        End Set
    End Property

    Public Property nIBMMovementThreads() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.nIBMMovementThreads))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.nIBMMovementThreads, value)
        End Set
    End Property

    ''' <summary>
    ''' Number of Effort distrubtion threads
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Not used by the Scientific Interface provided here so it can be set via code.</remarks>
    Public Property nEffortDistThreads() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.nEffortDistThreads))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.nEffortDistThreads, value)
        End Set
    End Property

    Public Property nSpaceThreads() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.nSpaceThreads))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.nSpaceThreads, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Ecospace should use its Individual Based Model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UseIBM() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.UseIBM))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.UseIBM, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace initialization biomass to Habitat adjusted or Ecopath base
    ''' </summary>
    ''' <remarks>True = Habitat adjusted, False = Ecopath base</remarks>
    ''' -----------------------------------------------------------------------
    Public Property UseNewMultiStanza() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.UseNewMultiStanza))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.UseNewMultiStanza, value)
        End Set
    End Property

    Public Property IFDPower() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.IFDPower))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.IFDPower, value)
        End Set
    End Property

    Public Property TotalTime() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TotalTime))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TotalTime, value)
        End Set
    End Property

    Public Property PacketsMultiplier() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PacketsMultiplier))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.PacketsMultiplier, value)
        End Set
    End Property

    Public Property Tolerance() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Tolerance))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.Tolerance, value)
        End Set
    End Property

    Public Property SOR() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.SOR))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.SOR, value)
        End Set
    End Property

    Public Property MaxNumberOfIterations() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MaxIterations))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.MaxIterations, value)
        End Set
    End Property

    Public Property ContaminantTracing() As Boolean
        Get
            Return CType(Me.GetVariable(eVarNameFlags.ConSimOnEcoSpace), Boolean)
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.ConSimOnEcoSpace, value)
        End Set
    End Property

    Public Property ContaminantTracingStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.ConSimOnEcoSpace)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.ConSimOnEcoSpace, value)
        End Set
    End Property

    Public Property SpinupEnabled() As Boolean
        Get
            Return CType(Me.GetVariable(eVarNameFlags.EcospaceSpinupEnabled), Boolean)
        End Get
        Set(ByVal value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceSpinupEnabled, value)
        End Set
    End Property

    Public Property SpinupEnabledStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceSpinupEnabled)
        End Get
        Friend Set(ByVal value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceSpinupEnabled, value)
        End Set
    End Property

    Public Property SpinupYears() As Single
        Get
            Return CType(Me.GetVariable(eVarNameFlags.EcospaceSpinupYears), Single)
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceSpinupYears, value)
        End Set
    End Property

    Public Property SpinupYearsStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceSpinupYears)
        End Get
        Friend Set(ByVal value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceSpinupYears, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eVarNameFlags.UseExact">UseExact</see> flag for this model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UseExact() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.UseExact))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.UseExact, value)
        End Set
    End Property

    Public Property UseHabCapGradientCorrections As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceUseHabCapGradCorrections))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceUseHabCapGradCorrections, value)
        End Set
    End Property

    Public Property MinForagingCapacity As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceMinForagingCapacity))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceMinForagingCapacity, value)
        End Set
    End Property

    Public Property IBMMovePacketOnStanza() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceIBMMovePacketOnStanza))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceIBMMovePacketOnStanza, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether data should be written as annual average values.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UseAnnualOuput() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceAutosaveAnnualOutput))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceAutosaveAnnualOutput, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether data should be written only for groups and fleets that 
    ''' are selected.
    ''' </summary>
    ''' <seealso cref="UseAnnualOuput"/>
    ''' -----------------------------------------------------------------------
    Public Property AutosaveSelectedGroupsFleetsOnly() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceAutosaveSelectedGroupsFleetsOnly))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceAutosaveSelectedGroupsFleetsOnly, value)
        End Set
    End Property


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Ecospace should save its data to the standard core output
    ''' directory and scenario-dependent subdirectories. If false, data will be saved
    ''' directly to the core output path ignoring the scenario-dependent subdirectory
    ''' structures.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UseCoreOutputDirectory() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceUseCoreOutputDir))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceUseCoreOutputDir, value)
        End Set
    End Property

    Public Property UseEffortDistThreshold() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.UseEffortDistThreshold))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.UseEffortDistThreshold, value)
        End Set
    End Property

    Public Property EffortDistThreshold() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EffortDistThreshold))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EffortDistThreshold, value)
        End Set
    End Property

    Public Property UseEcosimBiomassForcing() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceUseEcosimBiomassForcing))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceUseEcosimBiomassForcing, value)
        End Set
    End Property

    Public Property IsEcosimBiomassForcingLoaded() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceIsEcosimBiomassForcingLoaded))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceIsEcosimBiomassForcingLoaded, value)
        End Set
    End Property

    Public Property UseEcosimDiscardForcing() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceUseEcosimDiscardForcing))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceUseEcosimDiscardForcing, value)
        End Set
    End Property

    Public Property IsEcosimDiscardForcingLoaded() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceIsEcosimDiscardForcingLoaded))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceIsEcosimDiscardForcingLoaded, value)
        End Set
    End Property

    ''' <summary>
    ''' User defined output directory for Ecospace Area Average results
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Not used by the Scientific Interface. 
    ''' This allows an external application, console app or plugin, to specify custom output directories for Ecospace.
    ''' </remarks>
    Public Property EcospaceAreaOutputDir() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.EcospaceAreaOutputDir))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.EcospaceAreaOutputDir, value)
        End Set
    End Property

    ''' <summary>
    ''' User defined output directory for Ecospace Map results
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Not used by the Scientific Interface. 
    ''' This allows an external application, console app or plugin, to specify custom output directories for Ecospace.
    ''' </remarks>
    Public Property EcospaceMapOutputDir() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.EcospaceMapOutputDir))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.EcospaceMapOutputDir, value)
        End Set
    End Property

    Public Property FirstOutputTimeStep() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcospaceAutosaveFirstTimeStep))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcospaceAutosaveFirstTimeStep, value)
        End Set
    End Property

    Public Property SaveThreadingLog() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceSaveThreadingLog))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceSaveThreadingLog, value)
        End Set
    End Property

    Public Property UseSpatialEffortPenalty() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceDoPenaltySearch))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceDoPenaltySearch, value)
        End Set
    End Property

    Public Property PenaltyPower() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospacePenpow))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospacePenpow, value)
        End Set
    End Property

    Public Property AdjustEffortWeight() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceNoFishWeight))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceNoFishWeight, value)
        End Set
    End Property

    Public Property FirstPenaltyMonth() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcospaceFirstPenaltyMonth))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcospaceFirstPenaltyMonth, value)
        End Set
    End Property

    Public Property EffortRelaxationWeight() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceEffortRelaxationWeight))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcospaceEffortRelaxationWeight, value)
        End Set
    End Property

#End Region ' Variables by dot (.) operator

#Region " Utility methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of available <see cref="IEcospaceResultsWriter">Ecospace result writers</see>.
    ''' <seealso cref="ResultWriter"/>
    ''' <seealso cref="IEcospaceResultsWriter"/>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nResultWriters As Integer
        Get
            Return Me.m_EcospaceResultsWriters.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a <see cref="IEcospaceResultsWriter"/>.
    ''' <seealso cref="nResultWriters"/>
    ''' <seealso cref="IEcospaceResultsWriter"/>
    ''' </summary>
    ''' <param name="iIndex">One-based index of the result writer to obtain.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function ResultWriter(iIndex As Integer) As IEcospaceResultsWriter
        Try
            Return Me.m_EcospaceResultsWriters(iIndex - 1)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Return Nothing
    End Function

#End Region ' Utility methods

End Class
