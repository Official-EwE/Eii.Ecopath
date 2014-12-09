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

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Shape"/>
    ''' <summary>
    ''' Returns the points for a trapezoid shape.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Shape(ByVal nPoints As Integer) As Single()

        If (Me.ParamsChanged) Then
            Dim sYZero As Single = Me.ParamValue(1)
            Dim sYEnd As Single = Me.ParamValue(2)
            Dim sYBase As Single = Me.ParamValue(3)
            Dim sSteep As Single = Me.ParamValue(4)
            Dim xpt As Single
            Dim width As Single = sSteep
            Dim x0 As Single = 0
            If sYZero < 0 Then
                x0 = sYZero
                width = sSteep - sYZero
            End If

            Dim dx As Single = width / nPoints

            If sYBase = 0 Then sYBase = 1
            If sYZero > sYEnd Then sYEnd = sYZero
            If sYBase < sYZero Or sYBase < sYEnd Then sYBase = sYEnd + 1

            Dim yVal() As Single = New Single() {0, 0, 1, 1, 0, 0}
            Dim xVal() As Single = New Single() {x0, sYZero, sYEnd, sYBase, sSteep, width}

            'Break the line up into segments based on the xpoints the user entered
            'The location of the shoulder in the response function is determined by it's index position in the points array
            Dim iSegment() As Integer = New Integer() {0, Me.getIndex(sYZero, x0, sSteep, nPoints), Me.getIndex(sYEnd, x0, sSteep, nPoints), Me.getIndex(sYBase, x0, sSteep, nPoints), Me.getIndex(sSteep, x0, sSteep, nPoints), nPoints}

            '' JS 160914: This is not right; the original shape cannot be modified until the user clicks 'OK'
            'Dim shape As cEnviroResponseFunction = TryCast(Me.m_shape, cEnviroResponseFunction)
            'If Shape IsNot Nothing Then
            '    'set the extent of the data in the shape
            '    Shape.ResponseLeftLimit = x0
            '    Shape.ResponseRightLimit = sSteep
            'End If

            'loop over the segments and interpolate the points on the line
            For i As Integer = 0 To 4
                xpt = xVal(i)
                'loop from the start to the end position in this segment
                'and interpolate the y point on the line
                For j As Integer = iSegment(i) To iSegment(i + 1)
                    Me.m_points(j) = Me.LinearInterp(xpt, xVal(i), xVal(i + 1), yVal(i), yVal(i + 1))
                    xpt += dx
                Next j
            Next i
        End If

        Return MyBase.Shape(nPoints)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 1.0F
        Me.ParamValue(2) = 2.0F
        Me.ParamValue(3) = 3.0F
        Me.ParamValue(4) = 4.0F
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.IsCompatible"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function IsCompatible(datatype As eDataTypes) As Boolean
        Return Me.IsMediation(datatype)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ParamValue"/>
    ''' -----------------------------------------------------------------------
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

            ''This only sort of works
            ''The idea is to translate the object
            ''if one of the points is to far to the right.
            ''Because we don't know the point positions before the edit 
            ''we can't figure out the shift for the translate
            ''So just fake it...
            'If (a1 > b1) Then
            '    shift = a1 - a0
            '    MyBase.ParamValue(2) += shift
            '    MyBase.ParamValue(3) += shift
            '    MyBase.ParamValue(4) += shift
            'ElseIf (b1 > c1) Then
            '    shift = b1 - b0
            '    MyBase.ParamValue(3) += shift
            '    MyBase.ParamValue(4) += shift
            'ElseIf (c1 > d1) Then
            '    shift = c1 - c0
            '    MyBase.ParamValue(4) += shift
            'End If

        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ParamName"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            Select Case iParam
                Case 1 : Return My.Resources.CoreDefaults.PARAM_LEFT_BOTTOM
                Case 2 : Return My.Resources.CoreDefaults.PARAM_LEFT_TOP
                Case 3 : Return My.Resources.CoreDefaults.PARAM_RIGHT_TOP
                Case 4 : Return My.Resources.CoreDefaults.PARAM_RIGHT_BOTTOM
            End Select
            Return "?"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.nParameters"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property nParameters As Integer
        Get
            Return 4
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ShapeFunctionType"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ShapeFunctionType As Long
        Get
            Return eShapeFunctionType.Trapezoid
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Apply"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Apply(obj As Object) As Boolean
        If MyBase.Apply(obj) Then
            Dim shape As cEnviroResponseFunction = TryCast(obj, cEnviroResponseFunction)
            If shape IsNot Nothing Then
                'set the extent of the data in the shape
                Dim left As Single = 0
                If Me.ParamValue(1) < 0 Then left = Me.ParamValue(1)
                shape.ResponseLeftLimit = left
                shape.ResponseRightLimit = Me.ParamValue(4)
            End If

        End If
    End Function

#Region " Internals "

    Private Function getIndex(Xvalue As Single, x0 As Single, x1 As Single, TotalNPoints As Integer) As Integer
        'Debug.Assert(Xvalue >= x0 And Xvalue <= x1, Me.ToString + ".getIndex() value out of bounds.")
        'use the linear interpolator to find the index positon of Value
        'In this case we are interpolating the number of data points Xvalue is along the line
        'x0 and x1 are the first and last values of the x axis
        '0 and TotalNPoints are the number of data points/array indexes
        Return CInt(LinearInterp(Xvalue, x0, x1, 0, TotalNPoints))
    End Function

    Private Function LinearInterp(ByVal x As Single, x0 As Single, x1 As Single, y0 As Single, y1 As Single) As Single
        If ((x1 - x0) = 0) Then
            'mid point on the y axis
            Return (y0 + y1) / 2.0F
        Else
            Return y0 + (y1 - y0) * ((x - x0) / (x1 - x0))
        End If
    End Function

#End Region ' Internals 

End Class
