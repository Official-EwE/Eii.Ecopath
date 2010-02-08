Imports EwECore

Namespace Ecosim

    Public Class cEstimateVsCalc

        Private m_core As cCore = Nothing

        Public Sub New(ByVal core As cCore)
            Me.m_core = core
        End Sub

        Public Function Fish1(ByVal iGroup As Integer) As Single
            Dim group As cEcoPathGroupOutput = Me.m_core.EcoPathGroupOutputs(iGroup)
            Return group.MortCoFishRate
        End Function

        Public Function SimGE(ByVal iGroup As Integer) As Single
            Dim groupOut As cEcoPathGroupOutput = Me.m_core.EcoPathGroupOutputs(iGroup)
            If groupOut.QBOutput > 0 Then
                Return groupOut.PBOutput / groupOut.QBOutput
            Else
                Return 0.0!
            End If
        End Function

        Public Function SimQB(ByVal iGroup As Integer) As Single
            Return Me.m_core.EcoPathGroupOutputs(iGroup).QBOutput ' * FractionWOimport
        End Function

        Public Function SimDC(ByVal k As Integer, ByVal l As Integer) As Single

            Dim FractionWOimport As Single
            Dim asSimDC(Me.m_core.nGroups, Me.m_core.nGroups) As Single

            'VC210898   To take care of import (a part of the diet composition):
            '           Rescale diet so as not to incorporate import:
            For j As Integer = 1 To Me.m_core.nLivingGroups      'all living groups

                Dim group As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(j)
                If group.DietComp(0) > 0 Then
                    FractionWOimport = (1 - group.DietComp(0) / 1) 'There is import
                Else
                    FractionWOimport = 1
                End If
                For i As Integer = 1 To Me.m_core.nGroups 'prey
                    'Next cause an overflow if Fraction WOImport=0, therefore a trap
                    If FractionWOimport = 0 Then
                        asSimDC(i, j) = 0
                    Else
                        asSimDC(i, j) = group.DietComp(i) '/ FractionWOimport
                    End If
                Next
            Next
            Return asSimDC(k, l)
        End Function

        Public Function mo(ByVal iGroup As Integer) As Single
            Dim grp As cEcoPathGroupOutput = Me.m_core.EcoPathGroupOutputs(iGroup)
            Return (1 - grp.EEOutput) * grp.PBOutput
        End Function

        Public Function StartEatenOf(ByVal iGroup As Integer) As Single

            Dim sStartEatenOf As Single = 0.0!
            Dim i As Integer, j As Integer

            sStartEatenOf = 0
            For j = 1 To Me.m_core.nLivingGroups
                If SimQB(j) > 0 And SimDC(j, i) > 0 Then _
                    sStartEatenOf += Me.m_core.StartBiomass(j) * SimQB(j) * SimDC(j, i)
            Next
            Return sStartEatenOf

        End Function

    End Class

End Namespace ' Ecosim
