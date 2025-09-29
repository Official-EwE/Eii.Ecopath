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
using System.IO;

#endregion

namespace ValueChain
{


    /// <summary>
/// CSV writer for Value Chain results.
/// </summary>
    public class cValueChainResultsWriter
    {

        #region  Variables 

        private cValueChainData m_data = null;
        private cValueChainResults m_results = null;

        #endregion

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Shazaam!
    /// </summary>
    /// <param name="data"><see cref="cValueChainData">Value chain data</see> to plunder.</param>
    /// <param name="results"><see cref="cValueChainResults">Value chain results</see> to write.</param>
    /// -----------------------------------------------------------------------
        public cValueChainResultsWriter(cValueChainData data, cValueChainResults results)
        {
            m_data = data;
            m_results = results;
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Write results to CSV file.
    /// </summary>
    /// <param name="agg">Data aggregation method in use during the run.</param>
    /// <returns>True if successful.</returns>
    /// -----------------------------------------------------------------------
        public bool WriteResults(string strFile, cParameters.eAggregationModeType agg, string header = "")
        {
            return WriteResults(strFile, agg, 0, header);
        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="agg"></param>
    /// <param name="iItem"></param>
    /// <param name="header"></param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------
        public bool WriteResults(string strFile, cParameters.eAggregationModeType agg, int iItem, string header = "")
        {

            int iTimeStart = m_results.NumTimeSteps() > 1 ? 0 : 1;
            int iTimeEnd = m_results.NumTimeSteps() > 1 ? 0 : m_results.NumTimeSteps();

            // Dim pout As String = ""
            // Select Case Me.m_results.RunType
            // Case cModel.eRunTypes.Ecopath
            // pout = Path.Combine(Me.m_data.Core.DefaultOutputPath(eAutosaveTypes.Ecopath), "ValueChain")
            // Case cModel.eRunTypes.Ecosim
            // pout = Path.Combine(Me.m_data.Core.DefaultOutputPath(eAutosaveTypes.Ecosim), "ValueChain")
            // Case cModel.eRunTypes.Equilibrium
            // Return False
            // End Select

            try
            {
                for (int iStep = iTimeStart, loopTo = iTimeEnd; iStep <= loopTo; iStep++)
                {

                    using (var sw = new StreamWriter(strFile))
                    {

                        // Start write process

                        // Write EwE header
                        if (!string.IsNullOrWhiteSpace(header))
                        {
                            sw.WriteLine(header);
                        }

                        // Write data header
                        sw.Write("Variable");
                        foreach (cUnit u in m_data.GetUnits(cUnitFactory.eUnitType.All))
                        {
                            sw.Write(",");
                            sw.Write(cStringUtils.ToCSVField(u.Name));
                        }
                        sw.WriteLine("");

                        // Write data
                        foreach (cValueChainResults.eVariableType v in Enum.GetValues(typeof(cValueChainResults.eVariableType)))
                        {
                            sw.Write(cStringUtils.ToCSVField(v.ToString()));
                            foreach (cUnit u in m_data.GetUnits(cUnitFactory.eUnitType.All))
                            {
                                sw.Write(",");
                                float result = 0f;
                                if (iTimeEnd == 0)
                                {
                                    result = m_results.GetTotal(v, new cUnit[] { u }, iItem, cValueChainResults.GetVariableContributionType(v));
                                }
                                else
                                {
                                    result = m_results.GetTimeStepTotal(v, iStep, new cUnit[] { u }, iItem, cValueChainResults.GetVariableContributionType(v));
                                }
                                sw.Write(cStringUtils.FormatNumber(result));
                            }
                            sw.WriteLine("");
                        }
                        sw.Flush();
                        sw.Close();

                        // vars.Add(New cVariableStatus(eStatusFlags.OK, cStringUtils.Localize(My.Resources.PROMPT_SAVERESULT_DETAIL, strFile),
                        // eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))
                    }
                }
            }
            catch (Exception ex)
            {
                // Waah!
                // Me.m_msg = New cMessage(cStringUtils.Localize(My.Resources.PROMPT_SAVERESULTS_FAILED, pout, ex.Message),
                // eMessageType.DataExport, eCoreComponentType.Ecotracer, eMessageImportance.Warning)
                return false;
            }

            // ' Already has save result message?
            // If (Me.m_msg Is Nothing) Then
            // ' #No: create one
            // Me.m_msg = New cMessage(cStringUtils.Localize(My.Resources.PROMPT_SAVERESULTS_SUCCESS, pout),
            // eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            // ' Set hyperlink
            // Me.m_msg.Hyperlink = pout
            // End If

            // For i As Integer = 0 To vars.Count - 1
            // Me.m_msg.AddVariable(vars(i))
            // Next

            return true;

        }

    }
}