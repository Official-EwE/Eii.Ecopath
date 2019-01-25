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
    Public Shared Function GetDrivers(core As cCore, game As cGame, Optional datatype As cPressure.eDataTypes = cPressure.eDataTypes.NotSet) As cDriver()
        Dim lPressures As New List(Of cDriver)
        For Each vn As eVarNameFlags In cDriverFactory.SupportedVariables
            lPressures.AddRange(cDriverFactory.GetDrivers(core, game, vn, datatype))
        Next
        Return lPressures.ToArray()
    End Function

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain all drivers for a <see cref="SupportedVariables()">supported variable</see>.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Shared Function GetDrivers(core As cCore, game As cGame, vn As eVarNameFlags, datatype As cPressure.eDataTypes) As cDriver()

        Dim l As New List(Of cDriver)
        Dim d As cDriver = Nothing

        Debug.Assert(Array.IndexOf(SupportedVariables, vn) > -1)

        Select Case vn

            Case eVarNameFlags.LayerDriver
                For i As Integer = 1 To core.nEnvironmentalDriverLayers
                    d = New cEnvironmentalDriver(core, game, core.EcospaceBasemap.LayerDriver(i))
                    l.Add(d)
                Next

            Case eVarNameFlags.LayerMPA
                For i As Integer = 1 To core.nMPAs
                    d = New cMPADriver(core, game, core.EcospaceMPAs(i))
                    l.Add(d)
                Next

            Case eVarNameFlags.LayerHabitat
                For i As Integer = 1 To core.nHabitats - 1
                    d = New cHabitatDriver(core, game, core.EcospaceHabitats(i))
                    l.Add(d)
                Next

            Case eVarNameFlags.EcosimFleetEffort
                For i As Integer = 1 To core.nFleets
                    d = New cEffortDriver(core, game, core.EcopathFleetInputs(i))
                    l.Add(d)
                Next

            Case Else
                Debug.Assert(False, "Internal error: variable not supported")

        End Select

        l.RemoveAll(Function(tmp) tmp.DataType <> datatype And datatype <> cPressure.eDataTypes.NotSet)
        Return l.ToArray()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the list of variables that can be used to drive Ecospace through
    ''' MSP pressures.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Shared Function SupportedVariables() As eVarNameFlags()
        Return New eVarNameFlags() {eVarNameFlags.LayerDriver, eVarNameFlags.LayerMPA, eVarNameFlags.LayerHabitat, eVarNameFlags.EcosimFleetEffort}
    End Function

#End Region ' Internals

End Class
