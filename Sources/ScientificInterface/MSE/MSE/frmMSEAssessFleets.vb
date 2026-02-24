' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' ---------------------------------------------------------------------------
''' <summary>
''' Form class for assessing MSE Fleet CV values.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmMSEAssessFleets

    Private m_propStartYear As cProperty = Nothing
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmMSEAssessFleets)()

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
        Me.Grid = Me.m_grid
    End Sub

    Public Overrides Property UIContext() As cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As cUIContext)
            MyBase.UIContext = value
            Me.m_grid.UIContext = value
            Me.m_blocks.UIContext = value
        End Set
    End Property

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        Try

            ' Create and attach datasource
            Dim ds As New cMSEFishingColorBlockDataSource(Me.UIContext)
            Me.m_blocks.Attach(ds, New ucCVBlockSelector)

            ' Track MSE start year changes
            Me.m_propStartYear = Me.PropertyManager.GetProperty(Me.UIContext.Core.MSEManager.ModelParameters, eVarNameFlags.MSEStartYear)
            AddHandler Me.m_propStartYear.PropertyChanged, AddressOf Me.OnLastYearChanged

        Catch ex As Exception

        End Try

        ' Show form
        MyBase.OnLoad(e)

    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

        Try
            ' No longer track MSE start year changes
            RemoveHandler Me.m_propStartYear.PropertyChanged, AddressOf Me.OnLastYearChanged
            ' Release blocks
            Me.m_blocks.Dispose()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & " Exception: " & ex.Message)
        End Try
        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)

        If (ct And cStyleGuide.eChangeType.Colours) > 0 Then
            Me.m_blocks.Refresh()
        End If

    End Sub

    Private Sub OnLastYearChanged(prop As cProperty, changeFlags As cProperty.eChangeFlags)
        Try
            If (changeFlags And cProperty.eChangeFlags.Value) > 0 Then
                Me.m_blocks.Refresh()
            End If
        Catch ex As Exception
            m_logger.LogError(ex, Me.ToString & ".OnLastYearChanged() Exception")
        End Try
    End Sub

End Class

