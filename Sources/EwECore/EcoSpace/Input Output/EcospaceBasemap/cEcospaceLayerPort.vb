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
Imports DefaultRes = EwECore.My.Resources.CoreDefaults
Imports EwEUtils.SystemUtilities.cSystemUtils

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace port data.
''' </summary>
Public Class cEcospaceLayerPort
    Inherits cEcospaceLayerInteger

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, _
                   String.Format(DefaultRes.CORE_DEFAULT_PORT, iIndex), _
                   EwEUtils.Core.eVarNameFlags.LayerPort, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerPort
    End Sub

#Region " Cell interaction "

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))
            If (Me.Index = 0) Then
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    If data(iFleet, iRow, iCol) Then Return 1.0!
                Next
                Return cCore.NULL_VALUE
            Else
                Return CSng(IIf(data(Me.Index, iRow, iCol), 1.0!, 0.0!))
            End If
        End Get
        Set(ByVal value As Object)
            Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))
            ' ToDo: only allow coastal cells to be set
            If (Me.Index = 0) Then
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    data(iFleet, iRow, iCol) = (CSng(value) <> 0.0!)
                Next
            Else
                data(Me.Index, iRow, iCol) = (CSng(value) <> 0.0!)
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return 1.0!
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            Return 0.0!
        End Get
    End Property

    Protected Overrides Sub RecalcStats()

        'Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        'Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
        'Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))

        Me.m_iMaxValue = 1
        Me.m_iMinValue = 0
        Me.m_iNumValueCells = 1

        Me.m_bInvalidateStats = False

        'For iRow As Integer = 1 To bm.InRow
        '    For iCol As Integer = 1 To bm.InCol
        '        If layerDepth.IsWaterCell(iRow, iCol) Then
        '            For iFleet As Integer = 1 To Me.m_core.nFleets
        '                If data(iFleet, iRow, iCol) Then
        '                    Me.m_iNumValueCells += 1
        '                    Return
        '                End If
        '            Next iFleet
        '        End If
        '    Next iCol
        'Next iRow

    End Sub

#End Region ' Cell interaction

End Class
