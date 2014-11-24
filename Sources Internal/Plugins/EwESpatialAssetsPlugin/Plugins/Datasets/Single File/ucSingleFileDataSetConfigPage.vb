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

Option Strict On
Imports System.IO
Imports System.Windows.Forms
Imports EwECore.SpatialData
Imports EwESpatialAssetsPlugin.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Friend Class ucSingleFileDataSetConfigPage
    Implements IUIElement
    Implements IOptionsPage

    Private m_dataset As cSingleFileDataSetPlugin = Nothing

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_tbxName.Text = Me.m_dataset.DisplayName
        Me.m_tbxDescription.Text = Me.m_dataset.DataDescription
        Me.m_tbxFile.Text = Me.m_dataset.Source

        If (Me.m_dataset.TimeStart = Me.m_dataset.TimeEnd) Then
            Me.m_rbMonth.Checked = True
            Me.m_date.Value = Me.m_dataset.TimeStart
        Else
            Me.m_rbFirstTimeStep.Checked = True
            Me.m_date.Value = Date.Now
        End If

        If (Me.m_dataset.VarName = eVarNameFlags.NotSet) Then
            ' Allow all supported varnames
            Me.m_cmbVarName.Items.Add(eVarNameFlags.NotSet)
            If (Me.UIContext IsNot Nothing) Then
                For Each adt As cSpatialDataAdapter In Me.UIContext.Core.SpatialDataConnectionManager.Adapters
                    Me.m_cmbVarName.Items.Add(adt.VarName)
                Next
            End If
        Else
            ' Allow only dataset varname when configuring a pre-existing dataset
            Me.m_cmbVarName.Items.Add(Me.m_dataset.VarName)
        End If
        Me.m_cmbVarName.SelectedItem = Me.m_dataset.VarName

        If String.IsNullOrWhiteSpace(Me.m_dataset.Source) Then
            Me.DoBrowse()
        End If

    End Sub

#Region " Events "

    Private Sub OnBrowse(sender As System.Object, e As System.EventArgs) _
        Handles m_btnBrowse.Click
        Me.DoBrowse()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnDescriptiveChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tbxName.TextChanged, m_tbxDescription.TextChanged
        Me.UpdateControls()
    End Sub

    Private Sub OnSwitchToDatePicker(sender As System.Object, e As System.EventArgs) _
        Handles m_date.GotFocus
        Me.m_rbMonth.Checked = True
    End Sub

    Private Sub OnFormatVarname(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbVarName.Format
        Dim fmt As New cVarnameTypeFormatter()
        e.Value = fmt.GetDescriptor(e.ListItem)
    End Sub

#End Region ' Events

#Region " Interface implementation "

    Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
    Implements ScientificInterfaceShared.Controls.IUIElement.UIContext

    Public Function Apply() As IOptionsPage.eApplyResultType _
        Implements IOptionsPage.Apply
        Try
            Me.DoApply()
            Return IOptionsPage.eApplyResultType.Success
        Catch ex As Exception
            Return IOptionsPage.eApplyResultType.Failed
        End Try
    End Function

    Public Sub SetDefaults() Implements IOptionsPage.SetDefaults
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Dataset As EwEUtils.SpatialData.ISpatialDataSet
        Get
            Return Me.m_dataset
        End Get
        Set(ByVal value As EwEUtils.SpatialData.ISpatialDataSet)
            Debug.Assert(TypeOf value Is cSingleFileDataSetPlugin)
            Me.m_dataset = DirectCast(value, cSingleFileDataSetPlugin)
        End Set
    End Property

#End Region ' Interface implementation

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()
        Try
            If File.Exists(Me.m_tbxFile.Text) Then
                Me.m_pbInfo.BackgroundImage = ScientificInterfaceShared.My.Resources.OK
            Else
                Me.m_pbInfo.BackgroundImage = ScientificInterfaceShared.My.Resources.Critical
            End If
            RaiseEvent OnSingleFileConfigPageChanged(Me, New EventArgs())
        Catch ex As Exception

        End Try
    End Sub

    Private Function DoBrowse() As Boolean

        Dim dlg As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(cStringUtils.Localize(My.Resources.PROMPT_SELECTFILE, Me.m_tbxName.Text), _
                                                                        Me.m_dataset.Source, _
                                                                        Me.m_dataset.DialogReadFilter(True, False, True))
        If (dlg.ShowDialog(Me) = DialogResult.OK) Then
            Me.m_tbxFile.Text = dlg.FileName
            If (Me.m_dataset.Source Is Nothing) Then
                Me.m_tbxName.Text = Path.GetFileNameWithoutExtension(dlg.FileName)
            End If
            Me.UpdateControls()
            Return True
        End If
        Return False

    End Function

    Private Sub DoApply()

        Me.m_dataset.DisplayName = Me.m_tbxName.Text
        Me.m_dataset.DataDescription = Me.m_tbxDescription.Text
        Me.m_dataset.Source = Me.m_tbxFile.Text
        Me.m_dataset.VarName = DirectCast(Me.m_cmbVarName.SelectedItem, eVarNameFlags)

        If Me.m_rbFirstTimeStep.Checked Then
            Me.m_dataset.Time = DateTime.MinValue
        Else
            Me.m_dataset.Time = Me.m_date.Value
        End If

    End Sub

    Public Function CanApply() As Boolean _
        Implements IOptionsPage.CanApply
        Return File.Exists(Me.m_tbxFile.Text)
    End Function

    Public Event OnSingleFileConfigPageChanged(sender As IOptionsPage, args As System.EventArgs) _
        Implements ScientificInterfaceShared.Controls.IOptionsPage.OnChanged

    Public Function CanSetDefaults() As Boolean _
        Implements IOptionsPage.CanSetDefaults
        Return False
    End Function

End Class
