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
Namespace Controls

    Public Interface IFlowDiagramData
        Inherits IUIElement

        Sub Refresh()

        ReadOnly Property RenderFont() As Font

        ReadOnly Property TextColor() As Color

        ReadOnly Property GroupColor(ByVal iGroup As Integer) As Color
        ReadOnly Property PreyColor() As Color
        ReadOnly Property PredatorColor() As Color

        ReadOnly Property NumGroups() As Integer
        ReadOnly Property NumLivingGroups() As Integer

        ReadOnly Property Biomass(ByVal iIndex As Integer) As Single
        ReadOnly Property BiomassLabel(ByVal sBiomass As Single) As String

        ReadOnly Property GroupName(ByVal iIndex As Integer) As String
        ReadOnly Property GroupVisible(ByVal iGroup As Integer) As Boolean

        ReadOnly Property Diet(ByVal iPred As Integer, ByVal iPrey As Integer) As Single

        ReadOnly Property TrophicLevel(ByVal iIndex As Integer) As Single

        ReadOnly Property BiomassMax() As Single
        ReadOnly Property BiomassMin() As Single

        ReadOnly Property DietMin() As Single
        ReadOnly Property DietMax() As Single

    End Interface

End Namespace
