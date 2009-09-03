Option Strict On
Imports EwECore
Imports EwECore.DataSources
Imports System.Windows.Forms

Module CoreEcosimTutorial

    Sub Main()
        Dim core As cCore = cCore.GetInstance
        Dim datasource As IEwEDataSource = Nothing

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Code to create a data source and load a model into the core

        ' Use the data source factory to create a data source that can read an Access file (.mdb)
        datasource = cDataSourceFactory.Create(EwEUtils.Core.eDataSourceTypes.MDB)
        If (datasource Is Nothing) Then
            'cDataSourceFactory failed to create a data source
            Return
        End If

        Dim fd As New OpenFileDialog()
        If fd.ShowDialog() = DialogResult.OK Then
            'open the file with the data source
            If datasource.Open(fd.FileName, core) <> EwEUtils.Core.eDatasourceAccessType.Opened Then
                'the file did not contain a valid EwE6 model
                Return
            End If

            'Now we have a datasource with a valid model
            'next load the model data into the core
            If core.LoadModel(datasource) = False Then
                'the core failed to load the model from the data source
                Return
            End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'LOAD AN ECOSIM SCENARIO 
            Dim scenario As cEcoSimScenario

            'iterate over the scenarios in the current model
            For iscen As Integer = 1 To core.EcosimScenarioCount
                scenario = core.EcosimScenarios(iscen)
                System.Console.WriteLine(scenario.Name)
            Next iscen

            'load the first scenario
            'this will load all the ecosim data associated with this scenario
            If core.LoadEcosimScenario(1) = False Then
                Return
            End If

            Dim EcosimInputs As cEcoSimGroupInput
            Dim maxRelFeeding As Single, grpName As String
            For iGroup As Integer = 1 To core.nGroups
                EcosimInputs = core.EcoSimGroupInputs(iGroup)
                maxRelFeeding = EcosimInputs.MaxRelFeedingTime
                grpName = EcosimInputs.Name

                System.Console.WriteLine(grpName & " Max relative feeding time = " & maxRelFeeding.ToString)
            Next iGroup

            'set the number of years to run the model 
            core.EcoSimModelParameters.NumberYears = 10

            'Run Ecosim with the address of the sub that will recieve the results at each time step
            core.RunEcoSim(AddressOf EcosimResultsHandler)

            System.Console.WriteLine("Press [ENTER] to exit")
            System.Console.ReadLine()

        End If  ' End of open file dialog test. 

    End Sub

    ''' <summary>
    ''' This Sub will be called by the EwECore at each time step of Ecosim with the results of the current time step
    ''' </summary>
    ''' <param name="iTime">Current time step</param>
    ''' <param name="EcoSimResults">Results of the time step</param>
    ''' <remarks>The AddressOf this Sub get passed to Ecosim in the RunEcoSim() method. This tell Ecosim where to send the time step results.</remarks>
    Private Sub EcosimResultsHandler(ByVal iTime As Long, ByVal EcoSimResults As cEcoSimResults)

        System.Console.WriteLine("Time Step = " & iTime.ToString)
        For igroup As Integer = 1 To EcoSimResults.nGroups
            System.Console.Write(EcoSimResults.Biomass(igroup).ToString & ", ")
        Next igroup
        System.Console.Write(vbCrLf)

    End Sub


End Module
