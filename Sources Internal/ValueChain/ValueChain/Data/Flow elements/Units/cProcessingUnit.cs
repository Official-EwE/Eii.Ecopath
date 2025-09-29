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

    [TypeConverter(typeof(cPropertySorter))]
    [DefaultProperty("Name")]
    [Serializable()]
    public class cProcessingUnit : cEconomicUnit
    {

        #region  Private variables 

        protected float m_AgriculturalProducts = 0.0f;
        protected float m_AgriculturalInput = 0f;

        #endregion

        public cProcessingUnit() : base()
        {
        }

        #region  Calculations 

        protected override bool Calculate(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            bool bSucces = base.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            // ..but adds Agricultural costs
            bSucces = bSucces & CalcAgriculturalCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            // ..but adds Agricultural revenue from such products, should there by any
            bSucces = bSucces & CalcAgriculturalProducts(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

            // JS 23 Apr 25: debugging for inconsistencies. No issues found here.
            // Console.WriteLine("{0} @{1} -> {2} > B > {3}, {4} > V {5}", Me.Name, iTimeStep, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue)

            return bSucces;

        }

        protected virtual bool CalcAgriculturalCost(cValueChainResults result, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float AgriCost = sOutputBiomass * AgriculturalInput;
            result.Store(this, cValueChainResults.eVariableType.CostAgriculture, AgriCost, iTimeStep);
            return true;

        }

        protected virtual bool CalcAgriculturalProducts(cValueChainResults result, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float AgriRevenue = sOutputBiomass * AgriculturalProducts;
            result.Store(this, cValueChainResults.eVariableType.RevenueAgriculture, AgriRevenue, iTimeStep);
            return true;

        }

        #endregion

        #region  Properties 

        [Browsable(true)]
        [Category(sPROPCAT_PRODUCTS)]
        [DisplayName("Revenue (agricultural)")]
        [Description("Revenue for agricultural products per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(101)]
        public float AgriculturalProducts
        {
            get
            {
                return m_AgriculturalProducts;
            }
            set
            {
                m_AgriculturalProducts = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Cost (agricultural)")]
        [Description("Agricultural input cost per tonnes of products")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(102)]
        public float AgriculturalInput
        {
            get
            {
                return m_AgriculturalInput;
            }
            set
            {
                m_AgriculturalInput = value;
                SetChanged();
            }
        }

        public override string Category
        {
            get
            {
                return "Processing";
            }
        }

        [Browsable(false)]
        public override cUnitFactory.eUnitType UnitType
        {
            get
            {
                return cUnitFactory.eUnitType.Processing;
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