' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Namespace SpatialData

    ' ToDo: change storage from VarName to DataType

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class cSpatialDataStructures

        ''' <summary>Defined just in case ever needed</summary>
        Private ReadOnly m_pathDS As cEcopathDataStructures = Nothing
        ''' <summary>Defined just in case ever needed</summary>
        Private ReadOnly m_spaceDS As cEcospaceDataStructures = Nothing

        ''' <summary>Availalable data adapters</summary>
        Public DataAdapters As New List(Of cSpatialDataAdapter)
        ''' <summary>Max. number of external data connections per layer.</summary>
        Public Const cMAX_CONN As Integer = 6

        Public Sub New(pathDS As cEcopathDataStructures, spaceDS As cEcospaceDataStructures)
            Me.m_pathDS = pathDS
            Me.m_spaceDS = spaceDS
        End Sub

        Public Sub SetDefaults()

            Me.m_data.Clear()

            For Each adt As cSpatialDataAdapter In Me.DataAdapters
                adt.Initialize()
                Dim iLen As Integer = adt.MaxLength
                Dim arr(iLen, cMAX_CONN) As cAdapaterConfiguration
                For i As Integer = 0 To iLen
                    For j As Integer = 1 To cMAX_CONN
                        arr(i, j) = New cAdapaterConfiguration()
                    Next j
                Next i

                Me.m_data(adt.VarName) = arr
            Next adt

        End Sub

        Public ReadOnly Property GetDataAdapter(vn As eVarNameFlags) As cSpatialDataAdapter
            Get
                For Each adt As cSpatialDataAdapter In Me.DataAdapters
                    If (adt.VarName = vn) Then Return adt
                Next
                Return Nothing
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a spatial data configuration for a given (layer, connection slot) combination.
        ''' </summary>
        ''' <param name="varname"></param>
        ''' <param name="iIndex">One-based layer index</param>
        ''' <param name="iConnection">One-based connection index</param>
        ''' -------------------------------------------------------------------
        Friend ReadOnly Property Item(varname As eVarNameFlags, iIndex As Integer, iConnection As Integer) As cAdapaterConfiguration
            Get
                If Me.m_data.ContainsKey(varname) Then
                    Dim adata As cAdapaterConfiguration(,) = Me.m_data(varname)
                    If (iIndex >= 0 And iIndex < adata.Length) And (iConnection >= 0 And iConnection <= cMAX_CONN) Then
                        Return adata(iIndex, iConnection)
                    End If
                End If
                Return Nothing
            End Get
        End Property

#Region " Internals "

        ''' <summary>
        ''' Class for maintaining the details for a given spatial data connection.
        ''' </summary>
        Friend Class cAdapaterConfiguration

            ''' <summary>GUID to locally defined dataset.</summary>
            Public Property DatasetGUID As String
            ''' <summary>Typename to instantiate dataset.</summary>
            Public Property DatasetTypeName As String
            ''' <summary>Typename to instantiate converter.</summary>
            Public Property ConverterTypeName As String
            ''' <summary>Configuration to set up converter.</summary>
            Public Property ConverterConfig As String
            ''' <summary>Data scale, if any.</summary>
            Public Property Scale As Single
            ''' <summary></summary>
            Public Property ScaleType As eScaleType

            Public Property CustomDateStart As DateTime
            Public Property CustomDateEnd As DateTime

            Public Sub New()
                Me.Clear()
            End Sub

            Friend Sub Clear()
                Me.DatasetGUID = ""
                Me.DatasetTypeName = ""
                Me.ConverterTypeName = ""
                Me.ConverterConfig = ""
                Me.Scale = 1
                Me.ScaleType = 0
                Me.CustomDateStart = Date.MaxValue
                Me.CustomDateEnd = Date.MinValue
            End Sub

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Storage structure of data connections by varname.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_data As New Dictionary(Of eVarNameFlags, cAdapaterConfiguration(,))

#End Region ' Internals

    End Class

End Namespace
