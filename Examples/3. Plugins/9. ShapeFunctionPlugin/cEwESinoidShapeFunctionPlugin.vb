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

Option Strict On
Imports EwEPlugin
Imports EwECore

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in point to deliver a sinoid shape function to the Ecosim 'Change Shape' 
''' user interfaces.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEwESinoidShapeFunctionPlugin
    Implements EwEPlugin.IEcosimShapeFunctionPlugin

#Region " Internal vars "

    Private m_core As cCore = Nothing
    Private m_sYZero As Single = 0.5
    Private m_sAmplitude As Single = 0.5
    Private m_sRepetitions As Single = 1.5
    Private m_sOffset As Single = 0
    Private m_nPoints As Integer = 0

    Private Const D2R As Single = 2 * Math.PI / 360.0!

#End Region ' Internal vars

#Region " Generic plug-in bits "

    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwEShapeFunctionPlugin"
        End Get
    End Property

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "EwE development team"
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
        Catch ex As Exception
            ' Kaboom
        End Try
    End Sub

#End Region ' Generic plug-in bits

#Region " Shape function "

    Public Sub Init(shape As Object) _
        Implements EwEUtils.Core.IShapeFunction.Init

        If (Not TypeOf shape Is cForcingFunction) Then Return

        Dim ff As cForcingFunction = DirectCast(shape, cForcingFunction)

        ' Store properties
        Me.m_nPoints = ff.ShapeData.Length

        If (ff.ShapeFunctionType = eShapeFunctionType.NotSet) Then
            ' ToDo: recognize that we're the proper function type based on a field that does not 
            '       yet exist in the shapes
            Me.m_sYZero = ff.YZero
            Me.m_sAmplitude = ff.YEnd
            Me.m_sRepetitions = ff.YBase
            Me.m_sOffset = ff.Steep
        End If

    End Sub

    Public Function IsCompatible(datatype As EwEUtils.Core.eDataTypes) As Boolean _
        Implements EwEUtils.Core.IShapeFunction.IsCompatible

        ' This shape function only applies to forcing functions
        Return (datatype = EwEUtils.Core.eDataTypes.Forcing)

    End Function

    Public Sub Defaults() _
        Implements EwEUtils.Core.IShapeFunction.Defaults

        ' YZero = 1
        Me.ParamValue(1) = 1
        ' Amplitude = 0.5
        Me.ParamValue(2) = 0.5
        ' Repetition = 1
        Me.ParamValue(3) = 1
        ' Offset = 180 degrees
        Me.ParamValue(4) = 180

    End Sub

    Public ReadOnly Property nParameters As Integer _
        Implements EwEUtils.Core.IShapeFunction.nParameters
        Get
            Return 4
        End Get
    End Property

    Public ReadOnly Property ParamName(iParam As Integer) As String _
        Implements EwEUtils.Core.IShapeFunction.ParamName
        Get
            Select Case iParam
                Case 1 : Return "Y Zero"
                Case 2 : Return "Amplitude"
                Case 3 : Return "Repetitions"
                Case 4 : Return "Offset"
            End Select
            Return "?"
        End Get
    End Property

    Public Property ParamValue(iParam As Integer) As Single _
        Implements EwEUtils.Core.IShapeFunction.ParamValue
        Get
            Select Case iParam
                Case 1 : Return Me.m_sYZero
                Case 2 : Return Me.m_sAmplitude
                Case 3 : Return Me.m_sRepetitions
                Case 4 : Return Me.m_sOffset
            End Select
            Return cCore.NULL_VALUE
        End Get
        Set(value As Single)
            Select Case iParam
                Case 1 : Me.m_sYZero = value
                Case 2 : Me.m_sAmplitude = value
                Case 3 : Me.m_sRepetitions = CSng(Math.Max(0.00001, value))
                Case 4 : Me.m_sOffset = value
            End Select
        End Set
    End Property

    Public Function Shape(nPoints As Integer) As Single() _
        Implements EwEUtils.Core.IShapeFunction.Shape

        Dim points(Me.m_nPoints) As Single
        Dim sStep As Single = nPoints / (360.0! * Me.m_sRepetitions)
        Dim sAngle As Single = Me.m_sOffset Mod 360

        For i As Integer = 1 To Math.Min(Me.m_nPoints, nPoints)
            points(i) = CSng(Me.m_sYZero + Math.Sin(sAngle * D2R) * Me.m_sAmplitude)
        Next
        For i As Integer = nPoints + 1 To Me.m_nPoints
            points(i) = points(nPoints)
        Next
        Return points

    End Function

    Public Function Apply(shape As Object) As Boolean _
        Implements EwEUtils.Core.IShapeFunction.Apply

        If (Not TypeOf shape Is cForcingFunction) Then Return False

        Dim ff As cForcingFunction = DirectCast(shape, cForcingFunction)
        ff.YZero = Me.m_sYZero
        ff.YEnd = Me.m_sAmplitude
        ff.YBase = Me.m_sRepetitions
        ff.Steep = Me.m_sOffset
        Return True

    End Function

#End Region ' Shape function

End Class
