' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Import

    ''' ===========================================================================
    ''' <summary>
    ''' Import database dialog.
    ''' </summary>
    ''' ===========================================================================
    Public Class dlgImportDatabase

#Region " Private vars "

        Private m_wizard As cImportWizard = Nothing

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uic">UI context to connect to.</param>
        ''' <param name="strSource">Source of to the database to import.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext, strSource As String)

            Me.InitializeComponent()
            Me.m_wizard = New cImportWizard(uic, strSource, Me, Me.m_plWizardContent, Me.m_navigator)

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