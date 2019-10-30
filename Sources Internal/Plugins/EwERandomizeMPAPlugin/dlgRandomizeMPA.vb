' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Class dlgRandomizeMPA

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Property UIContext As cUIContext

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Dim core As cCore = Me.UIContext.Core

        Me.m_cmbSrcMPA.Items.Add("(none)")
        For i As Integer = 1 To Me.UIContext.Core.nMPAs
            Me.m_cmbDestMPA.Items.Add(core.EcospaceMPAs(i))
            Me.m_cmbSrcMPA.Items.Add(core.EcospaceMPAs(i))
        Next
        Me.m_cmbSrcMPA.SelectedIndex = 0

        Me.m_cbClosePerRegion.Enabled = (core.nRegions > 0)

        Me.CenterToScreen()
        Me.UpdateControls()

    End Sub

    Private Sub UpdateControls()
        Dim iSrc As Integer = Me.m_cmbSrcMPA.SelectedIndex
        Dim iDst As Integer = Me.m_cmbDestMPA.SelectedIndex
        Me.m_btnCloseCells.Enabled = (iDst > 0) And ((iSrc - 1) <> iDst)
    End Sub

    Private Sub OnFormatMPA(sender As Object, e As ListControlConvertEventArgs) Handles m_cmbDestMPA.Format, m_cmbSrcMPA.Format
        Dim fmt As New cCoreInterfaceFormatter()
        e.Value = fmt.ToString(e.ListItem, EwEUtils.Utilities.eDescriptorTypes.Name)
    End Sub

    Private Sub OnCloseCells(sender As Object, e As EventArgs) Handles m_btnCloseCells.Click

        Dim core As cCore = Me.UIContext.Core
        Dim mpaSrc As cEcospaceMPA = Nothing
        Dim mpaDst As cEcospaceMPA = DirectCast(Me.m_cmbDestMPA.SelectedItem, cEcospaceMPA)
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim mapDepth As cEcospaceLayerDepth = bm.LayerDepth
        Dim mapRegions As cEcospaceLayerRegion = bm.LayerRegion
        Dim mapMPADst As cEcospaceLayerMPA = bm.LayerMPA(mpaDst.Index)
        Dim mapMPASrc As cEcospaceLayerMPA = Nothing
        Dim nR As Integer = bm.InRow
        Dim nC As Integer = bm.InCol
        Dim rnd As New Random()

        If (Me.m_cmbSrcMPA.SelectedIndex > 0) Then
            mpaSrc = DirectCast(Me.m_cmbSrcMPA.SelectedItem, cEcospaceMPA)
            mapMPASrc = bm.LayerMPA(mpaSrc.Index)
        End If

        Dim iFrom As Integer = 1
        Dim iTo As Integer = If(Me.m_cbClosePerRegion.Checked, core.nRegions, 1)

        core.SetBatchLock(cCore.eBatchLockType.Update)
        For i As Integer = iFrom To iTo

            Dim lCells As New List(Of Integer)
            Dim nClaimed As Integer = 0
            Dim nArea As Integer = 0

            For iRow As Integer = 1 To nR
                For iCol As Integer = 1 To nC
                    Dim bUseCell As Boolean = mapDepth.IsWaterCell(iRow, iCol)
                    If (Me.m_cbClosePerRegion.Checked) Then
                        bUseCell = (i = CInt(mapRegions.Cell(iRow, iCol)))
                    End If
                    If bUseCell Then
                        ' Already claimed?
                        Dim bClaimed As Boolean = False
                        If (mapMPASrc IsNot Nothing) Then
                            bClaimed = (CInt(mapMPASrc.Cell(iRow, iCol)) > 0)
                        End If
                        If (Not bClaimed) Then
                            Dim x As Integer = bm.RowColToCell(iRow, iCol)
                            If (lCells.Count = 0) Then
                                lCells.Add(x)
                            Else
                                lCells.Insert(CInt((rnd.NextDouble * 13 * lCells.Count) Mod lCells.Count), x)
                            End If
                            mapMPADst.Cell(iRow, iCol) = 0
                        Else
                            nClaimed += 1
                            mapMPADst.Cell(iRow, iCol) = 1
                        End If
                        nArea += 1
                    End If
                Next iCol
            Next iRow

            Debug.Assert(nArea - nClaimed = lCells.Count)

            For x As Integer = 1 To CInt(Me.m_nudPercentage.Value * nArea / 100) - nClaimed
                Dim iRow, iCol As Integer
                bm.CellToRowCol(lCells(0), iRow, iCol)
                mapMPADst.Cell(iRow, iCol) = 1
                lCells.RemoveAt(0)
            Next
        Next

        mapMPADst.Invalidate()
        core.onChanged(mapMPADst)
        core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)

    End Sub

    Private Sub OnMPASelected(sender As Object, e As EventArgs) _
        Handles m_cmbDestMPA.SelectedIndexChanged, m_cmbSrcMPA.SelectedIndexChanged
        Me.UpdateControls()
    End Sub

End Class