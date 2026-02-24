' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

' To enforce dilligent programming
Imports EwECore

''' ---------------------------------------------------------------------------
''' <summary>
''' Class that computes all MonteCarlo-based indicators.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cMCIndicators
    Inherits cEcosimIndicators

#Region " Private variables "

    Private m_iIteration As Integer = 0

#End Region ' Private variables

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new instance of this class.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore">Core</see> to operate onto.</param>
    ''' <param name="ecopathDS">The <see cref="cEcopathDataStructures">Ecopath data structures</see> to operate onto.</param>
    ''' <param name="stanzaDS">The <see cref="cStanzaDatastructures">Stanza data structures</see> to operate onto.</param>
    ''' <param name="taxonDS">The <see cref="cTaxonDataStructures">Taxonomy data structures</see> to operate onto.</param>
    ''' <param name="ecosimDS">The <see cref="cEcosimDatastructures">Ecosim data structures</see> to operate onto.</param>
    ''' <param name="iIter">The Monte Carlo iteration to calculate the indicators for.</param>
    ''' <param name="iTime">The Ecosim time to calculate the indicators for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore,
                    ecopathDS As cEcopathDataStructures,
                    ecosimDS As cEcosimDatastructures,
                    iIter As Integer,
                    iTime As Integer,
                    stanzaDS As cStanzaDatastructures,
                    taxonDS As cTaxonDataStructures,
                    lookup As cTaxonAnalysis)

        MyBase.New(core, ecopathDS, ecosimDS, iTime, stanzaDS, taxonDS, lookup)
        Me.m_iIteration = iIter

    End Sub

#End Region ' Constructor

#Region " Core data access and public bits "

    '''' -----------------------------------------------------------------------
    '''' <summary>
    '''' Helper function to access the Ecosim time that this indicator represents.
    '''' </summary>
    '''' <returns>The Ecosim time that these indicators represent.</returns>
    '''' -----------------------------------------------------------------------
    'Public Function Iteration() As Integer
    '    Return Me.m_iIteration
    'End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cIndicators.ModelTLCatch"/>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function ModelTLCatch() As Single
        Return Me.EcosimDS.TLC(Me.Time)
    End Function

#End Region ' Core data access and public bits

End Class
