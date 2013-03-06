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
Imports System.ComponentModel
Imports System.Text
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, implements building of a flow diagram tree.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cFlowDiagramTree

#Region " Helper classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cFlowDiagramNode

            Enum eNodeTypes As Integer
                Circle = 1
                Rectangle = 2
            End Enum

            '''--------------------------------------------------------------------
            ''' <summary>
            ''' Draw a flow diagram node.
            ''' </summary>
            ''' <param name="g">Graphics to render the node onto.</param>
            ''' <param name="ptf">Point of the node center.</param>
            ''' <param name="nodetype"><see cref="eNodeTypes">Node render type</see>.</param>
            ''' <param name="iSize">Node render size, in pixels.</param>
            ''' <param name="clrLine">Node line colour.</param>
            ''' <param name="clrFill">Node fill colour.</param>
            '''--------------------------------------------------------------------
            Public Sub DrawNode(ByVal g As Graphics, _
                                ByVal ptf As PointF, _
                                ByVal nodetype As eNodeTypes, _
                                ByVal iSize As Integer, _
                                ByVal clrLine As Color, _
                                ByVal clrFill As Color)

                Using br As New SolidBrush(clrFill)
                    Using p As New Pen(clrLine)
                        Select Case nodetype
                            Case eNodeTypes.Circle
                                g.FillEllipse(br, ptf.X - CInt(iSize / 2), ptf.Y - CInt(iSize / 2), iSize, iSize)
                                g.DrawEllipse(p, ptf.X - CInt(iSize / 2), ptf.Y - CInt(iSize / 2), iSize, iSize)
                            Case eNodeTypes.Rectangle
                                g.FillRectangle(br, ptf.X - CInt(iSize / 2), ptf.Y - CInt(iSize / 2), iSize, iSize)
                                g.DrawRectangle(p, ptf.X - CInt(iSize / 2), ptf.Y - CInt(iSize / 2), iSize, iSize)
                            Case Else
                                Debug.Assert(False)
                        End Select
                    End Using
                End Using

            End Sub

            '''--------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="g"></param>
            ''' <param name="ptf"></param>
            ''' <param name="font"></param>
            ''' <param name="clrFont"></param>
            ''' <param name="strText">Formatted label text to draw.</param>
            '''--------------------------------------------------------------------
            Public Sub DrawLabel(ByVal g As Graphics, _
                                 ByVal ptf As PointF, _
                                 ByVal font As Font, _
                                 ByVal clrFont As Color, _
                                 ByVal strText As String)

                Using br As New SolidBrush(clrFont)
                    g.DrawString(strText, font, br, ptf, cFlowDiagramTree.g_fmt)
                End Using

            End Sub

            Friend Function CalcLabelSize(ByVal g As Graphics, _
                                          ByVal font As Font, _
                                          ByVal strText As String, _
                                          ByVal fmt As StringFormat, _
                                          ByVal iMaxWidth As Integer) As SizeF
                Return g.MeasureString(strText, font, iMaxWidth, fmt)
            End Function

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cFlowDiagramConnector

            Enum eConnectionType As Integer
                StraightLine = 1
                Arch = 2
            End Enum

#Region " Rendering "

            Public Sub DrawConnection(ByVal g As Graphics, _
                                      ByVal ptFrom As PointF, _
                                      ByVal ptTo As PointF, _
                                      ByVal clrLine As Color, _
                                      ByVal sWidth As Single, _
                                      ByVal connectiontype As eConnectionType)

                Dim pn As New Pen(clrLine, sWidth)

                Select Case connectiontype

                    Case eConnectionType.StraightLine
                        g.DrawLine(pn, ptFrom.X, ptFrom.Y, ptTo.X, ptTo.Y)

                    Case eConnectionType.Arch
                        ' Test if on top of each other
                        ' ToDo: use range comparison here
                        If (ptFrom.X <> ptTo.X) And (ptFrom.Y <> ptTo.Y) Then
                            Me.DrawArc(g, pn, ptFrom, ptTo)
                        Else
                            g.DrawLine(pn, ptFrom.X, ptFrom.Y, ptTo.X, ptTo.Y)
                        End If
                End Select

                pn.Dispose()

            End Sub

            Private Sub DrawArc(ByVal g As Graphics, ByVal pn As Pen, ByVal location1 As PointF, ByVal location2 As PointF)

                Dim sAngleSweep As Single = 90.0!
                Dim sAngleStart As Single = 0.0!
                Dim rcArc As RectangleF = New RectangleF(0, 0, 1, 1)
                Dim szArc As SizeF = New SizeF(Math.Abs(location1.X - location2.X) * 2, Math.Abs(location1.Y - location2.Y) * 2)

                If location1.X > location2.X And location1.Y > location2.Y Then
                    rcArc = New RectangleF(New PointF(location2.X, location2.Y - szArc.Height / 2), szArc)
                    sAngleStart = 180.0!
                    sAngleSweep = -90.0!
                ElseIf location1.X > location2.X And location1.Y < location2.Y Then
                    rcArc = New RectangleF(New PointF(location2.X, location1.Y), szArc)
                    sAngleStart = 180.0!
                    sAngleSweep = 90.0!
                ElseIf location1.X < location2.X And location1.Y > location2.Y Then
                    rcArc = New RectangleF(New PointF(location1.X - szArc.Width / 2, location2.Y - szArc.Height / 2), szArc)
                    sAngleStart = 0.0!
                    sAngleSweep = 90.0!
                ElseIf location1.X < location2.X And location1.Y < location2.Y Then
                    rcArc = New RectangleF(New PointF(location1.X - szArc.Width / 2, location1.Y), szArc)
                    sAngleStart = 0.0!
                    sAngleSweep = -90.0!
                End If

                g.DrawArc(pn, rcArc, sAngleStart, sAngleSweep)

            End Sub

#End Region ' Rendering

        End Class

#End Region ' Helper classes

#Region " Privates "

        Private m_data As IFlowDiagramData = Nothing
        Private m_colorramp As New cEwEColorRamp()
        Private m_iNumTrophicLevels As Integer = 6
        Private m_sAngle() As Single            '' To store where the angle is relative to 0
        Private m_asLabelOffsetX() As Single
        Private m_asLabelOffsetY() As Single
        Private m_node As cFlowDiagramNode = Nothing
        Private m_connectors As cFlowDiagramConnector = Nothing
        Private m_clrNode As Color = Color.LightGray
        Private m_bAutoNodeSize As Boolean = True
        Private m_iNodeSize As Integer = 10
        Private m_bIsDrawLabel As Boolean = True
        Private m_bIsNodeDrawValue As Boolean = False
        Private m_clrLine As Color = Color.Gray
        Private m_bAutoLineWidth As Boolean = False
        Private m_bShowTitle As Boolean = True
        Private m_iLineWidth As Integer = 1
        Private m_nodetype As cFlowDiagramNode.eNodeTypes = cFlowDiagramNode.eNodeTypes.Circle
        Private m_connectiontype As cFlowDiagramConnector.eConnectionType = cFlowDiagramConnector.eConnectionType.Arch
        Private m_colorusagetype As eColorUsageTypes = eColorUsageTypes.None
        Private m_tsShowLegend As TriState = TriState.UseDefault

        Private Shared g_fmt As New StringFormat()
        Private Shared g_wrapwidth As Integer = 150

#End Region ' Privates

        Public Enum eColorUsageTypes As Integer
            None
            EwE
            Value
            Flow
        End Enum

        Public Enum eHighlightType As Integer
            None
            Hidden
            LinkIn
            LinkOut
        End Enum

        Public Event OnChanged(ByVal sender As cFlowDiagramTree)

#Region " Constructor "

        Public Sub New(ByVal data As IFlowDiagramData)

            Debug.Assert(data IsNot Nothing)

            Me.m_data = data

            ReDim Me.m_sAngle(Me.m_data.NumGroups)
            ReDim Me.m_asLabelOffsetX(Me.m_data.NumGroups)
            ReDim Me.m_asLabelOffsetY(Me.m_data.NumGroups)

            Me.m_node = New cFlowDiagramNode()
            Me.m_connectors = New cFlowDiagramConnector()
            ' Elminate near-white colours
            Me.m_colorramp.ColorOffsetStart = 0.2!

            cFlowDiagramTree.g_fmt.Alignment = StringAlignment.Center
            Me.InitNodePositions()

        End Sub

#End Region ' Constructor

#Region " Drawing "

        Friend Sub DrawBackground(ByVal g As Graphics, ByVal rc As Rectangle)

            Dim iUnitHeight As Integer = CInt(rc.Height / Me.m_iNumTrophicLevels)
            Dim tsShowLegend As TriState = Me.ShowLegend

            Using brBack As New SolidBrush(Me.m_data.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.IMAGE_BACKGROUND))
                g.FillRectangle(brBack, rc)
            End Using

            Using brText As New SolidBrush(Me.m_data.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT))
                Using font As Font = Me.m_data.UIContext.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                    For i As Integer = 1 To m_iNumTrophicLevels - 1
                        g.DrawString((m_iNumTrophicLevels - i).ToString, font, brText, 20, i * iUnitHeight)
                        g.DrawLine(Pens.LightGray, 20, i * iUnitHeight, rc.Width - 20, i * iUnitHeight)
                    Next i
                End Using
            End Using

            If (tsShowLegend = TriState.UseDefault) Then
                Select Case Me.m_colorusagetype
                    Case eColorUsageTypes.Value, eColorUsageTypes.Flow
                        tsShowLegend = TriState.True
                End Select
            End If

            If (tsShowLegend = TriState.True) Then
                Me.DrawLegend(g, Me.m_data.ValueMax, New Point(5, 5), Me.m_data.Title)
            End If

        End Sub

        Friend Sub DrawTitle(ByVal g As Graphics, _
                             ByVal rc As Rectangle)

            Dim strTitle As String = Me.m_data.Title

            If (Not Me.m_bShowTitle) Or (String.IsNullOrWhiteSpace(strTitle)) Then Return

            Using brText As New SolidBrush(Me.m_data.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT))
                Using font As Font = Me.m_data.UIContext.StyleGuide.Font(cStyleGuide.eApplicationFontType.Title)
                    Dim szf As SizeF = g.MeasureString(strTitle, font)
                    g.DrawString(strTitle, font, brText, rc.X + (rc.Width - szf.Width) / 2, rc.Y + font.Height * 3)
                End Using
            End Using

        End Sub

        Friend Sub DrawNode(ByVal g As Graphics, _
                            ByVal rc As Rectangle, _
                            ByVal iGroup As Integer, _
                            ByVal bVisible As Boolean,
                            ByVal bHighlight As Boolean)

            Dim strLabel As String = Me.FormatLabelText(iGroup)
            Dim sValue As Single = Me.m_data.Value(iGroup)
            Dim sValueMax As Single = Me.m_data.ValueMax
            Dim clrPen As Color = Color.Black
            Dim clrFill As Color = Color.LightGray
            Dim iSize As Integer = Me.CalcNodeSize(sValue, sValueMax)

            If bVisible Then
                If (sValue = 0) Then
                    clrPen = Me.m_data.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT)
                    clrFill = Color.White
                Else
                    Select Case m_colorusagetype
                        Case eColorUsageTypes.EwE
                            clrFill = Me.m_data.GroupColor(iGroup)
                        Case eColorUsageTypes.Value
                            clrFill = Me.m_colorramp.GetColor(sValue, sValueMax)
                        Case Else
                            clrFill = Me.m_clrNode
                    End Select
                End If
            Else
                clrPen = Color.LightGray
                clrFill = Color.White
            End If

            Me.m_node.DrawNode(g, Me.NodeLocation(iGroup, rc), Me.NodeType, iSize, clrPen, clrFill)

            If bVisible Then
                clrPen = Me.TextColor
            Else
                clrPen = cColorUtils.GetVariant(Me.TextColor, 0.5!)
            End If

            If (Me.m_bIsDrawLabel) Then
                Me.m_node.DrawLabel(g, Me.LabelLocation(iGroup, rc), Me.RenderFont, clrPen, Me.FormatLabelText(iGroup))
            End If

        End Sub

        Friend Sub DrawConnection(ByVal g As Graphics, _
                                  ByVal rc As Rectangle, _
                                  ByVal iPred As Integer, _
                                  ByVal iPrey As Integer, _
                                  ByVal highlight As eHighlightType)

            Dim clrLine As Color = Me.m_clrLine
            Dim sDiet As Single = Me.m_data.LinkValue(iPred, iPrey)
            Dim sDietMax As Single = Me.m_data.LinkValueMax
            Dim sLineWidth As Single = 0.5!

            If sDiet <= 0 Then Return

            Select Case highlight

                Case eHighlightType.None
                    Select Case Me.m_colorusagetype
                        Case eColorUsageTypes.Flow
                            clrLine = Me.m_colorramp.GetColor(sDiet, sDietMax)
                        Case Else
                            ' Normal
                    End Select

                Case eHighlightType.Hidden
                    Return ' clrLine = Color.FromArgb(255, 240, 240, 240)

                Case eHighlightType.LinkIn
                    clrLine = Me.InLinkColor
                    sLineWidth = 2.0!

                Case eHighlightType.LinkOut
                    clrLine = Me.OutLinkColor
                    sLineWidth = 2.0!
            End Select

            sLineWidth *= Me.CalcLineWidth(sDiet, sDietMax)

            Me.m_connectors.DrawConnection(g, _
                                        Me.NodeLocation(iPred, rc), _
                                        Me.NodeLocation(iPrey, rc), _
                                        clrLine, _
                                        sLineWidth, _
                                        Me.LineConnectionType)
        End Sub

        Friend Sub DrawLegend(ByVal g As Graphics, _
                              ByVal sValMax As Single, ByVal ptTopLeft As Point, _
                              ByVal strTitle As String)

            Dim lgd As New cLegend(Me.m_data.UIContext, strTitle)
            lgd.AddGradient("", 0, sValMax)
            lgd.Draw(g, ptTopLeft)

        End Sub

#End Region ' Drawing

#Region " SetPosition "

        Public Sub MoveNode(ByVal rc As Rectangle, ByVal ptNew As PointF, ByVal iNode As Integer)
            Me.NodeLocation(iNode, rc) = ptNew
        End Sub

        Public Sub MoveLabel(ByVal rc As Rectangle, ByVal ptNew As PointF, ByVal iNode As Integer)
            Me.LabelLocation(iNode, rc) = ptNew
        End Sub

        Friend Sub InitNodePositions()

            Dim iNumTL As Integer = 4
            Dim aiGroupCount(iNumTL) As Integer
            Dim aiGroup(iNumTL) As Integer
            Dim iTL As Integer

            ' Calc how the groups are distributed over trophic levels [1, iNumTL+]
            For iGroup As Integer = 1 To Me.m_data.NumGroups
                iTL = iNumTL
                While (Me.m_data.TrophicLevel(iGroup) < iTL) And (iTL > 1)
                    iTL -= 1
                End While
                aiGroupCount(iTL) += 1
            Next

            ' Distribute groups horizontally
            For iGroup As Integer = 1 To Me.m_data.NumGroups

                iTL = iNumTL
                While (Me.m_data.TrophicLevel(iGroup) < iTL) And (iTL > 1)
                    iTL -= 1
                End While
                Me.m_sAngle(iGroup) = 360.0! * (aiGroup(iTL) + 0.5!) / aiGroupCount(iTL)

                aiGroup(iTL) += 1

            Next

        End Sub

#End Region ' SetPosition

#Region " Configuration "

        ' ToDo_JS: somehow globalize these properties 
        <Browsable(True), _
            Category("Appearance"), _
            DisplayName("Show title"), _
            Description("Draw the title on the flow diagram"), _
            DefaultValue(True)> _
        Public Property ShowTitle() As Boolean
            Get
                Return Me.m_bShowTitle
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowTitle = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Appearance"), _
            DisplayName("Number of trophic levels"), _
            Description("The number of trophic levels to display."), _
            DefaultValue(7)> _
        Public Property NumberOfTrophicLevels() As Integer
            Get
                Return Me.m_iNumTrophicLevels - 1
            End Get
            Set(ByVal value As Integer)
                If (value <> (Me.m_iNumTrophicLevels + 1)) Then
                    Me.m_iNumTrophicLevels = value + 1
                    RaiseEvent OnChanged(Me)
                    Me.InitNodePositions()
                End If
            End Set
        End Property

        <Browsable(True), _
            Category("Appearance"), _
            DisplayName("Auto-colour"), _
            Description("Define which aspect of the flow diagram to auto-colour: nodes by value, nodes by EwE default colour, or lines by diet"), _
            DefaultValue(eColorUsageTypes.None)> _
        Public Property AutoColorUsage() As eColorUsageTypes
            Get
                Return Me.m_colorusagetype
            End Get
            Set(ByVal value As eColorUsageTypes)
                Me.m_colorusagetype = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Appearance"), _
            DisplayName("Show legend"), _
            Description("True to always show the legend, false to always hide the legend, or UseDefault to use application wide settings."), _
            DefaultValue(TriState.UseDefault)> _
        Public Property ShowLegend As TriState
            Get
                Return Me.m_tsShowLegend
            End Get
            Set(ByVal value As TriState)
                Me.m_tsShowLegend = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
             Category("Node"), _
             DisplayName("Custom node color"), _
             Description("Custom color to use for nodes if nodes are not auto-colored."), _
             DefaultValue(&HFFD3D3D3)> _
        Public Property CustomNodeColor() As Color
            Get
                Return Me.m_clrNode
            End Get
            Set(ByVal value As Color)
                Me.m_clrNode = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Node"), _
            DisplayName("Auto-size nodes"), _
            Description("Set true to scale nodes to value."), _
            DefaultValue(True)> _
        Public Property AutoNodeSize() As Boolean
            Get
                Return Me.m_bAutoNodeSize
            End Get
            Set(ByVal value As Boolean)
                Me.m_bAutoNodeSize = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Node"), _
            DisplayName("Node custom size"), _
            Description("Custom node size to use if nodes are not set to auto-size."), _
            DefaultValue(True)> _
        Public Property CustomNodeSize() As Integer
            Get
                Return Me.m_iNodeSize
            End Get
            Set(ByVal value As Integer)
                Me.m_iNodeSize = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Node"), _
            DisplayName("Node type"), _
            Description("Shape type to use for rendering groups."), _
            DefaultValue(cFlowDiagramNode.eNodeTypes.Circle)> _
        Public Property NodeType() As cFlowDiagramNode.eNodeTypes
            Get
                Return Me.m_nodetype
            End Get
            Set(ByVal value As cFlowDiagramNode.eNodeTypes)
                Me.m_nodetype = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Line"), _
            DisplayName("Auto-scale lines"), _
            Description("Scale lines by flow amount."), _
            DefaultValue(False)> _
        Public Property AutoLineWidth() As Boolean
            Get
                Return Me.m_bAutoLineWidth
            End Get
            Set(ByVal value As Boolean)
                Me.m_bAutoLineWidth = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Line"), _
            DisplayName("Custom line width"), _
            Description("Custom line width to use if lines are not set to auto-scale."), _
            DefaultValue(1)> _
        Public Property CustomLineWidth() As Integer
            Get
                Return Me.m_iLineWidth
            End Get
            Set(ByVal value As Integer)
                Me.m_iLineWidth = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Line"), _
            DisplayName("Custom line color"), _
            Description("Custom color to use for lines if lines are not auto-colored.")> _
        Public Property CustomLineColor() As Color
            Get
                Return Me.m_clrLine
            End Get
            Set(ByVal value As Color)
                Me.m_clrLine = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Line"), _
            DisplayName("Line type"), _
            Description("Line type to use for rendering flows."), _
            DefaultValue(cFlowDiagramConnector.eConnectionType.Arch)> _
        Public Property LineConnectionType() As cFlowDiagramConnector.eConnectionType
            Get
                Return Me.m_connectiontype
            End Get
            Set(ByVal value As cFlowDiagramConnector.eConnectionType)
                If (value <> Me.m_connectiontype) Then
                    Me.m_connectiontype = value
                    RaiseEvent OnChanged(Me)
                End If
            End Set
        End Property

        <Browsable(True), _
            Category("Node"), _
            DisplayName("Show value in label"), _
            Description("Show value of groups in the node labels."), _
            DefaultValue(False)> _
        Public Property NodeDrawValue() As Boolean
            Get
                Return Me.m_bIsNodeDrawValue
            End Get
            Set(ByVal value As Boolean)
                Me.m_bIsNodeDrawValue = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

        <Browsable(True), _
            Category("Node"), _
            DisplayName("Draw labels"), _
            Description("Draw group name labels.")> _
        Public Property NodeDrawLabels() As Boolean
            Get
                Return Me.m_bIsDrawLabel
            End Get
            Set(ByVal value As Boolean)
                Me.m_bIsDrawLabel = value
                RaiseEvent OnChanged(Me)
            End Set
        End Property

#End Region ' Configuration

#Region " Public properties "

        Public Property NodeLocation(ByVal i As Integer, ByVal rc As Rectangle) As PointF
            Get
                Dim pt As PointF
                pt.X = CSng(Me.m_sAngle(i) / 360 * (rc.Width - 40)) + 20
                pt.Y = (Me.m_iNumTrophicLevels - Me.m_data.TrophicLevel(i)) * CInt(rc.Height / Me.m_iNumTrophicLevels)
                Return pt
            End Get
            Set(ByVal value As PointF)
                Dim angVal As Single = CSng((value.X - 20) / (rc.Width - 40) * 360)
                Me.m_sAngle(i) = Math.Max(0.0!, Math.Min(360.0!, angVal))
            End Set
        End Property

        Public Property LabelLocation(ByVal i As Integer, ByVal rc As Rectangle) As PointF
            Get
                Dim ptfNode As PointF = Me.NodeLocation(i, rc)
                Return New PointF(ptfNode.X + Me.m_asLabelOffsetX(i), ptfNode.Y + Me.m_asLabelOffsetY(i))
            End Get
            Set(ByVal value As PointF)
                Dim ptfNode As PointF = Me.NodeLocation(i, rc)
                Me.m_asLabelOffsetX(i) = value.X - ptfNode.X
                Me.m_asLabelOffsetY(i) = value.Y - ptfNode.Y
            End Set
        End Property

        Public Function IsNodeAtPoint(ByVal rc As Rectangle, ByVal ptfTest As PointF, _
                                      ByVal i As Integer, ByVal sValue As Single) As Boolean

            Dim ptfNodeLocation As PointF = Me.NodeLocation(i, rc)
            Dim sNodeSize As Single = CSng(Me.CalcNodeSize(sValue, Me.m_data.ValueMax))
            Dim rcf As New RectangleF(ptfNodeLocation.X - sNodeSize / 2, _
                                      ptfNodeLocation.Y - sNodeSize / 2, _
                                      sNodeSize, _
                                      sNodeSize)

            Return rcf.Contains(ptfTest)

        End Function

        Public Function IsLabelAtPoint(ByVal rc As Rectangle, _
                                       ByVal ptfTest As PointF, _
                                       ByVal i As Integer, _
                                       ByVal strLabel As String, _
                                       ByVal g As Graphics, _
                                       ByVal font As Font) As Boolean

            Dim ptfLabelLocation As PointF = Me.LabelLocation(i, rc)
            Dim szfLabel As SizeF = Me.m_node.CalcLabelSize(g, font, strLabel, cFlowDiagramTree.g_fmt, cFlowDiagramTree.g_wrapwidth)
            Dim rcf As New RectangleF(ptfLabelLocation.X - szfLabel.Width / 2, ptfLabelLocation.Y, szfLabel.Width, szfLabel.Height)

            Return rcf.Contains(ptfTest)

        End Function

#End Region ' Properties

#Region " EwE styling "

        Public Function RenderFont() As Font
            Dim uic As cUIContext = Me.m_data.UIContext
            Debug.Assert(uic IsNot Nothing)
            Return uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
        End Function

        Public Function TextColor() As Color
            Dim uic As cUIContext = Me.m_data.UIContext
            Debug.Assert(uic IsNot Nothing)
            Return uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
        End Function

        Public Function InLinkColor() As Color
            Dim uic As cUIContext = Me.m_data.UIContext
            Debug.Assert(uic IsNot Nothing)
            Return uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PREY)
        End Function

        Public Function OutLinkColor() As Color
            Dim uic As cUIContext = Me.m_data.UIContext
            Debug.Assert(uic IsNot Nothing)
            Return uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.PREDATOR)
        End Function

        Public Function FormatLabelText(iGroup As Integer) As String

            Dim sb As New StringBuilder()
            Dim sValue As Single = Me.m_data.Value(iGroup)
            Dim strName As String = Me.m_data.GroupName(iGroup)

            sb.AppendLine(strName)
            If Me.m_bIsNodeDrawValue And (sValue <> 0.0!) Then
                sb.AppendLine(Me.m_data.ValueLabel(sValue))
            End If
            Return sb.ToString

        End Function

#End Region ' EwE styling

#Region " Internals "

        Private ReadOnly Property CalcNodeSize(ByVal sValue As Single, ByVal sValueMax As Single) As Integer
            Get
                Dim iSize As Integer = Me.m_iNodeSize

                If Me.m_bAutoNodeSize Then
                    If sValue > 0 And sValueMax > 0 Then
                        ' Ln(values 1-11) make max ~2.5 => times 4 to scale to 10
                        ' Note that Math.Log = ln
                        iSize = CInt(Math.Log(1.2 + (10 * sValue / sValueMax)) * (1.2 * iSize))
                    End If
                End If
                Return Math.Max(2, iSize)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the size of a line in the diagram.
        ''' </summary>
        ''' <param name="sValue">Value represented by the line.</param>
        ''' <param name="sValueMax">Max value represented by all lines.</param>
        ''' -------------------------------------------------------------------
        Private ReadOnly Property CalcLineWidth(ByVal sValue As Single, ByVal sValueMax As Single) As Integer
            Get
                Dim sLineSize As Single = Me.m_iLineWidth

                If Me.m_bAutoLineWidth Then
                    If sValueMax > 0 And sValue > 0 Then
                        sLineSize = CSng(Math.Log(1.2 + (10 * sValue / sValueMax)) * 4) ' Log(values 1-11) make max ~2.5 => times 4 to scale to 10
                    End If
                End If

                Return CInt(Math.Min(Math.Max(1, sLineSize), 10))
            End Get
        End Property

#End Region ' Internals

    End Class

End Namespace