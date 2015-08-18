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

Public Class cEcosimEffortSummarizer
    Implements IHashSummarizer

    Private m_core As cCore

    Public Sub New(Core As cCore)
        Me.m_core = Core
    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "EcosimFishingEffort"
    End Function

    Public Sub Init() Implements IHashSummarizer.Init

    End Sub

    Public Function HashValues() As cHashValues() Implements IHashSummarizer.HashValues

        Dim EffortShps As cFishingEffortManger = Me.m_core.FishingEffortShapeManager
        Dim shape As cForcingFunction = Nothing
        Dim sbSummary As New Text.StringBuilder()

        Dim lstHashValues As New List(Of cHashValues)

        ' Do not use for-each
        For i As Integer = 0 To EffortShps.Count - 1
            shape = EffortShps.Item(i)
            If (sbSummary.Length > 0) Then sbSummary.Append("|")
            sbSummary.Append("f=" & i)
            sbSummary.Append(cStringConverters.ShapeToString(shape))

        Next

        lstHashValues.Add(New cHashValues(Me.Name, "FishingEffort", sbSummary.ToString))
        Return lstHashValues.ToArray()

    End Function

End Class
