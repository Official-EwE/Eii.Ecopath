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
' Copyright 1991-2013 UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'
Imports EwECore
Imports EwEPlugin
Imports System

Module ConsolePluginApp

    Sub Main()

        Dim core As New cCore()
        core.PluginManager = New cPluginManager()
        core.PluginManager.LoadPlugins()

        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

    End Sub

End Module
