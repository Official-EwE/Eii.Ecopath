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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports System.IO
Imports EwEUtils.Core

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Map settings interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsPedigree
        Implements IOptionsPage

#Region " Variables "

        Private m_uic As cUIContext = Nothing

#End Region ' Variables

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.m_uic = uic
            Me.InitializeComponent()

            If (Me.m_uic IsNot Nothing) Then
                Dim sg As cStyleGuide = Me.m_uic.StyleGuide
                Me.m_cbShowPedigreeIndicators.Checked = sg.ShowPedigree
            End If

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Control's load event which gets called every time the control gets loaded. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)

            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)

        End Sub

#End Region ' Event handlers

#Region " Public methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save colour selections back to the style guide.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide

            ' Apply colors to the style guide
            sg.SuspendEvents()

            Try
                sg.ShowPedigree = Me.m_cbShowPedigreeIndicators.Checked
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex, "ucOptionsPedigree::Apply")
            End Try

            sg.ResumeEvents()
            Return IOptionsPage.eApplyResultType.Success

        End Function

        Public Sub SetDefaults() _
            Implements IOptionsPage.SetDefaults

            Try
                Me.m_cbShowPedigreeIndicators.Checked = CBool(My.Settings.GetDefaultValue("ShowPedigree"))
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Public methods

    End Class

End Namespace


