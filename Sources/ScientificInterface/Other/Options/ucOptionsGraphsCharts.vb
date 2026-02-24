' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Graph settings interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsGraphsCharts
        Implements IOptionsPage
        Implements IUIElement

#Region " Variables "

        ''' <summary>Prevent loops.</summary>
        Private m_bInUpdate As Boolean = False

#End Region ' Variables

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.InitializeComponent()
            Me.UIContext = uic

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Init me!
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

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
        Public Event OnOptionsGraphsChanged(sender As IOptionsPage, args As System.EventArgs) _
            Implements IOptionsPage.OnChanged

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>
        ''' ------------------------------------------------------------------- 
        Public Function Apply() As IOptionsPage.eApplyResultType _
             Implements IOptionsPage.Apply

            If Not Me.CanApply Then Return IOptionsPage.eApplyResultType.Failed

            Dim tsShowLegends As TriState = TriState.UseDefault

            If Me.m_rbLegendAlways.Checked Then
                tsShowLegends = TriState.True
            End If

            Me.UIContext.StyleGuide.SuspendEvents()

            ' Update thumbnails, legend settings
            Me.UIContext.StyleGuide.ThumbnailSize = CInt(Me.m_nudThumbnailSize.Value)
            Me.UIContext.StyleGuide.NodeSymbolSize = CInt(Me.m_nudChartSymbols.Value)
            Me.UIContext.StyleGuide.ShowLegends = tsShowLegends

            Me.UIContext.StyleGuide.ResumeEvents()
            Me.UIContext.StyleGuide.FontsChanged()

            Return IOptionsPage.eApplyResultType.Success

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>
        ''' ------------------------------------------------------------------- 
        Public Sub SetDefaults() _
             Implements IOptionsPage.SetDefaults

            Me.UIContext.StyleGuide.ThumbnailSize = cCore.NULL_VALUE
            Me.UIContext.StyleGuide.NodeSymbolSize = cCore.NULL_VALUE

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

        Private Sub UpdateControls()

            Me.m_bInUpdate = True

            Me.m_nudThumbnailSize.Value = CDec(Math.Max(Me.m_nudThumbnailSize.Minimum, Math.Min(Me.m_nudThumbnailSize.Maximum, Me.UIContext.StyleGuide.ThumbnailSize)))
            Me.m_nudChartSymbols.Value = CDec(Math.Max(Me.m_nudChartSymbols.Minimum, Math.Min(Me.m_nudChartSymbols.Maximum, Me.UIContext.StyleGuide.NodeSymbolSize)))

            Select Case Me.UIContext.StyleGuide.ShowLegends
                Case TriState.UseDefault, TriState.False
                    Me.m_rbLegendSelective.Checked = True
                Case TriState.True
                    Me.m_rbLegendAlways.Checked = True
                    'Case TriState.False
                    '    Me.m_rbLegendNever.Checked = True
            End Select

            Me.m_bInUpdate = False

        End Sub

        Private Sub OnContentChanged(sender As Object, e As EventArgs) _
            Handles m_rbLegendSelective.CheckedChanged, m_rbLegendSelective.CheckedChanged,
                    m_nudThumbnailSize.ValueChanged, m_nudChartSymbols.ValueChanged

            If Me.m_bInUpdate Then Return

            RaiseEvent OnOptionsGraphsChanged(Me, New EventArgs())

        End Sub

#End Region ' Internals

    End Class

End Namespace

