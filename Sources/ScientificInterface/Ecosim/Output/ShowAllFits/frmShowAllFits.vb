'==============================================================================
'
' $Log: frmShowAllFits.vb,v $
' Revision 1.4  2009/05/11 01:50:58  jeroens
' Renamed command classes
'
' Revision 1.3  2009/01/16 18:30:43  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/11/08 23:51:54  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:31:51  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/09/23 16:14:57  jeroens
' TS 'Apply' -> 'Enable'
'
' Revision 1.6  2008/09/09 14:44:52  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.5  2008/08/14 01:52:53  jeroens
' Not mybase!
'
' Revision 1.4  2008/07/31 16:14:00  jeroens
' Removed showallfits form setting
'
' Revision 1.3  2008/05/16 02:12:08  jeroens
' Added clipping regions to ensure graphs do not overflow
'
' Revision 1.2  2008/05/16 01:17:16  jeroens
' Prevented from crashing on bizarre computed values
'
' Revision 1.1  2008/02/12 23:06:55  jeroens
' Revised and debugged
'
' Revision 1.18  2008/02/11 03:57:01  jeroens
' No longer buffers first year, instead obtained from Ecosim
'
' Revision 1.17  2008/01/21 04:06:39  jeroens
' Fixed shape max scale issues, once and for all
'
' Revision 1.16  2007/12/14 02:15:24  jeroens
' * Simplified
' * Localized
' * Started process to reduce buffered data
'
' Revision 1.15  2007/09/30 20:52:49  jeroens
' * Renamed resource(s)
'
' Revision 1.14  2007/09/24 17:57:05  sherman
' Try header
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Windows.Forms
Imports System.IO
Imports ScientificInterface.Other
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class frmShowAllFits

#Region " Private vars "

        Private m_core As cCore
        Private m_sDotSize As Single
        Private m_sLineWidth As Single
        Private m_sLRMargin As Single
        Private m_sTBMargin As Single
        Private m_fontAny As Font
        Private m_iCol As Integer
        Private m_iRow As Integer

        Private m_sPlotWidth As Single
        Private m_sPlotHeight As Single

        Private m_lPlots As New List(Of ShowAllFitsPlotData)

        Private m_bShowYear As Boolean
        Private m_bListWeight As Boolean
        Private m_nNumPlots As Integer
        Private m_NTimes As Integer

        Private m_lShownPlotsType As New List(Of eTimeSeriesType)

        Private m_sg As StyleGuide = StyleGuide.GetInstance()

#End Region ' Private vars

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            Me.m_core = cCore.GetInstance()
            Me.m_NTimes = m_core.nEcosimTimeSteps

            Me.LoadFormPos()
            Me.SetDefaultParams()
            Me.GatherPlotData()
            Me.CalcPlotParams()

            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleguideChanged

        End Sub

        Private Sub ShowAllFits_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleguideChanged

            SaveFormPos()
        End Sub

#Region " Rendering "

        Private Sub DrawPlots(ByRef g As Graphics, ByVal iWidth As Integer, ByVal iHeight As Integer)

            Dim pen As Pen = New Pen(Color.Black, m_sLineWidth)
            Dim ptfTL As PointF = toDevicePoint(New PointF(m_sLRMargin, 1.02F * m_iRow + m_sTBMargin), iWidth, iHeight)
            Dim szPosName As SizeF = toDeviceSize(New SizeF(0.02F, 0.02F), iWidth, iHeight)
            Dim pzPosGraph As SizeF = toDeviceSize(New SizeF(m_sPlotWidth, m_sPlotHeight), iWidth, iHeight)
            Dim iPlot As Integer = 0
            Dim plot As ShowAllFitsPlotData = Nothing
            Dim strTitle As String = ""
            Dim iRow, iCol As Integer
            Dim sPosX, sPosY As Single
            Dim data() As Single

            For i As Integer = 0 To Me.m_lPlots.Count - 1

                plot = Me.m_lPlots(i)

                If plot.Visible Then

                    iRow = CInt(Math.Floor(iPlot \ m_iCol))
                    iCol = iPlot Mod m_iCol
                    sPosX = ptfTL.X + iCol * pzPosGraph.Width
                    sPosY = ptfTL.Y + iRow * pzPosGraph.Height

                    ' ===============
                    ' Draw background
                    ' ===============
                    g.DrawRectangle(pen, sPosX, sPosY, pzPosGraph.Width, pzPosGraph.Height)

                    ' ===============
                    ' Draw title
                    ' ===============
                    If Me.m_bListWeight Then
                        strTitle = String.Format(My.Resources.GENERIC_LABEL_DETAILEDLABEL, plot.TimeSeries.Name, _
                            Me.m_sg.FormatNumber(plot.TimeSeries.WtType))
                    Else
                        strTitle = plot.TimeSeries.Name
                    End If
                    g.DrawString(strTitle, Me.m_fontAny, Brushes.Black, sPosX + szPosName.Width, sPosY + szPosName.Height)

                    ' Test axis for extreme values
                    If (Not Single.IsNaN(plot.YMax)) Then

                        g.Clip = New Region(New Rectangle(CInt(sPosX), CInt(sPosY), CInt(pzPosGraph.Width), CInt(pzPosGraph.Height)))

                        ' ===============
                        ' Draw time series
                        ' ===============
                        data = plot.TimeSeries.ShapeData
                        For k As Integer = 1 To data.Length - 1
                            If Math.Abs(data(k)) > 0 Then
                                Dim dotXRelPos As Single = CSng(m_sPlotWidth * (k - 0.5!) * (cCore.N_MONTHS / m_NTimes))
                                Dim dotYRelPos As Single = CSng(m_sPlotHeight * (1 - Math.Abs(data(k)) * plot.TSDataScale / plot.YMax))
                                Dim dotPos As SizeF = toDeviceSize(New SizeF(dotXRelPos, dotYRelPos), iWidth, iHeight)

                                If (dotYRelPos >= 0) Then
                                    g.DrawEllipse(pen, New RectangleF(sPosX + dotPos.Width - (0.5! * m_sDotSize), _
                                        sPosY + dotPos.Height - (0.5! * m_sDotSize), m_sDotSize, m_sDotSize))
                                End If
                            End If
                        Next

                        ' ===============
                        ' Draw results
                        ' ===============
                        data = plot.SimData
                        If Not data Is Nothing Then
                            For k As Integer = 1 To data.Length - 2

                                Dim x1RelPos As Single = m_sPlotWidth * k / m_NTimes
                                Dim x2RelPos As Single = m_sPlotWidth * (k + 1) / m_NTimes

                                Dim y1RelPos As Single = m_sPlotHeight * (1 - data(k) / plot.YMax)
                                Dim y2RelPos As Single = m_sPlotHeight * (1 - data(k + 1) / plot.YMax)

                                Dim p1Pos As SizeF = toDeviceSize(New SizeF(x1RelPos, y1RelPos), iWidth, iHeight)
                                Dim p2Pos As SizeF = toDeviceSize(New SizeF(x2RelPos, y2RelPos), iWidth, iHeight)

                                g.DrawLine(pen, sPosX + p1Pos.Width, sPosY + p1Pos.Height, sPosX + p2Pos.Width, sPosY + p2Pos.Height)

                            Next
                        Else
                            Console.WriteLine("ShowAllFits: Missing Ecosim data for results {0}, time series {1}", i, plot.TimeSeries.Name)
                        End If

                        ' Restore clip
                        g.Clip = New Region(New Rectangle(0, 0, iWidth, iHeight))

                    Else
                        Using p As New Pen(Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT))
                            g.DrawLine(p, sPosX, sPosY, sPosX + pzPosGraph.Width, sPosY + pzPosGraph.Height)
                            g.DrawLine(p, sPosX, sPosY + pzPosGraph.Height, sPosX + pzPosGraph.Width, sPosY)
                        End Using
                    End If

                    ' Plot handled
                    iPlot += 1
                End If
            Next i ' Next result

            If m_bShowYear Then

                Dim stepYear As Integer = m_NTimes \ (cCore.N_MONTHS * 4)
                Dim fs As Single
                If m_fontAny.Size > 12 Then
                    fs = m_fontAny.Size - 4
                Else
                    fs = 8.0!
                End If
                Dim fStyle As FontStyle = FontStyle.Regular
                If m_fontAny.Italic Then
                    fStyle = fStyle Or FontStyle.Italic
                End If

                Dim f As New Font(m_fontAny.FontFamily, fs, fStyle)

                Dim iEnd As Integer = m_iCol - 1
                If iPlot < m_iCol Then
                    iEnd = iPlot - 1
                End If
                For i As Integer = 0 To iEnd
                    Dim yPos As Single = ptfTL.Y + CInt(Math.Ceiling(iPlot / m_iCol)) * pzPosGraph.Height + toDeviceSize(New SizeF(0, 0.005F + m_sTBMargin), iWidth, iHeight).Height
                    For t As Integer = 0 To 3
                        g.DrawString((Me.m_core.EcosimFirstYear + t * stepYear).ToString, f, Brushes.Black, ptfTL.X + i * pzPosGraph.Width + (t * pzPosGraph.Width / 4), yPos)
                    Next

                Next
            End If

        End Sub

        Private Function GenerateImage() As System.Drawing.Image

            Dim img As Image = New Bitmap(pbPlots.Width, pbPlots.Height)
            Dim bg As Graphics = Graphics.FromImage(img)
            bg.Clear(pbPlots.BackColor)
            DrawPlots(bg, img.Width, img.Height)

            Return img

        End Function

#End Region ' Rendering

#Region " Internal mucky bits "

#Region " Form positioning "

        ' Maybe this one-hit wonder logic should move to EwEForm?

        Private Sub LoadFormPos()

            'Me.StartPosition = FormStartPosition.Manual
            'Me.Location = My.Settings.ShowAllFitsFormLocation
            'Me.Size = My.Settings.ShowAllFitsFormSize
            'Me.WindowState = My.Settings.ShowAllFitsFormWindowState

        End Sub

        Private Sub SaveFormPos()

            'If Me.WindowState = FormWindowState.Normal Then
            '    My.Settings.ShowAllFitsFormLocation = Me.Location
            '    My.Settings.ShowAllFitsFormSize = Me.Size
            'Else
            '    ' If window is maximized or minimized, use RestoreBounds to remember normal positions
            '    My.Settings.ShowAllFitsFormLocation = Me.RestoreBounds.Location
            '    My.Settings.ShowAllFitsFormSize = Me.RestoreBounds.Size
            'End If
            'My.Settings.ShowAllFitsFormWindowState = Me.WindowState
            'My.Settings.Save()

        End Sub

#End Region ' Form positioning

        ''' <summary>
        ''' Init the form with default values
        ''' </summary>
        Private Sub SetDefaultParams()

            ' Defaults
            m_sDotSize = 0.02 * 300
            m_sLineWidth = 1
            m_sLRMargin = 0.1
            m_sTBMargin = 0.1
            m_iCol = 3

            ' Update controls
            txbPlotsPerRow.Text = m_iCol.ToString
            txbDotSize.Text = (m_sDotSize / 300.0F).ToString
            txbLineWidth.Text = m_sLineWidth.ToString
            txbLRMargin.Text = m_sLRMargin.ToString
            txbTBMargin.Text = m_sTBMargin.ToString

            ' Defaults
            m_bShowYear = True
            m_bListWeight = True

            ' Select all options
            For i As Integer = 0 To clbOptions.Items.Count - 1
                clbOptions.SetItemChecked(i, True)
            Next

            ' Another default
            cbScaleFP.Checked = False

            ' Hmm
            SetPlotsType()
        End Sub

        Private Sub GatherPlotData()

            For iTS As Integer = 1 To m_core.nTimeSeries

                Dim ts As cTimeSeries = m_core.EcosimTimeSeries(iTS)
                Dim asSimData As Single() = Nothing

                If m_lShownPlotsType.Contains(ts.TimeSeriesType) Then

                    If TypeOf ts Is cGroupTimeSeries Then

                        Dim gts As cGroupTimeSeries = DirectCast(ts, cGroupTimeSeries)
                        Dim iGroup As Integer = gts.GroupIndex

                        ReDim asSimData(m_core.nEcosimTimeSteps)

                        Dim grpOutput As cEcosimGroupOutput = m_core.EcoSimGroupOutputs(iGroup)
                        For iTime As Integer = 1 To m_core.nEcosimTimeSteps
                            Select Case gts.TimeSeriesType

                                Case eTimeSeriesType.Catches, _
                                     eTimeSeriesType.CatchesForcing
                                    asSimData(iTime) = grpOutput.Yield(iTime)

                                Case eTimeSeriesType.TotalMortality
                                    asSimData(iTime) = grpOutput.TotalMort(iTime)

                                Case eTimeSeriesType.AverageWeight
                                    If grpOutput.isMultiStanza Then
                                        asSimData(iTime) = grpOutput.AvgWeight(iTime)
                                    End If

                                Case Else
                                    asSimData(iTime) = grpOutput.Biomass(iTime)

                            End Select
                        Next

                    End If
                End If

                Dim plot As New ShowAllFitsPlotData(ts, asSimData)
                Me.m_lPlots.Add(plot)
            Next iTS

        End Sub

        Private Sub CalcPlotParams()

            m_nNumPlots = RefreshTimeSeriesListbox()
            'm_NumPlots = 15

            If m_fontAny Is Nothing Then
                If m_nNumPlots < 19 Then
                    m_fontAny = New Font("Arial", 10, FontStyle.Bold)
                Else
                    m_fontAny = New Font("Arial", 8.25!, FontStyle.Bold)
                End If
            End If

            ' ToDo_JS: use EwEFormatProvider
            Try
                m_iCol = CInt(txbPlotsPerRow.Text)
                If m_iCol <= 0 Then m_iCol = 3
            Catch ex As Exception
                m_iCol = 3
            End Try
            m_sPlotWidth = 8.0F / m_iCol

            m_iRow = CInt(Math.Ceiling(m_nNumPlots / m_iCol))
            If cbScaleFP.Checked Then
                If m_iRow <= 10 Then m_iRow = 10
            End If
            m_sPlotHeight = 0.99F

            Try
                m_sLRMargin = CSng(txbLRMargin.Text)
                m_sTBMargin = CSng(txbTBMargin.Text)
                m_sDotSize = CSng(txbDotSize.Text) * 300.0F
                m_sLineWidth = CSng(txbLineWidth.Text)
            Catch ex As Exception

            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets the type of plots to show, based on user preferences of what to view.
        ''' </summary>
        ''' <remarks>
        ''' JS 11feb08: this method is called from SetDefaultParams
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub SetPlotsType()

            m_lShownPlotsType.Clear()

            For Each i As Integer In clbOptions.CheckedIndices
                If i = 0 Then
                    m_lShownPlotsType.Add(eTimeSeriesType.BiomassForcing)
                    m_lShownPlotsType.Add(eTimeSeriesType.BiomassRel)
                    m_lShownPlotsType.Add(eTimeSeriesType.BiomassAbs)
                ElseIf i = 1 Then
                    m_lShownPlotsType.Add(eTimeSeriesType.TotalMortality)
                ElseIf i = 2 Then
                    m_lShownPlotsType.Add(eTimeSeriesType.Catches)
                    m_lShownPlotsType.Add(eTimeSeriesType.CatchesForcing)
                End If
            Next

            m_lShownPlotsType.Add(eTimeSeriesType.AverageWeight)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Inadequately named method that toggles the visibility state of plots
        ''' based on user preferences.
        ''' </summary>
        ''' <returns>The number of visible plots</returns>
        ''' -------------------------------------------------------------------
        Private Function RefreshTimeSeriesListbox() As Integer

            Dim iNumVisiblePlots As Integer = 0
            Dim ts As cTimeSeries = Nothing

            For Each plot As ShowAllFitsPlotData In Me.m_lPlots
                ' Assume the worst
                plot.Visible = False
                ' Get TS
                ts = plot.TimeSeries
                ' Can show type?
                If m_lShownPlotsType.Contains(ts.TimeSeriesType) Then
                    ' Is applied?
                    If ts.Enabled() Then
                        ' Dunno what this is (yet)
                        If plot.Selected Then
                            ' Show the plot
                            plot.Visible = True
                            ' Count it
                            iNumVisiblePlots += 1
                        End If
                    End If
                End If
            Next

            Return iNumVisiblePlots

        End Function

        ''' <summary>
        ''' Helper method, transforms a model value (point) to a device value
        ''' </summary>
        ''' <param name="p"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <returns></returns>
        Private Function toDevicePoint(ByVal p As PointF, ByVal width As Integer, ByVal height As Integer) As PointF
            ' JS 11feb08: what are these hard-coded values 8, 2, 1.02F, 2.02F?
            '    8    : originates from EwE5
            '    2    : take both left and right margin into account
            '    1.02F:
            '    2.02F:
            ' Transforms the output value to the screen point value
            Dim screenPt As New PointF(p.X * width / (8 + 2 * m_sLRMargin), _
                            height - (height * p.Y) / (1.02F * m_iRow + 2.02F * m_sTBMargin))

            Return screenPt

        End Function

        ''' <summary>
        ''' Helper method, transforms a model value (size) to a device value
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function toDeviceSize(ByVal s As SizeF, ByVal width As Integer, ByVal height As Integer) As SizeF
            ' JS 11feb08: what are these hard-coded values 8, 2, 1.02F, 2.02F?
            '    8    :
            '    2    : take both left and right margin into account
            '    1.02F:
            '    2.02F:
            Dim size As New SizeF(width * s.Width / (8 + 2 * m_sLRMargin), _
                        height * s.Height / (1.02F * m_iRow + 2.02F * m_sTBMargin))

            Return size

        End Function

        Private Sub InvalidNumEntered(ByRef txb As TextBox, ByVal e As System.ComponentModel.CancelEventArgs)

            Try
                'If the user enters the non-positive number, we remind the user with an red icon. 
                If CSng(txb.Text) <= 0 Then
                    Me.epInput.SetError(txb, My.Resources.INVALID_NUMBER_ENTERED)
                    e.Cancel = True
                Else
                    Me.epInput.SetError(txb, "")
                End If
            Catch ex As Exception
                Me.epInput.SetError(txb, My.Resources.INVALID_NUMBER_ENTERED)
                e.Cancel = True
            End Try

        End Sub

#End Region ' Internal mucky bits

#Region " Event handlers "

        Private Sub btnFModify_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFModify.Click

            Dim dlgFont As New FontDialog
            dlgFont.Font = m_fontAny

            If dlgFont.ShowDialog <> Windows.Forms.DialogResult.Cancel Then

                m_fontAny = dlgFont.Font
                pbPlots.Invalidate()

            End If

        End Sub

        Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        ''' <summary>
        ''' HS = Hide / Show
        ''' </summary>
        Private Sub tsBtnHSPlots_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnHSPlots.Click

            Dim dlg As New dlgSelectAllFitsPlots(Me.m_lPlots)
            If (dlg.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                Me.CalcPlotParams()
                Me.pbPlots.Invalidate()
            End If

        End Sub

        Private Sub txbNum_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txbPlotsPerRow.Validated, txbTBMargin.Validated, txbLRMargin.Validated, txbLineWidth.Validated, txbDotSize.Validated
            CalcPlotParams()
            pbPlots.Invalidate()
        End Sub

        Private Sub txbPlotsPerRow_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txbPlotsPerRow.Validating
            Try
                'If the user enters the non-positive number, we remind the user with an red icon. 
                If CInt(txbPlotsPerRow.Text) <= 0 Then
                    Me.epInput.SetError(Me.txbPlotsPerRow, My.Resources.INVALID_NUMBER_ENTERED)
                    e.Cancel = True
                Else
                    Me.epInput.SetError(Me.txbPlotsPerRow, "")
                End If
            Catch ex As Exception
                Me.epInput.SetError(Me.txbPlotsPerRow, My.Resources.INVALID_NUMBER_ENTERED)
                e.Cancel = True
            End Try
        End Sub

        Private Sub txbLineWidth_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txbLineWidth.Validating
            InvalidNumEntered(txbLineWidth, e)
        End Sub

        Private Sub txbDotSize_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txbDotSize.Validating
            InvalidNumEntered(txbDotSize, e)
        End Sub

        Private Sub txbLRMargin_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txbLRMargin.Validating
            InvalidNumEntered(txbLRMargin, e)
        End Sub

        Private Sub txbTBMargin_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txbTBMargin.Validating
            InvalidNumEntered(txbTBMargin, e)
        End Sub

        Private Sub cbScaleFP_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbScaleFP.CheckedChanged
            CalcPlotParams()
            pbPlots.Invalidate()
        End Sub

        Private Sub tsBtnSaveImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnSaveImage.Click

            Dim img As Image = Nothing
            Dim imgFormat As System.Drawing.Imaging.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp
            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(My.Resources.FILEFILTER_IMAGE)
            If cmdFS.Result = Windows.Forms.DialogResult.OK Then

                Select Case cmdFS.FilterIndex
                    Case 1
                        imgFormat = System.Drawing.Imaging.ImageFormat.Bmp
                    Case 2
                        imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg
                    Case 3
                        imgFormat = System.Drawing.Imaging.ImageFormat.Gif
                    Case 4
                        imgFormat = System.Drawing.Imaging.ImageFormat.Png
                    Case 5
                        imgFormat = System.Drawing.Imaging.ImageFormat.Tiff
                    Case Else
                        Debug.Assert(False)
                End Select
                img = Me.GenerateImage()
                img.Save(cmdFS.FileName, imgFormat)
            End If

        End Sub

        Private Sub clbOptions_ItemCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles clbOptions.ItemCheck

            If e.Index <= 2 Then
                If e.Index = 0 Then
                    If e.NewValue = CheckState.Checked Then
                        m_lShownPlotsType.AddRange(New eTimeSeriesType() {eTimeSeriesType.BiomassRel, eTimeSeriesType.BiomassAbs, eTimeSeriesType.BiomassForcing})
                    ElseIf e.NewValue = CheckState.Unchecked Then
                        m_lShownPlotsType.Remove(eTimeSeriesType.BiomassRel)
                        m_lShownPlotsType.Remove(eTimeSeriesType.BiomassAbs)
                        m_lShownPlotsType.Remove(eTimeSeriesType.BiomassForcing)
                    End If
                ElseIf e.Index = 1 Then
                    If e.NewValue = CheckState.Checked Then
                        m_lShownPlotsType.Add(eTimeSeriesType.TotalMortality)
                    ElseIf e.NewValue = CheckState.Unchecked Then
                        m_lShownPlotsType.Remove(eTimeSeriesType.TotalMortality)
                    End If
                ElseIf e.Index = 2 Then
                    If e.NewValue = CheckState.Checked Then
                        m_lShownPlotsType.Add(eTimeSeriesType.Catches)
                        m_lShownPlotsType.Add(eTimeSeriesType.CatchesForcing)
                    ElseIf e.NewValue = CheckState.Unchecked Then
                        m_lShownPlotsType.Remove(eTimeSeriesType.Catches)
                        m_lShownPlotsType.Remove(eTimeSeriesType.CatchesForcing)
                    End If
                End If
                CalcPlotParams()
            End If

            If e.Index = 3 Then
                If e.NewValue = CheckState.Checked Then
                    m_bListWeight = True
                ElseIf e.NewValue = CheckState.Unchecked Then
                    m_bListWeight = False
                End If
            End If

            If e.Index = 4 Then
                If e.NewValue = CheckState.Checked Then
                    m_bShowYear = True
                ElseIf e.NewValue = CheckState.Unchecked Then
                    m_bShowYear = False
                End If
            End If

            pbPlots.Invalidate()

        End Sub

        Private Sub pbPlots_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pbPlots.Paint
            DrawPlots(e.Graphics, pbPlots.Width, pbPlots.Height)
        End Sub

        Private Sub tsBtnChangeYScale_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnChangeYScale.Click

            Dim dlgChYScale As New dlgChangeYScale(Me.m_lPlots)
            If (dlgChYScale.ShowDialog = Windows.Forms.DialogResult.OK) Then
                pbPlots.Invalidate()
            End If

        End Sub

        Private Sub OnStyleguideChanged(ByVal changeType As StyleGuide.eChangeType)
            ' Redraw
            Me.Invalidate()
        End Sub

#End Region ' Event handlers

#Region " Saving "

        Private Sub tsBtnSaveData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnSaveData.Click

            Dim strFileName As String = String.Empty
            Dim strPath As String = SelectDestFolder()
            Dim strTargetPath As String = ""
            Dim ts As cTimeSeries = Nothing
            Dim appl As AppLauncher = AppLauncher.GetInstance()

            If String.IsNullOrEmpty(strPath) Then Return

            ' ToDo_JS: Globalize this
            appl.SetStatusText("Saving fitting data...", TriState.True)

            For i As Integer = 1 To 3
                Select Case i
                    Case 1
                        strFileName = "Allfit_Biomass.csv"
                    Case 2 'Mortality Data
                        strFileName = "Allfit_Mortality.csv"
                    Case 3 'Catch Data
                        strFileName = "Allfit_Catches.csv"
                End Select

                strTargetPath = Path.Combine(strPath, strFileName)

                Using sw As StreamWriter = New StreamWriter(strTargetPath, False)

                    sw.Write("Year")
                    sw.Write(",")
                    For j As Integer = 1 To m_core.nTimeSeries
                        ts = m_core.EcosimTimeSeries(j)
                        Select Case i
                            Case 1
                                If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                                    sw.Write(ts.Name)
                                    sw.Write(",")
                                End If
                            Case 2
                                If ts.TimeSeriesType = eTimeSeriesType.TotalMortality Then
                                    sw.Write(String.Format("{0} Z", ts.Name))
                                    sw.Write(",")
                                End If
                            Case 3
                                If ts.TimeSeriesType = eTimeSeriesType.Catches Or ts.TimeSeriesType = eTimeSeriesType.CatchesForcing Then
                                    sw.Write(String.Format("{0} Yield", ts.Name))
                                    sw.Write(",")
                                End If
                        End Select
                    Next
                    sw.WriteLine()

                    Dim iPt As Integer = CInt(Math.Floor(cCore.N_MONTHS / 2))
                    For k As Integer = 1 To m_core.nEcosimTimeSteps \ 12
                        sw.Write((Me.m_core.EcosimFirstYear + k - 1).ToString)
                        sw.Write(",")
                        For j As Integer = 0 To Me.m_lPlots.Count - 1
                            Dim plot As ShowAllFitsPlotData = Me.m_lPlots(i)
                            If Not plot.SimData Is Nothing Then
                                ts = plot.TimeSeries
                                Select Case i
                                    Case 1
                                        If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                                            sw.Write(plot.SimData(iPt))
                                            sw.Write(",")
                                            If ts.ShapeData(k) > 0 Then
                                                If ts.DataQ > 0 Then
                                                    sw.Write(ts.ShapeData(k) / ts.DataQ)
                                                Else
                                                    sw.Write(ts.ShapeData(k))
                                                End If
                                                sw.Write(",")
                                            Else
                                                sw.Write(" ")
                                                sw.Write(",")
                                            End If
                                        End If
                                    Case 2
                                        If ts.TimeSeriesType = eTimeSeriesType.TotalMortality Then
                                            sw.Write(plot.SimData(iPt))
                                            sw.Write(",")
                                            If ts.ShapeData(k) > 0 Then
                                                If ts.DataQ > 0 Then
                                                    sw.Write(ts.ShapeData(k) / ts.DataQ)
                                                Else
                                                    sw.Write(ts.ShapeData(k))
                                                End If
                                                sw.Write(",")
                                            Else
                                                sw.Write(" ")
                                                sw.Write(",")
                                            End If
                                        End If
                                    Case 3
                                        If ts.TimeSeriesType = eTimeSeriesType.Catches Or ts.TimeSeriesType = eTimeSeriesType.CatchesForcing Then
                                            sw.Write(plot.SimData(iPt))
                                            sw.Write(",")
                                            If ts.ShapeData(k) > 0 Then
                                                sw.Write(ts.ShapeData(k))
                                                sw.Write(",")
                                            Else
                                                sw.Write(" ")
                                                sw.Write(",")
                                            End If
                                        End If
                                End Select
                            End If

                        Next
                        iPt = iPt + cCore.N_MONTHS
                        sw.WriteLine()
                    Next

                    sw.Close()

                End Using
            Next

            ' Clear status text
            appl.SetStatusText("", TriState.False)

            ' Notify user
            ' ToDo_JS: globalize this
            Me.m_core.Messages.SendMessage(New cMessage(String.Format("All fits data are saved to: {0}", strPath), _
                    eMessageType.NotSet, eCoreComponentType.EcoSim, eMessageImportance.Information))

        End Sub

        Private Function SelectDestFolder() As String

            Dim fbDlg As New FolderBrowserDialog

            fbDlg.SelectedPath = My.Settings.LastSelectedDirectory
            fbDlg.ShowNewFolderButton = True
            fbDlg.Description = My.Resources.ECOSIM_SAVE_ALL_FITS_VALUE_FOLDER_SELECTION

            If fbDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Return fbDlg.SelectedPath
            Else
                Return String.Empty
            End If

        End Function

#End Region ' Saving

#Region " Printing "

        Private Sub tsBtnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnPrint.Click

            PrintDialog1.Document = pdAllFits
            If PrintDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                pdAllFits.DocumentName = "Show all fits"
                pdAllFits.Print()
            End If

        End Sub

        Private Sub tsBtnPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsBtnPrintPreview.Click
            Try
                dlgPV.Document = pdAllFits
                dlgPV.ShowDialog()
            Catch ex As Exception
                Throw New Exception("Not able to preview: " & ex.ToString)
            End Try

        End Sub

        Private Sub pdAllFits_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles pdAllFits.PrintPage

            Dim g As Graphics = e.Graphics
            DrawPlots(g, e.MarginBounds.Width, e.MarginBounds.Height)

        End Sub

#End Region ' Printing

    End Class

End Namespace



