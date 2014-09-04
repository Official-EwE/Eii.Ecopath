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
Imports System.Text
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmUI

    Private m_core As cCore = Nothing

    Public Sub New(core As cCore)
        Me.InitializeComponent()
        Me.m_core = core
    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.m_core Is Nothing) Then Return

        ' Concoct a decent file name
        Dim scenario As cEwEScenario = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex)
        Dim strFile As String = cFileUtils.ToValidFileName(scenario.Name, False)
        Me.m_tbxFile.Text = Path.ChangeExtension(strFile, ".rgn")

        ' Restore last used layers
        Me.m_tbxLayers.Text = My.Settings.MRULayers

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnClosed(e As System.EventArgs)

        ' Preserve last used layers
        My.Settings.MRULayers = Me.m_tbxLayers.Text
        My.Settings.Save()

        MyBase.OnClosed(e)

    End Sub

#End Region 'Form overrides

#Region " Control events "

    Private Sub OnFileTextChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbxFile.TextChanged, m_tbxLayers.TextChanged

        Me.UpdateControls()

    End Sub

    Private Sub OnLayersValidated(sender As Object, e As System.EventArgs) _
        Handles m_tbxLayers.Validated

        Me.m_tbxLayers.Text = Me.ToLayers()
        Me.UpdateControls()

    End Sub

    Private Sub OnGenerateFile(sender As System.Object, e As System.EventArgs) _
        Handles m_btnGenerate.Click

        Dim strFile As String = cFileUtils.ToValidFileName(Me.m_tbxFile.Text, False)
        Dim sfd As SaveFileDialog = cEwEFileDialogHelper.SaveFileDialog("Save DAS region file", strFile, "DAS region files|*.rgn")
        If sfd.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.SaveRegionFile(sfd.FileName)
        End If

    End Sub

#End Region 'Control events

#Region " Internals "

    Private Sub UpdateControls()

        Dim bHasFile As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxFile.Text)
        Dim bHasLayers As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxLayers.Text)

        Me.m_btnGenerate.Enabled = bHasFile And bHasLayers

    End Sub

    Private Function SaveRegionFile(strFileName As String) As Boolean

        Dim writer As StreamWriter = Nothing
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        Dim strLayers = Me.ToLayers
        Dim sCellLength As Single = bm.CellLength
        Dim bSuccess As Boolean = True

        Try
            writer = New StreamWriter(strFileName)
            If (writer Is Nothing) Then Return False

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
            Me.SendMessage("DAS region file saved to " & strFileName, Path.GetDirectoryName(strFileName), bSuccess)

        Catch ex As Exception
            cLog.Write(ex, "DASWriterPlugin.SaveRegionFile")
            bSuccess = False
            Me.SendMessage(ex.Message, "", bSuccess)

        End Try
        Return True

    End Function

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

    Private Sub SendMessage(strMessage As String, strHyperlink As String, bSucces As Boolean)

        Dim msg As New cMessage(strMessage, eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        If bSucces Then
            msg.Hyperlink = strHyperlink
        Else
            msg.Importance = eMessageImportance.Critical
        End If
        Me.m_core.Messages.SendMessage(msg)

    End Sub

#End Region ' Internals

End Class