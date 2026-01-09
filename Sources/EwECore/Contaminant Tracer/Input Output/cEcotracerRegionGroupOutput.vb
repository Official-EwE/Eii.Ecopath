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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Public Class cEcotracerRegionGroupOutput
    Inherits cCoreInputOutputBase

    Private m_TracerData As cContaminantTracerDataStructures
    Private m_nRegions As Integer = 0
    Private m_nGroups As Integer = 0
    Private m_nTimeSteps As Integer = 0

#Region "Constructor"

    Public Sub New(core As cCore, TracerData As cContaminantTracerDataStructures)
        MyBase.New(core)

        Me.m_dataType = eDataTypes.EcotracerSimOutput
        Me.m_coreComponent = eCoreComponentType.Ecotracer

        Me.DBID = 1
        Me.Index = 1

        Me.m_TracerData = TracerData

    End Sub

#End Region

#Region "Implementation of GetVariable() GetVariable() GetStatus() SetStatus()"

    Public Overloads Function GetVariable(varName As eVarNameFlags, iRegion As Integer, iGroup As Integer, iTimeStep As Integer) As Single

        Try
            Select Case varName
                Case eVarNameFlags.Concentration
                    Return Me.m_TracerData.TracerConcByRegion(iRegion, iGroup, iTimeStep)
                Case eVarNameFlags.CEnvironment
                    Return Me.m_TracerData.TracerConcByRegion(iRegion, 0, iTimeStep)
                Case eVarNameFlags.CSum
                    Return Me.m_TracerData.TracerConcByRegion(iRegion, Me.m_nGroups + 1, iTimeStep)
                Case eVarNameFlags.ConcBio
                    Return Me.m_TracerData.TracerCBRegion(iRegion, iGroup, iTimeStep)
                Case eVarNameFlags.CBEnvironment
                    Return Me.m_TracerData.TracerCBRegion(iRegion, 0, iTimeStep)

            End Select

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function

    Public Overloads Function SetVariable(varName As eVarNameFlags, newValue As Single, iRegion As Integer, iGroup As Integer, iTimeStep As Integer) As Boolean

        Try
            Debug.Assert(False, "cEcotracerRegionGroupOutput.setVaraible() not supported at this time.")
            Return False
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Public Overloads Function GetStatus(varName As eVarNameFlags, iRegion As Integer, iGroup As Integer, iTimeStep As Integer) As eStatusFlags
        Return eStatusFlags.OK Or eStatusFlags.NotEditable
    End Function

    Public Overloads Function SetStatus(varName As eVarNameFlags, newValue As eStatusFlags, iRegion As Integer, iGroup As Integer, iTimeStep As Integer) As Boolean
        Debug.Assert(False, "Not implemented yet.")
    End Function
#End Region

#Region "Variable via dot '.' operator"

    Public Property Concentration(iRegion As Integer, iGroup As Integer, iTimeStep As Integer) As Single
        Get
            Try
                Return Me.GetVariable(eVarNameFlags.Concentration, iRegion, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(value As Single)
            Try
                Me.SetVariable(eVarNameFlags.Concentration, value, iRegion, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property


    Public Property CB(iRegion As Integer, iGroup As Integer, iTimeStep As Integer) As Single
        Get
            Try
                Return Me.GetVariable(eVarNameFlags.ConcBio, iRegion, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(value As Single)
            Try
                Me.SetVariable(eVarNameFlags.ConcBio, value, iRegion, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property

    Public Property CEnvironment(iRegion As Integer, iTimeStep As Integer) As Single
        Get
            Try
                Return Me.GetVariable(eVarNameFlags.CEnvironment, iRegion, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(value As Single)
            Try
                Me.SetVariable(eVarNameFlags.CEnvironment, value, iRegion, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property


    Public Property CBEnvironment(iRegion As Integer, iTimeStep As Integer) As Single

        Get
            Try
                Return Me.GetVariable(eVarNameFlags.CBEnvironment, iRegion, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get

        Set(value As Single)
            Try
                Me.SetVariable(eVarNameFlags.CBEnvironment, value, iRegion, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

    End Property

#End Region

#Region "Status Flags via dot '.' operator"

    Public Property ConcentrationStatus(iRegion As Integer, iGroup As Integer, iTimeStep As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.Concentration, iRegion, iGroup, iTimeStep)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Concentration, value, iRegion, iGroup, iTimeStep)
        End Set
    End Property

    Public Property CEnvironmentStatus(iRegion As Integer, iTimeStep As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.CEnvironment, iRegion, 0, iTimeStep)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CEnvironment, value, iRegion, 0, iTimeStep)
        End Set
    End Property

    Public Property CSumStatus(iRegion As Integer, iTimeStep As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.CSum, iRegion, Me.m_nGroups + 1, iTimeStep)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CSum, value, iRegion, Me.m_nGroups + 1, iTimeStep)
        End Set
    End Property

#End Region

End Class
