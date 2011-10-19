' Required external libraries
Imports EwECore
Imports EwECore.DataSources
Imports EwEUtils
Imports EwEUtils.Core

Module EwE7

    ' Main Sub routine called when program is run.
    Sub Main()
        Dim core As New cCore()                     ' (1) Define core and database variables
        Dim ds As IEwEDataSource = cDataSourceFactory.Create(eDataSourceTypes.Access2007)

        ds.Open("baltic.ewemdb", core)              ' (2) Tell the datasource to open a database called baltic.ewemdb 

        core.InitCore()                             ' (3) Tell the core to initialize 
        core.LoadModel(ds)                          ' (4) Tell the core to load a model from the datasource
        core.RunEcoPath()                           ' (4) Tell the core to run Ecopath

        ' (5) Write to the console the group name and Ecopath EE for all groups
        For igrp As Integer = 1 To core.nGroups
            Console.WriteLine("Group '" & core.EcoPathGroupOutputs(igrp).Name & "'" & _
                              " EE estimated to " & core.EcoPathGroupOutputs(igrp).EEOutput)

        Next igrp

        core.CloseModel()                           ' (6) Tell the core to close the Baltic model

        Console.WriteLine("Press a key to exit")    ' Tell the user to press a key
        Console.ReadKey()                           ' Wait for the key press
    End Sub

End Module
