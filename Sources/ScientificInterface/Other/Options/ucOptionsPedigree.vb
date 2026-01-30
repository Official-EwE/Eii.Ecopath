' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Pedigree settings interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsPedigree
        Implements IOptionsPage
        Implements IUIElement

        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of ucOptionsPedigree)()

#Region " Constructors "

        Public Sub New(uic As cUIContext)

            Me.UIContext = uic
            Me.InitializeComponent()

            Dim sg As cStyleGuide = Me.UIContext.StyleGuide
            Me.m_cbShowPedigreeIndicators.Checked = sg.ShowPedigree

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Control's load event which gets called every time the control gets loaded. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
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
        Public Event OnOptionsPedigreeChanged(sender As IOptionsPage, args As System.EventArgs) _
              Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            If Not Me.CanApply Then Return IOptionsPage.eApplyResultType.Failed

            Dim sg As cStyleGuide = Me.UIContext.StyleGuide

            ' Apply colors to the style guide
            sg.SuspendEvents()

            Try
                sg.ShowPedigree = Me.m_cbShowPedigreeIndicators.Checked
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                m_logger.LogError(ex, "ucOptionsPedigree::Apply")
            End Try

            sg.ResumeEvents()
            Return IOptionsPage.eApplyResultType.Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() _
                Implements IOptionsPage.SetDefaults

            Try
                Me.m_cbShowPedigreeIndicators.Checked = CBool(My.Settings.GetDefaultValue("ShowPedigree"))
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

    End Class

End Namespace

