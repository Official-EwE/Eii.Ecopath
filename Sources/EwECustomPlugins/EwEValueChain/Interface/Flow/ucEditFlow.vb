#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Windows.Forms

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
Public Class ucEditFlow

    Private m_data As cData = Nothing
    Private m_diagram As cFlowDiagram = Nothing

    Public Sub New(ByVal data As cData, ByVal diagram As cFlowDiagram)

        Me.InitializeComponent()

        Debug.Assert(data IsNot Nothing)
        Debug.Assert(diagram IsNot Nothing, "Cannot created diagram editor without a valid diagram")

        Me.Data = data
        Me.Diagram = diagram
        Me.UpdateControls()

        AddHandler Me.m_plFlow.EditModeChanged, AddressOf Me.OnEditModeChanged

    End Sub

#Region " Event handling "

#Region " Saving "

    Private Sub m_tsmiSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsmiSave.Click, m_tsbSave.ButtonClick

        Me.m_data.Save()

    End Sub

    Private Sub m_tsmiExportToImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsmiExportToImage.Click

        MsgBox("Image save functionality not yet implemented")

    End Sub

#End Region ' Saving

#Region " Diagram controls "

    'Private Sub OnDiagram(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    '    Handles m_tsddDiagram.Click
    '    ' ToDo: invoke add/remove diagram dialog
    '    Me.UpdateControls()
    'End Sub

    'Private Sub OnSelectDiagram(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim tsi As ToolStripItem = DirectCast(sender, ToolStripItem)
    '    Me.Diagram = DirectCast(tsi.Tag, cFlowDiagram)
    'End Sub

#End Region ' Diagram controls

#Region " Mode buttons "

    Private Sub tsbMove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbMove.Click
        Me.m_plFlow.EditMode = plFlow.eEditMode.Move
    End Sub

    Private Sub tsbLink_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbLink.Click
        Me.m_plFlow.EditMode = plFlow.eEditMode.Link
    End Sub

    Private Sub tsbDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbDelete.Click
        Me.m_plFlow.EditMode = plFlow.eEditMode.Delete
    End Sub

    Private Sub OnEditModeChanged(ByVal pl As plFlow, ByVal mode As plFlow.eEditMode)
        Me.UpdateControls()
    End Sub

#End Region ' Mode buttons

#Region " Creation buttons "

    Private Sub OnCreateProducersByLandings(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbCreateProducersByLandings.Click
        Me.m_plFlow.CreateProducersByLandings()
        Me.UpdateControls()
    End Sub

    Private Sub OnCreateProducersByFleet(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbCreateProducersByFleets.Click
        Me.m_plFlow.CreateProducersByFleet()
        Me.UpdateControls()
    End Sub

    Private Sub OnCreateProducer(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbCreateProducer.Click
        Me.m_plFlow.CreateUnit(cUnitFactory.eUnitType.Producer)
        Me.UpdateControls()
    End Sub

    Private Sub OnCreateProcessing(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbCreateProcessing.Click
        Me.m_plFlow.CreateUnit(cUnitFactory.eUnitType.Processing)
        Me.UpdateControls()
    End Sub

    Private Sub OnCreateDistribution(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbCreateDistribution.Click
        Me.m_plFlow.CreateUnit(cUnitFactory.eUnitType.Distribution)
        Me.UpdateControls()
    End Sub

    Private Sub OnCreateMarket(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbCreateMarket.Click
        Me.m_plFlow.CreateUnit(cUnitFactory.eUnitType.Market)
        Me.UpdateControls()
    End Sub

    Private Sub OnCreateConsumer(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbCreateConsumer.Click
        Me.m_plFlow.CreateUnit(cUnitFactory.eUnitType.Consumer)
        Me.UpdateControls()
    End Sub

#End Region ' Creation buttons

#Region " Control buttons "

    Private Sub OnArrangeLayout(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbArrange.Click
        Me.m_plFlow.Arrange()
        Me.UpdateControls()
    End Sub

    Private Sub OnShowGrid(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbShowGrid.Click
        Me.m_plFlow.ShowGrid = Not Me.m_plFlow.ShowGrid
        Me.UpdateControls()
    End Sub

#End Region ' Control buttons

#Region " Zoomzoom "

    Private Sub m_tsddZoom_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles m_tsddZoom.Click
        ' NOP
    End Sub

    Private Sub m_tsmiZoom50_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiZoom50.Click
        Me.m_plFlow.ZoomFactor = 0.5!
        Me.UpdateControls()
    End Sub

    Private Sub m_tsmiZoom75_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiZoom75.Click
        Me.m_plFlow.ZoomFactor = 0.75!
        Me.UpdateControls()
    End Sub

    Private Sub m_tsmiZoom100_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiZoom100.Click
        Me.m_plFlow.ZoomFactor = 1.0!
        Me.UpdateControls()
    End Sub

    Private Sub m_tsmiZoom125_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiZoom125.Click
        Me.m_plFlow.ZoomFactor = 1.25!
        Me.UpdateControls()
    End Sub

    Private Sub m_tsmiZoom150_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiZoom150.Click
        Me.m_plFlow.ZoomFactor = 1.5!
        Me.UpdateControls()
    End Sub

    Private Sub m_tsmiZoom200_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiZoom200.Click
        Me.m_plFlow.ZoomFactor = 2.0!
        Me.UpdateControls()
    End Sub

#End Region ' moozmooZ

#End Region ' Event handling

#Region " Internals "

    Private Sub UpdateControls()

        Dim fd As cFlowDiagram = Nothing
        Dim tsi As ToolStripItem = Nothing

        Me.m_tsbMove.Checked = (Me.m_plFlow.EditMode = plFlow.eEditMode.Move)
        Me.m_tsbLink.Checked = (Me.m_plFlow.EditMode = plFlow.eEditMode.Link)
        Me.m_tsbDelete.Checked = (Me.m_plFlow.EditMode = plFlow.eEditMode.Delete)
        Me.m_tsbShowGrid.Checked = Me.m_plFlow.ShowGrid

        Me.m_tsmiZoom50.Checked = (Me.m_plFlow.ZoomFactor = 0.5!)
        Me.m_tsmiZoom75.Checked = (Me.m_plFlow.ZoomFactor = 0.75)
        Me.m_tsmiZoom100.Checked = (Me.m_plFlow.ZoomFactor = 1.0!)
        Me.m_tsmiZoom125.Checked = (Me.m_plFlow.ZoomFactor = 1.25!)
        Me.m_tsmiZoom150.Checked = (Me.m_plFlow.ZoomFactor = 1.5!)
        Me.m_tsmiZoom200.Checked = (Me.m_plFlow.ZoomFactor = 2.0!)

        '' Update list of avialable diagrams
        'With Me.m_tsddDiagram.DropDownItems
        '    .Clear()
        '    For i As Integer = 0 To Math.Max(0, Me.m_data.FlowDiagramCount - 1)
        '        fd = Me.m_data.FlowDiagram(i)
        '        tsi = New ToolStripMenuItem()
        '        tsi.Tag = fd
        '        tsi.Text = fd.Name
        '        tsi.ToolTipText = String.Format("View diagram '{0}'", fd.Name)
        '        AddHandler tsi.Click, AddressOf OnSelectDiagram
        '        .Add(tsi)
        '    Next
        'End With

    End Sub

    Public Property Diagram() As cFlowDiagram
        Get
            Return Me.m_diagram
        End Get
        Set(ByVal value As cFlowDiagram)
            If Object.ReferenceEquals(value, Me.m_diagram) Then Return
            Me.m_diagram = value
            Me.m_plFlow.Init(Me.m_data, Me.m_diagram, Me.m_pgDetails)
        End Set
    End Property

    Public Property Data() As cData
        Get
            Return Me.m_data
        End Get
        Set(ByVal value As cData)
            Me.m_data = value
        End Set
    End Property

#End Region ' Internals

End Class
