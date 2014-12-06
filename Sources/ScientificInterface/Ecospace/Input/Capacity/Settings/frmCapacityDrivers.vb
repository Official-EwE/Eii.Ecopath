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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands

#End Region

Namespace Ecospace

    Public Class frmCapacityDrivers

        Public Sub New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            If (Me.UIContext Is Nothing) Then Return
            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cEditDriverLayersCommand.cCOMMAND_NAME)
            cmd.AddControl(Me.m_tsbnDefineDriverLayers)
        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            If (Me.UIContext Is Nothing) Then Return
            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cEditDriverLayersCommand.cCOMMAND_NAME)
            cmd.RemoveControl(Me.m_tsbnDefineDriverLayers)
            MyBase.OnFormClosed(e)
        End Sub

    End Class

End Namespace
