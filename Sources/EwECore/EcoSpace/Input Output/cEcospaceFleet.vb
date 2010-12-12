'==============================================================================
'
' $Log: cEcospaceFleet.vb,v $
' Revision 1.2  2009/01/16 18:30:23  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:21  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/05/29 22:22:44  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.4  2007/07/08 18:23:01  jeroens
' * Fixing todo's
'
' Revision 1.3  2007/06/05 02:42:26  jeroens
' + Added TotalEddMultiplier, exposing var SEmult
'
' Revision 1.2  2007/05/22 13:24:35  jeroens
' * Nitty-gritty
'
' Revision 1.1  2007/05/01 17:12:33  joeb
' Changed directory structure
'
' Revision 1.5  2007/03/28 01:16:33  jeroens
' * Changed all status modification access from Public to Friend
'
' Revision 1.4  2007/01/26 03:04:20  jeroens
' + Exposed EffectivePower, HabitatFishery, MPAFishery
'
' Revision 1.3  2007/01/23 16:58:33  jeroens
' + Added DBID to constructor
'
' Revision 1.2  2007/01/20 03:19:08  jeroens
' + Jogged down options for exposing cell data
'
' Revision 1.1  2007/01/19 04:13:14  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceFleet
    Inherits cCoreInputOutputBase

#Region " Constructor "

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Try

            Me.m_dataType = eDataTypes.EcospaceFleet
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.DBID = iDBID

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' EffectivePower
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.EffectivePower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' SEmult
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.SEmult, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Array variables

            ' HabitatFishery
            meta = New cVariableMetaData(False)
            val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.HabitatFishery, eStatusFlags.Null, eCoreCounterTypes.nHabitats, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' MPAFishery
            meta = New cVariableMetaData(False)
            val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.MPAFishery, eStatusFlags.Null, eCoreCounterTypes.nMPAs, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceGroup.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceGroup. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Properties by dot (.) operator "

    Public Property EffectivePower() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EffectivePower))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EffectivePower, value)
        End Set
    End Property

    Public Property HabitatFishery(ByVal iHabitat As Integer) As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.HabitatFishery, iHabitat))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.HabitatFishery, value, iHabitat)
        End Set
    End Property

    Public Property MPAFishery(ByVal iMPA As Integer) As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.MPAFishery, iMPA))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.MPAFishery, value, iMPA)
        End Set
    End Property

    Public Property TotalEffMultiplier() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.SEmult))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.SEmult, value)
        End Set
    End Property

#End Region ' Properties by dot (.) operator

#Region " Status by dot (.) operator "

    Public Property EffectivePowerStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EffectivePower)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EffectivePower, value)
        End Set
    End Property

    Public Property HabitatFisheryStatus(ByVal iHabitat As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.HabitatFishery, iHabitat)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.HabitatFishery, value, iHabitat)
        End Set
    End Property

    Public Property MPAFisheryStatus(ByVal iMPA As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MPAFishery, iMPA)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MPAFishery, value, iMPA)
        End Set
    End Property

#End Region ' Status by dot (.) operator 

End Class
