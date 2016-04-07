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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Drawing

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for defining a flow diagram. Although catered to groups and
    ''' consumption, this interface offers the possibility to reflect other
    ''' types of data in a flowdiagram-like structure.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IFlowDiagramData
        Inherits IUIElement

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the title of the flow diagram.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property Title As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the data for the flow diagram. This can be used to trigger
        ''' recalculations and recalibrations.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Sub Refresh()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the total number of groups in the flow diagram, including
        ''' living and non-living groups.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property NumGroups() As Integer

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of living groups in the flow diagram. Based on
        ''' the approach in Ecopath, occur before the non-living groups in the
        ''' total list of <see cref="NumGroups">groups</see>.
        ''' </summary>
        ''' <remarks>
        ''' Living groups can have incoming / predation and outgoing / prey links, 
        ''' whereas all non-living groups (<see cref="NumGroups"/> - <see cref="NumLivingGroups"/>)
        ''' can only have incoming / predation links.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        ReadOnly Property NumLivingGroups() As Integer

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the color to render a group in the flow diagram.
        ''' </summary>
        ''' <param name="iGroup">The index of the group to get the color for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property GroupColor(ByVal iGroup As Integer) As Color

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name to render a group in the flow diagram.
        ''' </summary>
        ''' <param name="iGroup">The index of the group to get the name for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property GroupName(ByVal iGroup As Integer) As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether a group should be rendered as visible.
        ''' </summary>
        ''' <param name="iGroup">The index of the group to get the visible state for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property IsGroupVisible(ByVal iGroup As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the value to reflect in the diagram for a group.
        ''' </summary>
        ''' <param name="iGroup">The index of the group to get the value for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property Value(ByVal iGroup As Integer) As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a formatted label for a <see cref="Value"/>.
        ''' </summary>
        ''' <param name="sValue">The value to format.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property ValueLabel(ByVal sValue As Single) As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the value to place on a link in the diagram.
        ''' </summary>
        ''' <param name="iPred">The index of the predator group / source of the link.</param>
        ''' <param name="iPrey">The index of the prey group / target of the link.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property LinkValue(ByVal iPred As Integer, ByVal iPrey As Integer) As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the rank for placing the group in the diagram. In foodwebs, the
        ''' rank would typcally be the trophic level of a group.
        ''' </summary>
        ''' <param name="iGroup">The index of the group to get the rank for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property TrophicLevel(ByVal iGroup As Integer) As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the maximum <see cref="Value"/> in the diagram.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property ValueMax() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the minimum <see cref="Value"/> in the diagram.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property ValueMin() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the maximum <see cref="LinkValue"/> in the diagram.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property LinkValueMax() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the minimum <see cref="LinkValue"/> in the diagram.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property LinkValueMin() As Single

    End Interface

End Namespace
