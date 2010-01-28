#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class EcosimResultsGridGroup
        : Inherits gridResultsBase

        Private m_iFleetSelected As Integer
        Private m_iNumVisibleGroups As Integer

        Public Sub New()
            MyBase.New()

            Me.m_iNumVisibleGroups = 0

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

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As cUIContext)
                MyBase.UIContext = value
                If (value IsNot Nothing) Then
                    AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                End If
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

            Dim lName As New List(Of String)
            lName.Add(String.Empty)

            ' OMG, what is this?!
            Dim aCalc() As Integer = {4, 7, 10}

            Me.m_iNumVisibleGroups = 0
            For iGroup As Integer = 1 To core.nGroups
                If Me.StyleGuide.GroupVisible(iGroup) Then
                    lName.Add(Core.EcoSimGroupOutputs(iGroup).Name)
                    Me.m_iNumVisibleGroups += 1
                End If
            Next

            Me.InitCells(m_iNumVisibleGroups + 1, lName.ToArray, aCalc)

            Me.UpdateData()

        End Sub

        Friend Sub UpdateData()

            Dim core As cCore = cCore.GetInstance()
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()
            Dim source As cEcosimGroupOutput = Nothing
            Dim irow As Integer

            Dim asTotal(0 To 10) As Single
            Me.InitTotalArray(asTotal)

            For iGroup As Integer = 1 To core.nGroups

                'Only display selected groups
                If sg.GroupVisible(iGroup) Then
                    irow += 1
                    source = core.EcoSimGroupOutputs(iGroup)

                    'clear all fleet cells
                    For icell As Integer = 5 To 10
                        SetCellValue(irow, icell, "")
                    Next

                    If source.BiomassStart > 0 Then SetCellValue(irow, 2, source.BiomassStart, asTotal)
                    If source.BiomassEnd > 0 Then SetCellValue(irow, 3, source.BiomassEnd, asTotal)

                    'The logic was pulled out from EwE5
                    If source.BiomassStart > 0 And source.BiomassEnd > 0 Then
                        SetCellValue(irow, 4, CSng(source.BiomassEnd / source.BiomassStart), asTotal)
                    End If

                    Dim fCS As Single = source.CatchStart(Me.SelFleetIndex)
                    If fCS > 0 Then SetCellValue(irow, 5, fCS, asTotal)

                    Dim fCE As Single = source.CatchEnd(Me.SelFleetIndex)
                    If fCE > 0 Then SetCellValue(irow, 6, fCE, asTotal)

                    If fCS > 0 And fCE > 0 Then
                        SetCellValue(irow, 7, CSng(fCE / fCS), asTotal)
                    End If

                    Dim fVS As Single = source.ValueStart(Me.SelFleetIndex)
                    If fVS > 0 Then SetCellValue(irow, 8, fVS, asTotal)

                    Dim fVE As Single = source.ValueEnd(Me.SelFleetIndex)
                    If fVE > 0 Then SetCellValue(irow, 9, fVE, asTotal)

                    If fVS > 0 And fVE > 0 Then
                        SetCellValue(irow, 10, CSng(fVE / fVS), asTotal)
                    End If

                End If

            Next

            'Display total values
            For columnIndex As Integer = 2 To Me.ColumnsCount - 1
                If columnIndex = 4 Or columnIndex = 7 Or columnIndex = 10 Then
                    If asTotal(columnIndex - 2) > 0 And asTotal(columnIndex - 1) > 0 Then
                        Me(Me.RowsCount - 1, columnIndex).Value = asTotal(columnIndex - 1) / asTotal(columnIndex - 2)
                    End If
                Else
                    If asTotal(columnIndex) > 0 Then
                        Me(Me.RowsCount - 1, columnIndex).Value = asTotal(columnIndex)
                    End If
                End If
            Next

            Me.Refresh()
        End Sub

#Region " Events "

        Private Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
            If (ct And cStyleGuide.eChangeType.GroupVisibility) > 0 Then
                Me.RefreshContent()
            End If
        End Sub

        Private Sub OnDisposed(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Disposed

            If Me.StyleGuide IsNot Nothing Then
                RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            End If

        End Sub

#End Region ' Events

    End Class

End Namespace
