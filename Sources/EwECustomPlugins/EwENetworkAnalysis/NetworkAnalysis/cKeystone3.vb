' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore



Friend Class cKeystone3

    Private Class cGroup
        Public Sub New(i As Integer, b As Single)
            Me.Index = i
            Me.Biomass = b
        End Sub
        Public Index As Integer
        Public Biomass As Single
        Public Epsilon As Single
        Public BC As Integer
        Public K3 As Single

        Public Overrides Function ToString() As String
            Return "cGroup " & Me.Index & ", " & Me.Biomass
        End Function

    End Class

    Private Class cGroupComparer
        Implements IComparer(Of cGroup)

        Public Function Compare(x As cGroup, y As cGroup) As Integer Implements System.Collections.Generic.IComparer(Of cGroup).Compare
            If (x.Biomass < y.Biomass) Then Return -1
            If (x.Biomass > y.Biomass) Then Return 1
            If (x.Index < y.Index) Then Return -1
            Return 1
        End Function

    End Class

    Public Shared Sub Calculate(data As cEcopathDataStructures, network As cEcoNetwork)

        Dim lGroups As New List(Of cGroup)
        Dim g As cGroup = Nothing
        Dim dSum As Double = 0

        For i As Integer = 1 To data.NumLiving
            g = New cGroup(i, data.B(i))

            dSum = 0
            For j As Integer = 1 To data.NumLiving
                If (i <> j) Then
                    dSum += (network.MTI(i, j) * network.MTI(i, j))
                End If
            Next
            g.Epsilon = CSng(Math.Sqrt(dSum))

            lGroups.Add(g)
        Next

        lGroups.Sort(New cGroupComparer())

        For i As Integer = 0 To data.NumLiving - 1
            g = lGroups(i)
            g.BC = data.NumLiving - i
            g.K3 = g.Epsilon * g.BC
            network.KeystoneIndex3(g.Index) = Math.Log10(g.K3)
        Next

    End Sub

End Class
