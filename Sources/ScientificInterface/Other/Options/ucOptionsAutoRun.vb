' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > file management interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsAutoRun
        Implements IOptionsPage
        Implements IUIElement

        Private m_qeh As cQuickEditHandler = Nothing
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of ucOptionsAutoRun)()

#Region " Constructors "

        Public Sub New(uic As cUIContext)
            Me.UIContext = uic
            Me.InitializeComponent()

            ' Autosave
            Me.m_grid.UIContext = uic

            Me.m_qeh = New cQuickEditHandler()
            Me.m_qeh.Attach(Me.m_grid, uic, Me.m_tsQuickEdit, False)

        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso Me.components IsNot Nothing Then
                Me.m_grid.UIContext = Nothing
                Me.m_qeh.Detach()
                Me.components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.m_tsbnQuickHelp.Image = SharedResources.Info
            Me.m_lblInfo.Visible = False
        End Sub

#End Region ' Overrides

#Region " Events "

        Private Sub OnShowQuickHelp(sender As Object, e As EventArgs) Handles m_tsbnQuickHelp.MouseDown
            Me.m_lblInfo.Visible = True
        End Sub

        Private Sub OnHideQuickHelp(sender As Object, e As EventArgs) Handles m_tsbnQuickHelp.MouseUp
            Me.m_lblInfo.Visible = False
        End Sub

        Private Sub OnSelectAll(sender As Object, e As EventArgs) Handles m_tsbnAll.Click
            Me.m_grid.CheckAll()
        End Sub

        Private Sub OnSelectNone(sender As Object, e As EventArgs) Handles m_tsbnNone.Click
            Me.m_grid.ClearAll()
        End Sub
#End Region ' Events

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
                Me.m_grid.Apply()

            Catch ex As Exception
                m_logger.LogError(ex, "ucOptionsAutoRun::Apply")
                Return IOptionsPage.eApplyResultType.Failed
            End Try

            Return IOptionsPage.eApplyResultType.Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() Implements IOptionsPage.SetDefaults

            Try
                Me.m_grid.ClearAll()
            Catch ex As Exception
                m_logger.LogError(ex, "ucOptionsAutoRun::SetDefaults")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanSetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Function CanSetDefaults() As Boolean _
            Implements IOptionsPage.CanSetDefaults
            Return True
        End Function

        Private Sub OnGridModified() Handles m_grid.OnValueChanged
            Try
                RaiseEvent OnOptionsFileManagementChanged(Me, New EventArgs())
            Catch ex As Exception

            End Try
        End Sub

#End Region

    End Class

End Namespace
