#Region " Imports "

Option Strict On
Imports System.Drawing.Color
Imports System.IO
Imports System.Math
Imports System.Threading
Imports System.ComponentModel
Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SAUPUtil.Misc.Colours
Imports SAUPUtil.SAUPData.Mapping

#End Region ' Imports

Namespace Ecopath.Controls.FlowDiagram

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
            ''' 
            ''' </summary>
            ''' <param name="g"></param>
            ''' <param name="ptf"></param>
            ''' <param name="nodetype"></param>
            ''' <param name="iSize"></param>
            ''' <param name="clr"></param>
            '''--------------------------------------------------------------------
            Public Sub DrawNode(ByVal g As Graphics, _
                                ByVal ptf As PointF, _
                                ByVal nodetype As eNodeTypes, _
                                ByVal iSize As Integer, _
                                ByVal clr As Color)

                Using brush As New SolidBrush(clr)

                    Select Case nodetype
                        Case eNodeTypes.Circle
                            g.FillEllipse(brush, ptf.X - CInt(iSize / 2), ptf.Y - CInt(iSize / 2), iSize, iSize)
                            g.DrawEllipse(Pens.Black, ptf.X - CInt(iSize / 2), ptf.Y - CInt(iSize / 2), iSize, iSize)
                        Case eNodeTypes.Rectangle
                            g.FillRectangle(brush, ptf.X - CInt(iSize / 2), ptf.Y - CInt(iSize / 2), iSize, iSize)
                            g.DrawRectangle(Pens.Black, ptf.X - CInt(iSize / 2), ptf.Y - CInt(iSize / 2), iSize, iSize)
                        Case Else
                            Debug.Assert(False)
                    End Select

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
            ''' <param name="strGroupName"></param>
            ''' <param name="strBiomass"></param>
            '''--------------------------------------------------------------------
            Public Sub DrawLabel(ByVal g As Graphics, _
                                 ByVal ptf As PointF, _
                                 ByVal font As Font, _
                                 ByVal clrFont As Color, _
                                 ByVal strGroupName As String, _
                                 Optional ByVal strBiomass As String = "")

                Using br As New SolidBrush(clrFont)

                    ' Draw group name
                    g.DrawString(strGroupName, font, br, ptf.X, ptf.Y)

                    ' Draw the biomass string
                    If (Not String.IsNullOrEmpty(strBiomass)) Then
                        g.DrawString(String.Format(My.Resources.FLOWDIAGRAM_LABEL_BIOMASS, strBiomass), _
                                     font, br, _
                                     ptf.X, ptf.Y + CInt(font.Size * 1.5))
                    End If

                End Using

            End Sub

            Public Function CalcLabelSize(ByVal g As Graphics, _
                                          ByVal font As Font, _
                                          ByVal strGroupName As String) As SizeF
                Return g.MeasureString(strGroupName, font)
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

            Public Sub DrawConnection(ByRef g As Graphics, _
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

            Private Sub DrawArc(ByRef g As Graphics, ByVal pn As Pen, ByVal location1 As PointF, ByVal location2 As PointF)

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

        Private m_data As cFlowDiagramData = Nothing
        Private m_colorramp As New SAUPColorRamp()
        Private m_iNumTrophicLevels As Integer = 6
        Private m_sAngle() As Single            '' To store where the angle is relative to 0
        Private m_asLabelOffsetX() As Single
        Private m_asLabelOffsetY() As Single
        Private m_node As cFlowDiagramNode = Nothing
        Private m_connectors As cFlowDiagramConnector = Nothing
        Private m_clrNode As Color = Color.LightGray
        Private m_bAutoNodeSize As Boolean = True
        Private m_iNodeSize As Integer = 10
        Private m_bIsNodeDrawBiomass As Boolean = False
        Private m_clrLine As Color = Color.Gray
        Private m_bAutoLineWidth As Boolean = False
        Private m_iLineWidth As Integer = 1
        Private m_nodetype As cFlowDiagramNode.eNodeTypes = cFlowDiagramNode.eNodeTypes.Circle
        Private m_connectiontype As cFlowDiagramConnector.eConnectionType = cFlowDiagramConnector.eConnectionType.Arch
        Private m_colorusagetype As eColorUsageTypes = eColorUsageTypes.None

#End Region ' Privates

        Public Enum eColorUsageTypes As Integer
            None
            Groups
            Biomass
            Flow
        End Enum

        Public Event OnChanged(ByVal sender As cFlowDiagramTree)

#Region " Constructor "

        Public Sub New(ByVal data As cFlowDiagramData)

            Debug.Assert(data IsNot Nothing)

            Me.m_data = data

            ReDim Me.m_sAngle(Me.m_data.NumGroups)
            ReDim Me.m_asLabelOffsetX(Me.m_data.NumGroups)
            ReDim Me.m_asLabelOffsetY(Me.m_data.NumGroups)

            Me.m_node = New cFlowDiagramNode()
            Me.m_connectors = New cFlowDiagramConnector()
            ' Elminate near-white colours
            Me.m_colorramp.ColorOffsetStart = 0.2!

            Me.InitNodePositions()

        End Sub

#End Region ' Constructor

#Region " Drawing "

        Friend Sub DrawBackground(ByVal g As Graphics, ByVal rc As Rectangle)

            Dim iUnitHeight As Integer = CInt(rc.Height / Me.m_iNumTrophicLevels)

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

            Select Case Me.m_colorusagetype
                Case eColorUsageTypes.Biomass
                    Me.DrawLegend(g, Me.m_data.BiomassMax, New Point(5, 5), SharedResources.HEADER_BIOMASS)
                Case eColorUsageTypes.Flow
                    Me.DrawLegend(g, Me.m_data.DietMax, New Point(5, 5), SharedResources.HEADER_DIET)
            End Select

        End Sub

        Friend Sub DrawNode(ByRef g As Graphics, _
                            ByVal rc As Rectangle, _
                            ByVal iGroup As Integer)

            Dim strGroupName As String = Me.m_data.GroupName(iGroup)
            Dim sBiomass As Single = Me.m_data.Biomass(iGroup)
            Dim sBiomassMax As Single = Me.m_data.BiomassMax
            Dim strBiomassLabel As String = ""
            Dim clrNode As Color

            If Me.m_bIsNodeDrawBiomass And sBiomass > 0.0! Then
                strBiomassLabel = Me.m_data.UIContext.StyleGuide.FormatNumber(sBiomass, cStyleGuide.eStyleFlags.OK)
            End If

            Select Case m_colorusagetype
                Case eColorUsageTypes.Groups
                    clrNode = Me.m_data.GroupColor(iGroup)
                Case eColorUsageTypes.Biomass
                    clrNode = Me.m_colorramp.GetColor(sBiomass, sBiomassMax)
                Case Else
                    clrNode = Me.m_clrNode
            End Select

            Me.m_node.DrawNode(g, _
                               Me.NodeLocation(iGroup, rc), _
                               Me.NodeType, _
                               Me.CalcNodeSize(sBiomass, sBiomassMax), _
                               clrNode)


            Me.m_node.DrawLabel(g, _
                                Me.LabelLocation(iGroup, rc), _
                                Me.m_data.RenderFont, _
                                Me.m_data.TextColor, _
                                strGroupName, _
                                strBiomassLabel)

        End Sub

        Friend Sub DrawConnection(ByRef g As Graphics, _
                                  ByVal rc As Rectangle, _
                                  ByVal iPred As Integer, _
                                  ByVal iPrey As Integer, _
                                  ByVal bHighlightAsPredator As Boolean, _
                                  ByVal bHighlightAsPrey As Boolean)

            Dim clrLine As Color = Me.m_clrLine
            Dim sDiet As Single = Me.m_data.Diet(iPred, iPrey)
            Dim sDietMax As Single = Me.m_data.DietMax
            Dim sLineWidth As Single = 0.5!

            If sDiet <= 0 Then Return

            If bHighlightAsPredator Then
                clrLine = Me.m_data.HighlightEatsColor
                sLineWidth = 2.0!
            ElseIf bHighlightAsPrey Then
                clrLine = Me.m_data.HighlightIsEatenColor
                sLineWidth = 2.0!
            Else
                Select Case Me.m_colorusagetype
                    Case eColorUsageTypes.Flow
                        clrLine = Me.m_colorramp.GetColor(sDiet, sDietMax)
                    Case Else
                        ' NOP
                End Select
            End If

            sLineWidth *= Me.CalcLineWidth(sDiet, sDietMax)

            Me.m_connectors.DrawConnection(g, _
                                        Me.NodeLocation(iPred, rc), _
                                        Me.NodeLocation(iPrey, rc), _
                                        clrLine, _
                                        sLineWidth, _
                                        Me.LineConnectionType)
        End Sub

        Friend Sub DrawLegend(ByRef g As Graphics, _
                              ByVal sValMax As Single, ByVal ptTopLeft As Point, _
                              ByVal strTitle As String, _
                              Optional ByVal iXSize As Integer = 75, _
                              Optional ByVal iYSize As Integer = 80)

            Dim iNumIntervals As Integer = 5
            Dim sValInc As Single = 0
            Dim iIconHeight As Integer = CInt((iYSize * 0.7) / iNumIntervals)
            Dim ptIconTL As Point = New Point(CInt(iXSize * 0.1 + ptTopLeft.X), CInt(iYSize * 0.3 + ptTopLeft.Y))
            Dim font As Font = Me.m_data.UIContext.StyleGuide.Font(cStyleGuide.eApplicationFontType.Legend)
            Dim pen As New Pen(Me.m_data.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT))
            Dim brush As New SolidBrush(Me.m_data.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT))
            Dim brLegend As Brush = Nothing

            g.DrawRectangle(pen, New Rectangle(ptTopLeft, New Size(iXSize, iYSize)))
            g.DrawString(strTitle, font, brush, New Point(CInt(iXSize * 0.2 + ptTopLeft.X), CInt(iYSize * 0.05 + ptTopLeft.Y)))

            For i As Single = 1 To iNumIntervals

                sValInc += sValMax / iNumIntervals

                brLegend = New SolidBrush(Me.m_colorramp.GetColor(sValInc, sValMax))
                g.FillRectangle(brLegend, New Rectangle(ptIconTL, New Size(CInt(iXSize * 0.2), iIconHeight)))
                brLegend.Dispose()

                g.DrawString(String.Format(SharedResources.HEADER_LESSTHAN, Me.m_data.UIContext.StyleGuide.FormatNumber(Me.GetNiceNumber(sValInc))), _
                             font, brush, _
                             New Point(CInt(ptIconTL.X + iXSize * 0.3), ptIconTL.Y))
                ptIconTL.Y += iIconHeight
            Next i

            font.Dispose()
            pen.Dispose()
            brush.Dispose()

        End Sub

        Private Function GetNiceNumber(ByVal sNum As Single) As Single
            If sNum > 100000 Then sNum = 0
            'Return CSng(Math.Round(val, 3))
            Return sNum
        End Function

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

        <Browsable(True), _
            Category("Misc."), _
            DisplayName("Auto-colour"), _
            Description("Define which aspect of the flow diagram to auto-colour: nodes by biomass, nodes by group colour, or lines by diet"), _
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
             Category("Node"), _
             DisplayName("Custom node color"), _
             Description("Custom color to use for nodes if nodes are not auto-colored.")> _
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
            Description("Scale nodes to biomass."), _
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
            Description("Line type to use for rendering flows.")> _
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
            Category("Misc."), _
            DisplayName("Number of trophic levels"), _
            Description("The number of trophic levels to display.")> _
        Public Property NumberOfTrophicLevels() As Integer
            Get
                Return m_iNumTrophicLevels - 1
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
            Category("Node"), _
            DisplayName("Show biomass in label"), _
            Description("Show biomass of groups in the node labels.")> _
        Public Property NodeDrawBiomass() As Boolean
            Get
                Return Me.m_bIsNodeDrawBiomass
            End Get
            Set(ByVal value As Boolean)
                Me.m_bIsNodeDrawBiomass = value
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
                                      ByVal i As Integer, ByVal sBiomass As Single) As Boolean

            Dim ptfNodeLocation As PointF = Me.NodeLocation(i, rc)
            Dim sNodeSize As Single = CSng(Me.CalcNodeSize(sBiomass, Me.m_data.BiomassMax))
            Dim rcf As New RectangleF(ptfNodeLocation.X - sNodeSize / 2, _
                                      ptfNodeLocation.Y - sNodeSize / 2, _
                                      sNodeSize, _
                                      sNodeSize)

            Return rcf.Contains(ptfTest)

        End Function

        Public Function IsLabelAtPoint(ByVal rc As Rectangle, _
                                       ByVal ptfTest As PointF, _
                                       ByVal i As Integer, _
                                       ByVal strGroupName As String, _
                                       ByVal g As Graphics, _
                                       ByVal font As Font) As Boolean

            Dim ptfLabelLocation As PointF = Me.LabelLocation(i, rc)
            Dim szfLabel As SizeF = Me.m_node.CalcLabelSize(g, font, strGroupName)
            Dim rcf As New RectangleF(ptfLabelLocation, szfLabel)

            Return rcf.Contains(ptfTest)

        End Function

#End Region ' Properties

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