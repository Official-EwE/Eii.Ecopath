Option Strict On

Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwECore.MSE
Imports EwECore.MSEBatchManager

Namespace MSEBatchManager


    Public Class cMSEBatchOutputWriter
        Implements EwECore.MSE.IMSEOutputWriter

        Private m_core As cCore
        Private m_dataDir As String
        Private m_MSEdata As cMSEDataStructures
        Private m_BatchData As cMSEBatchDataStructures

        Private m_nSim As Integer

        Private Const BIOMASS_FILENAME As String = "MSEBatch_Biomass"
        Private Const CATCH_FILENAME As String = "MSEBatch_Catch"
        Private Const MORT_FILENAME As String = "MSEBatch_FishingMort"
        Private Const PREDMORT_FILENAME As String = "MSEBatch_Pred"
        Private Const QB_FILENAME As String = "MSEBatch_QB"
        Private Const FEEDINGTIME_FILENAME As String = "MSEBatch_FeedingTime"

        Public Sub New(ByVal theCore As cCore, ByVal MSEData As cMSEDataStructures, ByVal MSEBatchData As cMSEBatchDataStructures)
            Me.m_core = theCore
            Me.m_MSEdata = MSEData
            Me.m_BatchData = MSEBatchData
        End Sub

        Public Sub InitBatchRun()

            For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                'only delete file that we are going to write too
                If Me.m_BatchData.isOuputSaved(iOut) Then

                    Try
                        Dim outfileName As String = Me.getOutputFileName(Me.OuputTypeToFileName(Me.m_BatchData.OuputType(iOut)), Me.getModelName)
                        'delete 
                        File.Delete(outfileName)
                    Catch ex As Exception
                        System.Console.WriteLine(ex.Message)
                    End Try

                End If
            Next

        End Sub


        Public Sub WriteBatchHeader()
            Dim header As New StringBuilder
            Dim strm() As StreamWriter
            Dim quote As String = """"
            Dim istrm As Integer

            strm = Me.getOuputStreams()

            For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                If Me.m_BatchData.isOuputSaved(iOut) Then

                    header = New StringBuilder()

                    header.AppendLine("Command_File," & quote & Me.m_BatchData.CommandFilename & quote)
                    Dim model As String = DirectCast(Me.m_core.DataSource.Connection, Database.cEwEAccessDatabase).Name
                    header.AppendLine("Model_Name," & quote & model & quote)
                    Dim scenario As String = Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex).Name
                    header.AppendLine("Ecosim_Scenario," & quote & scenario & quote)
                    header.AppendLine("Number_Groups," & Me.m_MSEdata.NGroups.ToString)
                    header.AppendLine("Number_Fleets," & Me.m_MSEdata.nFleets.ToString)

                    header.AppendLine("Run_Type," & CInt(Me.m_BatchData.RunType).ToString & "," & Me.m_BatchData.RunType.ToString)
                    header.AppendLine("Number_PP_Forcing," & Me.m_BatchData.nForcing.ToString)
                    header.AppendLine("Number_FleetControls," & Me.m_BatchData.nControlTypes.ToString)
                    header.AppendLine("Number_RunType_Iterations," & Me.m_BatchData.nParIters)
                    header.AppendLine("Number_Simulations," & Me.m_MSEdata.NTrials.ToString)

                    strm(istrm).Write(header)
                    strm(istrm).Close()
                    istrm += 1

                End If
            Next iOut

        End Sub

        Public Sub WriteIterationHeader(ByVal iForcing As Integer, ByVal iControl As Integer, ByVal iParameter As Integer)
            Dim header As New StringBuilder
            Dim strm() As StreamWriter
            Dim iStrm As Integer
            Dim quote As String = """"

            strm = Me.getOuputStreams()

            'loop over output types
            For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                'only save outputs where isOuputSaved() = True
                If Me.m_BatchData.isOuputSaved(iOut) Then

                    header = New StringBuilder()
                    Dim epdata As cEcopathDataStructures = Me.m_core.m_EcoPathData
                    header.AppendLine("")
                    header.AppendLine("Forcing_Name,Forcing_Index")
                    header.AppendLine(quote & Me.m_BatchData.ForcingNames(iForcing) & quote & "," & Me.m_BatchData.ForcingIndexes(iForcing).ToString)
                    header.AppendLine()

                    header.AppendLine("Fleet_Name, Fleet_Index, Control_Type")
                    For iflt As Integer = 1 To Me.m_MSEdata.nFleets
                        header.AppendLine(quote & epdata.FleetName(iflt) & quote & ", " & iflt.ToString & "," & CInt(Me.m_BatchData.ControlType(iControl, iflt)).ToString)
                    Next
                    header.AppendLine()

                    header.Append("Group_Name, Group_Index, ")
                    header.Append(Me.getRunTypeHeader() & ", ")
                    header.Append("Sim_Num")
                    For it As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                        header.Append(", " & it.ToString)
                    Next
                    header.AppendLine()

                    strm(iStrm).Write(header)
                    strm(iStrm).Close()
                    iStrm += 1

                End If

            Next iOut


        End Sub

        Public Function getOutputFileName(ByVal DataType As String, ByVal ModelName As String) As String
            Return Path.Combine(Me.DataDir, EwEUtils.Utilities.cFileUtils.ToValidFileName(DataType & "_" & Me.m_BatchData.RunType.ToString & "_" & ModelName & ".csv", False))
        End Function

        Public Sub saveIteration(ByVal ListOfData As Dictionary(Of cMSE.eResultsData, Single(,))) Implements IMSEOutputWriter.saveIteration

            Dim buff As StringBuilder
            Dim strm() As StreamWriter
            Dim esData As cEcosimDatastructures = Me.m_core.m_EcoSimData
            Dim epData As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim istrm As Integer
            Dim quote As String = """"
            Try
                Me.m_nSim += 1
                strm = Me.getOuputStreams()

                'loop over output types from the command file
                For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                    'only save outputs where isOuputSaved() = True
                    If Me.m_BatchData.isOuputSaved(iOut) Then

                        'WARNING: this only saves outputs that are by Group
                        For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                            Try

                                buff = New StringBuilder()
                                buff.Append(quote & epData.GroupName(igrp) & quote & ", ")
                                buff.Append(igrp.ToString & ", ")
                                buff.Append(Me.getRunTypeValue(igrp) & ", ")
                                buff.Append(Me.m_nSim)

                                Dim n As Integer = Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                                For its As Integer = 1 To n
                                    buff.Append(", ")
                                    'get the value for this output type as a string
                                    buff.Append(Me.getOuputValue(Me.m_BatchData.OuputType(iOut), igrp, its))
                                Next

                                strm(istrm).WriteLine(buff)
                                buff = Nothing
                            Catch ex As Exception
                                ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, epdata.GroupName(igrp)))
                                '     System.Console.WriteLine(Me.ToString & " Failed to write data to file " & getOutputFileName(BIOMASS_FILENAME, Me.getModelName()) & " Exception: " & ex.Message)
                            End Try
                        Next igrp

                        istrm += 1

                    End If
                Next iOut

                For istrm = 0 To strm.Length - 1
                    strm(istrm).Close()
                Next


            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".SaveIteration() Exception: " & ex.Message)
            End Try

        End Sub


        Private Function getOuputStreams() As StreamWriter()
            'ToDo_jb getOuputStreams handle errors opening stream
            Dim lstStreams As New List(Of StreamWriter)
            For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                'only save outputs where isOuputSaved() = True
                If Me.m_BatchData.isOuputSaved(iOut) Then
                    Dim outfileName As String = Me.getOutputFileName(Me.OuputTypeToFileName(Me.m_BatchData.OuputType(iOut)), Me.getModelName)
                    lstStreams.Add(New StreamWriter(outfileName, True))
                End If

            Next

            Return lstStreams.ToArray

        End Function

        Private Function OuputTypeToFileName(ByVal OutputType As eMSEBatchOuputTypes) As String
            Select Case OutputType

                Case eMSEBatchOuputTypes.Biomass
                    Return BIOMASS_FILENAME

                Case eMSEBatchOuputTypes.CatchByGroup
                    Return CATCH_FILENAME

                Case eMSEBatchOuputTypes.FeedingTime
                    Return FEEDINGTIME_FILENAME

                Case eMSEBatchOuputTypes.QB
                    Return QB_FILENAME

                Case eMSEBatchOuputTypes.FishingMortRate
                    Return MORT_FILENAME

                Case eMSEBatchOuputTypes.PredRate
                    Return PREDMORT_FILENAME

            End Select

            Debug.Assert(False, "Invalid output type " & OutputType.ToString)
            Return String.Empty

        End Function


        Private Function getOuputValue(ByVal OutputType As eMSEBatchOuputTypes, ByVal igrp As Integer, ByVal iTime As Integer) As String
            Select Case OutputType

                Case eMSEBatchOuputTypes.Biomass
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, iTime))

                Case eMSEBatchOuputTypes.CatchByGroup
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, iTime))

                Case eMSEBatchOuputTypes.FeedingTime
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.FeedingTime, igrp, iTime))

                Case eMSEBatchOuputTypes.QB
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.ConsumpBiomass, igrp, iTime))

                Case eMSEBatchOuputTypes.FishingMortRate
                    Dim sumF As Single
                    'sum F across all fleets
                    For iflt As Integer = 1 To Me.m_core.m_EcoSimData.nGear
                        sumF += Me.m_core.m_EcoSimData.ResultsSumFMortByGroupGear(igrp, iflt, iTime)
                    Next
                    Return cStringUtils.FormatSingle(sumF)

                Case eMSEBatchOuputTypes.PredRate
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.PredMort, igrp, iTime))

            End Select

            Return ""

        End Function

        Public ReadOnly Property DataDir() As String
            Get
                Return Me.m_BatchData.OuputDir
            End Get
        End Property

        Public Sub Init() Implements IMSEOutputWriter.Init

        End Sub


        Private Function getRunTypeHeader() As String
            Dim header As String

            Select Case Me.m_BatchData.RunType
                Case eMSEBatchRunTypes.Constant_F
                    header = "Constant_F"
                Case eMSEBatchRunTypes.Constant_Y
                    header = "Constant_Y"
                Case eMSEBatchRunTypes.TFM
                    header = "Blim, Bbase, Fmin, Fmax"
            End Select

            Debug.Assert(header IsNot String.Empty, Me.ToString & " Invalid run type.")
            Return header

        End Function

        Private Function getRunTypeValue(ByVal iGroup As Integer) As String
            Dim ouputStr As String

            Select Case Me.m_BatchData.RunType
                Case eMSEBatchRunTypes.Constant_F
                    ouputStr = Math.Round(Me.m_MSEdata.FixedF(iGroup), 5).ToString
                Case eMSEBatchRunTypes.Constant_Y
                    ouputStr = Math.Round(Me.m_MSEdata.TAC(iGroup), 5).ToString
                Case eMSEBatchRunTypes.TFM
                    ouputStr = Me.m_MSEdata.Blim(iGroup).ToString & ", " & Me.m_MSEdata.Bbase(iGroup).ToString & ", " & Me.m_MSEdata.Fmin(iGroup).ToString & ", " & Me.m_MSEdata.Fopt(iGroup).ToString
            End Select

            Debug.Assert(ouputStr IsNot String.Empty, Me.ToString & " Invalid run type.")
            Return ouputStr

        End Function


        Private Function getModelName() As String
            Dim modelPath As String = DirectCast(Me.m_core.DataSource.Connection, Database.cEwEAccessDatabase).Name
            Return Path.GetFileName(modelPath)
        End Function


        Public Sub setSimCounter()
            Me.m_nSim = 0
        End Sub

    End Class


#Region "Output by group"


    Public Class cMSEBatchOutputWriterByGroup
        Implements EwECore.MSE.IMSEOutputWriter

        Private m_core As cCore
        Private m_dataDir As String
        Private m_MSEdata As cMSEDataStructures
        Private m_BatchData As cMSEBatchDataStructures

        Private Const BIOMASS_FILENAME As String = "MSEBatch_Biomass"
        Private Const CATCH_FILENAME As String = "MSEBatch_CatchByGroup"
        Private Const MORT_FILENAME As String = "MSEBatch_TotalMort"
        Private Const PREDMORT_FILENAME As String = "MSEBatch_Pred"
        Private Const QB_FILENAME As String = "MSEBatch_QB"
        Private Const FEEDINGTIME_FILENAME As String = "MSEBatch_FeedingTime"



        Public Sub New(ByVal theCore As cCore, ByVal MSEData As cMSEDataStructures, ByVal MSEBatchData As cMSEBatchDataStructures)
            Me.m_core = theCore
            Me.m_MSEdata = MSEData
            Me.m_BatchData = MSEBatchData
        End Sub

        Public Sub InitBatchRun()

            For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                'only delete file that we are going to write too
                If Me.m_BatchData.isOuputSaved(iOut) Then


                    For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                        Try
                            Dim outfileName As String = Me.getOutputFileName(Me.OuputTypeToFileName(Me.m_BatchData.OuputType(iOut)), Me.m_core.m_EcoPathData.GroupName(igrp), Me.getModelName)
                            'delete 
                            File.Delete(outfileName)
                        Catch ex As Exception
                            System.Console.WriteLine(ex.Message)
                        End Try
                    Next

                End If
            Next

        End Sub


        Public Sub WriteBatchHeader()
            'Dim header As New StringBuilder
            'Dim strm() As StreamWriter
            'strm = Me.getOuputStreams()

            'For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

            '    If Me.m_BatchData.isOuputSaved(iOut) Then
            '        For igrp As Integer = 1 To Me.m_MSEdata.NGroups

            '            header = New StringBuilder()
            '            'write model, scenario and run type
            '            'header.AppendLine(Me.m_BatchData.)

            '        Next igrp
            '    End If
            'Next iOut

        End Sub

        Public Sub WriteIterationHeader(ByVal iForcing As Integer, ByVal iControl As Integer, ByVal iParameter As Integer)
            Dim header As New StringBuilder
            Dim strm() As StreamWriter
            Dim iStrm As Integer

            strm = Me.getOuputStreams()

            'loop over output types
            For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                'only save outputs where isOuputSaved() = True
                If Me.m_BatchData.isOuputSaved(iOut) Then
                    For igrp As Integer = 1 To Me.m_MSEdata.NGroups


                        header = New StringBuilder()
                        Dim epdata As cEcopathDataStructures = Me.m_core.m_EcoPathData
                        header.AppendLine("")
                        'header.AppendLine("MSE Run parameters")
                        'header.AppendLine("Primary production forcing")
                        header.AppendLine("Primary production, Name, Index")
                        header.AppendLine("PP Forcing," & Me.m_BatchData.ForcingNames(iForcing) & "," & Me.m_BatchData.ForcingIndexes(iForcing).ToString)
                        header.AppendLine()

                        'header.AppendLine("Control Types")
                        header.AppendLine("Fishing fleet , Control type")
                        For iflt As Integer = 1 To Me.m_MSEdata.nFleets
                            header.AppendLine(epdata.FleetName(iflt) & "," & Me.m_BatchData.ControlType(iControl, iflt).ToString)
                        Next
                        header.AppendLine()

                        'header.Append("Run type")
                        header.AppendLine(Me.getRunTypeHeader())
                        For i As Integer = 1 To Me.m_MSEdata.NGroups
                            'only if there is catch of some sort
                            If Me.m_core.m_EcoPathData.fCatch(i) > 0 Then
                                header.AppendLine(Me.getRunTypeValue(i))
                            End If
                        Next

                        header.AppendLine("")
                        header.AppendLine("MSE Data")

                        strm(iStrm).Write(header)
                        strm(iStrm).Close()
                        iStrm += 1
                    Next

                End If

            Next iOut


        End Sub

        Public Function getOutputFileName(ByVal DataType As String, ByVal GroupName As String, ByVal ModelName As String) As String
            Return Path.Combine(Me.DataDir, EwEUtils.Utilities.cFileUtils.ToValidFileName(DataType & "_" & GroupName & "_" & Me.m_BatchData.RunType.ToString & "_" & ModelName & ".csv", False))
        End Function

        Public Sub saveIteration(ByVal ListOfData As Dictionary(Of cMSE.eResultsData, Single(,))) Implements IMSEOutputWriter.saveIteration

            Dim buff As StringBuilder
            Dim strm() As StreamWriter
            Dim esData As cEcosimDatastructures = Me.m_core.m_EcoSimData
            Dim epData As cEcopathDataStructures = Me.m_core.m_EcoPathData
            Dim istrm As Integer
            Try

                strm = Me.getOuputStreams()

                'loop over output types from the command file
                For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                    'only save outputs where isOuputSaved() = True
                    If Me.m_BatchData.isOuputSaved(iOut) Then

                        ' strm.WriteLine(Me.m_BatchData.OuputType(iOut).ToString)

                        'WARNING: this only saves outputs that are by Group
                        For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                            Try

                                buff = New StringBuilder()
                                buff.Append(epData.GroupName(igrp))
                                Dim n As Integer = Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                                For its As Integer = 1 To n
                                    buff.Append(", ")
                                    'get the value for this output type as a string
                                    buff.Append(Me.getOuputValue(Me.m_BatchData.OuputType(iOut), igrp, its))
                                Next

                                strm(istrm).WriteLine(buff)
                                istrm += 1
                                buff = Nothing
                            Catch ex As Exception
                                ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, epdata.GroupName(igrp)))
                                '     System.Console.WriteLine(Me.ToString & " Failed to write data to file " & getOutputFileName(BIOMASS_FILENAME, Me.getModelName()) & " Exception: " & ex.Message)
                            End Try
                        Next igrp

                    End If
                Next iOut

                For istrm = 0 To strm.Length - 1
                    strm(istrm).Close()
                Next


            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".SaveIteration() Exception: " & ex.Message)
            End Try

        End Sub


        Private Function getOuputStreams() As StreamWriter()
            'ToDo_jb getOuputStreams handle errors opening stream
            Dim lstStreams As New List(Of StreamWriter)
            For iOut As Integer = 1 To Me.m_BatchData.nOuputTypes

                'only save outputs where isOuputSaved() = True
                If Me.m_BatchData.isOuputSaved(iOut) Then
                    For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                        Dim outfileName As String = Me.getOutputFileName(Me.OuputTypeToFileName(Me.m_BatchData.OuputType(iOut)), Me.m_core.m_EcoPathData.GroupName(igrp), Me.getModelName)
                        lstStreams.Add(New StreamWriter(outfileName, True))
                    Next

                End If

            Next

            Return lstStreams.ToArray

        End Function

        Private Function OuputTypeToFileName(ByVal OutputType As eMSEBatchOuputTypes) As String
            Select Case OutputType

                Case eMSEBatchOuputTypes.Biomass
                    Return BIOMASS_FILENAME

                Case eMSEBatchOuputTypes.CatchByGroup
                    Return CATCH_FILENAME

                Case eMSEBatchOuputTypes.FeedingTime
                    Return FEEDINGTIME_FILENAME

                Case eMSEBatchOuputTypes.QB
                    Return QB_FILENAME

                Case eMSEBatchOuputTypes.FishingMortRate
                    Return MORT_FILENAME

                Case eMSEBatchOuputTypes.PredRate
                    Return PREDMORT_FILENAME

            End Select

            Debug.Assert(False, "Invalid output type " & OutputType.ToString)
            Return String.Empty

        End Function


        Private Function getOuputValue(ByVal OutputType As eMSEBatchOuputTypes, ByVal igrp As Integer, ByVal iTime As Integer) As String
            Select Case OutputType

                Case eMSEBatchOuputTypes.Biomass
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, iTime))

                Case eMSEBatchOuputTypes.CatchByGroup
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, iTime))

                Case eMSEBatchOuputTypes.FeedingTime
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.FeedingTime, igrp, iTime))

                Case eMSEBatchOuputTypes.QB
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.ConsumpBiomass, igrp, iTime))

                Case eMSEBatchOuputTypes.FishingMortRate
                    Dim sumF As Single
                    'sum F across all fleets
                    For iflt As Integer = 1 To Me.m_core.m_EcoSimData.nGear
                        sumF += Me.m_core.m_EcoSimData.ResultsSumFMortByGroupGear(igrp, iflt, iTime)
                    Next
                    Return cStringUtils.FormatSingle(sumF)

                Case eMSEBatchOuputTypes.PredRate
                    Return cStringUtils.FormatSingle(Me.m_core.m_EcoSimData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.PredMort, igrp, iTime))

            End Select

            Return ""

        End Function

        Public ReadOnly Property DataDir() As String
            Get
                Return Me.m_BatchData.OuputDir
            End Get
        End Property

        Public Sub Init() Implements IMSEOutputWriter.Init

        End Sub


        Private Function getRunTypeHeader() As String
            Dim header As String

            Select Case Me.m_BatchData.RunType
                Case eMSEBatchRunTypes.Constant_F
                    header = "Group Name, F"
                Case eMSEBatchRunTypes.Constant_Y
                    header = "Group Name, Y"
                Case eMSEBatchRunTypes.TFM
                    header = "Group Name, Blim, Bbase, Fmax"
            End Select

            Debug.Assert(header IsNot String.Empty, Me.ToString & " Invalid run type.")
            Return header

        End Function

        Private Function getRunTypeValue(ByVal iGroup As Integer) As String
            Dim ouputStr As String

            Select Case Me.m_BatchData.RunType
                Case eMSEBatchRunTypes.Constant_F
                    ouputStr = Me.m_core.m_EcoPathData.GroupName(iGroup) & ", " & Me.m_MSEdata.FixedF(iGroup).ToString
                Case eMSEBatchRunTypes.Constant_Y
                    ouputStr = Me.m_core.m_EcoPathData.GroupName(iGroup) & ", " & Me.m_MSEdata.TAC(iGroup).ToString
                Case eMSEBatchRunTypes.TFM
                    ouputStr = Me.m_core.m_EcoPathData.GroupName(iGroup) & ", " & Me.m_MSEdata.Blim(iGroup).ToString & ", " & Me.m_MSEdata.Bbase(iGroup).ToString & ", " & Me.m_MSEdata.Fopt(iGroup).ToString
            End Select

            Debug.Assert(ouputStr IsNot String.Empty, Me.ToString & " Invalid run type.")
            Return ouputStr

        End Function


        Private Function getModelName() As String
            Dim modelPath As String = DirectCast(Me.m_core.DataSource.Connection, Database.cEwEAccessDatabase).Name
            Return Path.GetFileName(modelPath)
        End Function

    End Class

#End Region

End Namespace