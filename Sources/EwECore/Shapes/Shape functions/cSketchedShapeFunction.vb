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

Public Class cSketchedShapeFunction
    Inherits cShapeFunction

    Private m_shapeOrg As Single() = Nothing

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Init(obj As Object)
        MyBase.Init(obj)
        Me.m_shapeOrg = CType(Me.m_points.Clone(), Single())
        Me.ParamValue(1) = Me.Max
    End Sub

    Public Overrides Sub Defaults()
        ' NOP
    End Sub

    Public Overrides Function IsCompatible(datatype As EwEUtils.Core.eDataTypes) As Boolean
        Return (datatype = EwEUtils.Core.eDataTypes.Forcing) Or _
               (datatype = EwEUtils.Core.eDataTypes.Mediation) Or _
               (datatype = EwEUtils.Core.eDataTypes.PriceMediation) Or _
               (datatype = EwEUtils.Core.eDataTypes.CapacityMediation)
    End Function

    Public Overrides ReadOnly Property nParameters As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            Select Case iParam
                Case 1 : Return "Max"
            End Select
            Return "?"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Shape"/>
    ''' <summary>
    ''' Returns the points for a sketched shape.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Shape(ByVal nPoints As Integer) As Single()

        If (Me.ParamsChanged) Then
            Dim sMax As Single = Me.Max
            Dim sScale As Single = 1

            If (sMax > 0) Then
                sScale = Me.ParamValue(1) / sMax
            End If

            For i As Integer = 1 To nPoints
                Me.m_points(i) = Me.m_shapeOrg(i) * sScale
            Next i
        End If

        Return MyBase.Shape(nPoints)

    End Function

    Public Overrides ReadOnly Property ShapeFunctionType As eShapeFunctionType
        Get
            Return eShapeFunctionType.NotSet
        End Get
    End Property

End Class

