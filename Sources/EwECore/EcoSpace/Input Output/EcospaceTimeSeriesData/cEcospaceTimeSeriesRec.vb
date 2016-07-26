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

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core


Namespace EcospaceTimeSeries

    Public Class cEcospaceTimeSeriesRec

        Private Enum eDataCols
            Row
            Col
            GroupID
            Timestamp
            Value
        End Enum

        Public iGroupID As Integer
        Public Row As Integer
        Public Col As Integer
        Public TimeStamp As Date
        Public CellValue As Single

        Public VarType As eVarNameFlags

        Public Sub New(strRec As String, Optional DataType As eVarNameFlags = eVarNameFlags.EcospaceMapBiomass)
            Dim data() As String = EwEUtils.Utilities.cStringUtils.SplitQualified(strRec, ",")

            Me.Row = EwEUtils.Utilities.cStringUtils.ConvertToInteger(data(eDataCols.Row))
            Me.Col = EwEUtils.Utilities.cStringUtils.ConvertToInteger(data(eDataCols.Col))
            Me.iGroupID = EwEUtils.Utilities.cStringUtils.ConvertToInteger(data(eDataCols.GroupID))
            Me.TimeStamp = EwEUtils.Utilities.cStringUtils.ConvertToDate(data(eDataCols.Timestamp))
            Me.CellValue = EwEUtils.Utilities.cStringUtils.ConvertToSingle(data(eDataCols.Value))

            VarType = DataType
        End Sub

        Shared Function FromString(strRec As String, Optional DataType As eVarNameFlags = eVarNameFlags.EcospaceMapBiomass) As cEcospaceTimeSeriesRec
            Return New cEcospaceTimeSeriesRec(strRec, DataType)
        End Function

    End Class

End Namespace

