'==============================================================================
'
' $Log: ucBiomassPlot.vb,v $
' Revision 1.1  2008/09/26 07:31:49  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.27  2008/08/02 03:04:21  jeroens
' Renamed resources
'
' Revision 1.26  2008/07/18 17:51:08  jeroens
' Removed progress indicator
' Updated to new ZedGraphHelper interface
'
' Revision 1.25  2008/07/01 19:13:12  sherman
' Merged branch - Fix_Ecopat_EcosimUpdateBug
'
' Revision 1.24  2008/07/01 14:15:48  jeroens
' Fixed issue 375
'
' Revision 1.23  2008/06/02 00:01:47  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.22  2008/05/30 23:49:54  jeroens
' Fixed autoscale crash
'
' Revision 1.21  2008/05/14 18:45:54  jeroens
' Fixed autoscale issue
'
' Revision 1.20  2008/05/07 01:39:03  jeroens
' Fixed bugs 281, 378, 470
'
' Revision 1.19  2008/05/05 22:21:26  jeroens
' Shared progress bar
'
' Revision 1.18  2008/02/13 18:03:19  jeroens
' Fixed bug 417
'
' Revision 1.17  2008/02/13 16:52:22  jeroens
' Former HideGroups dialog invoked via central command
'
' Revision 1.16  2007/12/17 16:27:48  jeroens
' * Added metadata for custom graph axis
'
' Revision 1.15  2007/12/17 16:15:03  sherman
' Bug fix to ensure Y axis is not set <=0
'
' Revision 1.14  2007/12/15 02:40:32  jeroens
' * Fixed autoscale vs user def. scale
'
' Revision 1.13  2007/12/14 20:08:36  jeroens
' * sigh * User Y axis value does not work yet
'
' Revision 1.12  2007/12/14 18:50:06  jeroens
' no message
'
' Revision 1.11  2007/12/14 17:44:52  jeroens
' * Fixed graph options startup states
'
' Revision 1.10  2007/12/14 15:47:56  jeroens
' * Fixed hokey layout, uses toolbars instead
'
' Revision 1.9  2007/12/10 00:34:48  jeroens
' * Fixed bizarre load order bug
'
' Revision 1.8  2007/11/29 02:16:07  jeroens
' + Organized
' + Added format provider for text box
'
'==============================================================================

#Region "Imports Directive"

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands

#End Region

Namespace Ecosim

    ''' <summary>
    ''' Ecosim output Biomass plot
    ''' </summary>
    Public Class ucBiomassPlot

        ''' <summary>
        ''' State change flags
        ''' </summary>
        Private Enum eChangedStateTypes
            Scale
            ScaleValue
            Overlay
            Batch
        End Enum

#Region " Private vars "

        Private m_Graph As New EcosimOutputLineGraph
        Private m_GroupDisplayFlags() As Boolean
        Private WithEvents m_Applauncher As AppLauncher = AppLauncher.GetInstance()
        Private m_core As cCore = Nothing
        Private m_bClbToggle As Boolean = False
        Private m_bBatch As Boolean = False
        Private m_fpYAxisValue As cEwEFormatProvider = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

        End Sub

#End Region ' Constructor

#Region " Public properties "

        Public WriteOnly Property SSValue() As Single
            Set(ByVal value As Single)
                Me.tslblSSValue.Text = StyleGuide.GetInstance().FormatNumber(value)
            End Set
        End Property

        Public ReadOnly Property Plot() As EcosimOutputLineGraph
            Get
                Return m_Graph
            End Get
        End Property

        Public Sub AddValues(ByRef v(,) As Single)
            m_Graph.AddValues(v, m_Graph.IsOverlay)
            AddGroups()
            UpdateLayers()
        End Sub

        Public Property BatchMode() As Boolean
            Get
                Return m_bBatch
            End Get
            Set(ByVal value As Boolean)
                m_bBatch = value
                Me.UpdateFixedGraphYScale()
            End Set
        End Property

#End Region ' Public properties

#Region " Public interfaces "

        Public Sub EnableControls(ByVal bEnable As Boolean)
            Me.tcOutput.Enabled = bEnable
            Me.OverlayToolStripMenuItem.Enabled = bEnable
            Me.AutoscaleToolstripButton.Enabled = bEnable
            Me.AnnualOutputToolStripMenuItem.Enabled = bEnable
            Me.m_fpYAxisValue.Enabled = bEnable
        End Sub

        Public Sub DrawSummaryLines(ByVal StartYear As Single, ByVal EndYear As Single)
            Try
                m_Graph.DrawSummaryLines(StartYear, EndYear)
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Sub

#End Region ' Public interfaces

#Region " Events "

        Private Sub ucBiomassPlot_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = Nothing


            Me.m_core = cCore.GetInstance()
            Me.Dock = DockStyle.Fill

            Me.m_fpYAxisValue = New cEwEFormatProvider(Me.tstbxYAxisValue.Control, GetType(Single), _
                New cVariableMetaData(0.0!, Single.MaxValue, _
                    cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan)))
            Me.m_fpYAxisValue.Value = Me.m_Graph.FixedYAxisScaleMax

            cmd = cmdh.GetCommand("DisplayGroups")
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.AddControl(Me.tsbtnShowHideGroups)
            End If

            Me.plBiomassPlot.Controls.Add(m_Graph)

            Me.UpdateControls()

        End Sub

        Private Sub ucBiomassPlot_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("DisplayGroups")
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.RemoveControl(Me.tsbtnShowHideGroups)
            End If

            Me.m_fpYAxisValue = Nothing
            Me.m_Applauncher = Nothing
        End Sub

        Private Sub lbGroups_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbGroups.SelectedIndexChanged
            UpdateGraph()
        End Sub

        Private Sub tcOutput_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tcOutput.SelectedIndexChanged

            If tcOutput.SelectedIndex = 1 Then 'Return to layer

                For i As Integer = 0 To m_Graph.Layers.Count - 1
                    For j As Integer = 0 To m_Graph.Layers(i).Lines.Count - 1
                        If m_Graph.Layers(i).Lines(j).IsGrayOut Then
                            m_Graph.Layers(i).Lines(j).IsGrayOut = False
                        End If
                    Next
                Next
                m_Graph.IsGrayOut = False
                UpdateLayers()
                m_Graph.GenerateOutputImage()
            ElseIf tcOutput.SelectedIndex = 0 Then 'Return to group

                If lbGroups.SelectedIndex = -1 And lbGroups.Items.Count > 0 Then
                    lbGroups.SelectedIndex = 0
                End If

                If lbGroups.SelectedIndex > 0 Then
                    UpdateGraph()
                End If
            End If

        End Sub

        Private Sub clbLayers_ItemCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles clbLayers.ItemCheck
            If m_bClbToggle Then
                If e.NewValue = CheckState.Checked Then
                    m_Graph.Layers(e.Index).IsShown = True
                Else
                    m_Graph.Layers(e.Index).IsShown = False
                End If
                m_Graph.GenerateOutputImage()
                m_bClbToggle = False
            End If
        End Sub

        Private Sub clbLayers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles clbLayers.Click
            m_bClbToggle = True
        End Sub

        Private Sub OnToggleOverlay(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OverlayToolStripMenuItem.Click
            Me.OverlayToolStripMenuItem.Checked = Not Me.OverlayToolStripMenuItem.Checked
            Me.m_Graph.IsOverlay = Me.OverlayToolStripMenuItem.Checked
            Me.UpdateFixedGraphYScale()
        End Sub

        Private Sub OnToggleAnnualOutput(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnnualOutputToolStripMenuItem.Click
            Me.AnnualOutputToolStripMenuItem.Checked = Not Me.AnnualOutputToolStripMenuItem.Checked
            Me.m_Graph.IsShowAnnualOutput = Me.AnnualOutputToolStripMenuItem.Checked
            Me.UpdateFixedGraphYScale()
        End Sub

        Private Sub OnToggleAutoScale(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoscaleToolstripButton.Click
            If Me.AutoscaleToolstripButton.CheckState = CheckState.Checked Then
                Me.AutoscaleToolstripButton.CheckState = CheckState.Unchecked
            Else
                Me.AutoscaleToolstripButton.CheckState = CheckState.Checked
            End If
            Me.UpdateFixedGraphYScale()
        End Sub

        Private Sub OnDislayGroupsChanged() Handles m_Applauncher.DisplayGroupsChanged

            m_GroupDisplayFlags = AppLauncher.GetInstance.GroupDisplayFlags

            For i As Integer = 0 To m_Graph.Layers.Count - 1
                For j As Integer = 0 To m_Graph.Layers(i).Lines.Count - 1
                    m_Graph.Layers(i).Lines(j).IsShown = m_GroupDisplayFlags(j + 1)
                Next
            Next

            m_Graph.GenerateOutputImage()
            UpdateGroups()

        End Sub

        Private Sub OnTextBoxKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tstbxYAxisValue.KeyDown
            ' Is [ENTER]?
            If e.KeyCode = Keys.Enter Then Me.UpdateFixedGraphYScale(True)
        End Sub

        Private Sub OnValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tstbxYAxisValue.Leave, tstbxYAxisValue.Validated
            Me.UpdateFixedGraphYScale(True)
        End Sub

        Private Sub tstbxYAxisValue_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tstbxYAxisValue.TextChanged
            Me.UpdateFixedGraphYScale(True)
        End Sub

        Private Sub m_tsbSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbSet.Click
            Me.UpdateFixedGraphYScale(True)
        End Sub

#End Region ' Events

#Region " Internal Implementation "

        Private Sub UpdateFixedGraphYScale(Optional ByVal bForceFixedScale As Boolean = False)

            Dim bMustFixScale As Boolean = (Me.m_bBatch And Me.m_Graph.IsOverlay) Or (bForceFixedScale = True)

            If (Me.m_Graph Is Nothing) Then Return
            If (Me.m_fpYAxisValue Is Nothing) Then Return

            ' Set state
            If bMustFixScale = True Then
                Me.m_Graph.IsFixedYAxisScale = True
                Me.m_Graph.FixedYAxisScaleMax = Math.Max(0.0001!, CSng(Me.m_fpYAxisValue.Value))
            Else
                ' Set value
                Me.m_Graph.IsFixedYAxisScale = (Me.AutoscaleToolstripButton.CheckState = CheckState.Unchecked)
            End If
            ' Update self
            Me.UpdateControls()

        End Sub

        Private Sub UpdateGraph()

            If lbGroups.SelectedIndex <= 0 Then
                m_Graph.IsGrayOut = False
            Else
                m_Graph.IsGrayOut = True
            End If

            Dim iSec As Integer = lbGroups.SelectedIndex
            If iSec = -1 Then Return 'Nothing gets selected

            If iSec = 0 Then

                For i As Integer = 0 To m_Graph.Layers.Count - 1
                    For j As Integer = 0 To m_Graph.Layers(i).Lines.Count - 1
                        m_Graph.Layers(i).Lines(j).IsGrayOut = False
                    Next
                Next
            Else
                iSec = CInt(lbGroups.SelectedItem())
                For i As Integer = 0 To m_Graph.Layers.Count - 1
                    For j As Integer = 0 To m_Graph.Layers(i).Lines.Count - 1
                        m_Graph.Layers(i).Lines(j).IsGrayOut = (iSec <> (j + 1))
                    Next
                Next

            End If

            m_Graph.GenerateOutputImage()

        End Sub

        Private Sub UpdateGroups()

            lbGroups.Items.Clear()

            'Add "All groups" at the top
            lbGroups.Items.Add(0)

            If m_Graph.Layers.Count > 0 Then
                Dim iLayer As Integer = 0
                For j As Integer = 0 To m_Graph.Layers(iLayer).Lines.Count - 1
                    If m_Graph.Layers(iLayer).Lines(j).IsShown Then
                        lbGroups.Items.Add(j + 1)
                    End If
                Next
            End If

        End Sub

        Private Sub UpdateLayers()

            clbLayers.Items.Clear()

            For i As Integer = 0 To m_Graph.Layers.Count - 1
                Dim n As String = String.Format(My.Resources.LABEL_LAYER_NUMBERED, (i + 1).ToString)
                clbLayers.Items.Add(n, m_Graph.Layers(i).IsShown)
            Next

        End Sub

        Private Sub AddGroups()

            If lbGroups.Items.Count = 0 Then
                'Add "All groups" at the top
                lbGroups.Items.Add(0)
                For i As Integer = 1 To Me.m_core.nGroups
                    lbGroups.Items.Add(i)
                Next
            End If

        End Sub

        Private Sub UpdateControls()
            If Me.m_Graph.IsFixedYAxisScale Then
                Me.AutoscaleToolstripButton.CheckState = CheckState.Unchecked
            Else
                Me.AutoscaleToolstripButton.CheckState = CheckState.Checked
            End If
            Me.AutoscaleToolstripButton.Checked = (Me.m_Graph.IsFixedYAxisScale = False)
            Me.AutoscaleToolstripButton.Enabled = (Me.m_bBatch = False) Or (Me.m_bBatch = True And Me.m_Graph.IsOverlay = False)
            Me.OverlayToolStripMenuItem.Checked = Me.m_Graph.IsOverlay
            Me.AnnualOutputToolStripMenuItem.Checked = Me.m_Graph.IsShowAnnualOutput
        End Sub

#End Region ' Internal Implementation

#Region " Group list handling "

        ''' <summary>
        ''' Listbox drawItem method getting called when the drawMode is either OwnerDrawFixed or OwnerDrawVariable
        ''' </summary>
        ''' <remarks>To customize drawing so we can draw colorbox next to text</remarks>
        Private Sub lbItems_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles lbGroups.DrawItem

            ' get the sender of this event
            Dim s As ListBox = DirectCast(sender, ListBox)
            Dim iGroup As Integer = 0
            Dim strItemText As String = ""
            Dim clr As Color = Nothing
            Dim rect As Rectangle = Nothing

            If s Is Nothing Then Return
            If e.Index = -1 Then Return

            Try
                'The rectangle to draw the color box
                rect = New Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Height * 2, e.Bounds.Height - 4)
                iGroup = CInt(s.Items(e.Index))

                ' Sanity check
                If iGroup <= Me.m_core.nGroups Then
                    If iGroup = 0 Then
                        strItemText = My.Resources.VALUE_ALLGROUPS
                        clr = s.BackColor
                    Else
                        Dim group As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iGroup)
                        strItemText = group.Name
                        clr = StyleGuide.GetInstance().GroupColor(Me.m_core, iGroup)
                        'clr = group.PoolColorArgb
                    End If
                Else
                    strItemText = "" ' Deleted
                    clr = Color.Gray
                End If

                Me.DrawCustomItem(e, clr, strItemText, rect)

            Catch ex As Exception
                Debug.Assert(False)
                Return
            End Try
        End Sub

        ''' <summary>
        ''' Helper methods to draw a custom listcontrol item 
        ''' </summary>
        ''' <param name="e">DrawItemEventArgs sent by DrawItem event handler</param>
        ''' <param name="clr">The colorbox's color</param>
        ''' <param name="strItem">The text beside the colorbox</param>
        ''' <remarks>This method is called by both Listbox and Combobox drawItem event handlers</remarks>
        Private Sub DrawCustomItem(ByVal e As System.Windows.Forms.DrawItemEventArgs, _
                                    ByVal clr As Color, _
                                    ByRef strItem As String, _
                                    ByRef rcItem As Rectangle)


            ' Do nothing if there is no data
            If e.Index = -1 Then Return

            'If the item is selected, draw the correct background color
            e.DrawBackground()
            e.DrawFocusRectangle()

            'Get the listbox's graphics object
            Dim g As Graphics = e.Graphics

            'Draw color box
            g.FillRectangle(New SolidBrush(clr), rcItem)
            g.DrawRectangle(Pens.Black, rcItem)
            'Draw text 
            g.DrawString(strItem, e.Font, New SolidBrush(e.ForeColor), _
                            New RectangleF(e.Bounds.X + rcItem.Width + 4, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height))


        End Sub

#End Region

    End Class

End Namespace


