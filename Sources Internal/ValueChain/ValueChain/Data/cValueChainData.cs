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
using System.IO;
using System.Xml.Linq;

namespace ValueChain
{

    #endregion

    /// ===========================================================================
    /// <summary>
    /// Value chain central data storage.
    /// </summary>
    /// <remarks>
    /// Inherited from cCoreInputOutputBase to be able to use cCore.OnChanged.
    /// </remarks>
    /// ===========================================================================
    public class cValueChainData
    {

        #region  Private vars 

        private List<cUnit> m_lUnits = new List<cUnit>();
        private List<cLink> m_lLinks = new List<cLink>();

        private List<cUnit> m_lUnitDefaults = new List<cUnit>();
        private List<cLinkDefault> m_lLinkDefaults = new List<cLinkDefault>();

        private List<cFlowDiagram> m_lFlowDiagrams = new List<cFlowDiagram>();
        private List<cFlowPosition> m_lFlowPositions = new List<cFlowPosition>();

        private bool m_bInitializing = false;

        private static cValueChainData s_inst = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public cValueChainData()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            // Properly detach events
            while (m_lUnits.Count > 0)
                RemoveUnit(m_lUnits[0]);
            while (m_lUnitDefaults.Count > 0)
                RemoveUnitDefault(m_lUnitDefaults[0]);
            while (m_lLinks.Count > 0)
                RemoveLink(m_lLinks[0]);
            while (m_lLinkDefaults.Count > 0)
                RemoveLinkDefault(m_lLinkDefaults[0]);
            while (m_lFlowPositions.Count > 0)
                RemoveFlowPosition(m_lFlowPositions[0]);
            while (m_lFlowDiagrams.Count > 0)
                RemoveFlowDiagram(m_lFlowDiagrams[0]);
            IsChanged = false;
        }

        #region  Database access 

        /// <summary>
        /// Get/set whether the data has unsaved changes.
        /// </summary>
        public bool IsChanged { get; private set; }

        #endregion

        #region  Running 

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Init the data for a new run by resetting all units.
        /// </summary>
        /// -----------------------------------------------------------------------
        public bool InitRun()
        {
            cUnit unit = null;
            // Re-index 
            for (int iSequence = 0, loopTo = UnitCount() - 1; iSequence <= loopTo; iSequence++)
            {
                unit = Unit(iSequence);
                // Sequence is zero-based 
                unit.InitRun(iSequence);
            }
            return true;
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Init the data for a new run by resetting all units.
        /// </summary>
        /// -----------------------------------------------------------------------
        public bool InitTimeStep()
        {
            cUnit unit = null;
            for (int iUnit = 0, loopTo = UnitCount() - 1; iUnit <= loopTo; iUnit++)
            {
                unit = Unit(iUnit);
                unit.Clear();
            }

            return default;
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Diagnostics to determine whether the entire chain computed correctly.
        /// </summary>
        /// -----------------------------------------------------------------------
        public bool HasCompletedRun()
        {
            cUnit unit = null;
            for (int iSequence = 0, loopTo = UnitCount() - 1; iSequence <= loopTo; iSequence++)
            {
                unit = Unit(iSequence);
                if (unit.IsRunError)
                {
                    /* TODO ERROR: Skipped IfDirectiveTrivia
                    #If DEBUG Then
                    */
                    Debug.Assert(false, "Chain did not compute correctly for unit " + unit.Name);
                    /* TODO ERROR: Skipped EndIfDirectiveTrivia
                    #End If
                    */
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region  Parameters 

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Get the parameters that dictate how this monster will run.
        /// </summary>
        /// -----------------------------------------------------------------------
        public readonly cParameters Parameters;

        #endregion

        #region  Defaults 

        public cUnit GetUnitDefault(cUnitFactory.eUnitType unitType)
        {
            cUnit unit = null;
            // Try to find
            foreach (var currentUnit in m_lUnitDefaults)
            {
                unit = currentUnit;
                if (unit.UnitType == unitType)
                    return unit;
            }
            // Not found: create it
            unit = cUnitFactory.CreateUnitDefault(unitType);
            AddUnitDefault(unit);
            return unit;
        }

        public void AddUnitDefault(cUnit unit)
        {
            if (unit is not null)
            {
                m_lUnitDefaults.Add(unit);
                // Start listening for generic change events
                unit.OnChanged += OnEntityChanged;
            }
        }

        public void RemoveUnitDefault(cUnit unit)
        {
            if (unit is not null)
            {
                m_lUnitDefaults.Remove(unit);
                // Start listening for generic change events
                unit.OnChanged -= OnEntityChanged;
            }
        }

        public cLinkDefault GetLinkDefault(cLinkFactory.eLinkType linkType)
        {
            cLinkDefault link = null;
            // Try to find
            foreach (var currentLink in m_lLinkDefaults)
            {
                link = currentLink;
                if (link.LinkType == (int)linkType)
                    return link;
            }
            // Not found: create it
            link = cLinkFactory.CreateLinkDefault(linkType);
            AddLinkDefault(link);
            return link;
        }

        public void AddLinkDefault(cLinkDefault link)
        {
            if (link is not null)
            {
                m_lLinkDefaults.Add(link);
                // Start listening for generic change events
                link.OnChanged += OnEntityChanged;
            }
        }

        public void RemoveLinkDefault(cLinkDefault link)
        {
            if (link is not null)
            {
                m_lLinkDefaults.Remove(link);
                // Stop listening for generic change events
                link.OnChanged -= OnEntityChanged;
            }
        }

        #endregion

        #region  Units 

        public int UnitCount()
        {
            return m_lUnits.Count;
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Get a unit from the lit of units
    /// </summary>
    /// <param name="iIndex">Zero-based unit index.</param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------
        public cUnit Unit(int iIndex)
        {
            return m_lUnits[iIndex];
        }

        public cUnit[] GetUnits(cUnitFactory.eUnitType unitType)
        {
            var lUnits = new List<cUnit>();
            cUnit unit = null;
            var tUnit = cUnitFactory.MapType(unitType);
            bool bAdd = false;
            for (int i = 0, loopTo = m_lUnits.Count - 1; i <= loopTo; i++)
            {
                unit = m_lUnits[i];
                // Need to filter by unit type?
                if (tUnit is not null)
                {
                    // #Yes: check unit type
                    bAdd = tUnit.IsInstanceOfType(unit);
                }
                else
                {
                    // #No: assume all is well
                    bAdd = true;
                }

                // Hide default units
                if (unit.IsDefault)
                {
                    bAdd = false;
                }

                if (bAdd)
                {
                    lUnits.Add(unit);
                }
            }
            return lUnits.ToArray();
        }

        /// <summary>
    /// Create a unit in the database
    /// </summary>
    /// <param name="unitType"></param>
    /// <param name="strName"></param>
    /// <returns></returns>
    /// <remarks></remarks>
        public cUnit CreateUnit(cUnitFactory.eUnitType unitType, string strName)
        {
            var unit = cUnitFactory.CreateUnit(unitType);
            if (unit is not null)
            {
                // Populate unit with defaults
                unit.CopyFrom(GetUnitDefault(unitType));
                // Set default name
                unit.Name = strName;
                // Add it to the local admin
                AddUnit(unit);
            }
            return unit;
        }

        /// <summary>
    /// Delete a unit
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool DeleteUnit(cUnit unit)
        {

            // Remove all incoming links from the database
            for (int iLink = 0, loopTo = unit.LinkInCount() - 1; iLink <= loopTo; iLink++)
            {
                var link = unit.LinkIn(iLink);
                RemoveLink(link);
            }

            // Remove all outgoing links from the database
            for (int iLink = 0, loopTo1 = unit.LinkOutCount() - 1; iLink <= loopTo1; iLink++)
            {
                var link = unit.LinkOut(iLink);
                RemoveLink(link);
            }

            // Remove all related flow positions from the database
            foreach (cFlowPosition fp in FlowPositions(unit))
                RemoveFlowPosition(fp);

            // Remove the unit from local admin
            RemoveUnit(unit);

            return true;

        }

        /// <summary>
    /// Add a unit to the local administration
    /// </summary>
    /// <param name="unit"></param>
    /// <remarks></remarks>
        public void AddUnit(cUnit unit)
        {

            if (unit is null)
                return;

            // Add
            m_lUnits.Add(unit);
            IsChanged = true;

            // Start listening for generic change events
            unit.OnChanged += OnEntityChanged;

        }

        /// <summary>
    /// Remove a unit from the local administration
    /// </summary>
    /// <param name="unit"></param>
    /// <remarks></remarks>
        public void RemoveUnit(cUnit unit)
        {

            if (unit is null)
                return;

            // Stop listening for generic change events
            unit.OnChanged -= OnEntityChanged;

            try
            {
                m_lUnits.Remove(unit);
            }

            catch (Exception ex)
            {
                Debug.Assert(false);
            }

            // Remove all links from this unit
            while (unit.LinkInCount() > 0)
                RemoveLink(unit.LinkIn(0));

            // Remove all links to this unit
            while (unit.LinkOutCount() > 0)
                RemoveLink(unit.LinkOut(0));

            // Remove all flow positions pertaining to this unit
            foreach (cFlowPosition fp in FlowPositions(unit))
                RemoveFlowPosition(fp);

            m_lUnits.Remove(unit);

        }

        #region  Metier management 


        private void OnEntityChanged(cValueChainEntity element )
        {
            this.IsChanged = true;
        }

        // Friend Function FindEcopathGroupByID(iDBID As Integer) As cEcoPathGroupInput
        // Dim group As cEcoPathGroupInput = Nothing
        // For i As Integer = 1 To Me.m_core.nGroups
        // group = Me.m_core.EcopathGroupInputs(i)
        // If CInt(group.GetVariable(eVarNameFlags.DBID)) = iDBID Then Return group
        // Next
        // Return Nothing
        // End Function

        // Friend Function FindEcopathFleetByID(iDBID As Integer) As cEcopathFleetInput
        // Dim fleet As cEcopathFleetInput = Nothing
        // For i As Integer = 1 To Me.m_core.nFleets
        // fleet = Me.m_core.EcopathFleetInputs(i)
        // If CInt(fleet.GetVariable(eVarNameFlags.DBID)) = iDBID Then Return fleet
        // Next
        // Return Nothing
        // End Function

        #endregion

        #endregion

        #region  Links 

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Get all visible links of a given type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="bIncludeInvisible">Flag stating that also links that are not
        /// <see cref="cLink.IsVisible">visible</see> may be included.</param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------
        public cLink[] GetLinks(Type t, bool bIncludeInvisible = false)
        {

            var lLinks = new List<cLink>();
            cLink link = null;
            bool bAdd = false;
            for (int i = 0, loopTo = m_lLinks.Count - 1; i <= loopTo; i++)
            {
                link = m_lLinks[i];
                // Need to filter by unit type?
                if (t is not null)
                {
                    // #Yes: check link type
                    bAdd = t.Equals(link.GetType()) & (link.IsVisible() | bIncludeInvisible);
                }
                else
                {
                    // #No: assume all is well
                    bAdd = true;
                }

                if (bAdd)
                {
                    lLinks.Add(link);
                }
            }
            return lLinks.ToArray();

        }

        public int LinkCount()
        {
            return m_lLinks.Count;
        }

        public cLink LinkByID(int iDBID)
        {
            foreach (cLink link in m_lLinks)
            {
                if (link.DBID == iDBID)
                    return link;
            }
            return null;
        }

        public cLink Link(int iIndex)
        {
            return m_lLinks[iIndex];
        }

        public cLinkLandings CreateLandingsLink(cProducerUnit unitSource, cUnit unitTarget, string species, ref bool bError, bool bQuiet = false)
        {

            // Sanity check
            if (unitSource is null | unitTarget is null)
            {
                // ToDo: log this
                // If Not bQuiet Then Me.SendMessage(My.Resources.ERROR_LINK_NEEDUNITS)
                bError = true;
                return null;
            }

            // Check if link is allowed
            if (!cLinkFactory.CanCreateLink(unitSource, unitTarget))
            {
                // ToDo: log this
                // If Not bQuiet Then Me.SendMessage(My.Resources.ERROR_LINK_NOTALLOWED)
                bError = true;
                return null;
            }

            // Check for loop
            if (unitTarget.IsLoop(unitSource))
            {
                // ToDo: log this
                // If Not bQuiet Then Me.SendMessage(My.Resources.ERROR_LINK_LOOP)
                bError = true;
                return null;
            }

            // Check for already present link
            if (unitSource.HasTarget(unitTarget, species))
            {
                // ToDo: log this
                // Dim fmt As New cCoreInterfaceFormatter()
                // If Not bQuiet Then Me.SendMessage(cStringUtils.Localize(My.Resources.ERROR_LINK_DUPLICATE, fmt.ToString(species)))
                bError = true;
                return null;
            }

            // If (unitSource.Fleet.Landings(species.Index) = 0) Then Return Nothing

            var link = new cLinkLandings();

            // Provide link with defaults
            link.CopyFrom(GetLinkDefault(cLinkFactory.GetLinkType(unitSource, unitTarget)));

            link.Source = unitSource;
            link.Target = unitTarget;
            link.Species = species;
            IsChanged = true;

            AddLink(link);

            return link;
        }

        /// <summary>
    /// Create a link in the database 
    /// </summary>
    /// <param name="unitSource"></param>
    /// <param name="unitTarget"></param>
    /// <returns></returns>
        public cLink CreateLink(cUnit unitSource, cUnit unitTarget)
        {

            // Sanity check
            if (unitSource is null | unitTarget is null)
            {
                // ToDo: log this
                // Me.SendMessage(My.Resources.ERROR_LINK_NEEDUNITS)
                return null;
            }

            // Check if link is allowed
            if (!cLinkFactory.CanCreateLink(unitSource, unitTarget))
            {
                // ToDo: log this
                // Me.SendMessage(My.Resources.ERROR_LINK_NOTALLOWED)
                return null;
            }

            // Check if not already exists


            // Check for loop
            if (unitTarget.IsLoop(unitSource))
            {
                // ToDo: log this
                // Me.SendMessage(My.Resources.ERROR_LINK_LOOP)
                return null;
            }

            var link = new cLink();

            // Provide link with defaults
            link.CopyFrom(GetLinkDefault(cLinkFactory.GetLinkType(unitSource, unitTarget)));

            link.Source = unitSource;
            link.Target = unitTarget;
            IsChanged = true;

            AddLink(link);

            return link;

        }

        /// <summary>
    /// Add an output link to the local administration
    /// </summary>
    /// <param name="link"></param>
        public bool AddLink(cLink link)
        {

            // Sanity check
            Debug.Assert(link is not null);
            Debug.Assert(link.Target is not null);
            Debug.Assert(link.Source is not null);

            if (HasLink(link))
                return false;

            m_lLinks.Add(link);
            link.Source.AddLink(link);

            // Start listening for link change events
            link.OnChanged += OnEntityChanged;
            return true;

        }

        /// <summary>
        /// Remove an output link from the local administration
        /// </summary>
        /// <param name="link"></param>
        public void RemoveLink(cLink link)
        {

            // Sanity check
            Debug.Assert(link is not null);

            // Stop listening for link change events
            link.OnChanged -= OnEntityChanged;

            m_lLinks.Remove(link);
            link.Source.RemoveLink(link);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public bool HasLink(cLink link)
        {
            foreach (cLink l in m_lLinks)
            {
                if (link.Equals(l) & l.Equals(link))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region  Flow diagrams 

        /// <summary>
        /// Get the number of flow diagrams in the chain. There is always one.
        /// </summary>
        /// <returns></returns>
        public int FlowDiagramCount()
        {
            return m_lFlowDiagrams.Count;
        }

        /// <summary>
        /// Get a given flow diagram.
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        public cFlowDiagram FlowDiagram(int iIndex)
        {
            if (m_lFlowDiagrams.Count == 0)
            {
                CreateFlowDiagram(new cFlowDiagram());
            }
            return m_lFlowDiagrams[iIndex];
        }

        /// <summary>
        /// Create and add a new flow diagram.
        /// </summary>
        /// <param name="diagram"></param>
        public void CreateFlowDiagram(cFlowDiagram diagram)
        {
            AddFlowDiagram(diagram);
            IsChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagram"></param>
        public void DeleteFlowDiagram(cFlowDiagram diagram)
        {
            AddFlowDiagram(diagram);
            IsChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagram"></param>
        public void AddFlowDiagram(cFlowDiagram diagram)
        {
            m_lFlowDiagrams.Add(diagram);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagram"></param>
        public void RemoveFlowDiagram(cFlowDiagram diagram)
        {
            m_lFlowDiagrams.Remove(diagram);
        }

        #endregion

        #region  Flow positions 

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int FlowPositionCount()
        {
            return m_lFlowPositions.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        public cFlowPosition FlowPosition(int iIndex)
        {
            if (m_lFlowDiagrams.Count == 0)
            {
                CreateFlowDiagram(new cFlowDiagram());
            }
            return m_lFlowPositions[iIndex];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="diagram"></param>
        /// <returns></returns>
        public cFlowPosition CreateFlowPosition(cUnit unit, cFlowDiagram diagram)
        {

            // Sanity checks
            Debug.Assert(unit is not null);
            Debug.Assert(diagram is not null);

            var fp = new cFlowPosition();
            fp.Unit = unit;
            fp.Diagram = diagram;

            AddFlowPosition(fp);
            IsChanged = true;

            return fp;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public void AddFlowPosition(cFlowPosition pos)
        {

            // Sanity check
            Debug.Assert(pos is not null);
            Debug.Assert(pos.Unit is not null);

            // Start listening for generic change events
            pos.OnChanged += OnEntityChanged;

            m_lFlowPositions.Add(pos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public void RemoveFlowPosition(cFlowPosition pos)
        {

            // Sanity check
            Debug.Assert(pos is not null);

            // Stop listening for generic change events
            pos.OnChanged -= OnEntityChanged;

            m_lFlowPositions.Remove(pos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public cFlowPosition[] FlowPositions(cUnit unit)
        {
            var lfp = new List<cFlowPosition>();
            cFlowPosition fp = null;
            for (int i = 0, loopTo = m_lFlowPositions.Count - 1; i <= loopTo; i++)
            {
                fp = m_lFlowPositions[i];
                // Compare object references since DBIDs can be 0 (for unsaved objects)
                if (ReferenceEquals(fp.Unit, unit))
                    lfp.Add(fp);
            }
            return lfp.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagram"></param>
        /// <returns></returns>
        public cFlowPosition[] FlowPositions(cFlowDiagram diagram)
        {
            var lfp = new List<cFlowPosition>();
            cFlowPosition fp = null;
            for (int i = 0, loopTo = m_lFlowPositions.Count - 1; i <= loopTo; i++)
            {
                fp = m_lFlowPositions[i];
                // Compare object references since DBIDs can be 0 (for unsaved objects)
                if (ReferenceEquals(fp.Diagram, diagram))
                    lfp.Add(fp);
            }
            return lfp.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="diagram"></param>
        /// <returns></returns>
        public cFlowPosition FlowPosition(cUnit unit, cFlowDiagram diagram)
        {
            cFlowPosition fp = null;
            for (int i = 0, loopTo = m_lFlowPositions.Count - 1; i <= loopTo; i++)
            {
                fp = m_lFlowPositions[i];
                // Compare object references since DBIDs can be 0 (for unsaved objects)
                if (ReferenceEquals(fp.Diagram, diagram) & ReferenceEquals(fp.Unit, unit))
                {
                    return fp;
                }
            }
            return null;
        }

        #endregion

        #region  Core access 

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Get all units in the flow that are linked to this unit,
        /// either serving as source units or as target units.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------
        public List<cUnit> GetConnectedUnits(cUnit unit)
        {

            var lUnits = new List<cUnit>();

            // Sanity check
            Debug.Assert(unit is not null);

            GetSourceUnits(unit, lUnits);
            GetTargetUnits(unit, lUnits);

            // Sanity check
            Debug.Assert(lUnits.IndexOf(unit) == -1);

            // Add m'self
            lUnits.Add(unit);
            // Done
            return lUnits;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Load() 
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            return true;
        }

        #endregion

        #region  Internals 

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Get all units that serve as source units to a given unit.
        /// </summary>
        /// <param name="unit">The unit to test incoming links for.</param>
        /// <param name="lUnits">The list that will receive the linked units.</param>
        /// -----------------------------------------------------------------------
        private void GetSourceUnits(cUnit unit, List<cUnit> lUnits)
        {
            cUnit unitSource = null;
            for (int iLink = 0, loopTo = unit.LinkInCount() - 1; iLink <= loopTo; iLink++)
            {
                unitSource = unit.LinkIn(iLink).Source;
                if (lUnits.IndexOf(unitSource) == -1)
                {
                    lUnits.Add(unitSource);
                    GetSourceUnits(unitSource, lUnits);
                }
            }
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Get all units that link out of a given unit
        /// </summary>
        /// <param name="unit">The unit to test outgoing links for.</param>
        /// <param name="lUnits">The list that will receive the linked units.</param>
        /// -----------------------------------------------------------------------
        private void GetTargetUnits(cUnit unit, List<cUnit> lUnits)
        {
            cUnit unitTarget = null;
            for (int iLink = 0, loopTo = unit.LinkOutCount() - 1; iLink <= loopTo; iLink++)
            {
                unitTarget = unit.LinkOut(iLink).Target;
                if (lUnits.IndexOf(unitTarget) == -1)
                {
                    lUnits.Add(unitTarget);
                    GetTargetUnits(unitTarget, lUnits);
                }
            }
        }

        #endregion
    }
}