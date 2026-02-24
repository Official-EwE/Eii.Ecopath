' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cUnitFactory

    Public Enum eUnitType As Integer
        All = 0
        Producer
        Processing
        Distribution
        Wholesaler
        Retailer
        Consumer
    End Enum

    Public Shared Function CreateUnit(tClass As Type) As cUnit
        If Not GetType(cUnit).IsAssignableFrom(tClass) Then Return Nothing
        Return CType(System.Activator.CreateInstance(tClass), cUnit)
    End Function

    Public Shared Function CreateUnit(unitType As eUnitType) As cUnit
        Return CreateUnit(MapType(unitType))
    End Function

    Public Shared Function MapType(unitType As eUnitType) As Type
        Dim t As Type = Nothing
        Select Case unitType
            Case eUnitType.Producer : t = GetType(cProducerUnit)
            Case eUnitType.Processing : t = GetType(cProcessingUnit)
            Case eUnitType.Distribution : t = GetType(cDistributionUnit)
            Case eUnitType.Wholesaler : t = GetType(cWholesalerUnit)
            Case eUnitType.Retailer : t = GetType(cRetailerUnit)
            Case eUnitType.Consumer : t = GetType(cConsumerUnit)
        End Select
        Return t
    End Function

    Public Shared Function CreateUnitDefault(unitType As eUnitType) As cUnit
        Dim t As Type = Nothing
        Select Case unitType
            Case eUnitType.Producer : t = GetType(cProducerUnitDefault)
            Case eUnitType.Processing : t = GetType(cProcessingUnitDefault)
            Case eUnitType.Distribution : t = GetType(cDistributionUnitDefault)
            Case eUnitType.Wholesaler : t = GetType(cWholesalerUnitDefault)
            Case eUnitType.Retailer : t = GetType(cRetailerUnitDefault)
            Case eUnitType.Consumer : t = GetType(cConsumerUnitDefault)
        End Select
        Return CType(System.Activator.CreateInstance(t), cUnit)
    End Function

End Class
