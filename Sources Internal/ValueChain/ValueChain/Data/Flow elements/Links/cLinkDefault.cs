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
#endregion

namespace ValueChain
{
    /// ===========================================================================
    /// <summary>
    /// Class for holding default link properties, used when forging new links 
    /// between units in the flow.
    /// </summary>
    /// ===========================================================================
    [TypeConverter(typeof(cPropertySorter))]
    [DefaultProperty("Name")]
    [Serializable()]
    public class cLinkDefault : cValueChainEntity
    {

        #region  Shared definitions 

        protected const string cCATEGORY_GENERIC = "1. Generic";
        protected const string cCATEGORY_TRANSFER = "2. Transfer";

        #endregion

        #region  Privates 

        private cLinkFactory.eLinkType m_linkType = cLinkFactory.eLinkType.ProducerToProcessing;
        /// <summary>Link output biomass ratio.</summary>
        private float m_sBiomassRatio = 1.0f;
        /// <summary>Link output value per ton.</summary>
        private float m_sValuePerTon = 1.0f;
        /// <summary>Link output value ratio.</summary>
        private float m_sValueRatio = 1f;

        /// <summary>Flag stating whether this unit is allowed to broadcast change events.</summary>
        private bool m_bAllowEvents = true;

        #endregion

        #region  Constructor 

        public cLinkDefault() : base()
        {
        }

        #endregion

        #region  Properties 

        [Browsable(true)]
        [Category(cCATEGORY_GENERIC)]
        [DisplayName("Name")]
        [Description("Name of this link")]
        [cPropertySorter.PropertyOrder(1)]
        public virtual string Name
        {
            get
            {
                return "";
            }
            set
            {
                // 
            }
        }

        [Browsable(false)]
        public int LinkType
        {
            get
            {
                return (int)m_linkType;
            }
            set
            {
                m_linkType = (cLinkFactory.eLinkType)value;
            }
        }

        [Browsable(true)]
        [Category(cCATEGORY_TRANSFER)]
        [DisplayName("Biomass ratio")]
        [Description("Ratio of biomass change (proportion, [0-1])")]
        [DefaultValue(1.0f)]
        [cPropertySorter.PropertyOrder(1)]
        public virtual float BiomassRatio
        {
            get
            {
                return m_sBiomassRatio;
            }
            set
            {
                m_sBiomassRatio = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(cCATEGORY_TRANSFER)]
        [DisplayName("Value per ton")]
        [Description("Value per ton")]
        [DefaultValue(1.0f)]
        [cPropertySorter.PropertyOrder(2)]
        public virtual float ValuePerTon
        {
            get
            {
                return m_sValuePerTon;
            }
            set
            {
                m_sValuePerTon = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(cCATEGORY_TRANSFER)]
        [DisplayName("Value ratio")]
        [Description("Value ratio, the ratio between value of product and value of raw material (the input to the previous box)")]
        [DefaultValue(1.0f)]
        [cPropertySorter.PropertyOrder(3)]
        public virtual float ValueRatio
        {
            get
            {
                return m_sValueRatio;
            }
            set
            {
                m_sValueRatio = value;
                SetChanged();
            }
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// States whether a link is visible in the interface.
    /// </summary>
    /// <returns>True by default.</returns>
    /// -----------------------------------------------------------------------
        public virtual bool IsVisible()
        {
            return true;
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------
        public override string ToString()
        {
            return m_linkType.ToString();
        }

        #endregion

        public virtual bool IsDefault
        {
            get
            {
                return true;
            }
        }

    }
}