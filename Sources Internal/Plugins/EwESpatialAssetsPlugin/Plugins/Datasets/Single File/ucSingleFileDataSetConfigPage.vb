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
Imports System.Windows.Forms
Imports EwESpatialAssetsPlugin.SpatialData
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Friend Class ucSingleFileDataSetConfigPage

    Private m_dataset As cSingleFileDataSetPlugin = Nothing

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_tbxName.Text = Me.m_dataset.DisplayName
        Me.m_tbxDescription.Text = Me.m_dataset.Description
        Me.m_tbxFile.Text = Me.m_dataset.Source

        If (Me.m_dataset.TimeStart = Me.m_dataset.TimeEnd) Then
            Me.m_rbMonth.Checked = True
            Me.m_date.Value = Me.m_dataset.TimeStart
        Else
            Me.m_rbFirstTimeStep.Checked = True
            Me.m_date.Value = Date.Now
        End If

        If String.IsNullOrWhiteSpace(Me.m_dataset.Source) Then
            Me.DoBrowse()
        End If

    End Sub

    Protected Overrides Sub Dispose(bDispose As Boolean)
        Try
            Me.Apply()
            If Disposing And (components IsNot Nothing) Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(Disposing)
        End Try
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

#End Region ' Events

#Region " Interface implementation "

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

    Public Function Apply() As Boolean
        Try
            Me.DoApply()
        Catch ex As Exception
            Return False
        End Try
        Return True

    End Function

#End Region ' Interface implementation

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()

    End Sub

    Private Sub DoBrowse()
        Dim dlg As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(ScientificInterfaceShared.My.Resources.CAPTION_SELECT_FILE, Me.m_dataset.Source, Me.m_dataset.DialogReadFilter)
        If dlg.ShowDialog(Me) = DialogResult.OK Then
            Me.m_tbxFile.Text = dlg.FileName
        End If
    End Sub

    Private Sub DoApply()

        Me.m_dataset.DisplayName = Me.m_tbxName.Text
        Me.m_dataset.Description = Me.m_tbxDescription.Text
        Me.m_dataset.Source = Me.m_tbxFile.Text

        If Me.m_rbFirstTimeStep.Checked Then
            Me.m_dataset.Time = DateTime.MinValue
        Else
            Me.m_dataset.Time = Me.m_date.Value
        End If

    End Sub

End Class
