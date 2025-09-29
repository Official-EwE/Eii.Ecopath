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

namespace ValueChain
{

    #endregion

    [TypeConverter(typeof(cPropertySorter))]
    [DefaultProperty("Name")]
    [Serializable()]
    public abstract class cUnit : cValueChainEntity
    {

        protected const string sPROPCAT_GENERAL = "01. General";
        protected const string sPROPCAT_VALIDATION = "02. Validation";
        protected const string sPROPCAT_PRODUCTS = "03. Products ($/t)";
        protected const string sPROPCAT_REVENUE = "04. Revenue ($/effort)";
        protected const string sPROPCAT_SUBSIDIES = "05. Subsidies ($/t)";
        protected const string sPROPCAT_PAY = "06. Pay ($/t)";
        protected const string sPROPCAT_SHARE = "07. Share (% revenue)";
        protected const string sPROPCAT_INPUTCOST = "08. Input cost ($/t)";
        protected const string sPROPCAT_TAXES = "09. Taxes ($/t)";
        protected const string sPROPCAT_SOCIAL = "10. Social (#/t)";

        /// <summary>Index of the unit, which this unit needs to store its values in the Results object</summary>
        private int m_iSequence = 0;
        /// <summary>List of input variables that this unit needs in order to perform its calculations.</summary>
        protected List<cInput> m_lReceivedInputs = new List<cInput>();
        /// <summary>Name of the unit</summary>
        private string m_strName;
        /// <summary>Local name of the unit.</summary>
        private string m_strNameLocal;
        /// <summary>Nationality of a unit.</summary>
        private int m_iNationality;

        private bool m_bCanCompute = false;
        private bool m_bRunStarted = false;

        /// <summary>Units that receive outputs from this unit.</summary>
        protected List<cLink> m_llinkOutput = new List<cLink>();
        /// <summary>Units that provide inputs for this unit.</summary>
        protected List<cLink> m_llinkInput = new List<cLink>();

        #region  Constructor 

        /// -----------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------
        public cUnit() : base()
        {
        }

        #endregion

        #region  Links 

        public int LinkOutCount()
        {
            return m_llinkOutput.Count;
        }

        public cLink LinkOut(int iIndex)
        {
            return m_llinkOutput[iIndex];
        }

        /// <summary>
    /// Get all links directly linking to a target.
    /// </summary>
    /// <param name="unitTarget"></param>
    /// <returns></returns>
        public cLink[] Links(cUnit unitTarget)
        {
            var lLinks = new List<cLink>();
            foreach (cLink link in m_llinkOutput)
            {
                if (ReferenceEquals(link.Target, unitTarget))
                {
                    lLinks.Add(link);
                }
            }
            return lLinks.ToArray();
        }

        public void AddLink(cLink link)
        {

            // Sanity check
            Debug.Assert(ReferenceEquals(link.Source, this));

            m_llinkOutput.Add(link);
            link.Target.AddInputLink(link);
        }

        public void RemoveLink(cLink link)
        {
            m_llinkOutput.Remove(link);
            link.Target.RemoveInputLink(link);
        }

        public int LinkInCount()
        {
            return m_llinkInput.Count;
        }

        public cLink LinkIn(int iIndex)
        {
            return m_llinkInput[iIndex];
        }

        protected void AddInputLink(cLink link)
        {
            // Sanity check
            Debug.Assert(ReferenceEquals(link.Target, this));
            m_llinkInput.Add(link);
            UpdateComputeStatus();
        }

        protected void RemoveInputLink(cLink link)
        {
            m_llinkInput.Remove(link);
            UpdateComputeStatus();
        }

        public bool IsLoop(cUnit unit)
        {

            // Linked to self?
            bool bIsLoop = ReferenceEquals(unit, this);

            // If no loop yet
            if (!bIsLoop)
            {
                // Follow each output link
                foreach (cLink link in m_llinkOutput)
                {
                    // See the target link is the requesting unit
                    if (link.Target.IsLoop(unit))
                    {
                        bIsLoop = true;
                        break;
                    }
                }
            }

            return bIsLoop;
        }

        // Public Overridable Function HasTarget(unit As cUnit) As Boolean

        // ' Follow each output link
        // For Each link As cLink In Me.m_llinkOutput
        // ' See the target link is the requesting unit
        // If Object.ReferenceEquals(link.Target, unit) Then Return True
        // Next link
        // Return False

        // End Function

        #endregion

        #region  Running 

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Initialize the unit for a new Ecosim or Ecospace run.
    /// </summary>
    /// <param name="iSequence">The sequence number to assign to this unit for the run.</param>
    /// -----------------------------------------------------------------------
        internal virtual void InitRun(int iSequence)
        {
            Sequence = iSequence;
            m_bRunStarted = true;
            Clear();
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Initialize the unit for running a chain.
    /// </summary>
    /// -----------------------------------------------------------------------
        internal virtual void Clear()
        {
            // Clear all pending inputs
            m_lReceivedInputs.Clear();
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Calculate the economics for this unit.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="input"></param>
    /// <param name="iTimeStep"></param>
    /// <param name="iUnit">The unit to aggregate by.</param>
    /// -----------------------------------------------------------------------
        public virtual void Process(cValueChainResults results, cInput input, int iTimeStep, int iUnit)
        {

            float sTotalOutputBiomass = 0f;
            float sTotalOutputValue = 0f;
            float sValuePerTon = 0.0f;

            // Store received values
            m_lReceivedInputs.Add(input);

            // Sanity check
            Debug.Assert(m_lReceivedInputs.Count <= LinkInCount());

            // At least expected inputs received?
            if (m_lReceivedInputs.Count == LinkInCount())
            {

                // #Yes: Process combined inputs
                input = ProcessAndSumInputs(m_lReceivedInputs, results);

                // Store the amount that each fleet contributes to the total
                results.StoreContribution(iUnit, this, iTimeStep, input.Value, input.Tons);

                // Determine outgoing biomass
                foreach (cLink link in m_llinkOutput)
                {
                    // Determine output biomass for a single link
                    float sOutputBiomass = link.BiomassRatio * input.Tons;
                    float sOutputValue = 0f;

                    if (link.ValuePerTon != 1.0f & link.ValuePerTon != 0f | input.Tons == 0f)
                    {
                        sOutputValue = link.ValuePerTon * sOutputBiomass;
                    }
                    else
                    {
                        sOutputValue = input.Value / input.Tons * link.ValueRatio * sOutputBiomass;
                    }

                    sTotalOutputBiomass += sOutputBiomass;
                    sTotalOutputValue += sOutputValue;

                    link.Target.Process(results, new cInput(this, sOutputBiomass, sOutputValue), iTimeStep, iUnit);

                }

                // Running for all fleet?
                if (iUnit == 0)
                {
                    // #Yes: make all calculations. Calculations are not necessary when running for individual fleets
                    // where only transfer ratios are collected.
                    Calculate(results, input.Tons, input.Value, sTotalOutputBiomass, sTotalOutputValue, iTimeStep);
                }

            }

        }

        protected cInput ProcessAndSumInputs(List<cInput> lInputs, cValueChainResults results)
        {

            float sTonsTotal = 0.0f;
            float sValueTotal = 0.0f;

            foreach (cInput input in lInputs)
            {
                if (input.Tons > 0f)
                {

                    sTonsTotal += input.Tons;
                    sValueTotal += input.Value;

                    results.get_FlowsBiomass(input.Source.Sequence, Sequence);

                }
            }
            return new cInput(null, sTonsTotal, sValueTotal);

        }

        /// <summary>
    /// Make all calculations.
    /// </summary>
    /// <param name="results">The results object to store calculation results in.</param>
        protected virtual bool Calculate(cValueChainResults results, float sInputBiomass, float sInputValue, float sOutputBiomass, float sOutputValue, int iTimeStep)
        {

            // All good
            return true;

        }

        /// <summary>
    /// Assess whether a unit is ready to compute, e.g. when all its
    /// inputs are (in)directly connected to EwE model data.
    /// </summary>
        private void UpdateComputeStatus()
        {

            // Check if all input links can compute
            bool bCanCompute = true;
            bool bHasInputs = false;
            foreach (cLink LinkIn in m_llinkInput)
            {
                bCanCompute = bCanCompute & LinkIn.Source.CanCompute;
                bHasInputs = true;
            }
            bCanCompute = bCanCompute & bHasInputs;

            // No changes? Abort
            if (bCanCompute == CanCompute)
                return;

            m_bCanCompute = bCanCompute;

            foreach (cLink linkOut in m_llinkOutput)
                linkOut.Target.UpdateComputeStatus();

        }

        #endregion

        #region  Properties 

        public override string ToString()
        {
            return Name;
        }

        [Browsable(false)]
        public int Sequence
        {
            get
            {
                return m_iSequence;
            }
            private set
            {
                m_iSequence = value;
            }
        }

        [Browsable(false)]
        public abstract cUnitFactory.eUnitType UnitType { get; }

        [Browsable(false)]
        public virtual bool HasError
        {
            get
            {
                return false;
            }
        }

        // <Browsable(False)> _
        // Public Overridable ReadOnly Property Style() As cStyleGuide.eStyleFlags
        // Get
        // Return cStyleGuide.eStyleFlags.OK
        // End Get
        // End Property

        [Browsable(false)]
        public virtual bool CanCompute
        {
            get
            {
                return m_bCanCompute;
            }
        }

        [Browsable(false)]
        public virtual bool IsRunError
        {
            get
            {
                // Return if all results received OR when not ready to run yet
                return m_lReceivedInputs.Count < m_llinkInput.Count & m_bRunStarted;
            }
        }

        #region  General 

        [Browsable(true)]
        [Category(sPROPCAT_GENERAL)]
        [DisplayName("Name")]
        [Description("Name of this unit")]
        [cPropertySorter.PropertyOrder(1)]
        public virtual string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_GENERAL)]
        [DisplayName("Category")]
        [Description("Category to which this unit belongs")]
        [cPropertySorter.PropertyOrder(2)]
        public abstract string Category { get; }

        [Browsable(true)]
        [Category(sPROPCAT_VALIDATION)]
        [DisplayName("Biomass ratio")]
        [Description("Total biomass ratio passed out of this unit")]
        [cPropertySorter.PropertyOrder(8)]
        public virtual string BiomassRatio
        {
            get
            {
                float sTot = 0f;
                for (int i = 0, loopTo = LinkOutCount() - 1; i <= loopTo; i++)
                    sTot += LinkOut(i).BiomassRatio;
                return sTot.ToString();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_GENERAL)]
        [DisplayName("Nationality")]
        [Description("Nationality of this unit")]
        [cPropertySorter.PropertyOrder(4)]
        public virtual int Nationality
        {
            get
            {
                return m_iNationality;
            }
            set
            {
                m_iNationality = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(sPROPCAT_GENERAL)]
        [DisplayName("Name (local)")]
        [Description("Local name of this unit")]
        [cPropertySorter.PropertyOrder(5)]
        public virtual string NameLocal
        {
            get
            {
                return m_strNameLocal;
            }
            set
            {
                m_strNameLocal = value;
                SetChanged();
            }
        }

        [Browsable(false)]
        public abstract bool IsDefault { get; }

        #endregion

        #endregion

    }
}