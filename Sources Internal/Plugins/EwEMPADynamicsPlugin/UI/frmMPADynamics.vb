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
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmMPADynamics

    Private m_engine As cMPADynamicsEngine = Nothing

    Public Sub New(uic As cUIContext, engine As cMPADynamicsEngine)

        Me.UIContext = uic
        Me.m_engine = engine

        Me.InitializeComponent()

        Me.Text = My.Resources.PLUGIN_TITLE
        Me.TabText = Me.Text
        Me.m_tsbnLoadCSV.Image = SharedResources.openHS

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.UpdateGrid()
    End Sub

    Private Sub UpdateGrid()
        Dim states As ICollection(Of cMPAState) = Me.m_engine.MPAStates
        Me.m_dgvStates.Rows.Clear()
        For i As Integer = 0 To states.Count - 1
            Dim state As cMPAState = states(i)
            Me.m_dgvStates.Rows.Add(state.TimeStamp.ToShortDateString(),
                                    state.MPA.Name,
                                    state.ClosureText())
        Next
    End Sub

    Private Sub OnLoadCSV(sender As Object, e As EventArgs) Handles m_tsbnLoadCSV.Click
        ' ToDo: globalize this method
        Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog("Select MPA closed state CSV file", "", SharedResources.FILEFILTER_CSV)
        If (ofd.ShowDialog() = DialogResult.OK) Then
            Me.m_engine.LoadCSV(ofd.FileName)
            Me.UpdateGrid()
        End If
    End Sub

End Class