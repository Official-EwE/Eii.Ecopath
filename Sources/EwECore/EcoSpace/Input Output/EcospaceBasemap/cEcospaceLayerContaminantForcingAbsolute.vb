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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Layer providing access to Ecospace contaminant forcing data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceLayerContaminantForcingAbsolute
    Inherits cEcospaceLayerSingle

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        ' Nassssty! We canot use zero-indexed layers in EwE
        MyBase.New(theCore, manager, "", EwEUtils.Core.eVarNameFlags.LayerContaminantForcingAbsolute, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerContaminantForcingAbs
    End Sub

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Return d(iRow, iCol, 0)
            Catch ex As Exception

            End Try
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Object)
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Dim s As Single = Convert.ToSingle(value)
                d(iRow, iCol, 0) = s
                Me.Invalidate()
            Catch ex As Exception

            End Try
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overriden to include the group name into this layer's name
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function DefaultName() As String
        Return My.Resources.CoreDefaults.CORE_DEFAULT_ENVIRONMENT
    End Function

End Class

