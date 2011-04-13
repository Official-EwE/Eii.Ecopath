Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Parent grid cell for collapsing and expanding a collection of child rows.
    ''' </summary>
    ''' <remarks>
    ''' Cells of this type maintain a list of child rows that can be collapsed
    ''' or expanded via <see cref="EwEHierarchyGridCell.Expanded">Expanded</see>.
    ''' Add child rows via <see cref="EwEHierarchyGridCell.AddChildRow">AddChildRow</see>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class EwEHierarchyGridCell
        : Inherits EwECellBase

#Region " Private vars "

        Private m_bExpanded As Boolean = True
        Private m_viz As New cVisualizerEwECollapseExpandRowHeader()
        Private m_liChildRows As New List(Of Integer)

#End Region ' Private vars

        Public Sub New()
            MyBase.New("", GetType(String))
            Me.VisualModel = m_viz
            Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
            Me.Expanded = Me.m_bExpanded
        End Sub

        Public Property Expanded() As Boolean
            Get
                Return Me.m_bExpanded
            End Get
            Set(ByVal bExpanded As Boolean)
                ' Store flag
                Me.m_bExpanded = bExpanded
                ' Show/hide child rows
                Me.ShowHideChildren()
                ' Update viz
                Me.UpdateViz()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a child row to the hierarchy cell.
        ''' </summary>
        ''' <param name="iRow">Index of the row to add.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddChildRow(ByVal iRow As Integer)
            Dim iPos As Integer = 0
            ' Add in descending order
            While iPos < Me.m_liChildRows.Count()
                If (Me.m_liChildRows(iPos) < iRow) Then Exit While
                iPos += 1
            End While
            Me.m_liChildRows.Insert(iPos, iRow)

            Me.UpdateViz()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a child row from the hierarchy cell.
        ''' </summary>
        ''' <param name="iRow">Index of the row to remove.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveChildRow(ByVal iRow As Integer)
            Me.m_liChildRows.Remove(iRow)
            Me.UpdateViz()
        End Sub

        ''' <summary>
        ''' Get the number of child rows.
        ''' </summary>
        Public ReadOnly Property NumChildRows() As Integer
            Get
                Return Me.m_liChildRows.Count
            End Get
        End Property

        Private Sub ShowHideChildren()
            Dim g As GridVirtual = Me.Grid
            Dim ri As RowInfo = Nothing

            If g IsNot Nothing Then
                For Each iChild As Integer In Me.m_liChildRows
                    ri = g.Rows(iChild)
                    If Not Object.ReferenceEquals(ri, Nothing) Then
                        ri.Visible = Me.m_bExpanded
                    End If
                Next
            End If
        End Sub

        Public Overrides Sub OnClick(ByVal e As SourceGrid2.PositionEventArgs)
            ' MyBase.OnClick(e)
            Me.Expanded = Not Me.Expanded
        End Sub

        Public Overrides Property Style() As cStyleGuide.eStyleFlags
            Get
                Return cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Names
            End Get
            Set(ByVal value As cStyleGuide.eStyleFlags)
                ' No style
            End Set
        End Property

        Private Sub UpdateViz()
            Try
                ' Update visualizer
                If Me.NumChildRows = 0 Then
                    Me.m_viz.SetCollapsedState(cVisualizerEwECollapseExpandRowHeader.eCollapsedState.NoChildren)
                ElseIf Me.m_bExpanded Then
                    Me.m_viz.SetCollapsedState(cVisualizerEwECollapseExpandRowHeader.eCollapsedState.Expanded)
                Else
                    Me.m_viz.SetCollapsedState(cVisualizerEwECollapseExpandRowHeader.eCollapsedState.Collapsed)
                End If
            Catch ex As Exception

            End Try
        End Sub

    End Class

End Namespace
