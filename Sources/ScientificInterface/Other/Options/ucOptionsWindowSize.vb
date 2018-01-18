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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
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
    ''' User control; implements the Options > Window Size settings interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsWindowSize
        Implements IOptionsPage
        Implements IUIElement

        Private m_fpW As cEwEFormatProvider = Nothing
        Private m_fpH As cEwEFormatProvider = Nothing
        Private m_szFrame As Size
        Private m_bInUpdate As Boolean = False

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.UIContext = uic
            Me.InitializeComponent()

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

            Dim frm As Form = Me.UIContext.FormMain
            Dim szOut As Size = frm.Size
            Dim szIn As Size = frm.ClientRectangle.Size

            Me.m_szFrame = New Size(szOut.Width - szIn.Width, szOut.Height - szIn.Height)

            Me.m_fpW = New cEwEFormatProvider(Me.UIContext, Me.m_tbxW, GetType(Integer))
            Me.m_fpH = New cEwEFormatProvider(Me.UIContext, Me.m_tbxH, GetType(Integer))
            Me.m_fpW.Value = szOut.Width
            Me.m_fpH.Value = szOut.Height

            Me.m_bInUpdate = True
            Me.m_rbOut.Checked = True
            Me.m_bInUpdate = False

        End Sub

#End Region ' Event handlers

#Region " Public methods "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext As cUIContext _
                 Implements IUIElement.UIContext

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
        Public Event OnOptionChanged(sender As IOptionsPage, args As System.EventArgs) _
              Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            If Not Me.CanApply Then Return IOptionsPage.eApplyResultType.Failed

            Try

                Me.UIContext.FormMain.Size = Me.OuterSize

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex, "ucOptionsWindowSize::Apply")
            End Try

            Return IOptionsPage.eApplyResultType.Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() _
                Implements IOptionsPage.SetDefaults

            Try
                Me.m_rbIn.Checked = True
            Catch ex As Exception

            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanSetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Function CanSetDefaults() As Boolean _
            Implements IOptionsPage.CanSetDefaults
            Return True
        End Function

#End Region ' Public methods

#Region " Internals "

        Private Function OuterSize() As Size

            Dim w As Integer = CInt(Me.m_fpW.Value)
            Dim h As Integer = CInt(Me.m_fpH.Value)

            If (Me.m_rbIn.Checked) Then
                w += Me.m_szFrame.Width
                h += Me.m_szFrame.Height
            End If

            Return New Size(w, h)

        End Function

#End Region  ' Internals

        Private Sub OnSizeModeToggled(sender As System.Object, e As System.EventArgs) _
            Handles m_rbOut.CheckedChanged

            If (Me.m_bInUpdate) Then Return

            Dim w As Integer = CInt(Me.m_fpW.Value)
            Dim h As Integer = CInt(Me.m_fpH.Value)

            If Me.m_rbOut.Checked Then
                w += Me.m_szFrame.Width
                h += Me.m_szFrame.Height
            Else
                w -= Me.m_szFrame.Width
                h -= Me.m_szFrame.Height
            End If

            Me.m_fpW.Value = w
            Me.m_fpH.Value = h

        End Sub

    End Class

End Namespace


