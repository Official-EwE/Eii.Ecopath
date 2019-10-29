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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'


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