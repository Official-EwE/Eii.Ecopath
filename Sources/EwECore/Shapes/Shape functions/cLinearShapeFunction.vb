' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cLinearShapeFunction
    Inherits cShapeFunction

    Public Sub New()
        MyBase.New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Shape"/>
    ''' <summary>
    ''' Returns the points for a linear shape.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Shape(nPoints As Integer) As Single()

        If (Me.ParamsChanged) Then
            Dim A As Single = Me.ParamValue(1)
            Dim B As Single = Me.ParamValue(2)
            For i As Integer = 1 To nPoints
                Me.m_points(i) = A + (B - A) * (i - 1) / (nPoints - 1)
            Next i
        End If

        Return MyBase.Shape(nPoints)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 1
        Me.ParamValue(2) = 1
    End Sub

    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            Select Case iParam
                Case 1 : Return My.Resources.CoreDefaults.PARAM_START
                Case 2 : Return My.Resources.CoreDefaults.PARAM_END
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
            Return 2
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.ShapeFunctionType"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ShapeFunctionType As Long
        Get
            Return eShapeFunctionType.Linear
        End Get
    End Property

End Class
