'==============================================================================
'
' $Log: EwEHierarchyGridCell.vb,v $
' Revision 1.2  2009/03/30 17:09:33  jeroens
' Split cells
'
'==============================================================================

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2

Namespace Controls.EwEGrid

    <CLSCompliant(False)> _
    Public Class EwEHierarchyGridCell
        : Inherits EwECellBase

        Private m_bExpanded As Boolean = True
        Private m_viz As New cVisualizerEwECollapseExpandRowHeader()
        Private m_liChildRows As New List(Of Integer)

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
                ' Update visualizer
                Me.m_viz.Expanded = bExpanded
                ' Show/hide child rows
                Me.ShowHideChildren()
            End Set
        End Property

        Public Sub AddChildRow(ByVal iRow As Integer)
            Dim iPos As Integer = 0
            ' Add in descending order
            While iPos < Me.m_liChildRows.Count()
                If (Me.m_liChildRows(iPos) < iRow) Then Exit While
            End While
            Me.m_liChildRows.Insert(iPos, iRow)
        End Sub

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

        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return StyleGuide.eStyleFlags.NotEditable Or StyleGuide.eStyleFlags.Names
            End Get
            Set(ByVal value As StyleGuide.eStyleFlags)
                ' No style
            End Set
        End Property

    End Class

End Namespace
