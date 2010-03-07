#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Database
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmMain

#Region " Definitions "

    Private m_plugin As cPluginPoint = Nothing

#End Region ' Definitions

#Region " Constructor "

    Public Sub New(ByVal plugin As cPluginPoint, ByVal strTitle As String)
        InitializeComponent()

        Me.m_plugin = plugin

        Me.Text = strTitle
        Me.TabText = strTitle

        ' Expand all nodes
        For Each tn As TreeNode In Me.m_tvNav.Nodes
            Me.ExpandNodes(tn)
        Next

        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

    End Sub

#End Region ' Constructor

#Region " Event handlers "

    Private Sub ExpandNodes(ByVal tn As TreeNode)
        tn.ExpandAll()
        For Each tnChild As TreeNode In tn.Nodes
            Me.ExpandNodes(tnChild)
        Next
    End Sub

    Private Sub tvECost_AfterSelect(ByVal sender As System.Object, ByVal e As TreeViewEventArgs) _
        Handles m_tvNav.AfterSelect

        Select Case e.Node.Name
            Case "ndParameters"
                Me.ShowForm(New ucParameters(Me.m_plugin.Data, Me.m_plugin.Context))
            Case "ndProducer"
                Me.ShowForm(New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Producer))
            Case "ndProcessing"
                Me.ShowForm(New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Processing))
            Case "ndDistribution"
                Me.ShowForm(New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Distribution))
            Case "ndMarket"
                Me.ShowForm(New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Market))
            Case "ndConsumer"
                Me.ShowForm(New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Consumer))
            Case "ndFlow"
                Me.ShowForm(New ucEditFlow(Me.m_plugin.Context, Me.m_plugin.Data, Me.m_plugin.Data.FlowDiagram(0)))
            Case "ndDefaults"
                Me.ShowForm(New ucDefaults(Me.m_plugin.Context, Me.m_plugin.Data))
            Case "ndRun"
                Me.ShowForm(New ucResults(Me.m_plugin.Context, Me.m_plugin.Data, Me.m_plugin.Model, Me.m_plugin.Results))
        End Select

    End Sub

    Private Sub ShowForm(ByVal f As Control)

        Dim pl As Panel = Me.scMain.Panel2
        Dim ctrl As Control = Nothing

        pl.SuspendLayout()

        If TypeOf f Is IUIElement Then
            DirectCast(f, IUIElement).UIContext = Me.m_plugin.Context
        End If

        f.Dock = DockStyle.Fill
        While pl.Controls.Count > 0
            ctrl = pl.Controls(0)
            pl.Controls.Remove(ctrl)
            ctrl.Dispose()
        End While
        pl.Controls.Add(f)
        pl.ResumeLayout()

    End Sub

#End Region ' Event handlers

End Class