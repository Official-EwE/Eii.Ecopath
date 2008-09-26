'==============================================================================
'
' $Log: EwEHierarchyGridCell.vb,v $
' Revision 1.1  2008/09/26 07:31:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:08  jeroens
' Separated from Scientific Interface
'
' Revision 1.4  2008/05/29 22:23:01  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.3  2008/01/11 12:27:08  jeroens
' Child rows tracked by row index, no longer by simple child row count because child rows may not be placed in consecutive order
'
' Revision 1.2  2007/05/31 13:11:23  jeroens
' * Renamed StyleGuide StyleFlags to eStyleFlags
'
' Revision 1.1  2006/10/18 15:51:28  jeroens
' Initial version
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

#Region " Class PropertyRowHeaderParentCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' PropertyRowHeaderParentCell implements a PropertyRowHeaderCell rendered as an EwE name field.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class PropertyRowHeaderParentCell
        : Inherits PropertyRowHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cVisualizerEwEParentRowHeader()

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop">cProperty to attach to the cell</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            ' Call baseclass constructor
            MyBase.New(prop)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
        End Sub

#End Region ' Construction 

    End Class

#End Region ' Class PropertyRowHeaderParentCell 

#Region " Class PropertyRowHeaderChildCell "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' PropertyRowHeaderChildCell implements a PropertyRowHeaderCell rendered as an EwE name field.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class PropertyRowHeaderChildCell
        : Inherits PropertyRowHeaderCell

        ''' <summary>One visualizer for all cells</summary>
        Private Shared g_visualizer As New cVisualizerEwEChildRowHeader()

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces</param>
        ''' <param name="SourceSec">An optional secundary index in the VarName, or <see cref="cCore.NULL_VALUE">cCore.NULL_VALUE</see> when irrelevant</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
            Me.New(cPropertyManager.GetInstance().GetProperty(Source, VarName, SourceSec))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="prop">cProperty to attach to the cell</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty)
            ' Call baseclass constructor
            MyBase.New(prop)
            ' Set shared visualizer
            Me.VisualModel = g_visualizer
            Me.DataModel.EditableMode = SourceGrid2.EditableMode.None
        End Sub

#End Region ' Construction 

    End Class

#End Region ' Class PropertyRowHeaderChildCell

End Namespace
