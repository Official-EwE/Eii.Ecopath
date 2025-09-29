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

namespace ValueChain
{

    #endregion

    /// ===========================================================================
/// <summary>
/// Parameters that dictate the behaviour of the Value Chain plug-in.
/// </summary>
/// ===========================================================================
    [Serializable()]
    public class cParameters : cValueChainEntity
    {

        #region  Private vars 

        private bool m_bRunWithEcopath = false;
        private bool m_bRunWithEcosim = false;
        private bool m_bRunSearches = false;
        private bool m_bResultsByFleet = false;
        private List<int> m_liFleets = new List<int>();
        private bool m_bDeletePrompt = true;
        private eAggregationModeType m_aggmode = eAggregationModeType.FullModel;

        private bool m_bAutoSaveResults = false;

        #endregion

        #region  Properties 

        public enum eAggregationModeType : int
        {
            FullModel = 0,
            ByFleet,
            ByGroup
        }

        public List<int> EquilibriumFleetsToVary
        {
            get
            {
                return m_liFleets;
            }
        }

        public float EquilibriumEffortMin { get; set; } = 0.0f;
        public float EquilibriumEffortMax { get; set; } = 4.0f;
        public float EquilibriumEffortIncrement { get; set; } = 0.25f;

        public bool RunWithEcopath
        {
            get
            {
                return m_bRunWithEcopath;
            }
            set
            {
                if (value != m_bRunWithEcopath)
                {
                    m_bRunWithEcopath = value;
                    SetChanged();
                }
            }
        }

        public bool RunWithEcosim
        {
            get
            {
                return m_bRunWithEcosim;
            }
            set
            {
                if (m_bRunWithEcosim != value)
                {
                    m_bRunWithEcosim = value;
                    SetChanged();
                }
            }
        }

        public bool RunWithSearches
        {
            get
            {
                return m_bRunSearches;
            }
            set
            {
                if (m_bRunSearches != value)
                {
                    m_bRunSearches = value;
                    SetChanged();
                }
            }
        }

        public float ZoomFactor { get; set; } = 1.0f;

        public eAggregationModeType AggregationMode
        {
            get
            {
                return m_aggmode;
            }
            set
            {
                m_aggmode = value;
                SetChanged();
            }
        }

        [DefaultValue(true)]
        public bool DeletePrompt
        {
            get
            {
                return m_bDeletePrompt;
            }
            set
            {
                if (value != m_bDeletePrompt)
                {
                    m_bDeletePrompt = value;
                    SetChanged();
                }
            }
        }

        #endregion

    }
}