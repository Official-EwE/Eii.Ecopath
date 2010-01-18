#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls.Wizard

    ''' =======================================================================
    ''' <summary>
    ''' Interface for building a wizard page.
    ''' </summary>
    ''' =======================================================================
    Public Interface IWizardPage

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize a page with the wizard content.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Sub Init(ByVal wizard As cWizard)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close a wizard page. The wizard is most likely navigating away
        ''' from this page. The page would do well to save its content to the
        ''' wizard at this point.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Sub Close()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether a page allows to navigate backward.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function AllowNavBack() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether a page allows to navigate forward.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function AllowNavForward() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether a page is busy.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function IsBusy() As Boolean

    End Interface

End Namespace ' Controls.Wizard
