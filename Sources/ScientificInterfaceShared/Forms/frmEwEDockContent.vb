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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Extensions
Imports ScientificInterfaceShared.Style
Imports WeifenLuo.WinFormsUI.Docking

#End Region ' Imports

Namespace Forms

    ''' =======================================================================
    ''' <summary>
    ''' <see cref="DockContent">DockContent</see>-derived
    ''' foundation class for EwE forms and panels, extending the Docking library by
    ''' adding a simple AutoHide toggle.
    ''' </summary>
    ''' =======================================================================
    Public Class frmEwEDockContent
        Inherits DockContent

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.TabText = Me.Text
        End Sub

        Protected Overrides Sub OnClosing(e As System.ComponentModel.CancelEventArgs)
            MyBase.OnClosing(e)
        End Sub

#End Region ' Overrides

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Panel categoy types.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum ePanelType As Byte
            ''' <summary>Panels flagged as Documents for the EwE MDI framework.</summary>
            Document = 0
            ''' <summary>Panels flagged as System Panels for the EwE MDI framework.
            ''' Close All Documents will not close this panels.</summary>
            SystemPanel = 1
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the auto-hide setting for a panel.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property AutoHide() As Boolean
            Get
                Return Me.IsHiding()
            End Get
            Set(value As Boolean)
                MyBase.DockState = Me.TranslateDockState(MyBase.DockState, value)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the DockState for a document without interrupting the current
        ''' hidden state.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shadows Property DockState() As DockState
            Get
                Return MyBase.DockState
            End Get
            Set(value As DockState)
                MyBase.DockState = Me.TranslateDockState(value, Me.IsHiding)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the dock content is currently hiding.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function IsHiding() As Boolean
            Return (Me.DockState = DockState.DockTopAutoHide) Or
                   (Me.DockState = DockState.DockBottomAutoHide) Or
                   (Me.DockState = DockState.DockLeftAutoHide) Or
                   (Me.DockState = DockState.DockRightAutoHide)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States the type of content that this form displays.
        ''' </summary>
        ''' <returns>Returns <see cref="ePanelType.Document"/> by default.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function PanelType() As ePanelType
            Return ePanelType.Document
        End Function

#End Region ' Public access

#Region " Privates "

        Private Function TranslateDockState(state As DockState, bHide As Boolean) As DockState
            Select Case state
                Case DockState.DockBottom
                    If bHide Then state = DockState.DockBottomAutoHide
                Case DockState.DockBottomAutoHide
                    If bHide Then state = DockState.DockBottom
                Case DockState.DockLeft
                    If bHide Then state = DockState.DockLeftAutoHide
                Case DockState.DockLeftAutoHide
                    If bHide Then state = DockState.DockLeft
                Case DockState.DockRight
                    If bHide Then state = DockState.DockRightAutoHide
                Case DockState.DockRightAutoHide
                    If bHide Then state = DockState.DockRight
                Case DockState.DockTop
                    If bHide Then state = DockState.DockTopAutoHide
                Case DockState.DockTopAutoHide
                    If bHide Then state = DockState.DockTop
            End Select
            Return state
        End Function

#End Region ' Internals

    End Class

End Namespace
