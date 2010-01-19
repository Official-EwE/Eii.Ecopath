
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE

#End Region


<CLSCompliant(False)> _
Public Class gridRiskResults
    : Inherits EwEGrid

    Public Enum eGridType
        Group
        Fleet
    End Enum

    Private m_core As cCore
    Private m_type As eGridType


    Public Property GridType() As eGridType
        Get
            Return Me.m_type
        End Get
        Set(ByVal value As eGridType)
            Me.m_type = value
            Me.Update()
        End Set
    End Property

    Public Overloads Sub Update()
        MyBase.Update()
        Try
            Me.InitStyle()
            Me.FillData()
        Catch ex As Exception

        End Try
    End Sub

    Public Sub New()

        Me.m_core = cCore.GetInstance
        Me.m_type = eGridType.Group

    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 13)

        If Me.m_type = eGridType.Group Then
            Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUP)
            Me(0, 1) = New EwEColumnHeaderCell("Biomass Min.")
            Me(0, 2) = New EwEColumnHeaderCell("Biomass Max.")
            Me(0, 3) = New EwEColumnHeaderCell("Biomass CV")
            Me(0, 4) = New EwEColumnHeaderCell("Biomass Std.")
            Me(0, 5) = New EwEColumnHeaderCell("Biomass % below reference")
            Me(0, 6) = New EwEColumnHeaderCell("Biomass % above reference")

            Me(0, 7) = New EwEColumnHeaderCell("Catch Min.")
            Me(0, 8) = New EwEColumnHeaderCell("Catch Max.")
            Me(0, 9) = New EwEColumnHeaderCell("Catch CV")
            Me(0, 10) = New EwEColumnHeaderCell("Catch Std.")

            Me(0, 11) = New EwEColumnHeaderCell("Catch % below reference")
            Me(0, 12) = New EwEColumnHeaderCell("Catch % above reference")

        ElseIf Me.m_type = eGridType.Fleet Then
            Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
            Me(0, 1) = New EwEColumnHeaderCell("Catch Min.")
            Me(0, 2) = New EwEColumnHeaderCell("Catch Max.")
            Me(0, 3) = New EwEColumnHeaderCell("Catch CV")
            Me(0, 4) = New EwEColumnHeaderCell("Catch Std.")

            Me(0, 5) = New EwEColumnHeaderCell("Catch % below reference")
            Me(0, 6) = New EwEColumnHeaderCell("Catch % above reference")

            Me(0, 7) = New EwEColumnHeaderCell("Effort Min.")
            Me(0, 8) = New EwEColumnHeaderCell("Effort Max.")
            Me(0, 9) = New EwEColumnHeaderCell("Effort CV")
            Me(0, 10) = New EwEColumnHeaderCell("Catch Std.")
            Me(0, 11) = New EwEColumnHeaderCell("Effort % below reference")
            Me(0, 12) = New EwEColumnHeaderCell("Effort % above reference")

        End If

    End Sub

    Protected Overrides Sub FillData()
        Try
            'Why no property cells?????
            'PropertyCell() requires a ValueDescriptor property which cMSEStats objects can not populate
            'so we can not use PropertyCells with a cMSEStat object.
            'Grid cells need to be populated by hand
            Dim mse As cMSEManager = Me.m_core.MSEManager
            If mse Is Nothing Then Exit Sub

            Dim lstData1 As cCoreInputOutputList(Of cCoreInputOutputBase)
            Dim lstData2 As cCoreInputOutputList(Of cCoreInputOutputBase)

            Dim RowNames() As String

            If Me.m_type = eGridType.Group Then
                lstData1 = mse.BiomassStats
                lstData2 = mse.GroupCatchStats
            ElseIf Me.m_type = eGridType.Fleet Then
                lstData1 = mse.FleetStats
                lstData2 = mse.EffortStats
            End If

            Debug.Assert(lstData1 IsNot Nothing Or lstData2 IsNot Nothing, Me.ToString & ".FillData() Failed to find MSEStats object for " & Me.m_type.ToString)

            ReDim RowNames(lstData1.Count)
            For Each grp As cMSEStats In lstData1
                RowNames(grp.Index) = grp.Name
            Next

            'WARNING if you add data or changed the column orders 
            'then you MUST change cell styles in InitCells() 
            Me.InitCells(lstData1.Count, RowNames)

            For Each grp As cMSEStats In lstData1
                Me.SetCellValue(grp.Index, 1, grp.Min)
                Me.SetCellValue(grp.Index, 2, grp.Max)
                Me.SetCellValue(grp.Index, 3, grp.CV)
                Me.SetCellValue(grp.Index, 4, grp.Std)
                Me.SetCellValue(grp.Index, 5, grp.BelowLimit)
                Me.SetCellValue(grp.Index, 6, grp.AboveLimit)
            Next

            For Each grp As cMSEStats In lstData2
                Me.SetCellValue(grp.Index, 7, grp.Min)
                Me.SetCellValue(grp.Index, 8, grp.Max)
                Me.SetCellValue(grp.Index, 8, grp.CV)
                Me.SetCellValue(grp.Index, 9, grp.Std)
                Me.SetCellValue(grp.Index, 10, grp.BelowLimit)
                Me.SetCellValue(grp.Index, 11, grp.AboveLimit)
            Next

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub

    Private Sub InitCells(ByVal iRow As Integer, ByRef astrNames() As String)

        Dim cell As EwECell = Nothing
        Dim cnt As Integer = Me.RowsCount '- 1

        For rowIndex As Integer = cnt To iRow
            'Insert a new row
            Me.Rows.Insert(rowIndex)

            Me(rowIndex, 0) = New EwERowHeaderCell(astrNames(rowIndex))

            'not the best way to do this 
            'set the Style of the cell base on its column index not its contents
            For columnIndex As Integer = 1 To Me.ColumnsCount - 1

                cell = New EwECell(0.0!, GetType(Single))
                cell.Style = cStyleGuide.eStyleFlags.NotEditable

                'set the cell to Null if there is no catch or discards for this group
                If Me.m_type = eGridType.Group And columnIndex > 6 Then
                    Dim noCatch As Boolean = True
                    For iflt As Integer = 1 To Me.m_core.nFleets
                        If Me.m_core.FleetInputs(iflt).Landings(rowIndex) + Me.m_core.FleetInputs(iflt).Discards(rowIndex) > 0 Then
                            noCatch = False
                            Exit For
                        End If
                    Next
                    If noCatch Then
                        'no catch so set the style to NotEditable Null
                        cell.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
                    End If
                End If

                Me(rowIndex, columnIndex) = cell

            Next
        Next

    End Sub

    Private Sub SetCellValue(ByVal iRow As Integer, ByVal iCol As Integer, ByVal sValue As String)
        Try
            Me(iRow, iCol).Value = sValue
        Catch ex As Exception
            'do nothing??
        End Try
    End Sub

    Private Sub SetCellValue(ByVal iRow As Integer, ByVal iCol As Integer, ByVal sValue As Single)
        Try
            Me(iRow, iCol).Value = sValue
        Catch ex As Exception
            'do nothing??
        End Try
    End Sub


End Class
