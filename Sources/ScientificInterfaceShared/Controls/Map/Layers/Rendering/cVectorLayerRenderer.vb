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
Imports EwECore
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for rendering a Scientific Interface <see cref="cLayer">layer</see>
    ''' onto the base map.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cVectorLayerRenderer
        Inherits cLayerRenderer

#Region " Construction / destruction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vs"></param>
        ''' <param name="layerStyleFlags"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal vs As cVisualStyle, _
                       Optional ByVal layerStyleFlags As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet)
            MyBase.New(vs, layerStyleFlags)
        End Sub

#End Region ' Construction / destruction

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Render a cell of a layer.
        ''' </summary>
        ''' <param name="g">The graphics to render onto.</param>
        ''' <param name="rc">Device area to render cell onto.</param>
        ''' -----------------------------------------------------------------------
        Public MustOverride Sub Render(ByVal g As Graphics, _
                                       ByVal rc As Rectangle, _
                                       ByVal ptfTL As PointF, _
                                       ByVal ptBR As PointF, _
                                       ByVal style As cStyleGuide.eStyleFlags)

    End Class

End Namespace
