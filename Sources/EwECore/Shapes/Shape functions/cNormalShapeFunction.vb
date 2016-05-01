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

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cNormalShapeFunction
    Inherits cShapeFunction

    Public Sub New()
        MyBase.New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ParamName"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            Select Case iParam
                Case 1 : Return My.Resources.CoreDefaults.PARAM_SD_LEFT
                Case 2 : Return My.Resources.CoreDefaults.PARAM_SD_RIGHT
                Case 3 : Return My.Resources.CoreDefaults.PARAM_SD_WIDTH
                Case 4 : Return My.Resources.CoreDefaults.PARAM_MEAN
                Case 5 : Return My.Resources.CoreDefaults.PARAM_MAX
            End Select
            Return MyBase.ParamName(iParam)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ParamStatus"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ParamStatus(iParam As Integer) As eStatusFlags
        Get
            If (iParam = 3) Then Return eStatusFlags.NotEditable
            Return MyBase.ParamStatus(iParam)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Shape"/>
    ''' <summary>
    ''' Returns the points for an normal distributed shape.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Shape(nPoints As Integer) As Single()

        If (Me.ParamsChanged) Then
            Dim sYEnd As Single = Me.ParamValue(2)
            'normal distribution with a mean of Zero
            'User defines 
            '   Standard deviation on the left and right
            '   Width of the data in standard deviations 
            '   Width is important because values outside the bounds 
            '       are just the first or last value in the shape

            'Normal and Beta shapes are not used for Forcing functions
            'so it is only the shape we are interested in not that actual data
            'how the shape affects the data is defined by the user by where they place the baseline
            'If these are to be used as Forcing Function then we will need a way to 'scale' the data
            'as there is no way to in the Forcing Function interface to select where the baseline is.
            Dim nPtHalf As Integer = nPoints \ 2
            'SD left
            Dim sd As Single = Me.ParamValue(1) + 0.0000001F
            'width in SD
            Dim Wsd As Single = Me.ParamValue(3)

            'Delta X 
            Dim dx As Single = Wsd / (nPoints - 1)
            'Start X
            Dim x0 As Single = -Wsd * 0.5F
            Dim max As Single = Me.ParamValue(5)
            Dim x As Single
            For i As Integer = 1 To nPoints
                If i > nPtHalf Then
                    sd = sYEnd + 0.0000001F
                End If
                x = x0 + dx * (i - 1)
                Me.m_points(i) = CSng(Math.Exp(-0.5 * (x / sd) ^ 2)) * max
            Next

            'xxxxxxALTERNATIVE WAY TO USE THE PARAMETERS NOT IMPLEMENTED HERE xxxxxxxxxxxx
            'Case eShapeFunctionType.Normal

            '    'normal distribution with a mean of Zero
            '    'User defines 
            '    '   Standard deviation on the left and right
            '    '   Width of the data in standard deviations 
            '    '   Width is important because values outside the bounds 
            '    '       are just the first or last value in the shape

            '    'Normal and Beta shapes are not used for Forcing functions
            '    'so it is only the shape we are interested in not that actual data
            '    'how the shape affects the data is defined by the user by where they place the baseline
            '    'If these are to be used as Forcing Function then we will need a way to 'scale' the data
            '    'as there is no way to in the Forcing Function interface to select where the baseline is.
            '    Dim nPtHalf As Integer = nPoints \ 2
            '    'SD left
            '    Dim SDLeft As Single = sYZero '+ 0.0000001F
            '    Dim SDRight As Single = sYEnd ' + 0.0000001F
            '    If SDLeft = 0 Then SDLeft = 0.0000001F
            '    If SDRight = 0 Then SDLeft = 0.0000001F

            '    Dim Mean As Single = sSteep
            '    'width in SD
            '    Dim Wsd As Single = sYBase

            '    'width in user defined units
            '    Dim Wvals As Single = Math.Max(SDLeft, SDRight) * Wsd
            '    'Delta X 
            '    Dim dx As Single = Wvals / (nPoints - 1)
            '    'Start X
            '    Dim x0 As Single = (-Wvals * 0.5F)
            '    Dim x As Single
            '    Dim sd As Single = SDLeft
            '    For i As Integer = 1 To nPoints
            '        If i > nPtHalf Then
            '            sd = SDRight ' + 0.0000001F
            '        End If
            '        x = x0 + dx * (i - 1)
            '        Me.m_asDataWork(i) = CSng(Math.Exp(-0.5 * (x / sd) ^ 2))
            '    Next
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        End If

        Return MyBase.Shape(nPoints)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 1
        Me.ParamValue(2) = 1
        Me.ParamValue(3) = 10
        Me.ParamValue(4) = 0
        Me.ParamValue(5) = 1
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.IsCompatible"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function IsCompatible(datatype As eDataTypes) As Boolean
        Return Me.IsMediation(datatype)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.nParameters"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property nParameters As Integer
        Get
            Return 5
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ShapeFunctionType"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ShapeFunctionType As Long
        Get
            Return eShapeFunctionType.Normal
        End Get
    End Property

    Public Overrides Sub Init(obj As Object)
        MyBase.Init(obj)

        If (Me.ShapeFunctionType <> eShapeFunctionType.Normal) Then Return

        ' Max cannot be stored yet. For now, deduct max from shape
        Dim max As Single = Single.MinValue
        For Each val As Single In Me.m_points
            max = Math.Max(val, max)
        Next
        If (max <= 0) Then max = 1
        Me.ParamValue(5) = max

    End Sub

End Class
