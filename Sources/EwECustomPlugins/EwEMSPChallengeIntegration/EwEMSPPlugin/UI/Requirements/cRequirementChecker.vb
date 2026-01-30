' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore



Namespace UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Base class for validating aspects of the EwE model for suitability for MSP
    ''' gameplay.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public MustInherit Class cRequirementChecker

        ''' <summary>The <see cref="cCore"/> instance to check.</summary>
        Protected m_core As cCore = Nothing

        ''' <summary>Cached result of last requirement check.</summary>
        Private m_bIsMet As Boolean = False

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="cRequirementChecker"/> class.
        ''' </summary>
        ''' <param name="core">The core to check.</param>
        ''' ---------------------------------------------------------------------------
        Public Sub New(core As cCore)
            Me.m_core = core
            Me.CheckRequirements()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Core message handler, implemented to automatically trigger a requirement 
        ''' check when the user changes the relevant configuration of EwE.
        ''' </summary>
        ''' <param name="msg">The message to respond to.</param>
        ''' ---------------------------------------------------------------------------
        Public MustOverride Sub OnCoreMessage(msg As cMessage)

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' The validation logic, which should set the value of <see cref="RequirementsMet"/>.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected MustOverride Sub CheckRequirements()

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the requirements are met.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public Property RequirementsMet As Boolean
            Get
                Return Me.m_bIsMet
            End Get
            Protected Set(value As Boolean)
                Me.m_bIsMet = value
            End Set
        End Property

    End Class

End Namespace
