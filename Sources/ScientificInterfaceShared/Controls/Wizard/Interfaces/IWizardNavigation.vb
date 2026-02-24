' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Wizard

    ''' =======================================================================
    ''' <summary>
    ''' Interface for building a wizard navigation structure.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(True)>
    Public Interface IWizardNavigation

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attach a wizard to a navigation structure
        ''' </summary>
        ''' <param name="wizard">The wizard to attach.</param>
        ''' -------------------------------------------------------------------
        Sub Attach(wizard As cWizard)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Detach the current wizard from a navigation structure
        ''' </summary>
        ''' -------------------------------------------------------------------
        Sub Detach()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Wizard callback, informing the navigation structure to update
        ''' itself.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Sub UpdateNavigation()

    End Interface

End Namespace ' Controls.Wizard
