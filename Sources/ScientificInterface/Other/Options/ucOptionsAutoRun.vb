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
Imports System.IO
Imports EwECore
Imports ScientificInterfaceShared.Commands
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > file management interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsAutoRun
        Implements IOptionsPage
        Implements IUIElement

#Region " Private vars "

        Private m_cbh As cCheckboxHierarchy = Nothing
        Private m_autorunoptions As cAutoRunItemEngine = Nothing

#End Region ' Private vars

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)
            Me.UIContext = uic
            Me.InitializeComponent()

            ' Autosave
            Me.m_autorunoptions = New cAutoRunItemEngine(Me.UIContext)
            Me.m_autorunoptions.Attach(Me.m_plAutorun)

        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
                Me.m_autorunoptions.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

        End Sub

#End Region ' Overrides

#Region " interface implementation "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.OnChanged"/>
        ''' -------------------------------------------------------------------
        Public Event OnOptionsFileManagementChanged(sender As IOptionsPage, args As System.EventArgs) _
            Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanApply"/>
        ''' -------------------------------------------------------------------
        Public Function CanApply() As Boolean _
            Implements IOptionsPage.CanApply
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            If Not Me.CanApply Then Return IOptionsPage.eApplyResultType.Failed

            Try

                Me.m_autorunoptions.Apply()

            Catch ex As Exception
                cLog.Write(ex, "ucOptionsAutosave::Apply")
                Return IOptionsPage.eApplyResultType.Failed
            End Try

            Return IOptionsPage.eApplyResultType.Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() Implements IOptionsPage.SetDefaults

            Try
            Catch ex As Exception
                cLog.Write(ex, "ucOptionsAutosave::SetDefaults")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanSetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Function CanSetDefaults() As Boolean _
            Implements IOptionsPage.CanSetDefaults
            Return True
        End Function

#End Region

    End Class

End Namespace
