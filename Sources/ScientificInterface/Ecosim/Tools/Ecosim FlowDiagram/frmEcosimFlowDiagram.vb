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
Imports System.Data
Imports System.Drawing.Imaging
Imports System.IO
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form presenting the Ecosim Flow Diagram interface.
    ''' </summary>
    ''' <remarks>
    ''' Thank you, Naveen and Mark (USGS), for this contribution!
    ''' </remarks>
    ''' =======================================================================
    Public Class frmEcosimFlowDiagram
        Inherits frmEwE

#Region " Private variables "

        Private components As System.ComponentModel.IContainer = Nothing
        Private m_data As cEcosimFlowDiagramData = Nothing
        Private m_doodler As cFlowDiagramManager = Nothing
        Private m_tree As cEcosimTreeFlowDiagramRenderer = Nothing

        Private m_bMouseDown As Boolean = False
        Private WithEvents m_pbFlowDiagram As System.Windows.Forms.PictureBox
        Private WithEvents m_scContent As System.Windows.Forms.SplitContainer
        Private WithEvents m_tsFlowDiagram As cEwEToolstrip
        Private WithEvents m_pgFlowDiagram As System.Windows.Forms.PropertyGrid
        Private WithEvents m_tsbnExport As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnImport As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsmiSaveToImage As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tss2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsmiSettings As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tbxTimeStep As TextBox
        Private WithEvents m_slider As ScientificInterfaceShared.Controls.ucSlider
        Private WithEvents m_lblTimeStep As System.Windows.Forms.Label
        Private WithEvents m_lblMonth As System.Windows.Forms.Label
        Private WithEvents m_lblYear As System.Windows.Forms.Label
        Private WithEvents m_tbxMonth As TextBox
        Private WithEvents m_tbxYear As TextBox
        Private m_noofTimeSlicesPerYear As Integer
        Private TimeSeriesds As EwECore.cTimeSeriesDataset
        Private m_EcosimFirstYear As Integer
        Private m_dataGridViewCellStyle As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        'ShowBiomassLegend
        Private m_dt As DataTable
        Private m_dr0 As DataRow
        Private m_dr1 As DataRow
        Private m_dataGridView As DataGridView
        Private m_noofColumns As Integer = 12 'Noof colums are 12 for DataTable. 
        Private m_dataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Private m_dataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        'ShowFlowRateLegend
        Private m_mdataGridView As DataGridView
        Private m_iHighlightedNodePrev As Integer
        Private WithEvents m_tbxDelay As System.Windows.Forms.TextBox
        Private WithEvents m_lblDelay As System.Windows.Forms.Label
        Private WithEvents m_btnPlay As System.Windows.Forms.Button
        Private m_iHighlightedNode As Integer
        Private WithEvents m_btnStop As System.Windows.Forms.Button

        'SaveToBatchImages
        Private WithEvents m_tsmiSaveToBatchImage As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tss3 As System.Windows.Forms.ToolStripSeparator

        ' JS 03Mar15: Obtained from StyleGuide
        'Private preferdDPI As Integer = 300  

        Private m_fpDelay As cEwEFormatProvider = Nothing
        Private WithEvents m_tsmiResetLayout As System.Windows.Forms.ToolStripButton

        ' -- Animation(Play/Stop) --

        Private Enum eAnimationState As Integer
            Idle = 0
            Playing
            Paused
            Stopping
        End Enum

        Private m_animationstate As eAnimationState = eAnimationState.Idle

#End Region ' Private variables

#Region " Constructor/Destructor "

        Public Sub New()

            Me.InitializeComponent()

            ' This draws the control whenever it is resized
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            ' This supports mouse movement such as the mouse wheel
            Me.SetStyle(ControlStyles.UserMouse, True)

        End Sub

        Public Sub New(text As String)
            Me.New()
            'Set tab text
            Me.TabText = text
            ' Set the windows text
            Me.Text = text
        End Sub

#End Region ' Constructor 

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_tsbnImport.Image = SharedResources.ImportHS
            Me.m_tsbnExport.Image = SharedResources.ExportHS
            Me.m_tsmiResetLayout.Image = SharedResources.ResetHS

            Me.m_data = New cEcosimFlowDiagramData(Me.UIContext)
            Me.m_tree = New cEcosimTreeFlowDiagramRenderer(Me.m_data)
            Me.m_doodler = New cFlowDiagramManager(Me.m_data, Me.m_tree)

            Me.m_pgFlowDiagram.SelectedObject = Me.m_tree
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoPath}
            Me.UpdateControls()

            AddHandler Me.m_tree.OnChanged, AddressOf Me.OnTreeChanged
            AddHandler Me.m_tree.OnBiomassLegendChanged, AddressOf Me.OnTreeBiomassLegendChanged
            AddHandler Me.m_tree.OnFlowRateLegendChanged, AddressOf Me.OnTreeFlowRateLegendChanged

            'Slider Overriders
            Me.m_slider.Minimum = 1
            Me.m_slider.Maximum = Me.Core.nEcosimTimeSteps

            Me.m_noofTimeSlicesPerYear = Me.Core.EcoSimModelParameters.NumberSummaryTimeSteps

            'Check if the Loaded Model has timeseries Datasets
            Dim firstMonth As Integer = 2
            If Me.UIContext.Core.nTimeSeriesDatasets > 0 Then
                Me.TimeSeriesds = Me.UIContext.Core.TimeSeriesDataset(1)
                Me.m_EcosimFirstYear = Me.TimeSeriesds.FirstYear
                Me.m_tbxYear.Text = Me.m_EcosimFirstYear.ToString
                Me.m_tbxMonth.Text = cDateUtils.GetMonthName(firstMonth, False)
            Else
                Me.m_EcosimFirstYear = 0
                Me.m_tbxYear.Text = Me.m_EcosimFirstYear.ToString
                Me.m_tbxMonth.Text = cDateUtils.GetMonthName(firstMonth, False)
            End If

            'Default delay value of Slider Animation: 10ms, value range [10, 1000] ms
            Dim md As New cVariableMetaData(10, 1000, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            Me.m_fpDelay = New cEwEFormatProvider(Me.UIContext, Me.m_tbxDelay, GetType(Integer), md)
            Me.m_fpDelay.Value = 10

            ' Restore last layout for this scenario. This needs to be done before the tree gets configured next...
            Dim ad As cAuxiliaryData = Me.Core.AuxillaryData(Me.DataName())
            Me.m_doodler.Load(ad.Settings, Me.m_pbFlowDiagram)
            Me.m_tree.Load(ad.Settings)

            'Displaying Biomass values on Ecosim FlowDiagram
            Me.m_tree.NodeDrawValue = True
            Me.m_tree.Title = Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex).Name

        End Sub

        Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)

            If (Me.m_animationstate = eAnimationState.Playing) Then
                Me.m_animationstate = eAnimationState.Stopping
            End If
            MyBase.OnFormClosing(e)

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            RemoveHandler Me.m_tree.OnChanged, AddressOf Me.OnTreeChanged

            Me.m_fpDelay.Release()

            MyBase.OnFormClosed(e)

        End Sub

        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)

            ' Refresh the diagram data when ecopath data has changed
            If (msg.Source = eCoreComponentType.EcoPath) And
               (msg.Type = eMessageType.DataModified) Then
                Me.m_data.Refresh()
                Me.m_pbFlowDiagram.Invalidate()
            End If

        End Sub

        Protected Overrides Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)
            Me.m_pbFlowDiagram.Invalidate()
        End Sub

        Protected Overrides Function GetPrintContent(rcPrint As Rectangle) As Image

            Dim dpi As Integer = Me.StyleGuide.PreferredDPI
            Dim img As New Bitmap(rcPrint.Width, rcPrint.Height, PixelFormat.Format32bppArgb)
            img.SetResolution(dpi, dpi)
            Dim g As Graphics = Graphics.FromImage(img)
            Me.m_doodler.DrawFlowDiagram(g, rcPrint)
            g.Dispose()
            Return img

        End Function

#End Region ' Overrides

#Region " Events "

#Region " Drawing "

        Private Sub OnFlowDiagramResize(sender As Object, e As System.EventArgs) _
            Handles m_pbFlowDiagram.Resize
            Me.m_pbFlowDiagram.Invalidate()
        End Sub

        Private Sub OnFlowDiagramPaint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) _
            Handles m_pbFlowDiagram.Paint

            Dim rc As Rectangle = Me.m_pbFlowDiagram.ClientRectangle
            Me.m_doodler.DrawFlowDiagram(e.Graphics, rc)

        End Sub

        ''' <summary>
        ''' Override the bakcground paint routine to elimate flickering.
        ''' </summary>
        ''' <param name="pevent"></param>
        Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
            ' NOP
        End Sub

#End Region ' Drawing

#Region " Mouse Events "

        Private Sub OnFlowDiagramMouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) _
            Handles m_pbFlowDiagram.MouseDown

            Using g As Graphics = Me.CreateGraphics()
                Me.m_doodler.BeginDrag(Me.m_pbFlowDiagram.ClientRectangle, e.Location, g)
            End Using

        End Sub

        Private Sub OnFlowDiagramMouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) _
            Handles m_pbFlowDiagram.MouseUp
            Me.m_doodler.EndDrag(Me.m_data, e.Location)
        End Sub

        Private Sub OnFlowDiagramMouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) _
            Handles m_pbFlowDiagram.MouseMove

            Using g As Graphics = Me.CreateGraphics()
                Me.m_doodler.ProcessMouseMove(g, Me.m_pbFlowDiagram.ClientRectangle, e.Location)
            End Using
            Me.m_pbFlowDiagram.Invalidate()  'This redraws the FlowDiag with highlighted node
        End Sub


        Private Sub OnFlowDiagramMouseClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) _
            Handles m_pbFlowDiagram.MouseClick

            ' ToDo: globalize this method

            If Me.m_tree.ShowFlowRateLegend = True Then

                Me.m_iHighlightedNode = Me.m_doodler.HighlightNode

                If (Me.m_iHighlightedNode > 0) Then

                    'DataTable should be created only Once for highlighNode
                    If (Me.m_iHighlightedNode <> Me.m_iHighlightedNodePrev) Then

                        Try  'Delete previous DataTable if still exiting on the form

                            Me.m_mdataGridView.Dispose()  'Dellocating all the resources to DataGridView
                            Me.m_mdataGridView.ClearSelection()
                            Me.m_pbFlowDiagram.Controls.Remove(Me.m_mdataGridView)

                        Catch
                            'Noting
                        End Try

                        Me.m_mdataGridView = New DataGridView
                        Me.m_mdataGridView.Columns.Add("column", "header")
                        Me.m_mdataGridView.Rows.Add()
                        Me.m_mdataGridView.Rows.Add()
                        'adding first column values
                        Me.m_mdataGridView.Rows(0).Cells(0).Value = "Prey/Pred Rates"
                        Me.m_mdataGridView.Rows(1).Cells(0).Value = Me.m_data.ItemName(Me.m_iHighlightedNode)
                        Me.m_mdataGridView.Rows(1).Cells(0).Style.BackColor = Color.Gold

                        'Dim rowval As Integer = 0
                        Dim celval As Integer = 1
                        For j As Integer = 1 To Me.m_data.NumItems

                            If (Me.m_data.LinkValue(Me.m_iHighlightedNode, j) > 0) Then   'Pred:highlightNod Pray:j

                                Dim cons As Single = Me.Core.EcoPathGroupOutputs(j).Consumption(Me.m_iHighlightedNode)
                                Dim gpnm As String = Me.m_data.ItemName(j)

                                Me.m_mdataGridView.Columns.Add("column", "header")
                                Me.m_mdataGridView.Rows(0).Cells(celval).Value = gpnm
                                Me.m_mdataGridView.Rows(0).Cells(celval).Style.ForeColor = Color.Green
                                Me.m_mdataGridView.Rows(1).Cells(celval).Value = cons
                                Me.m_mdataGridView.Rows(1).Cells(celval).Style.ForeColor = Color.Green
                                celval += 1

                            ElseIf (Me.m_data.LinkValue(j, Me.m_iHighlightedNode) > 0) Then   'Pred:j Pray:highlightNod

                                Dim cons1 As Single = Me.Core.EcoPathGroupOutputs(Me.m_iHighlightedNode).Consumption(j)
                                Dim gpnm1 As String = Me.m_data.ItemName(j)

                                Me.m_mdataGridView.Columns.Add("column", "header")
                                Me.m_mdataGridView.Rows(0).Cells(celval).Value = gpnm1
                                Me.m_mdataGridView.Rows(0).Cells(celval).Style.ForeColor = Color.DarkRed
                                Me.m_mdataGridView.Rows(1).Cells(celval).Value = cons1
                                Me.m_mdataGridView.Rows(1).Cells(celval).Style.ForeColor = Color.DarkRed
                                celval += 1

                            End If

                        Next j
                        Me.m_iHighlightedNodePrev = Me.m_iHighlightedNode

                        'Displaying DataGridView on the form with properties set to it
                        Me.m_mdataGridView.Dock = DockStyle.Top
                        Me.m_mdataGridView.BackgroundColor = System.Drawing.SystemColors.Window
                        Me.m_mdataGridView.BorderStyle = BorderStyle.None
                        Me.m_mdataGridView.Size = New Size(720, 70)
                        Me.m_mdataGridView.ColumnHeadersVisible = False
                        Me.m_mdataGridView.RowHeadersVisible = False
                        Me.m_mdataGridView.AllowUserToAddRows = False
                        Me.m_mdataGridView.AllowUserToDeleteRows = False
                        Me.m_mdataGridView.AllowUserToOrderColumns = False
                        Me.m_mdataGridView.ReadOnly = True
                        Me.m_mdataGridView.MultiSelect = False
                        Me.m_mdataGridView.AllowUserToResizeColumns = False
                        Me.m_mdataGridView.AllowUserToResizeRows = False
                        'Adding DataGridView to the Flowdiagram control
                        Me.m_pbFlowDiagram.Controls.Add(Me.m_mdataGridView)
                    End If
                End If
            End If

            If Me.m_tree.ShowFlowRateLegend = False Then
                Try
                    Me.m_iHighlightedNodePrev = 0 'we can again select the same node
                    If (Me.m_mdataGridView IsNot Nothing) Then
                        Me.m_mdataGridView.Dispose()  'Dellocating all the resources to DataGridView
                        Me.m_mdataGridView.ClearSelection()
                        Me.m_pbFlowDiagram.Controls.Remove(Me.m_mdataGridView)
                    End If
                Catch
                    'nothing
                End Try
            End If
            Me.m_pbFlowDiagram.Invalidate()
        End Sub

#End Region ' Mouse Events

#Region " Tree events (wouldn't that be nice?)"

        Private Sub OnTreeChanged(sender As cTreeFlowDiagramRenderer)

            ' ToDo: globalize this method

            Dim strMessage As String = ""

            ' Enabling only one legend to display at a time
            If Me.m_tree.ShowLegend = TriState.True And Me.m_tree.ShowBiomassLegend = True And Me.m_tree.ShowFlowRateLegend = False Then
                Me.m_tree.ShowLegend = TriState.False
                strMessage = "Please select one legend at a time. Disabling ShowLegend"

            ElseIf Me.m_tree.ShowLegend = TriState.True And Me.m_tree.ShowBiomassLegend = False And Me.m_tree.ShowFlowRateLegend = True Then
                Me.m_tree.ShowLegend = TriState.False
                strMessage = "Please select one legend at a time. Disabling ShowLegend"

            ElseIf Me.m_tree.ShowLegend = TriState.False And Me.m_tree.ShowBiomassLegend = True And Me.m_tree.ShowFlowRateLegend = True Then
                Me.m_tree.ShowBiomassLegend = False
                strMessage = "Please select one legend at a time. Disabling ShowBiomassLegend"

            ElseIf Me.m_tree.ShowLegend = TriState.UseDefault And Me.m_tree.ShowBiomassLegend = True And Me.m_tree.ShowFlowRateLegend = True Then
                Me.m_tree.ShowBiomassLegend = False
                strMessage = "Please select one legend at a time. Disabling ShowBiomassLegend"
            End If

            If Not String.IsNullOrWhiteSpace(strMessage) Then
                Dim fmsg As New cFeedbackMessage(strMessage, eCoreComponentType.External, eMessageType.StateNotMet, eMessageImportance.Warning, eMessageReplyStyle.OK)
                Me.Core.Messages.SendMessage(fmsg)
                Me.m_pbFlowDiagram.Invalidate(True)
            End If

            Me.m_pbFlowDiagram.Invalidate()

            ' Preserve layout
            Dim ad As cAuxiliaryData = Me.Core.AuxillaryData(Me.DataName())
            Me.m_doodler.Save(ad.Settings, Me.m_pbFlowDiagram)
            Me.m_tree.Save(ad.Settings)

        End Sub

        Private Sub OnTreeBiomassLegendChanged(sender As cTreeFlowDiagramRenderer)

            If Me.m_tree.ShowBiomassLegend = True Then

                Me.m_dt = New DataTable
                For columnIndex As Integer = 1 To Me.m_noofColumns
                    Me.m_dt.Columns.Add()
                Next columnIndex

                'Adding elements to dynamically created rows
                'Looping through all the groups of model
                Dim groupIndex As Integer
                For groupIndex = 1 To Me.Core.nGroups
                    Me.m_dr0 = Me.m_dt.NewRow()  'Row with Biomass Names 
                    Me.m_dr1 = Me.m_dt.NewRow()  'Row with Biomass Values

                    For innerloop As Integer = 1 To Me.m_noofColumns
                        'Check to make sure to stay within nGroups
                        If groupIndex <= Me.Core.nGroups Then
                            Dim gpnm As String = Me.Core.EcoPathGroupInputs(groupIndex).Name
                            Dim bmss As Single = Me.Core.EcoSimGroupOutputs(groupIndex).Biomass(Me.CurrentTimestep)
                            Me.m_dr0(innerloop - 1) = gpnm
                            Me.m_dr1(innerloop - 1) = bmss
                            'incrementing the groupIndex outside forloop to keep track of next value
                            groupIndex += 1

                        End If
                    Next innerloop
                    'Adding the two newly created Rows to DataTable
                    Me.m_dt.Rows.Add(Me.m_dr0)
                    Me.m_dt.Rows.Add(Me.m_dr1)

                    groupIndex -= 1
                Next groupIndex

                'creating DataGridView 
                Me.m_dataGridView = New DataGridView
                Me.m_dataGridView.DataSource = Me.m_dt

                'DefaultcellStyles
                Me.m_dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
                Me.m_dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ScrollBar
                Me.m_dataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                Me.m_dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText
                Me.m_dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
                Me.m_dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
                Me.m_dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
                Me.m_dataGridView.DefaultCellStyle = Me.m_dataGridViewCellStyle1

                'AlternatingRowStyles
                Me.m_dataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                Me.m_dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
                Me.m_dataGridView.AlternatingRowsDefaultCellStyle = Me.m_dataGridViewCellStyle2

                'Displaying DataGridView on the form with properties set to it
                Me.m_dataGridView.Dock = DockStyle.Top
                Me.m_dataGridView.AutoResizeColumns()
                Me.m_dataGridView.Size = New Size(720, 137)
                Me.m_dataGridView.ColumnHeadersVisible = False
                Me.m_dataGridView.RowHeadersVisible = False
                Me.m_dataGridView.AllowUserToAddRows = False
                Me.m_dataGridView.AllowUserToDeleteRows = False
                Me.m_dataGridView.AllowUserToOrderColumns = False
                Me.m_dataGridView.ReadOnly = True
                Me.m_dataGridView.MultiSelect = False
                Me.m_dataGridView.AllowUserToResizeColumns = False
                Me.m_dataGridView.AllowUserToResizeRows = False
                Me.m_dataGridView.BackgroundColor = System.Drawing.SystemColors.Window
                Me.m_dataGridView.BorderStyle = BorderStyle.None

                'Adding DataGridView to the Flowdiagram control
                Me.m_pbFlowDiagram.Controls.Add(Me.m_dataGridView)

            End If

            If Me.m_tree.ShowBiomassLegend = False Then
                'checking if DataTable is already created when TriState is True.
                Try
                    If Me.m_dt.IsInitialized Then
                        If Me.m_dt.Rows.Count > 0 Then
                            Me.m_dataGridView.Dispose()  'Dellocating all the resources to DataGridView
                            Me.m_dataGridView.ClearSelection()
                            Me.m_pbFlowDiagram.Controls.Remove(Me.m_dataGridView) 'Removing from the control
                        End If
                    End If
                Catch e As Exception
                    'DataTable is not created. So nothing to Dellocate and remove
                End Try
            End If

            Me.m_pbFlowDiagram.Invalidate(True)

        End Sub


        Private Sub OnTreeFlowRateLegendChanged(sender As cTreeFlowDiagramRenderer)

            ' ToDo: globalize this

            If Me.m_tree.ShowFlowRateLegend = True Then
                Me.m_mdataGridView = New DataGridView
                Me.m_mdataGridView.Columns.Add("column", "header")
                Me.m_mdataGridView.Columns.Add("column", "header")
                Me.m_mdataGridView.Columns.Add("column", "header")
                Me.m_mdataGridView.Columns.Add("column", "header")
                Me.m_mdataGridView.Columns.Add("column", "header")
                Me.m_mdataGridView.Rows.Add()
                Me.m_mdataGridView.Rows.Add()
                'adding first column values
                Me.m_mdataGridView.Rows(0).Cells(0).Value = "Prey/Pred Rates"
                'm_mdataGridView.Rows(0).Cells(0).Style.BackColor = Color.White
                Me.m_mdataGridView.Rows(1).Cells(0).Value = "N/A: Hover over group"
                Me.m_mdataGridView.Rows(1).Cells(0).Style.BackColor = Color.Gold
                Me.m_mdataGridView.Dock = DockStyle.Top
                Me.m_mdataGridView.BackgroundColor = System.Drawing.SystemColors.Window
                Me.m_mdataGridView.BorderStyle = BorderStyle.None
                Me.m_mdataGridView.Size = New Size(720, 70)
                Me.m_mdataGridView.ColumnHeadersVisible = False
                Me.m_mdataGridView.RowHeadersVisible = False
                Me.m_mdataGridView.AllowUserToAddRows = False
                Me.m_mdataGridView.AllowUserToDeleteRows = False
                Me.m_mdataGridView.AllowUserToOrderColumns = False
                Me.m_mdataGridView.ReadOnly = True
                Me.m_mdataGridView.MultiSelect = False
                Me.m_mdataGridView.AllowUserToResizeColumns = False
                Me.m_mdataGridView.AllowUserToResizeRows = False
                'Adding DataGridView to the Flowdiagram control
                Me.m_pbFlowDiagram.Controls.Add(Me.m_mdataGridView)
            End If

            If Me.m_tree.ShowFlowRateLegend = False Then
                Try  'Delete previous DataTable if still exiting on the form
                    Me.m_mdataGridView.Dispose()  'Dellocating all the resources to DataGridView
                    Me.m_mdataGridView.ClearSelection()
                    Me.m_pbFlowDiagram.Controls.Remove(Me.m_mdataGridView)
                Catch
                    'Noting
                End Try
            End If
        End Sub


#End Region ' Tree events

#Region " Commands "

        Private Sub OnResetLayout(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiResetLayout.Click

            Me.m_tree.ResetLayout()
            Me.m_pbFlowDiagram.Invalidate()

        End Sub

        Private Sub OnLoadLayout(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnImport.Click

            Dim ifData As cXMLSettings = Nothing
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            ' ToDo: Globalize this
            cmdFO.Invoke(Me.FileName, SharedResources.FILEFILTER_FLOWDIAGRAM, 1, "Select flow diagram layout to load")

            If (cmdFO.Result = DialogResult.OK) Then
                Try
                    ifData = New cXMLSettings()
                    ifData.LoadFromFile(cmdFO.FileName)
                    Me.m_doodler.Load(ifData, Me.m_pbFlowDiagram)
                Catch ex As Exception
                    Dim msg As New cMessage(String.Format(SharedResources.FILE_LOAD_ERROR_DETAIL, cmdFO.FileName, ex.Message),
                                            eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
                    Me.Core.Messages.SendMessage(msg)
                End Try
            End If

        End Sub

        Private Sub OnSaveLayout(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnExport.Click

            Dim ifData As cXMLSettings = Nothing
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(Me.FileName, SharedResources.FILEFILTER_FLOWDIAGRAM, 1)

            If cmdFS.Result = System.Windows.Forms.DialogResult.OK Then
                Try
                    ifData = New cXMLSettings()
                    ifData.LoadFromFile(cmdFS.FileName)
                    Me.m_doodler.Save(ifData, Me.m_pbFlowDiagram)
                Catch ex As Exception
                    Dim msg As New cMessage(String.Format(SharedResources.FILE_SAVE_ERROR_DETAIL, cmdFS.FileName, ex.Message),
                                            eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
                    Me.Core.Messages.SendMessage(msg)
                End Try
            End If
        End Sub

        Private Sub OnSettings(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiSettings.Click

            Me.UpdateControls()

        End Sub

        Private Sub OnSaveToImage(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiSaveToImage.Click

            ' ToDo: globalize this

            Dim dpi As Integer = Me.StyleGuide.PreferredDPI
            Dim fmt As Imaging.ImageFormat = Imaging.ImageFormat.Bmp
            Dim fs As FileStream = Nothing
            Dim hdc As IntPtr = Nothing ' :)
            Dim mf As Metafile = Nothing
            Dim bmp As Bitmap = Nothing
            Dim s As Size = Me.Size

            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmdfs As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdfs.Invoke(Me.FileName, SharedResources.FILEFILTER_IMAGE & "|" & SharedResources.FILEFILTER_IMAGE_EMF, 6)

            If cmdfs.Result = DialogResult.OK Then
                Select Case cmdfs.FilterIndex
                    Case 2
                        fmt = Imaging.ImageFormat.Jpeg
                    Case 3
                        fmt = Imaging.ImageFormat.Gif
                    Case 4
                        fmt = Imaging.ImageFormat.Png
                    Case 5
                        fmt = Imaging.ImageFormat.Tiff
                    Case 6
                        bmp = New Bitmap(Me.m_pbFlowDiagram.Width, Me.m_pbFlowDiagram.Height, PixelFormat.Format32bppArgb)
                        fs = New FileStream(cmdfs.FileName, FileMode.Create)
                        Using g As Graphics = Graphics.FromImage(bmp)
                            hdc = g.GetHdc()
                            mf = New Metafile(fs, hdc, EmfType.EmfOnly)
                            g.ReleaseHdc(hdc)
                        End Using
                        Using g As Graphics = Graphics.FromImage(mf)
                            Threading.Thread.Sleep(500)
                            Dim xx As Integer = Me.m_pbFlowDiagram.ClientRectangle.X
                            Dim yy As Integer = Me.m_pbFlowDiagram.ClientRectangle.Y
                            Dim point1 As New Point(xx, yy + 25)
                            g.CopyFromScreen(Me.PointToScreen(point1), Point.Empty, s)
                        End Using
                        fs.Close()
                        mf.Dispose()
                        bmp.Dispose()
                        Return
                    Case Else
                        fmt = Imaging.ImageFormat.Bmp
                End Select

                bmp = Me.StyleGuide.GetImage(Me.m_pbFlowDiagram.Width, Me.m_pbFlowDiagram.Height, fmt, cmdfs.FileName)
                Using g As Graphics = Graphics.FromImage(bmp)
                    Threading.Thread.Sleep(500)
                    Dim xx As Integer = Me.m_pbFlowDiagram.ClientRectangle.X
                    Dim yy As Integer = Me.m_pbFlowDiagram.ClientRectangle.Y
                    Dim point1 As New Point(xx, yy + 25)
                    g.CopyFromScreen(Me.PointToScreen(point1), Point.Empty, s)

                End Using

                Try
                    bmp.Save(cmdfs.FileName, fmt)

                    Dim msg As New cMessage(String.Format(SharedResources.GENERIC_FILESAVE_SUCCES, "flow diagram image", cmdfs.FileName),
                                            eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Information)
                    msg.Hyperlink = Path.GetDirectoryName(cmdfs.FileName)
                    Me.Core.Messages.SendMessage(msg)

                Catch ex As Exception
                    cLog.Write(ex, "frmEcosimFD::saveimage(" & cmdfs.FileName & ")")
                    Dim msg As New cMessage(String.Format(SharedResources.FILE_SAVE_ERROR_DETAIL, cmdfs.FileName, ex.Message),
                                eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
                    Me.Core.Messages.SendMessage(msg)
                End Try
                bmp.Dispose()

            End If

        End Sub


        Private Sub OnSaveToBatchImage(sender As System.Object, e As System.EventArgs) _
          Handles m_tsmiSaveToBatchImage.Click

            Dim fmt As Imaging.ImageFormat = Imaging.ImageFormat.Bmp
            Dim cmdh As cCommandHandler = Me.CommandHandler
            Dim cmdfs As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim fs As FileStream = Nothing
            Dim hdc As IntPtr = Nothing ' :)
            Dim mf As Metafile = Nothing
            Dim bmp As Bitmap = New Bitmap(Me.m_pbFlowDiagram.Width * 2, Me.m_pbFlowDiagram.Height * 2, PixelFormat.Format32bppArgb)
            Dim rc As New Rectangle(0, 0, Me.m_pbFlowDiagram.ClientRectangle.Width * 2, Me.m_pbFlowDiagram.ClientRectangle.Height * 2)
            Dim bSuccess As Boolean = True

            ' JS 03Mar15: Changed to a single message with variable statuses
            Dim msgResult As New cMessage("", eMessageType.DataExport, eCoreComponentType.EcoSim, eMessageImportance.Information)

            cmdfs.Invoke(Me.FileName, SharedResources.FILEFILTER_IMAGE & "|" & SharedResources.FILEFILTER_IMAGE_EMF, 6)
            If cmdfs.Result = DialogResult.OK Then
                Select Case cmdfs.FilterIndex
                    Case 2
                        fmt = Imaging.ImageFormat.Jpeg
                    Case 3
                        fmt = Imaging.ImageFormat.Gif
                    Case 4
                        fmt = Imaging.ImageFormat.Png
                    Case 5
                        fmt = Imaging.ImageFormat.Tiff
                    Case Else
                        fmt = Imaging.ImageFormat.Bmp
                End Select

                bmp.SetResolution(Me.StyleGuide.PreferredDPI, Me.StyleGuide.PreferredDPI)

                Dim totTimeSteps As Integer = Me.Core.nEcosimTimeSteps
                Dim strPath As String = Path.GetDirectoryName(cmdfs.FileName)
                Dim strFile As String = Path.GetFileNameWithoutExtension(cmdfs.FileName)
                Dim strExt As String = fmt.ToString()

                cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_ECOSIMFD_SAVING, 0)

                'Iterating through all the timesteps and saving FlowDiagram for each TimeStep
                For currTimeStep As Integer = 1 To totTimeSteps

                    Me.CurrentTimestep = currTimeStep

                    'Calculating Year and Month for timestep
                    Dim currentYear As Integer = Me.m_EcosimFirstYear + CInt(Math.Truncate(Me.CurrentTimestep / Me.m_noofTimeSlicesPerYear))
                    Dim month As Integer = (Me.CurrentTimestep Mod Me.m_noofTimeSlicesPerYear) + 1
                    'Redraw the flow diagram with updated biomass values. 
                    Me.m_data.Refresh()
                    Me.m_pbFlowDiagram.Invalidate()
                    Using g As Graphics = Graphics.FromImage(bmp)
                        Me.m_doodler.DrawFlowDiagram(g, rc)
                    End Using

                    cApplicationStatusNotifier.UpdateProgress(Me.Core, My.Resources.STATUS_ECOSIMFD_SAVING, CSng(currTimeStep / totTimeSteps))

                    'Saving each timestep with a Regular file name pattern
                    Dim strDestFileName As String = cFileUtils.ToValidFileName(strFile & "_" & currentYear.ToString("D4") & "-" & month.ToString("D2") & "-" & currTimeStep.ToString("D4"), False)

                    Try

                        bmp.Save(Path.Combine(strPath, Path.ChangeExtension(strDestFileName, strExt)), fmt)

                        Dim strSuccess As String = String.Format(My.Resources.ECOSIM_FD_SAVE_SUCCESS_DETAIL, currTimeStep)
                        Dim vs As New cVariableStatus(eStatusFlags.OK, strSuccess, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.EcoSim, 0)
                        msgResult.AddVariable(vs)

                    Catch ex As Exception

                        cLog.Write(ex, "frmEcosimFD::SaveImage(" & Path.Combine(strPath, strDestFileName) & ")")
                        bSuccess = False

                        Dim strError As String = String.Format(My.Resources.ECOSIM_FD_SAVE_FAILURE_DETAIL, currTimeStep, ex.Message)
                        Dim vs As New cVariableStatus(eStatusFlags.ErrorEncountered, strError, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.EcoSim, 0)
                        msgResult.AddVariable(vs)

                    End Try

                Next currTimeStep
                bmp.Dispose()
                GC.Collect()

                If bSuccess Then
                    msgResult.Message = String.Format(My.Resources.ECOSIM_FD_SAVE_SUCCESS, strPath)
                    msgResult.Hyperlink = strPath
                Else
                    msgResult.Message = String.Format(My.Resources.ECOSIM_FD_SAVE_FAILURE, strPath)
                End If
                Me.Core.Messages.SendMessage(msgResult)

                cApplicationStatusNotifier.EndProgress(Me.Core)

            End If

        End Sub

#End Region ' Commands

#Region " Slider "

        Private Sub OnSliderValueChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_slider.ValueChanged

            If (Me.m_noofTimeSlicesPerYear = 0) Then Return

            Me.m_tbxTimeStep.Text = Me.m_slider.Value.ToString
            Me.CurrentTimestep = Me.m_slider.Value
            Dim currentYear As Integer = Me.m_EcosimFirstYear + CInt(Math.Truncate(Me.CurrentTimestep / Me.m_noofTimeSlicesPerYear))
            Dim month As Integer = (Me.CurrentTimestep Mod Me.m_noofTimeSlicesPerYear) + 1
            Me.m_tbxYear.Text = currentYear.ToString
            Me.m_tbxMonth.Text = cDateUtils.GetMonthName(month, False)

            'Updating the values of DataGridView as the slider is moved
            If Me.m_tree.ShowBiomassLegend = True Then
                Dim groupIndex As Integer = 1
                'Only selecting Rows with Biomass Values to speedup slider functionlity
                For rowCnt As Integer = 1 To Me.m_dt.Rows.Count
                    Dim editDataRow As DataRow = Me.m_dt.Rows(rowCnt)
                    editDataRow.BeginEdit()
                    For innerloop As Integer = 1 To Me.m_noofColumns
                        'Check to see if the GroupIndex is within nGroups count
                        If groupIndex <= Me.Core.nGroups Then
                            Dim biomss As Single = Me.Core.EcoSimGroupOutputs(groupIndex).Biomass(Me.CurrentTimestep)
                            editDataRow(innerloop - 1) = biomss
                            'Moving onto the next group to get name and Biomass value
                            groupIndex += 1
                        End If
                    Next innerloop

                    editDataRow.EndEdit()
                    rowCnt += 1
                Next rowCnt
            End If

            'Redraw the flow diagram with updated biomass values. 
            Me.m_data.Refresh()
            Me.m_pbFlowDiagram.Invalidate()

        End Sub

#End Region ' Slider

#Region "Slider Animation"

        Private Sub PlayBtn_click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnPlay.Click

            'Play button functionality
            Select Case Me.m_animationstate

                Case eAnimationState.Idle
                    Me.m_animationstate = eAnimationState.Playing
                    Dim thread As New System.Threading.Thread(AddressOf Me.ShowAnimationThread)
                    thread.IsBackground = True
                    thread.Start()

                Case eAnimationState.Paused
                    Me.m_animationstate = eAnimationState.Playing

                Case eAnimationState.Playing
                    Me.m_animationstate = eAnimationState.Paused

                Case eAnimationState.Stopping
                    ' Should not be here!
            End Select

            Me.UpdateControls()

        End Sub

        Private Sub StopBtn_click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnStop.Click

            Me.m_animationstate = eAnimationState.Stopping

        End Sub

        Private Sub ShowAnimationThread()

            Dim totTimeSteps As Integer = Me.Core.nEcosimTimeSteps
            Dim currTimeStep As Integer = 1

            ' JS 03Mar15: Break on a stop flag rather than shooting the thread
            While (Me.m_animationstate = eAnimationState.Playing)

                Me.AppendTextBox(Me.m_tbxTimeStep, currTimeStep.ToString)  'Cross Threads operating on Textbox
                Me.CurrentTimestep = currTimeStep
                Dim currentYear As Integer = Me.m_EcosimFirstYear + CInt(Math.Truncate(Me.CurrentTimestep / Me.m_noofTimeSlicesPerYear))
                Dim month As Integer = (Me.CurrentTimestep Mod Me.m_noofTimeSlicesPerYear) + 1
                Me.AppendTextBox(Me.m_tbxYear, currentYear.ToString)
                Me.AppendTextBox(Me.m_tbxMonth, cDateUtils.GetMonthName(month, False))
                'Redraw the flow diagram with updated biomass values. 
                Me.m_data.Refresh()
                Me.m_pbFlowDiagram.Invalidate()

                ' JS 03Mar15: This expensive call is not needed when running in a separate thread from the main UI
                ' Application.DoEvents()

                ' JS 03Mar15: Evaluate every time, is neat ;)
                Threading.Thread.Sleep(CInt(Me.m_fpDelay.Value))

                ' JS 03Mar15: Pause on state flag rather than pausing the actual thread
                While (Me.m_animationstate = eAnimationState.Paused)
                    ' Spin wheels
                End While

                currTimeStep = (currTimeStep Mod totTimeSteps) + 1

            End While

            Me.m_animationstate = eAnimationState.Idle

            If (Not Me.IsDisposed()) Then
                Me.BeginInvoke(New RunCompletedDelegate(AddressOf Me.UpdateControls))
            End If

        End Sub

        ' These delegates enables asynchronous UI element updates -----

        Private Delegate Sub AppendTextBoxDelegate(TB As TextBox, txt As String)
        Private Delegate Sub AppendButtonDelegate(Btn As Button, txt As String)
        Private Delegate Sub AppendSliderDelegate(sl As ucSlider, val As Integer)
        Private Delegate Sub RunCompletedDelegate()


        Private Sub AppendTextBox(TB As TextBox, txt As String)

            If Me.IsDisposed Then Return
            Try
                If TB.InvokeRequired Then
                    TB.Invoke(New AppendTextBoxDelegate(AddressOf Me.AppendTextBox), New Object() {TB, txt})
                Else
                    TB.Text = txt
                End If
            Catch exDisp As ObjectDisposedException
                ' Swallow this, just bad luck
            Catch ex As Exception
                cLog.Write(ex, "frmEcosimFD.AppendTextBox")
            End Try

        End Sub

        Private Sub AppendButton(Btn As Button, txt As String)

            If Me.IsDisposed Then Return
            Try
                If Btn.InvokeRequired Then
                    Btn.Invoke(New AppendButtonDelegate(AddressOf Me.AppendButton), New Object() {Btn, txt})
                Else
                    Btn.Text = txt
                End If
            Catch exDisp As ObjectDisposedException
                ' Swallow this, just bad luck
            Catch ex As Exception
                cLog.Write(ex, "frmEcosimFD.AppendButton")
            End Try
        End Sub

        Private Sub AppendSlider(sl As ucSlider, val As Integer)

            If Me.IsDisposed Then Return

            Try
                If sl.InvokeRequired Then
                    sl.Invoke(New AppendSliderDelegate(AddressOf Me.AppendSlider), New Object() {sl, val})
                Else
                    sl.Value = val
                End If
            Catch exDisp As ObjectDisposedException
                ' Swallow this, just bad luck
            Catch ex As Exception
                cLog.Write(ex, "frmEcosimFD.AppendSlider")
            End Try

        End Sub

#End Region

#End Region ' Events

#Region " Internals "

        Private Function DataName() As String
            Return "FD_SIM" & Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex).DBID
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Default file name placed at Core output location for Ecosim
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function FileName() As String
            Return Path.Combine(Me.Core.DefaultOutputPath(eAutosaveTypes.Ecosim), "Ecosim flow diagram")
        End Function

        Private Property CurrentTimestep As Integer
            Get
                Return Me.m_slider.Value
            End Get
            Set(value As Integer)
                Me.AppendSlider(Me.m_slider, value)
                If (Me.m_data IsNot Nothing) Then
                    Me.m_data.TimeStep = value
                End If
            End Set
        End Property

        Protected Overrides Sub UpdateControls()

            Me.m_scContent.Panel2Collapsed = Not Me.m_tsmiSettings.Checked
            Me.m_pgFlowDiagram.Invalidate()

            Select Case Me.m_animationstate
                Case eAnimationState.Idle
                    Me.m_btnPlay.Text = SharedResources.LABEL_PLAY
                    Me.m_btnStop.Enabled = False
                Case eAnimationState.Playing
                    Me.m_btnPlay.Text = SharedResources.LABEL_PAUSE
                    Me.m_btnStop.Enabled = True
                Case eAnimationState.Paused
                    Me.m_btnPlay.Text = SharedResources.LABEL_RESUME
                    Me.m_btnStop.Enabled = True
                Case eAnimationState.Stopping
                    Me.m_btnStop.Enabled = False
                    Me.m_btnPlay.Enabled = False
            End Select

        End Sub

        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcosimFlowDiagram))
            Me.m_pbFlowDiagram = New System.Windows.Forms.PictureBox()
            Me.m_scContent = New System.Windows.Forms.SplitContainer()
            Me.m_btnStop = New System.Windows.Forms.Button()
            Me.m_tbxDelay = New System.Windows.Forms.TextBox()
            Me.m_lblDelay = New System.Windows.Forms.Label()
            Me.m_btnPlay = New System.Windows.Forms.Button()
            Me.m_lblTimeStep = New System.Windows.Forms.Label()
            Me.m_lblMonth = New System.Windows.Forms.Label()
            Me.m_lblYear = New System.Windows.Forms.Label()
            Me.m_tbxMonth = New System.Windows.Forms.TextBox()
            Me.m_tbxYear = New System.Windows.Forms.TextBox()
            Me.m_tbxTimeStep = New System.Windows.Forms.TextBox()
            Me.m_slider = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_pgFlowDiagram = New System.Windows.Forms.PropertyGrid()
            Me.m_tsFlowDiagram = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsmiSettings = New System.Windows.Forms.ToolStripButton()
            Me.m_tss2 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsmiSaveToImage = New System.Windows.Forms.ToolStripButton()
            Me.m_tsmiSaveToBatchImage = New System.Windows.Forms.ToolStripButton()
            Me.m_tss3 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnImport = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnExport = New System.Windows.Forms.ToolStripButton()
            Me.m_tsmiResetLayout = New System.Windows.Forms.ToolStripButton()
            CType(Me.m_pbFlowDiagram, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_scContent, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scContent.Panel1.SuspendLayout()
            Me.m_scContent.Panel2.SuspendLayout()
            Me.m_scContent.SuspendLayout()
            Me.m_tsFlowDiagram.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_pbFlowDiagram
            '
            Me.m_pbFlowDiagram.BackColor = System.Drawing.SystemColors.Window
            Me.m_pbFlowDiagram.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_pbFlowDiagram, "m_pbFlowDiagram")
            Me.m_pbFlowDiagram.Name = "m_pbFlowDiagram"
            Me.m_pbFlowDiagram.TabStop = False
            '
            'm_scContent
            '
            resources.ApplyResources(Me.m_scContent, "m_scContent")
            Me.m_scContent.Name = "m_scContent"
            '
            'm_scContent.Panel1
            '
            Me.m_scContent.Panel1.Controls.Add(Me.m_btnStop)
            Me.m_scContent.Panel1.Controls.Add(Me.m_tbxDelay)
            Me.m_scContent.Panel1.Controls.Add(Me.m_lblDelay)
            Me.m_scContent.Panel1.Controls.Add(Me.m_btnPlay)
            Me.m_scContent.Panel1.Controls.Add(Me.m_lblTimeStep)
            Me.m_scContent.Panel1.Controls.Add(Me.m_lblMonth)
            Me.m_scContent.Panel1.Controls.Add(Me.m_lblYear)
            Me.m_scContent.Panel1.Controls.Add(Me.m_tbxMonth)
            Me.m_scContent.Panel1.Controls.Add(Me.m_tbxYear)
            Me.m_scContent.Panel1.Controls.Add(Me.m_tbxTimeStep)
            Me.m_scContent.Panel1.Controls.Add(Me.m_slider)
            Me.m_scContent.Panel1.Controls.Add(Me.m_pbFlowDiagram)
            '
            'm_scContent.Panel2
            '
            Me.m_scContent.Panel2.Controls.Add(Me.m_pgFlowDiagram)
            '
            'm_btnStop
            '
            resources.ApplyResources(Me.m_btnStop, "m_btnStop")
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_tbxDelay
            '
            resources.ApplyResources(Me.m_tbxDelay, "m_tbxDelay")
            Me.m_tbxDelay.Name = "m_tbxDelay"
            '
            'm_lblDelay
            '
            resources.ApplyResources(Me.m_lblDelay, "m_lblDelay")
            Me.m_lblDelay.BackColor = System.Drawing.Color.White
            Me.m_lblDelay.ForeColor = System.Drawing.SystemColors.GrayText
            Me.m_lblDelay.Name = "m_lblDelay"
            '
            'm_btnPlay
            '
            resources.ApplyResources(Me.m_btnPlay, "m_btnPlay")
            Me.m_btnPlay.Name = "m_btnPlay"
            Me.m_btnPlay.UseVisualStyleBackColor = True
            '
            'm_lblTimeStep
            '
            resources.ApplyResources(Me.m_lblTimeStep, "m_lblTimeStep")
            Me.m_lblTimeStep.BackColor = System.Drawing.Color.White
            Me.m_lblTimeStep.ForeColor = System.Drawing.SystemColors.GrayText
            Me.m_lblTimeStep.Name = "m_lblTimeStep"
            '
            'm_lblMonth
            '
            resources.ApplyResources(Me.m_lblMonth, "m_lblMonth")
            Me.m_lblMonth.BackColor = System.Drawing.Color.White
            Me.m_lblMonth.ForeColor = System.Drawing.SystemColors.GrayText
            Me.m_lblMonth.Name = "m_lblMonth"
            '
            'm_lblYear
            '
            resources.ApplyResources(Me.m_lblYear, "m_lblYear")
            Me.m_lblYear.BackColor = System.Drawing.Color.White
            Me.m_lblYear.ForeColor = System.Drawing.SystemColors.GrayText
            Me.m_lblYear.Name = "m_lblYear"
            '
            'm_tbxMonth
            '
            resources.ApplyResources(Me.m_tbxMonth, "m_tbxMonth")
            Me.m_tbxMonth.Name = "m_tbxMonth"
            Me.m_tbxMonth.ReadOnly = True
            Me.m_tbxMonth.ShortcutsEnabled = False
            '
            'm_tbxYear
            '
            resources.ApplyResources(Me.m_tbxYear, "m_tbxYear")
            Me.m_tbxYear.Name = "m_tbxYear"
            Me.m_tbxYear.ReadOnly = True
            Me.m_tbxYear.ShortcutsEnabled = False
            '
            'm_tbxTimeStep
            '
            resources.ApplyResources(Me.m_tbxTimeStep, "m_tbxTimeStep")
            Me.m_tbxTimeStep.Name = "m_tbxTimeStep"
            Me.m_tbxTimeStep.ReadOnly = True
            Me.m_tbxTimeStep.ShortcutsEnabled = False
            '
            'm_slider
            '
            resources.ApplyResources(Me.m_slider, "m_slider")
            Me.m_slider.BackColor = System.Drawing.SystemColors.Window
            Me.m_slider.CurrentKnob = 0
            Me.m_slider.Maximum = 100
            Me.m_slider.Minimum = 0
            Me.m_slider.Name = "m_slider"
            Me.m_slider.NumKnobs = 1
            '
            'm_pgFlowDiagram
            '
            Me.m_pgFlowDiagram.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText
            resources.ApplyResources(Me.m_pgFlowDiagram, "m_pgFlowDiagram")
            Me.m_pgFlowDiagram.Name = "m_pgFlowDiagram"
            '
            'm_tsFlowDiagram
            '
            Me.m_tsFlowDiagram.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsFlowDiagram.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiSettings, Me.m_tss2, Me.m_tsmiSaveToImage, Me.m_tsmiSaveToBatchImage, Me.m_tss3, Me.m_tsbnImport, Me.m_tsbnExport, Me.m_tsmiResetLayout})
            resources.ApplyResources(Me.m_tsFlowDiagram, "m_tsFlowDiagram")
            Me.m_tsFlowDiagram.Name = "m_tsFlowDiagram"
            Me.m_tsFlowDiagram.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsmiSettings
            '
            Me.m_tsmiSettings.CheckOnClick = True
            Me.m_tsmiSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsmiSettings, "m_tsmiSettings")
            Me.m_tsmiSettings.Name = "m_tsmiSettings"
            '
            'm_tss2
            '
            Me.m_tss2.Name = "m_tss2"
            resources.ApplyResources(Me.m_tss2, "m_tss2")
            '
            'm_tsmiSaveToImage
            '
            Me.m_tsmiSaveToImage.AutoToolTip = False
            resources.ApplyResources(Me.m_tsmiSaveToImage, "m_tsmiSaveToImage")
            Me.m_tsmiSaveToImage.Name = "m_tsmiSaveToImage"
            '
            'm_tsmiSaveToBatchImage
            '
            Me.m_tsmiSaveToBatchImage.AutoToolTip = False
            resources.ApplyResources(Me.m_tsmiSaveToBatchImage, "m_tsmiSaveToBatchImage")
            Me.m_tsmiSaveToBatchImage.Name = "m_tsmiSaveToBatchImage"
            '
            'm_tss3
            '
            Me.m_tss3.Name = "m_tss3"
            resources.ApplyResources(Me.m_tss3, "m_tss3")
            '
            'm_tsbnImport
            '
            Me.m_tsbnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnImport, "m_tsbnImport")
            Me.m_tsbnImport.Name = "m_tsbnImport"
            '
            'm_tsbnExport
            '
            Me.m_tsbnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnExport, "m_tsbnExport")
            Me.m_tsbnExport.Name = "m_tsbnExport"
            '
            'm_tsmiResetLayout
            '
            Me.m_tsmiResetLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsmiResetLayout, "m_tsmiResetLayout")
            Me.m_tsmiResetLayout.Name = "m_tsmiResetLayout"
            '
            'frmEcosimFlowDiagram
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_tsFlowDiagram)
            Me.Controls.Add(Me.m_scContent)
            Me.Name = "frmEcosimFlowDiagram"
            Me.TabText = ""
            CType(Me.m_pbFlowDiagram, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scContent.Panel1.ResumeLayout(False)
            Me.m_scContent.Panel1.PerformLayout()
            Me.m_scContent.Panel2.ResumeLayout(False)
            CType(Me.m_scContent, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scContent.ResumeLayout(False)
            Me.m_tsFlowDiagram.ResumeLayout(False)
            Me.m_tsFlowDiagram.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region ' Internals

    End Class

End Namespace