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
using System.ComponentModel;

namespace ValueChain
{

    #endregion

    /// ===========================================================================
/// <summary>
/// This class represents a group of Consumers in the Ecost economic model.
/// Consumers form the end of economic flow chains.
/// </summary>
/// ===========================================================================
    [TypeConverter(typeof(cPropertySorter))]
    [DefaultProperty("Name")]
    [Serializable()]
    public class cConsumerUnit : cUnit
    {

        public cConsumerUnit() : base()
        {
        }

        #region  Calculations 

        #endregion

        #region  Properties 

        #region  General 

        public override string Category
        {
            get
            {
                return "Consumer";
            }
        }

        #endregion

        [Browsable(false)]
        public override cUnitFactory.eUnitType UnitType
        {
            get
            {
                return cUnitFactory.eUnitType.Consumer;
            }
        }

        #endregion

        public override bool IsDefault
        {
            get
            {
                return false;
            }
        }

    }
}