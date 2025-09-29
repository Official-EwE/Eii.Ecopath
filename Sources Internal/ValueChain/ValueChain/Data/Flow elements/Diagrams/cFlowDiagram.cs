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
/// One single flow diagram.
/// </summary>
/// ===========================================================================
    [TypeConverter(typeof(cPropertySorter))]
    [DefaultProperty("Name")]
    [Serializable()]
    public class cFlowDiagram : cValueChainEntity
    {

        #region  Properties 

        [Browsable(true)]
        [DisplayName("Name")]
        [Description("Name of this diagram")]
        [cPropertySorter.PropertyOrder(1)]
        public virtual string Name { get; set; } = "Default";

        #endregion

    }
}