#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls.Wizard

    ''' =======================================================================
    ''' <summary>
    ''' Interface for building a wizard navigation structure.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(True)> _
    Public Interface IWizardNavigation

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attach a wizard to a navigation structure
        ''' </summary>
        ''' <param name="wizard">The wizard to attach.</param>
        ''' -------------------------------------------------------------------
        Sub Attach(ByVal wizard As cWizard)

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
