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
using System.Diagnostics;

namespace ValueChain
{

    #endregion


    /// ===========================================================================
/// <summary>
/// Value Chain results holder.
/// </summary>
/// ===========================================================================
    public class cValueChainResults
    {

        #region  Private helper class 

        /// =======================================================================
    /// <summary>
    /// Results for a single time step.
    /// </summary>
    /// =======================================================================
        private class cTimeStepResults
        {

            /// <summary>Ecost data that these results relate to.</summary>
            private cValueChainData m_data = null;
            /// <summary>Redundant: time step index</summary>
            private int m_iTimeStep = 0;
            /// <summary>Results(# variable types, # units)</summary>
            private float[,] m_results;

            public cTimeStepResults(cValueChainData data, int iTimeStep)
            {
                m_data = data;
                m_iTimeStep = iTimeStep;
                m_results = new float[Enum.GetNames(typeof(eVariableType)).Length + 1, m_data.UnitCount() + 1];
            }

            public float get_Results(int iVar, int iUnit)
            {
                return m_results[iVar, iUnit];
            }
            public void set_Results(int iVar, int iUnit, float value)
            {
                m_results[iVar, iUnit] = value;
            }

            public cTimeStepResults Clone()
            {
                var tsr = new cTimeStepResults(m_data, m_iTimeStep);
                for (int i = 0, loopTo = m_results.GetUpperBound(0); i <= loopTo; i++)
                {
                    for (int j = 0, loopTo1 = m_results.GetUpperBound(1); j <= loopTo1; j++)
                        tsr.set_Results(i, j, m_results[i, j]);
                }
                return tsr;
            }

            /// -------------------------------------------------------------------
        /// <summary>
        /// Helper method, calculates derived values for a timestep result.
        /// Derived variables are totals and sub-totals of result categories.
        /// </summary>
        /// -------------------------------------------------------------------
            protected internal void CalculateDerivedValues()
            {

                cUnit unit = null;

                // Note that although units provide different types of variables, all 
                // variable categories can still be bluntly Totaled. Variable values 
                // that are not used are 0 by default.

                // Calc derived vars for each unit
                for (int iUnit = 0, loopTo = m_data.UnitCount() - 1; iUnit <= loopTo; iUnit++)
                {

                    unit = m_data.Unit(iUnit);

                    // Revenue total
                    float sRevenue = 0.0f;
                    // Revenue breakdown
                    float sRevenueProductsOther = 0.0f;
                    float sRevenueTickets = 0f;

                    // Cost total
                    float sCost = 0.0f;
                    float sProfit = 0.0f;
                    float sTotalUtility = 0.0f;
                    // Cost breakdown
                    float sCostSalariesShares = 0.0f;
                    float sCostManagementRoyaltyCertificationObserver = 0.0f;
                    float sCostlInputOther = 0.0f;

                    // Jobs
                    float sTotalJobs = 0.0f;
                    // Jobs breakdown
                    float sTotalJobsMale = 0.0f;
                    float sTotalJobsFemale = 0.0f;

                    // Dependents total
                    float sDependentsTotal = 0.0f;
                    float sGDP = 0.0f;

                    sRevenueProductsOther = m_results[(int)eVariableType.RevenueProductsOther, unit.Sequence] + m_results[(int)eVariableType.RevenueAgriculture, unit.Sequence];

                    sRevenueTickets = m_results[(int)eVariableType.RevenueTickets, unit.Sequence];

                    sRevenue = sRevenueProductsOther + sRevenueTickets + m_results[(int)eVariableType.RevenueSubsidies, unit.Sequence];

                    // If isBroker = False Then  'this is not a broker, so the revenus from selling the product is theirs, and it counts in the utility
                    sRevenue += m_results[(int)eVariableType.RevenueProductsMain, unit.Sequence];

                    // Cost
                    sCostSalariesShares = m_results[(int)eVariableType.CostWorker, unit.Sequence] + m_results[(int)eVariableType.CostOwner, unit.Sequence];

                    sCostManagementRoyaltyCertificationObserver = m_results[(int)eVariableType.CostManagementRoyaltyCertification, unit.Sequence] + m_results[(int)eVariableType.CostObserver, unit.Sequence];

                    sCostlInputOther = m_results[(int)eVariableType.CostAgriculture, unit.Sequence] + m_results[(int)eVariableType.CostInput, unit.Sequence];

                    sCost = sCostSalariesShares + sCostlInputOther + m_results[(int)eVariableType.CostTaxes, unit.Sequence] + sCostManagementRoyaltyCertificationObserver;



                    sCost += m_results[(int)eVariableType.CostRawmaterial, unit.Sequence];

                    // Profit
                    float grossProfit = sRevenue - sCost;
                    // tax on profit:
                    if (grossProfit > 0f & unit is cEconomicUnit)
                    {
                        float TaxOnProfit = ((cEconomicUnit)unit).ProfitTax * grossProfit;
                        m_results[(int)eVariableType.CostTaxes, unit.Sequence] += TaxOnProfit;
                        sCost += TaxOnProfit;
                    }

                    sProfit = sRevenue - sCost;


                    // TotalUtility a.k.a. Throughput = cost when (profit < 0), revenue otherwise
                    sTotalUtility = sProfit < 0f ? sCost : sRevenue;

                    // Jobs
                    sTotalJobsMale = m_results[(int)eVariableType.NumberOfWorkerMales, unit.Sequence] + m_results[(int)eVariableType.NumberOfOwnerMales, unit.Sequence];
                    sTotalJobsFemale = m_results[(int)eVariableType.NumberOfWorkerFemales, unit.Sequence] + m_results[(int)eVariableType.NumberOfOwnerFemales, unit.Sequence];
                    sTotalJobs = sTotalJobsFemale + sTotalJobsMale;

                    // Dependents, total
                    sDependentsTotal = m_results[(int)eVariableType.NumberOfOwnerDependents, unit.Sequence] + m_results[(int)eVariableType.NumberOfWorkerDependents, unit.Sequence];

                    // Store
                    m_results[(int)eVariableType.RevenueProductsOther, unit.Sequence] = sRevenueProductsOther;
                    m_results[(int)eVariableType.RevenueTotal, unit.Sequence] = sRevenue;

                    m_results[(int)eVariableType.CostTotalInputOther, unit.Sequence] = sCostlInputOther;
                    m_results[(int)eVariableType.CostSalariesShares, unit.Sequence] = sCostSalariesShares;
                    m_results[(int)eVariableType.CostManagementRoyaltyCertificationObservers, unit.Sequence] = sCostManagementRoyaltyCertificationObserver;
                    m_results[(int)eVariableType.Cost, unit.Sequence] = sCost;
                    m_results[(int)eVariableType.Profit, unit.Sequence] = sProfit;
                    m_results[(int)eVariableType.TotalUtility, unit.Sequence] = sTotalUtility;

                    m_results[(int)eVariableType.NumberOfJobsFemaleTotal, unit.Sequence] = sTotalJobsFemale;
                    m_results[(int)eVariableType.NumberOfJobsMaleTotal, unit.Sequence] = sTotalJobsMale;
                    m_results[(int)eVariableType.NumberOfJobsTotal, unit.Sequence] = sTotalJobs;

                    m_results[(int)eVariableType.NumberOfDependentsTotal, unit.Sequence] = sDependentsTotal;

                    sGDP = m_results[(int)eVariableType.CostSalariesShares, unit.Sequence] + m_results[(int)eVariableType.CostTaxes, unit.Sequence] + m_results[(int)eVariableType.CostManagementRoyaltyCertificationObservers, unit.Sequence] + m_results[(int)eVariableType.Profit, unit.Sequence] - m_results[(int)eVariableType.RevenueSubsidies, unit.Sequence];


                    m_results[(int)eVariableType.GDPContribution, unit.Sequence] = sGDP;

                }

            }

        }

        #endregion

        #region  Private vars 

        /// <summary>The data to aggregate results for.</summary>
        private cValueChainData m_data = null;
        /// <summary>Dictionary[timestep, result] of results per time step.</summary>
        private Dictionary<int, cTimeStepResults> m_dtResultTimeStep = new Dictionary<int, cTimeStepResults>();
        /// <summary>Dictionary[key, result] of results for an equilbrium run.</summary>
        private Dictionary<object, cTimeStepResults> m_dtSnapshots = new Dictionary<object, cTimeStepResults>();

        /// <summary>Contributions of an item (fleet, group, ..) to a unit per timestep to the total value.</summary>
    /// <remarks>Indexed as (item, time step, unit sequence).</remarks>
        private float[,,] m_ValueContribution;
        /// <summary>Contributions of an item (fleet, group, ..) to a unit per timestep to the total biomass.</summary>
    /// <remarks>Indexed as (item, time step, unit sequence).</remarks>
        private float[,,] m_BiomassContribution;

        /// <summary>Max no of time steps.</summary>
        private int m_iMaxTimeStep = 0;
        /// <summary>Max no of items values are aggregated over.</summary>
        private int m_iMaxItem = 0;

        private int m_nTimeSteps = 1;

        /// <summary>The biomass flows from one unit to another (source x target)</summary>
        public double[,] m_BiomassFlows;

        #endregion

        #region  Public enums 

        /// <summary>
    /// Types of calculated results.
    /// </summary>
        public enum eVariableType : int
        {

            /// <summary> Production of fish products in tonnes </summary>
            Production,
            /// <summary> Production of fish products in corresponding live weight </summary>
            ProductionLive,

            CostRawmaterial,
            CostInput,
            CostAgriculture,
            CostManagementRoyaltyCertification,
            CostTaxes,
            CostOwner,
            CostWorker,

            /// <summary>Cost of observers</summary>
        /// <remarks>over tonnes</remarks>
            CostObserver,
            Cost,
            CostManagementRoyaltyCertificationObservers,
            CostSalariesShares,
            CostTotalInputOther,

            Profit,

            /// <summary> The value of the fish products  </summary>
            RevenueProductsMain,
            /// <summary> Revenue from Agricultural products, should they be making any such as a byproduct </summary>
            RevenueAgriculture,
            /// <summary> Revenue from ticket sale, which will be a function of effort </summary>
            RevenueTickets,
            /// <summary> The value of other products than the actual fish </summary>
        /// <remarks>over tonnes</remarks>
            RevenueProductsOther,
            /// <remarks>over tonnes</remarks>
            RevenueSubsidies,
            RevenueTotal,

            TotalUtility,

            NumberOfWorkerFemales,
            NumberOfWorkerMales,
            NumberOfWorkerPartTime,
            NumberOfWorkerOther,

            NumberOfOwnerFemales,
            NumberOfOwnerMales,
            NumberOfJobsTotal,

            NumberOfWorkerDependents,
            NumberOfOwnerDependents,
            NumberOfDependentsTotal,

            OutputBiomass,
            OutputBiomassLW,

            NumberOfJobsMaleTotal,
            NumberOfJobsFemaleTotal,

            // VC090401: added the factors below to calc by type of units:
            CostProducers,
            CostProcessors,
            CostDistributors,
            CostMarket,
            CostConsumer,
            RevenueProducers,
            RevenueProcessors,
            RevenueDistributors,
            RevenueMarket,
            // No revenue for consumers
            ProfitProducers,
            ProfitProcessors,
            ProfitDistributors,
            ProfitMarket,
            // No profit for consumers

            Landings,
            LandingsPrice,

            GDPContribution

        }

        public enum eGraphDataType : int
        {
            CostRevenue = 0,
            Cost,
            Revenue,
            Jobs,
            Dependents
        }

        #endregion

        #region  Construction 

        public cValueChainResults(cValueChainData data)
        {
            m_data = data;
        }

        #endregion

        #region  Public access 

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Return the collection of <see cref="eVariableType">variables</see> to
    /// populate a given <see cref="eGraphDataType">graph</see>.
    /// </summary>
    /// <param name="graph">The graph type to obtain variables for.</param>
    /// <returns>The collection of <see cref="eVariableType">variables</see> to
    /// populate a given <see cref="eGraphDataType">graph</see>.</returns>
    /// -----------------------------------------------------------------------
        public static eVariableType[] GetVariables(eGraphDataType graph)
        {

            eVariableType[] vars = null;

            switch (graph)
            {

                case eGraphDataType.CostRevenue:
                    {
                        vars = new eVariableType[] { eVariableType.RevenueTotal, eVariableType.Cost, eVariableType.Profit };
                        break;
                    }

                case eGraphDataType.Cost:
                    {
                        vars = new eVariableType[] { eVariableType.CostAgriculture, eVariableType.CostInput, eVariableType.CostManagementRoyaltyCertification, eVariableType.CostManagementRoyaltyCertificationObservers, eVariableType.CostRawmaterial };
                        break;
                    }

                case eGraphDataType.Revenue:
                    {
                        vars = new eVariableType[] { eVariableType.RevenueTickets, eVariableType.RevenueSubsidies, eVariableType.RevenueProductsMain, eVariableType.RevenueProductsOther, eVariableType.RevenueAgriculture };
                        break;
                    }

                case eGraphDataType.Jobs:
                    {
                        vars = new eVariableType[] { eVariableType.NumberOfJobsTotal, eVariableType.NumberOfJobsMaleTotal, eVariableType.NumberOfJobsFemaleTotal };
                        break;
                    }
                case eGraphDataType.Dependents:
                    {
                        vars = new eVariableType[] { eVariableType.NumberOfDependentsTotal, eVariableType.NumberOfWorkerDependents, eVariableType.NumberOfWorkerFemales, eVariableType.NumberOfWorkerMales, eVariableType.NumberOfOwnerMales, eVariableType.NumberOfOwnerFemales, eVariableType.NumberOfOwnerDependents };
                        break;
                    }

                default:
                    {
                        Debug.Assert(false);
                        break;
                    }

            }
            return vars;

        }

        public static eContributionType GetVariableContributionType(eVariableType @var)
        {

            switch (@var)
            {
                case eVariableType.NumberOfDependentsTotal:
                case eVariableType.NumberOfJobsFemaleTotal:
                case eVariableType.NumberOfJobsMaleTotal:
                case eVariableType.NumberOfJobsTotal:
                case eVariableType.NumberOfOwnerDependents:
                case eVariableType.NumberOfOwnerFemales:
                case eVariableType.NumberOfOwnerMales:
                case eVariableType.NumberOfWorkerDependents:
                case eVariableType.NumberOfWorkerFemales:
                case eVariableType.NumberOfWorkerMales:
                case eVariableType.NumberOfWorkerOther:
                case eVariableType.NumberOfWorkerPartTime:
                    {
                        return eContributionType.Biomass;
                    }
                case eVariableType.Production:
                case eVariableType.ProductionLive:
                    {
                        return eContributionType.Biomass;
                    }
            }

            return eContributionType.Value;

        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Reset results by destroying all cached computated data in preparation
    /// for a new search.
    /// </summary>
    /// <remarks>Call this method before starting a new search.</remarks>
    /// -----------------------------------------------------------------------
        public void Reset(int nFleets, int nGroups, int nTimesteps)
        {

            int nNumUnits = m_data.GetUnits(cUnitFactory.eUnitType.All).Length;
            int nItems = Math.Max(nFleets, nGroups);

            m_dtResultTimeStep.Clear();
            m_dtSnapshots.Clear();
            m_iMaxTimeStep = 0;
            m_iMaxItem = 0;
            m_nTimeSteps = nTimesteps;

            m_ValueContribution = new float[nItems + 1, nNumUnits + 1, Math.Max(1, nTimesteps) + 1];
            m_BiomassContribution = new float[nItems + 1, nNumUnits + 1, Math.Max(1, nTimesteps) + 1];
            m_BiomassFlows = new double[nNumUnits + 1, nNumUnits + 1];

        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Store a value of a particular variable type for a particular unit
    /// </summary>
    /// <param name="unit">Unit to save variable for</param>
    /// <param name="var">Type of the variable to save</param>
    /// <param name="sValue">Value to save</param>
    /// <returns>True if successful.</returns>
    /// -----------------------------------------------------------------------
        public bool Store(cUnit unit, eVariableType @var, float sValue, int iTimeStep)
        {

            try
            {
                m_iMaxTimeStep = Math.Max(m_iMaxTimeStep, iTimeStep);
                var rs = GetTimeStepResult(iTimeStep, true);
                rs.set_Results((int)@var, unit.Sequence, sValue);
            }

            catch (Exception ex)
            {
                return false;
            }
            return true;

        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Make a snapshot of a given time step, and store it under a given key.
    /// </summary>
    /// <param name="objKey">The key to store the snapshot for.</param>
    /// <param name="iTimeStep">The time step to store a snapshot for.</param>
    /// <returns>True if successful.</returns>
    /// -----------------------------------------------------------------------
        public bool StoreSnapshot(object objKey, int iTimeStep)
        {

            // Why on earth are we cloning here?!?! 
            var tsr = GetTimeStepResult(iTimeStep, true).Clone();
            m_dtSnapshots[objKey] = tsr;
            return true;

        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Returns list of all snapshot keys.
    /// </summary>
    /// <returns></returns>
    /// -----------------------------------------------------------------------
        public object[] Snapshots
        {
            get
            {
                var lsnapshotKeys = new List<object>();
                foreach (object key in m_dtSnapshots.Keys)
                    lsnapshotKeys.Add(key);
                lsnapshotKeys.Sort();
                return lsnapshotKeys.ToArray();
            }
        }

        public enum eContributionType
        {
            Value,
            Biomass
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get result for a given unit and variable at a given time step, optionally
    /// filtered by item (fleet, group, ..).
    /// </summary>
    /// <param name="var"></param>
    /// <param name="iTimeStep"></param>
    /// <param name="unit"></param>
    /// <param name="iItem"></param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------
        public float Result(cUnit unit, eVariableType @var, int iTimeStep, int iItem, eContributionType contr)
        {

            var rs = GetTimeStepResult(iTimeStep, false);
            if (rs is not null)
            {

                float sValue = rs.get_Results((int)@var, unit.Sequence);
                float sContrVal = 0f;
                float sContrBio = 0f;

                GetContributionRatios(iItem, unit, iTimeStep, ref sContrVal, ref sContrBio);

                switch (contr)
                {
                    case eContributionType.Value:
                        {
                            return sValue * sContrVal;
                        }
                    case eContributionType.Biomass:
                        {
                            return sValue * sContrBio;
                        }
                }
            }

            return 0f;

        }

        public void CalculateDerivedValues(int iTimeStep)
        {
            var rs = GetTimeStepResult(iTimeStep, false);
            if (rs is not null)
            {
                rs.CalculateDerivedValues();
            }
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get result for a given unit and variable at a given snapshot.
    /// </summary>
    /// <param name="var"></param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------
        public float SnapshotValue(cUnit unit, eVariableType @var, object objKey)
        {
            var tsr = GetSnapshot(objKey);
            if (tsr is not null)
                return tsr.get_Results((int)@var, unit.Sequence);
            return 0.0f;
        }

        public static eVariableType[] GetVariables()
        {
            return (eVariableType[])Enum.GetValues(typeof(eVariableType));
        }

        public int NumTimeSteps()
        {
            return m_iMaxTimeStep;
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get/set the amount of a flow between a source and target unit. Dimensioned
    /// as (source x target).
    /// </summary>
    /// <param name="iSource"><see cref="cUnit.Sequence"/> of source unit (donor).</param>
    /// <param name="iTarget"><see cref="cUnit.Sequence"/> of target unit (recipient).</param>
    /// -----------------------------------------------------------------------
        public double get_FlowsBiomass(int iSource, int iTarget)
        {
            return m_BiomassFlows[iSource, iTarget];
        }
        public void set_FlowsBiomass(int iSource, int iTarget, double value)
        {
            m_BiomassFlows[iSource, iTarget] = value;
        }

        #endregion

        #region  Totals 

        public float GetSnapshotTotal(eVariableType vartype, object objKey, cUnit[] lUnits = null)
        {
            float sTotal = 0.0f;

            if (lUnits is null)
            {
                foreach (cUnit unit in m_data.GetUnits(cUnitFactory.eUnitType.All))
                    sTotal += SnapshotValue(unit, vartype, objKey);
            }
            else
            {
                foreach (cUnit unit in lUnits)
                    sTotal += SnapshotValue(unit, vartype, objKey);
            }
            return sTotal;

        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get the total sum of a given variabe for a single time step.
    /// </summary>
    /// <param name="vartype"></param>
    /// <param name="iTimeStep"></param>
    /// <param name="lUnits"></param>
    /// <param name="iItem">Aggreagation item index, if any.</param>
    /// <param name="contr"><see cref="eContributionType"/> to extract contribution for.</param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------
        public float GetTimeStepTotal(eVariableType vartype, int iTimeStep, cUnit[] lUnits, int iItem, eContributionType contr)
        {

            float sTotal = 0.0f;

            if (lUnits is null)
            {
                lUnits = m_data.GetUnits(cUnitFactory.eUnitType.All);
            }

            foreach (cUnit unit in lUnits)
                sTotal += Result(unit, vartype, iTimeStep, iItem, contr);

            return sTotal;

        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get the total sum of a given variabe across all time steps.
    /// </summary>
    /// <param name="vartype">Variable to extract.</param>
    /// <param name="lUnits">Units to extract total for.</param>
    /// <param name="contr"><see cref="eContributionType">Contribution type</see>.</param>
    /// <param name="iItem">Item to filter by.</param>
    /// <returns>A total value.</returns>
    /// -----------------------------------------------------------------------
        public float GetTotal(eVariableType vartype, cUnit[] lUnits = null, int iItem = 0, eContributionType contr = eContributionType.Value)
        {

            float sTotal = 0.0f;

            if (lUnits is null)
            {
                lUnits = m_data.GetUnits(cUnitFactory.eUnitType.All);
            }

            for (int iTimestep = 1, loopTo = m_iMaxTimeStep; iTimestep <= loopTo; iTimestep++)
            {
                foreach (cUnit unit in lUnits)
                    sTotal += Result(unit, vartype, iTimestep, iItem, contr);
            }

            return sTotal;

        }

        #endregion

        #region  Internals 

        private cTimeStepResults GetTimeStepResult(int iTimeStep, bool bCreateIfMissing)
        {

            cTimeStepResults tsr = null;
            if (!m_dtResultTimeStep.ContainsKey(iTimeStep))
            {
                if (bCreateIfMissing)
                {
                    tsr = new cTimeStepResults(m_data, iTimeStep);
                    m_dtResultTimeStep.Add(iTimeStep, tsr);
                }
            }
            else
            {
                tsr = m_dtResultTimeStep[iTimeStep];
            }

            return tsr;

        }

        private cTimeStepResults GetSnapshot(object objKey)
        {

            cTimeStepResults tsr = null;
            if (m_dtSnapshots.ContainsKey(objKey))
                return m_dtSnapshots[objKey];
            return null;

        }

        #endregion

        #region  Contribution by item 

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Store the contribution of a single item to a unit at a given time step.
    /// </summary>
    /// <param name="iItem">The item to store the contribution for.</param>
    /// <param name="unit">The unit to store the contribution for.</param>
    /// <param name="iTimeStep">The time step to store the contribution for.</param>
    /// <param name="sValueContribution">The value contribution to store.</param>
    /// <param name="sBiomassContribution">The biomass contribution to store.</param>
    /// <remarks>
    /// The sum of contributions of all items should equal (or very,
    /// very closely approximate) the value for the unit for the default chain.
    /// </remarks>
    /// -----------------------------------------------------------------------
        public void StoreContribution(int iItem, cUnit unit, int iTimeStep, float sValueContribution, float sBiomassContribution)
        {

            bool bOkidoki = false;


            if (bOkidoki)
            {
                try
                {
                    // Append contribution in case this is called multiple times for a single ([fleet|group], unit combo)
                    m_ValueContribution[iItem, unit.Sequence, iTimeStep] += sValueContribution;
                    m_BiomassContribution[iItem, unit.Sequence, iTimeStep] += sBiomassContribution;
                }
                catch (Exception ex)
                {
                    // Whoah!
                }
            }

            m_iMaxItem = Math.Max(m_iMaxItem, iItem);
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get the value ratio that a single item contributed for a given unit and 
    /// time step, relative to the total value contribution for all items.
    /// </summary>
    /// <param name="iItem">Item to explore, 0 for all items.</param>
    /// <param name="unit"></param>
    /// <param name="iTimestep"></param>
    /// -----------------------------------------------------------------------
        public void GetContributionRatios(int iItem, cUnit unit, int iTimestep, ref float sValueContribution, ref float sBiomassContribution)
        {

            float sAllItemsValue = 0f; // Value contribution for 'all fleets' calculation
            float sAllItemsBiomass = 0f; // Biomass contribution for 'all fleets' calculation
            float sTotalValue = 0f; // Total value contribution for fleets - should equal sAllFleet!
            float sTotalBiomass = 0f; // Total biomass contribution for fleets - should equal sAllFleet!
            float sContrValue = 0f; // Value contribution for a single fleet
            float sContrBiomass = 0f; // Biomass contribution for a single fleet

            if (iItem == 0)
            {
                sValueContribution = 1f;
                sBiomassContribution = 1f;
                return;
            }

            sValueContribution = 0f;
            sBiomassContribution = 0f;

            try
            {
                sAllItemsValue = m_ValueContribution[0, unit.Sequence, iTimestep];
                sAllItemsBiomass = m_BiomassContribution[0, unit.Sequence, iTimestep];

                for (int i = 1, loopTo = m_iMaxItem; i <= loopTo; i++)
                {
                    sTotalValue += m_ValueContribution[i, unit.Sequence, iTimestep];
                    sTotalBiomass += m_BiomassContribution[i, unit.Sequence, iTimestep];
                }

                // ************** VALIDATION ***************
                // Contributions of all fleets [1..n] should equal the contribution of fleet 0
                // Debug.Assert(sAllFleetValue = sTotalValue, "Error: contribution of individual fleets does not match the contributions of all fleets.")
                // Debug.Assert(sAllFleetBiomass = sTotalBiomass, "Error: contribution of individual fleets does not match the contributions of all fleets.")
                // ************** VALIDATION ***************

                sContrValue = m_ValueContribution[iItem, unit.Sequence, iTimestep];
                sContrBiomass = m_BiomassContribution[iItem, unit.Sequence, iTimestep];
            }
            catch (Exception ex)
            {
                Debug.Assert(false, "VC: Failure obtaining contribution for item");
            }

            // Calc contributions
            if (sTotalValue > 0f)
            {
                sValueContribution = sContrValue / sTotalValue;
            }
            if (sTotalBiomass > 0f)
            {
                sBiomassContribution = sContrBiomass / sTotalBiomass;
            }

        }

        #endregion

        ~cValueChainResults()
        {
        }

    }
}