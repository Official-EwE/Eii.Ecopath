' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



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
