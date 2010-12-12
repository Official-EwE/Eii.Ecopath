Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceModelParameters
    Inherits cCoreInputOutputBase

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
            meta = New cVariableMetaData(0, 1000, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.nSolverThreads, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'groups per threads
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.nGroupsPerThread, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'space threads
            meta = New cVariableMetaData(0, 1000, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.nSpaceThreads, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'cells per thread
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(1, eVarNameFlags.nMapCellsPerThread, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'stanza packets multiplier
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0.5, eVarNameFlags.PacketsMultiplier, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
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

    ''' <summary>
    ''' Ecospace initialization biomass to Habitat adjusted or Ecopath base
    ''' </summary>
    ''' <remarks>True = Habitat adjusted, False = Ecopath base</remarks>
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

    Public Property nSolverThreads() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.nSolverThreads))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.nSolverThreads, value)
        End Set

    End Property

    Public Property nGroupsPerThread() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.nGroupsPerThread))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.nGroupsPerThread, value)
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

    Public Property nMapCellsPerThread() As Integer

        Get
            Return CInt(GetVariable(eVarNameFlags.nMapCellsPerThread))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.nMapCellsPerThread, value)
        End Set

    End Property


    ''' <summary>
    ''' Use the Individual Behavior Model
    ''' </summary>
    ''' <remarks></remarks>
    Public Property UseIBM() As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.UseIBM))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.UseIBM, value)
        End Set

    End Property


    ''' <summary>
    ''' Ecospace initialization biomass to Habitat adjusted or Ecopath base
    ''' </summary>
    ''' <remarks>True = Habitat adjusted, False = Ecopath base</remarks>
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

#End Region ' Variables by dot (.) operator

End Class
