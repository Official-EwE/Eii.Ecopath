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

#Region " Imports directive "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports directive

''' <summary>
''' Layer providing access to Ecospace migration data.
''' </summary>
Public Class cEcospaceLayerMigration
    Inherits cEcospaceLayerBoolean

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer that derives its data and identity from 
    ''' a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef theCore As cCore, ByVal manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerMigration, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerMigration
    End Sub

#End Region ' Construction

#Region " Cell interaction "

    Public Property Month As Integer = 1

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Return DirectCast(Me.Data, Boolean(,)(,))(Me.Index, Me.Month)(iRow, iCol)
        End Get
        Set(ByVal value As Object)
            DirectCast(Me.Data, Boolean(,)(,))(Me.Index, Me.Month)(iRow, iCol) = CBool(value)
        End Set
    End Property

#End Region ' Cell interaction

#Region " Overrides "

    Protected Overrides Function DefaultName() As String
        Return String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_MIGRATION, Me.Index, Me.m_core.EcoPathGroupInputs(Me.Index).Name)
    End Function

#End Region ' Overrides

End Class
