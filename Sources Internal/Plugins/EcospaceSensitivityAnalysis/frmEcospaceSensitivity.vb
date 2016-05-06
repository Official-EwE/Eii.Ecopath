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
' Copyright 1991-2012 UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

Imports EwECore
Imports System.Windows.Forms
Imports System.IO

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmEcospaceSensitivity

    Private m_plugin As cEcospaceSensitivityPluginPoint

    Private m_inInit As Boolean

    Private m_lstParControls As List(Of Control)

    Public Sub New()

        Me.m_lstParControls = New List(Of Control)

        ' This call is required by the designer.
        Me.InitializeComponent()


    End Sub

    Public Overrides ReadOnly Property IsRunForm As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>
    ''' OnLoad is called when a form is about to go 'live'. It is the perfect place to
    ''' perform last moment configurations before the form is made visible to the user.
    ''' </summary>
    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.Core IsNot Nothing) Then

            Dim model As cEwEModel = Me.Core.EwEModel

            m_lstParControls = New List(Of Control)
            m_lstParControls.Add(Me.m_txBounds)
            m_lstParControls.Add(Me.m_lbBounds)
            m_lstParControls.Add(Me.m_lbOutputFile)
            m_lstParControls.Add(Me.m_btOuputFile)
            m_lstParControls.Add(Me.m_lvFiles)
            m_lstParControls.Add(Me.m_btRemovalOutput)
            m_lstParControls.Add(Me.m_lbRemoval)

            Me.m_ttFiles.SetToolTip(Me.m_lvFiles, "Double click row to select external driver file.")

            Me.PopulateFileList()

            AddHandler Me.RunManager.OnProgress, AddressOf Me.onEcospaceTimeStep
            AddHandler Me.RunManager.OnStateChange, AddressOf Me.onStateChanged


        End If

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        RemoveHandler Me.RunManager.OnProgress, AddressOf Me.onEcospaceTimeStep
        RemoveHandler Me.RunManager.OnStateChange, AddressOf Me.onStateChanged
    End Sub

    Private Sub PopulateFileList()
        Dim item As ListViewItem
        For Each pair As cLayerFilePair In Me.RunManager.RunParameters.lstBoundsFiles
            item = Me.m_lvFiles.Items.Add(pair.MapLayer.Layer.Name)
            item.SubItems.Add(pair.File)
            item.Tag = pair
        Next
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        If Me.RunManager IsNot Nothing Then

            Me.m_inInit = True

            setRunState()

            Me.m_txBounds.Text = EwEUtils.Utilities.cStringUtils.FormatNumber(RunManager.RunParameters.Delta)

            If Not String.IsNullOrWhiteSpace(Me.RunManager.RunParameters.BoundsOutput) Then
                Me.m_lbOutputFile.Text = Path.GetFileName("'" + Me.RunManager.RunParameters.BoundsOutput) + "' in directory '" + Path.GetDirectoryName(Me.RunManager.RunParameters.BoundsOutput) + "'"
            End If

            If Not String.IsNullOrWhiteSpace(Me.RunManager.RunParameters.RemovalOutput) Then
                Me.m_lbRemoval.Text = Path.GetFileName("'" + Me.RunManager.RunParameters.RemovalOutput) + "' in directory '" + Path.GetDirectoryName(Me.RunManager.RunParameters.RemovalOutput) + "'"
            End If

            Me.m_inInit = False

        End If

    End Sub

    Private Sub setRunState()

        Select Case Me.RunManager.State

            Case cRunManager.eEcospaceSensitivityStates.Running

                If Me.RunManager.runType = cRunManager.eRunTypes.Bounds Then
                    Me.m_btStopRun.Enabled = True
                    Me.m_btRun.Enabled = False
                    Me.m_btRunRemoval.Enabled = False
                    Me.m_btStopRemoval.Enabled = False
                Else
                    Me.m_btStopRun.Enabled = False
                    Me.m_btRun.Enabled = False
                    Me.m_btRunRemoval.Enabled = False
                    Me.m_btStopRemoval.Enabled = True
                End If

                For Each con As Control In Me.m_lstParControls
                    con.Enabled = False
                Next


            Case cRunManager.eEcospaceSensitivityStates.Stopped

                Me.m_btRun.Enabled = True
                Me.m_btRunRemoval.Enabled = True

                Me.m_btStopRun.Enabled = False
                Me.m_btStopRemoval.Enabled = False

                For Each con As Control In Me.m_lstParControls
                    con.Enabled = True
                Next

                Me.m_pbRunProgress.Value = 0
                Me.m_pbTotalProgress.Value = 0

        End Select

    End Sub


    Private Sub UpdateParameters()

        If Me.m_inInit Then Return

        If Me.RunManager IsNot Nothing Then
            Dim dTemp As Single
            If Single.TryParse(Me.m_txBounds.Text, dTemp) Then
                Me.RunManager.RunParameters.Delta = dTemp
            End If

        End If

    End Sub

    Private ReadOnly Property RunManager As cRunManager
        Get
            Return Me.m_plugin.RunManager
        End Get
    End Property

    Public Sub Init(ByVal PluginPoint As cEcospaceSensitivityPluginPoint)
        m_plugin = PluginPoint

        UpdateControls()

    End Sub


    Private Sub m_btRun_Click(sender As System.Object, e As System.EventArgs) Handles m_btRun.Click
        Me.m_pbTotalProgress.Value = 0
        Me.RunManager.RunBounds()
    End Sub


    Private Sub onEcospaceTimeStep(TotalPercentDone As Single, RunPercentDone As Single, MapName As String)
        Try
            Me.m_pbRunProgress.Value = CInt(RunPercentDone * 100)
            Me.m_pbTotalProgress.Value = CInt(TotalPercentDone * 100)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub onStateChanged(newState As cRunManager.eEcospaceSensitivityStates)
        Try

            setRunState()

        Catch ex As Exception

        End Try
    End Sub


    Private Sub m_btOuputFile_Click(sender As System.Object, e As System.EventArgs) Handles m_btOuputFile.Click
        Dim SFD As New SaveFileDialog

        SFD.FileName = Me.m_plugin.RunManager.RunParameters.BoundsOutput
        SFD.Filter = "*.csv|*.csv|*.*|*.*"
        SFD.FilterIndex = 0

        'SFD.OverwritePrompt = False

        If SFD.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim filename As String = SFD.FileName
            Me.m_plugin.RunManager.RunParameters.BoundsOutput = filename
            Me.UpdateControls()

        End If
    End Sub

    Private Sub m_btStopRun_Click(sender As System.Object, e As System.EventArgs) Handles m_btStopRun.Click

        Me.RunManager.StopRun()

    End Sub

    Private Sub m_txBounds_TextChanged(sender As System.Object, e As System.EventArgs) Handles m_txBounds.TextChanged
        UpdateParameters()
    End Sub


    Private Sub On_lvFiles_DoubleClick(sender As System.Object, e As System.EventArgs) Handles m_lvFiles.DoubleClick

        Dim selItem As ListViewItem = Me.m_lvFiles.SelectedItems.Item(0)
        If selItem Is Nothing Then Return
        Try
            Debug.Assert(selItem.Tag IsNot Nothing)

            Dim pair As cLayerFilePair = DirectCast(selItem.Tag, cLayerFilePair)

            Dim fn As String = Me.selectFile(pair.MapLayer.Layer.Name)
            If Not String.IsNullOrEmpty(fn) Then
                pair.File = fn
            End If
            selItem.Text = pair.MapLayer.Layer.Name
            selItem.SubItems(1).Text = pair.File

        Catch ex As Exception

        End Try

    End Sub

    Private Function selectFile(LayerName As String) As String
        Dim Ofd As New OpenFileDialog

        Ofd.Filter = "*.asc|*.asc|*.*|*.*"
        Ofd.FilterIndex = 0
        Ofd.Title = "Select input driver file for " + LayerName

        If Ofd.ShowDialog = Windows.Forms.DialogResult.OK Then
            Return Ofd.FileName
        End If

        Return String.Empty

    End Function

    Private Sub m_btStopRemoval_Click(sender As System.Object, e As System.EventArgs) Handles m_btStopRemoval.Click
        Me.RunManager.StopRun()
    End Sub

    Private Sub m_btRunRemoval_Click(sender As System.Object, e As System.EventArgs) Handles m_btRunRemoval.Click
        Me.RunManager.RunRemoval()
    End Sub

    Private Sub m_btRemovalOutput_Click(sender As System.Object, e As System.EventArgs) Handles m_btRemovalOutput.Click
        Dim SFD As New SaveFileDialog

        SFD.FileName = Me.RunManager.RunParameters.RemovalOutput
        SFD.Filter = "*.csv|*.csv|*.*|*.*"
        SFD.FilterIndex = 0

        If SFD.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim filename As String = SFD.FileName
            Me.RunManager.RunParameters.RemovalOutput = filename
            Me.UpdateControls()
        End If
    End Sub

End Class