
Imports ScientificInterface.Other



''' <summary>
''' Implementation of Ecosim.IBlockSelector for the MSE forms
''' </summary>
''' <remarks></remarks>
Public Class ucCVBlockSelector
    Implements IUIElement
    Implements Ecosim.IBlockSelector

    'ToDo_jb 8-march-2010 ucCVBlockSelector has no way to change the number of blocks
    'this may or may not be necessary to implement. For now it has a fixed number(10) of blocks hopefully this will be good enough

    Private m_uic As cUIContext
    Private m_numBlocks As Integer
    Private m_curBlock As Integer
    Private m_cvs() As Single

    Public Event OnBlockSelected(ByVal sender As Ecosim.IBlockSelector) Implements Ecosim.IBlockSelector.OnBlockSelected
    Public Event OnNumBlocksChanged(ByVal sender As Ecosim.IBlockSelector) Implements Ecosim.IBlockSelector.OnNumBlocksChanged

    Public Event onValueChanged(ByVal newValue As Single, ByVal Index As Integer) Implements Ecosim.IBlockSelector.onValueChanged


#Region " IUIElement implementation "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cUIContext">UI context</see> to use.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Property UIContext() As cUIContext _
        Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Set(ByVal value As cUIContext)
            Me.m_uic = value

            Me.gridSelector.UIContext = value
            Me.gridSelector.BlockSelector = Me
            Me.gridSelector.Dock = DockStyle.None

        End Set
    End Property

#End Region ' IUIElement implementation

    'Private Sub onNumBlocksValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    '    Try

    '        Me.NumBlocks = Convert.ToInt32(Me.nudNumBlockCodes.Value)
    '        RaiseEvent OnNumBlocksChanged(Me)

    '    Catch ex As Exception
    '        Debug.Assert(False, Me.ToString & ".onNumbBlocksValueChanged() " & ex.Message)
    '    End Try

    'End Sub

    Public ReadOnly Property BlockColor(ByVal iBlock As Integer) As System.Drawing.Color Implements Ecosim.IBlockSelector.BlockColor
        Get
            Return Me.BlockColors(iBlock)
        End Get
    End Property

    Public ReadOnly Property BlockColors() As System.Drawing.Color() Implements Ecosim.IBlockSelector.BlockColors
        Get
            Dim lcolors As List(Of Color) = Me.m_uic.StyleGuide.GetEwE5ColorRamp(Me.NumBlocks)
            Return lcolors.ToArray
        End Get
    End Property

    Public Property NumBlocks() As Integer Implements Ecosim.IBlockSelector.NumBlocks
        Get
            Return m_numBlocks
        End Get

        Set(ByVal value As Integer)
            Me.m_numBlocks = value

            Try
                Me.setCVsToNBlocks()
                Me.gridSelector.Invalidate()
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".NumBlocks() Exception: " & ex.Message)
            End Try

        End Set

    End Property


    Public Property SelectedBlock() As Integer Implements Ecosim.IBlockSelector.SelectedBlock
        Get
            Return Me.m_curBlock
        End Get
        Set(ByVal value As Integer)

        End Set
    End Property

    Public ReadOnly Property SelectedBlockColor() As System.Drawing.Color Implements Ecosim.IBlockSelector.SelectedBlockColor
        Get
            Return Me.BlockColors(Me.m_curBlock)
        End Get
    End Property

    Public Property BlockValues() As Single()
        Get
            Return Me.m_cvs
        End Get

        Set(ByVal value As Single())
            Try
                Me.m_cvs = value
                Me.m_numBlocks = Me.m_cvs.Length - 1

            Catch ex As Exception
                'exception so set to defaults
                Me.setDefaultBlocks()
                Debug.Assert(False, Me.ToString & ".BlockValues() set to default " & ex.Message)
            End Try

            'populate the grid selector with the new values
            Me.gridSelector.populate()

        End Set

    End Property

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'sets the number of blocks and inits the cv values to defaults
        Me.setDefaultBlocks()

    End Sub

    Private Sub onGridValueChanged(ByVal newValue As Single, ByVal Index As Integer) Handles gridSelector.onValueChanged

        Try
            RaiseEvent onValueChanged(newValue, Index)
        Catch ex As Exception

        End Try
    End Sub


    Private Sub OnGridSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) Handles gridSelector.OnSelectionChanged
        Try
            '
            Me.m_curBlock = gridSelector.FocusCellPosition.Column
            Debug.Assert(Me.m_curBlock <= Me.NumBlocks, Me.ToString & ".OnSelectionChanged() selected block > total number of blocks!!!")
            RaiseEvent OnBlockSelected(Me)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".OnSelectionChanged() Exception: " & ex.Message)
        End Try


    End Sub

    Public Function BlocktoValue(ByVal iBlock As Integer) As Single Implements Ecosim.IBlockSelector.BlocktoValue
        Try
            Return Me.m_cvs(iBlock)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".BlocktoValue() iBlock out of bounds!")
        End Try

    End Function


    ''' <summary>
    ''' Convert a CV value into a BlockIndex
    ''' </summary>
    ''' <param name="cv">CV to search for</param>
    ''' <returns>Index of the CV in the IBlockSelector</returns>
    ''' <remarks>Finds the closest matching. </remarks>
    Public Function ValuetoBlock(ByVal cv As Single) As Integer Implements Ecosim.IBlockSelector.ValuetoBlock
        Try
            'This could probable find an exact match and still work 
            'ven if the user has edited the value of the currently selected block/cell
            Dim i As Integer
            'closest match
            Dim dif As Single
            Dim minDif As Single = Single.MaxValue
            Dim iDif As Integer
            For i = 1 To Me.NumBlocks
                dif = Math.Abs(cv - Me.m_cvs(i))
                If dif < minDif Then
                    minDif = dif
                    iDif = i
                End If
            Next

            'Warn the user if minDif is not zero not an exact match
            If minDif <> 0 Then
                'if something has changed in the control or the data this could happen
                'this will warn in the debug enviroment at least
                System.Console.WriteLine("Failed to find an exact match for the CV value " & cv.ToString & " the closest value will be used.")
            End If

            Return iDif

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ValuetoBlock() Exception: " & ex.Message)
        End Try

        Return Me.NumBlocks

    End Function


    Private Sub setDefaultBlocks()
        Me.NumBlocks = 10
        Me.setCVsToNBlocks()
    End Sub


    Private Sub setCVsToNBlocks()
        ReDim Me.m_cvs(Me.NumBlocks)
        For i As Integer = 1 To Me.NumBlocks
            Me.m_cvs(i) = CSng(Math.Round((i - 1) / Me.NumBlocks, 2))
        Next
    End Sub

    Private Sub ucCVBlockSelector_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize

        Try
            'for some reason that I can not figure out 
            'the grid will not automatically resize with the user control
            'so do it manually
            Dim control As Control = Me.Controls.Item(0)
            control.Left = 0
            control.Top = 0
            control.Size = Me.Size
        Catch ex As Exception
            Debug.Assert(False, "Manual resize of MSE Block Selector Grid Exception: " & ex.Message)
        End Try

    End Sub
End Class
