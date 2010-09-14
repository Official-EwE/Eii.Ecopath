#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace.Advection

    Public Class cAdvectionParameters
        Inherits cCoreInputOutputBase


        Public Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
            MyBase.New(theCore)

            Me.AllowValidation = False
            Me.DBID = DBID
            Me.m_dataType = eDataTypes.EcospaceAdvectionParameters
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.AllowValidation = False

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.FishingPolicyParameters, _
                                                        eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

            Dim val As cValue
            Dim meta As cVariableMetaData

            ' Coriolis
            meta = New cVariableMetaData(-1, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.Coriolis, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.Coriolis))
            val.Stored = False
            m_values.Add(val.varName, val)

            ' XVel
            meta = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.XVelocity, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.XVelocity))
            val.Stored = False
            m_values.Add(val.varName, val)

            ' YVel
            meta = New cVariableMetaData(Single.MinValue, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.YVelocity, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.YVelocity))
            val.Stored = False
            m_values.Add(val.varName, val)

            Me.ResetStatusFlags()

            AllowValidation = True

        End Sub

        Public Property Coriolis() As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.Coriolis))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.Coriolis, value)
            End Set
        End Property

        Public Property XVelocity() As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.XVelocity))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.XVelocity, value)
            End Set
        End Property

        Public Property YVelocity() As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.YVelocity))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.YVelocity, value)
            End Set
        End Property

    End Class

End Namespace
