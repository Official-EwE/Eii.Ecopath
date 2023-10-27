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

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecospace

    Public Class dlgDefineRegions
        Implements IUIElement

        Private m_uic As cUIContext = Nothing
        Private m_bInUpdate As Boolean = True

        Public Sub New(uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
            Me.m_dgvMapping.Columns(2).ValueType = GetType(Integer)
            Me.m_dgvMapping.Columns(3).ValueType = GetType(Integer)
        End Sub

        Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_nudNoRegions.Value = Me.UIContext.Core.nRegions

            Me.m_rbFromHabitats.Enabled = (Me.UIContext.Core.nHabitats > 0)
            Me.m_rbFromMPAs.Enabled = (Me.UIContext.Core.nMPAs > 0)

            Me.m_bInUpdate = False

            Me.UpdateControls()
            Me.CenterToScreen()

        End Sub

#Region " Events "

        Private Sub OnSelectionChanged(sender As Object, e As EventArgs) _
            Handles m_rbFromHabitats.CheckedChanged, m_rbFromMPAs.CheckedChanged, m_rbNone.CheckedChanged

            ' Prevent from responding too soon
            If Me.m_bInUpdate Then Return

            Me.UpdateMappingGrid()

        End Sub

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles m_btnOK.Click

            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters
            Dim nReg As Integer = CInt(Me.m_nudNoRegions.Value)

            parms.nRegions = nReg

            If Me.m_rbFromHabitats.Checked Then
                Me.AssignHabitatRegions()
            ElseIf Me.m_rbFromMPAs.Checked Then
                Me.AssignMPARegions()
            End If

            Me.Close()

        End Sub

        Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCancel.Click
            Try
                Me.Close()
            Catch ex As Exception

            End Try
        End Sub


#End Region ' Events

#Region " Internals "

        Private Sub UpdateMappingGrid()

            Dim core As cCore = Me.m_uic.Core

            Me.m_dgvMapping.Rows.Clear()

            If Me.m_rbFromHabitats.Checked Then
                For i As Integer = 1 To core.nHabitats - 1
                    Me.m_dgvMapping.Rows.Add({i, core.EcospaceHabitats(i).Name, i, i})
                Next
                Me.m_dgvMapping.Columns(3).Visible = False
            ElseIf Me.m_rbFromMPAs.Checked Then
                For i As Integer = 1 To core.nMPAs
                    Me.m_dgvMapping.Rows.Add({i, core.EcospaceMPAs(i).Name, i, i})
                Next
                Me.m_dgvMapping.Columns(3).Visible = True
            End If

        End Sub

        Private Sub UpdateControls()

            Dim bHasSel As Boolean = (Me.m_rbNone.Checked Or Me.m_rbFromMPAs.Checked Or Me.m_rbFromHabitats.Checked)
            Me.m_btnOK.Enabled = True

        End Sub

        Private Sub AssignMPARegions()

            If (Me.UIContext Is Nothing) Then Return

            Dim core As cCore = Me.m_uic.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim regions As cEcospaceLayerRegion = bm.LayerRegion
            Dim ll As cEcospaceLayer() = bm.Layers(eVarNameFlags.LayerMPA)

            For iRow As Integer = 1 To bm.InRow
                For iCol As Integer = 1 To bm.InCol

                    Dim iPriorityMax As Integer = 0
                    Dim iRegionSel As Integer = 0

                    For iMPA As Integer = 0 To core.nMPAs - 1
                        If CInt(ll(iMPA).Cell(iRow, iCol)) > 0 Then

                            Dim iPriority As Integer = CInt(Me.m_dgvMapping.Rows(iMPA).Cells(3).Value)
                            If (iPriority > iPriorityMax) Then
                                iPriorityMax = iPriority
                                iRegionSel = CInt(Me.m_dgvMapping.Rows(iMPA).Cells(2).Value)
                            End If
                        End If
                    Next iMPA

                    regions.Cell(iRow, iCol) = iRegionSel
                Next iCol
            Next iRow

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create regions from Habitats.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub AssignHabitatRegions()

            If (Me.UIContext Is Nothing) Then Return

            Dim core As cCore = Me.UIContext.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim parms As cEcospaceModelParameters = core.EcospaceModelParameters
            Dim regions As cEcospaceLayerRegion = bm.LayerRegion
            Dim ll As cEcospaceLayer() = bm.Layers(eVarNameFlags.LayerHabitat)

            Try

                For iRow As Integer = 1 To bm.InRow
                    For iCol As Integer = 1 To bm.InCol
                        Dim sCoverMax As Single = 0
                        Dim iRegionSel As Integer = 0
                        For iHab As Integer = 1 To core.nHabitats - 1
                            Dim sCover As Single = CSng(ll(iHab - 1).Cell(iRow, iCol))
                            If sCover > sCoverMax Then
                                sCoverMax = sCover
                                iRegionSel = CInt(Me.m_dgvMapping.Rows(iHab - 1).Cells(2).Value)
                            End If
                        Next
                        regions.Cell(iRow, iCol) = iRegionSel
                    Next iCol
                Next iRow
            Catch ex As Exception

            End Try
            regions.Invalidate()
            core.onChanged(regions)

        End Sub

#End Region ' Internals

    End Class

End Namespace
