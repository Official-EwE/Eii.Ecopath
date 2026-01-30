' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Strict Off
Imports System.Reflection
Imports EwEUtils.Utilities



Namespace Style

    Public Class cTypeFormatterFactory

        ''' <summary>
        ''' Factory method, explores the EwEUtils assembly for ITypeFormatter-derived
        ''' classes and returns a type formatter instance if this type implements
        ''' formatting of an indicated type.
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Public Shared Function GetTypeFormatter(t As Type) As ITypeFormatter

            ' For Each ass As Assembly In AppDomain.CurrentDomain.GetAssemblies()
            Dim ass As Assembly = Assembly.GetAssembly(GetType(ITypeFormatter))
            Dim tfm As Type = GetType(ITypeFormatter)

            For Each tTest As Type In ass.GetTypes
                If tfm.IsAssignableFrom(tTest) And tTest.GetConstructor(Type.EmptyTypes) IsNot Nothing Then
                    Dim f As ITypeFormatter = DirectCast(Activator.CreateInstance(tTest), ITypeFormatter)
                    If f.GetDescribedType.IsAssignableFrom(t) Then
                        Return f
                    End If
                End If
            Next
            'Next
            Return Nothing

        End Function

    End Class

End Namespace
