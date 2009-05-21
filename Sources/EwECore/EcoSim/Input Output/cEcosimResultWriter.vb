'==============================================================================
'
' $Log: cEcosimResultWriter.vb,v $
' Revision 1.3  2009/05/21 18:53:35  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.2  2009/05/19 13:44:56  jeroens
' Renamed result writer methods
'
' Revision 1.1  2009/05/18 20:23:59  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to write Ecosim results to a CSV file.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcosimResultWriter

#Region " Private vars "

    Private m_core As cCore = Nothing

    Private Enum eResultTypes As Integer
        Biomass = 0
        Mortality = 1
        Yield = 2
        ConsumptionBiomass = 3
        FeedingTime = 4
        AvgWeightOrProdCons = 5
        PredationMortality = 6
        Prey = 7
    End Enum

#End Region ' Private vars

#Region " Public interfaces "

    Public Sub New(ByVal core As cCore)
        Me.m_core = core
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save all available Ecosim results to a CSV file.
    ''' </summary>
    ''' <param name="strPath"></param>
    ''' <param name="bSaveAnnual"></param>
    ''' <param name="iGroup"></param>
    ''' <returns>True if saved successfully.</returns>
    ''' -----------------------------------------------------------------------
    Public Function WriteResults(ByVal strPath As String, _
                                 ByVal bSaveAnnual As Boolean, _
                                 Optional ByVal iGroup As Integer = cCore.NULL_VALUE) As Boolean

        Dim strMessageText As String = ""
        Dim msg As cMessage = Nothing
        Dim bSucces As Boolean = True

        If Not Me.m_core.StateMonitor.HasEcosimRan Then Return False

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
        Dim strModelDetails As String = GetModelDetails()
        Dim astrGroupNames As String = GetAllGroupNames()
        Dim grpOutput As cEcosimGroupOutput = Nothing

        Select Case resulttype

            Case eResultTypes.Biomass, _
                 eResultTypes.Mortality, _
                 eResultTypes.Yield, _
                 eResultTypes.ConsumptionBiomass, _
                 eResultTypes.FeedingTime, _
                 eResultTypes.AvgWeightOrProdCons

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
                        End Select
                    Next

                Next

                Return Me.SaveDataToFile(strFileName, bSaveAnnual, data, strModelDetails, astrGroupNames)

            Case eResultTypes.PredationMortality
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

                strModelDetails = String.Format("{0},Prey:,{1}, Gives predation mortality rates for this group", strModelDetails, grpOutput.Name)
                Return Me.SaveDataToFile(strFileName, bSaveAnnual, predData, strModelDetails, predNames.ToString)

            Case eResultTypes.Prey

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

                strModelDetails = String.Format("{0},Predator:,{1}, Shows diets as proportions", strModelDetails, grpOutput.Name)
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
                    strFileName = "EwE6-Simplot_annual_biomass.csv"
                Case eResultTypes.Mortality
                    strFileName = "EwE6-Simplot_annual_mortality.csv"
                Case eResultTypes.Yield
                    strFileName = "EwE6-Simplot_annual_yield.csv"
                Case eResultTypes.ConsumptionBiomass
                    strFileName = "EwE6-Simplot_annual_cons_biom.csv"
                Case eResultTypes.FeedingTime
                    strFileName = "EwE6-Simplot_annual_feedingtime.csv"
                Case eResultTypes.AvgWeightOrProdCons
                    strFileName = "EwE6-Simplot_annual_weight.csv"
                Case eResultTypes.PredationMortality
                    strFileName = "EwE6-Simplot_annual_predation.csv"
                Case eResultTypes.Prey
                    strFileName = "EwE6-Simplot_annual_prey.csv"
            End Select
        Else
            Select Case outputtype
                Case eResultTypes.Biomass
                    strFileName = "EwE6-Simplot_biomass.csv"
                Case eResultTypes.Mortality
                    strFileName = "EwE6-Simplot_mortality.csv"
                Case eResultTypes.Yield
                    strFileName = "EwE6-Simplot_yield.csv"
                Case eResultTypes.ConsumptionBiomass
                    strFileName = "EwE6-Simplot_cons_biom.csv"
                Case eResultTypes.FeedingTime
                    strFileName = "EwE6-Simplot_feedingtime.csv"
                Case eResultTypes.AvgWeightOrProdCons
                    strFileName = "EwE6-Simplot_weight.csv"
                Case eResultTypes.PredationMortality
                    strFileName = "EwE6-Simplot_predation.csv"
                Case eResultTypes.Prey
                    strFileName = "EwE6-Simplot_prey.csv"
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
                            sw.Write(sum(i) / cCore.N_MONTHS)
                            sw.Write(",")
                        Next
                        sw.WriteLine()
                    Next
                Else
                    'Each time steps
                    For j As Integer = 1 To data.GetLength(1) - 1
                        'For every group
                        For i As Integer = 1 To data.GetLength(0) - 1
                            sw.Write(data(i, j))
                            sw.Write(",")
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


    ''' <summary>
    ''' This saving format is based on EwE5 code
    ''' </summary>
    Private Function GetModelDetails() As String

        Dim str As New StringBuilder()

        str.Append(Me.m_core.DataSource.ToString)
        str.Append(",")
        'Add the model name
        str.Append(Me.m_core.EwEModel.Name)
        str.Append(",")
        'Add the active scenario name
        str.Append(Me.m_core.EcosimScenarios(m_core.ActiveEcosimScenarioIndex).Name)

        Return str.ToString()

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
