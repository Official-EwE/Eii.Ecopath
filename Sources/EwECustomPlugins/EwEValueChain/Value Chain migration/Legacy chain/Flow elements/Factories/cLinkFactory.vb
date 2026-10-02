' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace ValueChainMigrator.LegacyData

    ''' ===========================================================================
    ''' <summary>
    ''' Legacy factory class. Maintained for Access -> SQLite database conversion. 
    ''' Not used in the current implementation 
    ''' of the value chain.
    ''' </summary>
    ''' ===========================================================================
    <Obsolete()>
    Public Class cLinkFactory

        Public Enum eLinkType As Integer
            Unknown = 0
            ProducerToProcessing
            ProcessingToDistribution
            DistributionToWholeseller
            WholesellerToRetailer
            RetailerToConsumer
        End Enum

    End Class

End Namespace
