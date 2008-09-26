'==============================================================================
'
' $Log: frmIndicesWithoutPPREst.vb,v $
' Revision 1.1  2008/09/26 07:30:52  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.10  2008/06/18 21:48:14  joeh
' Compute and send Ecosim NA data to csv file - Take 2
'
' Revision 1.9  2008/06/18 20:16:03  joeh
' Plot Ascendency on flow in a second pane
'
' Revision 1.8  2008/06/14 00:00:28  joeh
' Compute and send ecosim NA data to csv file
'
' Revision 1.7  2008/06/09 22:26:14  joeh
' Extract data to CSV via ZedGraphControl - Take 1
'
' Revision 1.6  2007/06/22 19:12:47  joeh
' Modify GetInstance()
'
' Revision 1.5  2007/06/22 00:35:30  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.4  2007/06/20 23:34:01  joeh
' Add Panel as a new argument in GetInstance() and New()
'
' Revision 1.3  2007/06/20 18:13:58  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.IO
Imports ZedGraph

Public Class frmIndicesWithoutPPREst
    Private Shared m_IndicesWithoutPPREstInstance As frmIndicesWithoutPPREst

    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel
    Private m_IndicesWithoutPPREst As cIndicesWithoutPPREst

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As frmIndicesWithoutPPREst
        m_Panel = Panel

        If m_IndicesWithoutPPREstInstance Is Nothing Then m_IndicesWithoutPPREstInstance = New frmIndicesWithoutPPREst(NetworkManager, Panel)
        Return m_IndicesWithoutPPREstInstance
    End Function

    Private Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'm_Core = cCore.GetInstance()

    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        m_Panel = Panel
    End Sub

    Private Sub frmIndices_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim IndicesWithoutPPREst As cIndicesWithoutPPREst
        m_IndicesWithoutPPREst = cIndicesWithoutPPREst.GetInstance(m_NetworkManager, m_Panel)
        'm_IndicesWithoutPPREst.SetUpPanel()
        m_IndicesWithoutPPREst.CreatePlot(Me, ZedGraphControl)
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub ZedGraphControl_ContextMenuBuilder(ByVal sender As ZedGraph.ZedGraphControl, ByVal menuStrip As System.Windows.Forms.ContextMenuStrip, ByVal mousePt As System.Drawing.Point, ByVal objState As ZedGraph.ZedGraphControl.ContextMenuObjectState) Handles ZedGraphControl.ContextMenuBuilder
        Dim item As ToolStripMenuItem = New ToolStripMenuItem()
        item.Name = "Extract_CSV_Data"
        item.Tag = "Extract_CSV_Data_tag"
        item.Text = "Extract to CSV..."
        AddHandler item.Click, AddressOf ExtractToCSV
        menuStrip.Items.Add(item)
    End Sub

    Private Sub ExtractToCSV(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim myStream As StreamWriter
        Dim sfDiag As New SaveFileDialog()

        sfDiag.Filter = "csv files (*.csv)|*.csv|text files (*.txt)|*.txt|All files (*.*)|*.*"
        sfDiag.FilterIndex = 1
        sfDiag.RestoreDirectory = True

        If sfDiag.ShowDialog() = Windows.Forms.DialogResult.OK Then
            myStream = New StreamWriter(sfDiag.FileName)
            If (myStream IsNot Nothing) Then
                myStream.Write(ExtractData)
                myStream.Close()
            End If
        End If
    End Sub

    Private Function ExtractData() As String
        Dim str As String = ""

        str = str + My.Resources.COL_HDR_THROUGHPUT + ", "
        str = str + My.Resources.COL_HDR_CAPACITY_ECOSIM + ", "
        str = str + My.Resources.COL_HDR_ASCEND_IMPORT + ", "
        str = str + My.Resources.COL_HDR_ASCEND_FLOW + ", "
        str = str + My.Resources.COL_HDR_ASCEND_EXPORT + ", "
        str = str + My.Resources.COL_HDR_ASCEND_RESP + ", "
        str = str + My.Resources.COL_HDR_OVERHEAD_IMPORT + ", "
        str = str + My.Resources.COL_HDR_OVERHEAD_FLOW + ", "
        str = str + My.Resources.COL_HDR_OVERHEAD_EXPORT + ", "
        str = str + My.Resources.COL_HDR_OVERHEAD_RESP + ", "
        str = str + My.Resources.COL_HDR_PCI + ", "
        str = str + My.Resources.COL_HDR_FCI + ", "
        str = str + My.Resources.COL_HDR_PATH_LEN + ", "
        str = str + My.Resources.COL_HDR_EXPORT + ", "
        str = str + My.Resources.COL_HDR_RESP_ECOSIM + ", "
        str = str + My.Resources.COL_HDR_PRIM_PROD + ", "
        str = str + My.Resources.COL_HDR_PROD + ", "
        str = str + My.Resources.COL_HDR_BIOMASS + ", "
        str = str + My.Resources.COL_HDR_CATCH + ", "
        str = str + My.Resources.COL_HDR_PROP_FLOW_DET + ", "
        str = str + vbCrLf

        For i As Integer = 1 To m_NetworkManager.nEcosimTimesteps
            str = str + m_NetworkManager.ThroughputEcosim(i).ToString + ", "
            str = str + m_NetworkManager.CapacityEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendImportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendFlowEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendExportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendRespEcosim(i).ToString + ", "
            str = str + m_NetworkManager.OverheadImportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.OverheadFlowEcosim(i).ToString + ", "
            str = str + m_NetworkManager.OverheadExportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.OverheadRespEcosim(i).ToString + ", "
            str = str + m_NetworkManager.PCIEcosim(i).ToString + ", "
            str = str + m_NetworkManager.FCIEcosim(i).ToString + ", "
            str = str + m_NetworkManager.PathLengthEcosim(i).ToString + ", "
            str = str + m_NetworkManager.ExportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.RespEcosim(i).ToString + ", "
            str = str + m_NetworkManager.PrimaryProdEcosim(i).ToString + ", "
            str = str + m_NetworkManager.ProdEcosim(i).ToString + ", "
            str = str + m_NetworkManager.BiomassEcosim(i).ToString + ", "
            str = str + m_NetworkManager.CatchEcosim(i).ToString + ", "
            str = str + m_NetworkManager.PropFlowDetEcosim(i).ToString + ", "
            str = str + vbCrLf
        Next

        Return str
    End Function
End Class

