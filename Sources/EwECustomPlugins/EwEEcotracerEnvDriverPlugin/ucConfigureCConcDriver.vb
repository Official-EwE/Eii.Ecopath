' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Collections.Specialized
Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

Public Class ucConfigureCConcDriver
    Implements IOptionsPage
    Implements IUIElement

#Region " Private vars "

    Private m_ds As cEcotracerConcentrationDataset = Nothing
    Private m_bInUpdate As Boolean = False

#End Region ' Private vars 

#Region " Construction and destruction "

    Public Sub New(ds As cEcotracerConcentrationDataset)

        Me.InitializeComponent()

        Me.m_ds = ds
        Me.DoubleBuffered = True

    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso Me.components IsNot Nothing Then
                Me.components.Dispose()
            End If

        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

#End Region ' Construction and destruction

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As EventArgs)
        If (Me.UIContext Is Nothing) Then Return

        Dim core As cCore = Me.UIContext.Core
        For i As Integer = 1 To core.nGroups
            Me.m_cmbGroups.Items.Add(core.EcopathGroupInputs(i))
        Next

        Me.m_cmbGroups.SelectedIndex = Math.Max(0, Me.m_ds.Group - 1)

        MyBase.OnLoad(e)
    End Sub

#End Region ' Overrides

#Region " Options page "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write the content of the UI to the dataset.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function Apply() As IOptionsPage.eApplyResultType _
        Implements IOptionsPage.Apply

        Dim fmt As New cCoreInterfaceFormatter()
        Dim grp As cEcoPathGroupInput = DirectCast(Me.m_cmbGroups.SelectedItem, cEcoPathGroupInput)

        Me.m_ds.Group = grp.Index
        Me.m_ds.CustomName = cStringUtils.Localize(My.Resources.CCONC_DATASET_NAME, fmt.ToString(grp))
        Me.m_ds.CustomDescription = cStringUtils.Localize(My.Resources.CCONC_DATASET_DESCRIPTION, fmt.ToString(grp))

        Return IOptionsPage.eApplyResultType.Success

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' External check, replies whether the UI is ready to apply when all
    ''' expected data is in place.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function CanApply() As Boolean Implements IOptionsPage.CanApply
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' There are no valid defaults to set.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function CanSetDefaults() As Boolean Implements IOptionsPage.CanSetDefaults
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event to notify the world that the user has made modifications to the UI.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Event OnChanged(sender As IOptionsPage, args As System.EventArgs) _
        Implements IOptionsPage.OnChanged

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set default values in the form. Since there are none, this method will not 
    ''' do anything.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub SetDefaults() Implements IOptionsPage.SetDefaults
        ' Nothing to do here
    End Sub

    Private Sub OnFormatGroup(sender As Object, e As ListControlConvertEventArgs) Handles m_cmbGroups.Format
        Dim fmt As New cCoreInterfaceFormatter()
        e.Value = fmt.ToString(e.ListItem)
    End Sub

#End Region ' Options page

#Region " Public config "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide the EwE UI Context to this form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UIContext As cUIContext _
        Implements IUIElement.UIContext

#End Region ' Public config

End Class
