'==============================================================================
'
' $Log: PolicyColorBlocks.vb,v $
' Revision 1.1  2008/09/26 07:31:53  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.35  2008/06/02 00:01:40  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.34  2008/05/30 19:03:05  jeroens
' Fixed property confusion
'
' Revision 1.33  2008/05/29 22:23:01  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.32  2008/05/16 17:06:53  joeb
' Changes for new search objective datatypes
'
' Revision 1.31  2008/05/15 18:59:39  joeb
' Added BatchEdit to SearchBlocks
'
' Revision 1.30  2008/05/14 20:36:26  joeb
' SetGear now clears all years less than the base year
'
' Revision 1.29  2008/05/12 19:03:44  joeb
' Changes to search objects to support ISearchObjective interface
'
' Revision 1.28  2008/04/15 15:25:39  joeb
' How uses Property objects to handle core updates/messages
'
' Revision 1.27  2008/03/03 23:52:25  jeroens
' Prettinized
'
' Revision 1.26  2008/02/27 19:31:02  joeb
' Set Base Year
'
' Revision 1.25  2008/02/05 03:29:53  jeroens
' Fixed block redraw issue when resizing window
'
' Revision 1.24  2008/02/04 00:49:41  jeroens
' SetGear starts at year 2
' Sequential start year set to 2 by default
'
' Revision 1.23  2008/02/04 00:30:35  jeroens
' Yippee
'
' Revision 1.22  2008/02/03 03:55:51  jeroens
' Chasing down block bug, commenting code as I go
'
' Revision 1.21  2007/11/21 01:12:03  jeroens
' * Fixed bug 333
' - Removed local cache of fleet names
'
'==============================================================================

#Region "Imports directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports EwEUtils.Core

#End Region

Namespace Ecosim
    Public Class PolicyColorBlocks

        Private m_core As cCore
        Private m_FPManager As cFishingPolicyManager
        Private m_FPParams As cFishingPolicyParameters

        Private m_iTotalBlocks As Integer
        'Private m_FleetNames As New List(Of String)
        Private m_clrCurrent As Color
        Private m_BlockCells(,) As Integer
        Private m_bIsSketching As Boolean

        Private m_iRows As Integer
        Private m_iCols As Integer
        Private m_sFirstColWidth As Single
        Private m_sRowHeight As Single
        Private m_sColWidth As Single

        Private m_bIsFirstTimeLoaded As Boolean = True

        Private m_EcosimMsgHandler As cMessageHandler

        ''current base year is blocked out (zero/black) in the interface 
        'Private m_baseYear As Integer

        Private m_PropBaseYear As cProperty
        Private m_PropEcosimNYears As cProperty

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() 
            m_core = cCore.GetInstance()
            m_FPManager = m_core.FishingPolicyManager
            m_FPParams = m_FPManager.ModelParameters

        End Sub

        Private Sub Init()

            m_iTotalBlocks = m_core.EcoSimModelParameters.NumberYears
            nupSeqEndYear.Maximum = m_iTotalBlocks
            nupYearBlockNum.Maximum = m_iTotalBlocks

            m_bIsSketching = False

            If m_clrCurrent = Nothing Then
                m_clrCurrent = Color.Green
            End If

            ReDim m_BlockCells(Me.m_core.nFleets, m_iTotalBlocks)
            Dim fpFleetInput As cFishingPolicySearchBlock = Nothing

            For i As Integer = 1 To m_BlockCells.GetLength(0) - 1
                fpFleetInput = m_FPManager.SearchBlocks(i)
                For j As Integer = 1 To m_BlockCells.GetLength(1) - 1
                    m_BlockCells(i, j) = fpFleetInput.SearchBlocks(j)
                Next
            Next

        End Sub



        Public Property CurColor() As Color
            Get
                Return m_clrCurrent
            End Get
            Set(ByVal value As Color)
                m_clrCurrent = value
            End Set
        End Property

        Public ReadOnly Property ParmBlockCodes() As ParmBlockCodes
            Get
                Return m_blockCodes
            End Get
        End Property


        Private Sub PolicyColorBlocks_HandleDestroyed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.HandleDestroyed

            RemoveHandler Me.m_PropBaseYear.PropertyChanged, AddressOf OnPropChanged
            RemoveHandler Me.m_PropEcosimNYears.PropertyChanged, AddressOf OnPropChanged

        End Sub

        Private Sub PolicyColorBlocks_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' Designer mode test
            If Me.m_core Is Nothing Then Return

            Init()

            'Me.Dock = DockStyle.Fill
            nupYearBlockNum.Value = CDec(m_iTotalBlocks)
            nupSeqStartYear.Value = CDec(Math.Min(2, m_iTotalBlocks))
            nupSeqEndYear.Value = CDec(m_iTotalBlocks)
            m_bIsFirstTimeLoaded = False

            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
            '         Dim parameters As cFishingPolicyParameters = m_core.FishingPolicyManager.ObjectiveParameters

            Me.m_PropBaseYear = DirectCast(pm.GetProperty(m_core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchBaseYear), cIntegerProperty)
            AddHandler Me.m_PropBaseYear.PropertyChanged, AddressOf OnPropChanged

            Me.m_PropEcosimNYears = DirectCast(pm.GetProperty(m_core.EcoSimModelParameters, eVarNameFlags.EcoSimNYears), cIntegerProperty)
            AddHandler Me.m_PropEcosimNYears.PropertyChanged, AddressOf OnPropChanged

        End Sub

        Private Sub OnPropChanged(ByVal p As cProperty, ByVal cf As cProperty.eChangeFlags)

            'right now if anything changed just reload
            Init()
            pbFishingBlocks.Invalidate()

        End Sub


        Private Sub pbFishingBlocks_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pbFishingBlocks.Paint
            Dim g As Graphics = e.Graphics
            CalcParams(g)
            DrawRowCols(g)
        End Sub

        Private Sub pbFishingBlocks_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbFishingBlocks.MouseDown
            m_bIsSketching = True
        End Sub

        Private Sub pbFishingBlocks_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbFishingBlocks.MouseMove
            If m_bIsSketching Then
                ProcessCellClick(e.X, e.Y)
            End If
        End Sub

        Private Sub pbFishingBlocks_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbFishingBlocks.MouseUp
            m_bIsSketching = False
        End Sub

        Private Sub pbFishingBlocks_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbFishingBlocks.MouseClick
            ProcessCellClick(e.X, e.Y)
        End Sub

#Region "Private methods"

        Private Sub DrawRowCols(ByRef g As Graphics)

            'Draw row lines
            For i As Integer = 1 To m_iRows - 1
                Dim yPos As Single = 0 + i * m_sRowHeight
                g.DrawLine(Pens.Gray, 0, yPos, pbFishingBlocks.Width, yPos)
                g.DrawString(Me.m_core.FleetInputs(i).Name, pbFishingBlocks.Font, Brushes.Black, 1, yPos + 1)
            Next

            g.DrawLine(Pens.Gray, m_sFirstColWidth, pbFishingBlocks.Top, m_sFirstColWidth, pbFishingBlocks.Bottom)

            For j As Integer = 1 To m_iCols
                Dim xPos As Single = m_sFirstColWidth + (j - 1) * m_sColWidth
                g.DrawLine(Pens.Gray, xPos, 0, xPos, pbFishingBlocks.Height)
                Dim txt As String = (j Mod 10).ToString
                g.DrawString(txt, pbFishingBlocks.Font, Brushes.Black, xPos + 1, 1)
                'If j < 3 Then Console.WriteLine("Col {0} xPos = {1}", j, xPos)
            Next

            For i As Integer = 1 To m_iRows - 1
                For j As Integer = 1 To m_iCols - 1
                    Dim yPos As Single = i * m_sRowHeight
                    Dim xPos As Single = m_sFirstColWidth + (j - 1) * m_sColWidth
                    Dim tmpBrush As New SolidBrush(m_blockCodes.BlockColor(m_BlockCells(i, j)))
                    g.FillRectangle(tmpBrush, New RectangleF(xPos, yPos, m_sColWidth, m_sRowHeight))
                Next

            Next

        End Sub

        Private Sub CalcParams(ByRef g As Graphics)

            Me.m_iRows = Me.m_core.nFleets + 1
            Me.m_sRowHeight = CSng(pbFishingBlocks.Height / Me.m_iRows)

            Dim sLenMax As Single = -1
            For i As Integer = 0 To Me.m_core.nFleets - 1
                Dim tmpWidth As Single = g.MeasureString(Me.m_core.FleetInputs(i + 1).Name, pbFishingBlocks.Font).Width
                If sLenMax < tmpWidth Then sLenMax = tmpWidth
            Next

            'First column line 
            Me.m_sFirstColWidth = sLenMax + 10
            Me.m_iCols = Me.m_iTotalBlocks + 1
            Me.m_sColWidth = CSng((pbFishingBlocks.Width - Me.m_sFirstColWidth) / Me.m_iTotalBlocks)

        End Sub

        Private Sub ProcessCellClick(ByVal x As Integer, ByVal y As Integer)

            Dim iRow As Integer = CInt(Math.Floor(y / m_sRowHeight))
            Dim iCol As Integer = CInt(Math.Floor((x - m_sFirstColWidth) / m_sColWidth) + 1)

            If iRow < 0 Or iRow > m_iRows - 1 Then Return
            If iCol > m_iCols - 1 Then Return

            'Populate the SearchBlock first then use that value to populate the controls buffer
            'this allows the core to do validation

            ' Is row header clicked?
            If (iCol < 1) Then
                ' #Yes: is column header clicked? If so: cannot fill block row
                If iRow < 1 Then Return
                For i As Integer = 1 To m_BlockCells.GetLength(1) - 1
                    Me.m_FPManager.SearchBlocks(iRow).SearchBlocks(i) = Me.m_blockCodes.SelectedBlockNum
                    Me.m_BlockCells(iRow, i) = Me.m_FPManager.SearchBlocks(iRow).SearchBlocks(i)
                    ' Me.m_FPManager.FleetInputs(iRow).SearchBlocks(i) = Me.m_blockCodes.SelectedBlockNum
                Next
                ' Is column header clicked?
            ElseIf (iRow < 1) Then
                ' #Yes: is row header clicked? If so: cannot fill block column
                If iCol < 1 Then Return
                For i As Integer = 1 To m_BlockCells.GetLength(0) - 1
                    Me.m_FPManager.SearchBlocks(i).SearchBlocks(iCol) = Me.m_blockCodes.SelectedBlockNum
                    Me.m_BlockCells(i, iCol) = Me.m_FPManager.SearchBlocks(i).SearchBlocks(iCol)
                    '  Me.m_FPManager.FleetInputs(i).SearchBlocks(iCol) = Me.m_blockCodes.SelectedBlockNum
                Next
            Else
                ' Fill single block
                Me.m_FPManager.SearchBlocks(iRow).SearchBlocks(iCol) = Me.m_blockCodes.SelectedBlockNum
                Me.m_BlockCells(iRow, iCol) = Me.m_FPManager.SearchBlocks(iRow).SearchBlocks(iCol)
            End If

            pbFishingBlocks.Invalidate()

        End Sub

#End Region

        Private Sub btnSetEveryGear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetEveryGear.Click
            SetSeqColorCodes(2, m_iTotalBlocks, CInt(nupYearBlockNum.Value))
        End Sub

        Private Sub nupSeqStartYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nupSeqStartYear.ValueChanged

            Dim startYear As Integer = CInt(nupSeqStartYear.Value)
            Dim endYear As Integer = CInt(nupSeqEndYear.Value)

            nupSeqEndYear.Minimum = nupSeqStartYear.Value

            SetSeqColorCodes(startYear, endYear, m_iTotalBlocks)

        End Sub

        Private Sub nupSeqEndYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nupSeqEndYear.ValueChanged

            Dim startYear As Integer = CInt(nupSeqStartYear.Value)
            Dim endYear As Integer = CInt(nupSeqEndYear.Value)

            nupSeqStartYear.Maximum = nupSeqEndYear.Value

            SetSeqColorCodes(startYear, endYear, m_iTotalBlocks)

        End Sub

        Private Sub SetSeqColorCodes(ByVal startYear As Integer, ByVal endYear As Integer, ByVal yearPerBlock As Integer)

            If m_bIsFirstTimeLoaded Then Return
            If startYear > endYear Or startYear <= 0 Or endYear <= 0 Then Return
            If m_BlockCells Is Nothing Then Return
            If endYear > m_BlockCells.GetLength(1) - 1 Then endYear = m_BlockCells.GetLength(1) - 1

            Dim nColors As Integer = m_blockCodes.nBlockCodes - 1
            Dim yearSegment As Integer = CInt(Math.Ceiling(m_iTotalBlocks / yearPerBlock))
            Dim totalClr As Integer = yearSegment * Me.m_core.nFleets

            If totalClr > nColors Then
                m_blockCodes.nBlockCodes = totalClr + 1
            End If

            Dim colors As List(Of Color) = m_blockCodes.BlockColors
            Dim cnt As Integer = 1
            Dim stepSize As Integer = CInt(Math.Floor(m_blockCodes.nBlockCodes / totalClr))

            For i As Integer = 1 To m_BlockCells.GetLength(0) - 1
                'Console.WriteLine("iColor = {0} selClr = {1}", cnt, selClr)

                For j As Integer = 0 To yearSegment - 1
                    cnt += stepSize
                    For l As Integer = 1 To yearPerBlock
                        Dim jIndex As Integer = j * yearPerBlock + l
                        If jIndex <= endYear AndAlso jIndex >= startYear Then
                            m_BlockCells(i, jIndex) = cnt
                        End If
                    Next
                Next

                ' Black out blocks
                For j As Integer = 1 To startYear - 1
                    m_BlockCells(i, j) = 0
                Next
                For j As Integer = endYear + 1 To m_BlockCells.GetLength(1) - 1
                    m_BlockCells(i, j) = 0
                Next
            Next

            For iflt As Integer = 1 To Me.m_core.nFleets
                'the batch edit flag stops the searchblocks from sending out any messages
                m_FPManager.SearchBlocks(iflt).BatchEdit = True
                For iyr As Integer = 1 To Me.m_core.nEcosimYears
                    If iyr <= m_FPManager.ObjectiveParameters.BaseYear Then
                        'clear all blocks less than the baseyear
                        m_BlockCells(iflt, iyr) = 0
                    End If
                    m_FPManager.SearchBlocks(iflt).SearchBlocks(iyr) = m_BlockCells(iflt, iyr)
                Next
                m_FPManager.SearchBlocks(iflt).BatchEdit = False
            Next iflt

            pbFishingBlocks.Invalidate()

        End Sub

        Private Sub PolicyColorBlocks_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
            ' Redraw the fishing blocks
            Me.pbFishingBlocks.Invalidate()
        End Sub

    End Class

End Namespace

