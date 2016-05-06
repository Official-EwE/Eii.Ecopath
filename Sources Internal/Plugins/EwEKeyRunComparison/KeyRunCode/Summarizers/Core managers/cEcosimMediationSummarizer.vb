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
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cEcosimMediationSummarizer
    Implements IHashSummarizer

    Private m_core As cCore

    Public Sub New(Core As cCore)
        Me.m_core = Core
    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "EcosimMediationFunctions"
    End Function

    Public Sub Init() Implements IHashSummarizer.Init

    End Sub

    Public Function HashValues() As cHashValues() Implements IHashSummarizer.HashValues

        Dim interactions As cMediatedInteractionManager = Me.m_core.MediatedInteractionManager
        Dim shapes As cMediationManager = Me.m_core.MediationShapeManager
        Dim shape As cForcingFunction = Nothing
        Dim sbSummary As New Text.StringBuilder()

        Dim lstHashValues As New List(Of cHashValues)

        ' Do not use for-each
        For i As Integer = 0 To shapes.Count - 1
            shape = shapes.Item(i)

            ' Iterate over all interactions, and write details
            For j As Integer = 1 To Me.m_core.nGroups
                For k As Integer = 1 To Me.m_core.nGroups
                    If interactions.isPredPrey(j, k) Then
                        Dim ppi As cPredPreyInteraction = interactions.PredPreyInteraction(j, k)
                        For l As Integer = 1 To ppi.MaxNumShapes
                            Dim shapeTest As cForcingFunction = Nothing
                            Dim appl As eForcingFunctionApplication
                            If ppi.getShape(l, shapeTest, appl) Then
                                If Object.ReferenceEquals(shape, shapeTest) Then
                                    If (sbSummary.Length > 0) Then sbSummary.Append("|")
                                    sbSummary.Append(cStringConverters.AppliedToString(ppi, DirectCast(shape, cMediationFunction), appl))
                                End If
                            End If
                        Next l
                    End If 'interactions.isPredPrey(j, k)
                Next k
            Next j
        Next i

        lstHashValues.Add(New cHashValues(Me.Name, "Mediation", sbSummary.ToString))
        Return lstHashValues.ToArray()

    End Function

End Class
