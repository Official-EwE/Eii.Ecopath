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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' JSON file serialization class that reads a game configuration.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cJSONGameConfig

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' JSON serialization helper.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cGameDef

        ''' <summary>
        ''' Initializes a new instance of the <see cref="cGameDef"/> class.
        ''' </summary>
        Public Sub New()
            Me.pressures = New List(Of JObject)
            Me.outcomes = New List(Of JObject)
            Me.fishing = New List(Of JObject)
        End Sub

        Public Property region As String
        Public Property modelfile As String
        Public Property rows As Integer = 10
        Public Property columns As Integer = 10
        Public Property longitude As Single = -9999
        Public Property latitude As Single = -9999
        Public Property cellsize As Single = -9999
        Public Property timestep As Integer = 12
        Public Property pressures As IList(Of JObject)
        Public Property outcomes As IList(Of JObject)
        Public Property fishing As IList(Of JObject)
        Public Property outcomerange As Double = 10.0!

    End Class

#Region " Private vars "

    Private m_gamedef As cGameDef = Nothing

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cJSONGameConfig"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extracts game configuration from the specified JSON text.
    ''' </summary>
    ''' <param name="strJSON">The text to extract game information from.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Load(strJSON As String) As Boolean
        Me.m_gamedef = CType(JsonConvert.DeserializeObject(strJSON, GetType(cGameDef)), cGameDef)
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Loads a JSON file and extracts game configuration.
    ''' </summary>
    ''' <param name="strJSONfile">The JSON file to read and extract game information from.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadFile(strJSONfile As String) As Boolean

        Using sr As New StreamReader(strJSONfile)
            Dim strJSON As String = sr.ReadToEnd()
            Return Me.Load(strJSON)
        End Using
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the EwE model file provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EwEModelFile As String
        Get
            If (Me.m_gamedef Is Nothing) Then Return ""
            Return Me.m_gamedef.modelfile
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the EwE game mode provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Mode As String
        Get
            If (Me.m_gamedef Is Nothing) Then Return ""
            Return Me.m_gamedef.region
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of time steps per year provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Timestep As Integer
        Get
            If (Me.m_gamedef Is Nothing) Then Return 12
            Return Me.m_gamedef.timestep
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the game area longitude (western edge) provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Longitude As Double
        Get
            If (Me.m_gamedef Is Nothing) Then Return cCore.NULL_VALUE
            Return Me.m_gamedef.longitude
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the game area latitude (northern edge) provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Latitude As Double
        Get
            If (Me.m_gamedef Is Nothing) Then Return cCore.NULL_VALUE
            Return Me.m_gamedef.latitude
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the game area cell size provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property CellSize As Double
        Get
            If (Me.m_gamedef Is Nothing) Then Return cCore.NULL_VALUE
            Return Me.m_gamedef.cellsize
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the game area number of colums provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumColumns As Integer
        Get
            If (Me.m_gamedef Is Nothing) Then Return cCore.NULL_VALUE
            Return Me.m_gamedef.columns
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the game area number of rows provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumRows As Integer
        Get
            If (Me.m_gamedef Is Nothing) Then Return cCore.NULL_VALUE
            Return Me.m_gamedef.rows
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the game outcome range in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property OutcomeRange As Double
        Get
            If (Me.m_gamedef Is Nothing) Then Return 10.0!
            Return Me.m_gamedef.outcomerange
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the pressure definitions provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <JsonIgnoreAttribute()>
    Public ReadOnly Property Pressures As cPressure()
        Get
            Dim lPressures As New List(Of cPressure)
            If (Me.m_gamedef IsNot Nothing) Then
                For Each obj As JObject In Me.m_gamedef.pressures
                    Dim p As cPressure = New cPressure(obj.Property("name").Value.ToString, cPressure.eDataTypes.Grid)
                    lPressures.Add(p)
                Next
                For Each obj As JObject In Me.m_gamedef.fishing
                    Dim p As cPressure = New cPressure(obj.Property("name").Value.ToString, cPressure.eDataTypes.Scalar)
                    lPressures.Add(p)
                Next
            End If
            Return lPressures.ToArray()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the outcome definitions provided in the JSON text.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <JsonIgnoreAttribute()>
    Public ReadOnly Property Outcomes As cGrid()
        Get
            Dim lOutputs As New List(Of cGrid)
            If (Me.m_gamedef IsNot Nothing) Then
                For Each obj As JObject In Me.m_gamedef.outcomes
                    Dim g As New cGrid(obj.Property("name").Value.ToString, Me.m_gamedef.columns, Me.m_gamedef.rows)
                    lOutputs.Add(g)
                Next
            End If
            Return lOutputs.ToArray()
        End Get
    End Property

End Class
