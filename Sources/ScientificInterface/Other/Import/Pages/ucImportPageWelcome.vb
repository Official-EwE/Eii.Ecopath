#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.Wizard

#End Region ' Imports

Namespace Import

    ''' =======================================================================
    ''' <summary>
    ''' Import wizard welcome page.
    ''' </summary>
    ''' =======================================================================
    Public Class ucImportPageWelcome
        Implements IWizardPage

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the welcome page.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Init(ByVal wizard As cWizard, ByVal uic As cUIContext) _
            Implements IWizardPage.Init
            ' NOP
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close the welcome page.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Close() _
             Implements IWizardPage.Close
            ' NOP
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the welcome page is busy.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function IsBusy() As Boolean _
              Implements IWizardPage.IsBusy
            ' Page does not have a busy state
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the welcom page allows the wizard to navigate backward.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function AllowNavBack() As Boolean _
            Implements IWizardPage.AllowNavBack
            ' Page does not restrict backward navigation
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the welcom page allows the wizard to navigate backward.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function AllowNavForward() As Boolean _
            Implements IWizardPage.AllowNavForward
            ' Page does not restrict forward navigation
            Return True
        End Function

    End Class

End Namespace ' Import
