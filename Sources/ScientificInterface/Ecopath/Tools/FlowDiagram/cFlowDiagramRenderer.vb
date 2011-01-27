#Region " Imports "

Option Strict On

Imports System.Math
Imports EwEUtils.Win32Api
Imports SAUPUtil.SAUPData.Mapping
Imports SAUPUtil.Misc.Colours

#End Region ' Imports

Namespace Ecopath.Controls.FlowDiagram

    Public Class cFlowDiagramRenderer

        Private m_iHighlight As Integer = 0
        Private m_bIsMouseDown As Boolean = False
        Private m_tree As cFlowDiagramTree = Nothing
        Private m_data As cFlowDiagramData = Nothing

#Region " Constructor "

        Public Sub New(ByVal data As cFlowDiagramData, _
                       ByVal tree As cFlowDiagramTree)

            Me.m_data = data
            Me.m_tree = tree

        End Sub

#End Region ' Constructor

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the node to highlight.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Property HighlightNode() As Integer
            Get
                Return Me.m_iHighlight
            End Get
            Set(ByVal value As Integer)
                'If Not Me.m_bIsMouseDown Then
                If (Me.m_iHighlight <> value) Then
                    Me.m_iHighlight = value
                End If
                'End If
            End Set
        End Property

        Public Sub DrawFlowDiagram(ByVal g As Graphics, ByVal rc As Rectangle)

            ' Draw the objects
            Me.m_tree.DrawBackground(g, rc)

            ' Draw the connections
            For iPred As Integer = 1 To Me.m_data.NumLivingGroups()
                For iPrey As Integer = 1 To Me.m_data.NumGroups()
                    If Me.m_data.GroupVisible(iPred) And Me.m_data.GroupVisible(iPrey) Then
                        Me.m_tree.DrawConnection(g, rc, iPred, iPrey, Me.HighlightNode = iPred, Me.HighlightNode = iPrey)
                    End If
                Next
            Next

            ' Draw the nodes
            For j As Integer = 1 To Me.m_data.NumGroups()
                ' Draw each node
                'clr = colorramp.GetColor(Me.m_data.Biomass(j), sBiomassMax)
                If Me.m_data.GroupVisible(j) Then
                    Me.m_tree.DrawNode(g, rc, j)
                End If
            Next j

        End Sub

        Public Sub ProcessMouseMove(ByVal g As Graphics, ByVal rc As Rectangle, ByVal pt As PointF)

            Dim iNode As Integer = 0
            Dim ft As Font = Me.m_data.UIContext.StyleGuide.Font(cStyleGuide.eApplicationFontType.SubTitle)

            ' Dragging?
            Select Case Me.m_dragMode

                Case eDragMode.None

                    ' Not dragging: determine which node to highlight
                    Me.HighlightNode = 0

                    iNode = Me.GetLabelAtPoint(rc, pt, g, ft)
                    If iNode > 0 Then
                        Me.HighlightNode = iNode
                    Else
                        iNode = Me.GetNodeAtPoint(rc, pt)
                        If iNode > 0 Then
                            Me.HighlightNode = iNode
                        End If
                    End If

                Case eDragMode.Label
                    Me.m_tree.MoveLabel(rc, pt, Me.HighlightNode)

                Case eDragMode.Node
                    Me.m_tree.MoveNode(rc, pt, Me.HighlightNode)

            End Select

        End Sub

        Private Function GetNodeAtPoint(ByVal rc As Rectangle, ByVal pt As PointF) As Integer

            Dim iGroup As Integer = 1
            Dim iNodeAtPoint As Integer = 0

            While (iGroup <= Me.m_data.NumGroups) And (iNodeAtPoint = 0)
                If Me.m_tree.IsNodeAtPoint(rc, pt, iGroup, Me.m_data.Biomass(iGroup)) Then
                    iNodeAtPoint = iGroup
                End If
                iGroup += 1
            End While

            Return iNodeAtPoint
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="rc"></param>
        ''' <param name="pt"></param>
        ''' <param name="g">Graphics to measure label dimensions with.</param>
        ''' <param name="font">Font to measure label dimension with.</param>
        ''' <returns></returns>
        Private Function GetLabelAtPoint(ByVal rc As Rectangle, _
                                         ByVal pt As PointF, _
                                         ByVal g As Graphics, _
                                         ByVal font As Font) As Integer

            Dim iGroup As Integer = 1
            Dim iLabelAtPoint As Integer = 0

            While (iGroup <= Me.m_data.NumGroups) And (iLabelAtPoint = 0)
                If Me.m_tree.IsLabelAtPoint(rc, pt, iGroup, Me.m_data.GroupName(iGroup), g, font) Then
                    iLabelAtPoint = iGroup
                End If
                iGroup += 1
            End While

            Return iLabelAtPoint
        End Function

        Public Sub SaveToFile(ByVal inifile As cINIFile, ByVal rc As Rectangle)
            inifile.WriteInteger("Global", "NumGroups", Me.m_data.NumGroups)
            For i As Integer = 1 To Me.m_data.NumGroups
                inifile.WriteInteger("Locations", i.ToString + "x", CInt(Me.m_tree.NodeLocation(i, rc).X))
                inifile.WriteInteger("Locations", i.ToString + "y", CInt(Me.m_tree.NodeLocation(i, rc).Y))
                inifile.WriteInteger("Locations", i.ToString + "xlabel", CInt(Me.m_tree.LabelLocation(i, rc).X))
                inifile.WriteInteger("Locations", i.ToString + "ylabel", CInt(Me.m_tree.LabelLocation(i, rc).Y))
            Next i
        End Sub

        Public Function LoadFromFile(ByVal inifile As cINIFile, ByVal rc As Rectangle) As Boolean
            Dim ptf As PointF
            If Me.m_data.NumGroups = inifile.GetInteger("Global", "NumGroups", 0) Then
                For i As Integer = 1 To Me.m_data.NumGroups
                    ptf.X = inifile.GetInteger("Locations", i.ToString + "x", 0)
                    ptf.Y = inifile.GetInteger("Locations", i.ToString + "y", 0)
                    Me.m_tree.NodeLocation(i, rc) = ptf
                    ptf.X = inifile.GetInteger("Locations", i.ToString + "xlabel", 10)
                    ptf.Y = inifile.GetInteger("Locations", i.ToString + "ylabel", 10)
                    Me.m_tree.LabelLocation(i, rc) = ptf
                Next i
                Return True
            Else
                Return False
            End If

        End Function

#Region " Dragging "

        Private Enum eDragMode As Integer
            None
            Label
            Node
        End Enum

        Private m_dragMode As eDragMode = eDragMode.None
        Private m_ptDragOffset As PointF = Nothing

        Public Sub BeginDrag(ByVal rc As Rectangle, ByVal pt As PointF, ByVal g As Graphics)

            If Me.IsDragging Then Return

            ' Find the node under the cursor
            Dim iLabel As Integer = 0
            Dim iNode As Integer = 0
            Dim ft As Font = Me.m_data.RenderFont

            Me.HighlightNode = 0
            Me.m_ptDragOffset = pt

            iLabel = Me.GetLabelAtPoint(rc, pt, g, ft)
            If iLabel > 0 Then
                Me.HighlightNode = iLabel
                Me.m_dragMode = eDragMode.Label
            Else
                iNode = Me.GetNodeAtPoint(rc, pt)
                If iNode > 0 Then
                    Me.HighlightNode = iNode
                    Me.m_dragMode = eDragMode.Node
                End If
            End If

        End Sub

        Public Sub EndDrag(ByVal fdData As cFlowDiagramData, ByVal pt As PointF)
            Me.m_dragMode = eDragMode.None
        End Sub

        Public Function IsDragging() As Boolean
            Return (Me.m_dragMode <> eDragMode.None)
        End Function

#End Region ' Dragging

    End Class

End Namespace
