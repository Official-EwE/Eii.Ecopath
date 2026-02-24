' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cLinkFactory

    Public Enum eLinkType As Integer
        Unknown = 0
        ProducerToProcessing
        ProcessingToDistribution
        DistributionToWholeseller
        WholesellerToRetailer
        RetailerToConsumer
    End Enum

    Public Shared Function GetLinkType(src As cUnit, tgt As cUnit) As eLinkType
        If TypeOf src Is cProducerUnit And TypeOf tgt Is cProcessingUnit Then Return eLinkType.ProducerToProcessing
        If TypeOf src Is cProcessingUnit And TypeOf tgt Is cDistributionUnit Then Return eLinkType.ProcessingToDistribution
        If TypeOf src Is cDistributionUnit And TypeOf tgt Is cWholesalerUnit Then Return eLinkType.DistributionToWholeseller
        If TypeOf src Is cWholesalerUnit And TypeOf tgt Is cRetailerUnit Then Return eLinkType.WholesellerToRetailer
        If TypeOf src Is cRetailerUnit And TypeOf tgt Is cConsumerUnit Then Return eLinkType.RetailerToConsumer
        Return eLinkType.Unknown
    End Function

    Public Shared Function CanCreateLink(src As cUnit, tgt As cUnit) As Boolean
        ' Cannot link to producers
        If TypeOf (tgt) Is cProducerUnit Then Return False
        ' Cannot link from consumers
        If TypeOf (src) Is cConsumerUnit Then Return False
        ' For now all else is fine
        Return True
    End Function

    Public Shared Function CreateLinkDefault(linkType As eLinkType) As cLinkDefault
        Dim link As New cLinkDefault()
        link.LinkType = linkType
        Return link
    End Function

End Class
