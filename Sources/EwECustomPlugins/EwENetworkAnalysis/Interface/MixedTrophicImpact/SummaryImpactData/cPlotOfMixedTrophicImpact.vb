'==============================================================================
'
' $Log: cPlotOfMixedTrophicImpact.vb,v $
' Revision 1.9  2008/12/10 20:56:19  joeh
' Finalize the Suitability Plot
'
' Revision 1.8  2008/12/05 19:45:33  joeh
' Add "Impacting group" and "Impacted group"
'
' Revision 1.7  2008/12/04 21:41:53  joeh
' Change the location and size of ucPlotOfMixedTrophicImpact so that the upper end of the vertical labels will not be hidden
'
' Revision 1.6  2008/12/04 01:14:16  joeh
' Add ucPlotOfMixedTrophicImpact
'
' Revision 1.5  2008/12/03 20:49:19  joeh
' Incorportate Functional Response into Network Analysis - Take three
'
' Revision 1.4  2008/12/03 18:43:48  joeh
' Incorportate Functional Response into Network Analysis - Take two
'
' Revision 1.3  2008/12/02 03:05:37  joeh
' Initial version
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.Windows.Forms
Imports ScientificInterfaceShared
Imports System.Drawing
Imports System.IO
Imports EwEUtils.Commands
Imports System.Drawing.Imaging

#End Region ' Imports

Public Class cPlotOfMixedTrophicImpact
    Public Event AddToolStrip()

    Private Shared m_PlotOfMixedTrophicImpactInstance As cPlotOfMixedTrophicImpact

    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel
    Private m_asData(,) As Single
    Private m_astrLabelsX() As String
    Private m_astrLabelsY() As String

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cPlotOfMixedTrophicImpact
        m_Panel = Panel

        If m_PlotOfMixedTrophicImpactInstance Is Nothing Then m_PlotOfMixedTrophicImpactInstance = New cPlotOfMixedTrophicImpact(NetworkManager, Panel)
        Return m_PlotOfMixedTrophicImpactInstance
    End Function

    Private Sub New()
        '
    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        m_Panel = Panel

        ReDim m_asData(m_NetworkManager.nGroups + m_NetworkManager.nFleets, m_NetworkManager.nGroups + m_NetworkManager.nFleets)
        ReDim m_astrLabelsX(m_NetworkManager.nGroups + m_NetworkManager.nFleets)
        ReDim m_astrLabelsY(m_NetworkManager.nGroups + m_NetworkManager.nFleets)
        For i As Integer = 1 To m_NetworkManager.nGroups + m_NetworkManager.nFleets
            For j As Integer = 1 To m_NetworkManager.nGroups + m_NetworkManager.nFleets
                If j <= m_NetworkManager.nGroups Then
                    m_astrLabelsX(j) = m_NetworkManager.GroupName(j)
                Else
                    m_astrLabelsX(j) = m_NetworkManager.FleetName(j - m_NetworkManager.nGroups)
                End If
                m_astrLabelsY(j) = m_astrLabelsX(j)
                m_asData(i, j) = m_NetworkManager.MixedTrophicImpacts(j, i)
            Next j
        Next i
    End Sub

    Public Sub SetUpPanel()
        SetUpToolStrip()

        SetUpGrid()
    End Sub

    Public Sub CreatePlot() '(ByVal Frm As Form)
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim MixedTrophicImpactUC As ucPlotOfMixedTrophicImpact = _
            CType(m_Panel.Controls("ucPlotOfMixedTrophicImpact"), ucPlotOfMixedTrophicImpact)

        If MixedTrophicImpactUC Is Nothing Then
            MixedTrophicImpactUC = New ucPlotOfMixedTrophicImpact
            MixedTrophicImpactUC.Name = "ucPlotOfMixedTrophicImpact"
            MixedTrophicImpactUC.Location = New Point(0, ToolStrip.Height)
            MixedTrophicImpactUC.Size = New Size(m_Panel.Width, m_Panel.Height - ToolStrip.Height)
            m_Panel.Controls.Add(MixedTrophicImpactUC) 'MixedTrophicImpactVisible is defaulted to True
            'g = MixedTrophicImpactUC.CreateGraphics 'm_Panel.CreateGraphics
            'PlotToScreen(MixedTrophicImpactUC, g)
        Else
            MixedTrophicImpactUC.Visible = True
        End If

        AddHandler MixedTrophicImpactUC.Paint, AddressOf PaintUC
        AddHandler MixedTrophicImpactUC.Resize, AddressOf ResizeUC
    End Sub

    Public Sub SaveToEMF()
        Dim cmdh As CommandHandler = CommandHandler.GetInstance()
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
        Dim fs As FileStream = Nothing
        Dim bmp As Bitmap = Nothing
        Dim hdc As IntPtr = Nothing ' :)
        Dim mMetafile As Metafile = Nothing
        Dim MixedTrophicImpactUC As ucPlotOfMixedTrophicImpact

        MixedTrophicImpactUC = CType(m_Panel.Controls("ucPlotOfMixedTrophicImpact"), ucPlotOfMixedTrophicImpact)
        cmdFS.Invoke("emf files (*.emf)|*.emf|All files (*.*)|*.*", 1)
        If cmdFS.Result = Windows.Forms.DialogResult.OK Then
            MixedTrophicImpactUC.Refresh() 'm_Panel.Refresh()
            fs = New FileStream(cmdFS.FileName, FileMode.Create)
            'bmp = New Bitmap(200, 200, PixelFormat.Format32bppArgb)
            bmp = New Bitmap(MixedTrophicImpactUC.Width, MixedTrophicImpactUC.Height, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                hdc = g.GetHdc()
                mMetafile = New Metafile(fs, hdc, EmfType.EmfOnly)
                g.ReleaseHdc(hdc)
            End Using

            Using g As Graphics = Graphics.FromImage(mMetafile)
                PlotToEMF(g)
            End Using
            fs.Close()
            mMetafile.Dispose()
        End If
    End Sub

    Private Sub SetUpToolStrip()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim ToolStripLabel1 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripLabel2 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripLabel3 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripCombo1 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox
        Dim ToolStripCombo2 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox
        Dim ToolStripPrgBar As Windows.Forms.ToolStripProgressBar = New Windows.Forms.ToolStripProgressBar
        Dim ToolStripButton1 As Windows.Forms.ToolStripButton = New Windows.Forms.ToolStripButton
        Dim ToolStripButton2 As Windows.Forms.ToolStripButton = New Windows.Forms.ToolStripButton
        Dim ToolStripButton3 As Windows.Forms.ToolStripButton = New Windows.Forms.ToolStripButton
        Dim ToolStripButton4 As Windows.Forms.ToolStripButton = New Windows.Forms.ToolStripButton

        RemoveToolStrip()

        RaiseEvent AddToolStrip()

        ToolStrip = CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        ToolStripLabel1 = CType(ToolStrip.Items("tslblSelection1"), Windows.Forms.ToolStripLabel)
        ToolStripLabel2 = CType(ToolStrip.Items("tslblSelection2"), Windows.Forms.ToolStripLabel)
        ToolStripLabel3 = CType(ToolStrip.Items("tslblProgressBar"), Windows.Forms.ToolStripLabel)
        ToolStripCombo1 = CType(ToolStrip.Items("tscmbSelection1"), Windows.Forms.ToolStripComboBox)
        ToolStripCombo2 = CType(ToolStrip.Items("tscmbSelection2"), Windows.Forms.ToolStripComboBox)
        ToolStripPrgBar = CType(ToolStrip.Items("tspgbProgressBar"), Windows.Forms.ToolStripProgressBar)
        ToolStripButton1 = CType(ToolStrip.Items("tsbtnCancel"), Windows.Forms.ToolStripButton)
        ToolStripButton2 = CType(ToolStrip.Items("tsbtnOutputIndicesCSV"), Windows.Forms.ToolStripButton)
        ToolStripButton3 = CType(ToolStrip.Items("tsbtnOutputGraphEMF"), Windows.Forms.ToolStripButton)
        ToolStripButton4 = CType(ToolStrip.Items("tsbtnPrintGraph"), Windows.Forms.ToolStripButton)

        ToolStripLabel1.Visible = False
        ToolStripCombo1.Visible = False

        ToolStripLabel2.Visible = False
        ToolStripCombo2.Visible = False

        ToolStripLabel3.Visible = False
        ToolStripPrgBar.Visible = False
        ToolStripButton1.Visible = False
        ToolStripButton2.Visible = False
        ToolStripButton3.Visible = True
        ToolStripButton4.Visible = False

        ToolStrip.Refresh()
    End Sub

    Private Sub SetUpGrid()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim GraphPane As ZedGraphControl = _
            CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As Windows.Forms.TableLayoutPanel = _
            CType(m_Panel.Controls("tlpNetworkAnalysis"), Windows.Forms.TableLayoutPanel)
        'Dim FunctRespUC As ucFunctionalResponse = _
        '    CType(m_Panel.Controls("ucFUnctionalResponse"), ucFunctionalResponse)

        LogoPanel.Visible = False
        DataGrid.Visible = False
        GraphPane.Visible = False
        'If Not FunctRespUC Is Nothing Then FunctRespUC.Visible = False
    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        'Dim GraphPane As ZedGraphControl = _
        '    CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)

        If Not ToolStrip Is Nothing Then
            m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = Windows.Forms.DockStyle.Fill
            'GraphPane.Dock = DockStyle.Fill
        End If
    End Sub

    Private Sub PaintUC(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        Dim MixedTrophicImpactUC As ucPlotOfMixedTrophicImpact
        Dim g As Drawing.Graphics

        MixedTrophicImpactUC = CType(m_Panel.Controls("ucPlotOfMixedTrophicImpact"), ucPlotOfMixedTrophicImpact)
        'g = MixedTrophicImpactUC.CreateGraphics
        g = e.Graphics
        PlotToScreen(MixedTrophicImpactUC, g)
    End Sub

    Private Sub ResizeUC(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim MixedTrophicImpactUC As ucPlotOfMixedTrophicImpact

        MixedTrophicImpactUC = CType(m_Panel.Controls("ucPlotOfMixedTrophicImpact"), ucPlotOfMixedTrophicImpact)
        MixedTrophicImpactUC.Invalidate() 'm_Panel.Invalidate()
    End Sub

    Private Sub PlotToScreen(ByVal uc As ucPlotOfMixedTrophicImpact, ByVal g As Graphics)
        Dim ag As New ArrayGraph()
        Dim r As Rectangle
        Dim MixedTrophicImpactUC As ucPlotOfMixedTrophicImpact

        MixedTrophicImpactUC = CType(m_Panel.Controls("ucPlotOfMixedTrophicImpact"), ucPlotOfMixedTrophicImpact)
        r.X = MixedTrophicImpactUC.ClientRectangle.X
        r.Y = 0
        r.Width = MixedTrophicImpactUC.ClientRectangle.Width
        r.Height = MixedTrophicImpactUC.ClientRectangle.Height - r.Y
        ' Draw on client area only; me.width and me.height include space occupied by borders, caption bar, etc
        ag.Draw(g, r, m_asData, My.Resources.LBL_IMPACTED_GP, m_astrLabelsX, My.Resources.LBL_IMPACTING_GP, m_astrLabelsY)
    End Sub

    Private Sub PlotToEMF(ByVal g As Graphics)
        'g.DrawEllipse(Pens.Green, New Rectangle(10, 10, 380, 380))
        Dim ag As New ArrayGraph()
        Dim r As Rectangle
        Dim MixedTrophicImpactUC As ucPlotOfMixedTrophicImpact

        MixedTrophicImpactUC = CType(m_Panel.Controls("ucPlotOfMixedTrophicImpact"), ucPlotOfMixedTrophicImpact)
        r.X = MixedTrophicImpactUC.ClientRectangle.X
        r.Y = 0
        r.Width = MixedTrophicImpactUC.ClientRectangle.Width ' * 3
        r.Height = (MixedTrophicImpactUC.ClientRectangle.Height - r.Y) ' * 3
        ' Draw on client area only; me.width and me.height include space occupied by borders, caption bar, etc
        ag.Draw(g, r, m_asData, My.Resources.LBL_IMPACTED_GP, m_astrLabelsX, My.Resources.LBL_IMPACTING_GP, m_astrLabelsY)
    End Sub
End Class
