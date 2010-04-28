#Region " Imports "

Option Strict On
Imports ScientificInterface.Ecosim
Imports ScientificInterface.Other

#End Region ' Imports

''' <summary>
''' Implementation of IBlockSelector for the MSE forms
''' </summary>
Public Class ucCVBlockSelector
    Implements IUIElement
    Implements IBlockSelector

#Region " Private vars "

    'ToDo_jb 8-march-2010 ucCVBlockSelector has no way to change the number of blocks
    'this may or may not be necessary to implement. For now it has a fixed number(10) of blocks hopefully this will be good enough

    Private m_uic As cUIContext
    Private m_numBlocks As Integer
    Private m_iBlockCur As Integer
    Private m_cvs() As Single

    Public Event OnBlockSelected(ByVal sender As IBlockSelector) Implements IBlockSelector.OnBlockSelected
    Public Event OnNumBlocksChanged(ByVal sender As IBlockSelector) Implements IBlockSelector.OnNumBlocksChanged

    Public Event onValueChanged(ByVal newValue As Single, ByVal Index As Integer) Implements IBlockSelector.onValueChanged

#End Region ' Private vars

#Region " Constructor "

    Public Sub New()
        Me.InitializeComponent()
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.setDefaultBlocks()
    End Sub

#End Region ' Constructor

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

            If (Me.m_uic IsNot Nothing) Then
                Me.m_gridSelector.Detach()
            End If

            Me.m_uic = value

            If (Me.m_uic IsNot Nothing) Then
                Me.m_gridSelector.Attach(Me)
                Me.m_gridSelector.UIContext = value
            End If
        End Set
    End Property

#End Region ' IUIElement implementation

#Region " IBlockSelector implementation "

    ''' <inheritdoc cref="IBlockSelector.BlockColor"/>
    Public ReadOnly Property BlockColor(ByVal iBlock As Integer) As System.Drawing.Color _
        Implements IBlockSelector.BlockColor
        Get
            Return Me.BlockColors(iBlock)
        End Get
    End Property

    ''' <inheritdoc cref="IBlockSelector.BlockColors"/>
    Public ReadOnly Property BlockColors() As System.Drawing.Color() _
        Implements IBlockSelector.BlockColors
        Get
            Dim lcolors As List(Of Color) = Me.m_uic.StyleGuide.GetEwE5ColorRamp(Me.NumBlocks + 2) ' +2 to spread the max colours a bit better
            Return lcolors.ToArray
        End Get
    End Property

    ''' <inheritdoc cref="IBlockSelector.NumBlocks"/>
    Public Property NumBlocks() As Integer _
        Implements IBlockSelector.NumBlocks
        Get
            Return m_numBlocks
        End Get

        Set(ByVal value As Integer)
            Me.m_numBlocks = value
            Try
                Me.setCVsToNBlocks()
                Me.m_gridSelector.Invalidate()
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".NumBlocks() Exception: " & ex.Message)
            End Try
        End Set

    End Property

    ''' <inheritdoc cref="IBlockSelector.SelectedBlock"/>
    Public Property SelectedBlock() As Integer _
        Implements IBlockSelector.SelectedBlock
        Get
            Return Me.m_iBlockCur
        End Get
        Set(ByVal value As Integer)
            If (value <> Me.m_iBlockCur) Then
                Me.m_iBlockCur = value
                Me.m_gridSelector.SelectedBlock = value
            End If
        End Set
    End Property

    ''' <inheritdoc cref="IBlockSelector.SelectedBlock"/>
    Public ReadOnly Property SelectedBlockColor() As System.Drawing.Color _
        Implements IBlockSelector.SelectedBlockColor
        Get
            Return Me.BlockColors(Me.m_iBlockCur)
        End Get
    End Property

    ''' <inheritdoc cref="IBlockSelector.BlocktoValue"/>
    Public Function BlocktoValue(ByVal iBlock As Integer) As Single _
        Implements IBlockSelector.BlocktoValue
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
    Public Function ValuetoBlock(ByVal cv As Single) As Integer _
        Implements IBlockSelector.ValuetoBlock
        Try
            'This could probable find an exact match and still work 
            'even if the user has edited the value of the currently selected block/cell
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
                'this will warn in the debug environment at least
                System.Console.WriteLine("Failed to find an exact match for the CV value " & cv.ToString & " the closest value will be used.")
            End If

            Return iDif

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ValuetoBlock() Exception: " & ex.Message)
        End Try

        Return Me.NumBlocks

    End Function

#End Region ' IBlockSelector implementation

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' ONE based array of block values
    ''' </summary>
    ''' -----------------------------------------------------------------------
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
            Me.m_gridSelector.RefreshContent()

        End Set

    End Property

#End Region ' Public interfaces

#Region " Events "

    Private Sub onGridValueChanged(ByVal newValue As Single, ByVal Index As Integer) _
        Handles m_gridSelector.OnValueChanged

        Try
            RaiseEvent onValueChanged(newValue, Index)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnGridSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) _
        Handles m_gridSelector.OnSelectionChanged
        Try
            '
            Me.SelectedBlock = Me.m_gridSelector.SelectedBlock
            Debug.Assert(Me.m_iBlockCur <= Me.NumBlocks, Me.ToString & ".OnSelectionChanged() selected block > total number of blocks!!!")
            RaiseEvent OnBlockSelected(Me)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".OnSelectionChanged() Exception: " & ex.Message)
        End Try
    End Sub

    'Private Sub ucCVBlockSelector_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize

    '    Try
    '        'for some reason that I can not figure out 
    '        'the grid will not automatically resize with the user control
    '        'so do it manually

    '        ' JS: this is because the grid somehow clears its own dock style attribute in InitStyle

    '        Dim control As Control = Me.Controls.Item(0)
    '        control.Left = 0
    '        control.Top = 0
    '        control.Size = Me.Size
    '    Catch ex As Exception
    '        Debug.Assert(False, "Manual resize of MSE Block Selector Grid Exception: " & ex.Message)
    '    End Try

    'End Sub

#End Region ' Events

#Region " Internals "

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

#End Region ' Internals

End Class
