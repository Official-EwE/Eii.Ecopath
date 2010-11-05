#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph
Imports ScientificInterfaceShared.Controls
Imports EwECore
Imports System.Drawing

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Content manager-derived class, able to plot keystoneness-related values.
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class cKeystonenessGraph
    Inherits cContentManager

#Region " Private defs "

    ''' <summary>Max size for scaled symbols</summary>
    Private Const iMAX_SYMBOL_SIZE As Integer = 100

    ''' <summary>Graph representation styles.</summary>
    Private Enum eRepresentationType As Byte
        ''' <summary>Items reflected by black uni-sized circles.</summary>
        Circle
        ''' <summary>Items reflected by coloured circles scaled by biomass.</summary>
        CircleScaled
        ''' <summary>Items reflected by group index.</summary>
        Number
    End Enum

    ''' <summary>Graph content styles.</summary>
    Private Enum eContentType As Byte
        Keystoneness
        TotalEffectOverB
        KeystoneIndex2
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class; sorts line items by symbol sizes, smallest symbols on top
    ''' to make sure that large symbols do not obscure smaller symbols.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cSymbolSizeSorter
        Implements IComparer(Of ZedGraph.CurveItem)

        Public Function Compare(ByVal x As ZedGraph.CurveItem, ByVal y As ZedGraph.CurveItem) As Integer _
            Implements System.Collections.Generic.IComparer(Of ZedGraph.CurveItem).Compare
            If TypeOf (x) Is LineItem And TypeOf (y) Is LineItem Then
                Dim liX As LineItem = DirectCast(x, LineItem)
                Dim liY As LineItem = DirectCast(y, LineItem)
                If liX.Symbol.Size < liY.Symbol.Size Then Return -1
                If liX.Symbol.Size > liY.Symbol.Size Then Return 1
            End If
            Return 0
        End Function
    End Class

#End Region ' Private defs

#Region " Private vars "

    ''' <summary>Helper class molesting the graph.</summary>
    Private m_zgh As cZedGraphHelper = Nothing
    ''' <summary>Flag stating what data to plot.</summary>
    Private m_content As eContentType = eContentType.Keystoneness
    ''' <summary>Flag stating how data should be plot.</summary>
    Private m_representation As eRepresentationType = eRepresentationType.Circle

    ''' <summary>Custom toolstrip item</summary>
    Private m_tsStyle As ToolStripDropDownButton = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiCircles As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiCirclesScaled As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiNumbers As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsContent As ToolStripDropDownButton = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiKeyst As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiTotImpactOverB As ToolStripMenuItem = Nothing
    ''' <summary>Custom toolstrip item</summary>
    Private m_tsmiKeyst2 As ToolStripMenuItem = Nothing

#End Region ' Private vars

    Public Sub New()
        '
    End Sub

    Public Overrides Function PageTitle() As String
        Return "Keystoneness"
    End Function

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip, _
                                     ByVal uic As cUIContext) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip, uic)

        Me.Graph.Visible = bSucces
        Me.Toolstrip.Visible = bSucces
        Me.ToolstripShowOptionCSV()
        Me.AddToolstripItems()

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(uic, Me.Graph, 1)
        Me.m_zgh.ShowPointValue = True

        Return bSucces

    End Function

    Public Overrides Sub Detach()

        Me.RemoveToolstripItems()

        Me.m_zgh.Detach()
        Me.m_zgh = Nothing

        MyBase.Detach()

    End Sub

    Public Overrides Sub DisplayData()

        Dim pane As GraphPane = Nothing
        Dim li As LineItem = Nothing
        Dim curve As CurveItem = Nothing
        Dim ppl As PointPairList = Nothing
        Dim txt As ZedGraph.TextObj = Nothing
        Dim group As cCoreInputOutputBase = Nothing
        Dim sMaxB As Single = 0.0

        ' UpdateControls will take care of axis labels
        pane = Me.m_zgh.ConfigurePane("", "", "", False)
        ' All X-axis values are relative to 1
        pane.XAxis.Scale.Max = 1.0

        ' Clear curves
        pane.CurveList.Clear()
        ' Clear text objects and other misc objects that may have been added
        pane.GraphObjList.Clear()

        ' Precalc max Biomass (for CircleScaled style)
        For iGroup As Integer = 1 To Me.NetworkManager.nLivingGroups
            sMaxB = Math.Max(sMaxB, Me.NetworkManager.BiomassByGroup(iGroup))
        Next
        ' Avoid division by zero
        If sMaxB = 0 Then sMaxB = 1.0

        ' Create a unique line item for each group
        For iGroup As Integer = 1 To Me.NetworkManager.nLivingGroups

            ' Build line item
            ppl = New PointPairList()
            Select Case Me.Content

                Case eContentType.Keystoneness
                    ppl.Add(Me.NetworkManager.RelativeTotalImpact(iGroup), Me.NetworkManager.KeystoneIndex(iGroup))

                Case eContentType.TotalEffectOverB
                    ppl.Add(Me.NetworkManager.BiomassByGroup(iGroup) / sMaxB, Me.NetworkManager.RelativeTotalImpact(iGroup))

                Case eContentType.KeystoneIndex2
                    ppl.Add(Me.NetworkManager.RelativeTotalImpact(iGroup), Me.NetworkManager.TotalImpactOverBiomass(iGroup))

            End Select

            ' Get actual group
            group = Me.NetworkManager.Core.EcoPathGroupInputs(iGroup)

            ' Make things look purdy
            Select Case Me.m_representation

                Case eRepresentationType.Circle

                    ' Render values as uni-sized black circles
                    li = New LineItem(group.Name, ppl, Color.Black, SymbolType.Circle)
                    li.Line.Color = Color.Transparent
                    pane.CurveList.Add(li)

                Case eRepresentationType.CircleScaled

                    ' Render values as group-coloured circles, scaled to biomass
                    li = New LineItem(group.Name, ppl, Color.Black, SymbolType.Circle)
                    li.Line.Color = Color.Transparent
                    li.Symbol.Size = CSng(iMAX_SYMBOL_SIZE * Math.Sqrt(Me.NetworkManager.BiomassByGroup(iGroup) / sMaxB))
                    li.Symbol.Fill = New Fill(Me.StyleGuide.GroupColor(Me.NetworkManager.Core, group.Index))
                    pane.CurveList.Add(li)

                Case eRepresentationType.Number

                    ' Render values as black texts reflecting numeric group indices

                    ' Add hidden line for mouse value tracking
                    li = New LineItem(group.Name, ppl, Color.Transparent, SymbolType.None)
                    pane.CurveList.Add(li)

                    ' Add text label
                    txt = New ZedGraph.TextObj(CStr(group.Index), ppl(0).X, ppl(0).Y)
                    txt.ZOrder = ZOrder.A_InFront
                    With txt.FontSpec
                        .Fill.IsVisible = False
                        .Border.IsVisible = False
                        .FontColor = Color.Black
                    End With
                    pane.GraphObjList.Add(txt)

            End Select

        Next iGroup

        ' Sort curve list by symbol size, if applicable
        pane.CurveList.Sort(New cSymbolSizeSorter())

        ' Yo!
        Me.m_zgh.RescaleAndRedraw()
        Me.UpdateControls()

    End Sub

#Region " Internals "

    Private Property Representation() As eRepresentationType
        Get
            Return Me.m_representation
        End Get
        Set(ByVal representation As eRepresentationType)
            If (Me.m_representation <> representation) Then
                Me.m_representation = representation
                Me.DisplayData()
            End If
        End Set
    End Property

    Private Property Content() As eContentType
        Get
            Return Me.m_content
        End Get
        Set(ByVal content As eContentType)
            If (Me.m_content <> content) Then
                Me.m_content = content
                Me.DisplayData()
            End If
        End Set
    End Property

    Private Sub AddToolstripItems()

        Me.m_tsmiCircles = New ToolStripMenuItem(My.Resources.MNU_STYLE_CIRCLES)
        AddHandler Me.m_tsmiCircles.Click, AddressOf OnStyleCircles

        Me.m_tsmiCirclesScaled = New ToolStripMenuItem(My.Resources.MNU_STYLE_CIRCLES_SCALED)
        AddHandler Me.m_tsmiCirclesScaled.Click, AddressOf OnStyleCirclesScaled

        Me.m_tsmiNumbers = New ToolStripMenuItem(My.Resources.MNU_STYLE_NUMBERS)
        AddHandler Me.m_tsmiNumbers.Click, AddressOf OnStyleNumbers

        Me.m_tsStyle = New ToolStripDropDownButton(My.Resources.MNU_STYLE)
        Me.m_tsStyle.DropDownItems.Add(Me.m_tsmiCircles)
        Me.m_tsStyle.DropDownItems.Add(Me.m_tsmiCirclesScaled)
        Me.m_tsStyle.DropDownItems.Add(Me.m_tsmiNumbers)
        Me.Toolstrip.Items.Add(Me.m_tsStyle)

        Me.m_tsmiKeyst = New ToolStripMenuItem(My.Resources.MNU_CONTENT_KEYSTONE)
        AddHandler Me.m_tsmiKeyst.Click, AddressOf OnContentK

        Me.m_tsmiKeyst2 = New ToolStripMenuItem(My.Resources.MNU_CONTENT_KEYSTONE2)
        AddHandler Me.m_tsmiKeyst2.Click, AddressOf OnContentTIoverB

        Me.m_tsmiTotImpactOverB = New ToolStripMenuItem(My.Resources.MNU_CONTENT_TOTIMPACT_OVER_B)
        AddHandler Me.m_tsmiTotImpactOverB.Click, AddressOf OnContentTI

        Me.m_tsContent = New ToolStripDropDownButton(My.Resources.MNU_CONTENT)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiKeyst)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiTotImpactOverB)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiKeyst2)
        Me.Toolstrip.Items.Add(Me.m_tsContent)

    End Sub

    Private Sub RemoveToolstripItems()

        Me.Toolstrip.Items.Remove(Me.m_tsStyle)

        Me.m_tsStyle.DropDownItems.Clear()
        RemoveHandler Me.m_tsmiCircles.Click, AddressOf OnStyleCircles
        Me.m_tsmiCircles = Nothing
        RemoveHandler Me.m_tsmiCirclesScaled.Click, AddressOf OnStyleCirclesScaled
        Me.m_tsmiCirclesScaled = Nothing
        RemoveHandler Me.m_tsmiNumbers.Click, AddressOf OnStyleNumbers
        Me.m_tsmiNumbers = Nothing
        Me.m_tsStyle = Nothing

        Me.Toolstrip.Items.Remove(Me.m_tsContent)

        Me.m_tsContent.DropDownItems.Clear()
        RemoveHandler Me.m_tsmiKeyst.Click, AddressOf OnContentK
        Me.m_tsmiKeyst = Nothing
        RemoveHandler Me.m_tsmiTotImpactOverB.Click, AddressOf OnContentTI
        Me.m_tsmiTotImpactOverB = Nothing
        RemoveHandler Me.m_tsmiKeyst2.Click, AddressOf OnContentTIoverB
        Me.m_tsmiKeyst2 = Nothing
        Me.m_tsContent = Nothing

    End Sub

    Private Sub OnStyleCircles(ByVal sender As Object, ByVal arg As EventArgs)
        Me.Representation = eRepresentationType.Circle
    End Sub

    Private Sub OnStyleCirclesScaled(ByVal sender As Object, ByVal arg As EventArgs)
        Me.Representation = eRepresentationType.CircleScaled
    End Sub

    Private Sub OnStyleNumbers(ByVal sender As Object, ByVal arg As EventArgs)
        Me.Representation = eRepresentationType.Number
    End Sub

    Private Sub OnContentK(ByVal sender As Object, ByVal arg As EventArgs)
        Me.Content = eContentType.Keystoneness
    End Sub

    Private Sub OnContentTI(ByVal sender As Object, ByVal arg As EventArgs)
        Me.Content = eContentType.TotalEffectOverB
    End Sub

    Private Sub OnContentTIoverB(ByVal sender As Object, ByVal arg As EventArgs)
        Me.Content = eContentType.KeystoneIndex2
    End Sub

    Private Sub UpdateControls()

        Me.m_tsmiCircles.Checked = (Me.Representation = eRepresentationType.Circle)
        Me.m_tsmiCirclesScaled.Checked = (Me.Representation = eRepresentationType.CircleScaled)
        Me.m_tsmiNumbers.Checked = (Me.Representation = eRepresentationType.Number)

        Me.m_tsmiKeyst.Checked = (Me.Content = eContentType.Keystoneness)
        Me.m_tsmiTotImpactOverB.Checked = (Me.Content = eContentType.TotalEffectOverB)
        Me.m_tsmiKeyst2.Checked = (Me.Content = eContentType.KeystoneIndex2)

        Select Case Me.Content

            Case eContentType.Keystoneness
                Me.m_zgh.ConfigurePane("", My.Resources.LBL_RELTOTALIMPACT, My.Resources.LBL_KEYSTONENESS, False)

            Case eContentType.TotalEffectOverB
                Me.m_zgh.ConfigurePane("", "Relative biomass", My.Resources.LBL_RELTOTALIMPACT, False)

            Case eContentType.KeystoneIndex2
                Me.m_zgh.ConfigurePane("", My.Resources.LBL_RELTOTALIMPACT, My.Resources.LBL_KEYSTONE2, False)

        End Select

    End Sub

#End Region ' Internals

End Class
