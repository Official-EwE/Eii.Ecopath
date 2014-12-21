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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Core

    ''' <summary>
    ''' Interface for writing Ecospace time step results to file
    ''' </summary>
    Public Interface IEcospaceResultsWriter

        ''' <summary>
        ''' Save time step data to file.
        ''' </summary>
        ''' <param name="SpaceTimeStepResults">cEcospaceTimestep as object containing the data to save.</param>
        Sub WriteResults(ByVal SpaceTimeStepResults As Object)

        ''' <summary>
        ''' Init to the current cCore.
        ''' </summary>
        ''' <param name="theCore">The core to initialize with.</param>
        Sub Init(ByVal theCore As Object)

        ''' <summary>
        ''' Called when as Ecospace model run is about to start.
        ''' </summary>
        ''' <remarks>This can be used to initialized and file data at the start of a run.</remarks>
        Sub StartWrite()

        ''' <summary>
        ''' Called at the end of an Ecospace model run.
        ''' </summary>
        ''' <remarks>Cleanup after an Ecospace run has completed.</remarks>
        Sub EndWrite()

        ''' <summary>
        ''' Return the file extension that this writer supports.
        ''' </summary>
        Function FileExtension() As String

        ''' <summary>
        ''' Return the output path that this writter is going to use.
        ''' </summary>
        ReadOnly Property OutputPath As String

        ''' <summary>
        ''' Return the full path this writer will use for this Varname, Group, File extention and timestep.
        ''' </summary>
        ''' <param name="varname">eVarNameFlags of the variable.</param>
        ''' <param name="iGrp">Group index.</param>
        ''' <param name="strExt">File extention.</param>
        ''' <param name="iModelTimeStep">Model timestep</param>
        ''' <returns>Full path of the file</returns>
        ''' <remarks></remarks>
        Function GetGroupFileName(ByVal varname As eVarNameFlags, _
                                  ByVal iGrp As Integer, _
                                  ByVal strExt As String, _
                                  Optional ByVal iModelTimeStep As Integer = -9999) As String

        ''' <summary>
        ''' Return the full path this writer will use for this Varname, Group, File extention and timestep.
        ''' </summary>
        ''' <param name="varname">eVarNameFlags of the variable.</param>
        ''' <param name="iFlt">Fleet index.</param>
        ''' <param name="strExt">File extention.</param>
        ''' <param name="iModelTimeStep">Model timestep</param>
        ''' <returns>Full path of the file</returns>
        ''' <remarks></remarks>
        Function GetFleetFileName(ByVal varname As eVarNameFlags, _
                                  ByVal iFlt As Integer, _
                                  ByVal strExt As String, _
                                  Optional ByVal iModelTimeStep As Integer = -9999) As String

    End Interface

End Namespace
