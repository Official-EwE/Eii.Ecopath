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
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

#End Region ' Imports

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

