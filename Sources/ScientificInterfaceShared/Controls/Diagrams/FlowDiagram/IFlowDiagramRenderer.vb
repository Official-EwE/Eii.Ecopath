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

Imports ScientificInterfaceShared.Definitions

Namespace Controls

    ' ToDo_JS: document this interface

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base interface for drawing a specific type of flow diagram.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IFlowDiagramRenderer

        Enum eFDHighlightType As Integer
            None
            Selected
            LinkIn
            LinkOut
            GrayedOut
            Invisible
        End Enum

        ''' <summary>Draw the background of the flow diagram. Here trophic level lines etc. should be rendered.</summary>
        ''' <param name="g"></param>
        ''' <param name="rc"></param>
        ''' <param name="bTransparentBackground">Flag, stating if the backgroung should be transparent or solid</param>
        Sub DrawBackground(g As Graphics, rc As Rectangle, bTransparentBackground As Boolean)
        Sub DrawTitle(g As Graphics, rc As Rectangle)
        Sub DrawNode(g As Graphics, rc As Rectangle, iGroup As Integer, highlight As eFDHighlightType)
        Sub DrawConnection(g As Graphics, rc As Rectangle, iPred As Integer, iPrey As Integer, highlight As eFDHighlightType)
        Sub DrawLegend(g As Graphics, ptTopLeft As Point)

        Function RenderFont() As Font
        Function TextColor() As Color
        Function InLinkColor() As Color
        Function OutLinkColor() As Color
        Function HighlightColor() As Color
        Function FormatLabelText(iGroup As Integer) As String
        Property NodeLocation(i As Integer, rc As Rectangle) As PointF
        Property LabelLocation(i As Integer, rc As Rectangle) As PointF
        Property ShowHiddenNodes As eFDShowHiddenType

        Sub MoveNode(rc As Rectangle, ptNew As PointF, iNode As Integer)
        Sub MoveLabel(rc As Rectangle, ptNew As PointF, iNode As Integer)
        Function IsNodeAtPoint(rc As Rectangle, ptfTest As PointF, i As Integer, sValue As Single) As Boolean
        Function IsLabelAtPoint(rc As Rectangle, ptfTest As PointF, i As Integer, strLabel As String, g As Graphics, font As Font) As Boolean

    End Interface

End Namespace