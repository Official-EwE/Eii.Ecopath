' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Imports EwECore



''' -----------------------------------------------------------------------
''' <summary>
''' Factory to access available Ecospace drivers.
''' </summary>
''' -----------------------------------------------------------------------
Friend Class cDriverFactory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain all drivers available in a given Ecospace scenario.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetDrivers(core As cCore, game As cGame, Optional pressure As cPressure = Nothing) As cDriver()

        Dim l As New List(Of cDriver)
        Dim cat As Integer = 0
        Dim subt As Integer = 0

        If (pressure IsNot Nothing) Then
            Dim key As String = pressure.Name.ToLower()
            If (TypeOf pressure Is cEnvironmentalPressure) Then
                cat = 1
                If key.StartsWith(cGame.NAME_NOISE.ToLower()) Or key.StartsWith(cGame.NAME_SURFACE_DIST.ToLower()) Or key.StartsWith(cGame.NAME_BOTTOM_DIST.ToLower()) Then
                    subt = 1
                ElseIf key.StartsWith(cGame.NAME_PROTECTION.ToLower()) Then
                    subt = 2
                ElseIf key.StartsWith(cGame.NAME_ARTIFICIAL_HAB.ToLower()) Then
                    subt = 3
                End If
            ElseIf (TypeOf pressure Is cFishingEffortPressure) Then
                cat = 2
                subt = 1
            ElseIf (TypeOf pressure Is cFishingEcoPressure) Then
                cat = 2
                subt = 2
            End If
        End If

        If cat = 0 Or cat = 1 Then
            If (subt = 0 Or subt = 1) Then
                For i As Integer = 1 To core.nEnvironmentalDriverLayers
                    l.Add(New cEnvironmentalDriver(core, game, core.EcospaceBasemap.LayerDriver(i)))
                Next
            End If

            If (subt = 0 Or subt = 2) Then
                For i As Integer = 1 To core.nMPAs
                    l.Add(New cMPADriver(core, game, core.EcospaceMPAs(i)))
                Next
            End If

            If (subt = 0 Or subt = 3) Then
                For i As Integer = 1 To core.nHabitats - 1
                    l.Add(New cHabitatDriver(core, game, core.EcospaceHabitats(i)))
                Next
            End If
        End If

        If (cat = 0 Or cat = 2) Then
            If (subt = 0 Or subt = 1) Then
                For i As Integer = 1 To core.nFleets
                    l.Add(New cFleetEffortDriver(core, game, core.EcopathFleetInputs(i)))
                Next
            End If
            If (subt = 0 Or subt = 2) Then
                For i As Integer = 1 To core.nFleets
                    l.Add(New cFleetEcoDriver(core, game, core.EcopathFleetInputs(i)))
                Next
            End If
        End If

        Return l.ToArray()

    End Function

End Class
