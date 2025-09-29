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


namespace ValueChain
{

    #endregion

    /// ===========================================================================
/// <summary>
/// Position of a single unit in a flow diagram.
/// </summary>
/// ===========================================================================
    public class cFlowPosition : cValueChainEntity
    {

        #region  Private vars 

        private cFlowDiagram m_diagram = null;
        private cUnit m_unit = null;

        private int m_iX = 0;
        private int m_iY = 0;
        private int m_iWidth = 0;
        private int m_iHeight = 0;

        #endregion

        #region  Properties 

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get/set the diagram this flow position belongs to.
    /// </summary>
    /// -----------------------------------------------------------------------
        public cFlowDiagram Diagram
        {
            get
            {
                return m_diagram;
            }
            set
            {
                if (!ReferenceEquals(value, m_diagram))
                {
                    m_diagram = value;
                    SetChanged();
                }
            }
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get/set the unit that this position belongs to.
    /// </summary>
    /// -----------------------------------------------------------------------
        public cUnit Unit
        {
            get
            {
                return m_unit;
            }
            set
            {
                if (!ReferenceEquals(value, m_unit))
                {
                    m_unit = value;
                    SetChanged();
                }
            }
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get/set the X position.
    /// </summary>
    /// -----------------------------------------------------------------------
        public int Xpos
        {
            get
            {
                return m_iX;
            }
            set
            {
                if (value != m_iX)
                {
                    m_iX = value;
                    SetChanged();
                }
            }
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get/set the Y position.
    /// </summary>
    /// -----------------------------------------------------------------------
        public int Ypos
        {
            get
            {
                return m_iY;
            }
            set
            {
                if (value != m_iY)
                {
                    m_iY = value;
                    SetChanged();
                }
            }
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get/set the width.
    /// </summary>
    /// -----------------------------------------------------------------------
        public int Width
        {
            get
            {
                return m_iWidth;
            }
            set
            {
                if (value != m_iWidth)
                {
                    m_iWidth = value;
                    SetChanged();
                }
            }
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get/set the height.
    /// </summary>
    /// -----------------------------------------------------------------------
        public int Height
        {
            get
            {
                return m_iHeight;
            }
            set
            {
                if (value != m_iHeight)
                {
                    m_iHeight = value;
                    SetChanged();
                }
            }
        }

        #endregion

    }
}