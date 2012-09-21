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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports EwECore.Ecosim
Imports ScientificInterfaceShared.Style
Imports EwECore
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.IO
Imports System.Windows.Forms

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Main and only interface for this plug-in.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmMain

    'Private m_bRunning As Boolean = False
    Private m_engine As cMultiRunsEngine = Nothing

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        For Each out As cEcosimResultWriter.eResultTypes In [Enum].GetValues(GetType(cEcosimResultWriter.eResultTypes))
            Me.m_clbValues.Items.Add(out)
        Next

        Me.m_tbxSource.Text = My.Settings.PathIn
        Me.m_tbxDest.Text = My.Settings.PathOut
        Me.m_cbCreateRunFolder.Checked = My.Settings.CreateUniqueRunFolder
        If My.Settings.ReadAsMonth Then
            Me.m_rbMonthly.Checked = True
        Else
            Me.m_rbAnnual.Checked = True
        End If

        Me.m_engine = New cMultiRunsEngine(Me.UIContext)

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        ' Hmm
        Me.StoreSettings()
        ' Done
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Dim bHasFiles As Boolean = (Me.m_clbFilesSrc.CheckedItems.Count > 0)
        Dim bHasVars As Boolean = (Me.m_clbValues.CheckedItems.Count > 0)
        Dim bHasOutput As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxDest.Text)
        Dim bIsRunning As Boolean = Me.Core.StateMonitor.IsBusy()

        Me.m_btnRun.Enabled = bHasFiles And bHasOutput And bHasVars And Not bIsRunning

    End Sub

#End Region ' Form overrides

#Region " Event handlers "

    Private Sub OnBrowseIn(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseSrc.Click

        Try
            Me.BrowseToTextbox(Me.m_tbxSource, "Select source folder with CSV files")
        Catch ex As Exception
            ' Whoah
            cLog.Write(ex, "OnBrowseIn")
        End Try

    End Sub

    Private Sub OnSourceFolderChanged(sender As Object, e As System.EventArgs) _
        Handles m_tbxSource.TextChanged

        Try
            Me.UpdateSourceFiles()
            Me.UpdateControls()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnAllSrc(sender As System.Object, e As System.EventArgs) _
        Handles m_btnAllSrc.Click

        Try
            Me.CheckAll(Me.m_clbFilesSrc)
            Me.UpdateControls()
        Catch ex As Exception
            ' Whoah
            cLog.Write(ex, "OnAllScr")
        End Try

    End Sub

    Private Sub OnBrowseOut(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseOut.Click

        Try
            Me.BrowseToTextbox(Me.m_tbxDest, "Select destination folder")
            Me.UpdateControls()
        Catch ex As Exception
            ' Whoah
            cLog.Write(ex, "OnBrowseOut")
        End Try

    End Sub

    Private Sub OnAllVars(sender As System.Object, e As System.EventArgs) _
        Handles m_btnAllVars.Click

        Try
            Me.CheckAll(Me.m_clbValues)
            Me.UpdateControls()
        Catch ex As Exception
            ' Whoah
            cLog.Write(ex, "OnAllVars")
        End Try

    End Sub

    ''' <summary>
    ''' Display file without path.
    ''' </summary>
    Private Sub OnFormatFile(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_clbFilesSrc.Format

        e.Value = Path.GetFileName(CStr(e.ListItem))

    End Sub

    Private Sub OnFormatVariable(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_clbValues.Format

        ' Provide text for ecosim run type
        Dim fmt As New cEcosimResultTypeFormatter()
        e.Value = fmt.GetDescriptor(e.ListItem)

    End Sub

    Private Sub OnItemChecking(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) _
        Handles m_clbValues.ItemCheck, m_clbFilesSrc.ItemCheck
        ' Call UpdateControls after check has been handled
        Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls), Nothing)
    End Sub

    Private Sub OnRun(sender As System.Object, e As System.EventArgs) _
        Handles m_btnRun.Click

        If Me.m_engine.IsRunning Then
            Return
        End If

        Me.StoreSettings()
        Me.UpdateControls()

        Try
            Dim lFiles As New List(Of String)
            For Each item As Object In Me.m_clbFilesSrc.CheckedItems
                lFiles.Add(CStr(item))
            Next

            Dim lOptions As New List(Of cEcosimResultWriter.eResultTypes)
            For Each item As Object In Me.m_clbValues.CheckedItems
                lOptions.Add(DirectCast(item, cEcosimResultWriter.eResultTypes))
            Next

            Me.m_engine.Run(New cMultiRunsEngine.RunCompletedDelegate(AddressOf RunDone), lFiles.ToArray(), Me.m_tbxDest.Text, Me.m_rbMonthly.Checked, lOptions.ToArray())

        Catch ex As Exception
            ' Whoah
            cLog.Write(ex, "OnRun")
        End Try

    End Sub

    Private Delegate Sub RunDoneDelegate()

    Private Sub RunDone()
        If Me.InvokeRequired Then
            Me.Invoke(New RunDoneDelegate(AddressOf RunDone))
        Else
            Me.UpdateControls()
        End If
    End Sub

#End Region ' Event handlers

#Region " Drag and drop "

    Private Function GetDragDropFiles(data As IDataObject) As String()
        Dim lstr As New List(Of String)
        If data.GetDataPresent(DataFormats.FileDrop) Then
            For Each str As String In CType(data.GetData(DataFormats.FileDrop), String())
                If File.Exists(str) Then
                    If (String.Compare(Path.GetExtension(str), ".csv", True) = 0) Then
                        lstr.Add(str)
                    End If
                End If
            Next
        End If
        Return lstr.ToArray
    End Function

    Private Function GetDragDropFolder(data As IDataObject) As String
        If data.GetDataPresent(DataFormats.FileDrop) Then
            Dim astrData As String() = CType(data.GetData(DataFormats.FileDrop), String())
            If (astrData.Length = 1) Then
                If Directory.Exists(astrData(0)) Then
                    Return astrData(0)
                End If
            End If
        End If
        Return ""
    End Function

    Protected Overrides Sub OnDragOver(e As System.Windows.Forms.DragEventArgs)
        If (Me.GetDragDropFiles(e.Data).Length > 0) Or Not String.IsNullOrWhiteSpace(GetDragDropFolder(e.Data)) Then
            e.Effect = DragDropEffects.All
        End If
        MyBase.OnDragOver(e)
    End Sub

    Protected Overrides Sub OnDragDrop(e As System.Windows.Forms.DragEventArgs)
        Dim astrFiles As String() = Me.GetDragDropFiles(e.Data)
        Dim strFolder As String = GetDragDropFolder(e.Data)

        If (astrFiles.Length > 0) Then
            Me.m_tbxSource.Text = ""
            Me.m_clbFilesSrc.Items.Clear()
            For Each strFile In astrFiles
                Me.m_clbFilesSrc.Items.Add(strFile)
            Next
            Me.CheckAll(Me.m_clbFilesSrc)
        ElseIf Not String.IsNullOrWhiteSpace(strFolder) Then
            Me.m_tbxSource.Text = strFolder
            Me.CheckAll(Me.m_clbFilesSrc)
        End If
        MyBase.OnDragDrop(e)
    End Sub

#End Region ' Drag and drop

#Region " Internals "

    Private Sub StoreSettings()
        My.Settings.PathIn = Me.m_tbxSource.Text
        My.Settings.PathOut = Me.m_tbxDest.Text
        My.Settings.ReadAsMonth = Me.m_rbMonthly.Checked
        My.Settings.CreateUniqueRunFolder = Me.m_cbCreateRunFolder.Checked
        My.Settings.Save()
    End Sub

    Private Sub UpdateSourceFiles()

        Me.m_clbFilesSrc.Items.Clear()
        If Not Directory.Exists(Me.m_tbxSource.Text) Then Return

        For Each strFile As String In Directory.GetFiles(Me.m_tbxSource.Text, "*.csv")
            Me.m_clbFilesSrc.Items.Add(strFile)
        Next
        Me.m_clbFilesSrc.Sorted = True

    End Sub

    Private Sub CheckAll(ByVal clb As CheckedListBox)
        For i As Integer = 0 To clb.Items.Count - 1
            clb.SetItemChecked(i, True)
        Next
    End Sub

    Private Sub BrowseToTextbox(ByVal tbx As TextBox, ByVal strPrompt As String)
        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmdFO As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
        cmdFO.Invoke(tbx.Text, strPrompt)
        If cmdFO.Result = Windows.Forms.DialogResult.OK Then
            tbx.Text = cmdFO.Directory
        End If
    End Sub

#End Region ' Internals

End Class