#Region " Imports "

Option Strict On
Imports EwECore.Database
Imports ScientificInterfaceShared.Controls.Wizard

#End Region ' Imports

Namespace Import

    ''' =======================================================================
    ''' <summary>
    ''' Import wizard model selection page.
    ''' </summary>
    ''' =======================================================================
    Public Class ucImportPageModels
        Implements IWizardPage

#Region " Private vars "

        ''' <summary>The attached wizard.</summary>
        Private m_wizard As cImportWizard = Nothing

#End Region ' Private vars

#Region " Interface "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the model selection page with the wizard content.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Init(ByVal wizard As cWizard) _
             Implements IWizardPage.Init

            ' Sanity checks
            Debug.Assert(TypeOf (wizard) Is cImportWizard)

            ' Remember wizard
            Me.m_wizard = DirectCast(wizard, cImportWizard)

            ' Show database name in header
            Me.m_hdr.SubText = Me.m_wizard.Database
            ' Initialize output folder path
            Me.m_tbxOutputFolder.Text = Me.m_wizard.OutputFolder

            ' Initialize the grid
            Me.m_grid.Init(Me.m_wizard)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close the model selection page.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Close() _
              Implements IWizardPage.Close
            ' Clean-up
            Me.m_wizard = Nothing
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the model selection page is busy.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function IsBusy() As Boolean _
              Implements IWizardPage.IsBusy
            ' Page does not have a 'busy' mode
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the model selection page allows the wizard to 
        ''' navigate backward.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function AllowNavBack() As Boolean _
            Implements IWizardPage.AllowNavBack
            ' No restrictions
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the model selection page allows the wizard to 
        ''' navigate forward.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function AllowNavForward() As Boolean _
            Implements IWizardPage.AllowNavForward

            ' Can only navigate forward if the current page settings allow 
            ' the wizard to import.
            Return Me.m_wizard.HasModelSelectedForImport() And _
                   Me.m_wizard.HasValidOutputPath()

        End Function

#End Region ' Interface

#Region " Events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Grid reports a user change.
        ''' </summary>
        ''' <param name="grid"></param>
        ''' -------------------------------------------------------------------
        Private Sub OnGridEdited(ByVal grid As cImportGrid) _
            Handles m_grid.OnEdited
            Me.m_wizard.PageChanged(Me)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wants to browse for an output 
        ''' folder.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnBrowsePath(ByVal sender As System.Object, _
                                  ByVal e As EventArgs) _
             Handles m_btnBrowse.Click

            Dim fd As New FolderBrowserDialog()
            fd.Description = "Select folder to place imported model(s) into"
            fd.ShowNewFolderButton = True
            fd.SelectedPath = Me.m_tbxOutputFolder.Text

            If fd.ShowDialog(Me) = DialogResult.OK Then
                Me.m_tbxOutputFolder.Text = fd.SelectedPath
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the output folder has been modified.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnOutputPathChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tbxOutputFolder.TextChanged
            Me.m_wizard.OutputFolder = Me.m_tbxOutputFolder.Text
            Me.m_wizard.PageChanged(Me)
        End Sub

#End Region ' Events

    End Class

End Namespace
