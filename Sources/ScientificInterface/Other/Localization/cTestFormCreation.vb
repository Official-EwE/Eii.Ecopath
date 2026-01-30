' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Reflection



#If DEBUG Then

Namespace Other

    ''' <summary>
    ''' Quick and dirty utility that tests if all EwE ScInt classes are ready for LSA Creator
    ''' </summary>
    Public Class cTestFormCreation

        Public Sub New()

            ' --- TEST 1: test if all forms have a parameterless constructor ---
            Dim ass As Assembly = Assembly.GetExecutingAssembly()
            Dim tf As Type = GetType(Form)
            For Each t As Type In ass.GetTypes()
                If (tf.IsAssignableFrom(t)) And Not t.IsAbstract Then
                    Try
                        'Fire off default constructur and see what happens. LSA Creator neeeds this to work
                        Dim f As Form = DirectCast(Activator.CreateInstance(t), Form)
                        f.Dispose()
                    Catch ex As Exception

                    End Try
                End If
            Next

        End Sub

    End Class

End Namespace

#End If
