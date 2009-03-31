'==============================================================================
'
' $Log: cPSDParameters.vb,v $
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

            'PSDNumWeightClasses
            meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan), 0)
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

            Me.AllowValidation = True

        Catch ex As Exception

            Debug.Assert(False, ex.Message)
            cLog.Write(Me.ToString & ".New() Error: " & ex.Message)

        End Try

    End Sub

#End Region

#Region "Variables via dot (.) operator"

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

#End Region

End Class
