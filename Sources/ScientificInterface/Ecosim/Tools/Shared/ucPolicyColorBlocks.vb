#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports EwEUtils.Core

#End Region

Namespace Ecosim

#Region "Color Blocks User control"


    ''' =======================================================================
    ''' <summary>
    ''' Control implementing the policy blocks sketch user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class ucPolicyColorBlocks
        Implements IUIElement

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_clrCurrent As Color
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

        Private m_DataSource As IPolicyColorBlockDataSource
        Private m_bInit As Boolean

        Private m_blockCodes As IBlockSelector

#End Region ' Private vars

#Region "Public Methods and Properties"

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Me.Detach()
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        ''' <summary>
        ''' Attach a data source <see cref="IPolicyColorBlockDataSource">IPolicyColorBlockDataSource</see> and a block selector control <see cref="IBlockSelector">IBlockSelector</see> to the main PolicyColorBlock control 
        ''' </summary>
        ''' <param name="DataSource">Implementation of IPolicyColorBlockDataSource</param>
        ''' <param name="BlockCodes">Implementation of IBlockSelector</param>
        ''' <remarks>PolicyColorBlocks can be attached to different data sources and block selectors</remarks>
        Public Sub Attach(ByVal DataSource As IPolicyColorBlockDataSource, ByVal BlockCodes As IBlockSelector)

            If Me.m_bInit Then Me.Detach()

            Me.m_DataSource = DataSource
            Me.m_blockCodes = BlockCodes
            Me.m_blockCodes.UIContext = Me.UIContext

            Try

                Dim selector As Control = DirectCast(Me.m_blockCodes, Control)
                selector.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
                Me.tlpMain.Controls.Add(selector, 0, 1)

                'datasource decides if the control panel is visible
                If Not Me.m_DataSource.isControlPanelVisible Then
                    'the control panel is setup to be shown so we only have to turn it off
                    Me.pnlControls.Visible = False 'don't really have to do this
                    Me.pnlControls.Enabled = False 'or this
                    Me.tlpMain.ColumnStyles(1).Width = 0 'just hiding the column would work
                End If

                AddHandler BlockCodes.onValueChanged, AddressOf onCVValuesChanged

            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".Attach() Exception: " & ex.Message)
            End Try

            Me.m_DataSource.Attach(Me.m_blockCodes)

            Me.m_PropBaseYear = DirectCast(Me.m_uic.PropertyManager.GetProperty(Me.m_uic.Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchBaseYear), cIntegerProperty)
            AddHandler Me.m_PropBaseYear.PropertyChanged, AddressOf OnPropChanged

            Me.m_PropEcosimNYears = DirectCast(Me.m_uic.PropertyManager.GetProperty(Me.m_uic.Core.EcoSimModelParameters, eVarNameFlags.EcoSimNYears), cIntegerProperty)
            AddHandler Me.m_PropEcosimNYears.PropertyChanged, AddressOf OnPropChanged

            Me.m_bInit = True
            Me.UpdateControls()

        End Sub

        Public Sub Detach()

            If (Me.m_bInit) Then

                RemoveHandler Me.m_blockCodes.onValueChanged, AddressOf Me.onCVValuesChanged

                If (Me.m_PropBaseYear IsNot Nothing) Then
                    RemoveHandler Me.m_PropBaseYear.PropertyChanged, AddressOf OnPropChanged
                    Me.m_PropBaseYear = Nothing

                    RemoveHandler Me.m_PropEcosimNYears.PropertyChanged, AddressOf OnPropChanged
                    Me.m_PropEcosimNYears = Nothing

                End If
                Me.m_DataSource = Nothing
                Me.m_blockCodes = Nothing
            End If
            Me.UIContext = Nothing

        End Sub

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property


        ''' <summary>
        ''' Color of the currently selected block
        ''' </summary>
        Public Property CurColor() As Color
            Get
                Return m_clrCurrent
            End Get
            Set(ByVal value As Color)
                m_clrCurrent = value
            End Set
        End Property

        ''' <summary>
        ''' Implementation of IBlockSelector
        ''' </summary>
        Public Property ParmBlockCodes() As IBlockSelector
            Get
                Return m_blockCodes
            End Get
            Set(ByVal value As IBlockSelector)
                Me.m_blockCodes = value
            End Set
        End Property

#End Region

#Region " Events handlers "

        Private Sub btnSetEveryGear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSetGear.Click

            Me.SetSeqColorCodes(2, Me.m_DataSource.TotalBlocks, CInt(Me.m_nudNumYearsPerBlock.Value))

        End Sub

        Private Sub nupSeqStartYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudSeqStartYear.ValueChanged

            Dim startYear As Integer = CInt(m_nudSeqStartYear.Value)
            Dim endYear As Integer = CInt(m_nudSeqEndYear.Value)

            Me.m_nudSeqEndYear.Minimum = Me.m_nudSeqStartYear.Value

            If Me.m_bInit Then
                Me.SetSeqColorCodes(startYear, endYear, Me.m_DataSource.TotalBlocks)
            End If
        End Sub

        Private Sub nupSeqEndYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_nudSeqEndYear.ValueChanged

            Dim startYear As Integer = CInt(m_nudSeqStartYear.Value)
            Dim endYear As Integer = CInt(m_nudSeqEndYear.Value)

            Me.m_nudSeqStartYear.Maximum = Me.m_nudSeqEndYear.Value

            If Me.m_bInit Then
                Me.SetSeqColorCodes(startYear, endYear, Me.m_DataSource.TotalBlocks)
            End If

        End Sub

        Private Sub pbFishingBlocks_Paint(ByVal sender As System.Object, ByVal e As PaintEventArgs) _
            Handles m_pbFishingBlocks.Paint

            If (Me.UIContext Is Nothing) Then Return

            Try
                Me.CalcParams(e.Graphics)
                Me.DrawRowCols(e.Graphics)
            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".Paint() Exception: " & ex.Message)
            End Try

        End Sub

        Private Sub pbFishingBlocks_MouseDown(ByVal sender As System.Object, ByVal e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseDown

            Me.m_bIsSketching = True
            Me.m_DataSource.BatchEdit = True

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
            Me.m_DataSource.BatchEdit = False

        End Sub

        Private Sub pbFishingBlocks_MouseClick(ByVal sender As System.Object, ByVal e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseClick

            Me.ProcessCellClick(e.X, e.Y)

        End Sub

        Private Sub PolicyColorBlocks_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.SizeChanged

            ' Redraw the blocks
            Me.m_pbFishingBlocks.Invalidate()

        End Sub

#End Region ' Events handlers

#Region " Callbacks "

        Private Sub OnPropChanged(ByVal p As cProperty, ByVal cf As cProperty.eChangeFlags)

            'right now if anything changed just reload
            Me.UpdateControls()
            Me.m_pbFishingBlocks.Invalidate()

        End Sub

        ''' <summary>
        ''' Values in the grid selector have changed update the datasource
        ''' </summary>
        ''' <param name="newValue"></param>
        ''' <param name="Index"></param>
        ''' <remarks>Only the CV grid selector sends out this event</remarks>
        Private Sub onCVValuesChanged(ByVal newValue As Single, ByVal Index As Integer)
            Try
                Me.m_DataSource.Update()
            Catch ex As Exception

            End Try
        End Sub



#End Region ' Callbacks

#Region "Private methods"

        Private Sub UpdateControls()

            Me.m_DataSource.Init()

            m_nudSeqEndYear.Maximum = Me.m_DataSource.TotalBlocks
            m_nudNumYearsPerBlock.Maximum = Me.m_DataSource.TotalBlocks

            m_bIsSketching = False

            If m_clrCurrent = Nothing Then
                m_clrCurrent = Color.Green
            End If

            m_nudNumYearsPerBlock.Value = CDec(Me.m_DataSource.TotalBlocks)
            m_nudSeqStartYear.Value = CDec(Math.Min(2, Me.m_DataSource.TotalBlocks))
            m_nudSeqEndYear.Value = CDec(Me.m_DataSource.TotalBlocks)
            m_bIsFirstTimeLoaded = False

        End Sub


        Private Sub DrawRowCols(ByRef g As Graphics)

            If Not Me.m_bInit Then Return

            Try

                'Draw row lines
                For i As Integer = 1 To m_iRows - 1
                    Dim yPos As Single = 0 + i * m_sRowHeight
                    g.DrawLine(Pens.Gray, 0, yPos, m_pbFishingBlocks.Width, yPos)
                    g.DrawString(Me.m_DataSource.RowLabel(i), m_pbFishingBlocks.Font, Brushes.Black, 1, yPos + 1)
                Next

                g.DrawLine(Pens.Gray, m_sFirstColWidth, m_pbFishingBlocks.Top, m_sFirstColWidth, m_pbFishingBlocks.Bottom)

                For j As Integer = 1 To m_iCols
                    Dim xPos As Single = m_sFirstColWidth + (j - 1) * m_sColWidth
                    g.DrawLine(Pens.Gray, xPos, 0, xPos, m_pbFishingBlocks.Height)
                    'jb why are we just printing the single digit numbers???
                    'Dim txt As String = (j Mod 10).ToString
                    Dim txt As String = j.ToString
                    g.DrawString(txt, m_pbFishingBlocks.Font, Brushes.Black, xPos + 1, 1)
                    'If j < 3 Then Console.WriteLine("Col {0} xPos = {1}", j, xPos)
                Next

                For i As Integer = 1 To m_iRows - 1
                    For j As Integer = 1 To m_iCols - 1
                        Dim yPos As Single = i * m_sRowHeight
                        Dim xPos As Single = m_sFirstColWidth + (j - 1) * m_sColWidth
                        Dim tmpBrush As New SolidBrush(m_blockCodes.BlockColor(Me.m_DataSource.BlockCells(i, j)))
                        g.FillRectangle(tmpBrush, New RectangleF(xPos, yPos, m_sColWidth, m_sRowHeight))
                        'draw the value in the cell
                        'g.DrawString(Me.m_DataSource.BlockCells(i, j).ToString, m_pbFishingBlocks.Font, Brushes.Black, xPos, yPos)
                    Next

                Next

            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".DrawRowCols() Exception: " & ex.Message)
                Throw New ApplicationException(Me.ToString & ".DrawRowCols() Exception: " & ex.Message)
            End Try



        End Sub

        Private Sub CalcParams(ByRef g As Graphics)

            If Not Me.m_bInit Then Return
            Try
                Me.m_iRows = Me.m_DataSource.nRows + 1
                Me.m_sRowHeight = CSng(m_pbFishingBlocks.Height / Me.m_iRows)

                Dim sLenMax As Single = -1
                For i As Integer = 0 To Me.m_DataSource.nRows - 1
                    Dim tmpWidth As Single = g.MeasureString(Me.m_DataSource.RowLabel(i + 1), m_pbFishingBlocks.Font).Width
                    If sLenMax < tmpWidth Then sLenMax = tmpWidth
                Next

                'First column line 
                Me.m_sFirstColWidth = sLenMax + 10
                Me.m_iCols = Me.m_DataSource.TotalBlocks + 1
                Me.m_sColWidth = CSng((m_pbFishingBlocks.Width - Me.m_sFirstColWidth) / Me.m_DataSource.TotalBlocks)

            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".DrawRowCols() Exception: " & ex.Message)
                Throw New ApplicationException(Me.ToString & ".DrawRowCols() Exception: " & ex.Message)
            End Try

        End Sub

        Private Sub ProcessCellClick(ByVal x As Integer, ByVal y As Integer)

            If Not Me.m_bInit Then Return

            Dim iRow As Integer = CInt(Math.Floor(y / m_sRowHeight))
            Dim iCol As Integer = CInt(Math.Floor((x - m_sFirstColWidth) / m_sColWidth) + 1)

            If iRow < 0 Or iRow > m_iRows - 1 Then Return
            If iCol > m_iCols - 1 Then Return

            'BatchEdits have been set before this is called

            ' Is row header clicked?
            If (iCol < 1) Then

                ' #Yes: is column header clicked? If so: cannot fill block row
                If iRow < 1 Then Return

                For i As Integer = 1 To Me.m_DataSource.BlockCells.GetLength(1) - 1
                    Me.FillBlock(iRow, i)
                Next
            Else
                ' Is column header clicked?
                If (iRow < 1) Then
                    ' #Yes: is row header clicked? If so: cannot fill block column
                    If (iCol < 1) Then Return
                    For i As Integer = 1 To Me.m_DataSource.BlockCells.GetLength(0) - 1
                        Me.FillBlock(i, iCol)
                    Next
                Else
                    Me.FillBlock(iRow, iCol)
                End If
            End If

            Me.m_pbFishingBlocks.Invalidate()

        End Sub

        Private Sub FillBlock(ByVal iRow As Integer, ByVal iCol As Integer)

            If Not Me.m_bInit Then Return
            Me.m_DataSource.FillBlock(iRow, iCol)

        End Sub

        Private Sub SetSeqColorCodes(ByVal startYear As Integer, ByVal endYear As Integer, ByVal yearPerBlock As Integer)

            If Not Me.m_bInit Then Return
            If m_bIsFirstTimeLoaded Then Return

            Me.m_DataSource.SetSeqColorCodes(startYear, endYear, yearPerBlock)
            m_pbFishingBlocks.Invalidate()

        End Sub

#End Region

    End Class

#End Region

#Region "Interface for Datasource (IPolicyColorBlockDataSource)"

    ''' <summary>
    ''' Interface for the core data that is used to populate a ucPolicyColorBlocks control
    ''' </summary>
    ''' <remarks>Different types of core data use the ucPolicyColorBlocks differently this allows the core data to all look the same to the control. </remarks>
    Public Interface IPolicyColorBlockDataSource
        ''' <summary>
        ''' Values used to color the grid
        ''' </summary>
        ''' <value>2d matrix of integers dimensiond be TotalBlocks and nRows</value>
        ''' <remarks>The Values in the grid are maintained by the datasource</remarks>
        ReadOnly Property BlockCells() As Integer(,)

        ''' <summary>
        ''' Total number of columns/year in the grid/data.
        ''' </summary>
        ''' <value>Integer</value>
        ''' <remarks>This is the X axis of the grid. Number of years from the data source.</remarks>
        ReadOnly Property TotalBlocks() As Integer
        ''' <summary>
        ''' Total number of rows in the grid
        ''' </summary>
        ''' <remarks>In the data source this is the number of group/fleets</remarks>
        ReadOnly Property nRows() As Integer

        ''' <summary>
        ''' Labels for the rows
        ''' </summary>
        ''' <param name="iRow">One based index of the row</param>
        ''' <remarks>Group names or Fleet names depending on the data source</remarks>
        ReadOnly Property RowLabel(ByVal iRow As Integer) As String

        ''' <summary>
        ''' Turns Off/On core updates while adding values to core data
        ''' </summary>
        ''' <remarks>True in batch edit mode and the core updated. False updates the core with the edits.</remarks>
        Property BatchEdit() As Boolean

        ''' <summary>
        ''' Does this data source use the Control panel/block-sequence selector
        ''' </summary>
        ReadOnly Property isControlPanelVisible() As Boolean


        ''' <summary>
        ''' Attach an <see cref="IBlockSelector">IBlockSelector</see> object to this data source 
        ''' </summary>
        ''' <param name="Blocks">implementation of IBlockSelector</param>
        ''' <remarks>The data source need to listen to the Block selector and set the number of blocks and cv values</remarks>
        Sub Attach(ByVal Blocks As IBlockSelector)

        ''' <summary>
        ''' Init the data source
        ''' </summary>
        Sub Init()

        ''' <summary>
        ''' Fills the BlockCells with the currently selected block and updates the core values
        ''' </summary>
        ''' <param name="iRow">Row</param>
        ''' <param name="iCol">Column</param>
        Sub FillBlock(ByVal iRow As Integer, ByVal iCol As Integer)

        ''' <summary>
        ''' Sets a sequence of BlockCells
        ''' </summary>
        ''' <param name="startYear">Year for the first block to fill</param>
        ''' <param name="endYear">End of the sequence</param>
        ''' <param name="yearPerBlock">Number of years per unique block</param>
        Sub SetSeqColorCodes(ByVal startYear As Integer, ByVal endYear As Integer, ByVal yearPerBlock As Integer)

        ''' <summary>
        ''' Update the core data 
        ''' </summary>
        ''' <remarks>Use when the BlockSelector has change values of the blocks </remarks>
        Sub Update()
    End Interface

#End Region

End Namespace

