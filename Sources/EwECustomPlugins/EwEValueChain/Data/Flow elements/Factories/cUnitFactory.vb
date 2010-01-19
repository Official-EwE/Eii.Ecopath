#Region " Imports "

Option Strict On

#End Region ' Imports

Public Class cUnitFactory

    Public Enum eUnitType As Integer
        All = 0
        Producer
        ' NonExtractive
        Processing
        Distribution
        Market
        Consumer
    End Enum

    Public Shared Function CreateUnit(ByVal tClass As Type) As cUnit
        If Not GetType(cUnit).IsAssignableFrom(tClass) Then Return Nothing
        Return CType(System.Activator.CreateInstance(tClass), cUnit)
    End Function

    Public Shared Function CreateUnit(ByVal unitType As eUnitType) As cUnit
        Return CreateUnit(MapType(unitType))
    End Function

    Public Shared Function MapType(ByVal unitType As eUnitType) As Type
        Dim t As Type = Nothing
        Select Case unitType
            Case eUnitType.Producer : t = GetType(cProducerUnit)
                'Case eUnitType.NonExtractive : t = GetType(cNonExtractiveUnit)
            Case eUnitType.Processing : t = GetType(cProcessingUnit)
            Case eUnitType.Distribution : t = GetType(cDistributionUnit)
            Case eUnitType.Market : t = GetType(cMarketUnit)
            Case eUnitType.Consumer : t = GetType(cConsumerUnit)
        End Select
        Return t
    End Function

    Public Shared Function CreateUnitDefault(ByVal unitType As eUnitType) As cUnit
        Dim t As Type = Nothing
        Select Case unitType
            Case eUnitType.Producer : t = GetType(cProducerUnitDefault)
                'Case eUnitType.NonExtractive : t = GetType(cNonExtractiveUnitDefault)
            Case eUnitType.Processing : t = GetType(cProcessingUnitDefault)
            Case eUnitType.Distribution : t = GetType(cDistributionUnitDefault)
            Case eUnitType.Market : t = GetType(cMarketUnitDefault)
            Case eUnitType.Consumer : t = GetType(cConsumerUnitDefault)
        End Select
        Return CType(System.Activator.CreateInstance(t), cUnit)
    End Function

End Class
