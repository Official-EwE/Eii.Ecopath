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

        m_ResultsArray = Results_Array

        m_MSE = MSE
        m_Core = MSE.Core

        If m_ResultsArray.TotalAcrossFleets = True Then
            Start_index_for_iFleet = 0
        Else
            Start_index_for_iFleet = 1
        End If

        If m_ResultsArray.TotalAcrossGroups = True Then
            Start_index_for_iGrp = 0
        Else
            Start_index_for_iGrp = 1
        End If


        ReDim m_StreamWriters(m_ResultsArray.nFleets, m_ResultsArray.nGroups)

        For iGrp As Integer = Start_index_for_iGrp To m_Core.nGroups

            For iFleet As Integer = Start_index_for_iFleet To m_Core.nFleets
                If iGrp = 0 Then
                    GroupName = "AllGroups"
                Else
                    GroupName = m_Core.EcoPathGroupInputs(iGrp).Name & "_GroupNo" & iGrp
                End If
                If iFleet = 0 Then
                    FleetName = "_AllFleets"
                Else
                    FleetName = "_FleetNo" & iFleet
                End If
                strFile = cFileUtils.ToValidFileName(m_ResultsArray.FileNamePrefix & GroupName & FleetName & ".csv", False)

                writer = cMSEUtils.GetWriter(cMSEUtils.MSEFile(MSE.DataPath, FolderPath, strFile))
                msgReport.AddVariable(New cVariableStatus(eStatusFlags.OK, String.Format(My.Resources.STATUS_SAVED_DETAIL, strFile), eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))

                Debug.Assert(writer IsNot Nothing)

                m_StreamWriters(iFleet, iGrp) = writer

                'Setup the HCR F Targ file for igrp
                If Me.m_Core.SaveWithFileHeader Then m_StreamWriters(iFleet, iGrp).WriteLine(Me.m_Core.DefaultFileHeader(eAutosaveTypes.Ecosim))
                m_StreamWriters(iFleet, iGrp).Write("GroupName, FleetName, ModelID, StrategyName, ResultType")
                For iTime As Integer = 1 To m_ResultsArray.NumberOfTimeRecords
                    m_StreamWriters(iFleet, iGrp).Write("," & cStringUtils.FormatNumber(iTime))
                Next
                m_StreamWriters(iFleet, iGrp).WriteLine()

            Next iFleet

        Next

    End Sub

    Public Overrides Sub ReleaseWriters()
        For iGrp As Integer = Start_index_for_iGrp To m_Core.nGroups
            For iFleet As Integer = Start_index_for_iFleet To m_Core.nFleets
                cMSEUtils.ReleaseWriter(m_StreamWriters(iFleet, iGrp))
            Next
        Next
    End Sub

    Public Overrides Sub WriteResults()

        Dim GroupName As String
        Dim FleetName As String

        For iGrp As Integer = Start_index_for_iGrp To m_Core.nGroups
            For iFleet As Integer = Start_index_for_iFleet To m_Core.nFleets

                If iGrp = 0 Then
                    GroupName = "AllGroups"
                Else
                    GroupName = m_Core.EcoPathGroupInputs(iGrp).Name
                End If
                If iFleet = 0 Then
                    FleetName = "AllFleets"
                Else
                    FleetName = m_Core.FleetInputs(iFleet).Name
                End If

                For iStrategy = 1 To m_ResultsArray.nStrategies
                    'Output the Landings to file
                    m_StreamWriters(iFleet, iGrp).Write("{0},{1},{2},{3},{4}", _
                                                                  cStringUtils.ToCSVField(GroupName), _
                                                                  cStringUtils.ToCSVField(FleetName), _
                                                                  cStringUtils.FormatNumber(m_ResultsArray.ModelID), _
                                                                  cStringUtils.ToCSVField(StrategyName(iStrategy)), _
                                                                  cStringUtils.ToCSVField(m_ResultsArray.DataName))
                    For iTime = 1 To m_ResultsArray.NumberOfTimeRecords
                        m_StreamWriters(iFleet, iGrp).Write("," & cStringUtils.FormatNumber(m_ResultsArray.GetValue(iStrategy, iGrp, iFleet, iTime)))
                    Next
                    m_StreamWriters(iFleet, iGrp).WriteLine()
                Next iStrategy
            Next iFleet
        Next iGrp

    End Sub
End Class
