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

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecosim 'Apply Mediation to Primary Producer' 
    ''' interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmApplyFFDetritus
        Inherits frmApplyShapeBase

#Region " Constructor "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        
        Protected Overrides ReadOnly Property Grid() As gridApplyShapeBase
            Get
                Return Me.m_grid
            End Get
        End Property

#End Region

#Region " Event handlers "

        Private Sub tsBtnClearAll_Click(sender As System.Object, e As System.EventArgs) _
            Handles tsBtnClearAll.Click
            Me.ClearAll()
        End Sub

        Private Sub tsBtnSetAll_Click(sender As System.Object, e As System.EventArgs) _
            Handles tsBtnSetAll.Click
            Me.SetAll()
        End Sub

#End Region ' Event handlers

    End Class

End Namespace
