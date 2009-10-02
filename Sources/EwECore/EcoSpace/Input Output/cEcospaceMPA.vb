Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceMPA
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Try

            m_dataType = eDataTypes.EcospaceMPA
            m_coreComponent = eCoreComponentType.EcoSpace
            Me.DBID = iDBID

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcospaceMPA, eCoreComponentType.EcoSpace, Index, cCore.NULL_VALUE)

            ResetStatusFlags()

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Array variables

            ' MPAMonth
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.MPAMonth, eStatusFlags.OK, eCoreCounterTypes.nMonths, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceMPA.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceMPA. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Variables by dot '.' operator "

    ''' <summary>
    ''' Get/set if an MPA is OPEN for fishing for a given month.
    ''' </summary>
    Public Property MPAMonth(ByVal iMonth As Integer) As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MPAMonth, iMonth))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MPAMonth, value, iMonth)
        End Set
    End Property

#End Region ' Variables by dot '.' operator

#Region " Status by dot (.) operator "

    Public Property MPAMonthStatus(ByVal iMonth As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAMonth, iMonth)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MPAMonth, value, iMonth)
        End Set
    End Property

#End Region ' Status by dot (.) operator

End Class
