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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwECore

''' <summary>
''' This code provides an example how to load an EwE model, run Ecopath, and extract a few computed results.
''' </summary>
Module EcopathOutputs

    Sub Main()

        Dim core As New cCore()
        Dim bModelBalanced As Boolean = False

        ' Able to load model?
        If core.LoadModel("Tampa_Bay.EwEmdb") Then
            Console.WriteLine("Model loaded")

            ' Able to run Ecopath?
            If core.RunEcoPath(bModelBalanced) Then
                Console.WriteLine("Ecopath ran successfully")

                If bModelBalanced Then
                    Console.WriteLine("Ecopath balanced")
                Else
                    Console.WriteLine("Ecopath did not balance")
                End If
            Else
                Console.WriteLine("Ecopath did not run successfully")
            End If

            ' Write values that have been computed by Ecopath
            For i As Integer = 1 To core.nGroups

                ' Obtain Ecopath output information for a single group from the core
                Dim group As cEcoPathGroupOutput = core.EcoPathGroupOutputs(i)
                Console.WriteLine("Group " & i & ": B = " & group.Biomass & ", EE = " & group.EEOutput)

                ' Note that .NET provides many ways to make outputs look pretty. 
                ' Feel free to comment-out the versions below to see what they do:

                ' 1. The same as above, just more compactly written
                'Console.WriteLine("Group {0}: B = {1}, EE = {2}", i, group.Biomass, group.EEOutput)

                ' 2. Again the same, but with additional formatting to make results look more neat:
                '     Value 0, group number, is written to two characters;
                '     Value 1, computed group Biomass, is written to 9 characters with 5 decimals;
                '     Value 2, computed group EE, is also written to 9 characters with 7 decimals;
                'Console.WriteLine("Group {0,2}: B = {1,9:F5}, EE = {2,9:F7}", i, group.Biomass, group.EEOutput)

            Next

            core.CloseModel()
        Else
            Console.WriteLine("Model did not load")
        End If

        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

    End Sub

End Module
