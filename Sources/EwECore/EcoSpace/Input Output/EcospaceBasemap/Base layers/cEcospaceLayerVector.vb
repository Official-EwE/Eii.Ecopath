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
Imports EwECore.Core
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Base layer providing access to Ecospace data as cells, each representing a
''' vector with a X and Y component.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class cEcospaceLayerVector
    Inherits cEcospaceLayer

#Region " Private variables "

    ''' <summary>Layer max vector value.</summary>
    Protected m_sMaxValue As Single = 0.0!
    ''' <summary>Layer num of cells with a value.</summary>
    Private m_iNumValueCells As Integer = 0

    ''' <summary>States whether layer max value should be recalculated.</summary>
    ''' <remarks>True at startup to make sure that the max vector size is properly 
    ''' calculated when first queried.</remarks>
    Private m_bInvalidateStats As Boolean = True

#End Region ' Private variables

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer of vectors that derives its data and identity 
    ''' from a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' <param name="iIndex"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal manager As IEcospaceLayerManager, _
                   ByVal strName As String, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, strName, varName, iIndex, GetType(Single))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for a NxN layer of Vector values that derives its data from 
    ''' a manager, but that is a unique data entity in the EwE core.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="iDBID"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' <param name="iIndex"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal iDBID As Integer, _
                   ByVal manager As IEcospaceLayerManager, _
                   ByVal strName As String, _
                   ByVal varName As eVarNameFlags, _
                   Optional ByVal iIndex As Integer = cCore.NULL_VALUE)

        MyBase.New(theCore, iDBID, manager, strName, varName, iIndex, GetType(Single))

    End Sub

#End Region ' Construction

#Region " Cell interaction "

    ''' <summary>
    ''' Get/set a cell value in the form of Single(2), where index 0 represents
    ''' the X velocity, and index 1 represents the Y velocity of the value.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <param name="iCol"></param>
    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Return New Single() {Me.XVelocity(iRow, iCol), Me.YVelocity(iRow, iCol)}
        End Get
        Set(ByVal value As Object)
            Dim asValues As Single() = DirectCast(value, Single())
            Me.XVelocity(iRow, iCol) = asValues(0)
            Me.YVelocity(iRow, iCol) = asValues(1)
        End Set
    End Property

    ''' <summary>
    ''' Get X velocity data
    ''' </summary>
    Public MustOverride Property XVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single

    ''' <summary>
    ''' Get Y velocity data
    ''' </summary>
    Public MustOverride Property YVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single

    ''' <summary>
    ''' Get the max magnitude of all cells in the layer.
    ''' </summary>
    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            If Me.m_bInvalidateStats Then Me.RecalcStats()
            Return Me.m_sMaxValue
        End Get
    End Property

    ''' <summary>
    ''' Get the min magnitude of all cells in the layer.
    ''' </summary>
    Public Overrides ReadOnly Property MinValue() As Single
        Get
            Return 0
        End Get
    End Property

    ''' <inheritdocs cref="cEcospaceLayer.NumValueCells"/>
    Public Overrides ReadOnly Property NumValueCells As Integer
        Get
            If Me.m_bInvalidateStats Then Me.RecalcStats()
            Return Me.m_iNumValueCells
        End Get
    End Property

    Public Overrides Sub Invalidate()
        Me.m_bInvalidateStats = True
    End Sub

#End Region ' Cell interaction

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calc max vector size in data layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub RecalcStats()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap

        Me.m_sMaxValue = 0

        For iRow As Integer = 1 To bm.InRow
            For iCol As Integer = 1 To bm.InCol
                Me.m_sMaxValue = Math.Max(Me.m_sMaxValue, _
                                          Math.Max(Math.Abs(Me.XVelocity(iRow, iCol)), _
                                                   Math.Abs(Me.YVelocity(iRow, iCol))))
            Next iCol
        Next iRow
        Me.m_bInvalidateStats = False

    End Sub

#End Region ' Internals

End Class
