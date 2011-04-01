#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to write Ecosim results to a .csv file.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcosimResultWriter

#Region " Private vars "

    Private m_core As cCore = Nothing

    Private Enum eResultTypes As Integer
        Biomass = 0
        Mortality
        Yield
        ConsumptionBiomass
        FeedingTime
        AvgWeightOrProdCons
        PredationMortality
        Prey
        TL
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
    ''' <param name="bSaveAnnual">States whether to save annual averages (true)
    ''' or monthly values (false). True by default.</param>
    ''' <param name="iGroup">The group to save results for. This parameter only
    ''' applies to <see cref="eResultTypes.PredationMortality">prediation mortality</see>
    ''' and <see cref="eResultTypes.PredationMortality">prey</see> outputs. If
    ''' no group value is specified (i.e. iGroup equals <see cref="cCore.NULL_VALUE">core NULL</see>),
    ''' these two Ecosim outputs will not be saved.</param>
    ''' <returns>True if saved successfully.</returns>
    ''' -----------------------------------------------------------------------
    Public Function WriteResults(Optional ByVal strPath As String = "", _
                                 Optional ByVal bSaveAnnual As Boolean = True, _
                                 Optional ByVal iGroup As Integer = cCore.NULL_VALUE) As Boolean

        Dim strMessageText As String = ""
        Dim msg As cMessage = Nothing
        Dim bSucces As Boolean = True

        If Not Me.m_core.StateMonitor.HasEcosimRan Then Return False

        If String.IsNullOrEmpty(strPath) Then
            strPath = Me.m_core.OutputPath
        End If

        ' Try to make sure that the output path is there
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then Return False

        For Each outputtype As cEcosimResultWriter.eResultTypes In [Enum].GetValues(GetType(eResultTypes))
            Try
                bSucces = bSucces And Me.WriteResults(outputtype, strPath, bSaveAnnual, iGroup)
            Catch ex As Exception
                bSucces = False
                cLog.Write(String.Format("Exception in cEcosimResultWriter: {0}", ex.Message))
            End Try
        Next

        If bSucces Then
            If bSaveAnnual Then
                strMessageText = String.Format(My.Resources.CoreMessages.ECOSIM_RESULTSANNUAL_SAVE_SUCCESS, _
                                               strPath)
            Else
                strMessageText = String.Format(My.Resources.CoreMessages.ECOSIM_RESULTSMONTHLY_SAVE_SUCCESS, _
                                               strPath)
            End If
        Else
            If bSaveAnnual Then
                strMessageText = String.Format(My.Resources.CoreMessages.ECOSIM_RESULTSANNUAL_SAVE_FAILED, _
                                               strPath)
            Else
                strMessageText = String.Format(My.Resources.CoreMessages.ECOSIM_RESULTSMONTHLY_SAVE_FAILED, _
                                               strPath)
            End If
        End If

        msg = New cMessage(strMessageText, eMessageType.Any, eCoreComponentType.EcoSim, eMessageImportance.Information)
        Me.m_core.Messages.SendMessage(msg)

    End Function

#End Region ' Public interfaces

#Region " Internal helpers "

    Private Function WriteResults(ByVal resulttype As eResultTypes, _
                                  ByVal strPath As String, _
                                  ByVal bSaveAnnual As Boolean, _
                                  Optional ByVal iGroup As Integer = cCore.NULL_VALUE) As Boolean

        Dim strFileName As String = Me.GetOutputFileName(strPath, bSaveAnnual, resulttype)
        Dim strModelDetails As String = Me.GetModelDetails()
        Dim astrGroupNames As String = Me.GetAllGroupNames()
        Dim grpOutput As cEcosimGroupOutput = Nothing

        Select Case resulttype

            Case eResultTypes.Biomass, _
                 eResultTypes.Mortality, _
                 eResultTypes.Yield, _
                 eResultTypes.ConsumptionBiomass, _
                 eResultTypes.FeedingTime, _
                 eResultTypes.AvgWeightOrProdCons, _
                 eResultTypes.TL

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
                        End Select
                    Next

                Next

                Return Me.SaveDataToFile(strFileName, bSaveAnnual, data, strModelDetails, astrGroupNames)

            Case eResultTypes.PredationMortality

                If iGroup < 0 Then Return True

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

                strModelDetails = String.Format("{0}, Prey:, {1}, (predation mortality rates)", strModelDetails, grpOutput.Name)
                Return Me.SaveDataToFile(strFileName, bSaveAnnual, predData, strModelDetails, predNames.ToString)

            Case eResultTypes.Prey

                If iGroup < 0 Then Return True

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

                strModelDetails = String.Format("{0}, predator:, {1}, (diets as proportions)", strModelDetails, grpOutput.Name)
                Return Me.SaveDataToFile(strFileName, bSaveAnnual, preyData, strModelDetails, preyNames.ToString)

        End Select

        Return False

    End Function


    Private Function GetOutputFileName(ByVal strPath As String, _
                                      ByVal bSaveAnnual As Boolean, _
                                      ByVal outputtype As eResultTypes) As String

        Dim strFileName As String = ""
        If bSaveAnnual Then
            Select Case outputtype
                Case eResultTypes.Biomass
                    strFileName = "EwE6-Ecosim_annual_biomass.csv"
                Case eResultTypes.Mortality
                    strFileName = "EwE6-Ecosim_annual_mortality.csv"
                Case eResultTypes.Yield
                    strFileName = "EwE6-Ecosim_annual_yield.csv"
                Case eResultTypes.ConsumptionBiomass
                    strFileName = "EwE6-Ecosim_annual_cons_biom.csv"
                Case eResultTypes.FeedingTime
                    strFileName = "EwE6-Ecosim_annual_feedingtime.csv"
                Case eResultTypes.AvgWeightOrProdCons
                    strFileName = "EwE6-Ecosim_annual_weight.csv"
                Case eResultTypes.PredationMortality
                    strFileName = "EwE6-Ecosim_annual_predation.csv"
                Case eResultTypes.Prey
                    strFileName = "EwE6-Ecosim_annual_prey.csv"
                Case eResultTypes.TL
                    strFileName = "EwE6-Ecosim_annual_TL.csv"
            End Select
        Else
            Select Case outputtype
                Case eResultTypes.Biomass
                    strFileName = "EwE6-Ecosim_biomass.csv"
                Case eResultTypes.Mortality
                    strFileName = "EwE6-Ecosim_mortality.csv"
                Case eResultTypes.Yield
                    strFileName = "EwE6-Ecosim_yield.csv"
                Case eResultTypes.ConsumptionBiomass
                    strFileName = "EwE6-Ecosim_cons_biom.csv"
                Case eResultTypes.FeedingTime
                    strFileName = "EwE6-Ecosim_feedingtime.csv"
                Case eResultTypes.AvgWeightOrProdCons
                    strFileName = "EwE6-Ecosim_weight.csv"
                Case eResultTypes.PredationMortality
                    strFileName = "EwE6-Ecosim_predation.csv"
                Case eResultTypes.Prey
                    strFileName = "EwE6-Ecosim_prey.csv"
                Case eResultTypes.TL
                    strFileName = "EwE6-Ecosim_TL.csv"
            End Select
        End If
        Return Path.Combine(strPath, strFileName)
    End Function

    Private Function SaveDataToFile(ByVal strFileName As String, _
                                    ByVal bSaveYearly As Boolean, _
                                    ByVal data As Single(,), _
                                    ByVal strModelDetails As String, _
                                    ByVal strGroupNames As String) As Boolean

        Try
            'Overwritten the file
            Using sw As StreamWriter = New StreamWriter(strFileName, False)
                sw.WriteLine(strModelDetails)
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
                            sw.Write(cStringUtils.FormatSingle(sum(i) / cCore.N_MONTHS))
                            sw.Write(", ")
                        Next
                        sw.WriteLine()
                    Next
                Else
                    'Each time steps
                    For j As Integer = 1 To data.GetLength(1) - 1
                        'For every group
                        For i As Integer = 1 To data.GetLength(0) - 1
                            sw.Write(cStringUtils.FormatSingle(data(i, j)))
                            sw.Write(", ")
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
        sb.Append(Me.m_core.DataSource.ToString)
        sb.Append(", ")
        'Add the model name
        sb.Append(Me.m_core.EwEModel.Name)
        sb.Append(", ")
        'Add the active scenario name
        sb.Append(Me.m_core.EcosimScenarios(m_core.ActiveEcosimScenarioIndex).Name)

        ' Append time series name to scenario, if any
        If Me.m_core.ActiveTimeSeriesDatasetIndex > 0 Then
            sb.Append(" (")
            sb.Append(Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex).Name)
            sb.Append(" )")
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
