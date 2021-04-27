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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports SafenetUtils
Imports System.IO

#End Region ' 

Public Class cBiomassEmitter

    Public Sub New(data As cData)
        Me.Data = data
    End Sub

    Public Property Enabled As Boolean = False
    Public ReadOnly Property Data As cData = Nothing

#Region " Run "

    Public Sub InitForRun()
        If (Not Me.Enabled) Then Return
        ' Just in case. Nothing to do for now
    End Sub

    Public Function Apply(timestep As Integer) As Boolean

        If (Not Me.Enabled) Then Return False

        Dim core As cCore = Me.Data.Core
        Dim ecopathDS As cEcopathDataStructures = Me.Data.EcopathDS
        Dim ecospaceDS As cEcospaceDataStructures = Me.Data.EcospaceDS

        If (ecospaceDS.bInSpinUp) Then Return False

        Dim t As Date = core.EcospaceTimestepToAbsoluteTime(timestep)
        Dim d As cEcospaceLayerDepth = core.EcospaceBasemap.LayerDepth
        Dim nApplied As Integer = 0
        Dim bRel As Boolean = (Me.Data.ApplicationType = eApplicationType.Relative)

        For Each mt As cModelTrend In Me.Data.ModelTrends

            Dim bApplied As Boolean = False
            If (mt.CanRun And mt.Enable) Then
                Dim v As Single = mt.ForcingValue(t, mt.Group)
                Dim bUseValue As Boolean = (v > 0) And If(bRel, v <> 1, True)
                If (bUseValue) Then
                    Select Case Me.Data.TargetType
                        Case eTargetType.Region
                            For iCol As Integer = 1 To ecospaceDS.InCol
                                For iRow As Integer = 1 To ecospaceDS.InRow
                                    If d.IsWaterCell(iRow, iCol) Then
                                        If (ecospaceDS.Region(iRow, iCol) = mt.Target) Then
                                            If bRel Then
                                                ecospaceDS.Bcell(iRow, iCol, mt.Group) *= v
                                            Else
                                                ecospaceDS.Bcell(iRow, iCol, mt.Group) = v
                                            End If
                                            bApplied = True
                                        End If
                                    End If
                                Next iRow
                            Next iCol

                        Case eTargetType.MPA
                            Dim map As Integer(,) = ecospaceDS.MPA(mt.Target)
                            For iCol As Integer = 1 To ecospaceDS.InCol
                                For iRow As Integer = 1 To ecospaceDS.InRow
                                    If d.IsWaterCell(iRow, iCol) Then
                                        If (map(iRow, iCol) > 0) Then
                                            If bRel Then
                                                ecospaceDS.Bcell(iRow, iCol, mt.Group) *= v
                                            Else
                                                ecospaceDS.Bcell(iRow, iCol, mt.Group) = v
                                            End If
                                            bApplied = True
                                        End If
                                    End If
                                Next iRow
                            Next iCol

                        Case Else
                            ' Not supported yet
                            Debug.Assert(False)

                    End Select
                End If
            End If
            If (bApplied) Then nApplied += 1
        Next

        If (Me.Data.RuleTrends.Count > 0) And (nApplied = 0) Then

            Dim clustermaps(core.nMPAs) As Map
            For iMPA As Integer = 1 To core.nMPAs
                Dim rt As cRuleTrend = Me.Data.RuleTrends(iMPA - 1)
                If (rt.CanRun) Then
                    Dim map As New Map(ecospaceDS.MPA(iMPA), True)
                    clustermaps(iMPA) = map.Clusters().ClusterCount()
                End If
            Next

            For i As Integer = 1 To ecospaceDS.InRow
                For j As Integer = 1 To ecospaceDS.InCol
                    If (ecospaceDS.Depth(i, j) > 0) Then
                        ' For all fleets
                        Dim BMult(ecopathDS.NumGroups) As Single
                        Dim BMultCount(ecopathDS.NumGroups) As Integer
                        For f As Integer = 1 To ecospaceDS.nFleets
                            ' Is this is a fished water cell?
                            If (ecospaceDS.Depth(i, j) > 0) And (ecospaceDS.PAreaFished(f)(i, j) > 0 Or ecospaceDS.GearHab(f, 0)) Then
                                '#Yes: Cell is potentialy fished 
                                ' If this cell is closed to fishing for this fleet and month by any of the MPAs, then do not allow fishing here 
                                For iMPA As Integer = 1 To ecospaceDS.MPAno
                                    ' Is the MPA closed to this fleet, and is a rule in place?
                                    Dim rt As cRuleTrend = Me.Data.RuleTrends(iMPA - 1)
                                    If rt.CanRun And (ecospaceDS.MPA(iMPA)(i, j) > 0) And (Not ecospaceDS.MPAfishery(f, iMPA)) And (Not ecospaceDS.MPAmonth(ecospaceDS.MonthNow, iMPA)) Then
                                        ' #Yes: This MPA prohibits this fleet from fishing in this cell for this month
                                        For g As Integer = 1 To ecopathDS.NumGroups
                                            If (ecopathDS.Landing(f, g) + ecopathDS.Discard(f, g)) > 0 Then
                                                BMult(g) += rt.Multiplier(t, CInt(clustermaps(iMPA)(j - 1, i - 1)))
                                                BMultCount(g) += 1
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        Next f
                        ' Apply weighted multiplier across all rules to this cell
                        For g As Integer = 1 To ecopathDS.NumGroups
                            If (BMultCount(g) > 0) Then
                                ecospaceDS.Bcell(i, j, g) *= (BMult(g) / BMultCount(g))
                            End If
                        Next
                    End If
                Next j
            Next i
        End If

        If (nApplied > 0) Then
            Dim msg As New cMessage(cStringUtils.Localize(My.Resources.STATUS_EMITTED, nApplied, timestep, t.ToShortDateString()),
                                    eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Information)
            core.Messages.SendMessage(msg)
        End If

        Return True

    End Function

#End Region ' Run

End Class
