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
Imports System.Drawing.Imaging
Imports System.IO
Imports EwEUtils.Logging
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmEcopathDietTreeMaps

#Region " Private vars "

    Private m_doodler As cDietTreeMapRenderer
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmEcopathDietTreeMaps)()

#End Region ' Private vars

#Region " Constructor "

    Public Sub New()
        Me.InitializeComponent()

        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.UserMouse, True)

        Me.Text = My.Resources.LABEL_NAV_ECOPATH_OUTPUT_DIETTREEMAP
        Me.TabText = Me.Text

    End Sub

#End Region ' Constructor

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As EventArgs)

        Dim cmdh As cCommandHandler = Me.CommandHandler
        Dim cmd As cCommand = Nothing

        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_doodler = New cDietTreeMapRenderer(Me.UIContext)
        Me.m_pgSettings.SelectedObject = Me.m_doodler

        Me.m_tsmiFont.Image = SharedResources.CaseSensitive

        cmd = cmdh.GetCommand(cShowOptionsCommand.cCOMMAND_NAME)
        If cmd IsNot Nothing Then
            cmd.AddControl(Me.m_tsmiFont, eApplicationOptionTypes.Fonts)
        End If

        Me.UpdateControls()
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

        If (Me.UIContext Is Nothing) Then Return

        Dim cmdh As cCommandHandler = Me.CommandHandler
        Dim cmd As cCommand = Nothing

        ' Fonts
        cmd = cmdh.GetCommand(cShowOptionsCommand.cCOMMAND_NAME)
        If cmd IsNot Nothing Then
            cmd.RemoveControl(Me.m_tsmiFont)
        End If

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        Me.m_scContent.Panel2Collapsed = Not Me.m_tsmiSettings.Checked
        Me.m_pgSettings.Refresh()
    End Sub

#End Region ' Overrides

#Region " Drawing "

    Private Sub OnDiagramResize(sender As Object, e As System.EventArgs) _
            Handles m_pbDiagram.Resize

        Me.m_pbDiagram.Invalidate()

    End Sub

    Private Sub OnDiagramPaint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) _
            Handles m_pbDiagram.Paint

        If (Me.UIContext Is Nothing) Then Return

        Dim rc As Rectangle = Me.m_pbDiagram.ClientRectangle
        Me.m_doodler.Draw(e.Graphics, rc)

    End Sub

    ''' <summary>
    ''' Override the background paint routine to elimate flickering.
    ''' </summary>
    ''' <param name="pevent"></param>
    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' NOP
    End Sub

#End Region ' Drawing

#Region " Event handlers "

    Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
        MyBase.OnCoreMessage(msg)

        ' Refresh the diagram data when ecopath data has changed
        If (msg.Source = eCoreComponentType.Ecopath) And
               (msg.Type = eMessageType.DataModified) Then
            Me.m_pbDiagram.Invalidate()
        End If

    End Sub

    Private Sub OnOptionsChanged(sender As Object, e As EventArgs) Handles m_pgSettings.PropertyValueChanged
        Me.m_pbDiagram.Invalidate()
    End Sub

    Protected Overrides Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)
        Me.m_pbDiagram.Invalidate()
    End Sub

    Private Sub OnSettings(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiSettings.CheckedChanged
        Me.UpdateControls()
    End Sub

    Private Sub OnSaveToImage(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiSaveToImage.Click

        Dim fmt As Imaging.ImageFormat = Imaging.ImageFormat.Bmp
        Dim cmdh As cCommandHandler = Me.CommandHandler
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
        Dim fs As FileStream = Nothing
        Dim hdc As IntPtr = Nothing ' :)
        Dim mf As Metafile = Nothing
        Dim bmp As Bitmap = Nothing
        Dim rc As Rectangle = Me.m_pbDiagram.ClientRectangle

        cmdFS.Invoke(Me.FileName, SharedResources.FILEFILTER_IMAGE & "|" & SharedResources.FILEFILTER_IMAGE_EMF, 6)
        If cmdFS.Result = DialogResult.OK Then
            Select Case cmdFS.FilterIndex
                Case 2
                    fmt = Imaging.ImageFormat.Jpeg
                Case 3
                    fmt = Imaging.ImageFormat.Gif
                Case 4
                    fmt = Imaging.ImageFormat.Png
                Case 5
                    fmt = Imaging.ImageFormat.Tiff
                Case 6
                    bmp = New Bitmap(Me.m_pbDiagram.Width, Me.m_pbDiagram.Height, PixelFormat.Format32bppArgb)
                    fs = New FileStream(cmdFS.FileName, FileMode.Create)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        hdc = g.GetHdc()
                        mf = New Metafile(fs, hdc, EmfType.EmfOnly)
                        g.ReleaseHdc(hdc)
                    End Using
                    Using g As Graphics = Graphics.FromImage(mf)
                        Me.m_doodler.Draw(g, rc)
                    End Using
                    fs.Close()
                    mf.Dispose()
                    bmp.Dispose()
                    Return
                Case Else
                    fmt = Imaging.ImageFormat.Bmp
            End Select

            bmp = Me.StyleGuide.GetImage(Me.m_pbDiagram.Width, Me.m_pbDiagram.Height, fmt, cmdFS.FileName)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                Me.m_doodler.Draw(g, rc)
            End Using

            Try
                bmp.Save(cmdFS.FileName, fmt)

                ' ToDo: globalize this
                Dim msg As New cMessage(String.Format(SharedResources.GENERIC_FILESAVE_SUCCES, "Flow diagram image", cmdFS.FileName),
                                            eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(cmdFS.FileName)
                Me.Core.Messages.SendMessage(msg)

            Catch ex As Exception
                m_logger.LogError(ex, "frmFlowDiagram::SaveImage(" & cmdFS.FileName & ")")
                Dim msg As New cMessage(String.Format(SharedResources.FILE_SAVE_ERROR_DETAIL, cmdFS.FileName, ex.Message),
                                eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
                Me.Core.Messages.SendMessage(msg)
            End Try
            bmp.Dispose()

        End If

    End Sub

#End Region ' Event handlers

#Region " Internals "

    Private Function DataName() As String
        Return "FD01"
    End Function

    Protected Function FileName() As String
        Dim model As cEwEModel = Me.UIContext.Core.EwEModel
        Return cFileUtils.ToValidFileName(model.Name & " " & "DietTreeMap", False)
    End Function

#End Region ' Internals

End Class