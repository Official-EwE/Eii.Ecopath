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
    ''' Form implementing the MSE Group Reference Levels interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmGroupRefLevels

        Public Sub New()
            MyBase.New(New gridGroupRefLevels())
            Me.InitializeComponent()
        End Sub

        Private Sub OnReset(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnReset.Click
            Me.Core.ResetMSEGroupRefLevels()
        End Sub

    End Class

End Namespace
