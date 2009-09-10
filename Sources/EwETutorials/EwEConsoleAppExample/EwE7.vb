' Required external libraries
Imports EwECore
Imports EwECore.DataSources
Imports EwEUtils
Imports EwEUtils.Core

Module EwE7

    ' Main Sub routine called when program is run.
    Sub Main()
        Dim core As New cCore()                     ' (1) Define core and database variables
        Dim ds As IEwEDataSource = cDataSourceFactory.Create(eDataSourceTypes.ACCDB)

        ds.Open("baltic.ewemdb", core)              ' (2) Open the database called baltic.ewemdb.  

        core.InitCore()                             ' (3) Tells the core to do some initialization and load the model
        core.LoadModel(ds)
        core.RunEcoPath()                           ' (4) Runs Ecopath

                                                    ' (5) Writes to the console the group name and Ecopath EE for group 1.
        Console.WriteLine("Group '" & core.EcoPathGroupOutputs(1).Name & "'" & _
                          " EE estimated to " & core.EcoPathGroupOutputs(1).EEOutput)
        core.CloseModel()                           ' (6) Tells the model to shutdown properly.

        Console.WriteLine("Press a key to exit")    ' Waits and ask the user to hit a key before closing.
        Console.ReadKey()
    End Sub

End Module
