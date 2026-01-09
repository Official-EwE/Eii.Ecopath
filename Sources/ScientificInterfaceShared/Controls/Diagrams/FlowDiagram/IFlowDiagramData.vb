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
        ''' Get/set the name of the data displayed in the diagram.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property DataTitle As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the data for the flow diagram. This can be used to trigger
        ''' recalculations and recalibrations.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Sub Refresh()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the total number of items in the flow diagram, including
        ''' living and non-living items.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property NumItems() As Integer

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of living groups in the flow diagram. Based on
        ''' the approach in Ecopath, occur before the non-living groups in the
        ''' total list of <see cref="NumItems">groups</see>.
        ''' </summary>
        ''' <remarks>
        ''' Living groups can have incoming / predation and outgoing / prey links, 
        ''' whereas all non-living groups (<see cref="NumItems"/> - <see cref="NumLivingItems"/>)
        ''' can only have incoming / predation links.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        ReadOnly Property NumLivingItems() As Integer

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the color to render an item in the flow diagram.
        ''' </summary>
        ''' <param name="iItem">The index of the item to get a color for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property ItemColor(iItem As Integer) As Color

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name to render an item in the flow diagram.
        ''' </summary>
        ''' <param name="iItem">The index of the item to get the name for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property ItemName(iItem As Integer) As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the category to render an item in the flow diagram legend.
        ''' </summary>
        ''' <param name="iItem">The index of the item to get the category for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property ItemCategory(iItem As Integer) As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether an item should be rendered as visible.
        ''' </summary>
        ''' <param name="iIndex">The index of the item to get the visibility state for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property IsItemVisible(iIndex As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the value to reflect in the diagram for an item.
        ''' </summary>
        ''' <param name="iItem">The index of the item to get the value for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property Value(iItem As Integer) As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a formatted label for a <see cref="Value"/>.
        ''' </summary>
        ''' <param name="sValue">The value to format.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property ValueLabel(sValue As Single) As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the value to place on a link in the diagram.
        ''' </summary>
        ''' <param name="iPred">The index of the predatory source of the link.</param>
        ''' <param name="iPrey">The index of the prey target of the link.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property LinkValue(iPred As Integer, iPrey As Integer) As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the rank for placing an item in the diagram. In foodwebs, the
        ''' rank would typcally be the trophic level of a group or fleet.
        ''' </summary>
        ''' <param name="iItem">The index of the item to get the rank for.</param>
        ''' -------------------------------------------------------------------
        ReadOnly Property TrophicLevel(iItem As Integer) As Single

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
