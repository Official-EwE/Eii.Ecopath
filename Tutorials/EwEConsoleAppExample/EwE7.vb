' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

' Required external libraries
Imports EwECore
Imports EwECore.DataSources
Imports EwEUtils.Core

Module EwE7

    ' Main Sub routine called when program is run.
    Sub Main()

        ' 1) Create a core to work with in this application
        Dim core As New cCore()
        ' 2) Tell the core to initialize - this is mandatory
        core.InitCore()

        ' 3) Create a datasource that can work with Access 2007 databases, which is what EwE uses by default
        '    Note that the Access 2007 database drivers need to be installed for this code to work
        '    The EwE technical FAQ explain how to get these drivers (http://sources.ecopath.org/trac/Ecopath/wiki/TechnicalFAQ)
        Dim ds As IEwEDataSource = cDataSourceFactory.Create(eDataSourceTypes.Access2007)
        ' 4) Tell the datasource to connect the core to an ecopath model, in this case 'baltic.ewemdb'
        '    Note that the Open function returns whether the model could be connected to. We really
        '    should be testing whether the Open function was successful before plowing on. This sample
        '    omits such tests to focus on the bare flow of core interactions.
        ds.Open("baltic.ewemdb", core)

        ' 5) Start using the connected datasource: load the Ecopath model
        '    Note that cCore.LoadModel also provides success feedback which should be tested too.
        core.LoadModel(ds)
        ' 6) Tell the core to run Ecopath
        '    Note that cCore.RunEcopath returns True if the model balances
        core.RunEcoPath()

        ' 7) Dump out some results of the model to illustrate that it worked.
        For igrp As Integer = 1 To core.nGroups
            Console.WriteLine("Group '" & core.EcoPathGroupOutputs(igrp).Name & "'" & _
                              " EE estimated to " & core.EcoPathGroupOutputs(igrp).EEOutput)
        Next igrp

        ' 8) Tell the core to close the model
        core.CloseModel()

        ' 9) We're done: tell the user to press a key
        Console.WriteLine("Press a key to exit")
        ' .. and wait for the key press before terminating
        Console.ReadKey()

    End Sub

End Module
