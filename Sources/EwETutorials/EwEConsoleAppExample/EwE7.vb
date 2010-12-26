' Required external libraries
Imports EwECore
Imports EwECore.DataSources
Imports EwEUtils
Imports EwEUtils.Core

Module EwE7

    ' Main routine is called when the program is run.
    Sub Main()

        ' (1) Create a new core
        Dim core As New cCore()
        ' (2) Create a datasource to read 'mdb' formatted data
        Dim ds As IEwEDataSource = cDataSourceFactory.Create(eDataSourceTypes.MDB)

        ' (3) Tell the datasource to open a database called 'baltic.ewemdb'.  
        ds.Open("baltic.ewemdb", core)

        ' (4) Tell the core to initialize itself
        core.InitCore()
        ' (5) Tell the core to load a model from the datasource
        core.LoadModel(ds)
        ' (6) Tell the core to run Ecopath
        core.RunEcoPath()

        ' (7) Write to the console the group name and Ecopath EE for all groups
        For iGroup As Integer = 1 To core.nGroups
            ' (7.1) Get ecopath results group 'iGroup'
            Dim group As cEcoPathGroupOutput = core.EcoPathGroupOutputs(iGroup)
            ' (7.2) Write information for this group to the console
            Console.WriteLine("Group '" & group.Name & "' EE = " & group.EEOutput)
        Next

        ' (8) Tell the core to close the model
        core.CloseModel()

        ' Ask the user to hit a key before closing, and wait for the key press
        Console.WriteLine("Press a key to exit")
        Console.ReadKey()
    End Sub

End Module
