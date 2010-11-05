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
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' MTI graph with circles
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class cPlotOfMixedTrophicImpact
    Inherits cContentManager

    Private m_asData(,) As Single
    Private m_astrLabelsX() As String
    Private m_astrLabelsY() As String

    Public Sub New()
        '
    End Sub

    Public Overrides Function PageTitle() As String
        Return "Mixed tropic level impacts (external)"
    End Function

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

        AddHandler Me.Plot.Paint, AddressOf PaintUC
        AddHandler Me.Plot.Resize, AddressOf ResizeUC
        Return bSucces
    End Function

    Public Overrides Sub Detach()
        RemoveHandler Me.Plot.Paint, AddressOf PaintUC
        RemoveHandler Me.Plot.Resize, AddressOf ResizeUC
        MyBase.Detach()
    End Sub

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

    Public Overrides Function Filename(ByVal bAnnual As Boolean) As String
        Return "EwE6-NA_mixed-trophic-impact"
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
