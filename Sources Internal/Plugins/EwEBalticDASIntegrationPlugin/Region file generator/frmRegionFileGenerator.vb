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
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Main (and only) interface to the DAS region file generator utility thing bit.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmRegionFileGenerator

#Region " Private vars "

    Private m_uic As cUIContext = Nothing

#End Region 'Private vars

#Region " Construction "

    Public Sub New(ByVal uic As cUIContext)
        Me.InitializeComponent()
        Me.m_uic = uic
    End Sub

#End Region ' Construction

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        ' Sanity check
        If (Me.m_uic Is Nothing) Then Return

        Dim core As cCore = Me.m_uic.Core

        ' Concoct a decent file name
        Dim scenario As cEwEScenario = core.EcospaceScenarios(core.ActiveEcospaceScenarioIndex)
        Dim strFile As String = cFileUtils.ToValidFileName(scenario.Name, False)
        Me.m_tbxFile.Text = Path.ChangeExtension(strFile, ".rgn")

        ' Restore last used layers
        Me.m_tbxLayers.Text = My.Settings.MRULayers

        ' Initialize enabled state of UI elements
        Me.UpdateControls()

        Me.CenterToScreen()

    End Sub

    Protected Overrides Sub OnClosed(e As System.EventArgs)

        ' Preserve last used layers
        My.Settings.MRULayers = Me.m_tbxLayers.Text
        My.Settings.Save()

        ' Bye
        MyBase.OnClosed(e)

    End Sub

#End Region 'Form overrides

#Region " Control events "

    Private Sub OnFileTextChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbxFile.TextChanged

        Try

            ' Update UI control states in response to file name text changes
            Me.UpdateControls()

        Catch ex As Exception
            cLog.Write(ex, "frmRegionFileGenerator::OnFileTextChanged")
        End Try

    End Sub

    Private Sub OnLayersValidated(sender As Object, e As System.EventArgs) _
        Handles m_tbxLayers.Validated

        Try

            ' Re-validate the layers text
            Me.m_tbxLayers.Text = Me.ToLayers()
            ' Update UI control states in response to layer text changes
            Me.UpdateControls()

        Catch ex As Exception
            cLog.Write(ex, "frmRegionFileGenerator::OnLayersValidated")
        End Try

    End Sub

    Private Sub OnGenerateFile(sender As System.Object, e As System.EventArgs) _
        Handles m_btnGenerate.Click

        Try

            ' Prepare save file dialog
            Dim strTitle As String = My.Resources.CAPTION_GENREGION_SAVE
            Dim strFilter As String = My.Resources.FILFILTER_DAS_REGION
            Dim strFile As String = cFileUtils.ToValidFileName(Me.m_tbxFile.Text, False)
            Dim sfd As SaveFileDialog = cEwEFileDialogHelper.SaveFileDialog(strTitle, strFile, strFilter)

            ' User completes file pick process?
            If (sfd.ShowDialog = DialogResult.OK) Then
                ' Save successful?
                If Me.SaveRegionFile(sfd.FileName) Then
                    ' #Yes: close save form
                    Me.DialogResult = Windows.Forms.DialogResult.OK
                    Me.Close()
                End If
            End If

        Catch ex As Exception
            cLog.Write(ex, "frmRegionFileGenerator::OnGenerateFile")
        End Try

    End Sub

    Private Sub OnVisitSU(sender As System.Object, e As System.EventArgs) _
        Handles m_pbxSponsors.Click

        Try

            Me.VisitSponsor("http://su.se")

        Catch ex As Exception
            cLog.Write(ex, "frmRegionFileGenerator::OnVisitSU")
        End Try

    End Sub

#End Region ' Control events

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state of UI elements.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()

        Dim bHasFile As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxFile.Text)
        Dim bHasLayers As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxLayers.Text)

        Me.m_btnGenerate.Enabled = bHasFile And bHasLayers

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the actual region file from Ecospace parameters and the 
    ''' data that the user entered.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveRegionFile(strFileName As String) As Boolean

        Dim writer As StreamWriter = Nothing
        Dim core As cCore = Me.m_uic.Core
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        Dim strLayers As String = Me.ToLayers
        Dim sCellLength As Single = bm.CellLength
        Dim bSuccess As Boolean = True

        Try
            ' If this fails an exception is thrown
            writer = New StreamWriter(strFileName)

            ' Using simple DAS region file format:
            ' <nX> <nY> <nLayers> <cellsize in m> <lat> <lon>
            ' <layer 1> <layer 2> .. <layer n>
            ' <depth 1,1> .. <depth 1, nCols>
            ' .. .. 
            ' <depth nRows, 1> .. <depth nRows, nCols>

            writer.WriteLine("{0} {1} {2} {3} {4} {5}", _
                             bm.InCol, bm.InRow, _
                             strLayers.Split(" "c).Length, _
                             bm.CellLength, _
                             bm.PosBottomRight.Y, _
                             bm.PosBottomRight.X)
            writer.WriteLine(strLayers)
            For iRow As Integer = 1 To bm.InRow
                If (iRow > 1) Then writer.WriteLine()
                For iCol As Integer = 1 To bm.InCol
                    If (iCol > 1) Then writer.Write(" "c)
                    writer.Write(cStringUtils.ToCSVField(depth.Cell(iRow, iCol)))
                Next
            Next
            writer.Flush()
            writer.Close()
            Me.SendMessage(String.Format(My.Resources.STATUS_GENREGION_SAVE_SUCCESS, strFileName), _
                           Path.GetDirectoryName(strFileName), bSuccess)

        Catch ex As Exception
            ' Panic
            bSuccess = False
            ' Log event
            cLog.Write(ex, "frmRegionFileGenerator::SaveRegionFile")
            ' Notify user (without hyperlink)
            Me.SendMessage(String.Format(My.Resources.STATUS_GENREGION_SAVE_FAILED, strFileName, ex.Message), _
                           "", bSuccess)

        End Try
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a reasonably valid layers string from what the user entered.
    ''' </summary>
    ''' <returns>A reasonably valid layers string from what the user entered.</returns>
    ''' <remarks>
    ''' The layers string is broken up into valid single precision values which
    ''' are sorted in ascending order, and formatted together again in a string
    ''' using fixed US-EN number formatting.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function ToLayers() As String

        Dim sbLayers As New StringBuilder()
        Dim lLayers As New List(Of Single)
        Dim sDepth As Single = 0

        For Each strBit As String In Me.m_tbxLayers.Text.Split(" "c)
            If Not String.IsNullOrWhiteSpace(strBit) Then
                If Single.TryParse(strBit, sDepth) Then
                    lLayers.Add(sDepth)
                End If
            End If
        Next

        lLayers.Sort()

        For i As Integer = 0 To lLayers.Count - 1
            If (i > 0) Then sbLayers.Append(" ")
            sbLayers.Append(cStringUtils.ToCSVField(lLayers(i)))
        Next

        Return sbLayers.ToString()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Notify the user.
    ''' </summary>
    ''' <param name="strMessage">Message text.</param>
    ''' <param name="strHyperlink">Clickable link.</param>
    ''' <param name="bSucces">True if this message is a success, or false if this
    ''' message indicates a disaster of epic proportions.</param>
    ''' <remarks>
    ''' The message is always sent as a Feedback message to ensure the user sees it.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub SendMessage(strMessage As String, strHyperlink As String, bSucces As Boolean)

        Dim msg As New cFeedbackMessage(strMessage, _
                                        eCoreComponentType.External, eMessageType.DataExport, _
                                        eMessageImportance.Information, eMessageReplyStyle.OK)
        If bSucces Then
            msg.Hyperlink = strHyperlink
        Else
            msg.Importance = eMessageImportance.Critical
        End If
        Me.m_uic.Core.Messages.SendMessage(msg)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Visit one of the sponsors.
    ''' </summary>
    ''' <param name="strURL"></param>
    ''' -----------------------------------------------------------------------
    Private Sub VisitSponsor(ByVal strURL As String)

        ' Use EwE UI command system
        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
        Dim cmd As cBrowserCommand = DirectCast(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        cmd.Invoke(strURL)

    End Sub

#End Region ' Internals

End Class