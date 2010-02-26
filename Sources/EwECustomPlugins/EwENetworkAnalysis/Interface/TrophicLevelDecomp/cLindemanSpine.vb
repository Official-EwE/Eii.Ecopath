#Region " Imports "

Option Strict On
Imports EwEUtils
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports System.Drawing.Imaging
Imports ZedGraph
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class cLindemanSpine
    Inherits cContentManager

    Private m_graph As cLindemanSpineDiagram = Nothing
    Private m_plGraph As Panel = Nothing

    ''' <summary>Custom toolstrip item</summary>
    Private m_tsStyle As ToolStripDropDownButton = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiCollapseDet As ToolStripMenuItem = Nothing

    ''' <summary>Custom toolstrip item</summary>
    Private m_tsContent As ToolStripDropDownButton = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiShowImport As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiShowTST As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiShowBiomass As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiShowBiomassAccum As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip items for selecting Trophic Levels</summary>
    Private m_ltsmiTL As New List(Of ToolStripMenuItem)

    Private m_sg As cStyleGuide = Nothing


    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip, _
                                     ByVal uic As cUIContext) As Boolean

        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip, uic)

        Me.Plot.Visible = bSucces
        Me.Toolstrip.Visible = bSucces
        Me.ToolstripShowOptionEMF()
        Me.AddToolstripItems()

        Me.m_graph = New cLindemanSpineDiagram(manager, Me.StyleGuide)
        Me.m_plGraph = New Panel()
        Me.Plot.Controls.Add(Me.m_plGraph)
        Me.Plot.AutoScroll = True

        Me.m_sg = cStyleGuide.GetInstance()

        AddHandler Me.m_plGraph.Paint, AddressOf PaintUC
        AddHandler Me.m_plGraph.Resize, AddressOf ResizeUC
        AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

        Return bSucces

    End Function

    Public Overrides Sub Detach()

        RemoveHandler Me.m_plGraph.Paint, AddressOf PaintUC
        RemoveHandler Me.m_plGraph.Resize, AddressOf ResizeUC
        RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

        Me.Plot.Controls.Remove(Me.m_plGraph)

        Me.m_plGraph = Nothing
        Me.m_graph = Nothing
        Me.m_sg = Nothing

        Me.RemoveToolstripItems()

        MyBase.Detach()

    End Sub

    Public Overrides Sub DisplayData()
        Me.m_plGraph.Size = Me.m_graph.Size
        Me.m_plGraph.Refresh()
        Me.UpdateControls()
    End Sub

    Public Overrides Function Filename(ByVal bAnnual As Boolean) As String
        Return "EwE6-NA_lindeman-spine"
    End Function

    Public Overrides Sub SaveToEMF(ByVal strFileName As String)

        Dim fs As FileStream = Nothing
        Dim bmp As Bitmap = Nothing
        Dim hdc As IntPtr = Nothing ' :)
        Dim mf As Metafile = Nothing

        Me.Plot.Refresh() 'm_Panel.Refresh()
        fs = New FileStream(strFileName, FileMode.Create)
        bmp = New Bitmap(Me.Plot.Width, Me.Plot.Height, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            hdc = g.GetHdc()
            mf = New Metafile(fs, hdc, EmfType.EmfOnly)
            g.ReleaseHdc(hdc)
        End Using

        Using g As Graphics = Graphics.FromImage(mf)
            PlotToEMF(g)
        End Using
        fs.Close()
        mf.Dispose()
    End Sub

    Private Sub PaintUC(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        PlotToScreen(e.Graphics)
    End Sub

    Private Sub ResizeUC(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.Plot.Invalidate()
    End Sub

    Private Sub PlotToScreen(ByVal g As Graphics)
        Me.m_graph.Draw(g)
    End Sub

    Private Sub PlotToEMF(ByVal g As Graphics)
        Me.m_graph.Draw(g)
    End Sub

    Private Sub AddToolstripItems()

        Dim tsmi As ToolStripMenuItem = Nothing
        Dim tsmiChild As ToolStripMenuItem = Nothing

        Me.m_tsmiCollapseDet = New ToolStripMenuItem(My.Resources.MNU_STYLE_DETRITUS_COLLAPSE)
        AddHandler Me.m_tsmiCollapseDet.Click, AddressOf OnStyleCollapseDetritus

        Me.m_tsStyle = New ToolStripDropDownButton(My.Resources.MNU_STYLE)
        Me.m_tsStyle.DropDownItems.Add(Me.m_tsmiCollapseDet)
        Me.Toolstrip.Items.Add(Me.m_tsStyle)

        Me.m_tsmiShowImport = New ToolStripMenuItem(My.Resources.MNU_CONTENT_SHOW_IMPORT)
        AddHandler Me.m_tsmiShowImport.Click, AddressOf OnContentShowImport

        Me.m_tsmiShowTST = New ToolStripMenuItem(My.Resources.MNU_CONTENT_SHOW_TST)
        AddHandler Me.m_tsmiShowTST.Click, AddressOf OnContentShowTST

        Me.m_tsmiShowBiomass = New ToolStripMenuItem(My.Resources.MNU_CONTENT_SHOW_BIOMASS)
        AddHandler Me.m_tsmiShowBiomass.Click, AddressOf OnContentShowBiomass

        Me.m_tsmiShowBiomassAccum = New ToolStripMenuItem(My.Resources.MNU_CONTENT_SHOW_BIOMASSACCUM)
        AddHandler Me.m_tsmiShowBiomassAccum.Click, AddressOf OnContentShowBiomassAccum

        ' Create max TL sub menu
        tsmi = New ToolStripMenuItem(My.Resources.MNU_CONTENT_MAXTL)
        For iTL As Integer = 1 To Me.NetworkManager.nTrophicLevels
            tsmiChild = New ToolStripMenuItem(cStringUtils.ToRoman(iTL))
            tsmiChild.Tag = iTL
            AddHandler tsmiChild.Click, AddressOf OnContentNumTrophicLevels
            tsmi.DropDownItems.Add(tsmiChild)
            Me.m_ltsmiTL.Add(tsmiChild)
        Next

        Me.m_tsContent = New ToolStripDropDownButton(My.Resources.MNU_CONTENT)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiShowImport)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiShowTST)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiShowBiomass)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiShowBiomassAccum)
        Me.m_tsContent.DropDownItems.Add(New ToolStripSeparator)
        Me.m_tsContent.DropDownItems.Add(tsmi)
        Me.Toolstrip.Items.Add(Me.m_tsContent)

    End Sub

    Private Sub RemoveToolstripItems()

        Me.Toolstrip.Items.Remove(Me.m_tsStyle)

        Me.m_tsStyle.DropDownItems.Clear()
        RemoveHandler Me.m_tsmiCollapseDet.Click, AddressOf OnStyleCollapseDetritus
        Me.m_tsmiCollapseDet = Nothing
        Me.m_tsStyle = Nothing

        Me.Toolstrip.Items.Remove(Me.m_tsContent)

        Me.m_tsContent.DropDownItems.Clear()
        RemoveHandler Me.m_tsmiShowImport.Click, AddressOf OnContentShowImport
        Me.m_tsmiShowImport = Nothing
        RemoveHandler Me.m_tsmiShowTST.Click, AddressOf OnContentShowTST
        Me.m_tsmiShowTST = Nothing
        RemoveHandler Me.m_tsmiShowBiomass.Click, AddressOf OnContentShowBiomass
        Me.m_tsmiShowBiomass = Nothing
        RemoveHandler Me.m_tsmiShowBiomassAccum.Click, AddressOf OnContentShowBiomassAccum

        For Each tsmi As ToolStripMenuItem In Me.m_ltsmiTL
            RemoveHandler tsmi.Click, AddressOf OnContentNumTrophicLevels
        Next
        Me.m_ltsmiTL.Clear()

        Me.m_tsmiShowBiomassAccum = Nothing
        Me.m_tsContent = Nothing

    End Sub

    Private Sub OnStyleCollapseDetritus(ByVal sender As Object, ByVal arg As EventArgs)
        Me.m_graph.CollapseDetritus = Not Me.m_graph.CollapseDetritus
        Me.DisplayData()
    End Sub

    Private Sub OnContentShowImport(ByVal sender As Object, ByVal arg As EventArgs)
        Me.m_graph.ShowImport = Not Me.m_graph.ShowImport
        Me.DisplayData()
    End Sub

    Private Sub OnContentShowTST(ByVal sender As Object, ByVal arg As EventArgs)
        Me.m_graph.ShowTST = Not Me.m_graph.ShowTST
        Me.DisplayData()
    End Sub

    Private Sub OnContentShowBiomass(ByVal sender As Object, ByVal arg As EventArgs)
        Me.m_graph.ShowBiomass = Not Me.m_graph.ShowBiomass
        Me.DisplayData()
    End Sub

    Private Sub OnContentShowBiomassAccum(ByVal sender As Object, ByVal arg As EventArgs)
        Me.m_graph.ShowBiomassAccum = Not Me.m_graph.ShowBiomassAccum
        Me.DisplayData()
    End Sub

    Private Sub OnContentNumTrophicLevels(ByVal sender As Object, ByVal args As EventArgs)
        Me.m_graph.NumTrophicLevels = CInt(DirectCast(sender, ToolStripMenuItem).Tag)
        Me.DisplayData()
    End Sub

    Private Sub UpdateControls()

        Me.m_tsmiCollapseDet.Checked = Me.m_graph.CollapseDetritus

        Me.m_tsmiShowImport.Checked = Me.m_graph.ShowImport
        Me.m_tsmiShowTST.Checked = Me.m_graph.ShowTST
        Me.m_tsmiShowBiomass.Checked = Me.m_graph.ShowBiomass
        Me.m_tsmiShowBiomassAccum.Checked = Me.m_graph.ShowBiomassAccum

        For iTL As Integer = 1 To Me.NetworkManager.nTrophicLevels
            Me.m_ltsmiTL(iTL - 1).Checked = (iTL = Me.m_graph.NumTrophicLevels)
        Next

    End Sub

End Class
