' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports 

Namespace Ecospace

    Public Class frmEcospaceMPAEnforcement

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)

            Dim cmd As cCommand = Nothing
            MyBase.OnLoad(e)

            If (Me.CommandHandler Is Nothing) Then Return

            cmd = Me.CommandHandler.GetCommand("EditMPAs")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnDefineMPAs)

            Me.m_tsbnDefineMPAs.Image = SharedResources.MPA
            Me.m_lblInfo.Image = SharedResources.Info

        End Sub

        Private Sub m_lblInfo_Click(sender As Object, e As EventArgs) Handles m_lblInfo.Click
            Me.m_lblInfo.Visible = False
        End Sub

    End Class

End Namespace
