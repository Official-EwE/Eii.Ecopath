#Region " Imports "

Option Strict On

#End Region ' Imports

Public Class cLinkFactory

    Public Enum eLinkType As Integer
        Unknown = 0
        ProducerToProcessing
        ProcessingToDistribution
        DistributionToMarket
        MarketToConsumer
    End Enum

    Public Shared Function GetLinkType(ByVal src As cUnit, ByVal tgt As cUnit) As eLinkType
        If TypeOf src Is cProducerUnit And TypeOf tgt Is cProcessingUnit Then Return eLinkType.ProducerToProcessing
        If TypeOf src Is cProcessingUnit And TypeOf tgt Is cDistributionUnit Then Return eLinkType.ProcessingToDistribution
        If TypeOf src Is cDistributionUnit And TypeOf tgt Is cMarketUnit Then Return eLinkType.DistributionToMarket
        If TypeOf src Is cMarketUnit And TypeOf tgt Is cConsumerUnit Then Return eLinkType.MarketToConsumer
        Return eLinkType.Unknown
    End Function

    Public Shared Function CanCreateLink(ByVal src As cUnit, ByVal tgt As cUnit) As Boolean
        ' Cannot link to metier
        If TypeOf (tgt) Is cProducerUnit Then Return False
        ' Cannot link from consumers
        If TypeOf (src) Is cConsumerUnit Then Return False
        ' For now all else is fine
        Return True
    End Function

    Public Shared Function CreateLinkDefault(ByVal linkType As eLinkType) As cLinkDefault
        Dim link As New cLinkDefault()
        link.LinkType = linkType
        Return link
    End Function

End Class
