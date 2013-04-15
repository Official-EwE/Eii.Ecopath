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
