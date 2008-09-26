'==============================================================================
'
' $Log: EcosimResultsGridGroup.vb,v $
' Revision 1.1  2008/09/26 07:31:47  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.11  2008/06/02 00:01:33  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.10  2008/05/11 02:51:35  jeroens
' Standardized series of resource strings
'
' Revision 1.9  2008/03/06 02:36:25  jeroens
' Fixed refresh issue
'
' Revision 1.8  2007/10/12 15:20:50  joeb
' Changes for Results forms
'
' Revision 1.7  2007/09/20 18:54:16  joeb
' Bug Fixes
'
' Revision 1.6  2007/09/19 22:15:18  joeb
' Added Summary data
'
' Revision 1.5  2007/08/07 16:42:57  jeroens
' * Audited coding guidelines
'
' Revision 1.4  2007/05/03 18:58:04  fgao
' Linked group display to HideUI options..Only show chosen groups..
'
' Revision 1.3  2007/04/29 03:45:12  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class EcosimResultsGridGroup
        : Inherits gridResultsBase

        Private m_SelFleetIndex As Integer
        Private m_GroupDisplayFlags() As Boolean
        Private m_DisplayGrpCnt As Integer

        Public Sub New()
            MyBase.new()

            m_GroupDisplayFlags = AppLauncher.GetInstance.GroupDisplayFlags
            m_DisplayGrpCnt = 0

        End Sub

        Public Property SelFleetIndex() As Integer
            Get
                Return m_SelFleetIndex
            End Get
            Set(ByVal value As Integer)
                m_SelFleetIndex = value
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

            Dim lName As New List(Of String)
            lName.Add(String.Empty)

            ' Dim aName(core.nGroups) As String
            For i As Integer = 1 To core.nGroups
                If m_GroupDisplayFlags(i) Then
                    lName.Add(core.EcosimGroupSummaries(i).Name)
                    m_DisplayGrpCnt += 1
                End If
            Next

            Dim aCalc() As Integer = {4, 7, 10}

            Me.InitCells(m_DisplayGrpCnt + 1, lName.ToArray, aCalc)

            Me.UpdateData()

        End Sub

        Friend Sub UpdateData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cEcosimGroupSummary = Nothing

            Dim asTotal(0 To 10) As Single
            Me.InitTotalArray(asTotal)

            For groupIndex As Integer = 1 To core.nGroups

                'Only display selected groups
                If m_GroupDisplayFlags(groupIndex) Then

                    source = core.EcosimGroupSummaries(groupIndex)

                    'clear all fleet cells
                    For icell As Integer = 5 To 10
                        SetCellValue(groupIndex, icell, "")
                    Next

                    If source.BiomassStart > 0 Then SetCellValue(groupIndex, 2, source.BiomassStart, asTotal)
                    If source.BiomassEnd > 0 Then SetCellValue(groupIndex, 3, source.BiomassEnd, asTotal)

                    'The logic was pulled out from EwE5
                    If source.BiomassStart > 0 And source.BiomassEnd > 0 Then
                        SetCellValue(groupIndex, 4, CSng(source.BiomassEnd / source.BiomassStart), asTotal)
                    End If

                    Dim fCS As Single = source.CatchStart(Me.SelFleetIndex)
                    If fCS > 0 Then SetCellValue(groupIndex, 5, fCS, asTotal)

                    Dim fCE As Single = source.CatchEnd(Me.SelFleetIndex)
                    If fCE > 0 Then SetCellValue(groupIndex, 6, fCE, asTotal)

                    If fCS > 0 And fCE > 0 Then
                        SetCellValue(groupIndex, 7, CSng(fCE / fCS), asTotal)
                    End If

                    Dim fVS As Single = source.ValueStart(Me.SelFleetIndex)
                    If fVS > 0 Then SetCellValue(groupIndex, 8, fVS, asTotal)

                    Dim fVE As Single = source.ValueEnd(Me.SelFleetIndex)
                    If fVE > 0 Then SetCellValue(groupIndex, 9, fVE, asTotal)

                    If fVS > 0 And fVE > 0 Then
                        SetCellValue(groupIndex, 10, CSng(fVE / fVS), asTotal)
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

    End Class

End Namespace
