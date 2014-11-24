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

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecopath.Controls.FlowDiagram

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data for rendering the Ecopath groups and trophic links as a flow diagram.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cFlowDiagramGroupData
        Implements IFlowDiagramData

#Region " Internals "

        Private m_uic As cUIContext = Nothing
        Private m_sDietMin As Single = 0
        Private m_sDietMax As Single = 0
        Private m_sBiomassMin As Single = 0
        Private m_sBiomassMax As Single = 0

        Private m_bInvalid As Boolean = True

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
            Get
                Return Me.m_uic
            End Get
            Private Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.Refresh"/>
        ''' -------------------------------------------------------------------
        Public Sub Refresh() _
            Implements IFlowDiagramData.Refresh
            Me.m_bInvalid = True
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.NumGroups"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumGroups() As Integer _
              Implements IFlowDiagramData.NumGroups
            Get
                Return Me.m_uic.Core.nGroups
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.NumLivingGroups"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumLivingGroups() As Integer _
                Implements IFlowDiagramData.NumLivingGroups
            Get
                Return Me.m_uic.Core.nLivingGroups
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.Value"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Biomass(ByVal iIndex As Integer) As Single _
               Implements IFlowDiagramData.Value
            Get
                Return Me.m_uic.Core.EcoPathGroupOutputs(iIndex).Biomass
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
        ''' <inheritdocs cref="IFlowDiagramData.GroupName"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GroupName(ByVal iIndex As Integer) As String _
                Implements IFlowDiagramData.GroupName
            Get
                Return Me.m_uic.Core.EcoPathGroupInputs(iIndex).Name
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.GroupColor"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GroupColor(ByVal iGroup As Integer) As Color _
                Implements IFlowDiagramData.GroupColor
            Get
                Return Me.m_uic.StyleGuide.GroupColor(Me.m_uic.Core, iGroup)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.IsGroupVisible"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsGroupVisible(ByVal iGroup As Integer) As Boolean _
                Implements IFlowDiagramData.IsGroupVisible
            Get
                Return Me.m_uic.StyleGuide.GroupVisible(iGroup)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.LinkValue"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Diet(ByVal iPred As Integer, ByVal iPrey As Integer) As Single _
               Implements IFlowDiagramData.LinkValue
            Get
                Dim group As cEcoPathGroupInput = Me.m_uic.Core.EcoPathGroupInputs(iPred)
                Return group.DietComp(iPrey)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IFlowDiagramData.TrophicLevel"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property TrophicLevel(ByVal iIndex As Integer) As Single _
                Implements IFlowDiagramData.TrophicLevel
            Get
                Return Me.m_uic.Core.EcoPathGroupOutputs(iIndex).TTLX
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

            Me.m_sBiomassMax = 0
            Me.m_sBiomassMin = Single.MaxValue
            Me.m_sDietMax = 0
            Me.m_sDietMin = Single.MaxValue

            For i As Integer = 1 To Me.NumGroups
                For j As Integer = 1 To Me.NumGroups
                    Dim sDiet As Single = Me.Diet(i, j)
                    Me.m_sDietMax = Math.Max(Me.m_sDietMax, sDiet)
                    Me.m_sDietMin = Math.Min(Me.m_sDietMin, sDiet)

                    Dim sB As Single = Me.Biomass(i)
                    Me.m_sBiomassMax = Math.Max(Me.m_sBiomassMax, sDiet)
                    Me.m_sBiomassMin = Math.Min(Me.m_sBiomassMin, sDiet)
                Next j
            Next i

            Me.m_bInvalid = False

        End Sub

#End Region ' Interals

    End Class

End Namespace
