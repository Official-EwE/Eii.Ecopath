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
    /// Species-dependent link.
    /// </summary>
    /// ===========================================================================
    [TypeConverter(typeof(cPropertySorter))]
    [DefaultProperty("Landings")]
    [Serializable()]
    public class cLinkLandings : cLink
    {

        #region  Helper classes 


        #endregion

        #region  Private bits 

        private string m_species = "";

        #endregion

        public cLinkLandings() : base()
        {
        }

        #region  Ecopath integration 

        public override string Name { get; set; }

        #endregion

        #region  Overrides 

        [Browsable(false)]
        public virtual string Species
        {
            get
            {
                return m_species;
            }
            internal set
            {
                m_species = value;
            }
        }

        [Browsable(false)]
        public override float ValuePerTon
        {
            get
            {
                return 0f;
            }
            set
            {
                // nop
            }
        }

        public override bool IsDefault
        {
            get
            {
                return true;
            }
        }

        public override bool IsVisible()
        {
            // If (TypeOf Me.Source Is cProducerUnit) Then
            // Dim fleet As cEcopathFleetInput = DirectCast(Me.Source, cProducerUnit).Fleet
            // Dim group As cEcoPathGroupInput = Me.Group
            // If (fleet IsNot Nothing) And (group IsNot Nothing) Then
            // Return (fleet.Landings(group.Index) > 0)
            // End If
            // End If
            return true;
        }

        public override bool IsConfigured
        {
            get
            {
                return base.IsConfigured & !string.IsNullOrWhiteSpace(((cProducerUnit)Source).Fleet);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is cLinkLandings))
                return false;
            cLinkLandings ll = (cLinkLandings)obj;
            return base.Equals(obj) & string.Compare(ll.m_species, Species) == 0;
        }

        #endregion

    }
}