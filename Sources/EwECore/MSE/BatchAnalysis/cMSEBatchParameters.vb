


#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region

Public Class cMSEBatchParameters
    Inherits cCoreGroupBase


    Public Sub New(ByRef theCore As cCore, ByRef MSEBatchData As MSEBatchManager.cMSEBatchDataStructures, ByVal DBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData

        m_dataType = eDataTypes.MSEBatchParameters
        m_coreComponent = eCoreComponentType.MSE
        Me.AllowValidation = False
        Me.DBID = DBID

        'default OK status used for setVariable
        'see comment setVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.MSETFMNIteration, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMNIteration))
        m_values.Add(val.varName, val)

        Dim nTypes As Integer = [Enum].GetValues(GetType(eMSEBatchIterCalcTypes)).Length
        meta = New cVariableMetaData(0, nTypes, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.MSEBatchIterCalcType, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBatchIterCalcType))
        m_values.Add(val.varName, val)

        'Output Biomass
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.MSEBatchOutputBiomass, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBatchOutputBiomass))
        val.Stored = False
        m_values.Add(val.varName, val)

        'Output cb
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.MSEBatchOutputConBio, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBatchOutputConBio))
        val.Stored = False
        m_values.Add(val.varName, val)


        'Output Feeding Time
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.MSEBatchOutputFeedingTime, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBatchOutputFeedingTime))
        val.Stored = False
        m_values.Add(val.varName, val)


        'Output Pred rate
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.MSEBatchOutputPredRate, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBatchOutputPredRate))
        val.Stored = False
        m_values.Add(val.varName, val)


        'Output Catch
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.MSEBatchOutputCatch, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBatchOutputCatch))
        val.Stored = False
        m_values.Add(val.varName, val)

        'Output Catch
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.MSEBatchOutputFishingMortRate, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBatchOutputFishingMortRate))
        val.Stored = False
        m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub


    Public Property nTFMIteration As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MSETFMNIteration))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.MSETFMNIteration, value)
        End Set
    End Property

    Public Property IterCalcType As eMSEBatchIterCalcTypes
        Get
            Return CType(GetVariable(eVarNameFlags.MSEBatchIterCalcType), eMSEBatchIterCalcTypes)
        End Get

        Set(ByVal value As eMSEBatchIterCalcTypes)
            SetVariable(eVarNameFlags.MSEBatchIterCalcType, value)
        End Set
    End Property


    Public Property bSaveBiomass As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MSEBatchOutputBiomass))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MSEBatchIterCalcType, value)
        End Set
    End Property

    Public Property bSaveCatch As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MSEBatchOutputCatch))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MSEBatchOutputCatch, value)
        End Set
    End Property

    Public Property bSaveConsumptBio As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MSEBatchOutputConBio))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MSEBatchOutputConBio, value)
        End Set
    End Property

    Public Property bSaveFeedingTime As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MSEBatchOutputFeedingTime))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MSEBatchOutputFeedingTime, value)
        End Set
    End Property

    Public Property bSavePredRate As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MSEBatchOutputPredRate))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MSEBatchOutputPredRate, value)
        End Set
    End Property
    Public Property bSaveFishingMort As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MSEBatchOutputFishingMortRate))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MSEBatchOutputFishingMortRate, value)
        End Set
    End Property



End Class
