#Region " Imports "

Option Strict On
Imports SourceGrid2
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports System.Reflection
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Database.cEwEDatabase

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class ucLinkGrid
    : Inherits EwEGrid

    Private m_data As cData = Nothing
    Private m_api As PropertyInfo() = Nothing
    Private m_links As cLink() = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext, _
                   ByVal data As cData, _
                   ByVal t As Type)

        'Sanity check
        Debug.Assert(GetType(cLink).IsAssignableFrom(t))

        Me.m_data = data
        Me.m_api = cPropertyInfoHelper.GetAllowedProperties(t)
        Me.m_links = Me.m_data.GetLinks(t)

        ' Go!
        Me.UIContext = uic

    End Sub

#Region " Internals "

    Protected Overrides Sub InitLayout()
        MyBase.InitLayout()

        Me.GridToolTipActive = True
        Me.Selection.SelectionMode = GridSelectionMode.Cell
        Me.Selection.ProtectReadOnly = True

        Me.FixedColumnWidths = False

    End Sub
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        ' Properties show as columns, links listed on rows

        Dim nCols As Integer = 1 + Me.m_api.Length
        Dim nRows As Integer = 1 + Me.m_links.Length
        Dim strHeader As String = ""

        Me.Redim(nRows, nCols)
        Me.FixedRows = 1
        Me.FixedColumns = 1
        Me.AutoSize = True

        ' Set column headers
        For iCol As Integer = 0 To nCols - 1
            If iCol = 0 Then
                ' Index row
                strHeader = "Link"
            Else
                strHeader = Me.m_api(iCol - 1).Name
            End If
            Me(0, iCol) = New EwERowHeaderCell(strHeader)
        Next iCol

        ' Add link rows
        For iRow As Integer = 0 To nRows - 1
            If (iRow > 0) Then
                Me.AddLink(Me.m_links(iRow - 1), iRow)
            End If
        Next

        Me.AutoSizeColumn(0, 140)

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.FixedRows = 1
        Me.FixedColumns = 1
        Me.AutoSize = True
        Me.AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="link"></param>
    ''' <param name="iRow"></param>
    ''' -----------------------------------------------------------------------
    Private Sub AddLink(ByVal link As cLink, ByVal iRow As Integer)
        For iCol As Integer = 0 To Me.ColumnsCount - 1
            Me.AddCell(link, iRow, iCol)
        Next
    End Sub

    Protected Sub AddCell(ByVal link As cLink, ByVal iRow As Integer, ByVal iCol As Integer)


        Dim cell As Cells.Real.Cell = Nothing

        If iCol = 0 Then
            cell = New EwERowHeaderCell(CStr(iRow))
        Else
            Try

                Dim pi As PropertyInfo = Me.m_api(iCol - 1)
                If GetType(cOOPStorable).IsAssignableFrom(pi.PropertyType) Then
                    Dim obj As cOOPStorable = DirectCast(pi.GetValue(link, Nothing), cOOPStorable)
                    Dim strLabel As String = ""
                    If (obj IsNot Nothing) Then strLabel = obj.ToString
                    cell = New EwECell(strLabel, GetType(String), ScientificInterfaceShared.Style.cStyleGuide.eStyleFlags.NotEditable)
                Else
                    cell = New cPropertyInfoCell(link, pi)
                End If
            Catch ex As Exception

            End Try

        End If
        Me(iRow, iCol) = cell

    End Sub

#End Region ' Internals

End Class
