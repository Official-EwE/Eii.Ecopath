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
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.Text

#End Region ' Imports

Public Class cData

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_defs As New List(Of cFunctionDefinition)
    Private m_fns As New Dictionary(Of Long, IShapeFunction)

#End Region ' Private vars

    Public Sub New(core As cCore)
        Me.m_core = core

        Dim fns As IShapeFunction() = cShapeFunctionFactory.GetShapeFunctions(pm:=Me.m_core.PluginManager)
        For Each fn As IShapeFunction In fns
            If (fn.ShapeFunctionType <> eShapeFunctionType.NotSet) Then
                Me.m_fns(fn.ShapeFunctionType) = fn
            End If
        Next
    End Sub

    Public Property Delimiter As Char = ","c
    Public Property DecimalSeparator As Char = "."c
    Public Property DataType As eDataTypes = eDataTypes.Forcing

    Public Function Read(text As System.IO.TextReader) As Boolean

        Me.m_defs.Clear()

        Dim strLine As String = text.ReadLine()
        Dim bSucces As Boolean = True

        ' Skip header line, for now expect fixed order

        strLine = text.ReadLine()
        While Not String.IsNullOrWhiteSpace(strLine)
            Dim bits As String() = cStringUtils.SplitQualified(strLine, Delimiter)
            Dim fn As IShapeFunction = Me.ShapeFunction(Long.Parse(bits(1)))
            If (fn IsNot Nothing) Then
                Dim parms(4) As Single
                For i As Integer = 0 To 4
                    If bits.Length > i + 2 Then
                        parms(i) = cStringUtils.ConvertToSingle(bits(i + 2), 0, Me.DecimalSeparator)
                    Else
                        parms(i) = cCore.NULL_VALUE
                    End If
                Next
                Dim f As New cFunctionDefinition(bits(0), fn, parms(0), parms(1), parms(2), parms(3), parms(4))
                Me.m_defs.Add(f)
            Else
                bSucces = False
            End If
            strLine = text.ReadLine()
        End While

        Return bSucces

    End Function

    Public Function FunctionDefinitions() As cFunctionDefinition()
        Dim lDefs As New List(Of cFunctionDefinition)
        For Each fn As cFunctionDefinition In Me.m_defs
            If fn.ShapeFunction.IsCompatible(Me.DataType) Then
                lDefs.Add(fn)
            End If
        Next
        Return lDefs.ToArray()
    End Function

    Public Function ShapeFunctions() As IEnumerable(Of IShapeFunction)
        Return From fn As IShapeFunction In Me.m_fns.Values Order By fn.ShapeFunctionType
    End Function

#Region " Internals "

    Private Function ShapeFunction(ShapeFunctionType As Long) As IShapeFunction
        If (Me.m_fns.ContainsKey(ShapeFunctionType)) Then
            Return Me.m_fns(ShapeFunctionType)
        End If
        Return Nothing
    End Function

#End Region ' Internals

End Class
