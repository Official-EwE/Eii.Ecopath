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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace SpatialData

    Public Class cSpatialDataStructures

        ''' <summary>Availalable data adapters</summary>
        Public DataAdapters As New List(Of cSpatialDataAdapter)
       
        Public Sub SetDefaults()

            Me.m_data.Clear()

            For Each adt As cSpatialDataAdapter In Me.DataAdapters
                adt.Initialize()
                Dim iLen As Integer = adt.Length
                Dim arr(iLen) As cAdapaterConfiguration
                For i As Integer = 0 To iLen - 1 : arr(i) = New cAdapaterConfiguration() : Next
                Me.m_data(adt.VarName) = arr
            Next adt

        End Sub

        Public ReadOnly Property Item(varname As eVarNameFlags, iIndex As Integer) As cAdapaterConfiguration
            Get
                If Me.m_data.ContainsKey(varname) Then
                    Dim adata As cAdapaterConfiguration() = Me.m_data(varname)
                    If iIndex >= 0 And iIndex < adata.Length Then
                        Return adata(iIndex)
                    End If
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property NumItems(varname As eVarNameFlags) As Integer
            Get
                Debug.Assert(Me.m_data.ContainsKey(varname))
                Return Me.m_data(varname).Length
            End Get
        End Property

#Region " Internals "

        Public Class cAdapaterConfiguration
            Public DatasetGUID As String = ""
            Public Converter As String = ""
            Public ConverterConfig As String = ""
            Public Scale As Single = 1.0!
            Public ScaleType As Byte = 0
        End Class

        Private m_data As New Dictionary(Of eVarNameFlags, cAdapaterConfiguration())

#End Region ' Internals

    End Class

End Namespace
