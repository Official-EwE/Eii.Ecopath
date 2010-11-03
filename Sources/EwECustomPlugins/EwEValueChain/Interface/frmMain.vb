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

#Region " Vars "

    Private m_plugin As cPluginPoint = Nothing
    Private m_strForm As String = "N/A"

#End Region ' Vars

#Region " Constructor "

    Public Sub New(ByVal plugin As cPluginPoint, ByVal strTitle As String)
        InitializeComponent()

        Me.m_plugin = plugin

        Me.Text = strTitle
        Me.TabText = strTitle

        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

    End Sub

#End Region ' Constructor

    Public Overrides ReadOnly Property IsRunForm() As Boolean
        Get
            Return True
        End Get
    End Property

#Region " Event handlers "

    Private Sub ExpandNodes(ByVal tn As TreeNode)
        tn.ExpandAll()
        For Each tnChild As TreeNode In tn.Nodes
            Me.ExpandNodes(tnChild)
        Next
    End Sub

    Private Sub tvECost_AfterSelect(ByVal sender As System.Object, ByVal e As TreeViewEventArgs)

        Me.ShowForm(e.Node.Name)
    End Sub

    Public Sub ShowForm(ByVal strFormName As String)

        strFormName = Me.ResolveFormName(strFormName)

        If Me.m_strForm = strFormName Then Return

        Select Case strFormName
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
            Case Else
                Debug.Assert(False)
        End Select

        Me.m_strForm = strFormName

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Translate pageless node names to valid pages.
    ''' </summary>
    ''' <param name="strFormName"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ResolveFormName(ByVal strFormName As String) As String
        Select Case strFormName
            Case "" : Return "ndParameters"
            Case "ndTables" : Return "ndProducer"
        End Select
        Return strFormName
    End Function

    Private Sub ShowForm(ByVal f As Control)

        Dim ctrl As Control = Nothing

        Me.SuspendLayout()

        If TypeOf f Is IUIElement Then
            DirectCast(f, IUIElement).UIContext = Me.m_plugin.Context
        End If

        f.Dock = DockStyle.Fill
        While Me.Controls.Count > 0
            ctrl = Me.Controls(0)
            Me.Controls.Remove(ctrl)
            ctrl.Dispose()
        End While
        Me.Controls.Add(f)

        Me.ResumeLayout()

    End Sub

#End Region ' Event handlers

End Class