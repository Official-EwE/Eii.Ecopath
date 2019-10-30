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

        For i As Integer = 1 To Me.UIContext.Core.nMPAs
            Me.m_cmbMPA.Items.Add(core.EcospaceMPAs(i))
        Next
        Me.m_cmbMPA.SelectedIndex = 0

        Me.CenterToScreen()

    End Sub

    Private Sub m_cmbMPA_Format(sender As Object, e As ListControlConvertEventArgs) Handles m_cmbMPA.Format
        Dim fmt As New cCoreInterfaceFormatter()
        e.Value = fmt.ToString(e.ListItem, EwEUtils.Utilities.eDescriptorTypes.Name)
    End Sub

    Private Sub OnCloseCells(sender As Object, e As EventArgs) Handles m_btnCloseCells.Click

        Dim core As cCore = Me.UIContext.Core
        Dim mpa As cEcospaceMPA = DirectCast(Me.m_cmbMPA.SelectedItem, cEcospaceMPA)
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim mapDepth As cEcospaceLayerDepth = bm.LayerDepth
        Dim mapMPA As cEcospaceLayerMPA = bm.LayerMPA(mpa.Index)
        Dim nR As Integer = bm.InRow
        Dim nC As Integer = bm.InCol
        Dim rnd As New Random()

        core.SetBatchLock(cCore.eBatchLockType.Update)
        Dim lCells As New List(Of Integer)
        For iRow As Integer = 1 To nR
            For iCol As Integer = 1 To nC
                If mapDepth.IsWaterCell(iRow, iCol) Then
                    Dim x As Integer = (iRow - 1) * nC + iCol
                    If (lCells.Count = 0) Then
                        lCells.Add(x)
                    Else
                        lCells.Insert(CInt((rnd.NextDouble * 13 * lCells.Count) Mod lCells.Count), x)
                    End If
                    mapMPA.Cell(iRow, iCol) = 0
                End If
            Next iCol
        Next iRow

        For x As Integer = 1 To CInt(Me.m_nudPercentage.Value * lCells.Count / 100)
            Dim iRow As Integer = 1 + ((lCells(0) - 1) \ nC)
            Dim iCol As Integer = 1 + ((lCells(0) - 1) Mod nR)
            mapMPA.Cell(iRow, iCol) = 1
            lCells.RemoveAt(0)
        Next

        mapMPA.Invalidate()
        core.onChanged(mapMPA)
        core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)

    End Sub

End Class