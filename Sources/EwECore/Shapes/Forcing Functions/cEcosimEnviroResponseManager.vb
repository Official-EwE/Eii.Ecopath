' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports EwEUtils.Core

#End Region ' Imports


''' <summary>
''' Manager class to handle Ecosim Environmental Response functions
''' </summary>
Public Class cEcosimEnviroResponseManager
    Inherits cCoreInputOutputBase
    Implements IEnvironmentalResponseManager

    Private m_simData As cEcosimDatastructures
    Private m_medData As cMediationDataStructures

    Public Sub New(ByVal core As cCore)
        MyBase.New(core)
        Me.m_coreComponent = eCoreComponentType.EcosimResponseInteractionManager
        Me.m_dataType = eDataTypes.EcospaceMapResponse
    End Sub

    Friend Sub Init(ByVal EocsimData As cEcosimDatastructures, ByVal MediationData As cMediationDataStructures)
        Me.m_simData = EocsimData
        Me.m_medData = MediationData
        Me.m_simData.lstEnviroInputData = New List(Of IEnviroInputData)
    End Sub

    Public Sub Load(manager As cForcingFunctionShapeManager)
        Me.LoadFromCoreData(manager)
    End Sub

    Private Sub LoadFromCoreData(manager As cForcingFunctionShapeManager)

        'populate the list of IEnviroInputData objects that the user will interact with 
        'to change region related parameters from the interface
        For iEnv As Integer = 1 To manager.Count
            Try
                Dim EnviroData As New cEcosimEnviroInputData(Me, manager.Item(iEnv - 1))

                EnviroData.Init(Me.m_simData.CapEnvResData, Me.m_simData)
                EnviroData.IsDriverActive = True
                For iGroup As Integer = 1 To Me.m_simData.nGroups
                    EnviroData.ResponseIndexForGroup(iGroup) = Me.m_simData.EnvRespFuncIndex(iEnv, iGroup)
                Next
                Me.m_simData.lstEnviroInputData.Add(EnviroData)

            Catch ex As Exception
                Debug.Assert(False, "LoadFromCoreData Error: " & ex.Message)
                ' bSuccess = False
            End Try

        Next iEnv

        'update the stuff underneath with the newly loaded data
        Me.Update()

    End Sub

    Public Function OnChanged() As Boolean Implements IEnvironmentalResponseManager.onChanged

        Debug.Print(Me.ToString + ".OnChanged() not implemented in Core yet!")

        Try
            For Each env As cEcosimEnviroInputData In Me.m_simData.lstEnviroInputData
                For iGroup As Integer = 1 To Me.m_simData.nGroups
                    Me.m_simData.EnvRespFuncIndex(env.Index, iGroup) = env.ResponseIndexForGroup(iGroup)
                Next iGroup
            Next env
        Catch ex As Exception
            Debug.Assert(False, "Update Error: " & ex.Message)
            ' bSuccess = False
        End Try

        Try
            Me.m_core.onChanged(Me)
            Return True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".OnChanged() Exception: " + ex.Message)
        End Try

        Return False

    End Function

    Public ReadOnly Property nInputData() As Integer Implements IEnvironmentalResponseManager.nEnviroData
        Get
            Return Me.m_simData.lstEnviroInputData.Count
        End Get
    End Property


    Public ReadOnly Property InputData(ByVal iDataIndex As Integer) As IEnviroInputData Implements IEnvironmentalResponseManager.EnviroData
        Get
            If iDataIndex > 0 And iDataIndex <= Me.nInputData Then
                Return Me.m_simData.lstEnviroInputData(iDataIndex - 1)
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

End Class
