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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cEcosimPriceElasticitySummarizer
    Implements IHashSummarizer

    Private m_core As cCore

    Public Sub New(Core As cCore)
        Me.m_core = Core
    End Sub

    Public Sub Init() Implements IHashSummarizer.Init

    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "EcosimPriceFunctions"
    End Function

    Public Function HashValues() As cHashValues() Implements IHashSummarizer.HashValues

        Dim interactions As cMediatedInteractionManager = Me.m_core.MediatedInteractionManager
        Dim shapes As cLandingsMediationManager = Me.m_core.LandingsShapeManager
        Dim shape As cForcingFunction = Nothing
        Dim sbSummary As New Text.StringBuilder()

        Dim lstHashValues As New List(Of cHashValues)
        For i As Integer = 0 To shapes.Count - 1
            shape = shapes.Item(i)

            If (sbSummary.Length > 0) Then sbSummary.Append("|")

            ' Iterate over all interactions, and write details
            For iflt As Integer = 1 To Me.m_core.nFleets
                For iGrp As Integer = 1 To Me.m_core.nGroups
                    If interactions.isLandings(iflt, iGrp) Then
                        Dim fgi As cLandingsInteraction = interactions.LandingInteraction(iflt, iGrp)
                        For l As Integer = 1 To fgi.MaxNumShapes
                            Dim shapeTest As cForcingFunction = Nothing
                            Dim appl As eForcingFunctionApplication
                            If fgi.getShape(l, shapeTest, appl) Then
                                If Object.ReferenceEquals(shape, shapeTest) Then

                                    sbSummary.Append(cStringConverters.AppliedToString(fgi, DirectCast(shape, cLandingsMediationFunction), appl))

                                End If
                            End If 'If fgi.getShape(l, shapeTest, appl) Then
                        Next l
                    End If
                Next iGrp
            Next iflt
        Next i

        lstHashValues.Add(New cHashValues(Me.Name, "PriceElasticity", sbSummary.ToString))
        Return lstHashValues.ToArray()

    End Function

End Class
