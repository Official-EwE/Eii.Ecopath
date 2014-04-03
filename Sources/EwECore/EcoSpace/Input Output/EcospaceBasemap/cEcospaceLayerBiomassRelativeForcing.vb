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
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcospaceLayerBiomassRelativeForcing
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal iIndex As Integer)
        MyBase.New(theCore, manager, theCore.m_EcoPathData.GroupName(iIndex) + " " + iIndex.ToString, EwEUtils.Core.eVarNameFlags.LayerBiomassRelativeForcing, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerBiomassRelativeForcing
    End Sub

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Return d(iRow, iCol, Me.Index)
            Catch ex As Exception

            End Try
            Return cCore.NULL_VALUE
            ' Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
            ' If Me.ValidateCellPosition(iRow, iCol) Then Return d(iRow, iCol, Me.Index) Else Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Object)
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Dim s As Single = Convert.ToSingle(value)
                d(iRow, iCol, Me.Index) = s
                Me.Invalidate()
            Catch ex As Exception

            End Try
            'Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
            'Dim s As Single = Convert.ToSingle(value)
            'If Me.ValidateCellValue(value) Then
            '    If Me.ValidateCellPosition(iRow, iCol) Then
            '        d(iRow, iCol, Me.Index) = s
            '        Me.Invalidate()
            '    End If
            'End If
        End Set
    End Property


End Class
