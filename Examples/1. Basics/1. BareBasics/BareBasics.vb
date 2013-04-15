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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports EwECore

''' <summary>
''' Bare basics program that loads an EwE model and checks whether Ecopath balances.
''' </summary>
Module BareBasics

    Sub Main()

        ' Create a new core
        Dim core As New cCore()

        ' Can we load a model into the core?
        If core.LoadModel("Tampa_Bay.EwEmdb") Then
            Console.WriteLine("Model loaded")

            ' Does Ecopath balance?
            If core.RunEcoPath() Then
                Console.WriteLine("Ecopath balanced")
            Else
                Console.WriteLine("Ecopath did not balance")
            End If

            ' Done
            core.CloseModel()
        Else
            Console.WriteLine("Model did not load")
        End If

        ' Wait for the user to press a key before closing this program
        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

    End Sub

End Module
