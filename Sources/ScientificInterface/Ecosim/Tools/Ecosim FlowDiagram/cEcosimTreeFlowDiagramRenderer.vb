' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel



Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, implements the actual display of an EwE flow diagram using
    ''' a simple layout of nodes connected via arched lines.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEcosimTreeFlowDiagramRenderer
        Inherits cTreeFlowDiagramRenderer

#Region " Private vars "

        Private m_tsShowBiomassLegend As Boolean = False
        Private m_tsShowFlowRateLegend As Boolean = False

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(data As IFlowDiagramData)
            MyBase.New(data)
        End Sub

#End Region ' Constructor

#Region " Properties "

        Public Event OnBiomassLegendChanged(sender As cTreeFlowDiagramRenderer)
        Public Event OnFlowRateLegendChanged(sender As cTreeFlowDiagramRenderer)

        <Browsable(True),
            Category("Appearance"),
            cLocalizedDisplayName("GENERIC_SHOW_BIOMASS_LEGEND"),
            DefaultValue(False)>
        Public Property ShowBiomassLegend As Boolean
            Get
                Return Me.m_tsShowBiomassLegend
            End Get
            Set(value As Boolean)
                If (value <> Me.m_tsShowBiomassLegend) Then
                    Me.m_tsShowBiomassLegend = value
                    RaiseEvent OnBiomassLegendChanged(Me)
                    Me.Update()
                End If
            End Set
        End Property

        <Browsable(True),
            Category("Appearance"),
            cLocalizedDisplayName("GENERIC_SHOW_FLOW_RATE_LEGEND"),
            DefaultValue(TriState.False)>
        Public Property ShowFlowRateLegend As Boolean
            Get
                Return Me.m_tsShowFlowRateLegend
            End Get
            Set(value As Boolean)
                If (value <> Me.m_tsShowFlowRateLegend) Then
                    Me.m_tsShowFlowRateLegend = value
                    RaiseEvent OnFlowRateLegendChanged(Me)
                    Me.Update()
                End If
            End Set
        End Property

#End Region ' Properties

    End Class

End Namespace