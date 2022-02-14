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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright:
'    1991- Ecopath International Initiative, Barcelona, Spain
'
' Cefas MSE plug-in copyright: 
'    2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

Option Strict Off

Imports System.IO
Imports EwEUtils.Utilities
Imports EwECore
Imports EwEUtils.Core

Public Class cResultsWriter_2DArray
    Inherits cResultsWriter_Base

    Protected m_ResultsArray As cResultsCollector_2DArray
    Protected m_StreamWriters(,) As StreamWriter

    Private Start_index_for_iGrp As Integer
    Private Start_index_for_iFleet As Integer

    Public Overrides Sub Initialise(msgReport As EwECore.cMessage, MSE As cMSE, Results_Array As cResultsCollector_Base, FolderPath As cMSEUtils.eMSEPaths)

        Dim strFile As String
        Dim writer As StreamWriter
        Dim GroupName As String
        Dim FleetName As String

        Me.m_ResultsArray = Results_Array

        Me.m_MSE = MSE
        Me.m_Core = MSE.Core

        If Me.m_ResultsArray.TotalAcrossFleets = True Then
            Me.Start_index_for_iFleet = 0
        Else
            Me.Start_index_for_iFleet = 1
        End If

        If Me.m_ResultsArray.TotalAcrossGroups = True Then
            Me.Start_index_for_iGrp = 0
        Else
            Me.Start_index_for_iGrp = 1
        End If


        ReDim Me.m_StreamWriters(Me.m_ResultsArray.nFleets, Me.m_ResultsArray.nGroups)

        For iGrp As Integer = Me.Start_index_for_iGrp To Me.m_Core.nGroups

            For iFleet As Integer = Me.Start_index_for_iFleet To Me.m_Core.nFleets
                If iGrp = 0 Then
                    GroupName = "AllGroups"
                Else
                    GroupName = Me.m_Core.EcopathGroupInputs(iGrp).Name & "_GroupNo" & iGrp
                End If
                If iFleet = 0 Then
                    FleetName = "_AllFleets"
                Else
                    FleetName = "_FleetNo" & iFleet
                End If
                strFile = cFileUtils.ToValidFileName(Me.m_ResultsArray.FileNamePrefix & GroupName & FleetName & ".csv", False)

                writer = cMSEUtils.GetWriter(cMSEUtils.MSEFile(MSE.DataPath, FolderPath, strFile))
                msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, strFile), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

                Debug.Assert(writer IsNot Nothing)

                Me.m_StreamWriters(iFleet, iGrp) = writer

                'Setup the HCR F Targ file for igrp
                If Me.m_Core.SaveWithFileHeader Then Me.m_StreamWriters(iFleet, iGrp).WriteLine(Me.m_Core.DefaultFileHeader(eAutosaveTypes.Ecosim))
                Me.m_StreamWriters(iFleet, iGrp).Write("GroupName,FleetName,ModelID,StrategyName,ResultType")
                For iTime As Integer = 1 To Me.m_ResultsArray.NumberOfTimeRecords
                    Me.m_StreamWriters(iFleet, iGrp).Write("," & cStringUtils.FormatNumber(iTime))
                Next
                Me.m_StreamWriters(iFleet, iGrp).WriteLine()

            Next iFleet

        Next

    End Sub

    Public Overrides Sub ReleaseWriters()
        For iGrp As Integer = Me.Start_index_for_iGrp To Me.m_Core.nGroups
            For iFleet As Integer = Me.Start_index_for_iFleet To Me.m_Core.nFleets
                cMSEUtils.ReleaseWriter(Me.m_StreamWriters(iFleet, iGrp))
            Next
        Next
    End Sub

    Public Overrides Sub WriteResults()

        Dim GroupName As String
        Dim FleetName As String

        For iGrp As Integer = Me.Start_index_for_iGrp To Me.m_Core.nGroups
            For iFleet As Integer = Me.Start_index_for_iFleet To Me.m_Core.nFleets

                If iGrp = 0 Then
                    GroupName = "AllGroups"
                Else
                    GroupName = Me.m_Core.EcopathGroupInputs(iGrp).Name
                End If
                If iFleet = 0 Then
                    FleetName = "AllFleets"
                Else
                    FleetName = Me.m_Core.EcopathFleetInputs(iFleet).Name
                End If

                For iStrategy = 1 To Me.m_ResultsArray.nStrategies
                    'Output the Landings to file
                    If Me.m_MSE.Strategies(iStrategy - 1).RunThisStrategy = False Then Continue For
                    Me.m_StreamWriters(iFleet, iGrp).Write("{0},{1},{2},{3},{4}", _
                                                                  cStringUtils.ToCSVField(GroupName), _
                                                                  cStringUtils.ToCSVField(FleetName), _
                                                                  cStringUtils.FormatNumber(Me.m_ResultsArray.ModelID), _
                                                                  cStringUtils.ToCSVField(Me.StrategyName(iStrategy)), _
                                                                  cStringUtils.ToCSVField(Me.m_ResultsArray.DataName))
                    For iTime = 1 To Me.m_ResultsArray.NumberOfTimeRecords
                        Me.m_StreamWriters(iFleet, iGrp).Write("," & cStringUtils.FormatNumber(Me.m_ResultsArray.GetValue(iStrategy, iGrp, iFleet, iTime)))
                    Next
                    Me.m_StreamWriters(iFleet, iGrp).WriteLine()
                Next iStrategy
            Next iFleet
        Next iGrp

    End Sub
End Class
