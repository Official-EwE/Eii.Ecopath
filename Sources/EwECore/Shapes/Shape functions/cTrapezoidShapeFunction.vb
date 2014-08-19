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

#End Region ' Imports

Public Class cTrapezoidShapeFunction
    Inherits cShapeFunction

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function Shape(Optional nPoints As Integer = 1200) As Single()
        If (Me.ParamsChanged) Then
            Me.ParamsChanged = False
        End If
        Return Me.m_parameters
    End Function

    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 1.0F
        Me.ParamValue(2) = 2.0F
        Me.ParamValue(3) = 3.0F
        Me.ParamValue(4) = 4.0F
    End Sub

    Public Overrides Function IsRelevantDataType(datatype As eDataTypes) As Boolean
        Return (datatype = EwEUtils.Core.eDataTypes.Mediation) Or _
               (datatype = EwEUtils.Core.eDataTypes.PriceMediation)
    End Function

    Public Overrides Property ParamValue(ByVal iParam As Integer) As Single
        Get
            Return MyBase.ParamValue(iParam)
        End Get
        Set(value As Single)

            Dim a0 As Single = Me.ParamValue(1)
            Dim b0 As Single = Me.ParamValue(2)
            Dim c0 As Single = Me.ParamValue(3)
            Dim d0 As Single = Me.ParamValue(4)
            Dim shift As Single

            MyBase.ParamValue(iParam) = value

            Dim a1 As Single = Me.ParamValue(1)
            Dim b1 As Single = Me.ParamValue(2)
            Dim c1 As Single = Me.ParamValue(3)
            Dim d1 As Single = Me.ParamValue(4)

            ' JS ported from JoeB's logic in dlgChangeShape

            'This only sort of works
            'The idea is to translate the object
            'if one of the points is to far to the right.
            'Because we don't know the point positions before the edit 
            'we can't figure out the shift for the translate
            'So just fake it...
            If (a1 > b1) Then
                shift = a1 - a0
                MyBase.ParamValue(2) += shift
                MyBase.ParamValue(3) += shift
                MyBase.ParamValue(4) += shift
            ElseIf (b1 > c1) Then
                shift = b1 - b0
                MyBase.ParamValue(3) += shift
                MyBase.ParamValue(4) += shift
            ElseIf (c1 > d1) Then
                shift = c1 - c0
                MyBase.ParamValue(4) += shift
            End If

        End Set
    End Property

    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            ' ToDo: globalize this
            Select Case iParam
                Case 1 : Return "Left bottom"
                Case 2 : Return "Left top"
                Case 3 : Return "Right top"
                Case 4 : Return "Right bottom"
            End Select
            Return "?"
        End Get
    End Property

    Public Overrides ReadOnly Property nParameters As Integer
        Get
            Return 4
        End Get
    End Property

    Public Overrides ReadOnly Property ShapeFunctionType As EwECore.eShapeFunctionType
        Get
            Return eShapeFunctionType.Trapezoid
        End Get
    End Property

End Class
