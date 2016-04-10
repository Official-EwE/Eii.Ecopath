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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Text
Imports EwECore
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cStringConverters

    Public Shared Function ShapeToString(shapeData As cShapeData) As String

        If (shapeData Is Nothing) Then Return String.Empty
        Dim sb As New StringBuilder()

        Dim pts As Single() = shapeData.ShapeData ' Note that ShapeData returns a copy!
        For i As Integer = 0 To shapeData.nPoints - 1
            If (sb.Length > 0) Then sb.Append(",")
            sb.Append(cStringUtils.FormatNumber(pts(i), iNumDigits:=3, bFindRelevantDecimals:=True))
        Next

        ' Do not add the individual points; the string will become unusable in length for any practical purpose. 
        ' A hash is sufficient to know that the point data is somehow different
        Return cEncryptionUtilities.MD5(sb.ToString())

    End Function

    Public Shared Function AppliedToString(interaction As cPredPreyInteraction, ForcingFunction As cForcingFunction, appl As eForcingFunctionApplication) As String
        Dim sb As New Text.StringBuilder
        sb.Append(cStringConverters.InteractionToString(interaction.PreyIndex, "i", interaction.PreyIndex, "j", appl))
        sb.Append(cStringConverters.ForcingFunctionToString(ForcingFunction))
        Return sb.ToString

    End Function


    Public Shared Function AppliedToString(interaction As cPredPreyInteraction, ForcingFunction As cMediationFunction, appl As eForcingFunctionApplication) As String
        Dim sb As New Text.StringBuilder
        sb.Append(cStringConverters.InteractionToString(interaction.PredIndex, "i", interaction.PreyIndex, "j", appl))
        sb.Append(cStringConverters.ForcingFunctionToString(ForcingFunction))
        Return sb.ToString

    End Function


    Public Shared Function AppliedToString(interaction As cLandingsInteraction, ForcingFunction As cLandingsMediationFunction, appl As eForcingFunctionApplication) As String
        Dim sb As New Text.StringBuilder
        sb.Append(cStringConverters.InteractionToString(interaction.FleetIndex, "f", interaction.GroupIndex, "g", appl))
        sb.Append(cStringConverters.ForcingFunctionToString(ForcingFunction))
        Return sb.ToString

    End Function

    Private Shared Function InteractionToString(iA As Integer, Astr As String, iB As Integer, Bstr As String, appl As eForcingFunctionApplication) As String
        Return Astr & "=" & cStringUtils.FormatNumber(iA) & _
                 "," & Bstr & "=" & cStringUtils.FormatNumber(iB) & _
                  ",a=" & cStringUtils.FormatNumber(CInt(appl))
    End Function

    Private Shared Function ForcingFunctionToString(MedFunc As cMediationFunction) As String
        Dim sb As New Text.StringBuilder
        sb.Append("{")

        For i As Integer = 0 To MedFunc.NumGroups - 1
            sb.Append("g=" & MedFunc.Group(i).iGroupIndex & ",")
        Next

        For i As Integer = 0 To MedFunc.NumFleet - 1
            sb.Append("f=" & MedFunc.Fleet(i).iFleetIndex & ",")
        Next
        sb.Append("}")
        sb.Append(cStringConverters.ShapeToString(MedFunc))
        Return sb.ToString

    End Function

    Private Shared Function ForcingFunctionToString(MedFunc As cLandingsMediationFunction) As String
        Dim sb As New Text.StringBuilder
        sb.Append(":")

        For i As Integer = 0 To MedFunc.NumGroups - 1
            Dim grp As cLandingsMediatingGroup = DirectCast(MedFunc.Group(i), cLandingsMediatingGroup)
            sb.Append("{g=" & grp.iGroupIndex & ",f=" & grp.iFleetIndex & ",w=" & grp.Weight & "}")
        Next
        sb.Append(" ")
        sb.Append(cStringConverters.ShapeToString(MedFunc))
        Return sb.ToString

    End Function

    Private Shared Function ForcingFunctionToString(ForcingFunction As cForcingFunction) As String
        Return cStringConverters.ShapeToString(ForcingFunction)
    End Function

    Public Shared Function LayerToString(nRow As Integer, nCol As Integer, layer As cEcospaceLayer) As String

        If (layer Is Nothing) Then Return String.Empty
        Dim sb As New StringBuilder()

        For iRow As Integer = 1 To nRow
            For iCol As Integer = 1 To nCol
                Dim value As Object = layer.Cell(iRow, iCol)

                If (TypeOf layer Is cEcospaceLayerVector) Then
                    Dim vector As Single() = DirectCast(value, Single())
                    sb.Append(cStringUtils.FormatNumber(vector(0), iNumDigits:=3, bFindRelevantDecimals:=True))
                    sb.Append(cStringUtils.FormatNumber(vector(1), iNumDigits:=3, bFindRelevantDecimals:=True))
                Else
                    sb.Append(cStringUtils.FormatNumber(layer.Cell(iRow, iCol), iNumDigits:=3, bFindRelevantDecimals:=True))
                End If
            Next
        Next
        ' Do not add the individual map data; the string will become unusable in length for any practical purpose. 
        ' A hash is sufficient to know that the point data is somehow different
        Return cEncryptionUtilities.MD5(sb.ToString())

    End Function

    Public Shared Function ConnectionToString(conn As cSpatialDataConnection) As String

        If (conn Is Nothing) Then Return String.Empty
        If (Not conn.IsConfigured()) Then Return String.Empty

        Dim sb As New StringBuilder()
        sb.Append("d=" & cStringConverters.DatasetToString(conn.Dataset))
        sb.Append(",")
        sb.Append("c=" & cStringConverters.ConverterToString(conn.Converter))
        sb.Append(",")
        sb.Append("s=" & cStringUtils.FormatNumber(conn.Scale, iNumDigits:=3, bFindRelevantDecimals:=True))
        sb.Append(",")
        sb.Append("t=" & cStringUtils.FormatNumber(conn.ScaleType))
        Return sb.ToString()

    End Function

    Private Shared Function DatasetToString(dataset As ISpatialDataSet) As String
        If (dataset Is Nothing) Then Return String.Empty
        ' Do not add the individual dataset data; the string will become unusable in length for any practical purpose. 
        ' A hash is sufficient to know that the point data is somehow different
        Return cEncryptionUtilities.MD5(dataset.Summary())
    End Function

    Private Shared Function ConverterToString(converter As ISpatialDataConverter) As String
        If (converter Is Nothing) Then Return String.Empty
        ' Do not add the individual converter data; the string will become unusable in length for any practical purpose. 
        ' A hash is sufficient to know that the point data is somehow different
        Return cEncryptionUtilities.MD5(converter.Summary())
    End Function

End Class
