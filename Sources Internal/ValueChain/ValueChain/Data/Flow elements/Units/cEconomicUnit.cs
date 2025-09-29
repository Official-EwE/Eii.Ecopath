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
    public abstract class cEconomicUnit : cUnit
    {

        #region  Private variables 

        private float m_WorkerFemale = 0.0f;
        private float m_WorkerMale = 0.0f;
        private float m_WorkerOther = 0.0f;
        private float m_WorkerMalePay = 0.0f;
        private float m_WorkerFemalePay = 0.0f;
        private float m_WorkerOtherPay = 0.0f;
        private float m_WorkerMaleShare = 0.0f;
        private float m_WorkerFemaleShare = 0.0f;
        private float m_WorkerOtherShare = 0.0f;
        private float m_WorkerMaleDependents = 0.0f;
        private float m_WorkerFemaleDependents = 0.0f;
        private float m_WorkerParttime = 0.0f;
        private float m_OwnerMale = 0.0f;
        private float m_OwnerFemale = 0.0f;
        private float m_OwnerMalePay = 0.0f;
        private float m_OwnerFemalePay = 0.0f;
        private float m_OwnerMaleShare = 0.0f;
        private float m_OwnerFemaleShare = 0.0f;
        private float m_OwnerMaleDependents = 0f;
        private float m_OwnerFemaleDependents = 0f;
        private float m_EnergyProducts = 0f;
        private float m_IndustrialProducts = 0f;
        private float m_ServiceProducts = 0f;
        private float m_EnergyCost = 0f;
        private float m_CapitalCost = 0f;
        private float m_IndustrialCost = 0f;
        private float m_ServiceCost = 0f;
        private float m_ManagementCost = 0f;
        private float m_RoyaltyCost = 0f;
        private float m_CertificationCost = 0f;
        private float m_TaxesLicense = 0f;
        private float m_TaxesProfit = 0f;
        private float m_TaxesVAT = 0f;
        private float m_TaxesImport = 0f;
        private float m_TaxesExport = 0f;
        private float m_TaxesEnvironmental = 0f;
        private float m_TaxesProduction = 0f;
        private float m_SubsidyEnergy = 0f;
        private float m_SubsidyOther = 0f;

        // Public Amount As Single         'Amount in tonnes 
        // Public Benefit As Single        '
        // Public CapitalAmount As Single  'number of Capital units    VC DON'T THINK THIS IS NEEDED, ONLY PER TONNES
        // Public CapitalCost As Single    'per tons
        // Public EmployeesFemale As Single    'per tons
        // Public EmployeesMale As Single      'per tons
        // Public EmployersFemale As Single    'per tons
        // Public EmployersMale As Single      'per tons
        // Public LabourAmount As Single       'number of labour units per tons
        // Public LabourCost As Single         'per tons
        // Public ManagementCost As Single 'per unit produced?
        // Public ProductionUnits As Single     'Number of units per tons of product (boats, processors, distributors, etc)
        // Public RawAmount As Single      'Unit cost for buying a tonnes of raw material
        // Public RawCost As Single        'Unit cost for buying a tonnes of raw material
        // Public Price As Single          'Price for each product per tons
        // Public Revenue As Single        'Total revenue 
        // Public Subsidy As Single        'per tons produced
        // Public TaxEnvironmentalRate As Single   'per tonnes produced
        // Public TaxProductionRate As Single      'per tonnes produced
        // Public ProcessingCost As Single       'Cost for processing one tons (is in addition to raw cost)
        // Public Value As Single          'Value of production
        // Public WageFemale As Single     '$ per year
        // Public WageMale As Single       '$ per year

        private bool m_bBroker = false;

        #endregion

        #region  Constructor 

        /// -----------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------
        public cEconomicUnit() : base()
        {
        }

        #endregion

        #region  Calculations 

        protected override bool Calculate(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            // The production unit needs to do the same calculations as the MyBase=cEconomicUnit, but:
            bool bSucces = base.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

            // Production in weight
            results.Store(this, cValueChainResults.eVariableType.Production, sOutputBiomass, iTimeStep);

            bSucces = bSucces & CalcProductionLiveWeight(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

            // Revenue
            bSucces = bSucces & CalcProducts(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcSubsidy(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

            // Cost
            bSucces = bSucces & CalcRawmaterialCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcInputCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcManagementRoyaltyCertificationCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

            bSucces = bSucces & CalcTax(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcWorkerPay(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcOwnerPay(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

            // Social
            bSucces = bSucces & CalcWorkerFemales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcWorkerMales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcWorkerParttime(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcWorkerOther(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcOwnerFemales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcOwnerMales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcWorkerDependents(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            bSucces = bSucces & CalcOwnerDependents(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

            return bSucces;

        }

        #region  Production (weight)

        #endregion

        protected virtual bool CalcProductionLiveWeight(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float ToBeCalculated = 0f;
            results.Store(this, cValueChainResults.eVariableType.ProductionLive, ToBeCalculated, iTimeStep);

            return true;
        }

        #region  Revenue 

        protected virtual bool CalcProducts(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * (EnergyProducts + IndustrialProducts + ServiceProducts);

            results.Store(this, cValueChainResults.eVariableType.RevenueProductsOther, sSum, iTimeStep);
            if (Broker == false)
            {
                results.Store(this, cValueChainResults.eVariableType.RevenueProductsMain, sOutputValue, iTimeStep);
            }
            return true;
        }

        protected virtual bool CalcSubsidy(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * (SubsidyEnergy + SubsidyOther);
            results.Store(this, cValueChainResults.eVariableType.RevenueSubsidies, sSum, iTimeStep);
            return true;
        }

        #endregion

        #region  Cost 

        protected virtual bool CalcRawmaterialCost(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            if (Broker == false)
            {
                // Dim sSum As Single = sInputBiomass * sInputValue
                results.Store(this, cValueChainResults.eVariableType.CostRawmaterial, sInputValue, iTimeStep);
            }
            return true;
        }

        protected virtual bool CalcInputCost(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * (CapitalInput + EnergyCost + IndustrialCost + ServiceCost);
            results.Store(this, cValueChainResults.eVariableType.CostInput, sSum, iTimeStep);
            return true;
        }

        protected virtual bool CalcManagementRoyaltyCertificationCost(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * (m_ManagementCost + m_RoyaltyCost + m_CertificationCost);
            results.Store(this, cValueChainResults.eVariableType.CostManagementRoyaltyCertification, sSum, iTimeStep);
            return true;
        }



        protected virtual bool CalcTax(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * (TaxEnvironmental + TaxExport + TaxProduction + TaxVAT + m_TaxesImport + LicenseTax);
            // profit tax is calculated later, after all revenue and (other) cost is known (VC111117)
            results.Store(this, cValueChainResults.eVariableType.CostTaxes, sSum, iTimeStep);
            return true;
        }

        protected virtual bool CalcWorkerPay(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum;
            if (m_WorkerMalePay + m_WorkerFemalePay > 0f)
            {
                sSum = sOutputBiomass * (m_WorkerMalePay + m_WorkerFemalePay);
            }
            else
            {
                sSum = sOutputValue * (m_WorkerMaleShare + m_WorkerFemaleShare) / 100f;
            }
            results.Store(this, cValueChainResults.eVariableType.CostWorker, sSum, iTimeStep);
            return true;
        }

        protected virtual bool CalcOwnerPay(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum;
            if (m_OwnerMalePay + m_OwnerFemalePay > 0f)
            {
                sSum = sOutputBiomass * (m_OwnerMalePay + m_OwnerFemalePay);
            }
            else
            {
                sSum = sOutputValue * (m_OwnerMaleShare + m_OwnerFemaleShare) / 100f;
            }
            results.Store(this, cValueChainResults.eVariableType.CostOwner, sSum, iTimeStep);
            return true;
        }

        #endregion

        #region  Social 

        protected virtual bool CalcWorkerFemales(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * m_WorkerFemale;
            results.Store(this, cValueChainResults.eVariableType.NumberOfWorkerFemales, sSum, iTimeStep);
            return true;
        }

        protected virtual bool CalcWorkerMales(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * m_WorkerMale;
            results.Store(this, cValueChainResults.eVariableType.NumberOfWorkerMales, sSum, iTimeStep);

            return true;
        }

        protected virtual bool CalcWorkerParttime(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * m_WorkerParttime;
            results.Store(this, cValueChainResults.eVariableType.NumberOfWorkerPartTime, sSum, iTimeStep);
            return true;
        }

        protected virtual bool CalcWorkerOther(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * m_WorkerOther;
            results.Store(this, cValueChainResults.eVariableType.NumberOfWorkerOther, sSum, iTimeStep);

            return true;
        }

        protected virtual bool CalcOwnerMales(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * m_OwnerMale;
            results.Store(this, cValueChainResults.eVariableType.NumberOfOwnerMales, sSum, iTimeStep);

            return true;
        }

        protected virtual bool CalcOwnerFemales(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * m_OwnerFemale;
            results.Store(this, cValueChainResults.eVariableType.NumberOfOwnerFemales, sSum, iTimeStep);
            return true;
        }

        protected virtual bool CalcWorkerDependents(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * (m_WorkerFemaleDependents * m_WorkerFemale + m_WorkerMaleDependents * m_WorkerMale);
            results.Store(this, cValueChainResults.eVariableType.NumberOfWorkerDependents, sSum, iTimeStep);
            return true;
        }

        protected virtual bool CalcOwnerDependents(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            float sSum = sOutputBiomass * (m_OwnerFemaleDependents * m_OwnerFemale + m_OwnerMaleDependents * m_OwnerMale);
            results.Store(this, cValueChainResults.eVariableType.NumberOfOwnerDependents, sSum, iTimeStep);
            return true;
        }

        #endregion

        #endregion

        #region  Properties 

        #region  Products 

        [Browsable(true)]
        [Category(sPROPCAT_PRODUCTS)]
        [DisplayName("Energy products")]
        [Description("Energy products per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(2)]
        public float EnergyProducts
        {
            get
            {
                return m_EnergyProducts;
            }
            set
            {
                m_EnergyProducts = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_PRODUCTS)]
        [DisplayName("Industrial products")]
        [Description("Revenue of industrial products per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(3)]
        public float IndustrialProducts
        {
            get
            {
                return m_IndustrialProducts;
            }
            set
            {
                m_IndustrialProducts = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_PRODUCTS)]
        [DisplayName("Service products")]
        [Description("Revenue of services per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(4)]
        public float ServiceProducts
        {
            get
            {
                return m_ServiceProducts;
            }
            set
            {
                m_ServiceProducts = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SUBSIDIES)]
        [DisplayName("Energy subsidy")]
        [Description("Energy subsidy per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(1)]
        public float SubsidyEnergy
        {
            get
            {
                return m_SubsidyEnergy;
            }
            set
            {
                m_SubsidyEnergy = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SUBSIDIES)]
        [DisplayName("Other subsidies")]
        [Description("Other subsidies per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(2)]
        public float SubsidyOther
        {
            get
            {
                return m_SubsidyOther;
            }
            set
            {
                m_SubsidyOther = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_GENERAL)]
        [DisplayName("Broker")]
        [Description("States whether this unit functions as a broker")]
        [cPropertySorter.PropertyOrder(5)]
        public virtual bool Broker
        {
            get
            {
                return m_bBroker;
            }
            set
            {
                m_bBroker = value;
                SetChanged();
            }
        }
        #endregion

        #region  Pay 

        [Browsable(true)]
        [Category(sPROPCAT_PAY)]
        [DisplayName("Female worker pay")]
        [Description("Female worker pay per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(1)]
        public float WorkerFemalePay
        {
            get
            {
                return m_WorkerFemalePay;
            }
            set
            {
                m_WorkerFemalePay = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_PAY)]
        [DisplayName("Male worker pay")]
        [Description("Male worker pay per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(2)]
        public float WorkerMalePay
        {
            get
            {
                return m_WorkerMalePay;
            }
            set
            {
                m_WorkerMalePay = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_PAY)]
        [DisplayName("Female owners pay")]
        [Description("Female owners pay per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(3)]
        public float OwnerFemalePay
        {
            get
            {
                return m_OwnerFemalePay;
            }
            set
            {
                m_OwnerFemalePay = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_PAY)]
        [DisplayName("Male owners pay")]
        [Description("Male owners pay per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(4)]
        public float OwnerMalePay
        {
            get
            {
                return m_OwnerMalePay;
            }
            set
            {
                m_OwnerMalePay = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_PAY)]
        [DisplayName("Other worker pay")]
        [Description("Other worker pay per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(10)]
        public float WorkerOtherPay
        {
            get
            {
                return m_WorkerOtherPay;
            }
            set
            {
                m_WorkerOtherPay = value;
                SetChanged();
            }
        }

        #endregion

        #region  Share 

        [Browsable(true)]
        [Category(sPROPCAT_SHARE)]
        [DisplayName("Female worker share")]
        [Description("Female worker share in % of revenue")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(1)]
        public float WorkerFemaleshare
        {
            get
            {
                return m_WorkerFemaleShare;
            }
            set
            {
                m_WorkerFemaleShare = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SHARE)]
        [DisplayName("Male worker share")]
        [Description("Male worker share in % of revenue")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(2)]
        public float WorkerMaleshare
        {
            get
            {
                return m_WorkerMaleShare;
            }
            set
            {
                m_WorkerMaleShare = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SHARE)]
        [DisplayName("Female owners share")]
        [Description("Female owners share in % of revenue")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(3)]
        public float OwnerFemaleshare
        {
            get
            {
                return m_OwnerFemaleShare;
            }
            set
            {
                m_OwnerFemaleShare = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SHARE)]
        [DisplayName("Male owners share")]
        [Description("Male owners share in % of revenue")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(4)]
        public float OwnerMaleshare
        {
            get
            {
                return m_OwnerMaleShare;
            }
            set
            {
                m_OwnerMaleShare = value;
                SetChanged();
            }
        }

        #endregion

        #region  Input cost 

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Capital cost")]
        [Description("Capital cost per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(2)]
        public float CapitalInput
        {
            get
            {
                return m_CapitalCost;
            }
            set
            {
                m_CapitalCost = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Energy cost")]
        [Description("Energy cost per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(3)]
        public float EnergyCost
        {
            get
            {
                return m_EnergyCost;
            }
            set
            {
                m_EnergyCost = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Industrial cost")]
        [Description("Industrial cost per tonnes of product")]
        [DefaultValue(0)]
        [cPropertySorter.PropertyOrder(4)]
        public float IndustrialCost
        {
            get
            {
                return m_IndustrialCost;
            }
            set
            {
                m_IndustrialCost = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Services cost")]
        [Description("Services cost per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(5)]
        public float ServiceCost
        {
            get
            {
                return m_ServiceCost;
            }
            set
            {
                m_ServiceCost = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Management cost")]
        [Description("Management cost per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(6)]
        public float ManagementCost
        {
            get
            {
                return m_ManagementCost;
            }
            set
            {
                m_ManagementCost = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Royalty cost")]
        [Description("Royalty cost per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(7)]
        public float RoyaltyCost
        {
            get
            {
                return m_RoyaltyCost;
            }
            set
            {
                m_RoyaltyCost = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Certification cost")]
        [Description("Certification cost per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(8)]
        public float CertificationCost
        {
            get
            {
                return m_CertificationCost;
            }
            set
            {
                m_CertificationCost = value;
                SetChanged();
            }
        }

        #endregion

        #region  Taxes 

        [Browsable(true)]
        [Category(sPROPCAT_TAXES)]
        [DisplayName("Environmental tax")]
        [Description("Environmental tax per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(1)]
        public float TaxEnvironmental
        {
            get
            {
                return m_TaxesEnvironmental;
            }
            set
            {
                m_TaxesEnvironmental = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_TAXES)]
        [DisplayName("Export tax")]
        [Description("Export tax per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(2)]
        public float TaxExport
        {
            get
            {
                return m_TaxesExport;
            }
            set
            {
                m_TaxesExport = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_TAXES)]
        [DisplayName("Import tax")]
        [Description("Import tax per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(3)]
        public float TaxImport
        {
            get
            {
                return m_TaxesImport;
            }
            set
            {
                m_TaxesImport = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_TAXES)]
        [DisplayName("Production tax")]
        [Description("Production tax per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(4)]
        public float TaxProduction
        {
            get
            {
                return m_TaxesProduction;
            }
            set
            {
                m_TaxesProduction = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_TAXES)]
        [DisplayName("VAT tax")]
        [Description("VAT tax per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(6)]
        public float TaxVAT
        {
            get
            {
                return m_TaxesVAT;
            }
            set
            {
                m_TaxesVAT = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_TAXES)]
        [DisplayName("Profit tax (prop.)")]
        [Description("Tax as proportion of profit")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(6)]
        public float ProfitTax
        {
            get
            {
                return m_TaxesProfit;
            }
            set
            {
                m_TaxesProfit = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_TAXES)]
        [DisplayName("License tax")]
        [Description("License tax per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(7)]
        public float LicenseTax
        {
            get
            {
                return m_TaxesLicense;
            }
            set
            {
                m_TaxesLicense = value;
                SetChanged();
            }
        }

        #endregion

        #region  Social 

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("No. female workers")]
        [Description("Number of female workers per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(1)]
        public float WorkerFemale
        {
            get
            {
                return m_WorkerFemale;
            }
            set
            {
                m_WorkerFemale = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("No. male workers")]
        [Description("Number of male workers per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(2)]
        public float WorkerMale
        {
            get
            {
                return m_WorkerMale;
            }
            set
            {
                m_WorkerMale = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("No part-time workers")]
        [Description("Number of part-time workers per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(3)]
        public float WorkerParttime
        {
            get
            {
                return m_WorkerParttime;
            }
            set
            {
                m_WorkerParttime = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("No. other workers")]
        [Description("Number of other workers per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(4)]
        public float WorkerOther
        {
            get
            {
                return m_WorkerOther;
            }
            set
            {
                m_WorkerOther = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("No. female owners")]
        [Description("Number of female owners per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(10)]
        public float OwnerFemale
        {
            get
            {
                return m_OwnerFemale;
            }
            set
            {
                m_OwnerFemale = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("No. male owners")]
        [Description("Number of male owners per tonnes of product")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(11)]
        public float OwnerMale
        {
            get
            {
                return m_OwnerMale;
            }
            set
            {
                m_OwnerMale = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("Female worker dependents")]
        [Description("Number of dependents per female worker")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(20)]
        public float WorkerFemaleDependents
        {
            get
            {
                return m_WorkerFemaleDependents;
            }
            set
            {
                m_WorkerFemaleDependents = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("Male worker dependents")]
        [Description("Number of dependents per male worker")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(21)]
        public float WorkerMaleDependents
        {
            get
            {
                return m_WorkerMaleDependents;
            }
            set
            {
                m_WorkerMaleDependents = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("Female owner dependents")]
        [Description("Number of dependents per female owner")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(30)]
        public float OwnerFemaleDependents
        {
            get
            {
                return m_OwnerFemaleDependents;
            }
            set
            {
                m_OwnerFemaleDependents = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_SOCIAL)]
        [DisplayName("Male owner dependents")]
        [Description("Number of dependents per male owner")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(31)]
        public float OwnerMaleDependents
        {
            get
            {
                return m_OwnerMaleDependents;
            }
            set
            {
                m_OwnerMaleDependents = value;
                SetChanged();
            }
        }

        #endregion

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