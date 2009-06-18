'==============================================================================
'
' $Log: cPSDParameters.vb,v $
' Revision 1.10  2009/06/18 23:06:27  joeh
' Change the minimum value of No. of Weight Class from 1 to 2
'
' Revision 1.9  2009/04/28 19:15:01  joeh
' Remove val.Stored = False for PSDEnable
'
' Revision 1.8  2009/04/02 19:51:18  joeh
' Change the minimum value of PSDNumWeightClasses to 1
'
' Revision 1.7  2009/04/02 15:53:36  jeroens
' Added PSD enabled, group included
'
' Revision 1.6  2009/03/31 21:36:14  joeh
' Move all PSD computation routines to a new class cPSDModel
'
' Revision 1.5  2009/03/24 18:58:08  joeh
' Change PSDFirstWeightClass from integer to single
'
' Revision 1.4  2009/03/21 00:31:19  jeroens
' PSD params exposes nWeightClasses
'
' Revision 1.3  2009/03/19 22:23:39  jeroens
' Added Lohrenzen vars
'
' Revision 1.2  2009/03/18 13:25:22  jeroens
' Implemented v1.0
'
' Revision 1.1  2009/03/16 16:55:57  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEUtils.Core
Imports EwECore.ValueWrapper

''' <summary>
''' This class wraps the underlying particle size distribution data structures
''' </summary>
Public Class cPSDParameters
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Public Sub New(ByRef m_core As cCore)
        MyBase.New(m_core)

        Me.m_coreComponent = eCoreComponentType.EcoPath
        Me.m_dataType = eDataTypes.ParticleSizeDistribution

        Try

            Dim val As cValue
            Dim meta As cVariableMetaData

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, m_dataType, m_coreComponent, Index, cCore.NULL_VALUE)

            'no data validation at this time
            Me.AllowValidation = False

            'PSDEnabled
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.PSDEnabled, eStatusFlags.OK, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.PSDEnabled))
            m_values.Add(val.varName, val)

            'PSDNumWeightClasses
            meta = New cVariableMetaData(2, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), 2)
            val = New cValue(New Integer, eVarNameFlags.PSDNumWeightClasses, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.PSDNumWeightClasses))
            m_values.Add(val.varName, val)

            'PSDMortalityType
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), 0)
            val = New cValue(New Integer, eVarNameFlags.PSDMortalityType, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.PSDMortalityType))
            m_values.Add(val.varName, val)

            'PSDFirstWeightClass
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), 0)
            val = New cValue(New Single, eVarNameFlags.PSDFirstWeightClass, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.PSDFirstWeightClass))
            m_values.Add(val.varName, val)

            'ClimateType
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), 0)
            val = New cValue(New Integer, eVarNameFlags.ClimateType, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.ClimateType))
            m_values.Add(val.varName, val)

            'Number of points used in moving average
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan), 0)
            val = New cValue(New Integer, eVarNameFlags.NumPtsMovAvg, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NumPtsMovAvg))
            m_values.Add(val.varName, val)

            ' == ARRAY VARS ==
            'PSDIncluded
            meta = New cVariableMetaData()
            val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.PSDIncluded, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            val.Stored = False
            m_values.Add(val.varName, val)

            Me.AllowValidation = True

        Catch ex As Exception

            Debug.Assert(False, ex.Message)
            cLog.Write(Me.ToString & ".New() Error: " & ex.Message)

        End Try

    End Sub

#End Region

#Region "Variables via dot (.) operator"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether the PSD model is enabled in EwE.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PSDEnabled() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.PSDEnabled))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.PSDEnabled, value)
        End Set
    End Property

    Public Property MortalityType() As ePSDMortalityTypes
        Get
            Return DirectCast(GetVariable(eVarNameFlags.PSDMortalityType), ePSDMortalityTypes)
        End Get

        Set(ByVal value As ePSDMortalityTypes)
            SetVariable(eVarNameFlags.PSDMortalityType, value)
        End Set
    End Property

    Public Property NumWeightClasses() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.PSDNumWeightClasses))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.PSDNumWeightClasses, value)
        End Set
    End Property

    Public Property FirstWeightClass() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.PSDFirstWeightClass))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.PSDFirstWeightClass, value)
        End Set
    End Property

    Public Property ClimateType() As eClimateTypes
        Get
            Return DirectCast(GetVariable(eVarNameFlags.ClimateType), eClimateTypes)
        End Get

        Set(ByVal value As eClimateTypes)
            SetVariable(eVarNameFlags.ClimateType, value)
        End Set
    End Property

    Public Property NumPtsMovAvg() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.NumPtsMovAvg))
        End Get

        Set(ByVal value As Integer)
            SetVariable(eVarNameFlags.NumPtsMovAvg, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a given group is included in the PSD analysis.
    ''' </summary>
    ''' <param name="iGroup">Index of the group.</param>
    ''' -----------------------------------------------------------------------
    Public Property GroupIncluded(ByVal iGroup As Integer) As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.PSDIncluded, iGroup))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.PSDIncluded, value, iGroup)
        End Set
    End Property

#End Region

End Class
