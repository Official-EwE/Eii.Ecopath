#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class cGridEcospaceResultsGroup
        : Inherits gridResultsBase

        Private m_sg As cStyleGuide = Nothing
        Private m_iFleetSelected As Integer
        Private m_iNumVisibleGroups As Integer

        Public Sub New()
            MyBase.New()

            Me.m_iNumVisibleGroups = 0

            Me.m_sg = cStyleGuide.GetInstance()
            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        End Sub

        Public Property SelFleetIndex() As Integer
            Get
                Return Me.m_iFleetSelected
            End Get
            Set(ByVal value As Integer)
                Me.m_iFleetSelected = value
                Me.UpdateData()
            End Set
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Define column headers
            Me.Redim(1, 11)
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
            'Value (Start)
            Me(0, 8) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUESTART)
            'Value (End)
            Me(0, 9) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUEEND)
            'Value (E/S)
            Me(0, 10) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUEES)

        End Sub

        Protected Overrides Sub FillData()

            'This method init the cells, its visual and data models. 
            Dim core As cCore = cCore.GetInstance()
            Dim aCalc() As Integer = {4, 7, 10}

            Dim lName As New List(Of String)
            lName.Add(String.Empty)

            Me.m_iNumVisibleGroups = 0
            For iGroup As Integer = 1 To core.nGroups
                If Me.m_sg.GroupVisible(iGroup) Then
                    lName.Add(core.EcospaceGroupOutput(iGroup).Name)
                    m_iNumVisibleGroups += 1
                End If

            Next

            Me.InitCells(m_iNumVisibleGroups + 1, lName.ToArray, aCalc)

            Me.UpdateData()

        End Sub

        Private Sub UpdateData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cEcospaceGroupOutput = Nothing

            Dim totalValue(0 To 10) As Single
            Me.InitTotalArray(totalValue)

            For iGroup As Integer = 1 To core.nGroups

                'Only display selected groups
                If Me.m_sg.GroupVisible(iGroup) Then

                    source = core.EcospaceGroupOutput(iGroup)

                    SetCellValue(iGroup, 2, source.BiomassStart, totalValue)
                    SetCellValue(iGroup, 3, source.BiomassEnd, totalValue)

                    'The logic was pulled out from EwE5
                    If source.BiomassStart > 0 And source.BiomassEnd > 0 Then
                        SetCellValue(iGroup, 4, CSng(source.BiomassEnd / source.BiomassStart), totalValue)
                    End If

                    Dim fCS As Single = source.CatchStart(Me.SelFleetIndex)
                    SetCellValue(iGroup, 5, fCS, totalValue)

                    Dim fCE As Single = source.CatchEnd(Me.SelFleetIndex)
                    SetCellValue(iGroup, 6, fCE, totalValue)

                    If fCS > 0 And fCE > 0 Then
                        SetCellValue(iGroup, 7, CSng(fCE / fCS), totalValue)
                    End If

                    Dim fVS As Single = source.ValueStart(Me.SelFleetIndex)
                    SetCellValue(iGroup, 8, fVS, totalValue)

                    Dim fVE As Single = source.ValueEnd(Me.SelFleetIndex)
                    SetCellValue(iGroup, 9, fVE, totalValue)

                    If fVS > 0 And fVE > 0 Then
                        SetCellValue(iGroup, 10, CSng(fVE / fVS), totalValue)
                    End If

                End If

            Next

            'Display total values
            For columnIndex As Integer = 2 To Me.ColumnsCount - 1
                If columnIndex = 4 Or columnIndex = 7 Or columnIndex = 10 Then
                    If totalValue(columnIndex - 2) > 0 And totalValue(columnIndex - 1) > 0 Then
                        Me(Me.RowsCount - 1, columnIndex).Value = totalValue(columnIndex - 1) / totalValue(columnIndex - 2)
                    End If
                Else
                    If totalValue(columnIndex) > 0 Then
                        Me(Me.RowsCount - 1, columnIndex).Value = totalValue(columnIndex)
                    End If
                End If
            Next

        End Sub

#Region " Events "

        Private Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
            If (ct And cStyleGuide.eChangeType.GroupVisibility) > 0 Then
                Me.RefreshContent()
            End If
        End Sub

        Private Sub OnDisposed(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Disposed
            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        End Sub

#End Region ' Events

    End Class

End Namespace
