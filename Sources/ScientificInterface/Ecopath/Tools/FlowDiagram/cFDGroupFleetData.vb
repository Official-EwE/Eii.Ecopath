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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecopath.Controls.FlowDiagram

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data for rendering the Ecopath groups, fleets, and trophic links as a 
    ''' flow diagram.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cFlowDiagramFleetGroupData
        Implements IFlowDiagramData

#Region " Internals "

        Private m_sDietMin As Single = 0
        Private m_sDietMax As Single = 0
        Private m_sBiomassMin As Single = 0
        Private m_sBiomassMax As Single = 0

        Private m_bInvalid As Boolean = True

        Private m_TTLX_all() As Single

#End Region ' Internals

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            Me.UIContext = uic
        End Sub

#End Region ' Constructor

#Region " Properties "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.UIContext"/>
        ''' -------------------------------------------------------------------
        Friend Property UIContext() As cUIContext _
            Implements IFlowDiagramData.UIContext

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.Refresh"/>
        ''' -------------------------------------------------------------------
        Public Sub Refresh() _
            Implements IFlowDiagramData.Refresh
            Me.m_bInvalid = True
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.NumItems"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumGroups() As Integer _
              Implements IFlowDiagramData.NumItems
            Get
                Dim c As cCore = Me.UIContext.Core
                Return c.nGroups + c.nFleets
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.NumLivingitems"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumLivingGroups() As Integer _
                Implements IFlowDiagramData.NumLivingitems
            Get
                Dim c As cCore = Me.UIContext.Core
                Return c.nLivingGroups + c.nFleets
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.Value"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Biomass(ByVal iIndex As Integer) As Single _
               Implements IFlowDiagramData.Value
            Get
                Dim c As cCore = Me.UIContext.Core
                If (iIndex <= c.nGroups) Then Return c.EcoPathGroupOutputs(iIndex).Biomass
                iIndex -= c.nGroups
                Dim b As Single = 0
                For i As Integer = 1 To c.nGroups
                    b += c.FleetInputs(iIndex).Landings(i) + c.FleetInputs(iIndex).Discards(i)
                Next
                Return b
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.ValueLabel"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property BiomassLabel(sBiomass As Single) As String _
              Implements IFlowDiagramData.ValueLabel
            Get
                Return cStringUtils.Localize(My.Resources.FLOWDIAGRAM_LABEL_BIOMASS, Me.UIContext.StyleGuide.FormatNumber(sBiomass, cStyleGuide.eStyleFlags.OK))
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.ItemName(Integer)"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GroupName(ByVal iIndex As Integer) As String _
                Implements IFlowDiagramData.ItemName
            Get
                Dim c As cCore = Me.UIContext.Core
                If (iIndex <= c.nGroups) Then Return c.EcoPathGroupInputs(iIndex).Name
                iIndex -= c.nGroups
                Return c.FleetInputs(iIndex).Name
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.ItemColor(Integer)"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GroupColor(ByVal iIndex As Integer) As Color _
                Implements IFlowDiagramData.ItemColor
            Get
                Dim c As cCore = Me.UIContext.Core
                Dim sg As cStyleGuide = Me.UIContext.StyleGuide
                If (iIndex <= c.nGroups) Then Return sg.GroupColor(c, iIndex)
                iIndex -= c.nGroups
                Return sg.FleetColor(c, iIndex)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.IsItemVisible(Integer)"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsGroupVisible(ByVal iIndex As Integer) As Boolean _
                Implements IFlowDiagramData.IsItemVisible
            Get
                Dim c As cCore = Me.UIContext.Core
                Dim sg As cStyleGuide = Me.UIContext.StyleGuide
                If (iIndex <= c.nGroups) Then Return sg.GroupVisible(iIndex)
                iIndex -= c.nGroups
                Return sg.FleetVisible(iIndex)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.LinkValue"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Diet(ByVal iPred As Integer, ByVal iPrey As Integer) As Single _
               Implements IFlowDiagramData.LinkValue
            Get
                Dim c As cCore = Me.UIContext.Core
                If (iPred <= c.nGroups) And (iPrey <= c.nGroups) Then
                    Dim group As cEcoPathGroupInput = c.EcoPathGroupInputs(iPred)
                    Return group.DietComp(iPrey)
                End If
                iPred -= c.nGroups
                If (iPrey <= c.nGroups) Then
                    Dim fleet As cFleetInput = c.FleetInputs(iPred)
                    Return fleet.Landings(iPrey) + fleet.Discards(iPrey)
                End If
                Return 0
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.TrophicLevel"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property TrophicLevel(ByVal iIndex As Integer) As Single _
                Implements IFlowDiagramData.TrophicLevel
            Get
                Dim c As cCore = Me.UIContext.Core
                If (iIndex <= c.nGroups) Then Return c.EcoPathGroupOutputs(iIndex).TTLX
                iIndex -= c.nGroups

                Me.Recalc()
                Return Me.m_TTLX_all(iIndex)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.ValueMax"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property BiomassMax() As Single _
                Implements IFlowDiagramData.ValueMax
            Get
                If Me.m_bInvalid Then Me.Recalc()
                Return Me.m_sBiomassMax
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.ValueMin"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property BiomassMin() As Single _
               Implements IFlowDiagramData.ValueMin
            Get
                If Me.m_bInvalid Then Me.Recalc()
                Return Me.m_sBiomassMin
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.LinkValueMin"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property DietMin() As Single _
                 Implements IFlowDiagramData.LinkValueMin
            Get
                If Me.m_bInvalid Then Me.Recalc()
                Return Me.m_sDietMin
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.LinkValueMax"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property DietMax() As Single _
                  Implements IFlowDiagramData.LinkValueMax
            Get
                If Me.m_bInvalid Then Me.Recalc()
                Return Me.m_sDietMax
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.Title"/>
        ''' -------------------------------------------------------------------
        Public Property Title As String _
            Implements IFlowDiagramData.Title

#End Region ' Properties

#Region " Internals "

        Private Sub Recalc()

            If Not Me.m_bInvalid Then Return
            If (Me.UIContext Is Nothing) Then Return

            Dim c As cCore = Me.UIContext.Core
            Dim DC_all(c.nGroups + c.nFleets, c.nGroups + c.nFleets) As Single
            Dim B_all(c.nGroups + c.nFleets) As Single
            Dim PP_all(c.nGroups + c.nFleets) As Single
            ReDim Me.m_TTLX_all(c.nGroups + c.nFleets)

            Me.m_sBiomassMax = 0
            Me.m_sBiomassMin = Single.MaxValue
            Me.m_sDietMax = 0
            Me.m_sDietMin = Single.MaxValue

            For i As Integer = 1 To c.nGroups
                For j As Integer = 1 To c.nGroups
                    Dim sDiet As Single = Me.Diet(i, j)
                    Me.m_sDietMax = Math.Max(Me.m_sDietMax, sDiet)
                    Me.m_sDietMin = Math.Min(Me.m_sDietMin, sDiet)
                    DC_all(c.nFleets + i, c.nFleets + j) = sDiet

                    Dim sB As Single = Me.Biomass(i)
                    Me.m_sBiomassMax = Math.Max(Me.m_sBiomassMax, sDiet)
                    Me.m_sBiomassMin = Math.Min(Me.m_sBiomassMin, sDiet)
                    B_all(c.nFleets + i) = sB
                Next j
                PP_all(c.nFleets + i) = c.EcoPathGroupInputs(i).PP
            Next i

            For i As Integer = 1 To c.nFleets
                For j As Integer = 1 To c.nGroups
                    Dim sDiet As Single = Me.Diet(i + c.nGroups, j)
                    Me.m_sDietMax = Math.Max(Me.m_sDietMax, sDiet)
                    Me.m_sDietMin = Math.Min(Me.m_sDietMin, sDiet)
                    DC_all(i, c.nFleets + j) = sDiet

                    Dim sB As Single = Me.Biomass(i + c.nGroups)
                    Me.m_sBiomassMax = Math.Max(Me.m_sBiomassMax, sDiet)
                    Me.m_sBiomassMin = Math.Min(Me.m_sBiomassMin, sDiet)
                    B_all(i) = sB
                Next j
                ' Just to be explicit: fleets are full-on consumers
                PP_all(i) = 0
            Next i

            Dim fn As cEcoFunctions = c.EcoFunction
            fn.EstimateTrophicLevels(c.nFleets + c.nGroups, c.nFleets + c.nLivingGroups, PP_all, DC_all, m_TTLX_all)
            Me.m_bInvalid = False

        End Sub

#End Region ' Interals

    End Class

End Namespace
