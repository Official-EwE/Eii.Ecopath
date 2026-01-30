' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Text
Imports EwECore
Imports EwECore.Common
Imports EwEUtils.Utilities



''' <summary>
''' CSV writer for Value Chain results.
''' </summary>
Public Class cResultWriter

#Region " Variables "

    Private m_data As cData = Nothing
    Private m_results As cResults = Nothing
    Private m_msg As cMessage = Nothing

#End Region ' Variables

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Shazaam!
    ''' </summary>
    ''' <param name="data"><see cref="cData">Value chain data</see> to plunder.</param>
    ''' <param name="results"><see cref="cResults">Value chain results</see> to write.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(data As cData, results As cResults)
        Me.m_data = data
        Me.m_results = results
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write results to CSV file.
    ''' </summary>
    ''' <param name="agg">Data aggregation method in use during the run.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function WriteResults(agg As cParameters.eAggregationModeType) As Boolean
        Return Me.WriteResults(agg, 0, "")
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="agg"></param>
    ''' <param name="iItem"></param>
    ''' <param name="strItem"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function WriteResults(agg As cParameters.eAggregationModeType, iItem As Integer, strItem As String) As Boolean

        Dim vs As cVariableStatus = Nothing
        Dim iTimeStart As Integer = If(Me.m_results.RunType = cModel.eRunTypes.Ecopath, 0, 1)
        Dim iTimeEnd As Integer = If(Me.m_results.RunType = cModel.eRunTypes.Ecopath, 0, Me.m_results.NumTimeSteps)

        Dim pout As String = ""
        Select Case Me.m_results.RunType
            Case cModel.eRunTypes.Ecopath
                pout = Path.Combine(Me.m_data.Core.DefaultOutputPath(eAutosaveTypes.Ecopath), "ValueChain")
            Case cModel.eRunTypes.Ecosim
                pout = Path.Combine(Me.m_data.Core.DefaultOutputPath(eAutosaveTypes.Ecosim), "ValueChain")
            Case cModel.eRunTypes.Equilibrium
                Return False
        End Select
        If Not cFileUtils.IsDirectoryAvailable(pout, True) Then Return False

        Dim vars As New List(Of cVariableStatus)

        Try
            For iStep As Integer = iTimeStart To iTimeEnd
                Dim strFile As String = Me.GetFileName(agg, strItem, iStep)

                If String.IsNullOrWhiteSpace(strFile) Then Return False
                Using sw As New StreamWriter(Path.Combine(pout, strFile))

                    ' Start write process

                    ' Write EwE header
                    If Me.m_data.Core.SaveWithFileHeader Then
                        sw.WriteLine(Me.GetModelDetails())
                        sw.WriteLine()
                    End If

                    ' Write data header
                    sw.Write("Variable")
                    For Each u As cUnit In Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
                        sw.Write(",")
                        sw.Write(cStringUtils.ToCSVField(u.Name))
                    Next
                    sw.WriteLine("")

                    ' Write data
                    For Each v As cResults.eVariableType In [Enum].GetValues(GetType(cResults.eVariableType))
                        sw.Write(cStringUtils.ToCSVField(v.ToString))
                        For Each u As cUnit In Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
                            sw.Write(",")
                            Dim result As Single = 0
                            If (Me.m_results.RunType = cModel.eRunTypes.Ecopath) Then
                                result = Me.m_results.GetTotal(v, New cUnit() {u}, iItem, cResults.GetVariableContributionType(v))
                            Else
                                result = Me.m_results.GetTimeStepTotal(v, iStep, New cUnit() {u}, iItem, cResults.GetVariableContributionType(v))
                            End If
                            sw.Write(cStringUtils.FormatNumber(result))
                        Next
                        sw.WriteLine("")
                    Next
                    sw.Flush()
                    sw.Close()

                    vars.Add(New cVariableStatus(eStatusFlags.OK, cStringUtils.Localize(My.Resources.PROMPT_SAVERESULT_DETAIL, strFile),
                                                 eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))
                End Using
            Next
        Catch ex As Exception
            ' Waah!
            Me.m_msg = New cMessage(cStringUtils.Localize(My.Resources.PROMPT_SAVERESULTS_FAILED, pout, ex.Message),
                                    eMessageType.DataExport, eCoreComponentType.Ecotracer, eMessageImportance.Warning)
            Return False
        End Try

        ' Already has save result message?
        If (Me.m_msg Is Nothing) Then
            ' #No: create one
            Me.m_msg = New cMessage(cStringUtils.Localize(My.Resources.PROMPT_SAVERESULTS_SUCCESS, pout),
                                    eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            ' Set hyperlink
            Me.m_msg.Hyperlink = pout
        End If

        For i As Integer = 0 To vars.Count - 1
            Me.m_msg.AddVariable(vars(i))
        Next

        ' We're done, Jim
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="agg"></param>
    ''' <param name="strItem"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function GetFileName(agg As cParameters.eAggregationModeType, strItem As String, Optional iTimeStep As Integer = 0) As String

        Dim strFile As String = ""
        strFile = cStringUtils.Localize("valuechain_{0}.csv", agg.ToString())

        If Not String.IsNullOrWhiteSpace(strItem) Then
            strFile = cStringUtils.Localize("{0}_{1}", strFile, strItem)
        End If

        If (iTimeStep > 0) Then
            strFile = cStringUtils.Localize("{0}_{1:0000}", strFile, iTimeStep)
        End If

        Return cFileUtils.ToValidFileName(strFile, False) & ".csv"

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the save results message.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property Message As cMessage
        Get
            Return Me.m_msg
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get default model details to report in output file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetModelDetails() As String

        Dim sb As New StringBuilder()
        Dim core As cCore = Me.m_data.Core
        Dim dtFields As New Dictionary(Of String, String)
        dtFields("RunType") = (Me.m_results.RunType.ToString())

        ' Append header
        If (Me.m_results.RunType = cModel.eRunTypes.Ecopath) Then
            sb.AppendLine(core.DefaultFileHeader(eAutosaveTypes.Ecopath, extraFields:=dtFields))
        Else
            sb.AppendLine(core.DefaultFileHeader(eAutosaveTypes.Ecosim, extraFields:=dtFields))
        End If
        ' Append value chain run type

        Return sb.ToString()

    End Function

End Class
