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
using System.Diagnostics;

namespace ValueChain
{
    /// <summary>
    /// The value chain engine
    /// </summary>
    public class cValueChain
    {

        /// <summary>
        /// Value Chain constructor.
        /// </summary>
        /// <param name="data"></param>
        public cValueChain(cValueChainData data)
        {
            this.Data = data;
        }

        /// <summary>
        /// The data that the chain operates on.
        /// </summary>
        public cValueChainData Data { get; private set; }

        /// <summary>
        /// This code presumes that every producer unit already has its data set via
        /// <see cref="cProducerUnit.SetLandings(String, Single, Single)"/>, with optionally
        /// <see cref="cProducerUnit.SetEffort(Single)"/> set too.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="iTimeStep"></param>
        /// <returns></returns>
        private bool Run(cValueChainResults result, int iTimeStep)
        {
            // Prepare data for a time step
            Data.InitTimeStep();

            // For each producer
            foreach (cUnit unit in Data.GetUnits(cUnitFactory.eUnitType.Producer))
            {
                cProducerUnit prodUnit = (cProducerUnit)unit;
                try
                {
                    prodUnit.Process(result, iTimeStep, 0);
                }
                catch (Exception ex)
                {
                    // ToDo: log this
                    Debug.Assert(false, ex.Message);
                }
            }

            return true;
        }
    }
}