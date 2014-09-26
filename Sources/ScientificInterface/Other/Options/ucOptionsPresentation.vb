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

Imports System.IO
Imports WeifenLuo.WinFormsUI
Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Presentations settings interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsPresentation
        Implements IOptionsPage

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_cbHideModelBar.Checked = My.Settings.PresentationModeHideModelBar
            Me.m_cbHideMainMenu.Checked = My.Settings.PresentationModeHideMainMenu
            Me.m_cbHideStatusBar.Checked = My.Settings.PresentationModeHideStatusBar
            Me.m_cbCollapseNavPanel.Checked = My.Settings.PresentationModeCollapseNavPanel

            Me.UpdateControls()

        End Sub

#End Region ' Overrides

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanApply"/>
        ''' -------------------------------------------------------------------
        Public Function CanApply() As Boolean _
              Implements IOptionsPage.CanApply
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.OnChanged"/>
        ''' -------------------------------------------------------------------
        Public Event OnOptionsPresentationChanged(sender As IOptionsPage, args As System.EventArgs) _
               Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
             Implements IOptionsPage.Apply

            If Not Me.CanApply Then Return IOptionsPage.eApplyResultType.Failed

            My.Settings.PresentationModeHideModelBar = Me.m_cbHideModelBar.Checked
            My.Settings.PresentationModeHideMainMenu = Me.m_cbHideMainMenu.Checked
            My.Settings.PresentationModeHideStatusBar = Me.m_cbHideStatusBar.Checked
            My.Settings.PresentationModeCollapseNavPanel = Me.m_cbCollapseNavPanel.Checked

            Return IOptionsPage.eApplyResultType.Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() _
                 Implements IOptionsPage.SetDefaults

            Try
                Me.m_cbHideModelBar.Checked = CBool(My.Settings.GetDefaultValue("PresentationModeHideModelBar"))
                Me.m_cbHideMainMenu.Checked = CBool(My.Settings.GetDefaultValue("PresentationModeHideMainMenu"))
                Me.m_cbHideStatusBar.Checked = CBool(My.Settings.GetDefaultValue("PresentationModeHideStatusBar"))
                Me.m_cbCollapseNavPanel.Checked = CBool(My.Settings.GetDefaultValue("PresentationModeCollapseNavPanel"))
            Catch ex As Exception
                cLog.Write(ex, "ucOptionsPresentation::SetDefaults")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanSetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Function CanSetDefaults() As Boolean _
            Implements IOptionsPage.CanSetDefaults
            Return True
        End Function

#End Region ' Public access

#Region " Internals "

        Private Sub UpdateControls()

        End Sub

#End Region ' Internals

    End Class

End Namespace
