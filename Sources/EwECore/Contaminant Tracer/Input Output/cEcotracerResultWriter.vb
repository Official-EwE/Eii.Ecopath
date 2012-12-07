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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Ecostracer result writer.
''' </summary>
Public Class cEcotracerResultWriter

#Region " Private vars "

    Protected m_core As cCore = Nothing

#End Region ' Private vars

#Region " Construction "

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

#End Region ' Construction

#Region " Public interfaces "

    Public Function WriteEcosimResults() As Boolean

        Dim sw As StreamWriter = Nothing
        Dim scenario As cEcoSimScenario = Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex)
        Dim strFile As String = Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecotracer), _
                                             cFileUtils.ToValidFileName("Ecosim_" & scenario.Name & ".csv", False))
        sw = Me.OpenWriter(strFile)
        If (sw Is Nothing) Then Return False

        Me.WriteHeader(sw, False)
        Me.WriteBody(sw)
        Me.CloseWriter(sw, strFile)

        Return True

    End Function

    Public Function WriteEcospaceResults() As Boolean

        Dim sw As StreamWriter = Nothing
        Dim scenario As cEcospaceScenario = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex)
        Dim strFile As String = Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecotracer), _
                                             cFileUtils.ToValidFileName("Ecosim_" & scenario.Name & ".csv", False))
        sw = Me.OpenWriter(strFile)
        If (sw Is Nothing) Then Return False

        Me.WriteHeader(sw, True)
        Me.WriteBody(sw)
        Me.CloseWriter(sw, strFile)

        Return True

    End Function

#End Region ' Public interfaces

#Region " Internals "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Open a CSV file writer.
    ''' </summary>
    ''' <param name="strFile">File name to open the writer for.</param>
    ''' <returns>The writer, or nothing if an error occurred.</returns>
    ''' <remarks>Close the writer with <see cref="CloseWriter"/>.</remarks>
    ''' -------------------------------------------------------------------
    Protected Function OpenWriter(ByVal strFile As String) As StreamWriter

        Dim sw As StreamWriter = Nothing

        ' Abort if directory missing
        If cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) = False Then
            Me.SendErrorMessage(strFile, My.Resources.CoreMessages.OUTPUT_DIRECTORY_MISSING)
            Return Nothing
        End If

        Try
            sw = New StreamWriter(strFile)
        Catch ex As Exception
            Me.SendErrorMessage(strFile, ex.Message)
            Return Nothing
        End Try

        Return sw

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Close a CSV file writer.
    ''' </summary>
    ''' <param name="sw">The writer to close.</param>
    ''' <param name="strPath ">The path to the file of the writer.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Protected Function CloseWriter(sw As StreamWriter, strPath As String) As Boolean

        Dim bSuccess As Boolean = True

        Try
            sw.Flush()
            sw.Close()
            Me.SendSuccessMessage(strPath)
        Catch ex As Exception
            Me.SendErrorMessage(strPath, ex.Message)
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Write CSV header information.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Protected Sub WriteHeader(ByVal sw As StreamWriter, ByVal bSpace As Boolean)

        ' File
        sw.WriteLine("EwE version, " & cCore.Version)
        sw.WriteLine("Date, " & Date.Now.ToString())
        sw.WriteLine("ModelFile, " & Me.m_core.DataSource.ToString)
        sw.WriteLine("ModelName, " & Me.m_core.EwEModel.Name)
        sw.WriteLine("EcosimScenario, " & Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex).Name)
        If bSpace Then
            Debug.Assert(Me.m_core.ActiveEcospaceScenarioIndex > 0)
            sw.WriteLine("EcospaceScenario, " & Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex).Name)
        End If

        ' Append time series name to scenario, if any
        sw.Write("TimeSeries, ")
        If Me.m_core.ActiveTimeSeriesDatasetIndex > 0 Then
            sw.WriteLine(cStringUtils.ToCSVField(Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex).Name))
        Else
            sw.WriteLine("(none)")
        End If

    End Sub

    Protected Sub WriteBody(ByVal sw As StreamWriter)

        ' To write

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Message to use to report an error.
    ''' </summary>
    ''' <param name="strPath">Output file name.</param>
    ''' <param name="strReason">Reason of failure, most likely the text obtained from an exception.</param>
    ''' -------------------------------------------------------------------
    Protected Sub SendErrorMessage(ByVal strPath As String, ByVal strReason As String)
        Dim msg As cMessage = New cMessage(String.Format(My.Resources.CoreMessages.TRACER_RESULTS_SAVE_FAILED, strPath, strReason), _
                                           eMessageType.DataExport, eCoreComponentType.Ecotracer, eMessageImportance.Warning)
        Me.m_core.Messages.SendMessage(msg)
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Message to use to report a succes.
    ''' </summary>
    ''' <param name="strPath">Output file name.</param>
    ''' -------------------------------------------------------------------
    Protected Function SendSuccessMessage(ByVal strPath As String) As cMessage
        Dim msg As cMessage = New cMessage(String.Format(My.Resources.CoreMessages.TRACER_RESULTS_SAVE_SUCCESS, strPath), _
                                           eMessageType.DataExport, eCoreComponentType.Ecotracer, eMessageImportance.Information)
        msg.Hyperlink = Path.GetDirectoryName(strPath)
        Me.m_core.Messages.SendMessage(msg)
        Return Nothing
    End Function

#End Region ' Internals

End Class
