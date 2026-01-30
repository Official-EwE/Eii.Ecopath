' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecospace

    Public Class dlgDefineRegions
        Implements IUIElement

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_bInUpdate As Boolean = True

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New(uic As cUIContext)
            Me.InitializeComponent()
            Me.UIContext = uic
            Me.m_dgvMapping.Columns(2).ValueType = GetType(Integer)
            Me.m_dgvMapping.Columns(3).ValueType = GetType(Integer)
        End Sub

#End Region ' Construction / destruction

#Region " Public access "

        Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                Me.m_uic = value
                Me.m_acknowledgements.UIContext = value
            End Set
        End Property

#End Region ' Public access

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim core As cCore = Me.m_uic.Core

            Me.m_nudNoRegions.Value = core.nRegions

            Me.m_acknowledgements.AddLogo("FuMa", My.Resources.FuMa_logo, "https://futuremares.eu")

            Me.UpdateControls()
            Me.CenterToScreen()

            Me.m_bInUpdate = False

        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

            Me.m_acknowledgements.UIContext = Nothing
            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Overrides

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

            Dim core As cCore = Me.m_uic.Core

            Dim bHasSel As Boolean = (Me.m_rbNone.Checked Or Me.m_rbFromMPAs.Checked Or Me.m_rbFromHabitats.Checked)
            Dim bHasHabs As Boolean = (core.nHabitats > 1)
            Dim bHasMPAs As Boolean = (core.nMPAs > 0)

            Me.m_rbFromHabitats.Enabled = bHasHabs
            Me.m_rbFromMPAs.Enabled = bHasMPAs
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

