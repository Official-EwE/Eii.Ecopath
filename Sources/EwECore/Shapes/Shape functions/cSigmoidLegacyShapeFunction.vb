' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cSigmoidLegacyShapeFunction
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
            Dim sYZero As Single = Me.ParamValue(1)
            Dim sYEnd As Single = Me.ParamValue(2)
            Dim sYBase As Single = Me.ParamValue(3)
            Dim sSteep As Single = Me.ParamValue(4)
            Dim xHalf, xPow As Single

            If sYBase <> sYZero Then
                xHalf = CSng((sYEnd - sYZero) * ((cShapeFunction.xBase ^ sSteep) / (sYBase - sYZero)) - (cShapeFunction.xBase ^ sSteep))
            Else
                xHalf = 1000
            End If
            For i As Integer = 1 To nPoints
                xPow = CSng((i / nPoints) ^ sSteep)
                If (xHalf + xPow <> 0) Then
                    Me.m_points(i) = sYZero + ((sYEnd - sYZero) * xPow / (xHalf + xPow))
                End If
            Next i

        End If

        Me.ScaleData(nPoints, 1.0)

        Return MyBase.Shape(nPoints)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 0.0
        Me.ParamValue(2) = 2.0
        Me.ParamValue(3) = 0.5
        Me.ParamValue(4) = 3.0
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
    ''' <inheritdocs cref="cShapeFunction.ShapeFunctionType"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ShapeFunctionType As Long
        Get
            Return eShapeFunctionType.Sigmoid_Legacy
        End Get
    End Property

End Class

