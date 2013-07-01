Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports System.IO
Imports LumenWorks.Framework.IO.Csv
Imports System.Diagnostics



''' <summary>
''' Harvest Control Rules and Strategies all need to be public so they can be accessed in the frmTFMpolicy interface.
''' </summary>
''' <remarks></remarks>
Public Class HCR_Group
    Public GroupName4Biomass As String
    Public GroupNumber4Biomass As Integer
    Public LowerLimit As Double
    Public UpperLimit As Double
    Public GroupName4F As String
    Public GroupNumber4F As Integer
    Public MaxF As Double
    Public CostFunction As String

    Public ReadOnly Property toDisplayString
        Get
            Dim tmp As String
            tmp = "Biomass Group = " + GroupName4Biomass
            tmp += " , Biomass Index = " + GroupNumber4Biomass.ToString
            tmp += " , Fishing Mort. Group = " + GroupName4F
            tmp += " , Fishing Mort. Index = " + GroupNumber4F.ToString
            Return tmp
        End Get
    End Property

    Public Shared Function toCostFunctionString(eCostFunctionTypes As eCostFunctionTypes) As String
        Select Case eCostFunctionTypes

            Case EwEMSEPlugin.eCostFunctionTypes.Target
                Return "Target"
            Case EwEMSEPlugin.eCostFunctionTypes.Conservation
                Return "Conservation"
        End Select
        Return "Target"
    End Function


End Class


Public Enum HCRType
    Target = 0
    Conservation = 1
End Enum

Public Class cMSE
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimBeginTimestepPlugin

    'Public Strategies As New List(Of List(Of HCR_Group))
    'Private CurrentStrategy As List(Of HCR_Group)

    'Public Strategies As New List(Of Strategy)
    Public Strategies As New Strategies
    Private CurrentStrategy As Strategy

    Private MSEForm As frmMSE = Nothing
    Private mCore As cCore = Nothing
    Private m_uic As cUIContext = Nothing
    Private _ecosim As EwECore.Ecosim.cEcoSimModel = Nothing
    Private _simdata As cEcosimDatastructures
    Private _ecopath As Ecopath.cEcoPathModel
    Private _EcosimTimeStepDelegate As EwECore.Ecosim.EcoSimTimeStepDelegate
    Private StrategyIndex As Integer
    Private OriginalNTimesteps As Integer
    Private nPrimaryProducer As Integer
    Private NatMort() As Double
    Private NatMortName() As String
    Private ChangeInEffortLimits() As Double
    Const NoHCR_F As Integer = -9999

    Public DataPath As String = "C:\Users\Mark\Desktop\GAP\Data"
    Public ChangeEffortFlag As Boolean = False

    Enum DistributionType
        Uniform = 1
        Triangular = 2
    End Enum

    Private Sub ExtractChangeInEffortLimits()
        Dim EffortLimitsCSV As New CsvReader(New StreamReader(DataPath & "\Fleet\ChangesInEffortLimits.csv"), True)
        ReDim ChangeInEffortLimits(mCore.nFleets - 1)

        For i = 1 To mCore.nFleets
            ChangeInEffortLimits(i - 1) = -9999
        Next

        While Not EffortLimitsCSV.EndOfStream
            EffortLimitsCSV.ReadNextRecord()
            ChangeInEffortLimits(EffortLimitsCSV(0) - 1) = EffortLimitsCSV(2)
        End While

    End Sub

    Public Sub ExtractHCR()
        Dim StrategiesFileNames As String()
        Dim csv As CsvReader
        Dim tempHCRGroup As HCR_Group
        Dim Strategy As Strategy
        Dim datadir As String = Path.Combine(DataPath & "\Strategies")

        'Make sure this directory exists
        If Not Directory.Exists(datadir) Then
            MsgBox("Sorry this is not a valid data directory.", MsgBoxStyle.Critical)
            Return
        End If

        Strategies.DataDirectory = datadir

        'Get an array of strings giving the path to each HCR
        StrategiesFileNames = Directory.GetFiles(datadir)

        For Each HCRFileName In StrategiesFileNames 'loop through reading each HCR file
            csv = New CsvReader(New StreamReader(HCRFileName), True)
            'Create the new Strategy with the Filename as the strategy name
            Strategy = New Strategy(Path.GetFileNameWithoutExtension(HCRFileName), HCRFileName)
            While Not csv.EndOfStream 'Read each line in the file
                'Read all fields from csv and then add to the list that makes up the whole strategy
                csv.ReadNextRecord()
                'Each HCR Group needs to be a new object
                tempHCRGroup = New HCR_Group
                tempHCRGroup.GroupName4Biomass = csv(0)
                tempHCRGroup.GroupNumber4Biomass = csv(1)
                tempHCRGroup.LowerLimit = csv(2)
                tempHCRGroup.UpperLimit = csv(3)
                tempHCRGroup.GroupName4F = csv(4)
                tempHCRGroup.GroupNumber4F = csv(5)
                tempHCRGroup.MaxF = csv(6)
                tempHCRGroup.CostFunction = csv(7)
                Strategy.HCRules.Add(tempHCRGroup)
            End While
            Strategies.Add(Strategy)

        Next

    End Sub

    Private Function ExtractParamsCSV(ByRef param_name As String)
        Dim nIterations As Integer = Convert.ToInt32(MSEForm.txtnTrials.Text)
        Dim csv As New CsvReader(New StreamReader(DataPath & "\ParametersOut\" & param_name & "_out.csv"), True)
        Dim Params(nIterations - 1, csv.FieldCount - 1) As Double
        Dim iRecord As Integer = 0

        While Not csv.EndOfStream And iRecord < nIterations
            csv.ReadNextRecord()
            For iField = 1 To csv.FieldCount()
                Params(iRecord, iField - 1) = csv(iField - 1)
            Next
            iRecord += 1
        End While

        Return Params

        csv.Dispose()

    End Function

    Private Function ExtractVulnerabilitiesCSV()
        Dim nIterations As Integer = Convert.ToInt32(MSEForm.txtnTrials.Text)
        Dim csv As CsvReader
        Dim vulnerabilities(nIterations - 1, _ecopath.EcopathData.NumGroups - 1, _ecopath.EcopathData.NumGroups - 1) As Double

        For iIteration As Integer = 1 To nIterations
            csv = New CsvReader(New StreamReader(DataPath & "\ParametersOut\VulnerabilityIteration" & iIteration.ToString & "_out.csv"), True)
            While Not csv.EndOfStream
                csv.ReadNextRecord()
                For iPred As Integer = 1 To _ecopath.EcopathData.NumGroups
                    vulnerabilities(iIteration - 1, csv.CurrentRecordIndex, iPred - 1) = csv(iPred - 1)
                Next
            End While
        Next

        Return vulnerabilities

    End Function

    Private Sub LoadNaturalMortalites()

        Dim nRecords As Integer = 0
        Dim NatMortCSV As New CsvReader(New StreamReader(DataPath & "\naturalmortalities\NaturalMortalities.csv"), True)

        'Count how many records there are
        While NatMortCSV.ReadNextRecord
            nRecords += 1
        End While

        ReDim NatMort(nRecords - 1)
        ReDim NatMortName(nRecords - 1)

        NatMortCSV = New CsvReader(New StreamReader(DataPath & "\naturalmortalities\NaturalMortalities.csv"), True)

        While NatMortCSV.ReadNextRecord
            NatMortName(NatMortCSV.CurrentRecordIndex) = NatMortCSV("GroupName")
            NatMort(NatMortCSV.CurrentRecordIndex) = NatMortCSV("NaturalMortality")
        End While

    End Sub

    'Private Function LoadCostFunctionsCSV()
    '    Dim CostFunctionReader As CsvReader
    '    Dim CostFunctionArray(,) As String
    '    Dim CostFunctionArrayIndex As Integer = 0
    '    Dim StratFileNames() As String
    '    Dim FileName As String
    '    Dim FoundElement As Boolean

    '    'Get the names of the files in the strategies folder
    '    StratFileNames = Directory.GetFiles(DataPath & "\Strategies")

    '    'Redim the cost function array so that there are enough rows for each strategy
    '    ReDim CostFunctionArray(StratFileNames.Count - 2, 1)

    '    'Setup csvreader object
    '    CostFunctionReader = New CsvReader(New StreamReader(DataPath & "\Strategies\CostFunctionType.csv"), True)

    '    'Read the data in CostFunctionType.csv into the CostFunctionArray
    '    While Not CostFunctionReader.EndOfStream
    '        CostFunctionReader.ReadNextRecord()
    '        CostFunctionArray(CostFunctionArrayIndex, 0) = CostFunctionReader(0)
    '        CostFunctionArray(CostFunctionArrayIndex, 1) = CostFunctionReader(1)
    '        CostFunctionArrayIndex += 1
    '    End While

    '    'Check that all files in the strategies folder are represented in the CostFunctionArray
    '    For Each iFile In StratFileNames
    '        FileName = Path.GetFileName(iFile)
    '        If FileName = "CostFunctionType.csv" Then Continue For 'skip over the file that holds the costfunctions

    '        FoundElement = False
    '        For iCostFunctionElement = 0 To CostFunctionArray.GetLength(0) - 1
    '            If FileName = CostFunctionArray(iCostFunctionElement, 0) & ".csv" Then FoundElement = True
    '        Next

    '        If FoundElement = False Then
    '            Err.Raise(1000, "LoadCostFunctionsCSV", "Strategy file not listed in the CostFunctionType.csv file")
    '        End If

    '    Next

    '    Return CostFunctionArray

    'End Function

    Public Sub LoadSampledParams()

        Dim B(,) As Double
        Dim PB(,) As Double
        Dim QB(,) As Double
        Dim EE(,) As Double
        Dim BA(,) As Double
        Dim DenDepCatchability(,) As Double
        Dim FeedingTimeAdjustRate(,) As Double
        Dim MaxRelFeedingTime(,) As Double
        Dim OtherMortFeedingTime(,) As Double
        Dim PredEffectFeedingTime(,) As Double
        Dim QBMaxxQBio(,) As Double
        Dim SwitchingPower(,) As Double
        Dim Vulnerabilities(,,) As Double
        Dim nTrials As Integer
        Dim ecopathData As cEcopathDataStructures = Me._ecopath.EcopathData
        Dim ecosimData As cEcosimDatastructures = Me._ecosim.EcosimData
        Dim GoodDynamics As Boolean
        Dim NYearsProject = Convert.ToInt32(MSEForm.txtNYearsProject.Text)
        Dim BiomassProjected(NYearsProject * _ecosim.EcosimData.NumStepsPerYear - 1) As Double
        Dim Results As New DataTable
        Dim HCRFiles As String()
        Dim sw As StreamWriter
        Dim results_read As CsvReader
        Dim NumberIterationsAlreadyInResults As Integer
        Dim NumberIterationsAlreadyInFleets As Integer
        Dim TrajectoryCsv As StreamWriter
        Dim Trajectory2Csv As List(Of StreamWriter)             'Trajectories2 is similar to trajectories apart from it each file contains only 1 group
        Dim FleetCsv As StreamWriter
        Dim nFailedParameterisations As Integer = 0


        OriginalNTimesteps = _ecosim.EcosimData.NTimes

        'Output the final results
        If File.Exists(DataPath & "\Results\Results.csv") Then
            results_read = New CsvReader(New StreamReader(DataPath & "\Results\Results.csv"), True)
            'count the number of record
            While Not results_read.EndOfStream
                results_read.ReadNextRecord()
                NumberIterationsAlreadyInResults = results_read(0)
            End While
            results_read.Dispose()
            sw = New StreamWriter(DataPath & "\Results\Results.csv", True)
        Else
            sw = New StreamWriter(DataPath & "\Results\Results.csv", True)
            sw.WriteLine("Iteration,Strategy,GroupName,ResultName,Value")
        End If

        'check whether fleet.csv file exists and if so see how many iterations in it
        If File.Exists(DataPath & "\Results\Fleet.csv") Then
            results_read = New CsvReader(New StreamReader(DataPath & "\Results\Fleet.csv"), True)
            'count th number of records
            While Not results_read.EndOfStream()
                results_read.ReadNextRecord()
                NumberIterationsAlreadyInFleets = results_read(0)
            End While
            results_read.Dispose()
            'Create the csv writer for writing out individual fleets catches of each group
            FleetCsv = New StreamWriter(DataPath & "\Results\Fleet.csv", True)
        Else
            'Create the csv writer for writing out individual fleets catches of each group
            FleetCsv = New StreamWriter(DataPath & "\Results\Fleet.csv", True)
            FleetCsv.WriteLine("Iteration,Strategy,FleetName,GroupName,Value")
        End If

        'Get a list of all the strategy files in the strategies folder
        HCRFiles = Directory.GetFiles(DataPath & "\Strategies")

        'Extract the maximum percentage change in effort for each fleet from csv and put into array ChangeInEffortLimits
        'to be used by the optim to determine effort is beyond maximum change in effort
        ExtractChangeInEffortLimits()

        ''Load up what type of cost function to use for each strategy
        'Try
        '    CostFunctionTypeArray = LoadCostFunctionsCSV()
        'Catch e As Exception
        '    MessageBox.Show(e.ToString)
        'End Try

        'Load all the parameters from the <parameter name>_out.csv files
        B = ExtractParamsCSV("B")
        PB = ExtractParamsCSV("PB")
        QB = ExtractParamsCSV("QB")
        EE = ExtractParamsCSV("EE")
        BA = ExtractParamsCSV("BA")
        DenDepCatchability = ExtractParamsCSV("DenDepCatchability")
        FeedingTimeAdjustRate = ExtractParamsCSV("FeedingTimeAdjustRate")
        MaxRelFeedingTime = ExtractParamsCSV("MaxRelFeedingTime")
        OtherMortFeedingTime = ExtractParamsCSV("OtherMortFeedingTime")
        PredEffectFeedingTime = ExtractParamsCSV("PredEffectFeedingTime")
        QBMaxxQBio = ExtractParamsCSV("QBMaxxQBio")
        SwitchingPower = ExtractParamsCSV("SwitchingPower")
        Vulnerabilities = ExtractVulnerabilitiesCSV()

        'Prepare the trajectory csv with the column headings
        Trajectory2Csv = New List(Of StreamWriter)
        For igrp = 1 To mCore.nLivingGroups
            Trajectory2Csv.Add(New StreamWriter(DataPath & "\Results\Trajectories2\" & mCore.EcoPathGroupInputs(igrp).Name & ".csv", False))
            Trajectory2Csv(igrp - 1).Write("Trial,Strategy")
            For iTime = 1 To OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                Trajectory2Csv(igrp - 1).Write("," & iTime)
            Next
            Trajectory2Csv(igrp - 1).WriteLine()
        Next

        'Load the natural mortalities into class variable so that it can be used by EcosimBeginTimeStep
        'to convert the Instantaneous Fishing mortality for target F into an exploitation rate that can be used by EwE
        LoadNaturalMortalites()

        'load parameter values into ecopath and ecosim to be used
        nTrials = Convert.ToInt32(MSEForm.txtnTrials.Text)    '0 is the 1st dimension and 1' the second etc
        For iTrial = 1 To nTrials

            For igrp = 1 To mCore.nLivingGroups
                ecopathData.B(igrp) = B(iTrial - 1, igrp - 1)
                ecopathData.PB(igrp) = PB(iTrial - 1, igrp - 1)
                ecopathData.QB(igrp) = QB(iTrial - 1, igrp - 1)
                ecopathData.EE(igrp) = EE(iTrial - 1, igrp - 1)
                ecopathData.BA(igrp) = BA(iTrial - 1, igrp - 1)
            Next
            For igrp = 1 To mCore.nLivingGroups - nPrimaryProducer
                ecosimData.QmQo(igrp) = DenDepCatchability(iTrial - 1, igrp - 1)
                ecosimData.FtimeAdjust(igrp) = FeedingTimeAdjustRate(iTrial - 1, igrp - 1)
                ecosimData.FtimeMax(igrp) = MaxRelFeedingTime(iTrial - 1, igrp - 1)
                ecosimData.MoPred(igrp) = OtherMortFeedingTime(iTrial - 1, igrp - 1)
                ecosimData.RiskTime(igrp) = PredEffectFeedingTime(iTrial - 1, igrp - 1)
                ecosimData.CmCo(igrp) = QBMaxxQBio(iTrial - 1, igrp - 1)
                ecosimData.SwitchPower(igrp) = SwitchingPower(iTrial - 1, igrp - 1)
            Next

            For iPrey As Integer = 1 To Vulnerabilities.GetLength(1)
                For iPred As Integer = 1 To Vulnerabilities.GetLength(2)
                    ecosimData.VulMult(iPrey, iPred) = Vulnerabilities(iTrial - 1, iPrey - 1, iPred - 1)
                Next
            Next

            GoodDynamics = True

            Try

                Me._ecopath.Run()

                'make sure Ecosim computes the output data
                Me._ecosim.EcosimData.bTimestepOutput = True

                'No timestep call back
                Me._ecosim.TimeStepDelegate = Nothing

                'Run on the same thread 
                'this means Me._ecosim.Run() will block until Ecosim has finished running
                Me._ecosim.EcosimData.bMultiThreaded = False

                'increase the number of years for the projection
                mCore.EcoSimModelParameters.NumberYears = OriginalNTimesteps / _ecosim.EcosimData.NumStepsPerYear + NYearsProject

                For Each iStrategy In Strategies

                    CurrentStrategy = iStrategy
                    'Run Ecosim
                    Me._ecosim.Init(True)   'causes it to reset the f's to the base
                    Me._ecosim.Run()

                    'Check whether the biomass for any species goes beneath or hits zero
                    For iGrp As Integer = 1 To mCore.nLivingGroups
                        For iTimeStep As Integer = 1 To OriginalNTimesteps
                            'Console.Write(mCore.EcoSimGroupOutputs(iGrp).Biomass(iTimeStep).ToString & " ")
                            If Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp, iTimeStep) <= 0 Then GoodDynamics = False
                            'If mCore.EcoSimGroupOutputs(iGrp).Biomass(iTimeStep) <= 0 Then GoodDynamics = False
                            'Test
                            If GoodDynamics = False Then Exit For
                        Next
                        If GoodDynamics = False Then Exit For
                    Next
                    If GoodDynamics = False Then
                        Console.WriteLine("This set of parameters is no good")
                        nFailedParameterisations += 1
                        GoodDynamics = True
                        GoTo BadDynamics
                    Else
                        'Console.WriteLine("This parameter set is okay")
                    End If

                    'This creates the files we will write the biomass trajectories to
                    TrajectoryCsv = New StreamWriter(DataPath & "\Results\Trajectories\Trial" & iTrial & ".csv", False)
                    TrajectoryCsv.Write("Group, Strategy")
                    For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                        TrajectoryCsv.Write("," & iTime)
                    Next
                    TrajectoryCsv.WriteLine()


                    For iFleet = 1 To mCore.nFleets
                        For iGrp = 1 To mCore.nLivingGroups
                            'FleetCsv.WriteLine(iTrial + NumberIterationsAlreadyInFleets & "," & HCRFiles(Strategies.IndexOf(iStrategy)) & "," & mCore.FleetInputs(iFleet).Name & ",""" & mCore.EcoPathGroupInputs(iGrp).Name & """," & mCore.EcoSimGroupOutputs(iGrp).CatchEnd(iFleet))
                            FleetCsv.WriteLine(iTrial + NumberIterationsAlreadyInFleets & "," & IO.Path.GetFileNameWithoutExtension(HCRFiles(Strategies.IndexOf(iStrategy))) & "," & mCore.FleetInputs(iFleet).Name & ",""" & mCore.EcoPathGroupInputs(iGrp).Name & """," & Me._simdata.ResultsSumCatchByGroupGear(iGrp, iFleet, OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear))
                        Next
                    Next

                    For igrp = 1 To mCore.nLivingGroups
                        'calculate what the minimum biomass was for each group
                        For iTime As Integer = 1 To NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                            BiomassProjected(iTime - 1) = Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, OriginalNTimesteps + iTime)
                        Next

                        'Output to csv the biomass trajectories
                        TrajectoryCsv.Write("""" & mCore.EcoPathGroupInputs(igrp).Name & """," & IO.Path.GetFileNameWithoutExtension(HCRFiles(Strategies.IndexOf(iStrategy))))
                        For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                            TrajectoryCsv.Write("," & Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, iTime))
                        Next
                        TrajectoryCsv.WriteLine()

                        Trajectory2Csv(igrp - 1).Write(iTrial & "," & IO.Path.GetFileNameWithoutExtension(HCRFiles(Strategies.IndexOf(iStrategy))))
                        For iTime As Integer = 1 To OriginalNTimesteps + NYearsProject * _ecosim.EcosimData.NumStepsPerYear
                            Trajectory2Csv(igrp - 1).Write("," & Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, iTime))
                        Next
                        Trajectory2Csv(igrp - 1).WriteLine()

                        sw.WriteLine(NumberIterationsAlreadyInResults + iTrial & "," & IO.Path.GetFileNameWithoutExtension(HCRFiles(Strategies.IndexOf(iStrategy))) & ",""" & mCore.EcoPathGroupOutputs(igrp).Name & """,Biomass," & BiomassProjected.Min)
                        sw.WriteLine(NumberIterationsAlreadyInResults + iTrial & "," & IO.Path.GetFileNameWithoutExtension(HCRFiles(Strategies.IndexOf(iStrategy))) & ",""" & mCore.EcoPathGroupOutputs(igrp).Name & """,BiomassEnd," & Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, ecosimData.NTimes))
                        'Results.Rows.Add(iIteration, HCRFiles(Strategies.IndexOf(iStrategy)), mCore.EcoPathGroupOutputs(igrp).Name, "Catch", Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, ecosimData.NTimes))
                        'Console.WriteLine(mCore.EcoPathGroupInputs(igrp).Name & vbTab & Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, ecosimData.NTimes))
                        sw.WriteLine(NumberIterationsAlreadyInResults + iTrial & "," & IO.Path.GetFileNameWithoutExtension(HCRFiles(Strategies.IndexOf(iStrategy))) & ",""" & mCore.EcoPathGroupOutputs(igrp).Name & """,Catch," & Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, ecosimData.NTimes))

                    Next



                    TrajectoryCsv.Dispose()
                    For iFleet As Integer = 1 To mCore.nFleets
                        sw.WriteLine(NumberIterationsAlreadyInResults + iTrial & "," & IO.Path.GetFileNameWithoutExtension(HCRFiles(Strategies.IndexOf(iStrategy))) & ",""" & mCore.FleetInputs(iFleet).Name & """,TotalEndValue," & Me._simdata.ResultsSumValueByGear(iFleet, _ecosim.EcosimData.NTimes))
                    Next


                Next

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".RunEcosim() Exception: " & ex.Message)
            End Try

BadDynamics:  ' This is so that if the dynamics of a parameterisation are bad that we can skip out of the loops and onto the the next Trial

        Next

        For igrp = 1 To mCore.nLivingGroups
            Trajectory2Csv(igrp - 1).Dispose()
        Next

        'ecosimData.NTimes is the number of months so 17 years = 204 timesteps
        mCore.EcoSimModelParameters.NumberYears = OriginalNTimesteps / _ecosim.EcosimData.NumStepsPerYear

        'Provide user with a message stating how many of the Trials produced reasonable dynamics
        MsgBox(nTrials - nFailedParameterisations & " out of " & nTrials & " (" & (nTrials - nFailedParameterisations) * 100 / nTrials & "%) parameterisations produced reasonable dynamics and were used to generate results.")

        sw.Dispose()
        FleetCsv.Dispose()

    End Sub

    Public Function DirichletSample(ByVal nDimensions As Integer, ByRef a() As Single, ByRef DietMultiplier As Double)
        Dim u1, u2, b, p, x As Single
        Dim gamma(nDimensions - 1) As Single
        Dim dirichlet(nDimensions - 1) As Single
        Dim sumofgamma As Single

        For i = 0 To a.Length() - 1
            a(i) = a(i) * DietMultiplier
        Next

        For i As Integer = 0 To nDimensions - 1
step1:
            u1 = Rnd()
            b = (Math.E + a(i)) / Math.E
            p = b * u1
            If p >= 1 Then GoTo step3
step2:
            x = p ^ (1 / a(i))
            u2 = Rnd()
            If u2 > Math.Exp(-x) Then
                GoTo step1
            Else
                gamma(i) = x
                GoTo stepend
            End If
step3:
            x = -Math.Log((b - p) / a(i))
            u2 = Rnd()
            If u2 > x ^ (a(i) - 1) Then
                GoTo step1
            Else
                gamma(i) = x
            End If
stepend:
        Next

        sumofgamma = gamma.Sum()
        For i = 0 To nDimensions - 1
            dirichlet(i) = gamma(i) / sumofgamma
        Next

        Return (dirichlet)

    End Function

    Private Function UniformSample(ByVal min_par As Single, ByVal max_par As Single)

        Randomize()
        Return (min_par + Rnd() * (max_par - min_par))

    End Function

    Private Function TriangularSample(ByVal A_par As Single, ByVal B_par As Single, ByVal C_par As Single)
        Randomize()
        Dim U As Single = Rnd()
        If U < ((C_par - A_par) / (B_par - A_par)) Then
            Return A_par + Math.Sqrt(U * (B_par - A_par) * (C_par - A_par))
        Else
            Return B_par - Math.Sqrt((1 - U) * (B_par - A_par) * (B_par - C_par))
        End If

    End Function

    Private Sub SampleDietMatrix(ByRef Interacts(,) As Integer, ByRef MeanProportions(,) As Single, ByRef DietPropMultipliers() As Double)

        Dim MeanPropMod() As Single
        Dim SumInteractions(mCore.nLivingGroups - 1) As Single
        Dim TempDirichlet() As Single
        Dim PreyIndex As Integer
        Dim DirichStopWatch As New Stopwatch
        Dim NormaliseStopWatch As New Stopwatch
        Dim EcopathStopWatch As New Stopwatch
        Dim EcopathInternalStopWatch As New Stopwatch
        Dim ecopathData As cEcopathDataStructures = Me._ecopath.EcopathData
        Dim iPointer As Integer = 0

        'Dim DirichletArray(mCore.nLivingGroups - 1, mCore.nLivingGroups - 1) As Single

        'Array.Clear(DirichletArray, 0, DirichletArray.GetLength(1))

        'Generate a vector 'SumInteractions' that counts how many prey each predator has
        For iPred As Integer = 0 To mCore.nLivingGroups - 1
            For iPrey As Integer = 0 To mCore.nGroups
                SumInteractions(iPred) += Interacts(iPred, iPrey)
            Next
        Next

        For iPred As Integer = 0 To mCore.nLivingGroups - 1
            'mCore.EcoPathGroupInputs(iPred + 1).DietComp(0) = 0
            If (SumInteractions(iPred) = 0) Then    'No need to do any of this unless there is at least 1 prey for this parameter
                'Set all values to zero - if running slow might want to consider how this could be skipped - possibly setting whole array to zero at start
                For i = 0 To mCore.nGroups
                    ecopathData.DC(iPred + 1, i) = 0
                Next
            Else
                ' DirichStopWatch.Start()

                ReDim MeanPropMod(SumInteractions(iPred) - 1)
                iPointer = 0
                For iPrey = 0 To mCore.nGroups
                    If Interacts(iPred, iPrey) = 1 Then
                        MeanPropMod(iPointer) = MeanProportions(iPred, iPrey)
                        iPointer += 1
                    End If
                Next


                'Samples a set of Dirichlet distributed parameters
                TempDirichlet = DirichletSample(SumInteractions(iPred), MeanPropMod, DietPropMultipliers(iPred))

                ' DirichStopWatch.Stop()

                'Set all the diet values in ecopath to those sampled and checked to be within correct intervals
                PreyIndex = 0
                For i = 0 To TempDirichlet.GetLength(0) - 1
                    While Interacts(iPred, PreyIndex) = 0
                        ecopathData.DC(iPred + 1, PreyIndex) = 0
                        PreyIndex += 1
                    End While
                    ecopathData.DC(iPred + 1, PreyIndex) = TempDirichlet(i)
                    PreyIndex += 1
                Next

                ''Changes the output of TempDirichlet so that it is a vector of length mcore.nlivinggroups
                'If Interacts(iPred, 0) = 1 Then
                '    'mCore.EcoPathGroupInputs(iPred + 1).ImpDiet() = TempDirichlet(0)
                '    ecopathData.DC(iPred + 1, 0) = TempDirichlet(0)
                'End If
                'PreyIndex = 1
                ''EcopathStopWatch.Start()
                'For i = 1 To TempDirichlet.GetLength(0) - 1
                '    While Interacts(iPred, PreyIndex) = 0
                '        'DirichletArray(iPred, PreyIndex) = 0
                '        'mCore.EcoPathGroupInputs(iPred + 1).DietComp(PreyIndex) = 0
                '        ecopathData.DC(iPred + 1, PreyIndex) = 0
                '        PreyIndex += 1
                '    End While
                '    'DirichletArray(iPred, PreyIndex) = TempDirichlet(i)

                '    'EcopathInternalStopWatch.Start()
                '    'mCore.EcoPathGroupInputs(iPred + 1).DietComp(PreyIndex) = TempDirichlet(i)
                '    Console.WriteLine(TempDirichlet(i))
                '    ecopathData.DC(iPred + 1, PreyIndex) = TempDirichlet(i)
                '    'EcopathInternalStopWatch.Stop()
                '    PreyIndex += 1
                'Next
                ''EcopathStopWatch.Stop()

            End If

        Next

        'Console.WriteLine("Dirich Time = " & DirichStopWatch.ElapsedMilliseconds.ToString)
        'Console.WriteLine("Normalise Time = " & NormaliseStopWatch.ElapsedMilliseconds.ToString)
        'Console.WriteLine("Ecopath Time = " & EcopathStopWatch.ElapsedMilliseconds.ToString)
        'Console.WriteLine("Ecopath Internal Time = " & EcopathInternalStopWatch.ElapsedMilliseconds.ToString)

    End Sub

    'Private Sub SampleFunctionalGrpParams(ByVal ParamName As String, ByVal nIterations As Integer)
    '    Dim sPath As String = "C:\Users\mick\Desktop\GAP\Data"
    '    Dim csv = New CsvReader(New StreamReader(sPath & "/DistributionParameters/" & ParamName & ".csv"), True)
    '    Dim SampledValues(mCore.nGroups - 1, nIterations - 1) As Single

    '    For iGrp = 1 To mCore.nGroups
    '        csv.ReadNextRecord()
    '        For iIteration = 1 To nIterations
    '            If csv(1) = 1 Then
    '                SampledValues(iGrp - 1, iIteration - 1) = UniformSample(csv(2), csv(3))
    '            ElseIf csv(1) = 2 Then
    '                SampledValues(iGrp - 1, iIteration - 1) = TriangularSample(csv(2), csv(3), csv(4))
    '            End If
    '        Next
    '    Next

    '    Dim csvout As New StreamWriter(Path.Combine(sPath & "/ParametersOut/" & ParamName & ".csv"), True)
    '    csvout.Write(mCore.EcoPathGroupInputs(1).Name)
    '    If mCore.nGroups > 1 Then
    '        For iGrp = 2 To mCore.nGroups
    '            csvout.Write("," & mCore.EcoPathGroupInputs(iGrp).Name)
    '        Next
    '    End If
    '    csvout.WriteLine()
    '    For iIteration = 1 To nIterations
    '        csvout.Write(SampledValues(0, iIteration - 1))
    '        If mCore.nGroups > 1 Then
    '            For iGrp = 2 To mCore.nGroups
    '                csvout.Write("," & SampledValues(iGrp - 1, iIteration - 1))
    '            Next
    '        End If
    '        csvout.WriteLine()
    '    Next

    'End Sub

    Private Function CheckEcopathDistributionFilesOkay(ByVal csv As CsvReader, ByRef Param_Name As String)
        'Checks whether each of the Ecopath (not diet matrix) distribution files is has the correct functional groups in it
        'They should only have living groups
        'It does this by saving a true in the position of an array at the index at which it exists in EwE
        'It then sums the values in this array and checks that they are equal to nlivinggroups (TRUE=1)
        'The reason I have done this is to prevent the problem where a file might have replicate groups
        'If a file has replicate groups and we check each group to see if it is in EwE and it happens that the number
        'of groups in the file are equal to the number of living groups, the file will be wrongly accepted

        Dim Path As String = DataPath & "\DistributionParameters\" & Param_Name
        Dim correct(mCore.nGroups - 1) As Integer
        Dim TotalFound As Integer = 0

        'initialise correct to all zeros
        For i = 1 To mCore.nGroups
            correct(i - 1) = 0
        Next

        'cycle through each of the living functional groups each time checking if it exists in the file
        For igrp = 1 To mCore.nLivingGroups
            csv.ReadNextRecord()
            For xgrp = 1 To mCore.nGroups
                If csv(0) = _ecopath.EcopathData.GroupName(xgrp) Then
                    correct(xgrp - 1) += 1
                    Exit For
                End If
            Next
        Next

        'check that there are no replicates
        For igrp = 1 To mCore.nGroups
            If correct(igrp - 1) > 1 Then
                MsgBox("The distribution file for " & Param_Name & " has replicate groups in it.")
                Return False
            End If
        Next

        'sum all the values in correct to be use to diagnose whether there are the correct number of groups in the file
        For Each i In correct
            TotalFound += i
        Next

        If TotalFound < mCore.nLivingGroups Then 'Check whether there are too few groups in the file
            MsgBox("The distribution file for " & Param_Name & " does not have all the living groups in it.")
            Return False
        ElseIf TotalFound > mCore.nLivingGroups Then 'Check whether there are too many groups in the file
            MsgBox("The distribution file for " & Param_Name & " has non-living groups in it.")
            Return False
        Else
            Return True
        End If

    End Function

    Private Function CheckEcoSimDistributionFilesOkay(ByVal csv As CsvReader, ByRef Param_Name As String)
        'Checks whether each of the Ecosim distribution files (not vulnerabilities) is has the correct functional groups in it
        'They should have living groups excluding primary producers
        'It does this by saving a true in the position of an array at the index at which it exists in EwE
        'It then sums the values in this array and checks that they are equal to nlivinggroups (TRUE=1)
        'The reason I have done this is to prevent the problem where a file might have replicate groups
        'If a file has replicate groups and we check each group to see if it is in EwE and it happens that the number
        'of groups in the file are equal to the number of living groups, the file will be wrongly accepted

        Dim Path As String = DataPath & "\DistributionParameters\" & Param_Name
        Dim correct(mCore.nGroups - 1) As Integer
        Dim TotalFound As Integer = 0

        'initialise correct to all zeros
        For i = 1 To mCore.nGroups
            correct(i - 1) = 0
        Next

        'cycle through each of the living functional groups each time checking if it exists in the file
        While csv.ReadNextRecord
            For xgrp = 1 To mCore.nGroups
                If csv("GroupName") = _ecopath.EcopathData.GroupName(xgrp) Then
                    correct(xgrp - 1) += 1
                    Exit For
                End If
            Next
        End While

        'Check if any of the records are for groups which are primary producers
        For igrp = 1 To mCore.nGroups
            If correct(igrp - 1) > 0 And mCore.EcoPathGroupOutputs(igrp).IsProducer Then
                MsgBox("Your distribution file for " & Param_Name & " contains the group " & mCore.EcoPathGroupOutputs(igrp).Name & " in it. " & vbCrLf & "This is invalid because this group is a primary producer!")
                Return False
            End If
        Next

        'check that there are no replicates
        For igrp = 1 To mCore.nGroups
            If correct(igrp - 1) > 1 Then
                MsgBox("The distribution file for " & Param_Name & " has replicate groups in it.")
                Return False
            End If
        Next

        'sum all the values in correct to be use to diagnose whether there are the correct number of groups in the file
        For Each i In correct
            TotalFound += i
        Next

        If TotalFound < mCore.nLivingGroups - nPrimaryProducer Then 'Check whether there are too few groups in the file
            MsgBox("The distribution file for " & Param_Name & " does not have all the living groups that are not primary producers in it.")
            Return False
        ElseIf TotalFound > mCore.nLivingGroups - nPrimaryProducer Then 'Check whether there are too many groups in the file
            MsgBox("The distribution file for " & Param_Name & " has too many groups in it.")
            Return False
        Else
            Return True
        End If
    End Function

    Public Sub GenerateEcopathParamaters()

        Dim nLiving As Integer = mCore.nLivingGroups
        Dim nGroups As Integer = mCore.nGroups
        Dim MonteCarlo As cMonteCarloManager = mCore.EcosimMonteCarlo
        Dim nIterations As Integer = Convert.ToInt32(MSEForm.txtnTrials.Text)
        Dim b(nIterations, nGroups) As Single
        Dim ba(nIterations, nLiving) As Single
        Dim pb(nIterations, nLiving) As Single
        Dim qb(nIterations, nLiving) As Single
        Dim ee(nIterations, nLiving) As Single
        Dim TimeFindingBalanced As New Stopwatch

        'I am just altering the tolerance so that it can run faster; this needs deleting later
        MonteCarlo.EcopathEETolerance = Convert.ToSingle(MSEForm.txtTolerance.Text)

        'cMonteCarloManager.selectNewEcopathParameters() will alter the Ecopath Input parameters
        'We need to save the original state of Ecopath so it can be restored when we are done
        Me.SaveOriginalState()

        Try

            'Init some of the Monte Carlo parameters
            If Me.InitMonteCarloParameters() Then
                'Succeeded in intitializing Monte Carlo Parameters

                For iter As Integer = 1 To nIterations
                    Dim timestart As Date = Now
                    'Set the Ecopath parameters using the Monte Carlo input parameters set above

                    'For iMonteIterations As Integer = 1 To 1000000
                    'SampleDietMatrix() this takes too long to run so if to be included, we need to consider alternatives
                    'Console.WriteLine("Iteration: " & iMonteIterations)
                    TimeFindingBalanced.Start()
                    If MonteCarlo.selectNewEcopathParameters(10000000) Then

                        'write some of the new Ecopath parameters to the console window
                        'Again for debugging
                        Me.dumpEcopathParameters(iter)

                        For iGrp = 1 To nLiving
                            b(iter, iGrp) = MonteCarlo.Groups(iGrp).B
                            ba(iter, iGrp) = MonteCarlo.Groups(iGrp).BA
                            pb(iter, iGrp) = MonteCarlo.Groups(iGrp).PB
                            qb(iter, iGrp) = MonteCarlo.Groups(iGrp).QB
                            ee(iter, iGrp) = MonteCarlo.Groups(iGrp).EE
                        Next iGrp

                        ''This runs Ecosim without core support
                        'If Me.RunEcosim() Then
                        '    'dumps out some Ecosim results
                        '    Me.getEcosimResults()
                        'End If 'RunEcosim
                        'Exit For

                    Else
                        System.Console.WriteLine("Failed to find balanced Ecopath model")
                    End If ' MonteCarlo.selectNewEcopathParameters()


                    'Next

                    Console.WriteLine("Number of seconds to run iteration: " & (TimeFindingBalanced.ElapsedMilliseconds / 1000).ToString)
                    TimeFindingBalanced.Reset()

                Next iter
            Else
                Exit Sub
            End If 'Me.InitMonteCarloParameters()

            'Save the results to a .csv

            Dim sPath As String = DataPath & "\ParametersOut"
            Dim b_csvout As New StreamWriter(Path.Combine(sPath & "/b_out.csv"), False)
            Dim ba_csvout As New StreamWriter(Path.Combine(sPath & "/ba_out.csv"), False)
            Dim pb_csvout As New StreamWriter(Path.Combine(sPath & "/pb_out.csv"), False)
            Dim qb_csvout As New StreamWriter(Path.Combine(sPath & "/qb_out.csv"), False)
            Dim ee_csvout As New StreamWriter(Path.Combine(sPath & "/ee_out.csv"), False)

            b_csvout.Write(mCore.EcoPathGroupInputs(1).Name)
            ba_csvout.Write(mCore.EcoPathGroupInputs(1).Name)
            pb_csvout.Write(mCore.EcoPathGroupInputs(1).Name)
            qb_csvout.Write(mCore.EcoPathGroupInputs(1).Name)
            ee_csvout.Write(mCore.EcoPathGroupInputs(1).Name)

            For igrp As Integer = 2 To nLiving
                b_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
                ba_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
                pb_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
                qb_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
                ee_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
            Next

            b_csvout.WriteLine()
            ba_csvout.WriteLine()
            pb_csvout.WriteLine()
            qb_csvout.WriteLine()
            ee_csvout.WriteLine()

            For iter As Integer = 1 To nIterations
                b_csvout.Write(b(iter, 1))
                ba_csvout.Write(ba(iter, 1))
                pb_csvout.Write(pb(iter, 1))
                qb_csvout.Write(qb(iter, 1))
                ee_csvout.Write(ee(iter, 1))
                For igrp As Integer = 2 To nLiving
                    b_csvout.Write(", " & b(iter, igrp))
                    ba_csvout.Write(", " & ba(iter, igrp))
                    pb_csvout.Write(", " & pb(iter, igrp))
                    qb_csvout.Write(", " & qb(iter, igrp))
                    ee_csvout.Write(", " & ee(iter, igrp))
                Next
                b_csvout.WriteLine()
                ba_csvout.WriteLine()
                pb_csvout.WriteLine()
                qb_csvout.WriteLine()
                ee_csvout.WriteLine()
            Next

            b_csvout.Dispose()
            ba_csvout.Dispose()
            pb_csvout.Dispose()
            qb_csvout.Dispose()
            ee_csvout.Dispose()

        Catch ex As Exception

        End Try

        Me.RestoreOriginalState()


    End Sub

    Public Sub GenerateEcopathParamaters2()
        Dim nLiving As Integer = mCore.nLivingGroups
        Dim nGroups As Integer = mCore.nGroups
        Dim MonteCarlo As cMonteCarloManager = mCore.EcosimMonteCarlo
        Dim nTrials As Integer = Convert.ToInt32(MSEForm.txtnTrials.Text)
        Dim b(nTrials, nGroups) As Single
        Dim ba(nTrials, nLiving) As Single
        Dim pb(nTrials, nLiving) As Single
        Dim qb(nTrials, nLiving) As Single
        Dim ee(nTrials, nLiving) As Single
        Dim TimeFindingBalanced As New Stopwatch
        Dim csv_diet As CsvReader
        Dim csv_multipliers As CsvReader
        Dim MeanProportions(mCore.nLivingGroups - 1, mCore.nGroups) As Single
        Dim DietPropMultipliers(mCore.nLivingGroups - 1) As Double
        Dim Interacts(mCore.nLivingGroups - 1, mCore.nGroups) As Integer
        Dim sPath As String = DataPath & "\DistributionParameters"
        Dim SuccessfullyMassBalanced As Boolean = False

        'I am just altering the tolerance so that it can run faster; this needs deleting later
        MonteCarlo.EcopathEETolerance = Convert.ToSingle(MSEForm.txtTolerance.Text)

        'cMonteCarloManager.selectNewEcopathParameters() will alter the Ecopath Input parameters
        'We need to save the original state of Ecopath so it can be restored when we are done
        Me.SaveOriginalState()

        'Read in the values from the DietComposition.csv into each array
        csv_diet = New CsvReader(New StreamReader(sPath & "/DietComposition.csv"), True)
        For iPred As Integer = 1 To mCore.nLivingGroups
            For iPrey As Integer = 0 To mCore.nGroups
                csv_diet.ReadNextRecord()
                'Note about indices for interacts, lower and upper
                'The 1st index for predator runs from 0 and each element is equal to the same element+1 in mcore.ecopathgroupinputs
                'The 2nd index for prey runs from zero, where zero is the imports and then every other index is identical to mcore.ecopathgroupinputs
                Interacts(csv_diet(2) - 1, csv_diet(3)) = csv_diet(4)
                MeanProportions(csv_diet(2) - 1, csv_diet(3)) = csv_diet(5)
            Next
        Next

        'Read in the values from the DietCompositionMultipliers.csv
        csv_multipliers = New CsvReader(New StreamReader(sPath & "/DietCompositionMultipliers.csv"), True)
        Do While csv_multipliers.ReadNextRecord
            DietPropMultipliers(csv_multipliers(0) - 1) = csv_multipliers(2)
        Loop



        Try

            'Init some of the Monte Carlo parameters
            If Me.InitMonteCarloParameters() Then
                'Succeeded in intitializing Monte Carlo Parameters

                For iTrial As Integer = 1 To nTrials
                    Dim timestart As Date = Now
                    'Set the Ecopath parameters using the Monte Carlo input parameters set above

                    'For iMonteIterations As Integer = 1 To 1000000
                    'SampleDietMatrix() this takes too long to run so if to be included, we need to consider alternatives
                    'Console.WriteLine("Iteration: " & iMonteIterations)
                    TimeFindingBalanced.Start()

                    For iParameterSet = 1 To 100000000

                        'Write code here that generates a whole set of diet parameters to be used in combination with new ecopath parameters
                        'to be tested for the mass-balance criteria
                        'SampleDietMatrix(Interacts, MeanProportions, DietPropMultipliers)

                        If MonteCarlo.selectNewEcopathParameters(1) Then
                            SuccessfullyMassBalanced = True
                            'write some of the new Ecopath parameters to the console window
                            'Again for debugging
                            Me.dumpEcopathParameters(iTrial)
                            Console.WriteLine("Mass balanced @ trial " & iTrial)

                            For iGrp = 1 To nLiving
                                b(iTrial, iGrp) = MonteCarlo.Groups(iGrp).B
                                ba(iTrial, iGrp) = MonteCarlo.Groups(iGrp).BA
                                pb(iTrial, iGrp) = MonteCarlo.Groups(iGrp).PB
                                qb(iTrial, iGrp) = MonteCarlo.Groups(iGrp).QB
                                ee(iTrial, iGrp) = MonteCarlo.Groups(iGrp).EE
                            Next iGrp

                            Exit For

                            ''This runs Ecosim without core support
                            'If Me.RunEcosim() Then
                            '    'dumps out some Ecosim results
                            '    Me.getEcosimResults()
                            'End If 'RunEcosim
                            'Exit For
                            'Else
                            '    System.Console.WriteLine("Failed to find balanced Ecopath model")
                        End If ' MonteCarlo.selectNewEcopathParameters()

                    Next

                    'If MonteCarlo.selectNewEcopathParameters(10000) Then

                    '    'write some of the new Ecopath parameters to the console window
                    '    'Again for debugging
                    '    Me.dumpEcopathParameters(iTrial)

                    '    For iGrp = 1 To nLiving
                    '        b(iTrial, iGrp) = MonteCarlo.Groups(iGrp).B
                    '        ba(iTrial, iGrp) = MonteCarlo.Groups(iGrp).BA
                    '        pb(iTrial, iGrp) = MonteCarlo.Groups(iGrp).PB
                    '        qb(iTrial, iGrp) = MonteCarlo.Groups(iGrp).QB
                    '        ee(iTrial, iGrp) = MonteCarlo.Groups(iGrp).EE
                    '    Next iGrp

                    '    ''This runs Ecosim without core support
                    '    'If Me.RunEcosim() Then
                    '    '    'dumps out some Ecosim results
                    '    '    Me.getEcosimResults()
                    '    'End If 'RunEcosim
                    '    'Exit For

                    '    'Else
                    '    'System.Console.WriteLine("Failed to find balanced Ecopath model")
                    'End If ' MonteCarlo.selectNewEcopathParameters()

                    If SuccessfullyMassBalanced = True Then
                        Console.WriteLine("Successful found a set of parameters inc. diet matrix that is mass balanced!")
                    Else
                        Console.WriteLine("Failed to find mass-balanced parameter set!")
                    End If

                    'Next

                    Console.WriteLine("Number of seconds to run iteration: " & (TimeFindingBalanced.ElapsedMilliseconds / 1000).ToString)
                    TimeFindingBalanced.Reset()

                Next iTrial
            Else
                Exit Sub
            End If 'Me.InitMonteCarloParameters()

            'Save the results to a .csv

            sPath = DataPath & "\ParametersOut"
            Dim b_csvout As New StreamWriter(Path.Combine(sPath & "/b_out.csv"), False)
            Dim ba_csvout As New StreamWriter(Path.Combine(sPath & "/ba_out.csv"), False)
            Dim pb_csvout As New StreamWriter(Path.Combine(sPath & "/pb_out.csv"), False)
            Dim qb_csvout As New StreamWriter(Path.Combine(sPath & "/qb_out.csv"), False)
            Dim ee_csvout As New StreamWriter(Path.Combine(sPath & "/ee_out.csv"), False)

            b_csvout.Write(mCore.EcoPathGroupInputs(1).Name)
            ba_csvout.Write(mCore.EcoPathGroupInputs(1).Name)
            pb_csvout.Write(mCore.EcoPathGroupInputs(1).Name)
            qb_csvout.Write(mCore.EcoPathGroupInputs(1).Name)
            ee_csvout.Write(mCore.EcoPathGroupInputs(1).Name)

            For igrp As Integer = 2 To nLiving
                b_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
                ba_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
                pb_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
                qb_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
                ee_csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
            Next

            b_csvout.WriteLine()
            ba_csvout.WriteLine()
            pb_csvout.WriteLine()
            qb_csvout.WriteLine()
            ee_csvout.WriteLine()

            For iter As Integer = 1 To nTrials
                b_csvout.Write(b(iter, 1))
                ba_csvout.Write(ba(iter, 1))
                pb_csvout.Write(pb(iter, 1))
                qb_csvout.Write(qb(iter, 1))
                ee_csvout.Write(ee(iter, 1))
                For igrp As Integer = 2 To nLiving
                    b_csvout.Write(", " & b(iter, igrp))
                    ba_csvout.Write(", " & ba(iter, igrp))
                    pb_csvout.Write(", " & pb(iter, igrp))
                    qb_csvout.Write(", " & qb(iter, igrp))
                    ee_csvout.Write(", " & ee(iter, igrp))
                Next
                b_csvout.WriteLine()
                ba_csvout.WriteLine()
                pb_csvout.WriteLine()
                qb_csvout.WriteLine()
                ee_csvout.WriteLine()
            Next

            b_csvout.Dispose()
            ba_csvout.Dispose()
            pb_csvout.Dispose()
            qb_csvout.Dispose()
            ee_csvout.Dispose()

        Catch ex As Exception

        End Try

        Me.RestoreOriginalState()
    End Sub

    Private Function getEcosimResults() As Boolean
        Try
            'Because we ran Ecosim directly from cEcosimModel.Run() instead of via the core cCore.RunEcosim()
            'the Core output objects cCore.EcoSimGroupOutputs() will not be populated
            'Instead get the Ecosim results directly from the underlying arrays
            Dim sumb() As Single
            ReDim sumb(mCore.nLivingGroups)
            For igrp As Integer = 1 To mCore.nLivingGroups
                'sum biomass over all the Ecosim timesteps
                For itime As Integer = 1 To mCore.nEcosimTimeSteps
                    'see cEcosimModel.PopulateResults() for how ResultsOverTime(var,group,time) are stored
                    sumb(igrp) += Me._simdata.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, itime)
                Next itime

                System.Console.WriteLine("Average Biomass for " & Me._ecopath.EcopathData.GroupName(igrp) & " = " & (sumb(igrp) / mCore.nEcosimTimeSteps).ToString)

            Next igrp

        Catch ex As Exception

        End Try

        Return Nothing

    End Function

    Private Function InitMonteCarloParameters() As Boolean
        'loads the distribution parameters for the Ecopath parameters from csvs

        Try

            Dim Path As String = DataPath & "\DistributionParameters"
            Dim csv_B, csv_PB, csv_QB, csv_EE, csv_BA As CsvReader
            Dim MonteCarlo As cMonteCarloManager = mCore.EcosimMonteCarlo
            Dim MCGroup As cMonteCarloGroup
            Dim xgrp As Integer
            'Initialize Monte Carlo parameters for B, PB, QB, EE and BA
            'These are the group parameters in the EwE Monte Carlo runs form
            'CV Lower and Upper Limit

            csv_B = New CsvReader(New StreamReader(Path & "/B_Dist.csv"), True)
            csv_PB = New CsvReader(New StreamReader(Path & "/PB_Dist.csv"), True)
            csv_QB = New CsvReader(New StreamReader(Path & "/QB_Dist.csv"), True)
            csv_EE = New CsvReader(New StreamReader(Path & "/EE_Dist.csv"), True)
            csv_BA = New CsvReader(New StreamReader(Path & "/BA_Dist.csv"), True)

            If Not CheckEcopathDistributionFilesOkay(csv_B, "biomass") Then Return False
            If Not CheckEcopathDistributionFilesOkay(csv_PB, "production/biomass") Then Return False
            If Not CheckEcopathDistributionFilesOkay(csv_QB, "consumption/biomass") Then Return False
            If Not CheckEcopathDistributionFilesOkay(csv_EE, "ecotrophic efficiency") Then Return False
            If Not CheckEcopathDistributionFilesOkay(csv_BA, "biomass accumulation") Then Return False

            csv_B = New CsvReader(New StreamReader(Path & "/B_Dist.csv"), True)
            csv_PB = New CsvReader(New StreamReader(Path & "/PB_Dist.csv"), True)
            csv_QB = New CsvReader(New StreamReader(Path & "/QB_Dist.csv"), True)
            csv_EE = New CsvReader(New StreamReader(Path & "/EE_Dist.csv"), True)
            csv_BA = New CsvReader(New StreamReader(Path & "/BA_Dist.csv"), True)

            'Set the cv values first ==================================================================================================================
            For igrp = 1 To mCore.nLivingGroups

                xgrp = 1
                csv_B.ReadNextRecord()
                csv_BA.ReadNextRecord()
                csv_EE.ReadNextRecord()
                csv_PB.ReadNextRecord()
                csv_QB.ReadNextRecord()

                'Make sure that .csv files are set up with group names in same order because xgrp is found only from the B file
                'and then is assumed to be the same for all other files
                While MonteCarlo.Groups(xgrp).Name <> csv_B(0) 'And xgrp <= mCore.nLivingGroups
                    xgrp += 1
                End While

                'If xgrp > mCore.nLivingGroups Then
                '    MsgBox(csv_B(0) & " cannot be found! Please make changes to your input file and try again")
                '    Return False
                'End If
                'If Not MonteCarlo.Groups(xgrp).IsLiving Then
                '    MsgBox(csv_B(0) & " is an invalid non-living group. Please make changes to your input file and try again.")
                'End If


                MCGroup = MonteCarlo.Groups(xgrp)

                'Setting a CV value will automatically set the Lower and Upper limits
                'by Calling cEcosimMonteCarlo.CalculateUpperLowerLimits()
                'If you want to manually set limits it must be done after the CV has been set

                'CVs
                MCGroup.Bcv = csv_B(1)
                MCGroup.PBcv = csv_PB(1)
                MCGroup.QBcv = csv_QB(1)
                MCGroup.EEcv = csv_EE(1)
                MCGroup.BAcv = csv_BA(1)

            Next '========================================================================================================================================================

            'reset the connection to the csv files ready to be read from the beginning again
            csv_B.Dispose()
            csv_BA.Dispose()
            csv_EE.Dispose()
            csv_PB.Dispose()
            csv_QB.Dispose()

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitMonteCarloParameters() Exception: " & ex.Message)
        End Try

        Return False
    End Function

    ''' <summary>
    ''' Save any variable that will be changed so the model can be restore to it's original state 
    ''' </summary>
    ''' <remarks>This just stores a sub set of variable as an example</remarks>
    Private Sub SaveOriginalState()
        Try
            'Have the MonteCarloManager save the values it will alter
            mCore.EcosimMonteCarlo.SaveOriginalValues()

            'Now store the variables that this app will change so they can be restored in RestoreOriginalState()

            'The makes sure Ecopath does not make a fuss, popping up message boxes, when it fails to balance a model
            Me._ecopath.suppressMessages = True

            'Make sure nothing is listening to Ecosim when we run it
            Me._EcosimTimeStepDelegate = Me._ecosim.TimeStepDelegate
            Me._EcosimTimeStepDelegate = Nothing

            'Save any parameters that we are going to change 
            'This has not been implemented here but...
            'For igrp = 1 To Core.nLivingGroups
            '    MCGroup = MonteCarlo.Groups(igrp)
            '   _orgB(igrp) =  MCGroup.Bcv 
            '    'PB, QB...               
            'Next

        Catch ex As Exception

        End Try

    End Sub

    Private Sub dumpEcopathParameters(ByVal iteration As Integer)
        Dim nliving As Integer = Me.mCore.nLivingGroups
        Dim MonteCarlo As cMonteCarloManager = Me.mCore.EcosimMonteCarlo

        System.Console.WriteLine("Iteration = " & iteration.ToString)
        For igrp = 1 To nliving
            Dim mcGrp As cMonteCarloGroup = MonteCarlo.Groups(igrp)
            System.Console.Write(mcGrp.Name & " = " & mcGrp.B & " , ")
            'Other parameters...  mcGrp.PB
        Next igrp
        System.Console.WriteLine()

    End Sub

    Private Function RunEcosim() As Boolean

        Try

            'make sure Ecosim computes the output data
            Me._ecosim.EcosimData.bTimestepOutput = True

            'No timestep call back
            Me._ecosim.TimeStepDelegate = Nothing

            'Run on the same thread 
            'this means Me._ecosim.Run() will block until Ecosim has finished running
            Me._ecosim.EcosimData.bMultiThreaded = False

            'Run Ecosim without Core support 
            'This means Core Input/ouput objects will not be populate 
            'So you can not use cCore.EcoSimGroupOutputs() to retrieve the results
            Me._ecosim.Init(True)
            Return Me._ecosim.Run()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RunEcosim() Exception: " & ex.Message)
        End Try

        Return False

    End Function

    ''' <summary>
    ''' Restore the currently loaded model back to it's original state so that it can be run in the interface.
    ''' </summary>
    ''' <remarks>In some cases you may want to save changes you made to the model.</remarks>
    Private Sub RestoreOriginalState()
        Try
            'Have the MonteCarloManager restore it's variables to the original state
            mCore.EcosimMonteCarlo.RestoreOriginalValues()

            'Set the State variables that we changed back to their original state
            Me._ecopath.suppressMessages = False
            Me._ecosim.TimeStepDelegate = Me._EcosimTimeStepDelegate

            'Not included here but we should also set any Monte Carlo Parameters back to their original state
            'For example
            'For igrp = 1 To Core.nLivingGroups
            '    MCGroup = MonteCarlo.Groups(igrp)
            '    MCGroup.Bcv = _orgB(igrp)
            '    'PB, QB...               
            'Next

        Catch ex As Exception

        End Try

    End Sub

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "MSE Plugin"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            'Return EwEUtils.Core.eCoreExecutionState.EcosimCompleted
            Return EwEUtils.Core.eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Mark Platts CEFAS"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevlowestoft@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in to run CEFAS MSE"
        End Get
    End Property

    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        mCore = core

    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return Me.ControlText
        End Get
    End Property

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized

        _ecopath = objEcoPath
        _ecosim = objEcoSim

    End Sub

    Public Sub UIContext(ByVal uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Public Sub EcosimInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        Debug.Assert(TypeOf EcosimDatastructures Is cEcosimDatastructures, "EcosimInitialized() failed to pass in valid Ecosim Data!")
        If TypeOf EcosimDatastructures Is cEcosimDatastructures Then
            _simdata = DirectCast(EcosimDatastructures, cEcosimDatastructures)
        End If
    End Sub

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick
        Dim bHasForm As Boolean = False

        If MSEForm IsNot Nothing Then
            bHasForm = Not MSEForm.IsDisposed
        End If

        If Not bHasForm Then
            MSEForm = New frmMSE(mCore, Me)
            MSEForm.Initialize(m_uic)
            MSEForm.StartForm(sender, e, frmPlugin)
        End If

        'count the number of primary producers
        For igrp = 1 To mCore.nGroups
            If mCore.EcoPathGroupOutputs(igrp).IsProducer Then nPrimaryProducer += 1
        Next

    End Sub

    Public Sub Create2DimParams(ByVal ParamName As String)
        Dim sPath As String = DataPath & "\DistributionParameters"
        Dim csv = New CsvReader(New StreamReader(sPath & "\" & ParamName & ".csv"), True)
        Dim ParameterArray(mCore.nLivingGroups * mCore.nLivingGroups, 5) As Single
        Dim nIterations As Integer = Convert.ToInt32(MSEForm.txtnTrials.Text)
        Dim SampledParameters(nIterations, mCore.nLivingGroups, mCore.nLivingGroups)
        Dim eDistributionType As DistributionType

        For iGroup = 1 To mCore.nLivingGroups * mCore.nLivingGroups
            csv.ReadNextRecord()
            For iField = 1 To 5
                ParameterArray(iGroup - 1, iField) = csv(iField)
            Next
        Next

        'Generate an array of sample parameters
        For iGroup = 1 To mCore.nLivingGroups
            For jGroup = 1 To mCore.nLivingGroups
                eDistributionType = ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 1)
                For iIteration = 1 To nIterations

                    Select Case eDistributionType
                        Case DistributionType.Uniform
                            SampledParameters(iIteration - 1, iGroup, jGroup) = UniformSample(ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 2), ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 3))

                        Case DistributionType.Triangular
                            SampledParameters(iIteration - 1, iGroup, jGroup) = TriangularSample(ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 2), ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 3), ParameterArray((iGroup - 1) * mCore.nLivingGroups + jGroup - 1, 4))
                    End Select

                Next
            Next
        Next

        For iIteration = 1 To nIterations
            'Output the sampled parameters to a csv
            sPath = DataPath & "\ParametersOut"
            Dim csvout As New StreamWriter(Path.Combine(sPath & "\" & ParamName & ToString(iIteration) & "out.csv"), True)

            For igrp As Integer = 1 To mCore.nLivingGroups
                csvout.Write(",""" & mCore.EcoPathGroupInputs(igrp).Name & """")
            Next
            csvout.WriteLine()

            For jGroup = 1 To mCore.nLivingGroups
                For iGroup = 1 To mCore.nLivingGroups
                    csvout.Write("," & SampledParameters(iIteration - 1, iGroup - 1, jGroup - 1))
                Next
                csvout.WriteLine()
            Next

            csvout.Dispose()
        Next




    End Sub

    Public Sub Create1DimParams(ByVal ParamName As String)
        Dim sPath As String = DataPath & "\DistributionParameters"
        Dim csv = New CsvReader(New StreamReader(sPath & "\" & ParamName & ".csv"), True)
        Dim ParameterArray(mCore.nLivingGroups - nPrimaryProducer - 1, 3) As Single
        Dim nIterations As Integer = Convert.ToInt32(MSEForm.txtnTrials.Text)
        'Dim SampledParameters(nIterations, mCore.nLivingGroups - nPrimaryProducer)
        Dim eDistributionType As DistributionType
        Dim SampledParameters(nIterations - 1, mCore.nLivingGroups - nPrimaryProducer - 1) As Double
        'Dim SampledParameters As DataTable
        'Dim row As DataRow
        Dim GroupNames(mCore.nLivingGroups - nPrimaryProducer - 1) As String

        If Not CheckEcoSimDistributionFilesOkay(csv, ParamName) Then
            Exit Sub
        End If

        csv = New CsvReader(New StreamReader(sPath & "\" & ParamName & ".csv"), True)

        'Initialise the datatable
        'SampledParameters.Columns.Add("GroupName", GetType(String))
        'For i = 1 To nIterations
        '    SampledParameters.Columns.Add(i, GetType(Double))
        'Next

        'Read all the distribution information from the .csv file and into an array ParameterArray
        While csv.ReadNextRecord()
            GroupNames(csv.CurrentRecordIndex) = csv("GroupName")
            For iField = 2 To 5
                ParameterArray(csv.CurrentRecordIndex, iField - 2) = csv(iField)
            Next
        End While


        'Generate an array of sample parameters
        For iGroup = 1 To mCore.nLivingGroups - nPrimaryProducer
            eDistributionType = ParameterArray(iGroup - 1, 0)
            'row = SampledParameters.NewRow()
            'row("GroupName") = 
            For iIteration = 1 To nIterations

                Select Case eDistributionType
                    Case DistributionType.Uniform
                        'row(iIteration - 1) =
                        If iGroup > 63 Then
                            Console.WriteLine("iGroup is >63")
                        End If
                        SampledParameters(iIteration - 1, iGroup - 1) = UniformSample(ParameterArray(iGroup - 1, 1), ParameterArray(iGroup - 1, 2))
                    Case DistributionType.Triangular
                        SampledParameters(iIteration - 1, iGroup - 1) = TriangularSample(ParameterArray(iGroup - 1, 1), ParameterArray(iGroup - 1, 2), ParameterArray(iGroup - 1, 3))
                End Select

            Next
        Next

        'Output the sampled parameters to a csv
        sPath = DataPath & "\ParametersOut"
        Dim csvout As New StreamWriter(Path.Combine(sPath & "\" & ParamName & "_out.csv"), False)

        For igrp As Integer = 1 To mCore.nLivingGroups - nPrimaryProducer - 1
            csvout.Write("""" & GroupNames(igrp - 1) & """,")
        Next
        csvout.Write("""" & GroupNames(mCore.nLivingGroups - nPrimaryProducer - 1) & """")

        csvout.WriteLine()

        For iIteration = 1 To nIterations
            For iGroup = 1 To mCore.nLivingGroups - nPrimaryProducer - 1
                csvout.Write(SampledParameters(iIteration - 1, iGroup - 1) & ",")
            Next
            csvout.Write(SampledParameters(iIteration - 1, mCore.nLivingGroups - nPrimaryProducer - 1))
            csvout.WriteLine()
        Next

        csv.Dispose()
        csvout.Dispose()

    End Sub

    Public Sub CreateVulnerabilities()
        'Generate csv with vulnerabilities

        Dim nIterations As Integer = Convert.ToInt32(MSEForm.txtnTrials.Text)

        For iIteration = 1 To nIterations

            Dim sw As StreamWriter = New StreamWriter(DataPath & "\ParametersOut\VulnerabilityIteration" & iIteration.ToString() & "_out.csv", False)

            'Create random values for the vulnerabilities and store in a csv
            For igrppredator As Integer = 1 To _ecopath.EcopathData().NumGroups

                sw.Write(Convert.ToSingle(1 + Math.Exp(9 * (Rnd() - 0.5))))
                For igrpprey As Integer = 2 To _ecopath.EcopathData().NumGroups
                    sw.Write("," & Convert.ToSingle(1 + Math.Exp(9 * (Rnd() - 0.5))))
                Next igrpprey

                sw.WriteLine()
            Next igrppredator

            sw.Close()

        Next

    End Sub

    Private Function CalcFfromHCR(ByRef Biomass As Single, ByRef MinBiomass As Single, ByRef MaxBiomass As Single, ByRef FMax As Single)

        If Biomass > MaxBiomass Then
            Return Convert.ToDouble(FMax)
        ElseIf Biomass < MinBiomass Then
            Return 0
        Else
            Return Convert.ToDouble(((Biomass - MinBiomass) / (MaxBiomass - MinBiomass)) * FMax)
        End If

    End Function

    Public Function DetermineZeroEffortFleets(ByRef FTargCons(,) As Double)
        Dim ZeroEffortFleets As New List(Of Integer)

        For iGrp = 1 To mCore.nGroups

            If FTargCons(iGrp - 1, HCRType.Conservation) = 0 Then

                For iFleet = 1 To mCore.nFleets

                    If mCore.FleetInputs(iFleet).Landings(iGrp) + mCore.FleetInputs(iFleet).Discards(iGrp) > 0 And _
                        Not ZeroEffortFleets.Contains(iFleet) Then
                        ZeroEffortFleets.Add(iFleet)
                    End If

                Next

            End If

        Next

        Return ZeroEffortFleets

    End Function

    Public Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcosimBeginTimestepPlugin.EcosimBeginTimeStep

        Dim TechnologyCreep(mCore.nFleets) As Single 'an array where each element represents the percentage with which each fleet increases its catching efficiency each year
        'Must have nfleets+1 elements so for 10 fleets needs elements 0-10
        'This is because of the way code works in EwE
        Dim TargetF(mCore.nGroups) As Double
        Dim CostFunctionType(mCore.nGroups) As String
        Dim mincost As Double = 1000000
        Dim Fleets2Fit As List(Of Integer) = New List(Of Integer)
        Dim QMult(_ecosim.EcosimData.nGroups) As Double
        Dim xgrp As Integer
        Dim tempFConservation As Double
        'used so that we don't repeat same groups when cycling through HCRs
        Dim LastYearsEffort(_ecosim.EcopathData.NumFleet - 1) As Double
        Dim variable_results() As Double


        If ChangeEffortFlag = True Then 'Flag is only set to true when the button on the form is clicked
            'this is so that its only executed when ecosim is run from mseform

            If iTime > OriginalNTimesteps And (iTime - 1) Mod 12 = 0 Then

                'Initialise the TechnologyCreep array. Change this variable into a class level variable and set it only once.
                For iTechCreep As Integer = 1 To TechnologyCreep.Length - 1
                    TechnologyCreep(iTechCreep) = 1
                Next

                'determine what the target F should be from the HCR's and current biomass levels for each group
                'count number of unique groups to calc F for in strategy
                'For iGrp = 1 To mCore.nLivingGroups
                '    For Each i In CurrentStrategy
                '        If i.GroupNumber4F = iGrpF Then
                '            countFGroups += 1
                '            Exit For
                '        End If
                '    Next
                'Nextt

                Dim FTargetandConservation(mCore.nGroups - 1, 1) As Double
                For i = 1 To mCore.nGroups
                    FTargetandConservation(i - 1, HCRType.Target) = NoHCR_F
                    FTargetandConservation(i - 1, HCRType.Conservation) = NoHCR_F
                Next

                For Each iHCRGroup In CurrentStrategy.HCRules
                    If iHCRGroup.CostFunction = "Target" Then
                        If FTargetandConservation(iHCRGroup.GroupNumber4F - 1, HCRType.Target) = NoHCR_F Then
                            FTargetandConservation(iHCRGroup.GroupNumber4F - 1, HCRType.Target) = CalcFfromHCR(BiomassAtTimestep(iHCRGroup.GroupNumber4Biomass), 0, iHCRGroup.UpperLimit, iHCRGroup.MaxF)
                        Else
                            MsgBox("There is more than one hcr that specifies the target F for group " & iHCRGroup.GroupNumber4F)
                        End If
                    ElseIf iHCRGroup.CostFunction = "Conservation" Then
                        tempFConservation = CalcFfromHCR(BiomassAtTimestep(iHCRGroup.GroupNumber4Biomass), iHCRGroup.LowerLimit, iHCRGroup.UpperLimit, iHCRGroup.MaxF)
                        If tempFConservation < FTargetandConservation(iHCRGroup.GroupNumber4F - 1, HCRType.Conservation) Or FTargetandConservation(iHCRGroup.GroupNumber4F - 1, HCRType.Conservation) = NoHCR_F Then
                            FTargetandConservation(iHCRGroup.GroupNumber4F - 1, HCRType.Conservation) = tempFConservation
                        End If
                    End If
                Next


                For iGrp = 1 To mCore.nLivingGroups
                    For iNatMortName = 1 To NatMortName.Length()
                        If mCore.EcoPathGroupOutputs(iGrp).Name = NatMortName(xgrp) Then
                            xgrp = iNatMortName - 1
                            Exit For
                        End If
                    Next
                    If FTargetandConservation(iGrp - 1, HCRType.Target) <> NoHCR_F Then
                        FTargetandConservation(iGrp - 1, HCRType.Target) = (1 - Math.Exp(-FTargetandConservation(iGrp - 1, HCRType.Target) - NatMort(xgrp))) * (FTargetandConservation(iGrp - 1, HCRType.Target) / (FTargetandConservation(iGrp - 1, HCRType.Target) + NatMort(xgrp))) ' This is correct if we need to convert from instantaneous to yearly catch/biomass
                    End If
                    If FTargetandConservation(iGrp - 1, HCRType.Conservation) <> NoHCR_F Then
                        FTargetandConservation(iGrp - 1, HCRType.Conservation) = (1 - Math.Exp(-FTargetandConservation(iGrp - 1, HCRType.Conservation) - NatMort(xgrp))) * (FTargetandConservation(iGrp - 1, HCRType.Conservation) / (FTargetandConservation(iGrp - 1, HCRType.Conservation) + NatMort(xgrp))) ' This is correct if we need to convert from instantaneous to yearly catch/biomass
                    End If

                Next

                'Compiles a list of which fleets are affecting groups which have a zero conservation f
                'ZeroEffortFleetsList = DetermineZeroEffortFleets(FTargetandConservation)

                'Checks to see whether each fleet effects a group that has an HCR - if yes it adds it to fleets2fit so that we know what fleets to
                'change the effort for to try and achieve the target F's for each group
                For iFleet As Integer = 1 To mCore.nFleets
                    For Each iHCRGroup In CurrentStrategy.HCRules
                        If mCore.FleetInputs(iFleet).Landings(iHCRGroup.GroupNumber4F) + mCore.FleetInputs(iFleet).Discards(iHCRGroup.GroupNumber4F) > 0 Then
                            'If Not Fleets2Fit.Contains(iFleet) And Not ZeroEffortFleetsList.Contains(iFleet) Then
                            If Not Fleets2Fit.Contains(iFleet) Then
                                Fleets2Fit.Add(iFleet)
                            End If
                            Exit For
                        End If
                    Next
                Next

                'if there are no fleets to optimise for skip all this
                If Fleets2Fit.Count > 0 Then

                    'Not quite sure what QMult is but it is needed to calculate what F is in the optimised routine
                    For indexgrp As Integer = 1 To _ecosim.EcosimData.nGroups
                        QMult(indexgrp - 1) = _ecosim.EcosimData.QmQo(indexgrp) / (1 + (_ecosim.EcosimData.QmQo(indexgrp) - 1) * BiomassAtTimestep(indexgrp) / _ecosim.EcosimData.StartBiomass(indexgrp))
                    Next

                    'All the variables that are used by the optimisation routine
                    'Dim x(Fleets2Fit.Count - 1) As Double
                    'Dim epsg As Double = 0.0000000001
                    'Dim epsf As Double = 0
                    'Dim epsx As Double = 0
                    'Dim diffstep As Double = 0.000001
                    'Dim maxits As Integer = 0
                    'Dim state As minlbfgsstate = New XAlglib.minlbfgsstate() ' initializer can be dropped, but compiler will issue warning
                    'Dim rep As minlbfgsreport = New XAlglib.minlbfgsreport() ' initializer can be dropped, but compiler will issue warning
                    'Dim parameters = New List(Of Object)
                    'Dim AllEffortsAboveZero As Boolean
                    'Dim BestCost As Double = 0 'this holds the cost value of best fit so far for comparison with current iteration
                    'Dim BestEfforts(Fleets2Fit.Count - 1) As Double
                    ' if cost improves on current iteration replace all the saved effort values with current effort values



                    'New Linear programming method - ideally needs placing in its own sub

                    Dim lpsolve As New cLPSolver.lpsolve55
                    Dim lp As Integer
                    Dim solution_return_value As Integer

                    'Count how many groups we are setting target F's for
                    Dim countGroupsFTarget As Integer = 0
                    Dim countGroupsFConservation As Integer = 0
                    For i = 1 To mCore.nGroups
                        If FTargetandConservation(i - 1, HCRType.Target) <> NoHCR_F Then
                            countGroupsFTarget += 1
                        End If
                        If FTargetandConservation(i - 1, HCRType.Conservation) <> NoHCR_F Then
                            countGroupsFConservation += 1
                        End If
                    Next

                    cLPSolver.lpsolve55.Init()

                    lp = cLPSolver.lpsolve55.make_lp(0, mCore.nFleets + countGroupsFTarget * 2 + countGroupsFConservation * 2)
                    cLPSolver.lpsolve55.set_outputfile(lp, CurDir() & "\result_lin_prog_MSE" & iTime & ".txt")

                    cLPSolver.lpsolve55.set_timeout(lp, 0)

                    'Set up the factors to go with the effort of each fleet to work out what the fishing mortality is
                    Dim Constraints(mCore.nFleets + countGroupsFTarget * 2 + countGroupsFConservation * 2) As Double
                    Dim indexGroupConstraint = 0
                    Dim ObjectiveFunction(mCore.nFleets + countGroupsFTarget * 2 + countGroupsFConservation * 2) As Double
                    'Create constraints for f_targets
                    For iGrp = 1 To mCore.nGroups
                        If FTargetandConservation(iGrp - 1, HCRType.Target) <> NoHCR_F Then
                            Array.Clear(Constraints, 0, Constraints.Length)
                            For iFleet = 1 To mCore.nFleets
                                Constraints(iFleet) = TechnologyCreep(iFleet) * _ecosim.EcosimData.FishMGear(iFleet, iGrp) * (_ecosim.EcosimData.PropLandedTime(iFleet, iGrp) + _ecosim.EcosimData.Propdiscardtime(iFleet, iGrp)) * QMult(iGrp - 1)
                            Next
                            If Constraints(1) = 0 Then
                                Console.WriteLine("constraint(1)=0")
                            End If
                            Constraints(mCore.nFleets + indexGroupConstraint * 2 + 1) = 1
                            Constraints(mCore.nFleets + indexGroupConstraint * 2 + 2) = -1
                            indexGroupConstraint += 1
                            cLPSolver.lpsolve55.print_str(lp, "Contraint for group: " & iGrp)
                            cLPSolver.lpsolve55.add_constraint(lp, Constraints, cLPSolver.lpsolve55.lpsolve_constr_types.EQ, FTargetandConservation(iGrp - 1, HCRType.Target))
                            'cLPSolver.lpsolve55.print_lp(lp)
                        End If

                    Next
                    'Create constriants for f_conservations
                    indexGroupConstraint = 0
                    For iGrp = 1 To mCore.nGroups
                        If FTargetandConservation(iGrp - 1, HCRType.Conservation) <> NoHCR_F Then
                            Array.Clear(Constraints, 0, Constraints.Length)
                            For iFleet = 1 To mCore.nFleets
                                Constraints(iFleet) = TechnologyCreep(iFleet) * _ecosim.EcosimData.FishMGear(iFleet, iGrp) * (_ecosim.EcosimData.PropLandedTime(iFleet, iGrp) + _ecosim.EcosimData.Propdiscardtime(iFleet, iGrp)) * QMult(iGrp - 1)
                            Next
                            'If iTime = 217 Then
                            '    Console.WriteLine("The group is : " & iGrp)
                            '    Stop
                            'End If
                            Constraints(mCore.nFleets + countGroupsFTarget * 2 + indexGroupConstraint * 2 + 1) = 1000
                            Constraints(mCore.nFleets + countGroupsFTarget * 2 + indexGroupConstraint * 2 + 2) = -1000
                            indexGroupConstraint += 1
                            cLPSolver.lpsolve55.print_str(lp, "Contraint for group: " & iGrp)
                            cLPSolver.lpsolve55.add_constraint(lp, Constraints, cLPSolver.lpsolve55.lpsolve_constr_types.LE, FTargetandConservation(iGrp - 1, HCRType.Conservation))
                        End If
                    Next

                    'set up the constraints so that change in effort cannot be > some limit
                    For Each iFleet In Fleets2Fit
                        Array.Clear(Constraints, 0, Constraints.Length)
                        Constraints(iFleet) = 1
                        If _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1) = 0 Then
                            cLPSolver.lpsolve55.add_constraint(lp, Constraints, cLPSolver.lpsolve55.lpsolve_constr_types.GE, 0)
                        Else
                            cLPSolver.lpsolve55.add_constraint(lp, Constraints, cLPSolver.lpsolve55.lpsolve_constr_types.GE, _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1) - ChangeInEffortLimits(iFleet - 1) * _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1))
                        End If

                    Next

                    'These are all the constants that are added to the end of each simulataneous equation which allows for error between
                    'the actual and target f's - these are to be minimised
                    For i = 1 To countGroupsFTarget
                        ObjectiveFunction(mCore.nFleets + (i - 1) * 2 + 1) = 1
                        ObjectiveFunction(mCore.nFleets + (i - 1) * 2 + 2) = 1
                    Next
                    For i = 1 To countGroupsFConservation
                        ObjectiveFunction(mCore.nFleets + countGroupsFTarget * 2 + (i - 1) * 2 + 1) = 1
                        ObjectiveFunction(mCore.nFleets + countGroupsFTarget * 2 + (i - 1) * 2 + 2) = 1
                    Next

                    cLPSolver.lpsolve55.set_obj_fn(lp, ObjectiveFunction)
                    cLPSolver.lpsolve55.print_lp(lp)
                    cLPSolver.lpsolve55.set_minim(lp)

                    solution_return_value = cLPSolver.lpsolve55.solve(lp)
                    If solution_return_value <> 0 Then MsgBox("Linear Programming solution not optimal. LP Solve code:" & solution_return_value)
                    Console.WriteLine("solution return value = " & solution_return_value)

                    cLPSolver.lpsolve55.print_str(lp, solution_return_value & ": " & cLPSolver.lpsolve55.get_objective(lp) & vbLf)

                    cLPSolver.lpsolve55.print_objective(lp)
                    cLPSolver.lpsolve55.print_solution(lp, 1)
                    cLPSolver.lpsolve55.print_constraints(lp, 1)

                    ReDim variable_results(cLPSolver.lpsolve55.get_Ncolumns(lp))
                    cLPSolver.lpsolve55.get_variables(lp, variable_results)

                    'If Not cLPSolver.lpsolve55.is_feasible(lp, variable_results, 0) Then
                    '    Console.WriteLine()
                    '    MsgBox("Timestep:" & iTime & "There was no possible solution of efforts within the ranges specified that could produce the fishing mortalities desired." & vbCrLf & "One possible solution could be to increase the range that the efforts can vary", MsgBoxStyle.Exclamation)
                    '    If iTime = 229 Then Console.WriteLine("Failed")
                    'End If
                    'If iTime = 229 Then
                    '    For i = 1 To cLPSolver.lpsolve55.get_Ncolumns(lp)
                    '        Console.Write(variable_results(i) & vbTab)
                    '    Next
                    'End If

                    'Set the fishing effort according to what the optimised efforts were just calculated
                    For iFleet = 1 To mCore.nFleets
                        For iMonth = 1 To 12
                            If Fleets2Fit.IndexOf(iFleet) = -1 Then 'If fleet doesnt effect a group that has a HCR set effort to what it was end of last year
                                _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1)
                            Else
                                _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = variable_results(iFleet - 1)
                            End If
                        Next
                    Next

                    'Calc the squared error between fishing mortalities and 

                    'Set up all the variables that we want to send to be used inside optisation routine
                    'they need to be sent as a single list of objects, but can be set back to individually named variables once into optimised routine
                    'parameters.Add(FTargetandConservation)
                    'parameters.Add(Fleets2Fit)
                    'parameters.Add(QMult)
                    'parameters.Add(TechnologyCreep)
                    'parameters.Add(iTime)

                    'Use XAglib to optimise Effort2ProduceF subroutine
                    'For iIteration = 1 To Convert.ToInt32(MSEForm.txtOptimIterations.Text)
                    '    AllEffortsAboveZero = False
                    '    'Use XAglib to optimise Effort2ProduceF subroutine
                    '    Do Until AllEffortsAboveZero = True

                    '        'Initialise the starting point for Effort
                    '        For iFleet = 1 To Fleets2Fit.Count
                    '            x(iFleet - 1) = Rnd() * 4
                    '        Next
                    '        XAlglib.minlbfgscreatef(3, x, diffstep, state)
                    '        XAlglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits)
                    '        XAlglib.minlbfgsoptimize(state, AddressOf Effort2produceF, Nothing, parameters)
                    '        XAlglib.minlbfgsresults(state, x, rep)
                    '        'Console.WriteLine("Starting Point = " & iIteration & "; Timestep = " & iTime)
                    '        AllEffortsAboveZero = True
                    '        For i = 1 To x.Length
                    '            If x(i - 1) < 0 Then AllEffortsAboveZero = False
                    '        Next
                    '    Loop
                    '    Console.Write(state.csobj.f)
                    '    For iEffort = 1 To x.GetLength(0)
                    '        Console.Write(vbTab & x(iEffort - 1))
                    '    Next
                    '    Console.WriteLine()

                    '    If state.csobj.f < BestCost Then
                    '        BestCost = state.csobj.f
                    '        For i = 1 To Fleets2Fit.Count
                    '            BestEfforts(i - 1) = x(i - 1)
                    '        Next
                    '    End If

                    'Next

                    'Set the fishing effort according to what the optimised efforts were just calculated
                    'For iFleet = 1 To mCore.nFleets
                    '    For iMonth = 1 To 12
                    '        If Fleets2Fit.IndexOf(iFleet) = -1 Then 'If fleet doesnt effect a group that has a HCR set effort to what it was end of last year
                    '            _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1)
                    '        Else
                    '            If BestEfforts(Fleets2Fit.IndexOf(iFleet)) < 0 Then 'If the optimised effort is < 0 (which should only ever be slight below 0 because
                    '                'of penalty for values<0) then set effort to 0
                    '                _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = 0
                    '            Else
                    '                _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = BestEfforts(Fleets2Fit.IndexOf(iFleet)) 'Set effort to optimised
                    '            End If
                    '        End If
                    '    Next
                    'Next


                End If

                'Set the efforts of all fleets which can a conservation species with an F=0 to zero
                'For Each iFleet In ZeroEffortFleetsList
                '    For iMonth = 1 To 12
                '        _ecosim.EcosimData.FishRateGear(iFleet, iTime - 1 + iMonth) = 0
                '    Next
                'Next

                'Calculates what the F's are for each species given the effort
                For iMonth = 0 To 11
                    _ecosim.SetFtimeFromGear(Nothing, iTime + iMonth, TechnologyCreep, True)
                Next


            End If
        End If
    End Sub

    Private Sub Effort2produceF(ByVal x As Double(), ByRef func As Double, ByVal obj As List(Of Object))

        Dim FTargetandConservation As Double(,) = obj(0)
        Dim fleets2fit As List(Of Integer) = obj(1)
        Dim QMult As Double() = obj(2)
        Dim TechnologyCreep As Single() = obj(3)
        Dim iTime As Integer = obj(4)
        Dim GroupF(mCore.nGroups - 1) As Double
        Const Penalty As Double = 1000000000
        Const PenaltyEBelowZero As Double = Penalty ' this is a constant that multiplies by the penalty function
        Const PenaltyFaboveTarget As Double = Penalty
        Const PenaltyFOutsideLimitChange As Double = Penalty
        Dim IndexInX_Effort As Integer

        func = 0 ' func is the cost function. Initialise by setting to 0

        'Initialise Calculated F's by setting to zero
        For igrp = 1 To mCore.nLivingGroups
            GroupF(igrp - 1) = 0
        Next

        'Calculate what F's the specified effort (x) will produce
        For Each iFleet2Fit In fleets2fit
            IndexInX_Effort = fleets2fit.IndexOf(iFleet2Fit)
            For iGrp = 1 To mCore.nGroups
                GroupF(iGrp - 1) += TechnologyCreep(iFleet2Fit - 1) * _ecosim.EcosimData.FishMGear(iFleet2Fit, iGrp) * x(IndexInX_Effort) * (_ecosim.EcosimData.PropLandedTime(iFleet2Fit, iGrp) + _ecosim.EcosimData.Propdiscardtime(iFleet2Fit, iGrp)) * QMult(iGrp - 1)
            Next
            If x(IndexInX_Effort) < 0 Then func += PenaltyEBelowZero * Math.Abs(x(IndexInX_Effort)) ^ 2
            If Math.Abs(x(IndexInX_Effort) - _ecosim.EcosimData.FishRateGear(iFleet2Fit, iTime - 1)) > ChangeInEffortLimits(iFleet2Fit - 1) Then func += PenaltyFOutsideLimitChange * (x(IndexInX_Effort) - _ecosim.EcosimData.FishRateGear(iFleet2Fit, iTime - 1)) ^ 2
        Next

        'Compare the produced F's with those that we are trying to obtain and calculated the cost function accordingly
        For iGrp As Integer = 1 To mCore.nLivingGroups
            'apply the different possible cost function to each group given in the HCR

            If FTargetandConservation(iGrp - 1, HCRType.Target) <> NoHCR_F And FTargetandConservation(iGrp - 1, HCRType.Conservation) = NoHCR_F Then
                'Values in left column only of FTargetandConservation mean target F only
                func += (FTargetandConservation(iGrp - 1, HCRType.Target) - GroupF(iGrp - 1)) ^ 2
            ElseIf FTargetandConservation(iGrp - 1, HCRType.Target) = NoHCR_F And FTargetandConservation(iGrp - 1, HCRType.Conservation) <> NoHCR_F Then
                'Values in right column only of FTargetandConservation means conservation F only
                If GroupF(iGrp - 1) > FTargetandConservation(iGrp - 1, HCRType.Conservation) Then func += PenaltyFaboveTarget * (GroupF(iGrp - 1) - FTargetandConservation(iGrp - 1, HCRType.Conservation)) ^ 2
            ElseIf FTargetandConservation(iGrp - 1, HCRType.Target) <> NoHCR_F And FTargetandConservation(iGrp - 1, HCRType.Conservation) <> NoHCR_F Then
                'Values in both columns of FTargetandConservation means both target and conservation
                func += (FTargetandConservation(iGrp - 1, HCRType.Target) - GroupF(iGrp - 1)) ^ 2
                If GroupF(iGrp - 1) > FTargetandConservation(iGrp - 1, HCRType.Conservation) Then func += PenaltyFaboveTarget * (GroupF(iGrp - 1) - FTargetandConservation(iGrp - 1, HCRType.Conservation)) ^ 2
            End If

        Next

    End Sub

    Friend ReadOnly Property Core As cCore
        Get
            Return Me.mCore
        End Get
    End Property

    'Private Sub CalculateFError(ByRef eps() As Double)
    '    Dim Fopt(mCore.nGroups - 1) As Double
    '    For iGrp As Integer = 1 To mCore.nLivingGroups
    '        Fopt(iGrp - 1) = 0
    '        For iFleet As Integer 1 to mCore.nFleets
    '            Fopt(iGrp - 1) = Fopt(iGrp - 1) + (mCore.FleetInputs(iFleet).Landings(iGrp) + mCore.FleetInputs(iFleet).Discards(iGrp)) * eps(iGrp - 1)
    '        Next
    '    Next
    'End Sub

End Class
