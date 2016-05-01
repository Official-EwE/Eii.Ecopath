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
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Example plug-in point to add noise to an existing shape.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEwENoseShapeFunctionPlugin
    Implements EwEPlugin.IEcosimShapeFunctionPlugin

#Region " Internal vars "

    ''' <summary>The core to operate on.</summary>
    Private m_core As cCore
    ''' <summary>The points of the shape</summary>
    Protected m_points As Single() = Nothing

#End Region ' Internal vars

#Region " Generic plug-in bits "

    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwE6.example.shapefunction.noise"
        End Get
    End Property

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "EwE development team / Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String _
        Implements EwEPlugin.IEcosimShapeFunctionPlugin.DisplayName
        Get
            Return My.Resources.NAME_NOISE
        End Get
    End Property

    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return My.Resources.DESCRIPTION_NOISE
        End Get
    End Property

    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
            Me.Defaults()
        Catch ex As Exception
            ' Kaboom
        End Try
    End Sub

#End Region ' Generic plug-in bits

#Region " Shape parameters "

    ''' <summary>
    ''' Min noise value.
    ''' </summary>
    Private Property Band As Single = 0.1

    Public ReadOnly Property ParamStatus(iParam As Integer) As eStatusFlags _
    Implements IShapeFunction.ParamStatus
        Get
            Return eStatusFlags.OK
        End Get
    End Property

#End Region ' Shape parameters

#Region " Shape function "

    Public Sub Init(shape As Object) _
        Implements IEcosimShapeFunctionPlugin.Init

        If (Not TypeOf shape Is cForcingFunction) Then Return
        Dim ff As cForcingFunction = DirectCast(shape, cForcingFunction)

        Me.m_points = ff.ShapeData

        If (ff.ShapeFunctionType <> Me.ShapeFunctionType) Then Return

        Me.ParamValue(1) = ff.YZero
        Me.ParamValue(2) = ff.YEnd
        Me.ParamValue(3) = ff.YBase
        Me.ParamValue(4) = ff.Steep

    End Sub

    Public Function IsCompatible(datatype As eDataTypes) As Boolean _
        Implements IEcosimShapeFunctionPlugin.IsCompatible

        ' This shape function only applies to any type of function
        Return True

    End Function

    Public Sub Defaults() _
        Implements IEcosimShapeFunctionPlugin.Defaults

        Me.Band = 0.1

    End Sub

    Public ReadOnly Property nParameters As Integer _
        Implements IEcosimShapeFunctionPlugin.nParameters
        Get
            ' Tell EwE that the noise shape function has three configurable parameters
            Return 1
        End Get
    End Property

    Public ReadOnly Property ParamName(iParam As Integer) As String _
        Implements IEcosimShapeFunctionPlugin.ParamName
        Get
            ' Tell the EwE interface the name of configurable parameter 'iParam'
            Select Case iParam
                Case 1 : Return My.Resources.PARAM_BAND
            End Select
            Return "?"
        End Get
    End Property

    Public ReadOnly Property ParamUnit(iParam As Integer) As String _
        Implements IEcosimShapeFunctionPlugin.ParamUnit
        Get
            ' No units for any of the noise function parameters
            Return ""
        End Get
    End Property

    Public Property ParamValue(iParam As Integer) As Single _
        Implements IEcosimShapeFunctionPlugin.ParamValue
        Get
            ' Tell EwE the values of each configurable parameter
            Select Case iParam
                Case 1 : Return Me.Band
            End Select
            Return cCore.NULL_VALUE
        End Get
        Set(value As Single)
            ' Allow EwE to set the value of each configurable parameter
            Select Case iParam
                Case 1 : Me.Band = value
            End Select
        End Set
    End Property

    Public Function Shape(nPoints As Integer) As Single() _
        Implements IEcosimShapeFunctionPlugin.Shape

        ' Tell EwE the actual shape, computed from the current parameter values
        Dim pt As Integer = 1
        Dim rnd As New Random()

        While (pt < Me.m_points.Length)
            Me.m_points(pt) += CSng(Me.Band * rnd.NextDouble() - Me.Band / 2)
            pt += 1
        End While

        ' Done
        Return Me.m_points

    End Function

    Public ReadOnly Property ShapeFunctionType As Long _
        Implements IEcosimShapeFunctionPlugin.ShapeFunctionType
        Get
            ' This is quite a random number
            Return -421300666
        End Get
    End Property

    Public Function Apply(shape As Object) As Boolean _
        Implements IEcosimShapeFunctionPlugin.Apply

        If (Not TypeOf shape Is cForcingFunction) Then Return False
        Dim shp As cForcingFunction = DirectCast(shape, cForcingFunction)
        shp.ShapeData = Me.m_points
        shp.ShapeFunctionType = Me.ShapeFunctionType

        ' Store configuration parameters
        shp.YZero = Me.ParamValue(1)
        shp.YEnd = Me.ParamValue(2)
        shp.YBase = Me.ParamValue(3)
        shp.Steep = Me.ParamValue(4)
        Return True

    End Function

#End Region ' Shape function

End Class
