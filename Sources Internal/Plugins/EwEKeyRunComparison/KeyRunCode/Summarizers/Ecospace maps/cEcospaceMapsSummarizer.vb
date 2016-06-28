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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Base class for generating hash codes for Ecospace maps.
''' </summary>
Public Class cEcospaceMapsSummarizer
    Implements IHashSummarizer

#Region " Protected vars "

    Protected m_core As cCore = Nothing

#End Region ' Protected vars

    Private m_lVars As New List(Of eVarNameFlags)

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "EcospaceMaps"
    End Function

    Public Overridable Sub Init() _
        Implements IHashSummarizer.Init

        ' Only add input layers for the Ecospace model. Do not add MPA search layers

        Me.m_lVars.Add(eVarNameFlags.LayerDepth)
        Me.m_lVars.Add(eVarNameFlags.LayerAdvection)
        Me.m_lVars.Add(eVarNameFlags.LayerBiomassForcing)
        Me.m_lVars.Add(eVarNameFlags.LayerBiomassRelativeForcing)
        Me.m_lVars.Add(eVarNameFlags.LayerExclusion)
        Me.m_lVars.Add(eVarNameFlags.LayerHabitat)
        Me.m_lVars.Add(eVarNameFlags.LayerHabitatCapacityInput)
        Me.m_lVars.Add(eVarNameFlags.LayerMigration)
        Me.m_lVars.Add(eVarNameFlags.LayerMLD)
        Me.m_lVars.Add(eVarNameFlags.LayerPort)
        Me.m_lVars.Add(eVarNameFlags.LayerRelCin)
        Me.m_lVars.Add(eVarNameFlags.LayerRelPP)
        Me.m_lVars.Add(eVarNameFlags.LayerSail)
        Me.m_lVars.Add(eVarNameFlags.LayerUpwelling)
        Me.m_lVars.Add(eVarNameFlags.LayerWind)
        Me.m_lVars.Add(eVarNameFlags.LayerMPA)

        ' Used driver layers are hashed in cEcospaceCapacitySummarizer
        'Me.m_lVars.Add(eVarNameFlags.LayerDriver)

        ' Capacity output maps are not hashed right now. That could be wrong?
        'Me.m_lVars.Add(eVarNameFlags.LayerHabitatCapacity)

    End Sub

    Public Overridable Function HashValues() As cHashValues() _
        Implements IHashSummarizer.HashValues

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim layers() As cEcospaceLayer = Nothing
        Dim layer As cEcospaceLayer = Nothing
        Dim vn As eVarNameFlags = eVarNameFlags.NotSet
        Dim lResults As New List(Of cHashValues)

        ' StringBuilder version is twice as slow as the binary version

#If 0 Then
        Dim sb As New StringBuilder()
        For i As Integer = 0 To Me.m_lVars.Count - 1

            vn = Me.m_lVars(i)
            layers = bm.Layers(vn)

            For j As Integer = 0 To layers.Length - 1

                layer = layers(j)

                ' Skip zero-indexed layers (all fleets, etc)
                If (layer.Index > 0) Then
                    If (sb.Length > 0) Then sb.Append(" ")
                    sb.Append(layer.Index & ":" & cStringConverters.LayerToString(bm.InRow, bm.InCol, layer))
                End If
            Next j

            lResults.Add(New cHashValues(Me.Name, vn, sb.ToString))
            sb.Clear()

        Next i
#Else
        Dim vc As New cValueCollector()
        For i As Integer = 0 To Me.m_lVars.Count - 1

            vn = Me.m_lVars(i)
            layers = bm.Layers(vn)

            For j As Integer = 0 To layers.Length - 1

                layer = layers(j)

                ' Skip zero-indexed layers (all fleets, etc)
                If (layer.Index > 0) Then
                    If TypeOf (layer) Is cEcospaceLayerBoolean Then
                        vc.Add(bm.InRow, bm.InCol, DirectCast(layer, cEcospaceLayerBoolean))
                    ElseIf TypeOf layer Is cEcospaceLayerSingle Then
                        vc.Add(bm.InRow, bm.InCol, DirectCast(layer, cEcospaceLayerSingle))
                    ElseIf TypeOf layer Is cEcospaceLayerInteger Then
                        vc.Add(bm.InRow, bm.InCol, DirectCast(layer, cEcospaceLayerInteger))
                    End If
                End If
            Next j

            lResults.Add(New cHashValues(Me.Name, vn, cEncryptionUtilities.MD5(vc.Bytes())))
            vc.Clear()
        Next i
#End If

        Return lResults.ToArray()

    End Function

    Protected ReadOnly Property Core As cCore
        Get
            Return Me.m_core
        End Get
    End Property

End Class
