' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports System.Math
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class that renderes a <see cref="IFlowDiagramData">flow diagram</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cFlowDiagramManager

#Region " Private vars "

        Private m_iHighlight As Integer = 0
        Private m_bIsMouseDown As Boolean = False
        Private m_tree As IFlowDiagramRenderer = Nothing
        Private m_data As IFlowDiagramData = Nothing

        Private Enum eDragMode As Integer
            None
            Label
            Node
        End Enum

        Public Enum eColorUsageTypes As Integer
            None
            EwE
            Value
            Flow
        End Enum

        Public Enum eHighlightType As Integer
            None
            Hidden
            Selected
            LinkIn
            LinkOut
        End Enum

        Private m_dragMode As eDragMode = eDragMode.None
        Private m_ptDragOffset As PointF = Nothing

#End Region ' Private vars

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor for a flow diagram renderer.
        ''' </summary>
        ''' <param name="data">The <see cref="IFlowDiagramData">data</see> for the flow diagram.</param>
        ''' <param name="tree">The <see cref="IFlowDiagramRenderer"/> tree to do 
        ''' the actual rendering and UI interactions.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal data As IFlowDiagramData, _
                       ByVal tree As IFlowDiagramRenderer)

            Me.m_data = data
            Me.m_tree = tree

        End Sub

#End Region ' Constructor

#Region " Configuration "

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

#End Region ' Configuration

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Master draw instruction. There can be only one.
        ''' </summary>
        ''' <param name="g">Graphics to draw onto.</param>
        ''' <param name="rc">Rectangle to draw within.</param>
        ''' -------------------------------------------------------------------
        Public Sub DrawFlowDiagram(ByVal g As Graphics, ByVal rc As Rectangle)

            Dim hl As eHighlightType = eHighlightType.None

            Me.m_tree.DrawBackground(g, rc)
            Me.m_tree.DrawTitle(g, rc)
            Me.m_tree.DrawLegend(g, Me.m_data.ValueMax, New Point(5, 5), Me.m_data.Title)

            ' Draw the connections
            For iPred As Integer = 1 To Me.m_data.NumLivingGroups()
                For iPrey As Integer = 1 To Me.m_data.NumGroups()
                    ' Determine highlight state
                    hl = eHighlightType.None
                    If Me.m_data.IsGroupVisible(iPred) And Me.m_data.IsGroupVisible(iPrey) Then
                        If (Me.HighlightNode = iPred) Then hl = eHighlightType.LinkIn
                        If (Me.HighlightNode = iPrey) Then hl = eHighlightType.LinkOut
                    Else
                        hl = eHighlightType.Hidden
                    End If
                    Me.m_tree.DrawConnection(g, rc, iPred, iPrey, hl)
                Next
            Next

            ' Draw the nodes
            For j As Integer = 1 To Me.m_data.NumGroups()

                ' Determine node highlight state
                hl = eHighlightType.None
                If Not Me.m_data.IsGroupVisible(j) Then hl = eHighlightType.Hidden
                If (Me.HighlightNode = j) Then hl = eHighlightType.Selected

                ' ToDo: check if node is a prey or pred of the selected node

                ' Draw each node
                Me.m_tree.DrawNode(g, rc, j, hl)
            Next j

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Handle a mouse move operation.
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rc"></param>
        ''' <param name="pt"></param>
        ''' -------------------------------------------------------------------
        Public Sub ProcessMouseMove(ByVal g As Graphics, ByVal rc As Rectangle, ByVal pt As PointF)

            Dim iNode As Integer = 0

            ' Dragging?
            Select Case Me.m_dragMode

                Case eDragMode.None

                    ' Not dragging: determine which node to highlight
                    Me.HighlightNode = 0

                    Using ft As Font = Me.m_tree.RenderFont
                        iNode = Me.GetLabelAtPoint(rc, pt, g, ft)
                    End Using

                    If iNode > 0 Then
                        Me.HighlightNode = iNode
                    Else
                        iNode = Me.GetNodeAtPoint(rc, pt)
                        If iNode > 0 Then
                            Me.HighlightNode = iNode
                        End If
                    End If

                Case eDragMode.Label
                    Me.m_tree.MoveLabel(rc, New PointF(pt.X - Me.m_ptDragOffset.X, pt.Y - Me.m_ptDragOffset.Y), Me.HighlightNode)

                Case eDragMode.Node
                    Me.m_tree.MoveNode(rc, New PointF(pt.X - Me.m_ptDragOffset.X, pt.Y - Me.m_ptDragOffset.Y), Me.HighlightNode)

            End Select

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the diagram layout to a file.
        ''' </summary>
        ''' <param name="inifile">The <see cref="cXMLINIfile">file</see> to save to.</param>
        ''' <param name="rc">The rectangle to scale the diagram to.</param>
        ''' -------------------------------------------------------------------
        Public Function SaveToFile(ByVal inifile As cXMLINIfile, ByVal rc As Rectangle) As Boolean

            Try

                inifile.SaveSetting("Global", "NumGroups", Me.m_data.NumGroups)
                For i As Integer = 1 To Me.m_data.NumGroups
                    inifile.SaveSetting("Locations", i.ToString + "x", CStr(Me.m_tree.NodeLocation(i, rc).X))
                    inifile.SaveSetting("Locations", i.ToString + "y", CStr(Me.m_tree.NodeLocation(i, rc).Y))
                    inifile.SaveSetting("Locations", i.ToString + "xlabel", CStr(Me.m_tree.LabelLocation(i, rc).X))
                    inifile.SaveSetting("Locations", i.ToString + "ylabel", CStr(Me.m_tree.LabelLocation(i, rc).Y))
                Next i
                inifile.Flush()

            Catch ex As Exception
                ' ToDo: send an error message
                cLog.Write(ex, "FlowDiagram.SaveToFile")
                Return False
            End Try
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load the diagram layout from a file.
        ''' </summary>
        ''' <param name="inifile">The <see cref="cXMLINIfile">file</see> to load from.</param>
        ''' <param name="rc">The rectangle to scale the diagram to.</param>
        ''' -------------------------------------------------------------------
        Public Function LoadFromFile(ByVal inifile As cXMLINIfile, ByVal rc As Rectangle) As Boolean

            Try

                Dim ptf As PointF
                Dim iNumGroups As Integer = Math.Min(CInt(inifile.GetSetting("Global", "NumGroups", "0")), Me.m_data.NumGroups)
                For i As Integer = 1 To iNumGroups
                    ptf.X = CInt(inifile.GetSetting("Locations", i.ToString + "x", "0"))
                    ptf.Y = CInt(inifile.GetSetting("Locations", i.ToString + "y", "0"))
                    Me.m_tree.NodeLocation(i, rc) = ptf
                    ptf.X = CInt(inifile.GetSetting("Locations", i.ToString + "xlabel", "10"))
                    ptf.Y = CInt(inifile.GetSetting("Locations", i.ToString + "ylabel", "10"))
                    Me.m_tree.LabelLocation(i, rc) = ptf
                Next i

            Catch ex As Exception
                ' ToDo: send an error message
                cLog.Write(ex, "FlowDiagram.SaveToFile")
                Return False
            End Try
            Return True

        End Function

#End Region ' Public access

#Region " Dragging "

        Public Sub BeginDrag(ByVal rc As Rectangle, ByVal pt As PointF, ByVal g As Graphics)

            If Me.IsDragging Then Return

            ' Find the node under the cursor
            Dim iNode As Integer = 0
            Dim ptItem As PointF

            Me.HighlightNode = 0

            Using ft As Font = Me.m_tree.RenderFont
                iNode = Me.GetLabelAtPoint(rc, pt, g, ft)
            End Using

            If iNode > 0 Then
                Me.HighlightNode = iNode
                Me.m_dragMode = eDragMode.Label
                ptItem = Me.m_tree.LabelLocation(iNode, rc)
                Me.m_ptDragOffset = New PointF(pt.X - ptItem.X, pt.Y - ptItem.Y)
            Else
                iNode = Me.GetNodeAtPoint(rc, pt)
                If iNode > 0 Then
                    Me.HighlightNode = iNode
                    Me.m_dragMode = eDragMode.Node
                    ptItem = Me.m_tree.NodeLocation(iNode, rc)
                    Me.m_ptDragOffset = New PointF(pt.X - ptItem.X, pt.Y - ptItem.Y)
                End If
            End If

        End Sub

        Public Sub EndDrag(ByVal fdData As IFlowDiagramData, ByVal pt As PointF)
            Me.m_dragMode = eDragMode.None
        End Sub

        Public Function IsDragging() As Boolean
            Return (Me.m_dragMode <> eDragMode.None)
        End Function

#End Region ' Dragging

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a node at a location.
        ''' </summary>
        ''' <param name="rc">Flow diagram area to find the node within.</param>
        ''' <param name="pt">Point to test for.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function GetNodeAtPoint(ByVal rc As Rectangle, ByVal pt As PointF) As Integer

            Dim iGroup As Integer = 1
            Dim iNodeAtPoint As Integer = 0

            While (iGroup <= Me.m_data.NumGroups) And (iNodeAtPoint = 0)
                If Me.m_tree.IsNodeAtPoint(rc, pt, iGroup, Me.m_data.Value(iGroup)) Then
                    iNodeAtPoint = iGroup
                End If
                iGroup += 1
            End While

            Return iNodeAtPoint
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a label at a location.
        ''' </summary>
        ''' <param name="rc">Flow diagram area to find the label within.</param>
        ''' <param name="pt">Point to test for.</param>
        ''' <param name="g">Graphics to measure label dimensions with.</param>
        ''' <param name="font">Font to measure label dimension with.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function GetLabelAtPoint(ByVal rc As Rectangle, _
                                         ByVal pt As PointF, _
                                         ByVal g As Graphics, _
                                         ByVal font As Font) As Integer

            Dim iGroup As Integer = 1
            Dim iLabelAtPoint As Integer = 0

            While (iGroup <= Me.m_data.NumGroups) And (iLabelAtPoint = 0)
                If Me.m_tree.IsLabelAtPoint(rc, pt, iGroup, Me.m_tree.FormatLabelText(iGroup), g, font) Then
                    iLabelAtPoint = iGroup
                End If
                iGroup += 1
            End While

            Return iLabelAtPoint
        End Function

#End Region ' Internals

    End Class

End Namespace
