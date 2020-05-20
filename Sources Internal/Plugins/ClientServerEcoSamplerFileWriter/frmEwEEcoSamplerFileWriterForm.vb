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

Imports EwECore
Imports System.Windows.Forms

Imports ScientificInterfaceShared.Controls

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmEwEEcoSamplerFileWriterForm

    Private m_plugin As cEcoSamplerFileWriterPlugin

    Public Sub New()

        ' This call is required by the designer.
        Me.InitializeComponent()


    End Sub

    ''' <summary>
    ''' OnLoad is called when a form is about to go 'live'. It is the perfect place to
    ''' perform last moment configurations before the form is made visible to the user.
    ''' </summary>
    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        ' If (Me.Core IsNot Nothing) Then
        Try

            Me.m_txtFileName.Text = Me.m_plugin.FileWriter.FileName

        Catch ex As Exception

        End Try



        ' End If

    End Sub


    Public Sub Init(ByVal PluginPoint As cEcoSamplerFileWriterPlugin)
        m_plugin = PluginPoint
    End Sub

    Private Sub frmEwEEcoSamplerFileWriterForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub m_btSelectFile_Click(sender As Object, e As EventArgs) Handles m_btSelectFile.Click
        Dim defalutDir As String = Me.m_plugin.Core.DefaultOutputPath(EwEUtils.Core.eAutosaveTypes.Ecopath)
        Dim sfd As SaveFileDialog = cEwEFileDialogHelper.SaveFileDialog("Ecopath Parameters .csv file", "", "csv files|*.csv|All files (*.*)|*.*")

        If IO.File.Exists(m_plugin.FileWriter.FileName) Then
            sfd.FileName = m_plugin.FileWriter.FileName
        Else
            sfd.InitialDirectory = defalutDir
            sfd.FileName = m_plugin.FileWriter.DefautlFilename
        End If

        If sfd.ShowDialog() = DialogResult.OK Then
            Me.m_txtFileName.Text = sfd.FileName
            m_plugin.FileWriter.FileName = Me.m_txtFileName.Text
        End If
    End Sub

    Private Sub m_btSave_Click(sender As Object, e As EventArgs) Handles m_btSave.Click


        m_plugin.FileWriter.ToCSVFile(m_plugin.FileWriter.FileName)



    End Sub
End Class