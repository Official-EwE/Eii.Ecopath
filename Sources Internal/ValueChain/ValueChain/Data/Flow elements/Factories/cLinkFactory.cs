// ===============================================================================
// This file is part of Ecopath with Ecosim (EwE)
//
// EwE is free software: you can redistribute it and/or modify it under the terms
// of the GNU General Public License version 2 as published by the Free Software 
// Foundation.
//
// EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
// PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with EwE.
// If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
//
//
// Copyright 1991- 
//    Ecopath International Initiative, Barcelona, Spain
// ===============================================================================

#region  Imports 


namespace ValueChain
{

    #endregion

    public class cLinkFactory
    {

        public enum eLinkType : int
        {
            Unknown = 0,
            ProducerToProcessing,
            ProcessingToDistribution,
            DistributionToWholeseller,
            WholesellerToRetailer,
            RetailerToConsumer
        }

        public static eLinkType GetLinkType(cUnit src, cUnit tgt)
        {
            if (src is cProducerUnit & tgt is cProcessingUnit)
                return eLinkType.ProducerToProcessing;
            if (src is cProcessingUnit & tgt is cDistributionUnit)
                return eLinkType.ProcessingToDistribution;
            if (src is cDistributionUnit & tgt is cWholesalerUnit)
                return eLinkType.DistributionToWholeseller;
            if (src is cWholesalerUnit & tgt is cRetailerUnit)
                return eLinkType.WholesellerToRetailer;
            if (src is cRetailerUnit & tgt is cConsumerUnit)
                return eLinkType.RetailerToConsumer;
            return eLinkType.Unknown;
        }

        public static bool CanCreateLink(cUnit src, cUnit tgt)
        {
            // Cannot link to producers
            if (tgt is cProducerUnit)
                return false;
            // Cannot link from consumers
            if (src is cConsumerUnit)
                return false;
            // For now all else is fine
            return true;
        }

        public static cLinkDefault CreateLinkDefault(eLinkType linkType)
        {
            var link = new cLinkDefault();
            link.LinkType = (int)linkType;
            return link;
        }

    }
}