#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to write Ecosim results to a .csv file.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEcosimResultWriter

#Region " Private vars "

        Private m_core As cCore = Nothing

        Public Enum eResultTypes As Integer
            Biomass = 0
            Mortality
            Yield
            ConsumptionBiomass
            FeedingTime
            AvgWeightOrProdCons
            PredationMortality
            Prey
            TL
            Value
        End Enum

#End Region ' Private vars

#Region " Public interfaces "

        Public Sub New(ByVal core As cCore)
            Me.m_core = core
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Save all available Ecosim results to a .csv file.
        ''' </summary>
        ''' <param name="strPath">The path to write to. If not specified, output is
        ''' written to <see cref="cCore.OutputPath">the core output path</see>.</param>
        ''' <param name="results">The results to write, or nothing to write all results.</param>
        ''' <returns>True if saved successfully.</returns>
        ''' -----------------------------------------------------------------------
        Public Function WriteResults(Optional ByVal strPath As String = "", _
                                     Optional ByVal results As eResultTypes() = Nothing) As Boolean

            Dim msg As cMessage = Nothing
            Dim bSucces As Boolean = True

            If Not Me.m_core.StateMonitor.HasEcosimRan Then Return False

            If String.IsNullOrEmpty(strPath) Then
                strPath = Me.m_core.OutputPath
            End If

            ' Try to make sure that the output path is there
            If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
                msg = New cMessage(String.Format(My.Resources.CoreMessages.ECOSIM_SAVE_FAILED, strPath, "Output directory does not exist"), eMessageType.DataExport, eCoreComponentType.EcoSim, eMessageImportance.Information)
                Me.m_core.Messages.SendMessage(msg)
                Return False
            End If

            For Each outputtype As cEcosimResultWriter.eResultTypes In [Enum].GetValues(GetType(eResultTypes))
                If Me.ShouldWriteResult(results, outputtype) Then
                    Try
                        If Not Me.WriteResults(outputtype, strPath, True) Or Not Me.WriteResults(outputtype, strPath, False) Then
                            bSucces = False
                            msg = New cMessage(String.Format(My.Resources.CoreMessages.ECOSIM_RESULTS_SAVE_FAILED, strPath, outputtype.ToString), eMessageType.DataExport, eCoreComponentType.EcoSim, eMessageImportance.Warning)
                            Me.m_core.Messages.SendMessage(msg)
                        End If
                    Catch ex As Exception
                        bSucces = False
                        cLog.Write(String.Format("Exception in cEcosimResultWriter: {0}", ex.Message))
                    End Try
                End If
            Next

            If bSucces Then
                msg = New cMessage(String.Format(My.Resources.CoreMessages.ECOSIM_RESULTS_SAVE_SUCCESS, strPath), eMessageType.DataExport, eCoreComponentType.EcoSim, eMessageImportance.Information)
                Me.m_core.Messages.SendMessage(msg)
            End If
            Return bSucces

        End Function

#End Region ' Public interfaces

#Region " Internal helpers "

        Private Function ShouldWriteResult(ByVal aResults As eResultTypes(), ByVal result As eResultTypes) As Boolean

            If (aResults Is Nothing) Then Return True
            If (aResults.Length = 0) Then Return True
            Return (Array.IndexOf(aResults, result) > -1)

        End Function

        Private Function WriteResults(ByVal resulttype As eResultTypes, _
                                      ByVal strPath As String, _
                                      ByVal bSaveAnnual As Boolean) As Boolean

            Dim strModelDetails As String = Me.GetModelDetails()
            Dim strDataDetails As String = ""
            Dim astrGroupNames As String = Me.GetAllGroupNames()
            Dim grpOutput As cEcosimGroupOutput = Nothing
            Dim bSuccess As Boolean = True

            Select Case resulttype

                Case eResultTypes.Biomass, _
                     eResultTypes.Mortality, _
                     eResultTypes.Yield, _
                     eResultTypes.ConsumptionBiomass, _
                     eResultTypes.FeedingTime, _
                     eResultTypes.AvgWeightOrProdCons, _
                     eResultTypes.TL, _
                     eResultTypes.Value

                    Dim data(m_core.nGroups, m_core.nEcosimTimeSteps) As Single
                    For i As Integer = 1 To m_core.nGroups
                        grpOutput = m_core.EcoSimGroupOutputs(i)
                        For j As Integer = 1 To m_core.nEcosimTimeSteps
                            Select Case resulttype
                                Case eResultTypes.Biomass
                                    data(i, j) = grpOutput.Biomass(j)
                                Case eResultTypes.Mortality
                                    data(i, j) = grpOutput.TotalMort(j)
                                Case eResultTypes.Yield
                                    data(i, j) = grpOutput.Yield(j)
                                Case eResultTypes.ConsumptionBiomass
                                    data(i, j) = grpOutput.ConsumpBiomass(j)
                                Case eResultTypes.FeedingTime
                                    data(i, j) = grpOutput.FeedingTime(j)
                                Case eResultTypes.AvgWeightOrProdCons
                                    If grpOutput.isMultiStanza Then
                                        data(i, j) = grpOutput.AvgWeight(j)
                                    Else
                                        data(i, j) = grpOutput.ProdConsump(j)
                                    End If
                                Case eResultTypes.TL
                                    data(i, j) = grpOutput.TL(j)
                                Case eResultTypes.Value
                                    data(i, j) = grpOutput.Value(j)

                            End Select
                        Next

                    Next
                    strDataDetails = "Data, " & resulttype.ToString
                    bSuccess = Me.SaveDataToFile(Me.GetOutputFileName(strPath, bSaveAnnual, resulttype), _
                                                 bSaveAnnual, data, _
                                                 strModelDetails, strDataDetails, astrGroupNames)

                Case eResultTypes.PredationMortality

                    For iGroup As Integer = 1 To Me.m_core.nGroups

                        grpOutput = m_core.EcoSimGroupOutputs(iGroup)

                        Dim iNumPred As Integer = 0
                        Dim predNames As New StringBuilder

                        For i As Integer = 1 To m_core.nLivingGroups
                            If grpOutput.isPred(i) Then
                                iNumPred += 1
                                predNames.Append("""" & m_core.EcoSimGroupOutputs(i).Name & """")
                                predNames.Append(",")
                            End If
                        Next

                        If (predNames.Length > 0) Then

                            Dim predData(iNumPred, m_core.nEcosimTimeSteps) As Single
                            iNumPred = 1

                            For i As Integer = 1 To m_core.nLivingGroups
                                If grpOutput.isPred(i) Then
                                    For j As Integer = 1 To m_core.nEcosimTimeSteps
                                        predData(iNumPred, j) = grpOutput.Predation(i, j)
                                    Next
                                    iNumPred += 1
                                End If
                            Next
                            strDataDetails = "Data, " & Chr(34) & resulttype.ToString & " of " & grpOutput.Name & Chr(34)

                            bSuccess = bSuccess And Me.SaveDataToFile(Me.GetOutputFileName(strPath, bSaveAnnual, resulttype, grpOutput.Name), _
                                                                      bSaveAnnual, predData, _
                                                                      strModelDetails, strDataDetails, predNames.ToString)
                        End If
                    Next

                Case eResultTypes.Prey

                    For iGroup As Integer = 1 To Me.m_core.nGroups

                        grpOutput = m_core.EcoSimGroupOutputs(iGroup)

                        Dim iNumPrey As Integer = 0
                        Dim preyNames As New StringBuilder

                        For i As Integer = 1 To m_core.nLivingGroups
                            If grpOutput.isPrey(i) Then
                                iNumPrey += 1
                                preyNames.Append("""" & m_core.EcoSimGroupOutputs(i).Name & """")
                                preyNames.Append(",")
                            End If
                        Next

                        If (preyNames.Length > 0) Then

                            Dim preyData(iNumPrey, m_core.nEcosimTimeSteps) As Single
                            iNumPrey = 1

                            For i As Integer = 1 To m_core.nLivingGroups
                                If grpOutput.isPrey(i) Then
                                    For j As Integer = 1 To m_core.nEcosimTimeSteps
                                        preyData(iNumPrey, j) = grpOutput.PreyPercentage(i, j)
                                    Next
                                    iNumPrey += 1
                                End If
                            Next

                            strDataDetails = "Data, " & Chr(34) & resulttype.ToString & " of " & grpOutput.Name & Chr(34)
                            bSuccess = bSuccess And Me.SaveDataToFile(Me.GetOutputFileName(strPath, bSaveAnnual, resulttype, grpOutput.Name), _
                                                  bSaveAnnual, preyData, _
                                                  strModelDetails, strDataDetails, preyNames.ToString)
                        End If
                    Next
            End Select

            Return bSuccess

        End Function

        Private Function GetOutputFileName(ByVal strPath As String, _
                                          ByVal bSaveAnnual As Boolean, _
                                          ByVal outputtype As eResultTypes, _
                                          Optional ByVal GroupName As String = "") As String

            Dim strFileName As String = ""
            Dim strExt As String = ".csv"

            If bSaveAnnual Then
                Select Case outputtype
                    Case eResultTypes.Biomass
                        strFileName = Me.m_core.EcosimOutputFileLocation("Biomass_annual", "", strExt)
                    Case eResultTypes.Mortality
                        strFileName = Me.m_core.EcosimOutputFileLocation("Mortality_annual", "", strExt)
                    Case eResultTypes.Yield
                        strFileName = Me.m_core.EcosimOutputFileLocation("Yield_annual", "", strExt)
                    Case eResultTypes.ConsumptionBiomass
                        strFileName = Me.m_core.EcosimOutputFileLocation("Cons_biom_annual", "", strExt)
                    Case eResultTypes.FeedingTime
                        strFileName = Me.m_core.EcosimOutputFileLocation("Feedingtime_annual", "", strExt)
                    Case eResultTypes.AvgWeightOrProdCons
                        strFileName = Me.m_core.EcosimOutputFileLocation("Weight_annual", "", strExt)
                    Case eResultTypes.PredationMortality
                        strFileName = Me.m_core.EcosimOutputFileLocation("Predation_annual " & GroupName, "", strExt)
                    Case eResultTypes.Prey
                        strFileName = Me.m_core.EcosimOutputFileLocation("Prey_annual " & GroupName, "", strExt)
                    Case eResultTypes.TL
                        strFileName = Me.m_core.EcosimOutputFileLocation("TL_annual", "", strExt)
                    Case eResultTypes.Value
                        strFileName = Me.m_core.EcosimOutputFileLocation("Value_annual", "", strExt)
                End Select
            Else
                Select Case outputtype
                    Case eResultTypes.Biomass
                        strFileName = Me.m_core.EcosimOutputFileLocation("Biomass", "", strExt)
                    Case eResultTypes.Mortality
                        strFileName = Me.m_core.EcosimOutputFileLocation("Mortality", "", strExt)
                    Case eResultTypes.Yield
                        strFileName = Me.m_core.EcosimOutputFileLocation("Yield", "", strExt)
                    Case eResultTypes.ConsumptionBiomass
                        strFileName = Me.m_core.EcosimOutputFileLocation("Cons_biom", "", strExt)
                    Case eResultTypes.FeedingTime
                        strFileName = Me.m_core.EcosimOutputFileLocation("Feedingtime", "", strExt)
                    Case eResultTypes.AvgWeightOrProdCons
                        strFileName = Me.m_core.EcosimOutputFileLocation("Weight", "", strExt)
                    Case eResultTypes.PredationMortality
                        strFileName = Me.m_core.EcosimOutputFileLocation("Predation " & GroupName, "", strExt)
                    Case eResultTypes.Prey
                        strFileName = Me.m_core.EcosimOutputFileLocation("Prey " & GroupName, "", strExt)
                    Case eResultTypes.TL
                        strFileName = Me.m_core.EcosimOutputFileLocation("TL", "", strExt)
                    Case eResultTypes.Value
                        strFileName = Me.m_core.EcosimOutputFileLocation("Value", "", strExt)
                End Select
            End If
            Return Path.Combine(strPath, strFileName)
        End Function

        Private Function SaveDataToFile(ByVal strFileName As String, _
                                        ByVal bSaveYearly As Boolean, _
                                        ByVal data As Single(,), _
                                        ByVal strModelDetails As String, _
                                        ByVal strDataDetails As String, _
                                        ByVal strGroupNames As String) As Boolean

            Try
                'Overwritten the file
                Using sw As StreamWriter = New StreamWriter(strFileName, False)
                    sw.WriteLine(strModelDetails)
                    sw.WriteLine(strDataDetails)
                    sw.WriteLine()
                    sw.WriteLine(strGroupNames)
                    If bSaveYearly Then
                        Dim simYears As Integer = CInt((data.GetLength(1) - 1) / cCore.N_MONTHS)
                        Dim nGroups As Integer = data.GetLength(0) - 1
                        Dim sum(nGroups) As Single
                        For j As Integer = 1 To simYears
                            ReDim sum(nGroups)
                            For i As Integer = 1 To nGroups
                                For k As Integer = 1 To cCore.N_MONTHS
                                    sum(i) = sum(i) + data(i, (j - 1) * cCore.N_MONTHS + k)
                                Next
                                If i > 1 Then sw.Write(", ")
                                sw.Write(cStringUtils.FormatSingle(sum(i) / cCore.N_MONTHS))
                            Next
                            sw.WriteLine()
                        Next
                    Else
                        'Each time steps
                        For j As Integer = 1 To data.GetLength(1) - 1
                            'For every group
                            For i As Integer = 1 To data.GetLength(0) - 1
                                If i > 1 Then sw.Write(", ")
                                sw.Write(cStringUtils.FormatSingle(data(i, j)))
                            Next
                            sw.WriteLine()
                        Next
                    End If
                    sw.Close()

                End Using

            Catch ex As Exception
                Return False
            End Try
            Return True

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get default model details to report in output file.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Function GetModelDetails() As String

            Dim sb As New StringBuilder()

            ' File
            sb.Append("ModelFile,")
            sb.AppendLine(Me.m_core.DataSource.ToString)
            'Add the model name
            sb.Append("ModelName, ")
            sb.AppendLine(Me.m_core.EwEModel.Name)
            'Add the active scenario name
            sb.Append("EcosimScenario,")
            sb.AppendLine(Me.m_core.EcosimScenarios(m_core.ActiveEcosimScenarioIndex).Name)

            ' Append time series name to scenario, if any
            sb.Append("TimeSeries,")
            If Me.m_core.ActiveTimeSeriesDatasetIndex > 0 Then
                sb.Append(Chr(34) & Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex).Name & Chr(34))
            Else
                sb.Append("(none)")
            End If

            Return sb.ToString()

        End Function

        Private Function GetAllGroupNames() As String

            Dim str As New StringBuilder()

            For i As Integer = 1 To Me.m_core.nGroups
                str.Append("""" & Me.m_core.EcoSimGroupOutputs(i).Name & """")
                If i <> Me.m_core.nGroups Then str.Append(",")
            Next

            Return str.ToString()

        End Function

#End Region ' Internal helpers

    End Class

End Namespace
