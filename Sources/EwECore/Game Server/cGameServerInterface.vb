Imports EwEUtils.Core

Public Class cGameServerInterface

    Private m_core As cCore
    ' Private m_dctDataTypes As Dictionary(Of EwEUtils.Core.eDataTypes, Object)
    Private m_dctCoreListData As Dictionary(Of EwEUtils.Core.eDataTypes, cCoreInputOutputList(Of EwECore.cCoreInputOutputBase))

    Private m_dctCoreData As Dictionary(Of EwEUtils.Core.eDataTypes, EwECore.cCoreInputOutputBase)


    Public Sub New(ByRef theCore As cCore)
        m_core = theCore
    End Sub

    Friend Sub Init()

        m_dctCoreListData = New Dictionary(Of EwEUtils.Core.eDataTypes, cCoreInputOutputList(Of EwECore.cCoreInputOutputBase))
        m_dctCoreData = New Dictionary(Of EwEUtils.Core.eDataTypes, EwECore.cCoreInputOutputBase)
        'ecopath
        m_dctCoreListData.Add(eDataTypes.EcoPathGroupInput, m_core.m_EcoPathInputs)
        m_dctCoreListData.Add(eDataTypes.EcoPathGroupOutput, m_core.m_EcoPathOutputs)

        m_dctCoreListData.Add(eDataTypes.FleetInput, m_core.m_FleetsInput)

        'ecosim
        m_dctCoreListData.Add(eDataTypes.EcoSimGroupOutput, m_core.m_EcoSimGroupOutputs)
        m_dctCoreListData.Add(eDataTypes.EcosimFleetOutput, m_core.m_EcosimFleetOutputs)
        m_dctCoreListData.Add(eDataTypes.EcoSimScenario, m_core.m_EcoSimScenarios)
        m_dctCoreListData.Add(eDataTypes.EcosimFisheriesRegulation, m_core.m_EcosimFisheriesRegulations)
        m_dctCoreListData.Add(eDataTypes.EcoSimGroupInput, m_core.m_EcoSimGroups)

        'EcoSpace
        m_dctCoreListData.Add(eDataTypes.EcospaceRegionResults, m_core.m_EcospaceRegionSummaries)
        m_dctCoreListData.Add(eDataTypes.EcospaceGroupOuput, m_core.m_EcospaceGroupOuputs)
        m_dctCoreListData.Add(eDataTypes.EcospaceFleetOuput, m_core.m_EcospaceFleetOutputs)
        m_dctCoreListData.Add(eDataTypes.EcospaceMPA, m_core.m_EcospaceMPAs)
        m_dctCoreListData.Add(eDataTypes.EcospaceHabitat, m_core.m_EcospaceHabitats)

        'MSE 
        m_dctCoreListData.Add(eDataTypes.MSEGroupOutputs, Me.m_core.MSEManager.GroupOutputs)

        ''Wow check this out HACK HACK HACK
        'Dim list As New cCoreInputOutputList(Of cCoreInputOutputBase)(eDataTypes.MSEOutput, 0)
        'list.Add(Me.m_core.MSEManager.Output)
        'm_dctCoreListData.Add(eDataTypes.MSEOutput, list)

        m_dctCoreData.Add(eDataTypes.MSEOutput, Me.m_core.MSEManager.Output)

    End Sub

    Public ReadOnly Property CoreDataList(ByVal DataType As EwEUtils.Core.eDataTypes) As cCoreInputOutputList(Of EwECore.cCoreInputOutputBase)
        Get
            Dim data As cCoreInputOutputList(Of EwECore.cCoreInputOutputBase)
            If Me.m_dctCoreListData.ContainsKey(DataType) Then
                data = m_dctCoreListData.Item(DataType)
            End If
            Return data
        End Get
    End Property

    Public ReadOnly Property CoreData(ByVal DataType As EwEUtils.Core.eDataTypes) As EwECore.cCoreInputOutputBase
        Get
            Dim data As cCoreInputOutputBase
            If m_dctCoreData.ContainsKey(DataType) Then
                data = m_dctCoreData(DataType)
            End If
            Return data
        End Get
    End Property

    Public ReadOnly Property CoreData(ByVal DataType As EwEUtils.Core.eDataTypes, ByVal Index As Integer) As EwECore.cCoreInputOutputBase
        Get
            Dim data As EwECore.cCoreInputOutputBase
            If Me.m_dctCoreListData.ContainsKey(DataType) Then
                data = m_dctCoreListData.Item(DataType).Item(Index)
            End If
            Debug.Assert(data IsNot Nothing, Me.ToString & ".CoreData( " & DataType.ToString & ", " & Index.ToString & " ) not found in core data!")
            Return data
        End Get
    End Property

    Public Function ContainKey(ByVal DataType As EwEUtils.Core.eDataTypes) As Boolean

        If Me.m_dctCoreListData.ContainsKey(DataType) Or Me.m_dctCoreData.ContainsKey(DataType) Then
            Return True
        End If
        Return False

    End Function

End Class
