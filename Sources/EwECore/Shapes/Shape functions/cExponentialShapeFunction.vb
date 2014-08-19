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

Public Class cExponentialShapeFunction
    Inherits cShapeFunction

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function Shape(Optional nPoints As Integer = 1200) As Single()
        If (Me.ParamsChanged) Then
            Dim sYZero As Single = Me.ParamValue(1)
            Dim sYEnd As Single = Me.ParamValue(2)
            Dim sYBase As Single = Me.ParamValue(3)
            Dim expK As Single

            If sYZero > 0 Then
                expK = CSng((1 / cShapeFunction.xBase) * Math.Log(sYBase / sYZero))
            Else
                expK = 10
            End If

            For i As Integer = 1 To nPoints
                Dim sTmp As Single = CSng(sYZero * Math.Exp(expK * i / nPoints))
                If sTmp > 1 Then sTmp = 1
                Me.m_points(i) = sTmp
            Next i
            Me.ParamsChanged = False
        End If
        Return Me.m_points
    End Function

    Public Overrides Sub Defaults()
        Me.ParamValue(1) = 1.0
        Me.ParamValue(2) = 5.0
        Me.ParamValue(3) = 0.2
    End Sub

    Public Overrides Function IsRelevantDataType(datatype As EwEUtils.Core.eDataTypes) As Boolean
        Return (datatype = EwEUtils.Core.eDataTypes.Forcing) Or _
               (datatype = EwEUtils.Core.eDataTypes.Mediation) Or _
               (datatype = EwEUtils.Core.eDataTypes.PriceMediation)
    End Function

    Public Overrides ReadOnly Property nParameters As Integer
        Get
            Return 3
        End Get
    End Property

    Public Overrides ReadOnly Property ShapeFunctionType As EwECore.eShapeFunctionType
        Get
            Return eShapeFunctionType.Exponential
        End Get
    End Property

End Class
