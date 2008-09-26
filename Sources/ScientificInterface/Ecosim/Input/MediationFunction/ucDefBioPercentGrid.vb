'==============================================================================
'
' $Log: ucDefBioPercentGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:38  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/09/19 14:14:38  jeroens
' Fixed issue 496
'
' Revision 1.6  2008/08/11 16:13:59  jeroens
' Generalized EndEditHandler
'
' Revision 1.5  2008/08/02 03:04:21  jeroens
' Renamed resources
'
' Revision 1.4  2008/06/02 00:01:42  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.3  2008/04/07 02:31:22  jeroens
' Cleaning up resources
'
' Revision 1.2  2007/11/15 15:10:10  jeroens
' * Woops
'
' Revision 1.1  2007/11/15 15:01:00  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwECore
Imports SourceGrid2

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class ucDefBioPercentGrid
        Inherits EwEGrid

        ''' <summary></summary>
        Private m_core As cCore = Nothing
        ''' <summary></summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Enum eColumnTypes As Integer
            Name = 0
            Weight
        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create the grid
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()

            MyBase.New()
            Me.m_core = cCore.GetInstance()

        End Sub

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="sWeight"></param>
        ''' -------------------------------------------------------------------
        Public Event OnWeightChanged(ByVal obj As cCoreInputOutputBase, ByVal sWeight As Single)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="sWeight"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Add(ByVal obj As cCoreInputOutputBase, ByVal sWeight As Single) As Boolean
            If (Me.FindRow(obj) <> -1) Then Return False

            Dim iRow As Integer = Me.AddRow()
            Dim ewec As EwECell = Nothing

            ewec = New EwECell(obj.Name, GetType(String))
            ewec.Style = StyleGuide.eStyleFlags.Names Or StyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.Name) = ewec

            Me(iRow, eColumnTypes.Weight) = New Cells.Real.Cell(sWeight, GetType(Single))
            Me(iRow, eColumnTypes.Weight).Behaviors.Add(m_bm)

            Me.RowItem(iRow) = obj

            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Remove(ByVal obj As cCoreInputOutputBase) As Boolean
            Dim iRow As Integer = Me.FindRow(obj)
            If (iRow = -1) Then Return False
            Me.Rows.Remove(iRow)
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Find(ByVal obj As cCoreInputOutputBase) As Boolean
            Return (Me.FindRow(obj) > -1)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function SelectedItem() As cCoreInputOutputBase
            Return Me.RowItem(Me.Selection.GetRange().Start.Row)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Hack and slash results to feed to graph control, ugh
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function Data() As Dictionary(Of cCoreInputOutputBase, Single)
            Dim dt As New Dictionary(Of cCoreInputOutputBase, Single)
            Dim obj As cCoreInputOutputBase = Nothing
            Dim sWeight As Single = 0.0

            For iRow As Integer = 1 To Me.RowsCount - 1
                obj = Me.RowItem(iRow)
                sWeight = CSng(Me(iRow, eColumnTypes.Weight).Value)
                dt.Add(obj, sWeight)
            Next

            Return dt
        End Function

#End Region ' Public interfaces

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function FindRow(ByVal obj As cCoreInputOutputBase) As Integer
            Dim objTest As cCoreInputOutputBase = Nothing
            For iRow As Integer = 1 To Me.RowsCount - 1
                objTest = Me.RowItem(iRow)
                If Object.ReferenceEquals(obj, objTest) Then Return iRow
            Next
            Return -1
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' -------------------------------------------------------------------
        Private Property RowItem(ByVal iRow As Integer) As cCoreInputOutputBase
            Get
                If (iRow <= 0) Then Return Nothing
                Return DirectCast(Me(iRow, eColumnTypes.Name).Tag, cCoreInputOutputBase)
            End Get
            Set(ByVal value As cCoreInputOutputBase)
                Me(iRow, eColumnTypes.Name).Tag = value
            End Set
        End Property

        Private Sub ucDefBioPercentGrid_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
            If Me.ColumnsCount = [Enum].GetValues(GetType(eColumnTypes)).Length Then
                Me.Columns(eColumnTypes.Name).Width = Math.Max(150, Me.ClientRectangle.Width - 150)
            End If
        End Sub

#End Region ' Internals "

#Region " Grid overrides "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the grid.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Redim columns
            Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

            ' Name cell, editable this time
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_NAME)
            ' Weight cell
            Me(0, eColumnTypes.Weight) = New EwEColumnHeaderCell(My.Resources.HEADER_RELATIVEWEIGHT)

            Me.FixedColumns = 2
            Me.Dock = DockStyle.Fill

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub FillData()
            ' HAH!
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.Columns(eColumnTypes.Weight).Width = 150
            Me.Columns(eColumnTypes.Name).Width = Math.Max(150, Me.ClientRectangle.Width - 150)
            Me.FixedColumns = 1
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <returns>
        ''' True if the value change is allowed, False to block the value change.
        ''' </returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

            If (p.Column = eColumnTypes.Weight) Then
                RaiseEvent OnWeightChanged(Me.RowItem(p.Row), Single.Parse(CStr(cell.GetValue(p))))
            End If
            Return True

        End Function

#End Region ' Grid overrides

    End Class

End Namespace
