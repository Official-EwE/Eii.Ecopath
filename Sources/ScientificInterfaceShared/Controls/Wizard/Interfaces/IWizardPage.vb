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
        Sub Init(ByVal wizard As cWizard, ByVal uic As cUIContext)

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
        ''' States whether a page allows its parent wizard to navigate away to 
        ''' a previous page in the page chain.
        ''' </summary>
        ''' <remarks>
        ''' Note that pages should not try to make assumptions about their 
        ''' placement in the page chain. The parent wizard is responsible for 
        ''' handling beginning and end of page chains. This flag merely states
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Function AllowNavBack() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether a page allows its parent wizard to navigate forward
        ''' to a next page in the page chain.
        ''' </summary>
        ''' <remarks>
        ''' Note that pages should not try to make assumptions about their 
        ''' placement in the page chain. The parent wizard is responsible for 
        ''' handling beginning and end of page chains. This flag merely states
        ''' </remarks>
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
