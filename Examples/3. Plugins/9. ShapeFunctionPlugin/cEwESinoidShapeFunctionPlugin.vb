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
''' Example plug-in point to deliver a sinoid shape function to the Ecosim 
''' 'Change Shape' user interfaces. Feel free, go wild, and add your own shapes.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEwESinoidShapeFunctionPlugin
    Implements EwEPlugin.IEcosimShapeFunctionPlugin

#Region " Internal vars "

    ''' <summary>The core to operate on.</summary>
    Private m_core As cCore
    ''' <summary>The points of the shape</summary>
    Protected m_points As Single() = Nothing

    ''' <summary>Degree-to-radians conversion factor.</summary>
    Private Const cDegToRad As Single = Math.PI / 180.0!
    ''' <summary>Another handy one.</summary>
    Private Const cTwoPI As Single = Math.PI * 2.0!

#End Region ' Internal vars

#Region " Generic plug-in bits "

    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwE6.example.shapefunction.sinoid"
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
            Return My.Resources.NAME_SINOID
        End Get
    End Property

    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return My.Resources.DESCIRPTION_SINOID
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
    ''' Y zero parameter of the Sinoid shape.
    ''' </summary>
    Private Property YZero As Single

    ''' <summary>
    ''' Sinoid amplitude.
    ''' </summary>
    Private Property Amplitude As Single

    ''' <summary>
    ''' Number of sinoid repetitions.
    ''' </summary>
    Private Property Repetitions As Single

    ''' <summary>
    ''' Offset angle (in decimal degrees) to start the sinoid with.
    ''' </summary>
    Private Property Offset As Single

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

        ' This shape function only applies to forcing functions
        Return (datatype = eDataTypes.Forcing)

    End Function

    Public Sub Defaults() _
        Implements IEcosimShapeFunctionPlugin.Defaults

        ' Pick some nice defaults
        Me.YZero = 1
        Me.Amplitude = 0.5
        Me.Repetitions = 1
        Me.Offset = 0

    End Sub

    Public ReadOnly Property nParameters As Integer _
        Implements IEcosimShapeFunctionPlugin.nParameters
        Get
            ' Tell EwE that the Sinoid shape function has four configurable parameters
            Return 4
        End Get
    End Property

    Public ReadOnly Property ParamName(iParam As Integer) As String _
        Implements IEcosimShapeFunctionPlugin.ParamName
        Get
            ' Tell EwE the names of each configurable parameter
            Select Case iParam
                Case 1 : Return My.Resources.PARAM_YZERO
                Case 2 : Return My.Resources.PARAM_AMPLITUDE
                Case 3 : Return My.Resources.PARAM_REPETITION
                Case 4 : Return My.Resources.PARAM_OFFSET
            End Select
            Return "?"
        End Get
    End Property

    Public ReadOnly Property ParamUnit(iParam As Integer) As String _
        Implements IEcosimShapeFunctionPlugin.ParamUnit
        Get
            ' Tell EwE the units of configurable parameters, if any
            Select Case iParam
                Case 4
                    ' The 'offset' parameter must be specified in decimal degrees
                    Return My.Resources.UNIT_OFFSET
            End Select
            Return ""
        End Get
    End Property

    Public Property ParamValue(iParam As Integer) As Single _
        Implements IEcosimShapeFunctionPlugin.ParamValue
        Get
            ' Tell EwE the value of each configurable parameter
            Select Case iParam
                Case 1 : Return Me.YZero
                Case 2 : Return Me.Amplitude
                Case 3 : Return Me.Repetitions
                Case 4 : Return Me.Offset
            End Select
            Return cCore.NULL_VALUE
        End Get
        Set(value As Single)
            ' Allow EwE to set the value of each configurable parameter
            Select Case iParam
                Case 1 : Me.YZero = value
                Case 2 : Me.Amplitude = value
                Case 3 : Me.Repetitions = value
                Case 4 : Me.Offset = value
            End Select
        End Set
    End Property

    Public Function Shape(nPoints As Integer) As Single() _
        Implements IEcosimShapeFunctionPlugin.Shape

        ' Tell EwE the actual shape, computed from the current parameter values

        Dim dStep As Double = (Me.Repetitions * 360.0!) / nPoints
        Dim dAngle As Double = Me.Offset Mod 360.0!

        For i As Integer = 1 To Math.Min(Me.m_points.Length, nPoints)
            Me.m_points(i) = Me.YZero + CSng(Math.Sin(dAngle * cDegToRad)) * Me.Amplitude
            dAngle = (dAngle + dStep) Mod 360.0!
        Next

        ' Complete the rest of the shape by repeating the last value until the end of the shape
        For i As Integer = nPoints + 1 To Me.m_points.Length - 1
            Me.m_points(i) = Me.m_points(nPoints)
        Next

        ' Done
        Return Me.m_points

    End Function

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

    Public ReadOnly Property ShapeFunctionType As Long _
        Implements IEcosimShapeFunctionPlugin.ShapeFunctionType
        Get
            ' This is quite a random number
            Return -421300667
        End Get
    End Property

#End Region ' Shape function

    Public ReadOnly Property ParamStatus(iParam As Integer) As eStatusFlags _
        Implements IShapeFunction.ParamStatus
        Get
            Return eStatusFlags.OK
        End Get
    End Property
End Class
