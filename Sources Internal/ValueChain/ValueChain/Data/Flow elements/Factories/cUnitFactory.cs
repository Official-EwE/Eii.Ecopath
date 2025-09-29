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

using System;

namespace ValueChain
{

    #endregion

    public class cUnitFactory
    {

        public enum eUnitType : int
        {
            All = 0,
            Producer,
            Processing,
            Distribution,
            Wholesaler,
            Retailer,
            Consumer
        }

        public static cUnit CreateUnit(Type tClass)
        {
            if (!typeof(cUnit).IsAssignableFrom(tClass))
                return null;
            return (cUnit)Activator.CreateInstance(tClass);
        }

        public static cUnit CreateUnit(eUnitType unitType)
        {
            return CreateUnit(MapType(unitType));
        }

        public static Type MapType(eUnitType unitType)
        {
            Type t = null;
            switch (unitType)
            {
                case eUnitType.Producer:
                    {
                        t = typeof(cProducerUnit);
                        break;
                    }
                case eUnitType.Processing:
                    {
                        t = typeof(cProcessingUnit);
                        break;
                    }
                case eUnitType.Distribution:
                    {
                        t = typeof(cDistributionUnit);
                        break;
                    }
                case eUnitType.Wholesaler:
                    {
                        t = typeof(cWholesalerUnit);
                        break;
                    }
                case eUnitType.Retailer:
                    {
                        t = typeof(cRetailerUnit);
                        break;
                    }
                case eUnitType.Consumer:
                    {
                        t = typeof(cConsumerUnit);
                        break;
                    }
            }
            return t;
        }

        public static cUnit CreateUnitDefault(eUnitType unitType)
        {
            Type t = null;
            switch (unitType)
            {
                case eUnitType.Producer:
                    {
                        t = typeof(cProducerUnitDefault);
                        break;
                    }
                case eUnitType.Processing:
                    {
                        t = typeof(cProcessingUnitDefault);
                        break;
                    }
                case eUnitType.Distribution:
                    {
                        t = typeof(cDistributionUnitDefault);
                        break;
                    }
                case eUnitType.Wholesaler:
                    {
                        t = typeof(cWholesalerUnitDefault);
                        break;
                    }
                case eUnitType.Retailer:
                    {
                        t = typeof(cRetailerUnitDefault);
                        break;
                    }
                case eUnitType.Consumer:
                    {
                        t = typeof(cConsumerUnitDefault);
                        break;
                    }
            }
            return (cUnit)Activator.CreateInstance(t);
        }

    }
}