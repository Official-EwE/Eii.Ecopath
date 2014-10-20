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

#End Region ' Imports

Public Class cSigmoidShapeFunction
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

        Return MyBase.Shape(nPoints)

    End Function

    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 0.0
        Me.ParamValue(2) = 2.0
        Me.ParamValue(3) = 0.5
        Me.ParamValue(4) = 3.0
    End Sub

    Public Overrides Function IsCompatible(datatype As EwEUtils.Core.eDataTypes) As Boolean
        Return (datatype = EwEUtils.Core.eDataTypes.Forcing) Or _
               (datatype = EwEUtils.Core.eDataTypes.Mediation) Or _
               (datatype = EwEUtils.Core.eDataTypes.PriceMediation) Or _
               (datatype = EwEUtils.Core.eDataTypes.CapacityMediation)
    End Function

    Public Overrides ReadOnly Property nParameters As Integer
        Get
            Return 4
        End Get
    End Property

    Public Overrides ReadOnly Property ShapeFunctionType As EwECore.eShapeFunctionType
        Get
            Return eShapeFunctionType.Sigmoid
        End Get
    End Property
End Class

