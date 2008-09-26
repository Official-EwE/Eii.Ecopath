
Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcosimGroupOutput
    Inherits cCoreGroupBase

#Region "Private Data"

    Private m_pred(,) As Single
    Private m_prey(,) As Single

#End Region

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal iGroup As Integer)
        MyBase.New(TheCore)

        Dim val As cValue = Nothing

        Me.DBID = iGroup '????
        Me.Index = iGroup
        Me.m_DataType = eDataTypes.EcoSimGroupOutput

        'no validators
        'MaxRelPB

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimAvgPred, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                 AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimAvgPrey, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                 AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)


        ' Biomass over all the time steps
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimBiomass, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        ' Yield over all the time steps
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimYield, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                 AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimFeedingTime, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                 AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimConsumpBiomass, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                 AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimPredMort, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                 AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimFishMort, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                 AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimTotalMort, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimProdConsump, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimAvgWeight, eStatusFlags.NotEditable, eCoreCounterTypes.nEcosimTimeSteps, _
                                AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        'isPred
        val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.isPred, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, _
                                AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

        'isPrey
        val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.isPrey, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, _
                                AddressOf m_core.GetCoreCounter)
        m_values.Add(val.varName, val)

    End Sub

#End Region

#Region "Overridden base class methods"

    Friend Overrides Function Resize() As Boolean
        MyBase.Resize()

        ReDim m_pred(m_core.nGroups, m_core.nEcosimTimeSteps)
        ReDim m_prey(m_core.nGroups, m_core.nEcosimTimeSteps)

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

    Public Property Biomass(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimBiomass, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimBiomass, value, iTime)
        End Set

    End Property

    Public Property Yield(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimYield, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimYield, value, iTime)
        End Set

    End Property

    Public Property ConsumpBiomass(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimConsumpBiomass, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimConsumpBiomass, value, iTime)
        End Set

    End Property

    Public Property FeedingTime(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFeedingTime, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFeedingTime, value, iTime)
        End Set

    End Property

    Public Property PredMort(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimPredMort, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimPredMort, value, iTime)
        End Set

    End Property
    ''' <summary>
    ''' Predation mort rate + fishing mort rate
    ''' </summary>
    Public Property FishMort(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimFishMort, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimFishMort, value, iTime)
        End Set

    End Property

    Public Property TotalMort(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimTotalMort, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimTotalMort, value, iTime)
        End Set

    End Property

    ''' <summary>
    ''' Production / Consumption (Ecopath GE)
    ''' </summary>
    Public Property ProdConsump(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimProdConsump, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimProdConsump, value, iTime)
        End Set

    End Property

    Public Property AvgWeight(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimAvgWeight, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimAvgWeight, value, iTime)
        End Set

    End Property

    Public Property AvgPredConsumption(ByVal iGroup As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimAvgPred, iGroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimAvgPred, value, iGroup)
        End Set

    End Property

    Public Property AvgPreyConsumption(ByVal igroup As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimAvgPrey, igroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimAvgPrey, value, igroup)
        End Set

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
    Public Property PreyPercentage(ByVal iPreyGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return m_prey(iPreyGroup, iTime)
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".PreyConsumption() " & ex.Message)
            End Try
        End Get

        Set(ByVal value As Single)
            Try
                m_prey(iPreyGroup, iTime) = value
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".PreyConsumption() " & ex.Message)
            End Try
        End Set
    End Property

    ''' <summary>
    ''' Predation rate on this group by a group (predator)
    ''' </summary>
    ''' <param name="iPredGroup">Index of group that predates on this group</param>
    ''' <param name="iTime">Ecosim time step</param>
    ''' <value></value>
    ''' <returns>Predation on this group</returns>
    ''' <remarks></remarks>
    Public Property Predation(ByVal iPredGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return m_pred(iPredGroup, iTime)
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".PredConsumption() " & ex.Message)
            End Try
        End Get

        Set(ByVal value As Single)
            Try
                m_pred(iPredGroup, iTime) = value
            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".PredConsumption() " & ex.Message)
            End Try
        End Set
    End Property

#End Region

End Class
