'==============================================================================
'
' $Log: cPlotOfMixedTrophicImpact.vb,v $
' Revision 1.18  2009/05/01 17:42:59  jeroens
' Inherited from cContentManager
'
' Revision 1.17  2009/04/22 22:27:28  joeh
' Move MTI data assignment from New to CreatePlot
'
' Revision 1.16  2009/04/17 18:53:00  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.15  2009/04/17 01:07:04  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.14  2009/04/16 21:52:37  joeh
' Add Legends to the MTI plot
'
' Revision 1.13  2009/04/15 23:22:27  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.12  2009/04/15 18:13:53  joeh
' Make plot re-sizable
'
' Revision 1.11  2009/04/14 18:21:13  joeh
' Add separator to tool strip
'
' Revision 1.10  2009/04/09 20:04:47  joeh
' Add "Bar graph" button to plot bar graph for MTI
'
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
'==============================================================================

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

'MTI graph with circles
Public Class cPlotOfMixedTrophicImpact
    Inherits cContentManager

    Private m_asData(,) As Single
    Private m_astrLabelsX() As String
    Private m_astrLabelsY() As String

    Public Sub New()
        '
    End Sub

    Public Overrides Sub Attach(ByVal manager As cNetworkManager, _
                                 ByVal datagrid As DataGridView, _
                                 ByVal graph As ZedGraphControl, _
                                 ByVal plot As ucPlot)
        MyBase.Attach(manager, datagrid, graph, plot)
        Me.Plot.Visible = True
        AddHandler Me.Plot.Paint, AddressOf PaintUC
        AddHandler Me.Plot.Resize, AddressOf ResizeUC
    End Sub

    Public Overrides Sub Detach()
        RemoveHandler Me.Plot.Paint, AddressOf PaintUC
        RemoveHandler Me.Plot.Resize, AddressOf ResizeUC
        MyBase.Detach()
    End Sub

    Public Overrides Function RequiresToolstrip() As Boolean
        Return True
    End Function

    Public Overrides Sub DisplayData()

        ReDim m_asData(NetworkManager.nGroups + NetworkManager.nFleets, NetworkManager.nGroups + NetworkManager.nFleets)
        ReDim m_astrLabelsX(NetworkManager.nGroups + NetworkManager.nFleets)
        ReDim m_astrLabelsY(NetworkManager.nGroups + NetworkManager.nFleets)
        For i As Integer = 1 To NetworkManager.nGroups + NetworkManager.nFleets
            For j As Integer = 1 To NetworkManager.nGroups + NetworkManager.nFleets
                If j <= NetworkManager.nGroups Then
                    m_astrLabelsX(j) = NetworkManager.GroupName(j)
                Else
                    m_astrLabelsX(j) = NetworkManager.FleetName(j - NetworkManager.nGroups)
                End If
                m_astrLabelsY(j) = m_astrLabelsX(j)
                m_asData(i, j) = NetworkManager.MixedTrophicImpacts(j, i)
            Next j
        Next i

        Me.Plot.Invalidate()

    End Sub

    Public Overrides Sub SaveToEMF(ByVal strFileName As String)

        Dim fs As FileStream = Nothing
        Dim bmp As Bitmap = Nothing
        Dim hdc As IntPtr = Nothing ' :)
        Dim mf As Metafile = Nothing

        Me.Plot.Refresh() 'm_Panel.Refresh()
        fs = New FileStream(strFileName, FileMode.Create)
        'bmp = New Bitmap(200, 200, PixelFormat.Format32bppArgb)
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

    Public Overrides Sub SetUpToolStrip(ByVal ts As ToolStrip)

        MyBase.SetupToolstrip(ts)

        Dim tsbBntExportEMF As ToolStripButton = DirectCast(ts.Items("tsbtnOutputGraphEMF"), ToolStripButton)
        tsbBntExportEMF.Visible = True
        ts.Refresh()

    End Sub

    Private Sub PaintUC(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        PlotToScreen(e.Graphics)
    End Sub

    Private Sub ResizeUC(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.Plot.Invalidate()
    End Sub

    Private Sub PlotToScreen(ByVal g As Graphics)

        Dim ag As New ArrayGraph()
        Dim r As Rectangle
        Dim astrLegends() As String = {My.Resources.LBL_POSITIVE, My.Resources.LBL_NEGATIVE}

        r.X = Me.Plot.ClientRectangle.X
        r.Y = 0
        r.Width = Me.Plot.ClientRectangle.Width
        r.Height = Me.Plot.ClientRectangle.Height - r.Y
        ag.Draw(g, r, m_asData, My.Resources.LBL_IMPACTED_GP, m_astrLabelsX, My.Resources.LBL_IMPACTING_GP, m_astrLabelsY, _
                astrLegends)
    End Sub

    Private Sub PlotToEMF(ByVal g As Graphics)

        Dim ag As New ArrayGraph()
        Dim r As Rectangle
        Dim astrLegends() As String = {My.Resources.LBL_POSITIVE, My.Resources.LBL_NEGATIVE}

        r.X = Me.Plot.ClientRectangle.X
        r.Y = 0
        r.Width = Me.Plot.ClientRectangle.Width ' * 3
        r.Height = (Me.Plot.ClientRectangle.Height - r.Y) ' * 3
        ' Draw on client area only; me.width and me.height include space occupied by borders, caption bar, etc
        ag.Draw(g, r, m_asData, My.Resources.LBL_IMPACTED_GP, m_astrLabelsX, My.Resources.LBL_IMPACTING_GP, m_astrLabelsY, _
                astrLegends)
    End Sub

End Class
