' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSY

    Public Class cMSYDataStructures

#Region " Constants "

        Public Const F_MAX As Single = 5.0 '3.0
        Public Const F_STEPSIZE As Single = 0.03

#End Region ' Constants

#Region " Private variables "

        Private m_epData As cEcopathDataStructures
        Private m_simData As cEcosimDatastructures

#End Region ' Private variables

#Region "Public Variables"

        Public nYearsPerTrial As Integer = 40
        Public iSelGroupFleet As Integer
        Public ValueBase As Single

        ''' <summary>Groups that have their Biomass forced.</summary>
        Public ForceGroupB() As Boolean

        Public AssessmentType As eMSYAssessmentTypes

        Public RunLengthMode As eMSYRunLengthModeTypes = eMSYRunLengthModeTypes.FixedF
        Public MaxRelF As Single

        Public FStepSize As Single

        Public FSelectionMode As eMSYFSelectionModeType

        'Public bb() As Single

        Public lstResults As List(Of cMSYFResult)
        Public BaseLineResult As cMSYFResult = Nothing
        Public Optimum As cMSYOptimum = Nothing

        ''' <summary>
        ''' Type of MSY Run FMSY or SingleRunMSY
        ''' </summary>
        ''' <remarks></remarks>
        Public MSYRunType As eMSYRunTypes

        Public bStopRun As Boolean

#End Region

#Region "Public Methods"

        Public Sub New(EcopathData As cEcopathDataStructures, EcosimData As cEcosimDatastructures)
            Me.m_epData = EcopathData
            Me.m_simData = EcosimData
        End Sub

        Public Sub RedimVars()

            Try
                Debug.Assert(Me.m_simData IsNot Nothing, Me.ToString & ".RedimVars() Invalid Ecosim data object!")
                Debug.Assert(Me.m_epData IsNot Nothing, Me.ToString & ".RedimVars() Invalid Ecopath data object!")

                'An Ecosim scenario is about to be loaded by the database
                'Redim variables for data

                ReDim Me.ForceGroupB(Me.nGroups)

                Me.lstResults = New List(Of cMSYFResult)
                Me.SetDefaultParameters()

            Catch ex As Exception

            End Try

        End Sub

        Public Sub SetDefaultParameters()
            Me.nYearsPerTrial = 40
            Me.AssessmentType = eMSYAssessmentTypes.StationarySystem
            Me.MaxRelF = F_MAX
            Me.FStepSize = F_STEPSIZE
            Me.FSelectionMode = eMSYFSelectionModeType.Groups
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property nGroups() As Integer
            Get
                Return Me.m_epData.NumGroups
            End Get
        End Property

        Public ReadOnly Property nTimeSteps() As Integer
            Get
                Return Me.m_simData.NTimes
            End Get
        End Property

        Public ReadOnly Property nYears() As Integer
            Get
                Return Me.m_simData.NumYears
            End Get
        End Property

        Public ReadOnly Property nFleets() As Integer
            Get
                Return Me.m_epData.NumFleet
            End Get
        End Property

        Public ReadOnly Property nLiving() As Integer
            Get
                Return Me.m_epData.NumLiving
            End Get
        End Property

#End Region

    End Class

End Namespace
