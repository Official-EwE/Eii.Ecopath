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
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class dlgRandomizeMPA

    Private m_imp As cEcospaceImportExportASCIIData = Nothing

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
        Me.HaveUserSelectWeightMap()
    End Sub

    Private Sub OnCloseCells(sender As Object, e As EventArgs) Handles m_btnCloseCells.Click
        Me.CloseCells()
    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub UpdateControls()
        Dim iDst As Integer = Me.m_cmbDestMPA.SelectedIndex
        Me.m_btnCloseCells.Enabled = (iDst >= 0)
        Me.m_tbxWeight.Text = ""
    End Sub

    Private Sub HaveUserSelectWeightMap()

        Me.m_imp = Nothing
        Me.m_tbxWeight.Text = ""

        Dim ofd As New OpenFileDialog()
        ofd.Filter = SharedResources.FILEFILTER_ASC
        ofd.CheckFileExists = True

        If ofd.ShowDialog() = DialogResult.OK Then
            Dim imp As New cEcospaceImportExportASCIIData(Me.UIContext.Core)
            If imp.Read(ofd.FileName) Then
                Me.m_imp = imp
                Me.m_tbxWeight.Text = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Close cells of a target MPA to achieve a given percentage of cells 
    ''' closed in the area. The given percentage is calculated across all 
    ''' watercells. Cells where any type of enforcement is active (except for 
    ''' the target MPA) contribute towards the percentage to close. The code 
    ''' can proportionally close cells in regions. Optionally, a weight ASC 
    ''' map can be provided. High priority cells are sampled first before a 
    ''' lower priority is considered.
    ''' </summary>
    Private Sub CloseCells()

        Dim core As cCore = Me.UIContext.Core
        Dim mpaDst As cEcospaceMPA = DirectCast(Me.m_cmbDestMPA.SelectedItem, cEcospaceMPA)
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim mapDepth As cEcospaceLayerDepth = bm.LayerDepth
        Dim mapRegions As cEcospaceLayerRegion = bm.LayerRegion
        Dim mapDest As cEcospaceLayerMPA = bm.LayerMPA(mpaDst.Index)
        Dim nR As Integer = bm.InRow
        Dim nC As Integer = bm.InCol
        Dim rnd As New Random()

        ' -- Determine if regions need to be proportionally closed --
        Dim iFrom As Integer = 1
        Dim iTo As Integer = If(Me.m_cbClosePerRegion.Checked, core.nRegions, 1)

        ' -- Make an inventary of cells closed to fishing by any MPA other than the target MPA --
        Dim mapProtected(nR, nC) As Boolean
        For iMPA As Integer = 1 To core.nMPAs
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

        ' Start making changes
        core.SetBatchLock(cCore.eBatchLockType.Update)
        Try
            ' -- For all areas --
            For iArea As Integer = iFrom To iTo

                Dim nAreaCells As Integer = 0
                Dim nOccupiedCells As Integer = 0
                Dim dtCells As New Dictionary(Of Integer, List(Of Integer))

                For iRow As Integer = 1 To nR
                    For iCol As Integer = 1 To nC

                        ' Is this a water cell in the current area?
                        Dim bUseCell As Boolean = mapDepth.IsWaterCell(iRow, iCol)
                        If (Me.m_cbClosePerRegion.Checked) Then
                            bUseCell = bUseCell And (iArea = CInt(mapRegions.Cell(iRow, iCol)))
                        End If

                        If bUseCell Then
                            ' Count cell and wipe it
                            nAreaCells += 1
                            mapDest.Cell(iRow, iCol) = 0

                            ' Get the weight this value belongs to, 0 if no weight map is provided
                            Dim iKey As Integer = 0
                            If (Me.m_imp IsNot Nothing) Then
                                iKey = CInt(Me.m_imp.Value(iRow, iCol))
                            End If

                            ' Just to make sure
                            If (iKey >= 0) Then

                                ' Is the cell not protected?
                                If mapProtected(iRow, iCol) = False Then
                                    ' Get cell index
                                    Dim x As Integer = bm.RowColToCell(iRow, iCol)
                                    ' Make sure a list exists to add the cell index to
                                    If (Not dtCells.ContainsKey(iKey)) Then
                                        dtCells(iKey) = New List(Of Integer)
                                    End If
                                    ' Add cell index to the list at a random location (to shuffle the list)
                                    If (dtCells(iKey).Count = 0) Then
                                        dtCells(iKey).Add(x)
                                    Else
                                        dtCells(iKey).Insert(CInt((rnd.NextDouble * 13 * dtCells(iKey).Count) Mod dtCells(iKey).Count), x)
                                    End If
                                Else
                                    ' Count occupied cell
                                    nOccupiedCells += 1
                                End If
                            End If
                        End If
                    Next iCol
                Next iRow

                ' Get the weight keys, sort em, and inverse with highest weight first
                Dim keys As Integer() = dtCells.Keys.ToArray()
                Array.Sort(keys)
                Array.Reverse(keys)

                ' Construct cell index list to sample from
                Dim lCells As New List(Of Integer)
                For j As Integer = 0 To keys.Length - 1
                    lCells.AddRange(dtCells(keys(j)))
                Next

                ' Update destination map
                For x As Integer = 1 To CInt(Math.Ceiling(Me.m_nudPercentage.Value * nAreaCells / 100)) - nOccupiedCells
                    Dim iRow, iCol As Integer
                    bm.CellToRowCol(lCells(0), iRow, iCol)
                    mapDest.Cell(iRow, iCol) = 1
                    lCells.RemoveAt(0)
                Next
            Next
        Catch ex As Exception
            cLog.Write(ex)
        End Try

        ' Commit changes
        mapDest.Invalidate()
        core.onChanged(mapDest)
        core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)

    End Sub

#End Region ' Internals

End Class