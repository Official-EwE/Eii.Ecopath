' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class that performs the actual file generation.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cRegionFileWriter

    Private m_core As cCore = Nothing

    Public Sub New(ByVal core As cCore)
        Me.m_core = core
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the actual region file from Ecospace parameters and the 
    ''' data that the user entered.
    ''' </summary>
    ''' <param name="strFileName">File to save to.</param>
    ''' <param name="strLayers">Layers to add.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Save(ByVal strFileName As String, _
                             ByVal strLayers As String) As Boolean

        Dim writer As StreamWriter = Nothing
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        Dim sCellLength As Single = bm.CellLength
        Dim bSuccess As Boolean = True

        Try
            ' If this fails an exception is thrown
            writer = New StreamWriter(strFileName)

            ' Using simple DAS region file format:
            ' <nX> <nY> <nLayers> <baltic latitude cellsize in m> <ll lat cell center> <ll lon cell center> 
            ' <layer 1> <layer 2> .. <layer n>
            ' <depth 1,1> .. <depth 1, nCols>
            ' .. .. 
            ' <depth nRows, 1> .. <depth nRows, nCols>

            writer.WriteLine("{0} {1} {2} {3} {4} {5}", _
                             bm.InCol, bm.InRow, _
                             strLayers.Split(" "c).Length, _
                             cStringUtils.ToCSVField(bm.CellLength * 999.9), _
                             cStringUtils.ToCSVField(ToDASCoord(bm.PosBottomRight.Y + bm.CellSize / 2)), _
                             cStringUtils.ToCSVField(ToDASCoord(bm.PosTopLeft.X + bm.CellSize / 2)))

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
            Me.SendMessage(String.Format(My.Resources.STATUS_SAVE_SUCCESS, strFileName), _
                           Path.GetDirectoryName(strFileName), bSuccess)

        Catch ex As Exception
            ' Panic
            bSuccess = False
            ' Log event
            cLog.Write(ex, "cRegionFileWriter::Generate")
            ' Notify user (without hyperlink)
            Me.SendMessage(String.Format(My.Resources.STATUS_SAVE_FAILED, strFileName, ex.Message), _
                           "", bSuccess)

        End Try
        Return True

    End Function

#Region " Internals "

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
        Me.m_core.Messages.SendMessage(msg)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert a position in degrees to a coordinate for DAS, speciefied as
    ''' [deg][min].
    ''' </summary>
    ''' <param name="sPos"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ToDASCoord(ByVal sPos As Single) As Integer

        sPos = ((sPos Mod 360) + 360) Mod 360

        Dim sDeg As Single = CSng(Math.Truncate(sPos))
        Dim sMin As Single = CSng(sPos - sDeg)
        Return CInt(Math.Truncate(sPos) * 100) + CInt(sMin * 60.0!)

    End Function

#End Region ' Internals

End Class
