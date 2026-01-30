' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Manager class to handle Ecosim Mortality Response functions
''' </summary>
Public Class cEcosimMortalityResponseManager
    Inherits cCoreInputOutputBase
    Implements IEnvironmentalResponseManager

    Private m_simData As cEcosimDatastructures
    Private m_medData As cMediationDataStructures

    Private m_lstEnviroData As List(Of IEnviroInputData)

    Public Sub New(ByVal core As cCore)
        MyBase.New(core)
        Me.m_coreComponent = eCoreComponentType.EcosimMortalityResponseInteractionManager
        Me.m_dataType = eDataTypes.EcosimMortalityResponseFunctionManager
    End Sub

    Friend Sub Init(ByVal EocsimData As cEcosimDatastructures, ByVal MediationData As cMediationDataStructures)
        Me.m_simData = EocsimData
        Me.m_medData = MediationData
        Me.m_lstEnviroData = New List(Of IEnviroInputData)
    End Sub

    Public Sub Load(manager As cForcingFunctionShapeManager)
        Me.LoadFromCoreData(manager)
    End Sub

    Private Sub LoadFromCoreData(manager As cForcingFunctionShapeManager)

        If Me.m_lstEnviroData Is Nothing Then
            Me.m_lstEnviroData = New List(Of IEnviroInputData)
        End If
        Me.m_lstEnviroData.Clear()

        'populate the list of IEnviroInputData objects that the user will interact with 
        'to change region related parameters from the interface
        For iEnv As Integer = 1 To manager.Count
            Try
                Dim EnviroData As New cEcosimEnviroInputData(Me, manager.Item(iEnv - 1))

                EnviroData.Init(Me.m_simData.CapEnvResData, Me.m_simData)
                EnviroData.IsCapacityEnabled = True
                For iGroup As Integer = 1 To Me.m_simData.nGroups
                    EnviroData.ResponseIndexForGroup(iGroup, False) = Me.m_simData.MortalityRespFuncIndex(EnviroData.Index, iGroup)
                Next
                'Me.m_simData.lstEnviroInputData.Add(EnviroData)
                Me.m_lstEnviroData.Add(EnviroData)

            Catch ex As Exception
                Debug.Assert(False, "LoadFromCoreData Error: " & ex.Message)
                ' bSuccess = False
            End Try

        Next iEnv

        'update the stuff underneath with the newly loaded data
        Me.Update()

    End Sub

    Public Function OnChanged() As Boolean Implements IEnvironmentalResponseManager.onChanged

        Dim bSuccess As Boolean = True

        Try
            For Each env As cEcosimEnviroInputData In Me.m_lstEnviroData
                For iGroup As Integer = 1 To Me.m_simData.nGroups

                    'Update the core data
                    Me.m_simData.MortalityRespFuncIndex(env.Index, iGroup) = env.ResponseIndexForGroup(iGroup)

                Next iGroup
            Next env
            Me.m_core.onChanged(Me, eMessageType.DataModified)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".OnChanged() Exception: " + ex.Message)
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    Public ReadOnly Property nInputData() As Integer Implements IEnvironmentalResponseManager.nEnviroData
        Get
            Return Me.m_lstEnviroData.Count
        End Get
    End Property


    Public ReadOnly Property InputData(ByVal iDataIndex As Integer) As IEnviroInputData Implements IEnvironmentalResponseManager.EnviroData
        Get
            If iDataIndex > 0 And iDataIndex <= Me.nInputData Then
                Return Me.m_lstEnviroData(iDataIndex - 1)
            End If
            Return Nothing
        End Get

    End Property

    Public ReadOnly Property EnviroData(layer As cEcospaceLayer) As IEnviroInputData Implements IEnvironmentalResponseManager.EnviroData
        Get
            Debug.Assert(False, "Oppss Map(layer As cEcospaceLayer) should not be called for this implementation!")
            Return Nothing
        End Get
    End Property

    Public Sub Update()
        Debug.Print(Me.ToString + ".Update() not implemented yet!")
    End Sub


    Public ReadOnly Property MediationData As cMediationDataStructures Implements IEnvironmentalResponseManager.MediationData
        Get
            Return Me.m_medData
        End Get
    End Property


    Public ReadOnly Property SimData As cEcosimDatastructures Implements IEnvironmentalResponseManager.SimData
        Get
            Return Me.m_simData
        End Get
    End Property


    Public ReadOnly Property SpaceData As cEcospaceDataStructures Implements IEnvironmentalResponseManager.SpaceData
        Get
            Debug.Assert(False, Me.ToString + ".SpaceData() not valid for this implementation.")
            Return Nothing
        End Get
    End Property


    Private Sub CheckResponseOverlap(EnviroFuntion As cEcosimEnviroInputData, iGrp As Integer)
        Dim msg As New Text.StringBuilder

        'is there overlap of the response function and data at t=1
        If EnviroFuntion.ResponseFunction(iGrp, 1) = 0 Then
            'Nope 
            msg.AppendLine(My.Resources.CoreMessages.ECOSIM_RESPONSE_NO_OVERLAP)
        End If

        'If Me.m_simData.QBoutside(iGrp) <> 0 Then
        '    Dim break As String = String.Empty
        '    If msg.Length > 0 Then break = EwEUtils.Utilities.cStringUtils.vbCrLf
        '    msg.Append(break & My.Resources.CoreMessages.ECOSIM_RESPONSE_DIET)
        'End If

        If Not String.IsNullOrEmpty(msg.ToString) Then
            Me.m_core.Messages.SendMessage(New cMessage(msg.ToString, eMessageType.DataModified, eCoreComponentType.EcosimCapacityResponseInteractionManager, eMessageImportance.Warning))
        End If

    End Sub

    Public Overrides Sub Clear()
        MyBase.Clear()
        If Me.m_lstEnviroData IsNot Nothing Then
            Me.m_lstEnviroData.Clear()
            'Me.m_lstEnviroData = Nothing
        End If
    End Sub

    Public Sub UpdateLayer(layer As cEcospaceLayer) Implements IEnvironmentalResponseManager.UpdateLayer
        'Nope not for Ecosim
    End Sub
End Class
