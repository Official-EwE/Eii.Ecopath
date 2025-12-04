

Option Strict On

Imports System.IO
Imports EwECore
Imports EwECore.Ecosim
Imports EwECore.ExternalData
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Logging
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace MSE

    Public Interface IMSEModelWrapper

        Sub Init(Ecosim As Ecosim.cEcosimModel)
        Sub Init(Ecospace As cEcoSpace)

        Function InitForRun() As Boolean

        Function SetBaseFFromGear() As Boolean

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

            End Select

            Return Nothing

        End Function

    End Class

    Public Class cMSEEcoSimWrapper
        Implements IMSEModelWrapper

        Private m_Ecosim As cEcosimModel

        Private m_OnTimeStepDelegate As IMSEModelWrapper.onModelTimeStepDelegate = Nothing


        Private WriteOnly Property IMSEModelWrapper_onModelTimeStep As IMSEModelWrapper.onModelTimeStepDelegate Implements IMSEModelWrapper.onModelTimeStep
            Set(value As IMSEModelWrapper.onModelTimeStepDelegate)
                m_OnTimeStepDelegate = value
                Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onEcosimTimestep
            End Set
        End Property

        Public Sub Init(Ecosim As cEcosimModel) Implements IMSEModelWrapper.Init
            Me.m_Ecosim = Ecosim

            Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onEcosimTimestep
        End Sub

        Public Sub Init(Ecospace As cEcoSpace) Implements IMSEModelWrapper.Init
            Debug.Assert(False, "Invalid Model Type")
        End Sub

        Public Function InitForRun() As Boolean Implements IMSEModelWrapper.InitForRun
            Me.m_Ecosim.Init(False)
        End Function

        Public Function SetBaseFFromGear() As Boolean Implements IMSEModelWrapper.SetBaseFFromGear
            Me.m_Ecosim.SetBaseFFromGear()
        End Function



        Private Sub onEcosimTimestep(iTime As Long, data As cEcoSimResults)
            Try
                m_OnTimeStepDelegate(CInt(iTime))
            Catch ex As ArgumentException

            End Try
        End Sub
    End Class


    Public Class cMSEEcoSpaceSimWrapper
        Implements IMSEModelWrapper

        Public WriteOnly Property onModelTimeStep As IMSEModelWrapper.onModelTimeStepDelegate Implements IMSEModelWrapper.onModelTimeStep
            Set(value As IMSEModelWrapper.onModelTimeStepDelegate)
                Throw New NotImplementedException()
            End Set
        End Property

        'Public Property onModelTimeStep As onModelTimeStepDelegate Implements IMSEModelWrapper.onModelTimeStep
        '    Get
        '        Throw New NotImplementedException()
        '    End Get
        '    Set(value As onModelTimeStepDelegate)
        '        Throw New NotImplementedException()
        '    End Set
        'End Property

        Public Sub Init(Ecosim As cEcosimModel) Implements IMSEModelWrapper.Init
            Throw New NotImplementedException()
        End Sub

        Public Sub Init(Ecospace As cEcoSpace) Implements IMSEModelWrapper.Init
            Throw New NotImplementedException()
        End Sub

        Public Function InitForRun() As Boolean Implements IMSEModelWrapper.InitForRun
            Throw New NotImplementedException()
        End Function

        Public Function SetBaseFFromGear() As Boolean Implements IMSEModelWrapper.SetBaseFFromGear
            Throw New NotImplementedException()
        End Function
    End Class

End Namespace
