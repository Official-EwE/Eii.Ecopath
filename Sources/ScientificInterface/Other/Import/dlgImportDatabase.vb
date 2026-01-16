' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

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