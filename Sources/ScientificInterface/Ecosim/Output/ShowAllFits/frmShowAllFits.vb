#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports System.Windows.Forms
Imports System.IO
Imports ScientificInterface.Other
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form implementing the Ecosim Show All Fits interface.
    ''' </summary>
    ''' <remarks>
    ''' Why are we not using ZedGraph here?!
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class frmShowAllFits

#Region " Private vars "

        Private m_sDotSize As Single
        Private m_sLineWidth As Single
        Private m_sLRMargin As Single
        Private m_sTBMargin As Single
        Private m_iCol As Integer
        Private m_iRow As Integer

        Private m_sPlotWidth As Single
        Private m_sPlotHeight As Single

        Private m_lPlots As New List(Of cShowAllFitsPlotData)

        Private m_nNumPlots As Integer
        Private m_NTimes As Integer

        Private m_lShownPlotsType As New List(Of eTimeSeriesType)

#End Region ' Private vars

        Public Sub New()

            Me.InitializeComponent()

        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_NTimes = Me.Core.nEcosimTimeSteps
            Me.SetDefaultParams()
            Me.GatherPlotData()
            Me.CalcPlotParams()

            AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleguideChanged

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim}
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleguideChanged
            MyBase.OnFormClosed(e)
        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)
            Me.m_pbPlots.Invalidate()
        End Sub

#Region " Rendering "

        Private Sub DrawPlots(ByRef g As Graphics, ByVal iWidth As Integer, ByVal iHeight As Integer)

            Dim pen As Pen = New Pen(Color.Black, m_sLineWidth)
            Dim ptfTL As PointF = toDevicePoint(New PointF(m_sLRMargin, 1.02F * m_iRow + m_sTBMargin), iWidth, iHeight)
            Dim szPosName As SizeF = toDeviceSize(New SizeF(0.02F, 0.02F), iWidth, iHeight)
            Dim pzPosGraph As SizeF = toDeviceSize(New SizeF(m_sPlotWidth, m_sPlotHeight), iWidth, iHeight)
            Dim iPlot As Integer = 0
            Dim plot As cShowAllFitsPlotData = Nothing
            Dim strTitle As String = ""
            Dim iRow, iCol As Integer
            Dim sPosX, sPosY As Single
            Dim data() As Single
            Dim ftCaption As Font = Me.StyleGuide.Font(cStyleGuide.eApplicationFontType.SubTitle)
            Dim ftScale As Font = Me.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)

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
                    If Me.m_chkShowWeight.Checked Then
                        strTitle = String.Format(My.Resources.GENERIC_LABEL_DETAILEDLABEL, plot.TimeSeries.Name, _
                            Me.StyleGuide.FormatNumber(plot.TimeSeries.WtType))
                    Else
                        strTitle = plot.TimeSeries.Name
                    End If

                    g.DrawString(strTitle, ftCaption, Brushes.Black, sPosX + szPosName.Width, sPosY + szPosName.Height)

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
                        Using p As New Pen(Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT))
                            g.DrawLine(p, sPosX, sPosY, sPosX + pzPosGraph.Width, sPosY + pzPosGraph.Height)
                            g.DrawLine(p, sPosX, sPosY + pzPosGraph.Height, sPosX + pzPosGraph.Width, sPosY)
                        End Using
                    End If

                    ' Plot handled
                    iPlot += 1
                End If
            Next i ' Next result

            If Me.m_chkShowYear.Checked Then

                Dim stepYear As Integer = m_NTimes \ (cCore.N_MONTHS * 4)
                'Dim fs As Single

                Dim iEnd As Integer = m_iCol - 1
                If iPlot < m_iCol Then
                    iEnd = iPlot - 1
                End If
                For i As Integer = 0 To iEnd
                    Dim yPos As Single = ptfTL.Y + CInt(Math.Ceiling(iPlot / m_iCol)) * pzPosGraph.Height + toDeviceSize(New SizeF(0, 0.005F + m_sTBMargin), iWidth, iHeight).Height
                    For t As Integer = 0 To 3
                        g.DrawString((Me.Core.EcosimFirstYear + t * stepYear).ToString, ftScale, Brushes.Black, ptfTL.X + i * pzPosGraph.Width + (t * pzPosGraph.Width / 4), yPos)
                    Next

                Next
            End If

            ftCaption.Dispose()
            ftScale.Dispose()

        End Sub

        Private Function GenerateImage() As System.Drawing.Image

            Dim img As Image = New Bitmap(m_pbPlots.Width, m_pbPlots.Height)
            Dim bg As Graphics = Graphics.FromImage(img)
            bg.Clear(m_pbPlots.BackColor)
            DrawPlots(bg, img.Width, img.Height)

            Return img

        End Function

#End Region ' Rendering

#Region " Internal mucky bits "

        ''' <summary>
        ''' Init the form with default values
        ''' </summary>
        Private Sub SetDefaultParams()

            ' Defaults
            m_sDotSize = 3
            m_sLineWidth = 1
            m_sLRMargin = 0.1
            m_sTBMargin = 0.1
            m_iCol = 3

            ' Update controls
            Me.m_nudRowNum.Value = Me.m_iCol
            Me.m_nudDotSize.Value = CDec(Me.m_sDotSize)
            Me.m_nudLineWidth.Value = CDec(Me.m_sLineWidth)
            Me.m_nudMarginLR.Value = CDec(Me.m_sLRMargin)
            Me.m_nudMarginTB.Value = CDec(Me.m_sTBMargin)

            ' Hmm
            SetPlotTypes()
        End Sub

        Private Sub GatherPlotData()

            Dim ts As cTimeSeries = Nothing
            Dim asSimData(Me.Core.nEcosimTimeSteps) As Single
            Dim plot As cShowAllFitsPlotData = Nothing
            Dim gts As cGroupTimeSeries = Nothing
            Dim iGroup As Integer = 0
            Dim grpOutput As cEcosimGroupOutput = Nothing

            For iTS As Integer = 1 To Me.Core.nTimeSeries

                ts = Me.Core.EcosimTimeSeries(iTS)

                If m_lShownPlotsType.Contains(ts.TimeSeriesType) Then

                    If TypeOf ts Is cGroupTimeSeries Then

                        gts = DirectCast(ts, cGroupTimeSeries)
                        iGroup = gts.GroupIndex

                        If (iGroup > 0) Then

                            grpOutput = Me.Core.EcoSimGroupOutputs(iGroup)

                            For iTime As Integer = 1 To Me.Core.nEcosimTimeSteps

                                asSimData(iTime) = 0.0!

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
                End If

                plot = New cShowAllFitsPlotData(ts, asSimData)
                Me.m_lPlots.Add(plot)
            Next iTS

        End Sub

        Private Sub CalcPlotParams()

            Me.m_nNumPlots = Me.SetPlotVisibility()
            'm_NumPlots = 15

            Me.m_iCol = CInt(Me.m_nudRowNum.Value)
            If Me.m_iCol <= 0 Then Me.m_iCol = 3
            Me.m_sPlotWidth = 8.0F / Me.m_iCol

            Me.m_iRow = CInt(Math.Ceiling(Me.m_nNumPlots / Me.m_iCol))
            If Me.m_chkScaleForPrinter.Checked Then
                If Me.m_iRow <= 10 Then Me.m_iRow = 10
            End If

            If Me.m_chkShowYear.Checked Then
                Me.m_sPlotHeight = 0.99F
            Else
                Me.m_sPlotHeight = 1.0F
            End If

            Me.m_sLRMargin = CSng(Me.m_nudMarginLR.Value)
            Me.m_sTBMargin = CSng(Me.m_nudMarginTB.Value)
            Me.m_sDotSize = CSng(Me.m_nudDotSize.Value)
            Me.m_sLineWidth = CSng(Me.m_nudLineWidth.Value)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets the type of plots to show, based on user preferences of what to view.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SetPlotTypes()

            Me.m_lShownPlotsType.Clear()

            If Me.m_chkShowB.Checked Then
                Me.m_lShownPlotsType.Add(eTimeSeriesType.BiomassForcing)
                Me.m_lShownPlotsType.Add(eTimeSeriesType.BiomassRel)
                Me.m_lShownPlotsType.Add(eTimeSeriesType.BiomassAbs)
            End If

            If Me.m_chkShowZ.Checked Then
                m_lShownPlotsType.Add(eTimeSeriesType.TotalMortality)
            End If

            If Me.m_chkShowCatch.Checked Then
                m_lShownPlotsType.Add(eTimeSeriesType.Catches)
                m_lShownPlotsType.Add(eTimeSeriesType.CatchesForcing)
            End If

            Me.m_lShownPlotsType.Add(eTimeSeriesType.AverageWeight)

            Me.UpdatePlots()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Toggle the visibility state of plots based on user preferences.
        ''' </summary>
        ''' <returns>The number of visible plots</returns>
        ''' -------------------------------------------------------------------
        Private Function SetPlotVisibility() As Integer

            Dim iNumVisiblePlots As Integer = 0
            Dim ts As cTimeSeries = Nothing

            For Each plot As cShowAllFitsPlotData In Me.m_lPlots
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, transforms a model value (point) to a device value
        ''' </summary>
        ''' <param name="p"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, transforms a model value (size) to a device value
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -------------------------------------------------------------------
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the plots by recalculating content and redrawing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdatePlots()
            Me.CalcPlotParams()
            Me.m_pbPlots.Invalidate()
        End Sub

#Region " Printing "

        Private Sub pdAllFits_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) _
            Handles m_printdocAllFits.PrintPage
            Dim g As Graphics = e.Graphics
            DrawPlots(g, e.MarginBounds.Width, e.MarginBounds.Height)
        End Sub

#End Region ' Printing

#Region " Saving "

        Private Sub SaveToCSV(ByVal strPath As String)

            Dim strFileName As String = Me.Core.EwEModel.Name
            Dim strTargetPath As String = ""
            Dim ts As cTimeSeries = Nothing
            Dim plot As cShowAllFitsPlotData = Nothing
            Dim sw As StreamWriter = Nothing
            Dim msg As cMessage = Nothing
            Dim bSucces As Boolean = True

            If String.IsNullOrEmpty(strPath) Then Return

            msg = New cMessage(String.Format(My.Resources.STATUS_DATA_SAVING_SUCCESS, strPath), _
                               eMessageType.NotSet, eCoreComponentType.EcoSim, eMessageImportance.Information)

            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_PLEASE_WAIT, TriState.True)

            For i As Integer = 1 To 3
                Select Case i
                    Case 1
                        strFileName &= "_Allfit_Biomass.csv"
                    Case 2 'Mortality Data
                        strFileName &= "_Allfit_Mortality.csv"
                    Case 3 'Catch Data
                        strFileName &= "_Allfit_Catches.csv"
                End Select
                strTargetPath = Path.Combine(strPath, cFileUtils.ToValidFileName(strFileName, False))

                Try
                    sw = New StreamWriter(strTargetPath, False)
                Catch ex As Exception
                    ' Notify user
                    msg = New cMessage(String.Format(My.Resources.STATUS_DATA_SAVING_FAILURE, strPath, ex.Message), _
                            eMessageType.NotSet, eCoreComponentType.EcoSim, eMessageImportance.Critical)
                    bSucces = False
                    sw = Nothing
                End Try

                If (sw IsNot Nothing) Then

                    Try

                        sw.Write("Year")
                        sw.Write(",")
                        For j As Integer = 1 To Me.Core.nTimeSeries

                            plot = Me.m_lPlots(j - 1)
                            ts = plot.TimeSeries

                            Select Case i
                                Case 1
                                    If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                                        sw.Write("Predicted " & ts.Name)
                                        sw.Write(",")
                                        sw.Write("Observed " & ts.Name)
                                        sw.Write(",")
                                    End If
                                Case 2
                                    If ts.TimeSeriesType = eTimeSeriesType.TotalMortality Then
                                        sw.Write(String.Format("Predicted {0} Z", ts.Name))
                                        sw.Write(",")
                                        sw.Write(String.Format("Observed {0} Z", ts.Name))
                                        sw.Write(",")
                                    End If
                                Case 3
                                    If ts.TimeSeriesType = eTimeSeriesType.Catches Or ts.TimeSeriesType = eTimeSeriesType.CatchesForcing Then
                                        sw.Write(String.Format("Predicted {0} Yield", ts.Name))
                                        sw.Write(",")
                                        sw.Write(String.Format("Observed {0} Yield", ts.Name))
                                        sw.Write(",")
                                    End If
                            End Select
                        Next
                        sw.WriteLine()

                        Dim iPt As Integer = CInt(Math.Floor(cCore.N_MONTHS / 2))
                        For k As Integer = 1 To Me.Core.nEcosimTimeSteps \ 12
                            sw.Write((Me.Core.EcosimFirstYear + k - 1).ToString)
                            sw.Write(",")
                            For j As Integer = 1 To Me.m_lPlots.Count

                                plot = Me.m_lPlots(j - 1)
                                ts = plot.TimeSeries

                                Select Case i
                                    Case 1
                                        If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                                            sw.Write(plot.SimData(iPt))
                                            sw.Write(",")
                                            If ts.ShapeData(k) > 0 Then
                                                sw.Write(ts.ShapeData(k) * plot.TSDataScale)
                                            Else
                                                sw.Write(" ")
                                            End If
                                            sw.Write(",")
                                        End If
                                    Case 2
                                        If ts.TimeSeriesType = eTimeSeriesType.TotalMortality Then
                                            sw.Write(plot.SimData(iPt))
                                            sw.Write(",")
                                            If ts.ShapeData(k) > 0 Then
                                                sw.Write(ts.ShapeData(k) * plot.TSDataScale)
                                            Else
                                                sw.Write(" ")
                                            End If
                                            sw.Write(",")
                                        End If
                                    Case 3
                                        If ts.TimeSeriesType = eTimeSeriesType.Catches Or ts.TimeSeriesType = eTimeSeriesType.CatchesForcing Then
                                            sw.Write(plot.SimData(iPt))
                                            sw.Write(",")
                                            If ts.ShapeData(k) > 0 Then
                                                sw.Write(ts.ShapeData(k) * plot.TSDataScale)
                                            Else
                                                sw.Write(" ")
                                            End If
                                            sw.Write(",")
                                        End If
                                End Select

                            Next
                            iPt = iPt + cCore.N_MONTHS
                            sw.WriteLine()
                        Next
                    Catch ex As Exception
                        msg = New cMessage(String.Format(My.Resources.STATUS_DATA_SAVING_FAILURE, strPath, ex.Message), _
                                           eMessageType.NotSet, eCoreComponentType.EcoSim, eMessageImportance.Critical)
                        bSucces = False
                    Finally
                        sw.Close()
                    End Try

                End If

                If bSucces = False Then
                    Exit For
                End If

            Next

            ' Clear status text
            cApplicationStatusNotifier.SetStatusText("", TriState.False)

            ' Notify user
            Me.Core.Messages.SendMessage(msg)

        End Sub

#End Region ' Saving

#End Region ' Internal mucky bits

#Region " Event handlers "

        Private Sub OnSaveAsCSVClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiSaveAsCSV.Click

            Dim dlg As New FolderBrowserDialog()

            dlg.SelectedPath = My.Settings.LastSelectedDirectory
            dlg.ShowNewFolderButton = True
            dlg.Description = My.Resources.PROMPT_FOLDER_SELECTION

            If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Me.SaveToCSV(dlg.SelectedPath)
            End If

        End Sub

        Private Sub OnSaveAsImage(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiSaveAsImage.Click

            Dim img As Image = Nothing
            Dim imgFormat As System.Drawing.Imaging.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp
            Dim cmdh As cCommandHandler = Me.CommandHandler
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

        Private Sub OnPrintClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiPrint.Click

            Dim dlgPrint As New PrintDialog()
            dlgPrint.UseEXDialog = True

            dlgPrint.Document = m_printdocAllFits
            If dlgPrint.ShowDialog() = Windows.Forms.DialogResult.OK Then
                ' ToDo: globalize this
                m_printdocAllFits.DocumentName = "Show all fits"
                m_printdocAllFits.Print()
            End If

        End Sub

        Private Sub OnPrintPreviewClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiPrintPreview.Click

            Dim dlg As New PrintPreviewDialog()
            Dim msg As cMessage = Nothing

            dlg.Document = Me.m_printdocAllFits

            Try
                dlgPV.ShowDialog()
            Catch ex As Exception
                msg = New cMessage("Unable to preview: " & ex.ToString, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Critical)
                Me.Core.Messages.SendMessage(msg)
            End Try

        End Sub

        Private Sub OnShowDataChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_chkShowB.CheckedChanged, _
                    m_chkShowZ.CheckedChanged, _
                    m_chkShowCatch.CheckedChanged
            Me.SetPlotTypes()
        End Sub

        Private Sub OnShowDetailsChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_chkShowWeight.CheckedChanged, _
                    m_chkShowYear.CheckedChanged, _
                    m_chkScaleForPrinter.CheckedChanged
            Me.UpdatePlots()
        End Sub

        Private Sub OnDotSizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudDotSize.ValueChanged
            Me.UpdatePlots()
        End Sub

        Private Sub OnLineWidthChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudLineWidth.ValueChanged
            Me.UpdatePlots()
        End Sub

        Private Sub OnRowNumChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudRowNum.ValueChanged
            Me.UpdatePlots()
        End Sub

        Private Sub OnMarginLRChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudMarginLR.ValueChanged
            Me.UpdatePlots()
        End Sub

        Private Sub OnMarginTBChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudMarginTB.ValueChanged
            Me.UpdatePlots()
        End Sub

        ''' <summary>
        ''' HS = Hide / Show
        ''' </summary>
        Private Sub tsBtnHSPlots_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiChoosePlots.Click

            Dim dlg As New dlgSelectAllFitsPlots(Me.m_lPlots)
            If (dlg.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                Me.UpdatePlots()
            End If

        End Sub

        Private Sub OnPaintPlots(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles m_pbPlots.Paint

            Me.DrawPlots(e.Graphics, m_pbPlots.Width, m_pbPlots.Height)

        End Sub

        Private Sub tsBtnChangeYScale_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbnScale.Click

            Dim dlgChYScale As New dlgChangeYScale(Me.m_lPlots)
            If (dlgChYScale.ShowDialog = Windows.Forms.DialogResult.OK) Then
                m_pbPlots.Invalidate()
            End If

        End Sub

        Private Sub OnStyleguideChanged(ByVal changeType As cStyleGuide.eChangeType)
            ' Redraw
            Me.Invalidate()
        End Sub

        Private Sub OnToggleOptions(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiOptions.Click
            Me.m_scMain.Panel1Collapsed = (Me.m_tsmiOptions.Checked = False)
        End Sub

#End Region ' Event handlers

    End Class

End Namespace



