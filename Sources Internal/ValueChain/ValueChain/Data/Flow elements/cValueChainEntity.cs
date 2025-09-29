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

using System;
using System.ComponentModel;
using System.Reflection;

namespace ValueChain
{

    public abstract class cValueChainEntity
    {

        public int DBID { get; set; }

        /// <summary>Flag stating whether an cOOPStorabe instance is allowed to 
    /// broadcast <see cref="OnChanged">OnChanged</see>events.</summary>
        private bool m_bAllowEvents = true;
        /// <summary>Flag preventing looped updates.</summary>
        private bool m_bInUpdate = false;

        /// ---------------------------------------------------------------
    /// <summary>
    /// Event to notify that instance unit has changed
    /// </summary>
    /// <param name="obj">The <see cref="cValueChainEntity">instance</see>
    /// that changed</param>
    /// ---------------------------------------------------------------
        public event OnChangedEventHandler OnChanged;

        public delegate void OnChangedEventHandler(cValueChainEntity obj);

        public void CopyFrom(cValueChainEntity obj)
        {
            // Me.AllowEvents = False
            PropertyInfo[] apiSrc = null;
            PropertyInfo[] apiTgt = null;

            if (obj is null)
                return;

            // Copy all copyable properties
            apiSrc = obj.GetType().GetProperties();
            apiTgt = GetType().GetProperties();
            foreach (var piSrc in apiSrc)
            {
                if (string.Compare(piSrc.Name, "DBID") != 0)
                {
                    foreach (var piTgt in apiTgt)
                    {
                        if ((piSrc.Name ?? "") == (piTgt.Name ?? ""))
                        {
                            try
                            {
                                if (piTgt.CanWrite)
                                {
                                    piTgt.SetValue(this, piSrc.GetValue(obj, null), null);
                                }
                            }
                            catch (Exception ex)
                            {
                                /* TODO ERROR: Skipped IfDirectiveTrivia
                                #If VERBOSE_LEVEL >= 2 Then
                                *//* TODO ERROR: Skipped DisabledTextTrivia
                                                                ' Ok, this did not work
                                                                Console.WriteLine("Woops: failed to copy prop {0} : {1}", piTgt.Name, ex.Message)
                                *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                                #End If
                                */
                            }
                        }
                    }
                }
            }
            // Me.AllowEvents = True
        }


        private bool m_bIsDirty = false;

        public void SetChanged(bool bIsDirty = true)
        {
            m_bIsDirty = bIsDirty;

            if (m_bAllowEvents)
            {
                if (m_bInUpdate == false)
                {
                    // Set deadlonk prevention lock
                    m_bInUpdate = true;
                    // Raise event
                    OnChanged?.Invoke(this);
                    // Release deadlonk prevention lock
                    m_bInUpdate = false;
                }
            }

        }

        public bool IsChanged
        {
            get
            {
                return m_bIsDirty;
            }
        }

        /// ---------------------------------------------------------------
    /// <summary>
    /// Get/set whether this instance is allowed to send
    /// <see cref="OnChanged">change events</see>.
    /// </summary>
    /// ---------------------------------------------------------------
        [Browsable(false)]
        public bool AllowEvents
        {
            get
            {
                return m_bAllowEvents;
            }
            set
            {
                m_bAllowEvents = value;
                if (m_bAllowEvents)
                    SetChanged();
            }
        }

    }
}