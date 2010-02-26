#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Control implementing the policy blocks scetch user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class ucPolicyColorBlocks
        Implements IUIElement

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
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

        Private m_PropBaseYear As cProperty
        Private m_PropEcosimNYears As cProperty

#End Region ' Private vars

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
                Me.m_blockCodes.UIContext = Me.UIContext

                If (value IsNot Nothing) Then
                    Me.m_FPManager = Me.UIContext.Core.FishingPolicyManager
                    Me.m_FPParams = Me.m_FPManager.ModelParameters
                    Me.Init()
                End If
            End Set
        End Property

        Public Property CurColor() As Color
            Get
                Return m_clrCurrent
            End Get
            Set(ByVal value As Color)
                m_clrCurrent = value
            End Set
        End Property

        Public ReadOnly Property ParmBlockCodes() As ucParmBlockCodes
            Get
                Return m_blockCodes
            End Get
        End Property

#Region " Overloads "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

        Protected Overrides Sub OnHandleDestroyed(ByVal e As System.EventArgs)
            MyBase.OnHandleDestroyed(e)

            If (Me.m_PropBaseYear IsNot Nothing) Then
                RemoveHandler Me.m_PropBaseYear.PropertyChanged, AddressOf OnPropChanged
                Me.m_PropBaseYear = Nothing

                RemoveHandler Me.m_PropEcosimNYears.PropertyChanged, AddressOf OnPropChanged
                Me.m_PropEcosimNYears = Nothing
            End If

        End Sub

#End Region ' Overloads

#Region " Events "

        Private Sub btnSetEveryGear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSetGear.Click

            Me.SetSeqColorCodes(2, Me.m_iTotalBlocks, CInt(Me.m_nudNumYearsPerBlock.Value))

        End Sub

        Private Sub nupSeqStartYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudSeqStartYear.ValueChanged

            Dim startYear As Integer = CInt(m_nudSeqStartYear.Value)
            Dim endYear As Integer = CInt(m_nudSeqEndYear.Value)

            Me.m_nudSeqEndYear.Minimum = Me.m_nudSeqStartYear.Value

            Me.SetSeqColorCodes(startYear, endYear, Me.m_iTotalBlocks)

        End Sub

        Private Sub nupSeqEndYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudSeqEndYear.ValueChanged

            Dim startYear As Integer = CInt(m_nudSeqStartYear.Value)
            Dim endYear As Integer = CInt(m_nudSeqEndYear.Value)

            Me.m_nudSeqStartYear.Maximum = Me.m_nudSeqEndYear.Value

            Me.SetSeqColorCodes(startYear, endYear, Me.m_iTotalBlocks)

        End Sub

        Private Sub pbFishingBlocks_Paint(ByVal sender As System.Object, ByVal e As PaintEventArgs) _
            Handles m_pbFishingBlocks.Paint
            If (Me.UIContext Is Nothing) Then Return
            Me.CalcParams(e.Graphics)
            Me.DrawRowCols(e.Graphics)
        End Sub

        Private Sub pbFishingBlocks_MouseDown(ByVal sender As System.Object, ByVal e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseDown
            Me.m_bIsSketching = True
        End Sub

        Private Sub pbFishingBlocks_MouseMove(ByVal sender As System.Object, ByVal e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseMove
            If Me.m_bIsSketching Then
                Me.ProcessCellClick(e.X, e.Y)
            End If
        End Sub

        Private Sub pbFishingBlocks_MouseUp(ByVal sender As System.Object, ByVal e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseUp
            Me.m_bIsSketching = False
        End Sub

        Private Sub pbFishingBlocks_MouseClick(ByVal sender As System.Object, ByVal e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseClick
            Me.ProcessCellClick(e.X, e.Y)
        End Sub

        Private Sub PolicyColorBlocks_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.SizeChanged
            ' Redraw the fishing blocks
            Me.m_pbFishingBlocks.Invalidate()
        End Sub

#End Region ' Events

#Region " Callbacks "

        Private Sub OnPropChanged(ByVal p As cProperty, ByVal cf As cProperty.eChangeFlags)

            'right now if anything changed just reload
            Init()
            m_pbFishingBlocks.Invalidate()

        End Sub

#End Region ' Callbacks

#Region "Private methods"

        Private Sub Init()

            m_iTotalBlocks = Me.m_uic.Core.EcoSimModelParameters.NumberYears
            m_nudSeqEndYear.Maximum = m_iTotalBlocks
            m_nudNumYearsPerBlock.Maximum = m_iTotalBlocks

            m_bIsSketching = False

            If m_clrCurrent = Nothing Then
                m_clrCurrent = Color.Green
            End If

            ReDim m_BlockCells(Me.m_uic.Core.nFleets, m_iTotalBlocks)
            Dim fpFleetInput As cFishingPolicySearchBlock = Nothing

            For i As Integer = 1 To m_BlockCells.GetLength(0) - 1
                fpFleetInput = m_FPManager.SearchBlocks(i)
                For j As Integer = 1 To m_BlockCells.GetLength(1) - 1
                    m_BlockCells(i, j) = fpFleetInput.SearchBlocks(j)
                Next
            Next

            m_nudNumYearsPerBlock.Value = CDec(m_iTotalBlocks)
            m_nudSeqStartYear.Value = CDec(Math.Min(2, m_iTotalBlocks))
            m_nudSeqEndYear.Value = CDec(m_iTotalBlocks)
            m_bIsFirstTimeLoaded = False

            Me.m_PropBaseYear = DirectCast(Me.m_uic.PropertyManager.GetProperty(Me.m_uic.Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchBaseYear), cIntegerProperty)
            AddHandler Me.m_PropBaseYear.PropertyChanged, AddressOf OnPropChanged

            Me.m_PropEcosimNYears = DirectCast(Me.m_uic.PropertyManager.GetProperty(Me.m_uic.Core.EcoSimModelParameters, eVarNameFlags.EcoSimNYears), cIntegerProperty)
            AddHandler Me.m_PropEcosimNYears.PropertyChanged, AddressOf OnPropChanged

        End Sub

        Private Sub DrawRowCols(ByRef g As Graphics)

            'Draw row lines
            For i As Integer = 1 To m_iRows - 1
                Dim yPos As Single = 0 + i * m_sRowHeight
                g.DrawLine(Pens.Gray, 0, yPos, m_pbFishingBlocks.Width, yPos)
                g.DrawString(Me.m_uic.Core.FleetInputs(i).Name, m_pbFishingBlocks.Font, Brushes.Black, 1, yPos + 1)
            Next

            g.DrawLine(Pens.Gray, m_sFirstColWidth, m_pbFishingBlocks.Top, m_sFirstColWidth, m_pbFishingBlocks.Bottom)

            For j As Integer = 1 To m_iCols
                Dim xPos As Single = m_sFirstColWidth + (j - 1) * m_sColWidth
                g.DrawLine(Pens.Gray, xPos, 0, xPos, m_pbFishingBlocks.Height)
                Dim txt As String = (j Mod 10).ToString
                g.DrawString(txt, m_pbFishingBlocks.Font, Brushes.Black, xPos + 1, 1)
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

            Me.m_iRows = Me.m_uic.Core.nFleets + 1
            Me.m_sRowHeight = CSng(m_pbFishingBlocks.Height / Me.m_iRows)

            Dim sLenMax As Single = -1
            For i As Integer = 0 To Me.m_uic.Core.nFleets - 1
                Dim tmpWidth As Single = g.MeasureString(Me.m_uic.Core.FleetInputs(i + 1).Name, m_pbFishingBlocks.Font).Width
                If sLenMax < tmpWidth Then sLenMax = tmpWidth
            Next

            'First column line 
            Me.m_sFirstColWidth = sLenMax + 10
            Me.m_iCols = Me.m_iTotalBlocks + 1
            Me.m_sColWidth = CSng((m_pbFishingBlocks.Width - Me.m_sFirstColWidth) / Me.m_iTotalBlocks)

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
                    Me.FillBlock(iRow, i)
                Next
            Else
                ' Is column header clicked?
                If (iRow < 1) Then
                    ' #Yes: is row header clicked? If so: cannot fill block column
                    If (iCol < 1) Then Return
                    For i As Integer = 1 To m_BlockCells.GetLength(0) - 1
                        Me.FillBlock(i, iCol)
                    Next
                Else
                    Me.FillBlock(iRow, iCol)
                End If
            End If

            Me.m_pbFishingBlocks.Invalidate()

        End Sub

        Private Sub FillBlock(ByVal iRow As Integer, ByVal iCol As Integer)

            ' Sanity checks
            If (iCol <= Me.m_FPManager.ObjectiveParameters.BaseYear) Then Return

            If (iRow < 1) Then Return
            If (iRow > m_BlockCells.GetLength(0) - 1) Then Return

            ' Fill single block
            Me.m_FPManager.SearchBlocks(iRow).SearchBlocks(iCol) = Me.m_blockCodes.SelectedBlock
            Me.m_BlockCells(iRow, iCol) = Me.m_FPManager.SearchBlocks(iRow).SearchBlocks(iCol)

        End Sub

        Private Sub SetSeqColorCodes(ByVal startYear As Integer, ByVal endYear As Integer, ByVal yearPerBlock As Integer)

            If m_bIsFirstTimeLoaded Then Return
            If startYear > endYear Or startYear <= 0 Or endYear <= 0 Then Return
            If m_BlockCells Is Nothing Then Return
            If endYear > m_BlockCells.GetLength(1) - 1 Then endYear = m_BlockCells.GetLength(1) - 1

            Dim nColors As Integer = m_blockCodes.NumBlocks - 1
            Dim yearSegment As Integer = CInt(Math.Ceiling(m_iTotalBlocks / yearPerBlock))
            Dim totalClr As Integer = yearSegment * Me.m_uic.Core.nFleets

            If totalClr > nColors Then
                m_blockCodes.NumBlocks = totalClr + 1
            End If

            Dim cnt As Integer = 1
            Dim stepSize As Integer = CInt(Math.Floor(m_blockCodes.NumBlocks / totalClr))

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

            For iflt As Integer = 1 To Me.m_uic.Core.nFleets
                'the batch edit flag stops the searchblocks from sending out any messages
                m_FPManager.SearchBlocks(iflt).BatchEdit = True
                For iyr As Integer = 1 To Me.m_uic.Core.nEcosimYears
                    If iyr <= m_FPManager.ObjectiveParameters.BaseYear Then
                        'clear all blocks less than the baseyear
                        m_BlockCells(iflt, iyr) = 0
                    End If
                    m_FPManager.SearchBlocks(iflt).SearchBlocks(iyr) = m_BlockCells(iflt, iyr)
                Next
                m_FPManager.SearchBlocks(iflt).BatchEdit = False
            Next iflt

            m_pbFishingBlocks.Invalidate()

        End Sub

#End Region

    End Class

End Namespace

