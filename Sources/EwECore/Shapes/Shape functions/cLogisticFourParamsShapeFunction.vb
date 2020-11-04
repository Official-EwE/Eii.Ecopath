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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports


Public Class cLogisticFourParamsShapeFunction
    Inherits cShapeFunction

    Public Sub New()
        MyBase.New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Shape"/>
    ''' <summary>
    ''' Returns the points for a sigmoid shape.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Shape(nPoints As Integer) As Single()

        If (Me.ParamsChanged) Then

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            Dim XMin As Single = Me.ParamValue(1)
            Dim XMax As Single = Me.ParamValue(2)
            Dim Inf As Single = Me.ParamValue(3)
            Dim Slope As Single = Me.ParamValue(4)
            Dim ymin As Single = 0.000000001
            Dim ymax As Single = 1

            Dim dx As Single = (XMax - XMin) / nPoints
            Dim x As Single = XMin
            For i As Integer = 1 To nPoints
                x = (i - 1) * dx
                Me.m_points(i) = CSng(1.0 + (-1.0 / (1.0 + (x / Inf) ^ Slope)))

                System.Console.WriteLine(x.ToString + ", " + Me.m_points(i).ToString)
            Next i
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        End If
        Return MyBase.Shape(nPoints)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 0.0
        Me.ParamValue(2) = 1
        Me.ParamValue(3) = 0.5
        Me.ParamValue(4) = 0.9

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.IsCompatible(eDataTypes)"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function IsCompatible(datatype As eDataTypes) As Boolean
        Return Me.IsForcing(datatype) Or Me.IsMediation(datatype)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.IsDistribution()"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property IsDistribution As Boolean
        Get
            Return True
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
    ''' <inheritdocs cref="cShapeFunction.ParamName"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            Select Case iParam
                Case 1 : Return My.Resources.CoreDefaults.PARAM_LOGISTIC_XMIN' "X min" PARAM_LOGISTIC_XMIN
                Case 2 : Return My.Resources.CoreDefaults.PARAM_LOGISTIC_XMAX 'Return "X max"
                Case 3 : Return My.Resources.CoreDefaults.PARAM_LOGISTIC_INFLECTION'Return "Inflection"
                Case 4 : Return My.Resources.CoreDefaults.PARAM_LOGISTIC_SLOPE 'Return "Slope"
            End Select

            Return MyBase.ParamName(iParam)

        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ShapeFunctionType"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ShapeFunctionType As Long
        Get
            Return eShapeFunctionType.Logistic_4Params
        End Get
    End Property

    Public Overrides Function Apply(obj As Object) As Boolean
        If Not MyBase.Apply(obj) Then
            Return False
        End If
        Try
            If (TypeOf obj Is cEnviroResponseFunction) Then
                Dim shp As cEnviroResponseFunction = DirectCast(obj, cEnviroResponseFunction)
                Debug.Assert(shp.ShapeFunctionType = eShapeFunctionType.Logistic_4Params)
                shp.ResponseLeftLimit = Me.ParamValue(1)
                shp.ResponseRightLimit = Me.ParamValue(2)
            End If
        Catch ex As Exception

        End Try

        Return True

    End Function
End Class
