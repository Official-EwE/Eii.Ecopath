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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Option Strict On
Imports EwECore

Namespace Controls.Map.Layers

    Public Class ucLayerEditorRegion

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return
            Dim iNumRegions As Integer = Me.UIContext.Core.nRegions

            Me.Editor.CellValueMax = iNumRegions
            Me.m_nudNoRegions.Value = iNumRegions
            Me.m_nudRegion.Maximum = iNumRegions

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim iNumH2O As Integer = bm.InRow * bm.InCol
            Dim iMinCluster As Integer = 1
            Dim iMaxCluster As Integer = Math.Max(bm.InRow, bm.InCol)

            ' Try to set to no more than 500 cells
            Dim bFound As Boolean = (iNumH2O < 500)
            While Not bFound
                iMinCluster += 1
                bFound = ((iNumH2O / (iMinCluster * iMinCluster)) < 500)
            End While

            Me.m_nudClusterSize.Value = iMinCluster
            Me.m_nudClusterSize.Minimum = iMinCluster
            Me.m_nudClusterSize.Maximum = iMaxCluster

        End Sub

        Public Overrides Sub UpdateContent(editor As cLayerEditor)
            MyBase.UpdateContent(editor)
            If (Me.UIContext Is Nothing) Then Return

            Dim iVal As Integer

            ' Sanity check
            If (Me.m_nudNoRegions Is Nothing) Then Return
            If (Me.m_nudRegion Is Nothing) Then Return

            ' Set control value
            iVal = CInt(editor.CellValue)

            Me.m_nudRegion.Value = iVal
            Me.m_nudRegion.Maximum = CDec(editor.CellValueMax)

        End Sub

#End Region ' Overrides

#Region " Event handlers "

        Private Sub OnCreateFromCell(sender As System.Object, e As System.EventArgs) _
            Handles m_btnFromCell.Click
            If (TypeOf Me.Editor Is cLayerEditorRegion) Then
                Try
                    DirectCast(Me.Editor, cLayerEditorRegion).CreateCellRegions(CInt(Me.m_nudClusterSize.Value))
                Catch ex As Exception

                End Try
            End If
        End Sub

        Private Sub OnCreateFromMPA(sender As System.Object, e As System.EventArgs) _
            Handles m_btnFromMPAs.Click
            If (TypeOf Me.Editor Is cLayerEditorRegion) Then
                Try
                    DirectCast(Me.Editor, cLayerEditorRegion).CreateMPARegions()
                Catch ex As Exception

                End Try
            End If
        End Sub

        Private Sub OnCreateFromHabitat(sender As System.Object, e As System.EventArgs) _
            Handles m_btnFromHabitats.Click
            If (TypeOf Me.Editor Is cLayerEditorRegion) Then
                Try
                    DirectCast(Me.Editor, cLayerEditorRegion).CreateHabitatRegions()
                Catch ex As Exception

                End Try
            End If
        End Sub

        Private Sub OnNumRegionsChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudNoRegions.ValueChanged

            If (Me.UIContext Is Nothing) Then Return

            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters
            parms.nRegions = CInt(Me.m_nudNoRegions.Value)

            Me.Editor.CellValueMax = parms.nRegions
            'Me.m_nudRegion.Value = Math.Min(Me.m_nudRegion.Value, parms.nRegions)
            'Me.m_nudRegion.Maximum = parms.nRegions
            Me.UpdateContent(Me.Editor)

        End Sub

        Private Sub OnDrawRegionChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudRegion.ValueChanged

            If (Me.UIContext Is Nothing) Then Return

            Me.Editor.CellValue = CInt(Me.m_nudRegion.Value)

        End Sub

#End Region ' Event handlers


    End Class

End Namespace
