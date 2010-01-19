#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Reflection
Imports System.ComponentModel
Imports SourceGrid2
Imports ScientificInterfaceShared.Controls.EwEGrid

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
    Private m_dtProps As New Dictionary(Of String, List(Of PropertyInfo))
    Private m_pi As PropertyInfo()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="unitType"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal data As cData, ByVal unitType As cUnitFactory.eUnitType)
        Me.m_data = data
        Me.m_pi = cPropertyInfoHelper.GetAllowedProperties(GetType(cLink))
        Me.RefreshContent()
    End Sub

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()
        Dim nCols As Integer = Me.m_pi.Length + 2
        Dim nRows As Integer = Me.m_data.LinkCount + 1
        Dim strHeader As String = ""

        Me.Redim(nRows, nCols)
        Me.FixedRows = 1
        Me.FixedColumns = 1
        Me.AutoSize = True

        For iCol As Integer = 0 To nRows - 1
            If iCol = 0 Then
                ' Index row
                strHeader = ""
            Else
                strHeader = Me.m_pi(iCol - 1).Name
            End If
            Me(iCol, 0) = New EwERowHeaderCell(strHeader)
        Next iCol

        For iRow As Integer = 1 To nRows - 1
            Me.AddLink(Me.m_data.Link(iRow - 1), iRow)
        Next

        Me.AutoSizeColumn(0, 140)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="link"></param>
    ''' <param name="iCol"></param>
    ''' -----------------------------------------------------------------------
    Private Sub AddLink(ByVal link As cLink, ByVal iCol As Integer)
        For iRow As Integer = 0 To Me.RowsCount - 1
            'Me.AddCell(unit, iRow, iCol)
        Next
    End Sub

#End Region ' Internals

End Class
