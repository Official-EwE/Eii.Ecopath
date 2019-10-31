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

    Private m_imp As New cEcospaceImportExportASCIIData

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Dim core As cCore = Me.UIContext.Core

        For i As Integer = 1 To Me.UIContext.Core.nMPAs
            Me.m_cmbDestMPA.Items.Add(core.EcospaceMPAs(i))
        Next

        Me.m_cbClosePerRegion.Enabled = (core.nRegions > 0)

        Me.CenterToScreen()
        Me.UpdateControls()

    End Sub

    Public Property UIContext As cUIContext

#Region " Events "

    Private Sub OnFormatMPA(sender As Object, e As ListControlConvertEventArgs) Handles m_cmbDestMPA.Format
        Dim fmt As New cCoreInterfaceFormatter()
        e.Value = fmt.ToString(e.ListItem, EwEUtils.Utilities.eDescriptorTypes.Name)
    End Sub

    Private Sub OnMPASelected(sender As Object, e As EventArgs) _
        Handles m_cmbDestMPA.SelectedIndexChanged
        Me.UpdateControls()
    End Sub

    Private Sub OnBrowse(sender As Object, e As EventArgs) Handles m_btnPick.Click
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "ASCII maps|*.asc;*.txt"
        ofd.CheckFileExists = True
        If ofd.ShowDialog() = DialogResult.OK Then
            Me.m_imp.Read(ofd.FileName)
            ' ToDO: validate if compatible with basemap
        End If
    End Sub

    Private Sub OnCloseCells(sender As Object, e As EventArgs) Handles m_btnCloseCells.Click

        Dim core As cCore = Me.UIContext.Core
        Dim mpaDst As cEcospaceMPA = DirectCast(Me.m_cmbDestMPA.SelectedItem, cEcospaceMPA)
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim mapDepth As cEcospaceLayerDepth = bm.LayerDepth
        Dim mapRegions As cEcospaceLayerRegion = bm.LayerRegion
        Dim mapMPADst As cEcospaceLayerMPA = bm.LayerMPA(mpaDst.Index)
        Dim nR As Integer = bm.InRow
        Dim nC As Integer = bm.InCol
        Dim rnd As New Random()

        Dim iFrom As Integer = 1
        Dim iTo As Integer = If(Me.m_cbClosePerRegion.Checked, core.nRegions, 1)
        Dim dtCells As New Dictionary(Of Integer, List(Of Integer))


        Dim mapProtected(nR, nC) As Boolean
        For iMPA As Integer = 1 To 5 ' core.nMPAs
            Dim mpa As cEcospaceMPA = core.EcospaceMPAs(iMPA)
            If mpa.IsActive And iMPA <> mpaDst.Index Then
                Dim map As cEcospaceLayerMPA = bm.LayerMPA(iMPA)
                For iRow As Integer = 1 To nR
                    For iCol As Integer = 1 To nC
                        If (CInt(map.Cell(iRow, iCol)) > 0) Then
                            mapProtected(iRow, iCol) = True
                        End If
                    Next iCol
                Next iRow
            End If
        Next iMPA

        core.SetBatchLock(cCore.eBatchLockType.Update)
        For i As Integer = iFrom To iTo

            Dim nArea As Integer = 0
            Dim nClaimed As Integer = 0

            For iRow As Integer = 1 To nR
                For iCol As Integer = 1 To nC

                    Dim bUseCell As Boolean = mapDepth.IsWaterCell(iRow, iCol)
                    If (Me.m_cbClosePerRegion.Checked) Then
                        bUseCell = (i = CInt(mapRegions.Cell(iRow, iCol)))
                    End If

                    If bUseCell Then
                        nArea += 1
                        mapMPADst.Cell(iRow, iCol) = 0

                        Dim iKey As Integer = 0
                        If (Me.m_imp.NumCells > 0) Then
                            iKey = CInt(Me.m_imp.Value(iRow, iCol))
                        End If

                        If (iKey >= 0) Then
                            If (Not dtCells.ContainsKey(iKey)) Then
                                dtCells(iKey) = New List(Of Integer)
                            End If

                            If mapProtected(iRow, iCol) = False Then
                                Dim x As Integer = bm.RowColToCell(iRow, iCol)
                                If (dtCells(iKey).Count = 0) Then
                                    dtCells(iKey).Add(x)
                                Else
                                    dtCells(iKey).Insert(CInt((rnd.NextDouble * 13 * dtCells(iKey).Count) Mod dtCells(iKey).Count), x)
                                End If
                            Else
                                nClaimed += 1
                            End If
                        End If
                    End If
                Next iCol
            Next iRow

            Dim keys As Integer() = dtCells.Keys.ToArray()
            Array.Sort(keys)
            Array.Reverse(keys)

            Dim lCells As New List(Of Integer)
            For j As Integer = 0 To keys.Length - 1
                lCells.AddRange(dtCells(keys(j)))
            Next

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

#End Region ' Events

#Region " Internals "

    Private Sub UpdateControls()
        Dim iDst As Integer = Me.m_cmbDestMPA.SelectedIndex
        Me.m_btnCloseCells.Enabled = (iDst > 0)
    End Sub

#End Region ' Internals

End Class