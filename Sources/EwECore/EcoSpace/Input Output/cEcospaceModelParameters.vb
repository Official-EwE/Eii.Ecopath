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
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceModelParameters
    Inherits cCoreInputOutputBase

    Private N_CORES_HUNGABEE As Integer = 2048

#Region " Constructor "

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData
        ' Dim desc() As Char

        Try

            Me.DBID = DBID

            m_dataType = eDataTypes.EcospaceModelParameter
            m_coreComponent = eCoreComponentType.EcoSpace
            Me.AllowValidation = False

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' Number of time steps per year
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.NumTimeStepsPerYear, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Number of regions
            meta = New cVariableMetaData(0, 20000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.EcospaceRegionNumber, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' PredictEffort
            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.PredictEffort, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'AdjustSpace
            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.AdjustSpace, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'Total time
            meta = New cVariableMetaData(0, cCore.MAX_RUN_LENGTH, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(1, eVarNameFlags.TotalTime, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Tolerance
            meta = New cVariableMetaData(0.000001, 0.1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(1, eVarNameFlags.Tolerance, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' SOR (W)
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), 0.9)
            val = New cValue(1, eVarNameFlags.SOR, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Max num iterations
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.MaxIterations, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' UseExact
            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.UseExact, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))

            m_values.Add(val.varName, val)
            'Contaminant tracing
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.ConSimOnEcoSpace, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.ConSimOnEcoSpace))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            ' Multi threading vars

            'solver threads
            meta = New cVariableMetaData(0, N_CORES_HUNGABEE, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.nGridSolverThreads, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'space threads
            meta = New cVariableMetaData(0, N_CORES_HUNGABEE, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.nSpaceThreads, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'Number of effort distribution threads
            meta = New cVariableMetaData(0, N_CORES_HUNGABEE, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.nEffortDistThreads, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'stanza packets multiplier
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0.5, eVarNameFlags.PacketsMultiplier, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'summary data
            'StartSummaryTime

            ' meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            meta = Nothing
            val = New cValue(1, eVarNameFlags.EcospaceSummaryTimeStart, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.EcospaceSummaryTimeStart))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'EndSummaryTime 
            ' meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            meta = Nothing
            val = New cValue(1, eVarNameFlags.EcospaceSummaryTimeEnd, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.EcospaceSummaryTimeEnd))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'NumSummaryTimeSteps
            'meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            meta = Nothing
            val = New cValue(1, eVarNameFlags.EcospaceNumberSummaryTimeSteps, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.EcospaceNumberSummaryTimeSteps))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)


            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.UseNewMultiStanza, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.UseIBM, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.IFDPower, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'meta = New cVariableMetaData(0, 2, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            'val = New cValue(1, eVarNameFlags.EcospaceCapCalType, eStatusFlags.Null, eValueTypes.Int, _
            '                    meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            'm_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.EcospaceIBMMovePacketOnStanza, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'Core Ouput Dir
            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.EcospaceUseCoreOutputDir, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            ' Save Annual
            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.EcospaceUseAnnualOutput, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.bUseEffortDistThreshold, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.EffortDistThreshold, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData()
            val = New cValue(1, eVarNameFlags.EcospaceUseLocalMemory, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(255)
            val = New cValue("", eVarNameFlags.EcospaceAreaOutputDir, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            val.Stored = False
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(255)
            val = New cValue("", eVarNameFlags.EcospaceMapOutputDir, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.AffectsRunState = False
            val.Stored = False
            m_values.Add(val.varName, val)


            meta = Nothing
            val = New cValue(1, eVarNameFlags.EcospaceFirstOutputTimeStep, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.EcospaceFirstOutputTimeStep))
            val.Stored = False
            val.AffectsRunState = False
            m_values.Add(val.varName, val)

            'set status flags to default values
            ResetStatusFlags()

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceModelParameters.")
            cLog.Write(Me.ToString & ".New(..) Error creating new cEcospaceModelParameters. Error: " & ex.Message)
        End Try
    End Sub

#End Region ' Constructor

#Region " Overrides "

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
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
    ''' Get/set whether Ecospace should automatically save region summary output 
    ''' for every time step.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property SaveRegions As Boolean
        Get
            Return (Me.m_core.Autosave(eAutosaveTypes.Ecospace) = True)
        End Get
        Set(value As Boolean)
            Me.m_core.Autosave(eAutosaveTypes.Ecospace) = value
        End Set
    End Property


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Ecospace should automatically save ASC files.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property SaveASC As Boolean
        Get
            Return (Me.m_core.Autosave(eAutosaveTypes.EcospaceResults) = True) And _
                   (String.Compare(Me.m_core.AutosaveFormat(eAutosaveTypes.EcospaceResults), cEcospaceASCMapResultsWriter.cDATA_NAME, True) = 0)
        End Get
        Set(value As Boolean)
            Me.m_core.Autosave(eAutosaveTypes.EcospaceResults) = value
            Me.m_core.AutosaveFormat(eAutosaveTypes.EcospaceResults) = cEcospaceASCMapResultsWriter.cDATA_NAME
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Ecospace should automatically save CSV files.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property SaveCSV As Boolean
        Get
            Return (Me.m_core.Autosave(eAutosaveTypes.EcospaceResults) = True) And _
                  (String.Compare(Me.m_core.AutosaveFormat(eAutosaveTypes.EcospaceResults), cEcospaceCSVMapResultsWriter.cDATA_NAME, True) = 0)
        End Get
        Set(value As Boolean)
            Me.m_core.Autosave(eAutosaveTypes.EcospaceResults) = value
            Me.m_core.AutosaveFormat(eAutosaveTypes.EcospaceResults) = cEcospaceCSVMapResultsWriter.cDATA_NAME
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Ecospace should automatically save PNG files for every
    ''' time step.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property SavePNG As Boolean
        Get
            Return (Me.m_core.Autosave(eAutosaveTypes.EcospaceResults) = True) And _
                  (String.Compare(Me.m_core.AutosaveFormat(eAutosaveTypes.EcospaceResults), ".png", True) = 0)
        End Get
        Set(value As Boolean)
            Me.m_core.Autosave(eAutosaveTypes.EcospaceResults) = value
            Me.m_core.AutosaveFormat(eAutosaveTypes.EcospaceResults) = ".png"
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of time steps per year for this model. Internally,
    ''' this value will be recalculated to the ratio of the time step size (years).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumberOfTimeStepsPerYear() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.NumTimeStepsPerYear))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.NumTimeStepsPerYear, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of regions for this scenario.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property nRegions() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.EcospaceRegionNumber))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.EcospaceRegionNumber, value)
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
            Return CBool(GetVariable(eVarNameFlags.AdjustSpace))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.AdjustSpace, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the  <see cref="cEcospaceDataStructures.PredictEffort">PredictEffort</see> for this model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PredictEffort() As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.PredictEffort))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.PredictEffort, value)
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
            Return CInt(GetVariable(eVarNameFlags.EcospaceSummaryTimeStart))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.EcospaceSummaryTimeStart, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Start of the last summary period (in years).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EndSummaryTime() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.EcospaceSummaryTimeEnd))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.EcospaceSummaryTimeEnd, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Number to time steps to summarize the data over.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumberSummaryTimeSteps() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.EcospaceNumberSummaryTimeSteps))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.EcospaceNumberSummaryTimeSteps, value)
        End Set

    End Property

    Public Property nGridSolverThreads() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.nGridSolverThreads))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.nGridSolverThreads, value)
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
            Return CInt(GetVariable(eVarNameFlags.nEffortDistThreads))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.nEffortDistThreads, value)
        End Set

    End Property


    Public Property nSpaceThreads() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.nSpaceThreads))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.nSpaceThreads, value)
        End Set

    End Property


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Ecospace should use its Individual Based Model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UseIBM() As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.UseIBM))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.UseIBM, value)
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
            Return CBool(GetVariable(eVarNameFlags.UseNewMultiStanza))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.UseNewMultiStanza, value)
        End Set

    End Property


    Public Property IFDPower() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.IFDPower))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.IFDPower, value)
        End Set

    End Property

    Public Property TotalTime() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.TotalTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.TotalTime, value)
        End Set

    End Property

    Public Property PacketsMultiplier() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.PacketsMultiplier))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.PacketsMultiplier, value)
        End Set
    End Property

    Public Property Tolerance() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.Tolerance))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.Tolerance, value)
        End Set

    End Property

    Public Property SOR() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.SOR))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.SOR, value)
        End Set

    End Property

    Public Property MaxNumberOfIterations() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.MaxIterations))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.MaxIterations, value)
        End Set

    End Property

    Public Property ContaminantTracing() As Boolean

        Get
            Return CType(GetVariable(eVarNameFlags.ConSimOnEcoSpace), Boolean)
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.ConSimOnEcoSpace, value)
        End Set

    End Property

    Public Property ContaminantTracingStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.ConSimOnEcoSpace)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.ConSimOnEcoSpace, value)
        End Set

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eVarNameFlags.UseExact">UseExact</see> flag for this model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UseExact() As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.UseExact))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.UseExact, value)
        End Set

    End Property

    Public Property IBMMovePacketOnStanza() As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.EcospaceIBMMovePacketOnStanza))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.EcospaceIBMMovePacketOnStanza, value)
        End Set

    End Property

    ' ''' -----------------------------------------------------------------------
    ' ''' <summary>
    ' ''' Set the <see cref="eEcospaceCapacityCalType">inputs</see> that Ecospace uses to calculate capacity.
    ' ''' </summary>
    ' ''' -----------------------------------------------------------------------
    'Public Property CapacityCalculationType() As eEcospaceCapacityCalType

    '    Get
    '        Return CType(GetVariable(eVarNameFlags.EcospaceCapCalType), eEcospaceCapacityCalType)
    '    End Get

    '    Set(ByVal value As eEcospaceCapacityCalType)
    '        SetVariable(eVarNameFlags.EcospaceCapCalType, value)
    '    End Set

    'End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether data should be written as annual average values.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UseAnnualOuput() As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.EcospaceUseAnnualOutput))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.EcospaceUseAnnualOutput, value)
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
            Return CBool(GetVariable(eVarNameFlags.EcospaceUseCoreOutputDir))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.EcospaceUseCoreOutputDir, value)
        End Set

    End Property

    Public Property UseEffortDistThreshold() As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.bUseEffortDistThreshold))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.bUseEffortDistThreshold, value)
        End Set

    End Property

    Public Property EffortDistThreshold() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EffortDistThreshold))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EffortDistThreshold, value)
        End Set

    End Property

    Public Property UseLocalMemory() As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.EcospaceUseLocalMemory))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.EcospaceUseLocalMemory, value)
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
            Return CStr(GetVariable(eVarNameFlags.EcospaceAreaOutputDir))
        End Get

        Set(ByVal value As String)
            SetVariable(eVarNameFlags.EcospaceAreaOutputDir, value)
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
            Return CStr(GetVariable(eVarNameFlags.EcospaceMapOutputDir))
        End Get

        Set(ByVal value As String)
            SetVariable(eVarNameFlags.EcospaceMapOutputDir, value)
        End Set

    End Property


    Public Property FirstOutputTimeStep() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.EcospaceFirstOutputTimeStep))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.EcospaceFirstOutputTimeStep, value)
        End Set

    End Property


#End Region ' Variables by dot (.) operator

End Class
