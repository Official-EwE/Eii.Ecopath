'==============================================================================
'
' $Log: frmTFMpolicy.vb,v $
' Revision 1.3  2008/10/08 22:14:16  jeroens
' Drag w SHIFT
'
' Revision 1.2  2008/10/08 21:18:24  jeroens
' Globalized
'
' Revision 1.1  2008/10/08 17:57:35  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports System.Windows.Forms
Imports ZedGraph

#End Region ' Imports

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' =======================================================================
    Public Class frmTargetFishingMortalityPolicy

#Region " Internals "

        Private Enum eDragType As Integer
            None = 0
            BLim
            BBaseFopt
            Fopt
        End Enum

        ''' <summary><see cref="ZedGraphHelper">Helper</see> to manipulate the graph.</summary>
        Private m_zgh As ZedGraphHelper = Nothing
        ''' <summary>Group selected in the form.</summary>
        Private m_group As cEcoSimGroupInput = Nothing
        ''' <summary>Graph drag mode.</summary>
        Private m_dragtype As eDragType = eDragType.None

#End Region ' Internals

#Region " Events "

        Private Sub HandleLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim core As cCore = cCore.GetInstance()

            Me.m_zgh = New ZedGraphHelper(Me.m_graph, 1)
            Me.m_zgh.ConfigurePane("", My.Resources.HEADER_BIOMASS, My.Resources.HEADER_TFM, True)

            Me.m_zgh.AllowZoom = False
            Me.m_zgh.AllowPan = False
            Me.m_zgh.AllowEdit = True

            ' Hahaha
            If (core.nGroups > 0) Then
                Me.m_grid.Group = cCore.GetInstance().EcoSimGroupInputs(1)
            End If

        End Sub

        Private Sub HandleFormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
            ' Clean up
            Me.Group = Nothing
            Me.m_zgh = Nothing
        End Sub

        Private Sub HandleGridSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) _
                Handles m_grid.OnSelectionChanged
            ' Update group selection according to user actions in the grid
            Me.Group = Me.m_grid.Group
        End Sub

        Private Sub HandlePropertyChanged(ByVal prop As cProperty, ByVal cf As cProperty.eChangeFlags)
            ' A relevant property has changed: redraw the graph
            Me.Redraw()
        End Sub

#End Region ' Events

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group in the form
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property Group() As cEcoSimGroupInput
            Get
                Return Me.m_group
            End Get
            Set(ByVal value As cEcoSimGroupInput)

                Dim pm As cPropertyManager = cPropertyManager.GetInstance()

                ' Unregister
                If (Me.m_group IsNot Nothing) Then
                    RemoveHandler pm.GetProperty(Me.m_group, eVarNameFlags.BLim).PropertyChanged, AddressOf HandlePropertyChanged
                    RemoveHandler pm.GetProperty(Me.m_group, eVarNameFlags.BBase).PropertyChanged, AddressOf HandlePropertyChanged
                    RemoveHandler pm.GetProperty(Me.m_group, eVarNameFlags.Fopt).PropertyChanged, AddressOf HandlePropertyChanged
                End If

                ' Update
                Me.m_group = value

                ' Register
                If (Me.m_group IsNot Nothing) Then
                    AddHandler pm.GetProperty(Me.m_group, eVarNameFlags.BLim).PropertyChanged, AddressOf HandlePropertyChanged
                    AddHandler pm.GetProperty(Me.m_group, eVarNameFlags.BBase).PropertyChanged, AddressOf HandlePropertyChanged
                    AddHandler pm.GetProperty(Me.m_group, eVarNameFlags.Fopt).PropertyChanged, AddressOf HandlePropertyChanged
                End If

                ' Ledlaw the glaph
                Me.Redraw()

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Redraw the quota curve.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Redraw()

            If Me.m_zgh Is Nothing Then Return

            Dim lpts As New PointPairList
            Dim line As LineItem = Nothing
            Dim lLines As New List(Of LineItem)

            If (Me.m_group IsNot Nothing) Then

                ' Add points
                lpts.Add(0, 0)
                lpts.Add(Me.m_group.BLim, 0)
                lpts.Add(Me.m_group.BBase, Me.m_group.FOpt) ' Point order?
                lpts.Add(Me.m_group.BBase * 4, Me.m_group.FOpt) ' Max X value?

                ' Add text items to the points
                'text = new TextObj("Upgrade", 700F, 50.0F );
                '// rotate the text 90 degrees
                'text.FontSpec.Angle = 90;
                '// Align the text such that the Right-Center is at (700, 50) in user scale coordinates
                'text.Location.AlignH = AlignH.Right;
                'text.Location.AlignV = AlignV.Center;
                '// Disable the border and background fill options for the text
                'text.FontSpec.Fill.IsVisible = false;
                'text.FontSpec.Border.IsVisible = false;
                'myPane.GraphObjList.Add( text );

                line = New LineItem(Me.m_group.Name, lpts, Color.DarkOrange, SymbolType.Circle)
                line.Line.Width = 2.0

                lLines.Add(line)
                ' Plot graph, but rescale ONLY when not dragging
                Me.m_zgh.PlotLines(lLines, 1, (Me.m_dragtype = eDragType.None))
            Else
                Me.m_zgh.PlotLines(Nothing)
            End If

        End Sub

#End Region ' Internals

#Region " Dragging "

        Private Sub HandleGraphKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) _
                Handles m_graph.KeyDown

            If Control.ModifierKeys = Keys.Shift Then
                Me.m_graph.Cursor = Cursors.Hand
            End If

        End Sub

        Private Sub HandleGraphKeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) _
                Handles m_graph.KeyUp

            If Control.ModifierKeys = Keys.Shift Then
                Me.m_graph.Cursor = Cursors.Default
            End If

        End Sub

        Private Function HandleGraphMouseDownEvent(ByVal sender As ZedGraphControl, ByVal e As MouseEventArgs) As Boolean _
                Handles m_graph.MouseDownEvent

            Dim pane As GraphPane = sender.GraphPane
            Dim pt As PointF = New PointF(e.X, e.Y)
            Dim curve As CurveItem = Nothing
            Dim iIndex As Integer = 0

            ' Point-dragging is activated by an 'Shift' key and mousedown combination
            If (Control.ModifierKeys = Keys.Shift) Then

                ' Find the point that was clicked, and make sure the point list is editable
                If (pane.FindNearestPoint(pt, curve, iIndex) And (TypeOf curve.Points Is PointPairList)) Then
                    ' Set drag operation type
                    Me.m_dragtype = DirectCast(iIndex, eDragType)
                End If
            End If

            Return (Me.m_dragtype <> eDragType.None)

        End Function

        Private Function HandleGraphMouseMoveEvent(ByVal sender As ZedGraphControl, ByVal e As MouseEventArgs) As Boolean _
                Handles m_graph.MouseMoveEvent

            Dim pane As GraphPane = sender.GraphPane
            Dim pt As PointF = New PointF(e.X, e.Y)
            Dim dX As Double = 0.0
            Dim dy As Double = 0.0

            ' Dragging?
            If (Me.m_dragtype <> eDragType.None) Then
                ' Translate value
                pane.ReverseTransform(pt, dX, dy)

                Select Case Me.m_dragtype
                    Case eDragType.BLim
                        Me.m_group.BLim = Math.Max(0, Math.Min(CSng(dX), Me.m_group.BBase))
                    Case eDragType.BBaseFopt
                        Me.m_group.BBase = Math.Max(Me.m_group.BLim, CSng(dX))
                        Me.m_group.FOpt = Math.Max(0, CSng(dy))
                    Case eDragType.Fopt
                        Me.m_group.FOpt = Math.Max(0, CSng(dy))
                End Select

            End If
        End Function

        Private Function HandleGraphMouseUpEvent(ByVal sender As ZedGraphControl, ByVal e As MouseEventArgs) As Boolean _
                Handles m_graph.MouseUpEvent

            Me.m_dragtype = eDragType.None
            Me.m_zgh.RescaleAndRedraw()

        End Function

#End Region ' Dragging

    End Class

End Namespace
