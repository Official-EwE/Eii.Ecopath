#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE
Imports ScientificInterface.Other


#End Region


''' <summary>
''' EwEGrid that handles selection of color blocks for a ucCVBlockSelector
''' </summary>
''' <remarks>Color and values for the cells come from the ucCVBlockSelector(parent control)</remarks>
<CLSCompliant(False)> _
Public Class gridSelectColorBlock
    Inherits EwEGrid

    Private Const CV_ROW As Integer = 1
    Private m_parent As ucCVBlockSelector
    Private m_nblocks As Integer

    Private m_orgValue As Single

    Public Event onValueChanged(ByVal newValue As Single, ByVal Index As Integer)

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

        Me.Dock = DockStyle.None

        Me.Redim(3, Me.m_parent.NumBlocks + 1)
        Me(0, 0) = New EwEColumnHeaderCell("") 'dummy row
        Me(CV_ROW, 0) = New EwERowHeaderCell("CV")
        Me(2, 0) = New EwERowHeaderCell("Color")

        'hide the first row
        'sourcegrid will explode if you try to edit the first row so hide it and put the cv values in the second row
        Me.Rows(0).Height = 0

        Me.FixedColumns = 1
        Me.FixedRows = 1
        Me.HScroll = True

    End Sub

    Protected Overrides Sub FillData()

        If Me.m_parent Is Nothing Then Return

        'Color and values come from parent control
        Dim cvs() As Single = Me.m_parent.blockvalues

        For i As Integer = 1 To Me.m_parent.NumBlocks

            'hidden row
            Me(0, i) = New EwEColumnHeaderCell()
            Me(0, i).Value = cvs(i).ToString

            Me(CV_ROW, i) = New EwECell(cvs(i), cvs(i).GetType)

            Dim vm As New SourceGrid2.VisualModels.Common
            vm.BackColor = Me.m_parent.BlockColor(i)
            Dim cell As New Cell("", GetType(String))
            cell.VisualModel = vm
            Me(2, i) = cell

        Next

    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Private Sub gridSelectColorBlock_CellGotFocus(ByVal sender As Object, ByVal e As SourceGrid2.PositionCancelEventArgs) Handles Me.CellGotFocus

        If e.Position.Row <> CV_ROW Then Return

        Try
            ' Parse using UI default number formatting
            m_orgValue = Single.Parse(CStr(e.Cell.GetValue(e.Position)))
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & " CellGotFocus() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub gridSelectColorBlock_CellLostFocus(ByVal sender As Object, ByVal e As SourceGrid2.PositionCancelEventArgs) Handles Me.CellLostFocus

        If e.Position.Row <> CV_ROW Then Return

        Try
            Dim newvalue As Single
            ' Parse using UI default number formatting
            newvalue = Single.Parse(CStr(e.Cell.GetValue(e.Position)))
            Dim dif As Single = CSng(Math.Round(newvalue - Me.m_orgValue, 2))

            'has the cell been edited
            If dif <> 0.0 Then
                Dim col As Integer = e.Position.Column
                Me.m_parent.BlockValues(col) = newvalue
                RaiseEvent onValueChanged(newvalue, col)
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
