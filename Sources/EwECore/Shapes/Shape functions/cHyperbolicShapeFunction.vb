' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cHyperbolicShapeFunction
    Inherits cShapeFunction

    Public Sub New()
        MyBase.New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Shape"/>
    ''' <summary>
    ''' Returns the points for a hyperbolic shape.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Shape(nPoints As Integer) As Single()

        If (Me.ParamsChanged) Then
            Dim YZero As Single = Me.ParamValue(1)
            Dim YEnd As Single = Me.ParamValue(2)
            Dim YBase As Single = Me.ParamValue(3)
            Dim Steep As Single = 1.0
            Dim xHalf, xPow As Single

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'jb 22-Nov-2018 no point in including sSteep parameter. 
            'It's not doing anything
            'If sYBase <> sYZero Then
            '    xHalf = CSng((sYEnd - sYZero) * ((cShapeFunction.xBase ^ sSteep) / (sYBase - sYZero)) - (cShapeFunction.xBase ^ sSteep))
            'Else
            '    xHalf = 1000
            'End If
            'For i As Integer = 1 To nPoints
            '    xPow = CSng((i / nPoints) ^ sSteep)
            '    If (xHalf + xPow <> 0) Then
            '        Me.m_points(i) = sYZero + ((sYEnd - sYZero) * xPow / (xHalf + xPow))
            '    End If
            'Next i
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


            If YBase <> YZero Then
                xHalf = CSng((YEnd - YZero) * (cShapeFunction.xBase / (YBase - YZero)) - cShapeFunction.xBase)
            Else
                xHalf = 1000
            End If
            For i As Integer = 1 To nPoints
                xPow = CSng((i / nPoints))
                If (xHalf + xPow <> 0) Then
                    Me.m_points(i) = YZero + ((YEnd - YZero) * xPow / (xHalf + xPow))
                End If
            Next i
        End If

        Return MyBase.Shape(nPoints)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 1.0
        Me.ParamValue(2) = 3.0
        Me.ParamValue(3) = 0.75
        Me.ParamValue(4) = 1.0F 'Scalar
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ParamName"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            Select Case iParam
                Case 1
                    Return "Y Zero"
                Case 2
                    Return "Y End"
                Case 3
                    Return "Y Base"

                'Only override the scalar name
                Case 4 : Return "Y scalar"
            End Select
            Return MyBase.ParamName(iParam)
        End Get
    End Property

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
            Return False
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
            Return eShapeFunctionType.Hyperbolic
        End Get
    End Property

End Class
