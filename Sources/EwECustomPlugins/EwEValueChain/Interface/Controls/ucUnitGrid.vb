#Region " Imports "

Option Strict On
Imports SourceGrid2
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports System.Drawing
Imports System.Reflection
Imports System.ComponentModel
Imports System.Windows.Forms
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class ucUnitGrid
    : Inherits EwEGrid

    Private m_data As cData = Nothing
    Private m_unitType As cUnitFactory.eUnitType = cUnitFactory.eUnitType.Producer
    Private m_lUnits As List(Of cUnit) = Nothing

    Private m_dtProps As New Dictionary(Of String, List(Of PropertyInfo))
    Private m_api As PropertyInfo() = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="unitType"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal data As cData, ByVal unitType As cUnitFactory.eUnitType)

        Me.m_data = data
        Me.m_unitType = unitType

        ' Get all defined units of this type
        Me.m_lUnits = Me.m_data.GetUnits(Me.m_unitType)

        ' Get list of properties supported by this type
        Me.m_api = cPropertyInfoHelper.GetAllowedProperties(cUnitFactory.MapType(Me.m_unitType))

    End Sub

#Region " Events "

    Private Sub OnDisposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Me.m_lUnits = Nothing
    End Sub

#End Region ' Events

#Region " Internals "

    Protected Overrides Sub InitLayout()
        MyBase.InitLayout()

        Me.GridToolTipActive = True
        Me.Selection.SelectionMode = GridSelectionMode.Cell
        'Me.ContextMenuStyle = SourceGrid2.ContextMenuStyle.AutoSize Or _
        '                      SourceGrid2.ContextMenuStyle.CellContextMenu Or _
        '                      SourceGrid2.ContextMenuStyle.CopyPasteSelection
        'Me.Selection.AutoCopyPaste = False
        'Me.Selection.AutoClear = False
        Me.Selection.ProtectReadOnly = True

        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        Dim strHeader As String = ""
        Dim pi As PropertyInfo = Nothing
        Dim pd As PropertyDescriptor = Nothing

        Me.Redim(Me.m_api.Length + 1, Me.m_lUnits.Count + 1)
        Me.FixedRows = 1
        Me.FixedColumns = 1
        Me.AutoSize = True

        ' For every row
        For iRow As Integer = 0 To Me.RowsCount - 1
            If iRow = 0 Then
                strHeader = Me.m_unitType.ToString
            Else
                ' Get property info
                pi = Me.m_api(iRow - 1)
                ' Extract name
                strHeader = pi.Name
                ' Try to fing 'DisplayName' if available. This field is available through
                ' underlying PropertyDescriptor *sigh*
                pd = cPropertyConverter.FindOrigPropertyDescriptor(pi)
                ' Does pd exist?
                If pd IsNot Nothing Then
                    ' #Yes: has DisplayName?
                    If Not String.IsNullOrEmpty(pd.DisplayName) Then
                        ' #Yes: use it
                        strHeader = pd.DisplayName
                    End If
                End If
            End If

            ' Populate row
            Me(iRow, 0) = New EwERowHeaderCell(strHeader)

        Next iRow

        For iCol As Integer = 0 To Me.m_lUnits.Count - 1
            Me.AddUnit(Me.m_lUnits(iCol), iCol + 1)
        Next

        Me.AutoSizeColumn(0, 140)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="unit"></param>
    ''' <param name="iCol"></param>
    ''' -----------------------------------------------------------------------
    Private Sub AddUnit(ByVal unit As cUnit, ByVal iCol As Integer)
        For iRow As Integer = 0 To Me.RowsCount - 1
            Me.AddCell(unit, iRow, iCol)
        Next
    End Sub

    Protected Sub AddCell(ByVal unit As cUnit, ByVal iRow As Integer, ByVal iCol As Integer)
        Dim cell As Cells.Real.Cell = Nothing

        If iRow = 0 Then
            cell = New EwERowHeaderCell(CStr(iCol))
        Else
            cell = New cPropertyInfoCell(unit, Me.m_api(iRow - 1))
        End If
        Me(iRow, iCol) = cell

    End Sub

#End Region ' Internals

End Class
