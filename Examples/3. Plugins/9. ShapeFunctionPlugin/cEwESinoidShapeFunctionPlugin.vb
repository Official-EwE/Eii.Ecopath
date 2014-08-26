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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin

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
    ''' <summary>The number of poitns in shape operated on.</summary>
    Private m_nPoints As Integer

    ''' <summary>Degree-to-radians conversion factor.</summary>
    Private Const cDegToRad As Single = Math.PI / 180.0!
    ''' <summary>Another handy one.</summary>
    Private Const cTwoPI As Single = Math.PI * 2.0!

#End Region ' Internal vars

#Region " Generic plug-in bits "

    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "Sinoid shape function"
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

    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in that provides a sinoid shape function"
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
        Implements EwEUtils.Core.IShapeFunction.Init

        If (Not TypeOf shape Is cForcingFunction) Then Return

        Dim ff As cForcingFunction = DirectCast(shape, cForcingFunction)

        ' Store shape properties for later use
        Me.m_nPoints = ff.ShapeData.Length

        ' Do not initialize setup parameters until we have a way to safely deduct
        ' that this plug-in was used to modify a shape in the first place. 
        'Me.m_sYZero = ff.YZero
        'Me.m_sAmplitude = ff.YEnd
        'Me.m_sRepetitions = ff.YBase
        'Me.m_sOffset = ff.Steep

    End Sub

    Public Function IsCompatible(datatype As EwEUtils.Core.eDataTypes) As Boolean _
        Implements EwEUtils.Core.IShapeFunction.IsCompatible

        ' This shape function only applies to forcing functions
        Return (datatype = EwEUtils.Core.eDataTypes.Forcing)

    End Function

    Public Sub Defaults() _
        Implements EwEUtils.Core.IShapeFunction.Defaults

        ' Pick some nice defaults
        Me.YZero = 1
        Me.Amplitude = 0.5
        Me.Repetitions = 1
        Me.Offset = 0

    End Sub

    Public ReadOnly Property nParameters As Integer _
        Implements EwEUtils.Core.IShapeFunction.nParameters
        Get
            ' Tell the EwE interface that the Sinoid shape has four configurable parameters
            Return 4
        End Get
    End Property

    Public ReadOnly Property ParamName(iParam As Integer) As String _
        Implements EwEUtils.Core.IShapeFunction.ParamName
        Get
            ' Tell the EwE interface the name of configurable parameter 'iParam'
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
        Implements EwEUtils.Core.IShapeFunction.ParamUnit
        Get
            ' Tell the EwE interface the unit of configurable parameter 'iParam', if any
            Select Case iParam
                Case 4
                    ' The 'offset' parameter must be specified in decimal degrees
                    Return My.Resources.UNIT_OFFSET
            End Select
            Return ""
        End Get
    End Property

    Public Property ParamValue(iParam As Integer) As Single _
        Implements EwEUtils.Core.IShapeFunction.ParamValue
        Get
            ' Tell the EwE interface the value of configurable parameter 'iParam'
            Select Case iParam
                Case 1 : Return Me.YZero
                Case 2 : Return Me.Amplitude
                Case 3 : Return Me.Repetitions
                Case 4 : Return Me.Offset
            End Select
            Return cCore.NULL_VALUE
        End Get
        Set(value As Single)
            ' Allow the EwE interface to set the value of configurable parameter 'iParam'
            Select Case iParam
                Case 1 : Me.YZero = value
                Case 2 : Me.Amplitude = value
                Case 3 : Me.Repetitions = value
                Case 4 : Me.Offset = value
            End Select
        End Set
    End Property

    Public Function Shape(nPoints As Integer) As Single() _
        Implements EwEUtils.Core.IShapeFunction.Shape

        ' Tell the EwE interface the actual shape of the sinoid, computed using the current parameter values

        Dim points(Me.m_nPoints) As Single
        Dim dStep As Double = (Me.Repetitions * 360.0!) / nPoints
        Dim dAngle As Double = Me.Offset Mod 360.0!

        For i As Integer = 1 To Math.Min(Me.m_nPoints, nPoints)
            points(i) = Me.YZero + CSng(Math.Sin(dAngle * cDegToRad)) * Me.Amplitude
            dAngle = (dAngle + dStep) Mod 360.0!
        Next

        ' Complete the rest of the shape by repeating the last value until the end of the shape
        For i As Integer = nPoints + 1 To Me.m_nPoints
            points(i) = points(nPoints)
        Next

        ' Done
        Return points

    End Function

    Public Function Apply(shape As Object) As Boolean _
        Implements EwEUtils.Core.IShapeFunction.Apply

        If (Not TypeOf shape Is cForcingFunction) Then Return False

        Dim ff As cForcingFunction = DirectCast(shape, cForcingFunction)

        ' Do not store setup parameters until we have a way to safely deduct
        ' what plug-in was used to modify a shape.

        'ff.YZero = Me.m_sYZero
        'ff.YEnd = Me.m_sAmplitude
        'ff.YBase = Me.m_sRepetitions
        'ff.Steep = Me.m_sOffset

        Return True

    End Function

#End Region ' Shape function

End Class
