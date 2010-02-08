#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Database
Imports EwEUtils.Database

#End Region ' Imports

Namespace Import

    ''' ===========================================================================
    ''' <summary>
    ''' Import database dialog.
    ''' </summary>
    ''' ===========================================================================
    Public Class dlgImportDatabase

#Region " Private vars "

        Private m_wizard As cImportWizard = Nothing
        ''' <summary>The import log file.</summary>
        Private m_strLogFileName As String = ""

#End Region ' Private vars

        Public Sub New(ByVal uic As cUIContext, ByVal db As cEwEDatabase)

            Me.InitializeComponent()
            Me.m_wizard = New cImportWizard(uic, db, Me, Me.m_plWizardContent, Me.m_navigator)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the file name that was last selected.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ImportedFileName() As String
            Get
                Return Me.m_wizard.Filename
            End Get
        End Property

    End Class

End Namespace