' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports EwECore.Style
Imports EwECore.ValueWrapper
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging



''' ---------------------------------------------------------------------------
''' <summary>
''' Writer to save Ecopath estimates to a CSV file.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcopathResultWriter

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_data As cEcopathDataStructures = Nothing
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcopathResultWriter)()

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="core">The core instance to write result for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore)
        Me.m_core = core
        Me.m_data = core.m_EcopathData
    End Sub

#End Region ' Constructor

#Region " Public access "

    Public Function WriteResults(Optional bQuiet As Boolean = False) As Boolean

        If (Not Me.m_core.StateMonitor.HasEcopathRan) Then Return False

        Dim msg As cMessage = Nothing
        Dim bSucces As Boolean = True

        Dim strPath As String = Me.m_core.DefaultOutputPath(eAutosaveTypes.EcopathResults)

        ' Try to make sure that the output path is there
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
            msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSIM_SAVE_FAILED, strPath, My.Resources.CoreMessages.OUTPUT_DIRECTORY_MISSING),
                               eMessageType.DataExport, eCoreComponentType.Ecosim, eMessageImportance.Information)
            If (Not bQuiet) Then
                Me.m_core.Messages.SendMessage(msg)
            Else
                m_logger.LogInformation(msg.Message)
            End If
            Return False
        End If

        If Me.WriteBasicEsimates(strPath) And Me.WriteEcopathStats(strPath) Then
            msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_RESULTS_SAVED_SUCCESS, strPath),
                               eMessageType.DataExport, eCoreComponentType.Ecopath, eMessageImportance.Information)
            ' Provide hyperlink to the directory with the files
            msg.Hyperlink = strPath
            If (Not bQuiet) Then
                Me.m_core.Messages.SendMessage(msg)
            Else
                m_logger.LogInformation(msg.Message)
            End If
        Else
            msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOPATH_RESULTS_SAVED_FAILED, strPath),
                               eMessageType.DataExport, eCoreComponentType.Ecopath, eMessageImportance.Warning)
            If (Not bQuiet) Then
                Me.m_core.Messages.SendMessage(msg)
            Else
                m_logger.LogInformation(msg.Message)
            End If
        End If
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write Ecopath estimates to a CSV file.
    ''' </summary>
    ''' <param name="strPath">The target path.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function WriteBasicEsimates(strPath As String) As Boolean

        Dim sw As StreamWriter = Nothing
        Dim bSuccess As Boolean = True

        Try
            sw = New StreamWriter(Path.Combine(strPath, "basic_estimates.csv"))
        Catch ex As Exception
            bSuccess = False
        End Try

        If (sw IsNot Nothing) Then

            If Me.m_core.SaveWithFileHeader Then
                sw.Write(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecopath))
                sw.WriteLine()
            End If

            sw.WriteLine("GroupNo,Group,B,PB,QB,EE,GE,FishMort,PredMort,BioAccum,NetMig,OtherMort,NatMort")
            For i As Integer = 1 To Me.m_data.NumGroups

                Dim grp As cEcopathGroupOutput = Me.m_core.EcopathGroupOutputs(i)

                sw.Write(i)
                sw.Write(",")
                sw.Write(cStringUtils.ToCSVField(Me.m_data.GroupName(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.B(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.PB(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.QB(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.EE(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(Me.m_data.GE(i)))
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.MortCoFishRate)) ' FishMort
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.MortCoPredMort)) ' PredMort
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.BioAccumRatePerYear)) ' BioAccum
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.MortCoNetMig)) ' NetMig
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.MortCoOtherMort)) ' OtherMort
                sw.Write(",")
                sw.Write(cStringUtils.FormatSingle(grp.NatMortPerTotMort)) ' NatMort
                sw.WriteLine()
            Next
            sw.Flush()
            sw.Close()
        Else
            bSuccess = False
            m_logger.LogError(Me.ToString + ".WriteCSV() failed to open file.")
        End If
        Return bSuccess

    End Function

    Private Function WriteEcopathStats(strPath As String) As Boolean

        Dim sw As StreamWriter = Nothing
        Dim bSuccess As Boolean = True

        Try
            sw = New StreamWriter(Path.Combine(strPath, "ecopath_stats.csv"))
        Catch ex As Exception
            bSuccess = False
        End Try

        If (sw IsNot Nothing) Then

            If Me.m_core.SaveWithFileHeader Then
                sw.Write(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecopath))
                sw.WriteLine()
            End If

            Dim data As cEcopathStats = Me.m_core.EcopathStats
            Dim fmt As New cVarnameTypeFormatter()

            ' Todo: add units

            sw.WriteLine("parameter,value")

            For Each var As eVarNameFlags In data.m_values.Keys
                Dim val As cValue = data.m_values(var)
                If var <> eVarNameFlags.Index And var <> eVarNameFlags.Name And var <> eVarNameFlags.DBID Then
                    sw.Write(cStringUtils.ToCSVField(fmt.ToString(var)))
                    sw.Write(",")
                    sw.Write(cStringUtils.ToCSVField(val.Value))
                    sw.WriteLine()
                End If
            Next
            sw.Flush()
            sw.Close()
        Else
            bSuccess = False
            m_logger.LogError(Me.ToString + ".WriteCSV() failed to open file.")
        End If
        Return bSuccess

    End Function

#End Region ' Public access

End Class
