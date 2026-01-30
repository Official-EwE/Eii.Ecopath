' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Strict Off
Imports System.IO
Imports EwEUtils.Utilities
Imports EwECore
Imports EwECore.Common

Public Class cResultsWriter_1DArray
    Inherits cResultsWriter_Base

    Protected m_ResultsArray As cResultsCollector_1DArray
    Protected m_StreamWriters As List(Of StreamWriter)

    Public Overrides Sub Initialise(msgReport As EwECore.cMessage, MSE As cMSE, Results_Array As cResultsCollector_Base, FolderPath As cMSEUtils.eMSEPaths)

        Dim strFile As String
        Dim writer As StreamWriter

        Me.m_ResultsArray = Results_Array

        Me.m_MSE = MSE
        Me.m_Core = MSE.Core
        Me.m_StreamWriters = New List(Of StreamWriter)

        For iElement = 1 To Me.m_ResultsArray.nElements
            strFile = cFileUtils.ToValidFileName(Me.m_ResultsArray.FileNamePrefix & Me.m_ResultsArray.ElementName(iElement) & "_" & Me.m_ResultsArray.Dim_Name & "No" & iElement & ".csv", False)

            writer = cMSEUtils.GetWriter(cMSEUtils.MSEFile(MSE.DataPath, FolderPath, strFile))
            msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, strFile), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

            Debug.Assert(writer IsNot Nothing)

            'Setup the HCR F Targ file for igrp
            Me.m_StreamWriters.Add(writer)
            If Me.m_Core.SaveWithFileHeader Then Me.m_StreamWriters(iElement - 1).WriteLine(Me.m_Core.DefaultFileHeader(eAutosaveTypes.Ecosim))
            Me.m_StreamWriters(iElement - 1).Write(Me.m_ResultsArray.Dim_Name & "Name,ModelID,StrategyName,ResultType")
            For iTime As Integer = 1 To Me.m_ResultsArray.NumberOfTimeRecords
                Me.m_StreamWriters(iElement - 1).Write("," & cStringUtils.FormatNumber(iTime))
            Next
            Me.m_StreamWriters(iElement - 1).WriteLine()

        Next

    End Sub

    Public Overrides Sub ReleaseWriters()
        For Each iStreamWriter In Me.m_StreamWriters
            cMSEUtils.ReleaseWriter(iStreamWriter)
        Next
        Me.m_StreamWriters.Clear()
    End Sub

    Public Overrides Sub WriteResults()

        For iElement = 1 To Me.m_ResultsArray.nElements
            For iStrategy = 1 To Me.m_ResultsArray.nStrategies
                If Me.m_MSE.Strategies(iStrategy - 1).RunThisStrategy = False Then Continue For
                Me.m_StreamWriters(iElement - 1).Write("{0},{1},{2},{3}",
                       cStringUtils.ToCSVField(Me.m_ResultsArray.ElementName(iElement)),
                       cStringUtils.FormatNumber(Me.m_ResultsArray.ModelID),
                       cStringUtils.ToCSVField(Me.StrategyName(iStrategy)),
                       cStringUtils.ToCSVField(Me.m_ResultsArray.DataName))
                For iTime = 1 To Me.m_ResultsArray.NumberOfTimeRecords
                    Me.m_StreamWriters(iElement - 1).Write("," & Me.m_ResultsArray.GetValue_Formatted4CSV(iStrategy, iElement, iTime))
                Next
                Me.m_StreamWriters(iElement - 1).WriteLine()
            Next
        Next

    End Sub
End Class
