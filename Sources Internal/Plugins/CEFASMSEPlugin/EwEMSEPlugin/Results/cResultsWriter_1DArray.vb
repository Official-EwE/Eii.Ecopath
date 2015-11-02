Imports System.IO
Imports EwEUtils.Utilities
Imports EwECore
Imports EwEUtils.Core

Public Class cResultsWriter_1DArray
    Inherits cResultsWriter_Base

    Protected m_ResultsArray As cResultsCollector_1DArray
    Protected m_StreamWriters As List(Of StreamWriter)


    Public Overrides Sub Initialise(msgReport As EwECore.cMessage, MSE As cMSE, Results_Array As cResultsCollector_Base, FolderPath As cMSEUtils.eMSEPaths)

        Dim strFile As String
        Dim writer As StreamWriter

        m_ResultsArray = Results_Array

        m_MSE = MSE
        m_Core = MSE.Core
        m_StreamWriters = New List(Of StreamWriter)

        For iElement = 1 To m_ResultsArray.nElements
            strFile = cFileUtils.ToValidFileName(m_ResultsArray.FileNamePrefix & m_ResultsArray.ElementName(iElement) & "_" & m_ResultsArray.Dim_Name & "No" & iElement & ".csv", False)

            writer = cMSEUtils.GetWriter(cMSEUtils.MSEFile(MSE.DataPath, FolderPath, strFile))
            msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, strFile), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

            Debug.Assert(writer IsNot Nothing)

            'Setup the HCR F Targ file for igrp
            m_StreamWriters.Add(writer)
            If Me.m_Core.SaveWithFileHeader Then m_StreamWriters(iElement - 1).WriteLine(Me.m_Core.DefaultFileHeader(eAutosaveTypes.Ecosim))
            m_StreamWriters(iElement - 1).Write(m_ResultsArray.Dim_Name & "Name, ModelID, StrategyName, ResultType")
            For iTime As Integer = 1 To m_ResultsArray.NumberOfTimeRecords
                m_StreamWriters(iElement - 1).Write("," & cStringUtils.FormatNumber(iTime))
            Next
            m_StreamWriters(iElement - 1).WriteLine()

        Next

    End Sub

    Public Overrides Sub ReleaseWriters()
        For Each iStreamWriter In m_StreamWriters
            cMSEUtils.ReleaseWriter(iStreamWriter)
        Next
        m_StreamWriters.Clear()
    End Sub

    Public Overrides Sub WriteResults()

        For iElement = 1 To m_ResultsArray.nElements
            For iStrategy = 1 To m_ResultsArray.nStrategies
                m_StreamWriters(iElement - 1).Write("{0},{1},{2},{3}", _
                       cStringUtils.ToCSVField(m_ResultsArray.ElementName(iElement)), _
                       cStringUtils.FormatNumber(m_ResultsArray.ModelID), _
                       cStringUtils.ToCSVField(StrategyName(iStrategy)), _
                       cStringUtils.ToCSVField(m_ResultsArray.DataName))
                For iTime = 1 To m_ResultsArray.NumberOfTimeRecords
                    m_StreamWriters(iElement - 1).Write("," & m_ResultsArray.GetValue_Formatted4CSV(iStrategy, iElement, iTime))
                Next
                m_StreamWriters(iElement - 1).WriteLine()
            Next
        Next

    End Sub
End Class
