#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.Wizard

#End Region ' Imports

Namespace Import

    ''' =======================================================================
    ''' <summary>
    ''' Import wizard page 1 - welcome.
    ''' </summary>
    ''' =======================================================================
    Public Class ucImportPageWelcome
        Implements IWizardPage

        Public Sub Init(ByVal wizard As ScientificInterfaceShared.Controls.Wizard.cWizard) _
            Implements ScientificInterfaceShared.Controls.Wizard.IWizardPage.Init
            ' NOP
        End Sub

        Public Sub Close() _
            Implements ScientificInterfaceShared.Controls.Wizard.IWizardPage.Close
            ' NOP
        End Sub

        Public Function IsBusy() As Boolean _
            Implements ScientificInterfaceShared.Controls.Wizard.IWizardPage.IsBusy
            ' Page is never too busy to be dismissed
            Return False
        End Function

        Public Function AllowNavBack() As Boolean _
            Implements ScientificInterfaceShared.Controls.Wizard.IWizardPage.AllowNavBack
            ' Page does not restrict backward navigation
            Return True
        End Function

        Public Function AllowNavForward() As Boolean _
            Implements ScientificInterfaceShared.Controls.Wizard.IWizardPage.AllowNavForward
            ' Page does not restrict forward navigation
            Return True
        End Function

    End Class

End Namespace ' Import
