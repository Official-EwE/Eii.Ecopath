#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE
Imports ScientificInterface.Other
Imports SourceGrid2


#End Region


''' <summary>
''' EwEGrid that handles selection of color blocks for a ucCVBlockSelector
''' </summary>
''' <remarks>Color and values for the cells come from the ucCVBlockSelector(parent control)</remarks>
<CLSCompliant(False)> _
Public Class gridSelectColorBlock
    Inherits EwEGrid

#Region " Helper class "

    ''' =======================================================================
    ''' <summary>
    ''' Helper class for drawing a colored CV cell.
    ''' </summary>
    ''' <remarks>
    ''' This class is hard-wired to gridSelectColorBlock.
    ''' </remarks>
    ''' =======================================================================
    Private Class cCVCellVisualizer
        Inherits VisualModels.Common

        Private m_parent As gridSelectColorBlock = Nothing

        Public Sub New(ByVal parent As gridSelectColorBlock)
            Debug.Assert(parent IsNot Nothing)
            Me.m_parent = parent
        End Sub

        Protected Overrides Sub DrawCell_Background(ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, _
                                                    ByVal p_CellPosition As SourceGrid2.Position, _
                                                    ByVal e As PaintEventArgs, _
                                                    ByVal p_ClientRectangle As System.Drawing.Rectangle, _
                                                    ByVal p_Status As SourceGrid2.DrawCellStatus)

            Me.BackColor = Me.m_parent.BlockColor(p_CellPosition.Column - 1)
            MyBase.DrawCell_Background(p_Cell, p_CellPosition, e, p_ClientRectangle, DrawCellStatus.Normal)

        End Sub

        Protected Overrides Sub DrawCell_Border(ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, _
                                                ByVal p_CellPosition As SourceGrid2.Position, _
                                                ByVal e As PaintEventArgs, _
                                                ByVal p_ClientRectangle As Rectangle, _
                                                ByVal p_Status As SourceGrid2.DrawCellStatus)

            Dim border As Border = Nothing

            If (p_CellPosition.Column - 1 = Me.m_parent.SelectedBlock) Then
                border = New Border(Me.m_parent.HighlightColor, 3)
                Me.Border = New RectangleBorder(border)
            Else
                Me.Border = Nothing
            End If

            MyBase.DrawCell_Border(p_Cell, p_CellPosition, e, p_ClientRectangle, DrawCellStatus.Normal)

        End Sub

    End Class

#End Region ' Helper class

#Region " Private vars "

    Private m_parent As ucCVBlockSelector = Nothing
    Private m_vm As cCVCellVisualizer = Nothing
    Private m_orgValue As Single = cCore.NULL_VALUE
    ''' <summary>Number of blocks.</summary>
    Private m_nblocks As Integer = 0
    ''' <summary>Selected block.</summary>
    Private m_iBlock As Integer = 0

    Public Event OnValueChanged(ByVal newValue As Single, ByVal Index As Integer)

#End Region ' Private vars

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Constructor

    Public WriteOnly Property BlockSelector() As ucCVBlockSelector
        Set(ByVal value As ucCVBlockSelector)
            Me.m_parent = value
            Me.InitStyle()
            Me.FillData()
        End Set
    End Property

    Protected Overrides Sub InitStyle()

        ' ToDo: localize this method

        If Me.m_parent Is Nothing Then Return

        MyBase.InitStyle()

        Me.Redim(2, Me.m_parent.NumBlocks + 1)
        Me(0, 0) = New EwERowHeaderCell("CV")
        Me(1, 0) = New EwERowHeaderCell("Color")

        'hide the first row
        ' JB: sourcegrid will explode if you try to edit the first row so hide it and put the cv values in the second row
        ' JS: this is because the first row is set as fixed. Turn this off and you're ok
        'Me.Rows(0).Height = 0
        Me.FixedRows = 0

        Me.FixedColumns = 1
        Me.HScroll = True

    End Sub

    Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
        Return DockStyle.None
    End Function

    Public Property SelectedBlock() As Integer
        Get
            Return Me.m_iBlock
        End Get
        Set(ByVal value As Integer)
            If (value <> Me.m_iBlock) Then
                Me.m_iBlock = value
                Me.InvalidateCells()
            End If
        End Set
    End Property

    Protected Overrides Sub FillData()

        If (Me.m_parent Is Nothing) Then Return
        If (Me.StyleGuide Is Nothing) Then Return

        If (Me.m_vm Is Nothing) Then
            Me.m_vm = New cCVCellVisualizer(Me)
        End If

        'Color and values come from parent control
        Dim cvs() As Single = Me.m_parent.BlockValues

        For i As Integer = 1 To Me.m_parent.NumBlocks

            Me(0, i) = New EwECell(cvs(i), cvs(i).GetType)

            Dim cell As New Cell("", GetType(String))
            cell.VisualModel = Me.m_vm
            cell.EditableMode = EditableMode.None
            cell.EnableEdit = False
            Me(1, i) = cell

        Next

    End Sub

    Protected ReadOnly Property BlockColor(ByVal i As Integer) As Color
        Get
            If (i < Me.m_parent.NumBlocks) Then
                Return Me.m_parent.BlockColor(i)
            End If
            Return Color.White
        End Get
    End Property

    Protected ReadOnly Property HighlightColor() As Color
        Get
            If (Me.StyleGuide IsNot Nothing) Then
                Return Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
            End If
            Return Color.Orange
        End Get
    End Property

    Private Sub gridSelectColorBlock_CellGotFocus(ByVal sender As Object, ByVal e As SourceGrid2.PositionCancelEventArgs) _
        Handles Me.CellGotFocus

        Try
            ' Parse using UI default number formatting
            Me.m_orgValue = Single.Parse(CStr(Me(0, e.Position.Column).Value))
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & " CellGotFocus() Exception: " & ex.Message)
        End Try
        ' Set selected block
        Me.SelectedBlock = e.Position.Column - 1

    End Sub

    Private Sub gridSelectColorBlock_CellLostFocus(ByVal sender As Object, ByVal e As SourceGrid2.PositionCancelEventArgs) _
        Handles Me.CellLostFocus

        Try
            Dim newvalue As Single
            ' Parse using UI default number formatting
            newvalue = Single.Parse(CStr(Me(0, e.Position.Column).Value))
            Dim dif As Single = CSng(Math.Round(newvalue - Me.m_orgValue, 2))

            'has the cell been edited
            If dif <> 0.0 Then
                Dim col As Integer = e.Position.Column
                Me.m_parent.BlockValues(col) = newvalue
                RaiseEvent OnValueChanged(newvalue, col)
            End If

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & " CellLostFocus() Exception: " & ex.Message)
        End Try

    End Sub

    Public Sub populate()
        Try
            Me.InitStyle()
            Me.FillData()
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

End Class
