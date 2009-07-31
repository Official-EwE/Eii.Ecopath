
Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcosimGroupOutput
    Inherits cCoreGroupBase

#Region "Private Data"

    Private m_simData As cEcosimDatastructures
    'dictionary of vars and wrappers that directly access the core data
    Private m_coreData As New Dictionary(Of eVarNameFlags, IResultsWrapper)

#End Region

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal EcosimData As cEcosimDatastructures, ByVal iGroup As Integer)
        MyBase.New(TheCore)

        Debug.Assert(TheCore IsNot Nothing)
        Debug.Assert(EcosimData IsNot Nothing)

        m_simData = EcosimData

        Dim val As cValue = Nothing

        Me.DBID = iGroup '????
        Me.Index = iGroup
        Me.m_dataType = eDataTypes.EcoSimGroupOutput

        'See Me.Init() for list of variables

        'Boolean vars use same structures as other vars
        'isPred
        val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.isPred, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, _
                                AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        'isPrey
        val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.isPrey, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, _
                                AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)


        'no validators
        val = New cValue(0, eVarNameFlags.EcosimGroupBiomassStart, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(0, eVarNameFlags.EcosimGroupBiomassEnd, eStatusFlags.OK, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        'no validators
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimGroupCatchEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimGroupCatchStart, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        'no validators
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimGroupValueStart, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimGroupValueEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)


    End Sub


    Public Sub Init()

        'the results arrays of ecosim are redim for each run
        'this means the reference to the results data is lost on each run 
        'so reset the reference
        m_coreData.Clear()

        'cEcosimDataStrucures.ResultsOverTime(var,group,time) Var and Group are fixed
        m_coreData.Add(eVarNameFlags.EcosimBiomass, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.Biomass, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimBiomassRel, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.BiomassRel, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimYield, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.Yield, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimFeedingTime, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.FeedingTime, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimConsumpBiomass, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.ConsumpBiomass, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimPredMort, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.PredMort, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimFishMort, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.FishMort, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimTotalMort, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.TotalMort, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimAvgWeight, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.AvgWeight, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimProdConsump, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.ProdConsump, Me.Index))

        m_coreData.Add(eVarNameFlags.EcosimMortVPred, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.MortVPred, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimMortVFishing, New c3DResultsWrapper2Fixed(m_simData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.MortVFishing, Me.Index))

        'cEcosimDataStrucures.ResultsAvgByPreyPred(var,group,time) Var and Group are fixed
        m_coreData.Add(eVarNameFlags.EcosimAvgPred, New c3DResultsWrapper2Fixed(m_simData.ResultsAvgByPreyPred, cEcosimDatastructures.eEcosimPreyPredResults.Pred, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimAvgPrey, New c3DResultsWrapper2Fixed(m_simData.ResultsAvgByPreyPred, cEcosimDatastructures.eEcosimPreyPredResults.Prey, Me.Index))

        'cEcosimDataStrucures.PredPreyResultsOverTime(var,prey,pred,time) Var and Prey are fixed
        m_coreData.Add(eVarNameFlags.EcosimPredConsumpTime, New c4DResultsWrapper(m_simData.PredPreyResultsOverTime, cEcosimDatastructures.eEcosimPreyPredResults.Consumption, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimPreyPercentageTime, New c4DResultsWrapper(m_simData.PredPreyResultsOverTime, cEcosimDatastructures.eEcosimPreyPredResults.Prey, Me.Index))
        m_coreData.Add(eVarNameFlags.EcosimPredRateTime, New c4DResultsWrapper(m_simData.PredPreyResultsOverTime, cEcosimDatastructures.eEcosimPreyPredResults.Pred, Me.Index))

        'cEcosimDataStrucures.Elect(group,group,time) First Group is fixed
        m_coreData.Add(eVarNameFlags.EcosimElectivityTime, New c3DResultsWrapper(m_simData.Elect, Me.Index))

    End Sub

#End Region

#Region "Overridden base class methods"


    Public Overrides Function GetVariable(ByVal VarName As EwEUtils.Core.eVarNameFlags, Optional ByVal iIndex1 As Integer = -9999, Optional ByVal iIndex2 As Integer = -9999, Optional ByVal iIndex3 As Integer = cCore.NULL_VALUE) As Object

        If Not m_coreData.ContainsKey(VarName) Then
            'NOT in list of sim vars so get the value from the base class GetVariable(...)
            Return MyBase.GetVariable(VarName, iIndex1, iIndex2)
        Else
            'Varname is access directly via the core data
            Return m_coreData.Item(VarName).Value(iIndex1, iIndex2)
        End If

    End Function

#End Region

#Region "Status flag setting"

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray
                        For i = 1 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Str

                        If CStr(value.Value) = "" Then
                            value.Status = eStatusFlags.NotEditable Or eStatusFlags.Null
                        Else
                            value.Status = eStatusFlags.NotEditable Or eStatusFlags.OK
                        End If

                    Case Else
                        value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                End Select

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function

#End Region

#Region "Properties via dot operator"

    ''' <summary>
    ''' Is this igroup a predator of this group
    ''' </summary>
    ''' <param name="iGroup"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property isPred(ByVal iGroup As Integer) As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.isPred, iGroup))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.isPred, value, iGroup)
        End Set

    End Property

    ''' <summary>
    ''' Does this group prey on this iGroup
    ''' </summary>
    ''' <param name="iGroup"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property isPrey(ByVal iGroup As Integer) As Boolean

        Get
            Return CBool(GetVariable(eVarNameFlags.isPrey, iGroup))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.isPrey, value, iGroup)
        End Set

    End Property

    Public ReadOnly Property Biomass(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimBiomass, iTime))
        End Get

    End Property


    Public ReadOnly Property BiomassRel(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimBiomassRel, iTime))
        End Get

    End Property


    Public ReadOnly Property Yield(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimYield, iTime))
        End Get

    End Property

    Public ReadOnly Property ConsumpBiomass(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimConsumpBiomass, iTime))
        End Get

    End Property

    Public ReadOnly Property FeedingTime(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFeedingTime, iTime))
        End Get

    End Property

    Public ReadOnly Property PredMort(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimPredMort, iTime))
        End Get

    End Property
    ''' <summary>
    ''' Predation mort rate + fishing mort rate
    ''' </summary>
    Public ReadOnly Property FishMort(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFishMort, iTime))
        End Get

    End Property

    Public ReadOnly Property TotalMort(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimTotalMort, iTime))
        End Get

    End Property

    ''' <summary>
    ''' Production / Consumption (Ecopath GE)
    ''' </summary>
    Public ReadOnly Property ProdConsump(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimProdConsump, iTime))
        End Get

    End Property

    Public ReadOnly Property AvgWeight(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimAvgWeight, iTime))
        End Get

    End Property


    Public ReadOnly Property MortVPred(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimMortVPred, iTime))
        End Get

    End Property



    Public ReadOnly Property MortVFishing(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimMortVFishing, iTime))
        End Get

    End Property


    Public ReadOnly Property AvgPredConsumption(ByVal iGroup As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimAvgPred, iGroup))
        End Get

    End Property

    Public ReadOnly Property AvgPreyConsumption(ByVal igroup As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimAvgPrey, igroup))
        End Get

    End Property



#End Region

#Region "Variables arrayed by group and time"

    ''' <summary>
    ''' Percentage of a group this group consumes
    ''' </summary>
    ''' <param name="iPreyGroup">Index of group that this group preys on</param>
    ''' <param name="iTime">Ecosim time step</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property PreyPercentage(ByVal iPreyGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return CSng(GetVariable(eVarNameFlags.EcosimPreyPercentageTime, iPreyGroup, iTime))
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".PreyPercentage() " & ex.Message)
            End Try
        End Get

    End Property

    ''' <summary>
    ''' Predation rate on this prey by a pred 
    ''' </summary>
    ''' <param name="iPredGroup">Index of group that predates on this group</param>
    ''' <param name="iTime">Ecosim time step</param>
    ''' <value></value>
    ''' <returns>Predation on this group</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Predation(ByVal iPredGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return CSng(GetVariable(eVarNameFlags.EcosimPredRateTime, iPredGroup, iTime))
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".Predation() " & ex.Message)
            End Try
        End Get

    End Property


    Public ReadOnly Property Consumption(ByVal iPredGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return CSng(GetVariable(eVarNameFlags.EcosimPredConsumpTime, iPredGroup, iTime))
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".Consumption() " & ex.Message)
            End Try
        End Get

    End Property

    Public ReadOnly Property Electivity(ByVal iPredGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return CSng(GetVariable(eVarNameFlags.EcosimElectivityTime, iPredGroup, iTime))
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".Electivity() " & ex.Message)
            End Try
        End Get

    End Property

#End Region

#Region "Summary values"

    Public Property BiomassStart() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupBiomassStart))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupBiomassStart, value)
        End Set
    End Property

    Public Property BiomassEnd() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupBiomassEnd))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupBiomassEnd, value)
        End Set
    End Property


    Public Property CatchStart(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupCatchStart, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupCatchStart, value, iFleet)
        End Set
    End Property


    Public Property CatchEnd(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupCatchEnd, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupCatchEnd, value, iFleet)
        End Set
    End Property


    Public Property ValueStart(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupValueStart, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupValueStart, value, iFleet)
        End Set
    End Property

    Public Property ValueEnd(ByVal iFleet As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimGroupValueEnd, iFleet))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimGroupValueEnd, value, iFleet)
        End Set
    End Property

#End Region

End Class
