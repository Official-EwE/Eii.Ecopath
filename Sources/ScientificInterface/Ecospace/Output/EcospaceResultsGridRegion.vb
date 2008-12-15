#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class cGridEcospaceResultsRegion
        : Inherits gridResultsBase

        Private m_SelRegionIndex As Integer

        Public Property SelRegionIndex() As Integer
            Get
                Return m_SelRegionIndex
            End Get
            Set(ByVal value As Integer)
                m_SelRegionIndex = value
                Me.UpdateData()
            End Set
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            'Define column header
            Me.Redim(1, 8)
            Me(0, 0) = New EwEColumnHeaderCell("")
            'Group name
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            'Biomass (Start)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSSTART)
            'Biomass (End)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSEND)
            'Biomass (E/S)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSES)
            'Catch (Start)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHSTART)
            'Catch (End)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHEND)
            'Catch (E/S)
            Me(0, 7) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCHES)


        End Sub

        Protected Overrides Sub FillData()

            'This method init the cells, its visual and data models. 
            Dim core As cCore = cCore.GetInstance()

            Dim aName(core.nGroups) As String
            For i As Integer = 1 To core.nGroups
                aName(i) = core.EcospaceGroupSummary(i).Name
            Next

            Dim aCalc() As Integer = {4, 7}

            Me.InitCells(core.nGroups + 1, aName, aCalc)

            ReDim aName(core.nFleets)
            For i As Integer = 1 To core.nFleets
                aName(i) = core.EcospaceFleetSummary(i).Name
            Next

            Me.InitCells(core.nFleets + 1, aName, aCalc)

        End Sub

        Private Sub UpdateData()

            Dim core As cCore = cCore.GetInstance()
            Try

                Dim source As cEcospaceRegionSummary = core.EcospaceRegionSummary(Me.SelRegionIndex)

                'The array for storing total values
                Dim totalValue(0 To 7) As Single
                Me.InitTotalArray(totalValue)

                'Add group output
                For groupIndex As Integer = 1 To core.nGroups

                    Dim gBS As Single = source.BiomassStart(groupIndex)
                    Dim gBE As Single = source.BiomassEnd(groupIndex)

                    SetCellValue(groupIndex, 2, gBS, totalValue)
                    SetCellValue(groupIndex, 3, gBE, totalValue)
                    If gBS > 0 And gBE > 0 Then
                        SetCellValue(groupIndex, 4, CSng(gBE / gBS), totalValue)
                    End If
                Next

                Dim rowIndex As Integer = core.nGroups + 1

                For columnIndex As Integer = 2 To 4
                    Me(rowIndex, columnIndex).Value = totalValue(columnIndex)
                Next

                'Add fleet output
                Dim cntGroups As Integer = core.nGroups + 1

                For fleetIndex As Integer = 1 To core.nFleets
                    Dim sum1 As Single = 0
                    Dim sum2 As Single = 0
                    For groupIndex As Integer = 1 To core.nGroups
                        Dim fgCS As Single = source.CatchFleetGroupStart(fleetIndex, groupIndex)
                        Dim fgCE As Single = source.CatchFleetGroupEnd(fleetIndex, groupIndex)
                        If fgCS >= 0 Then sum1 += fgCS
                        If fgCE >= 0 Then sum2 += fgCE
                    Next

                    rowIndex = cntGroups + fleetIndex
                    SetCellValue(rowIndex, 5, sum1, totalValue)
                    SetCellValue(rowIndex, 6, sum2, totalValue)

                    If sum1 > 0 And sum2 > 0 Then
                        SetCellValue(rowIndex, 7, CSng(sum2 / sum1), totalValue)
                    End If
                Next

                'The row for total value - Fleet
                For columnIndex As Integer = 5 To Me.ColumnsCount - 1
                    Me(Me.RowsCount - 1, columnIndex).Value = totalValue(columnIndex)
                Next
            Catch ex As Exception
                Debug.Assert(False, "Error in " & Me.ToString & ".UpdateData() " & ex.Message)
            End Try


        End Sub

    End Class

End Namespace
