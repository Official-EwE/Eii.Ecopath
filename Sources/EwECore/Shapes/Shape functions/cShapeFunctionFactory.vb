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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Reflection
Imports EwEPlugin
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Base class for implementing EwE core shape functions.
''' </summary>
Public Class cShapeFunctionFactory

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="shape"></param>
    ''' <param name="pm">Ignored for now.</param>
    ''' <returns></returns>
    Public Shared Function GetShapeFunctions(ByVal shape As cForcingFunction, _
                                             Optional ByVal pm As cPluginManager = Nothing) As IShapeFunction()

        Dim lfs As New List(Of IShapeFunction)
        Dim fs As IShapeFunction = Nothing

        ' Get all shape functions provided by the core
        For Each c As Type In Assembly.GetAssembly(GetType(cCore)).GetTypes()
            If (c.IsPublic) And (Not c.IsAbstract) And (GetType(cShapeFunction).IsAssignableFrom(c)) Then
                Dim bCompatible As Boolean = False

                Try
                    fs = CType(Activator.CreateInstance(c), IShapeFunction)
                    If (shape Is Nothing) Then
                        bCompatible = True
                    Else
                        bCompatible = (fs.IsCompatible(shape.DataType))
                    End If

                    If (bCompatible) Then
                        fs.Init(shape)
                        lfs.Add(fs)
                    End If
                Catch ex As Exception
                    Debug.Assert(False, ex.Message)
                    cLog.Write(ex, "cShapeFunctionFactory.GetShapeFunctions(" & c.ToString & ")")
                End Try

            End If
        Next

        If (pm IsNot Nothing) Then
            ' Get all shape functions provided as plug-ins
            For Each c As IPlugin In pm.GetPlugins(GetType(IEcosimShapeFunctionPlugin))
                Dim bCompatible As Boolean = False
                fs = CType(c, IShapeFunction)

                If (shape Is Nothing) Then
                    bCompatible = True
                Else
                    bCompatible = (fs.IsCompatible(shape.DataType))
                End If

                If (bCompatible) Then
                    fs.Init(shape)
                    lfs.Add(fs)
                End If
            Next
        End If

        Return lfs.ToArray()
    End Function

End Class
