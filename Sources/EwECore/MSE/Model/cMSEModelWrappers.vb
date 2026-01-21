

Option Strict On

Imports System.IO
Imports EwECore
Imports EwECore.Ecosim
Imports EwECore.ExternalData
Imports EwECore.Plugins
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Logging
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace MSE

    Public Interface IMSEModelWrapper

        Sub Init(Core As cCore, Ecosim As cEcosimModel, EcoSpace As cEcoSpace)
        'Sub Init(Ecospace As cEcoSpace)

        Function InitForRun(ByVal bFullInitialization As Boolean) As Boolean

        Sub InitForTrial()

        Function Run() As Boolean

        Function SetBaseFFromGear() As Boolean

        Function SetFtimeFromGear(ByVal t As Integer, ByVal QYear() As Single, ByVal PredEffort As Boolean, Optional ForcedDiscards As Boolean = False) As Boolean

        Function CatchbyGroupFleetTimeStep(igrp As Integer, iFleet As Integer, iyear As Integer) As Single

        Property SearchMode As eSearchModes

        WriteOnly Property onModelTimeStep As onModelTimeStepDelegate

        Delegate Sub onModelTimeStepDelegate(ByVal iTime As Integer)

    End Interface

    Public Enum eModelTypes
        Ecosim
        EcoSpace
    End Enum

    Public NotInheritable Class MSEModelFactory

        Shared Function ModelFactory(Type As eModelTypes) As IMSEModelWrapper

            Select Case Type
                Case eModelTypes.Ecosim
                    Return New cMSEEcoSimWrapper()
                Case eModelTypes.EcoSpace
                    Return New cMSEEcoSpaceWrapper()
            End Select

            Return Nothing

        End Function

    End Class

    Public Class cMSEEcoSimWrapper
        Implements IMSEModelWrapper

        Private m_Ecosim As cEcosimModel

        Private m_OnModelTimeStepDelegate As IMSEModelWrapper.onModelTimeStepDelegate = Nothing


        Private WriteOnly Property IMSEModelWrapper_onModelTimeStep As IMSEModelWrapper.onModelTimeStepDelegate Implements IMSEModelWrapper.onModelTimeStep
            Set(value As IMSEModelWrapper.onModelTimeStepDelegate)
                m_OnModelTimeStepDelegate = value
            End Set
        End Property

        Public Property SearchMode As eSearchModes Implements IMSEModelWrapper.SearchMode
            Get
                Return m_Ecosim.SearchData.SearchMode
            End Get
            Set(value As eSearchModes)
                m_Ecosim.SearchData.SearchMode = value
            End Set
        End Property

        Public Sub Init(Core As cCore, Ecosim As cEcosimModel, EcoSpace As cEcoSpace) Implements IMSEModelWrapper.Init
            Me.m_Ecosim = Ecosim
        End Sub


        Public Function InitForRun(ByVal bFullInitialization As Boolean) As Boolean Implements IMSEModelWrapper.InitForRun
            Me.m_Ecosim.Init(bFullInitialization)
            Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onEcosimTimestep
        End Function

        Public Function SetBaseFFromGear() As Boolean Implements IMSEModelWrapper.SetBaseFFromGear
            Me.m_Ecosim.SetBaseFFromGear()
        End Function



        Private Sub onEcosimTimestep(iTime As Long, data As cEcoSimResults)
            Try
                If m_OnModelTimeStepDelegate <> Nothing Then
                    m_OnModelTimeStepDelegate(CInt(iTime))
                End If
            Catch ex As ArgumentException

            End Try
        End Sub

        Public Function Run() As Boolean Implements IMSEModelWrapper.Run
            Dim bRetval As Boolean
            bRetval = Me.m_Ecosim.Run()
            Me.m_Ecosim.TimeStepDelegate = Nothing
            Return bRetval
        End Function

        Public Function SetFtimeFromGear(t As Integer, QYear() As Single, PredEffort As Boolean, Optional ForcedDiscards As Boolean = False) As Boolean Implements IMSEModelWrapper.SetFtimeFromGear
            Me.m_Ecosim.SetFtimeFromGear(t, QYear, PredEffort, ForcedDiscards)
        End Function

        Public Sub InitForTrial() Implements IMSEModelWrapper.InitForTrial
            Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onEcosimTimestep
        End Sub

        Public Function CatchbyGroupFleetTimeStep(igrp As Integer, iFlt As Integer, iTime As Integer) As Single Implements IMSEModelWrapper.CatchbyGroupFleetTimeStep
            Return Me.m_Ecosim.EcosimData.ResultsSumCatchByGroupGear(igrp, iFlt, iTime)
        End Function
    End Class


    Public Class cMSEEcoSpaceWrapper
        Implements IMSEModelWrapper


        Private m_Ecospace As cEcoSpace
        Private m_Core As cCore

        Private m_OnModelTimeStepDelegate As IMSEModelWrapper.onModelTimeStepDelegate = Nothing

        Public WriteOnly Property onModelTimeStep As IMSEModelWrapper.onModelTimeStepDelegate Implements IMSEModelWrapper.onModelTimeStep
            Set(value As IMSEModelWrapper.onModelTimeStepDelegate)
                Me.m_OnModelTimeStepDelegate = value
            End Set
        End Property

        Public Property SearchMode As eSearchModes Implements IMSEModelWrapper.SearchMode
            Get
                Return m_Ecospace.SearchData.SearchMode
            End Get
            Set(value As eSearchModes)
                m_Ecospace.SearchData.SearchMode = value
            End Set
        End Property

        Public Sub Init(Core As cCore, Ecosim As cEcosimModel, EcoSpace As cEcoSpace) Implements IMSEModelWrapper.Init
            m_Core = Core
            m_Ecospace = EcoSpace
        End Sub

        Public Sub InitForTrial() Implements IMSEModelWrapper.InitForTrial
            'Throw New NotImplementedException()
            'Me.m_Ecospace.SearchData.initForRun(Me.m_Ecospace.EcoPathData, Me.m_Ecospace.EcoSimData)
        End Sub

        Public Function InitForRun(ByVal bFullInitialization As Boolean) As Boolean Implements IMSEModelWrapper.InitForRun
            'Throw New NotImplementedException()
            'm_Ecospace.TimeStepDelegate = AddressOf EcoSpaceTimeStepDelegate

        End Function

        Public Function SetBaseFFromGear() As Boolean Implements IMSEModelWrapper.SetBaseFFromGear
            'throw New NotImplementedException()
        End Function

        Public Function Run() As Boolean Implements IMSEModelWrapper.Run
            Return Me.m_Core.RunEcospace(AddressOf EcoSpaceCoreTimeStepDelegate, RunOnThread:=False)
        End Function

        Public Function SetFtimeFromGear(t As Integer, QYear() As Single, PredEffort As Boolean, Optional ForcedDiscards As Boolean = False) As Boolean Implements IMSEModelWrapper.SetFtimeFromGear
            'Throw New NotImplementedException()
        End Function

        Private Sub EcoSpaceCoreTimeStepDelegate(ByRef EcospaceResults As cEcospaceTimestep)
            Try
                If m_OnModelTimeStepDelegate <> Nothing Then
                    m_OnModelTimeStepDelegate(EcospaceResults.iTimeStep)
                End If
            Catch ex As ArgumentException

            End Try
        End Sub

        Public Function CatchbyGroupFleetTimeStep(igrp As Integer, iFlt As Integer, iTime As Integer) As Single Implements IMSEModelWrapper.CatchbyGroupFleetTimeStep
            Return Me.m_Ecospace.EcoSpaceData.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, iFlt, igrp, iTime)
        End Function

        'Private Sub EcoSpaceTimeStepDelegate(ByVal iTime As Integer)
        '    Try
        '        If m_OnModelTimeStepDelegate <> Nothing Then
        '            m_OnModelTimeStepDelegate(iTime)
        '        End If
        '    Catch ex As ArgumentException

        '    End Try
        'End Sub
    End Class

End Namespace
