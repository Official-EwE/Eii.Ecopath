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

Public Class cResultsWriter_2DArray_Group_Group
    Inherits cResultsWriter_Base

    Protected m_ResultsArray As cResultsCollector_2DArray_Group_Group
    Protected m_StreamWriters(,) As StreamWriter

    Private Start_index_for_iPred As Integer
    Private Start_index_for_iPrey As Integer

    Public Overrides Sub Initialise(msgReport As EwECore.cMessage, MSE As cMSE, Results_Array As cResultsCollector_Base, FolderPath As cMSEUtils.eMSEPaths)

        Dim strFile As String
        Dim writer As StreamWriter
        Dim PredName As String
        Dim PreyName As String

        Me.m_ResultsArray = Results_Array

        Me.m_MSE = MSE
        Me.m_Core = MSE.Core

        If Me.m_ResultsArray.TotalAcrossPred = True Then
            Me.Start_index_for_iPred = 0
        Else
            Me.Start_index_for_iPred = 1
        End If

        If Me.m_ResultsArray.TotalAcrossPrey = True Then
            Me.Start_index_for_iPrey = 0
        Else
            Me.Start_index_for_iPrey = 1
        End If


        ReDim Me.m_StreamWriters(Me.m_ResultsArray.nPrey, Me.m_ResultsArray.nPred)

        For iPred As Integer = Me.Start_index_for_iPred To Me.m_Core.nGroups

            For iPrey As Integer = Me.Start_index_for_iPrey To Me.m_Core.nGroups
                If iPred = 0 Then
                    PredName = "AllPred"
                Else
                    PredName = Me.m_Core.EcoPathGroupInputs(iPred).Name & "PredNo" & iPred
                End If
                If iPrey = 0 Then
                    PreyName = "AllPrey"
                Else
                    PreyName = Me.m_Core.EcoPathGroupInputs(iPrey).Name & "PreyNo" & iPrey
                End If
                strFile = cFileUtils.ToValidFileName(Me.m_ResultsArray.FileNamePrefix & PredName & "__" & PreyName & ".csv", False)

                writer = cMSEUtils.GetWriter(cMSEUtils.MSEFile(MSE.DataPath, FolderPath, strFile))
                msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, strFile), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

                Debug.Assert(writer IsNot Nothing)

                Me.m_StreamWriters(iPrey, iPred) = writer

                'Setup the HCR F Targ file for igrp
                If Me.m_Core.SaveWithFileHeader Then Me.m_StreamWriters(iPrey, iPred).WriteLine(Me.m_Core.DefaultFileHeader(eAutosaveTypes.Ecosim))
                Me.m_StreamWriters(iPrey, iPred).Write("PredName,PreyName,ModelID,StrategyName,ResultType")
                For iTime As Integer = 1 To Me.m_ResultsArray.NumberOfTimeRecords
                    Me.m_StreamWriters(iPrey, iPred).Write("," & cStringUtils.FormatNumber(iTime))
                Next
                Me.m_StreamWriters(iPrey, iPred).WriteLine()

            Next iPrey

        Next

    End Sub

    Public Overrides Sub ReleaseWriters()
        For iPred As Integer = Me.Start_index_for_iPred To Me.m_Core.nGroups
            For iPrey As Integer = Me.Start_index_for_iPrey To Me.m_Core.nFleets
                cMSEUtils.ReleaseWriter(Me.m_StreamWriters(iPrey, iPred))
            Next
        Next
    End Sub

    Public Overrides Sub WriteResults()

        Dim PredName As String
        Dim PreyName As String

        For iPred As Integer = Me.Start_index_for_iPred To Me.m_Core.nGroups
            For iPrey As Integer = Me.Start_index_for_iPrey To Me.m_Core.nGroups

                If iPred = 0 Then
                    PredName = "AllGroups"
                Else
                    PredName = Me.m_Core.EcoPathGroupInputs(iPred).Name
                End If
                If iPrey = 0 Then
                    PreyName = "AllGroups"
                Else
                    PreyName = Me.m_Core.EcoPathGroupInputs(iPrey).Name
                End If

                For iStrategy = 1 To Me.m_ResultsArray.nStrategies
                    'Output the Landings to file
                    If Me.m_MSE.Strategies(iStrategy - 1).RunThisStrategy = False Then Continue For
                    Me.m_StreamWriters(iPrey, iPred).Write("{0},{1},{2},{3},{4}",
                                                                  cStringUtils.ToCSVField(PredName),
                                                                  cStringUtils.ToCSVField(PreyName),
                                                                  cStringUtils.FormatNumber(Me.m_ResultsArray.ModelID),
                                                                  cStringUtils.ToCSVField(Me.StrategyName(iStrategy)),
                                                                  cStringUtils.ToCSVField(Me.m_ResultsArray.DataName))
                    For iTime = 1 To Me.m_ResultsArray.NumberOfTimeRecords
                        Me.m_StreamWriters(iPrey, iPred).Write("," & cStringUtils.FormatNumber(Me.m_ResultsArray.GetValue(iStrategy, iPred, iPrey, iTime)))
                    Next
                    Me.m_StreamWriters(iPrey, iPred).WriteLine()
                Next iStrategy
            Next iPrey
        Next iPred

    End Sub
End Class

