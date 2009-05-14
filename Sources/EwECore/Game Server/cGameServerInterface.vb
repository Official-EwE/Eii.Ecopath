Imports EwEUtils.Core

Public Class cGameServerInterface

    Private m_core As cCore
    ' Private m_dctDataTypes As Dictionary(Of EwEUtils.Core.eDataTypes, Object)
    Private m_dctCoreData As Dictionary(Of EwEUtils.Core.eDataTypes, cCoreInputOutputList(Of EwECore.cCoreInputOutputBase))

    Public Sub New(ByRef theCore As cCore)
        m_core = theCore
    End Sub

    Friend Sub Init()

        m_dctCoreData = New Dictionary(Of EwEUtils.Core.eDataTypes, cCoreInputOutputList(Of EwECore.cCoreInputOutputBase))

        'ecopath
        m_dctCoreData.Add(eDataTypes.EcoPathGroupInput, m_core.m_EcoPathInputs)
        m_dctCoreData.Add(eDataTypes.EcoPathGroupOutput, m_core.m_EcoPathOutputs)

        m_dctCoreData.Add(eDataTypes.FleetInput, m_core.m_FleetsInput)

        'ecosim
        m_dctCoreData.Add(eDataTypes.EcoSimGroupOutput, m_core.m_EcoSimGroupOutputs)
        m_dctCoreData.Add(eDataTypes.EcosimFleetOutput, m_core.m_EcosimFleetOutputs)
        m_dctCoreData.Add(eDataTypes.EcoSimScenario, m_core.m_EcoSimScenarios)
        m_dctCoreData.Add(eDataTypes.EcosimFisheriesRegulation, m_core.m_EcosimFisheriesRegulations)
        m_dctCoreData.Add(eDataTypes.EcoSimGroupInput, m_core.m_EcoSimGroups)

        'EcoSpace
        m_dctCoreData.Add(eDataTypes.EcospaceRegionResults, m_core.m_EcospaceRegionSummaries)
        m_dctCoreData.Add(eDataTypes.EcospaceGroupOuput, m_core.m_EcospaceGroupOuputs)
        m_dctCoreData.Add(eDataTypes.EcospaceFleetOuput, m_core.m_EcospaceFleetOutputs)
        m_dctCoreData.Add(eDataTypes.EcospaceMPA, m_core.m_EcospaceMPAs)
        m_dctCoreData.Add(eDataTypes.EcospaceHabitat, m_core.m_EcospaceHabitats)

        'MSE 
        m_dctCoreData.Add(eDataTypes.MSEGroupOutputs, Me.m_core.MSEManager.GroupOutputs)

    End Sub


    Public ReadOnly Property CoreData() As Dictionary(Of EwEUtils.Core.eDataTypes, cCoreInputOutputList(Of EwECore.cCoreInputOutputBase))
        Get
            Return m_dctCoreData
        End Get
    End Property


End Class
