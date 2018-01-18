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

#End Region ' Imports

Public Class cUnitFactory

    Public Enum cUnitFormatter As Integer
        All = 0
        Producer
        Processing
        Distribution
        Wholesaler
        Retailer
        Consumer
    End Enum

    Public Shared Function CreateUnit(ByVal tClass As Type) As cUnit
        If Not GetType(cUnit).IsAssignableFrom(tClass) Then Return Nothing
        Return CType(System.Activator.CreateInstance(tClass), cUnit)
    End Function

    Public Shared Function CreateUnit(ByVal unitType As cUnitFormatter) As cUnit
        Return CreateUnit(MapType(unitType))
    End Function

    Public Shared Function MapType(ByVal unitType As cUnitFormatter) As Type
        Dim t As Type = Nothing
        Select Case unitType
            Case cUnitFormatter.Producer : t = GetType(cProducerUnit)
            Case cUnitFormatter.Processing : t = GetType(cProcessingUnit)
            Case cUnitFormatter.Distribution : t = GetType(cDistributionUnit)
            Case cUnitFormatter.Wholesaler : t = GetType(cWholesalerUnit)
            Case cUnitFormatter.Retailer : t = GetType(cRetailerUnit)
            Case cUnitFormatter.Consumer : t = GetType(cConsumerUnit)
        End Select
        Return t
    End Function

    Public Shared Function CreateUnitDefault(ByVal unitType As cUnitFormatter) As cUnit
        Dim t As Type = Nothing
        Select Case unitType
            Case cUnitFormatter.Producer : t = GetType(cProducerUnitDefault)
            Case cUnitFormatter.Processing : t = GetType(cProcessingUnitDefault)
            Case cUnitFormatter.Distribution : t = GetType(cDistributionUnitDefault)
            Case cUnitFormatter.Wholesaler : t = GetType(cWholesalerUnitDefault)
            Case cUnitFormatter.Retailer : t = GetType(cRetailerUnitDefault)
            Case cUnitFormatter.Consumer : t = GetType(cConsumerUnitDefault)
        End Select
        Return CType(System.Activator.CreateInstance(t), cUnit)
    End Function

End Class
