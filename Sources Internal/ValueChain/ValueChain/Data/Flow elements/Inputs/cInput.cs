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

namespace ValueChain
{

    /// ===========================================================================
    /// <summary>
    /// A value that entered a cUnit during processing.
    /// </summary>
    /// ===========================================================================
    public class cInput
    {

        private float m_sTons = 0.0f;
        private float m_sValue = 1.0f;
        private cUnit m_src = null;

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sTons">Weight of the product, in tons</param>
        /// <param name="sValue">Total value of the product.</param>
        /// -----------------------------------------------------------------------
        public cInput(cUnit src, float sTons, float sValue)
        {
            m_src = src;
            m_sTons = sTons;
            m_sValue = sValue;
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Get the weight of input in tons of this input.
        /// </summary>
        /// -----------------------------------------------------------------------
        public float Tons
        {
            get
            {
                return m_sTons;
            }
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Get the total value of this input.
        /// </summary>
        /// -----------------------------------------------------------------------
        public float Value
        {
            get
            {
                return m_sValue;
            }
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// The <see cref="cUnit">source</see> of this unit.
        /// </summary>
        /// -----------------------------------------------------------------------
        public cUnit Source
        {
            get
            {
                return m_src;
            }
        }

    }
}