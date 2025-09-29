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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace ValueChain
{

    #endregion


    /// <summary>
/// 
/// </summary>
    [TypeConverter(typeof(cPropertySorter))]
    [DefaultProperty("Name")]
    [Serializable()]
    public class cProducerUnit : cEconomicUnit
    {

        private class cLandingsInput
        {
            public void Clear()
            {
                Landings = 0f;
                Value = 0f;
            }
            public float Landings { get; set; }
            public float Value { get; set; }
        }

        public cProducerUnit() : base()
        {
        }

        #region  Private vars 

        private string m_fleet = "";
        private Dictionary<string, cLandingsInput> m_records = new Dictionary<string, cLandingsInput>();
        private float m_sEffort = 1f;

        private float m_sObserverCost = 0.0f;
        private float m_sObserverRate = 1.0f;
        private float m_sOriginalOutputBiomass = 0.0f;

        private float m_sTicketProducts = 0f;


        #endregion

        #region  Overrides 

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Initialize the unit for a new run.
    /// </summary>
    /// <param name="iSequence"></param>
    /// -----------------------------------------------------------------------
        internal override void InitRun(int iSequence)
        {
            base.InitRun(iSequence);
            // Reset local vars for the next run
            m_sOriginalOutputBiomass = 0.0f;
            m_records.Clear();
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Initialize the unit for a new time step.
    /// </summary>
    /// -----------------------------------------------------------------------
        internal override void Clear()
        {
            base.Clear();
            // Clear totals prior to a run!
            m_records.Clear();
        }

        public bool HasTarget(cUnit unit, string species)
        {

            // Follow each output link
            for (int iLink = 0, loopTo = LinkOutCount() - 1; iLink <= loopTo; iLink++)
            {
                var link = LinkOut(iLink);
                if (link is cLinkLandings)
                {
                    cLinkLandings linkSpec = (cLinkLandings)link;
                    if (ReferenceEquals(linkSpec.Target, unit) & string.Compare(linkSpec.Species, species, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                }
                // See the target link is the requesting unit
                else if (ReferenceEquals(link.Target, unit))
                    return true;
            }
            return false;

        }

        #endregion

        #region  Calculations 

        protected override bool Calculate(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            bool bSucces;

            // VC090310: Producer cost needs to reflect ecosim effort. 
            // We need to calculate the base cost from the standard calculations
            // below, but then change the effort-related cost based on Ecosim effort.

            // First time step?
            // VC090808: problem with this is that the user may have changed effort even in the first time step.
            // this will mess up calculations, but can't find an easy way to calculate ecopath baseline???????? 
            // 
            // VC:  because of the problem above, I force the effort to be 1 at timestep 1.

            // ' JS110325: Added sanity check
            // If (results.RunType = eRunTypes.Snapshot) Then
            // Debug.Assert(iTimeStep = 1, "Snapshot should use time step 1 only")
            // End If

            // JS250916: effort needs to be spoon-fed; cannot be automatically obtained anymore

            if (iTimeStep == 1)
            {
                // #Yes: store base biomass
                m_sOriginalOutputBiomass = sOutputBiomass;
                // Do not use effort this time step
                m_sEffort = 1f;
            }

            // The production unit needs to do the same calculations as the MyBase=cEconomicUnit, but:
            bSucces = base.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

            // Calc AddsObserver costs
            bSucces = bSucces & CalcObserverCost(results, sOutputBiomass, iTimeStep);

            // VC090310: Categories of costs and how they are handled:
            // Commercial fisheries
            // Related to tonnes: Pay/Share, all taxes, revenue (apart from subsidies), certification cost
            // Related to effort: Energy, Industrial, services, capital, observers, management, license, subsidies

            // Recreational fisheries
            // Effort: related to biomass of target species (sigmoid relationship)
            // Income: related to effort (for guide operations); 0 if private boats
            // Cost: modeled same way as for commercial fisheries

            // Eco tours
            // Effort: related to biomass of target species (sigmoid relationship)
            // Income: related to effort; using ticket revenue: m_sTicketProducts 
            // Cost: modeled same way as for commercial fisheries 

            return bSucces;

        }

        protected override bool CalcProducts(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            // Now add to this the revenue from paying customers
            float sSum = m_sEffort * m_sTicketProducts;
            results.Store(this, cValueChainResults.eVariableType.RevenueTickets, sSum, iTimeStep);

            // Use standard calculations, which is desirable so we do not have to keep 
            // updating formulas in different places in case standard calculations were 
            // to change       '
            // Last part is the usual biomass related part:
            // Dim sSum As Single = sOutputBiomass * (Me.EnergyProducts + Me.IndustrialProducts + Me.ServiceProducts)
            return base.CalcProducts(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);

        }

        protected override bool CalcRawmaterialCost(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            return results.Store(this, cValueChainResults.eVariableType.CostRawmaterial, 0f, iTimeStep);

        }

        protected override bool CalcInputCost(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            // Need to include effort in our calculations
            if (m_sEffort != 1f)
            {
                // #Yes: do NOT use sOutputBiomass, but instead use base biomass x effort
                float sSum = m_sOriginalOutputBiomass * m_sEffort * (CapitalInput + EnergyCost + IndustrialCost + ServiceCost);
                return results.Store(this, cValueChainResults.eVariableType.CostInput, sSum, iTimeStep);
            }
            else
            {
                return base.CalcInputCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            }

        }

        protected override bool CalcManagementRoyaltyCertificationCost(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            // the costs for management and royalties are proportional to effort
            if (m_sEffort != 1f)
            {
                float sSum = m_sEffort * m_sOriginalOutputBiomass * (ManagementCost + RoyaltyCost);

                // the cost for certification is assumed proportional to landings, so add this
                sSum += sOutputBiomass * CertificationCost;

                return results.Store(this, cValueChainResults.eVariableType.CostManagementRoyaltyCertification, sSum, iTimeStep);
            }
            else  // just like other calculations:
            {
                return base.CalcManagementRoyaltyCertificationCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            }

        }

        protected override bool CalcSubsidy(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            if (m_sEffort != 1f)
            {
                float sSum = m_sEffort * m_sOriginalOutputBiomass * (SubsidyEnergy + SubsidyOther);
                results.Store(this, cValueChainResults.eVariableType.RevenueSubsidies, sSum, iTimeStep);
                return true;
            }
            else
            {
                return base.CalcSubsidy(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            }
        }

        protected virtual bool CalcObserverCost(cValueChainResults results, float sOutputBiomass, int iTimeStep)
        {

            float sObsCost = 0f;
            if (m_sEffort != 1f)
            {
                sObsCost = m_sOriginalOutputBiomass * m_sEffort * (ObserverCost * ObserverRate);
            }
            else
            {
                sObsCost = sOutputBiomass * (ObserverCost * ObserverRate);
            }
            return results.Store(this, cValueChainResults.eVariableType.CostObserver, sObsCost, iTimeStep);

        }

        /// <summary>
    /// The number of jobs for producers is a function of effort, while their salary isn't
    /// </summary>
    /// <param name="results"></param>
    /// <param name="sInputBiomass"></param>
    /// <param name="sInputValue"></param>
    /// <param name="sOutputBiomass"></param>
    /// <param name="sOutputValue"></param>
    /// <param name="iTimeStep"></param>
    /// <returns></returns>
    /// <remarks></remarks>
        protected override bool CalcWorkerFemales(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {
            if (m_sEffort != 1f)
            {
                float sSum = m_sEffort * m_sOriginalOutputBiomass * WorkerFemale;
                return results.Store(this, cValueChainResults.eVariableType.NumberOfWorkerFemales, sSum, iTimeStep);
            }
            else
            {
                return base.CalcWorkerFemales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            }

        }

        protected override bool CalcWorkerMales(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            if (m_sEffort != 1f)
            {
                float sSum = m_sEffort * m_sOriginalOutputBiomass * WorkerMale;
                return results.Store(this, cValueChainResults.eVariableType.NumberOfWorkerMales, sSum, iTimeStep);
            }
            else
            {
                return base.CalcWorkerMales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            }

        }

        protected override bool CalcOwnerMales(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            if (m_sEffort != 1f)
            {
                float sSum = m_sEffort * m_sOriginalOutputBiomass * OwnerMale;
                return results.Store(this, cValueChainResults.eVariableType.NumberOfOwnerMales, sSum, iTimeStep);
            }
            else
            {
                return base.CalcOwnerMales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            }

        }

        protected override bool CalcOwnerFemales(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            if (m_sEffort != 1f)
            {
                float sSum = m_sEffort * m_sOriginalOutputBiomass * OwnerFemale;
                return results.Store(this, cValueChainResults.eVariableType.NumberOfOwnerFemales, sSum, iTimeStep);
            }
            else
            {
                return base.CalcOwnerFemales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep);
            }

        }

        #endregion

        #region  Overrides 

        [Browsable(false)]
        public override bool HasError
        {
            get
            {
                return m_fleet is null | !string.IsNullOrWhiteSpace(UnlikelyOutputs);
            }
        }

        // <Browsable(False)>
        // Public Overrides ReadOnly Property Style() As cStyleGuide.eStyleFlags
        // Get
        // Dim st As cStyleGuide.eStyleFlags = MyBase.Style
        // If (Me.m_fleet IsNot Nothing) Then st = st Or cStyleGuide.eStyleFlags.ValueComputed
        // If (Me.HasError) Then st = st Or cStyleGuide.eStyleFlags.ErrorEncountered
        // Return st
        // End Get
        // End Property

        #endregion

        #region  Alternate name 

        private string GenerateName()
        {
            if (string.IsNullOrWhiteSpace(m_fleet))
                return "! No fleet";
            return m_fleet;
        }

        public override string Name
        {
            get
            {
                string strName = base.Name;
                if (string.IsNullOrEmpty(strName))
                {
                    strName = GenerateName();
                }
                return strName;
            }
            set
            {
                // Setting generated name?
                if (string.Compare(value, GenerateName()) == 0)
                {
                    // #Yes: Clear the base name
                    base.Name = "";
                }
                else
                {
                    // #No: Set the base name
                    base.Name = value;
                }
            }
        }

        #endregion

        #region  Properties 

        public override string BiomassRatio
        {
            get
            {
                // Count # of active links
                int iNumActiveLinks = 0;
                for (int i = 0, loopTo = LinkOutCount() - 1; i <= loopTo; i++)
                {
                    if (LinkOut(i).BiomassRatio > 0f)
                    {
                        iNumActiveLinks += 1;
                    }
                }
                return base.BiomassRatio + " / " + iNumActiveLinks.ToString();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_VALIDATION)]
        [DisplayName("Unlikely outputs")]
        [Description("Names of groups that are landed and transferred through the chain with an unlikely biomass ratios that exceed 1")]
        [cPropertySorter.PropertyOrder(7)]
        public string UnlikelyOutputs
        {
            get
            {

                var totals = new Dictionary<string, float>();
                var sbError = new StringBuilder();

                for (int i = 0, loopTo = LinkOutCount() - 1; i <= loopTo; i++)
                {
                    cLinkLandings ll = (cLinkLandings)LinkOut(i);
                    if (!string.IsNullOrWhiteSpace(ll.Species))
                    {
                        float stotal = 0f;
                        totals.TryGetValue(ll.Species, out stotal);
                        totals[ll.Species] = stotal + ll.BiomassRatio;
                    }
                }

                foreach (var spp in totals.Keys)
                {
                    if (totals[spp] > 1.0f)
                    {
                        if (sbError.Length > 0)
                        {
                            sbError.Append(",");
                        }
                        sbError.Append(spp + ": " + totals[spp].ToString("R"));
                    }
                }
                return sbError.ToString();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Monitoring cost")]
        [Description("Cost for monitors (if on board) per tonnes. Assumed to vary with effort")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(20)]
        public float ObserverCost
        {
            get
            {
                return m_sObserverCost;
            }
            set
            {
                m_sObserverCost = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_INPUTCOST)]
        [DisplayName("Monitor coverage rate")]
        [Description("Monitor coverage rate, (proportion of boats with observers onboard)")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(21)]
        public float ObserverRate
        {
            get
            {
                return m_sObserverRate;
            }
            set
            {
                m_sObserverRate = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_REVENUE)]
        [DisplayName("Ticket revenue")]
        [Description("Revenue from paying customers at Ecopath baseline effort (unity effort). Revenue assumed proportional to effort.")]
        [DefaultValue(0.0f)]
        [cPropertySorter.PropertyOrder(1)]
        public float TicketProducts
        {
            get
            {
                return m_sTicketProducts;
            }
            set
            {
                m_sTicketProducts = value;
                SetChanged();
            }
        }

        public override string Category
        {
            get
            {
                return "Producer";
            }
        }

        [Browsable(false)]
        public override cUnitFactory.eUnitType UnitType
        {
            get
            {
                return cUnitFactory.eUnitType.Producer;
            }
        }

        [Browsable(false)]
        public override bool CanCompute
        {
            get
            {
                return true;
            }
        }

        #region  Ecopath integration 

        [Browsable(false)]
        public virtual string Fleet
        {
            get
            {
                return m_fleet;
            }
            set
            {
                m_fleet = value;
            }
        }

        #endregion

        #endregion

        #region  Landings 

        public void SetEffort(float sEffort)
        {
            m_sEffort = sEffort;
        }

        /// <summary>
    /// 
    /// </summary>
    /// <param name="species"></param>
    /// <param name="sBiomass">Total biomass landed in area</param>
    /// <param name="sValue">Total value landed in area</param>
        public void SetLandings(string species, float sBiomass, float sValue)
        {

            if (string.IsNullOrWhiteSpace(species))
                return;

            cLandingsInput @record = null;
            if (!m_records.TryGetValue(species, out @record))
                @record = new cLandingsInput();
            @record.Landings = sBiomass;
            @record.Value = sValue;
            m_records[species] = @record;

        }

        public void Process(cValueChainResults results, int iTimeStep, int iItem)
        {

            float sTotalOutputBiomass = 0f;
            float sTotalOutputValue = 0f;

            float sBTot = 0f;
            float sValTot = 0f;
            foreach (cLandingsInput r in m_records.Values)
            {
                sBTot += r.Landings;
                sValTot += r.Value;
            }

            // No item specified?
            if (iItem == 0)
            {
                // #Yes: perform all calculations
                Calculate(results, sBTot, 0f, sBTot, sValTot, iTimeStep);
            }

            // Determine outgoing biomass ratios for each species
            var totalSppB = new Dictionary<string, float>();
            foreach (cLink link in m_llinkOutput)
            {
                // Sanity check
                if (link is cLinkLandings)
                {
                    cLinkLandings ll = (cLinkLandings)link;
                    if (!string.IsNullOrWhiteSpace(ll.Species) & ll.IsVisible())
                    {
                        float s = 0f;
                        totalSppB.TryGetValue(ll.Species, out s);
                        s += ll.BiomassRatio;
                        totalSppB[ll.Species] = s;
                    }
                }
            }

            // Determine outgoing biomass
            foreach (cLink link in m_llinkOutput)
            {

                float sBiomass = 0.0f;
                float sValue = 0.0f;
                // the above was called sPrice, but it is value, so renamed

                Debug.Assert(link is cLinkLandings);

                cLinkLandings ll = (cLinkLandings)link;
                if (!string.IsNullOrWhiteSpace(ll.Species) & ll.IsVisible())
                {
                    float s = 0f;
                    totalSppB.TryGetValue(ll.Species, out s);
                    if (s > 0f)
                    {
                        var r = m_records[ll.Species];
                        sBiomass += r.Landings * ll.BiomassRatio / totalSppB[ll.Species];

                        if (ll.ValueRatio == 1.0f)
                        {
                            sValue += r.Value * ll.BiomassRatio / totalSppB[ll.Species];
                        }
                        else
                        {
                            sValue += ll.ValueRatio * r.Landings * ll.BiomassRatio / totalSppB[ll.Species];
                        }

                    }
                }

                // Process every link to ensure that target units receive all inputs!
                if (sBiomass > 0f)
                {
                    // VC: I changed the process line to pass sPrice/sBiomass as the third parameter (instead of sPrice). 
                    // it is supposed to be the price per unit biomass
                    // it was multiplying an extra time with the total catches (sBiomass) as it was.
                    link.Target.Process(results, new cInput(this, sBiomass, sValue), iTimeStep, iItem);
                }
                else
                {
                    // Process link to make the chain work, even though no data travels over this link!
                    link.Target.Process(results, new cInput(this, sBiomass, sValue), iTimeStep, iItem);
                }

                sTotalOutputBiomass += sBiomass;
                sTotalOutputValue += sValue; // * sBiomass

            }

            results.StoreContribution(iItem, this, iTimeStep, sValTot, sBTot);

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