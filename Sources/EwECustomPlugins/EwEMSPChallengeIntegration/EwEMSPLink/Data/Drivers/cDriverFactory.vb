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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

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

        Dim key As String = ""
        If (pressure IsNot Nothing) Then key = pressure.Name
        Return GetDrivers(core, game, key)

    End Function

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain all drivers for the given fixed named key, or for all drivers if the key is omitted.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Shared Function GetDrivers(core As cCore, game As cGame, key As String) As cDriver()

        Dim l As New List(Of cDriver)
        Dim d As cDriver = Nothing

        If String.IsNullOrWhiteSpace(key) Or key.StartsWith(cGame.NAME_NOISE) Or key.StartsWith(cGame.NAME_SURFACE_DIST) Or key.StartsWith(cGame.NAME_BOTTOM_DIST) Then
            For i As Integer = 1 To core.nEnvironmentalDriverLayers
                d = New cEnvironmentalDriver(core, game, core.EcospaceBasemap.LayerDriver(i))
                l.Add(d)
            Next
        End If

        If String.IsNullOrWhiteSpace(key) Or key.StartsWith(cGame.NAME_PROTECTION) Then
            For i As Integer = 1 To core.nMPAs
                d = New cMPADriver(core, game, core.EcospaceMPAs(i))
                l.Add(d)
            Next
        End If

        If String.IsNullOrWhiteSpace(key) Or key.StartsWith(cGame.NAME_ARTIFICIAL_HAB) Then
            For i As Integer = 1 To core.nHabitats - 1
                d = New cHabitatDriver(core, game, core.EcospaceHabitats(i))
                l.Add(d)
            Next
        End If

        If String.IsNullOrWhiteSpace(key) Or key.StartsWith(cGame.NAME_FISHING_INT) Then
            For i As Integer = 1 To core.nFleets
                d = New cEffortMulitiplierDriver(core, game, core.EcopathFleetInputs(i))
                l.Add(d)
            Next
        End If

        If String.IsNullOrWhiteSpace(key) Or key.StartsWith(cGame.NAME_FISHING_ECO) Then
            For i As Integer = 1 To core.nFleets
                d = New cEcologicalGearDriver(core, game, core.EcopathFleetInputs(i))
                l.Add(d)
            Next
        End If

        Return l.ToArray()

    End Function

#End Region ' Internals

End Class
