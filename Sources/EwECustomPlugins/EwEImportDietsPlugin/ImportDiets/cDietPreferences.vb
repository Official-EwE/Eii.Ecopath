' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cDietPreferences

    Public DietPref(,) As Single
    Public Biomass() As Single
    Public nGroups As Integer

    Public Sub New(EcopathData As EwECore.cEcopathDataStructures)
        Me.nGroups = EcopathData.NumGroups

        Me.DietPref = New Single(Me.nGroups + 1, Me.nGroups + 1) {}
        Me.Biomass = New Single(Me.nGroups) {}

        Debug.Assert(Me.DietPref.Length = EcopathData.DC.Length, Me.ToString + "New()  Oppss Diet Matrix messed up. Really this be impossible!")

        Array.Copy(EcopathData.DC, Me.DietPref, EcopathData.DC.Length)
        Array.Copy(EcopathData.B, Me.Biomass, EcopathData.B.Length)

    End Sub

    Public Sub New(NumGroups As Integer)
        Me.nGroups = NumGroups

        Me.DietPref = New Single(Me.nGroups, Me.nGroups) {}
        Me.Biomass = New Single(Me.nGroups) {}

    End Sub

End Class