' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
'    Scottish Association for Marine Science, Oban, Scotland
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports EwECore
Imports EwECore.FitToTimeSeries
Imports System.Windows.Forms


#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface ISFPIteration

    ''' -----------------------------------------------------------------------
    ''' <summary>Enumerator defining possible base search modes.</summary>
    ''' -----------------------------------------------------------------------
    Enum eBaseSearchMode As Integer
        Baseline = 0
        Fishing = 1
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>Enumerator defining possible vulnerability search values.</summary>
    ''' -----------------------------------------------------------------------
    Enum eVulSearchMode As Integer
        ''' <summary>Search by predator.</summary>
        Predator = 0
        ''' <summary>Search by predator and prey.</summary>
        PredPrey = 1
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>Enumerator defining possible iteration run state values.</summary>
    ''' -----------------------------------------------------------------------
    Enum eRunState As Integer
        ''' <summary>Iteration has not ran yet.</summary>
        Idle = 0
        ''' <summary>Iteration is scheduled to run.</summary>
        Pending
        ''' <summary>Iteration starting.</summary>
        Initializing
        ''' <summary>Iteration running.</summary>
        Running
        ''' <summary>Iteration stopping.</summary>
        Stopping
        ''' <summary>Iteration ran successfully.</summary>
        Completed
        ''' <summary>Iteration encountered an error while running.</summary>
        [Error]
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the iteration
    ''' </summary>
    ''' <param name="core">The core to initialize to.</param>
    ''' <param name="params">The <see cref="cSFPParameters"/> instance to initialize to.</param>
    ''' -----------------------------------------------------------------------
    Sub Init(core As cCore, params As cSFPParameters)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the parameter configuration to Ecosim, eg make all the input parameter 
    ''' tweaks to Ecosim, Fit to TS, vunerability searches etc but do not run yet
    ''' </summary>
    ''' <returns>True if load successful</returns>
    ''' -----------------------------------------------------------------------
    Function Load(core As cCore) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run the Iteration
    ''' </summary>
    ''' <returns>True if run successful</returns>
    ''' -----------------------------------------------------------------------
    Function Run(core As cCore) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the results of the iteration
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' -----------------------------------------------------------------------
    Function SaveResults(core As cCore) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cSFPParameters"/> that the iteration can use.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Parameters As cSFPParameters

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the hypothesis / iteration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property Name As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the iteration type to be Baseline = true or Fishing = false
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property BaseSearchMode As eBaseSearchMode

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the K value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property K As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the EstimatedV value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property EstimatedV As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of spline points.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property SplinePoints As Integer

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the computed Sum of Squares.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property SS As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    '''  Get the computed Sum of Squares per <see cref="cTimeSeries.Index">time series</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property TimeSeriesSS As Single()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the computed Akaike Information Criterion.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property AIC As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the computed Akaike Information Criterion with a correction for finite sample sizes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property AICc As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set if this iteration is allowed to run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Enabled As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eRunState">run state</see> of an iteration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property RunState As eRunState

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the messages to accompany the <see cref="RunState"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property RunStateMessages As String()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the time it took to complete an iteration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Elapsed As TimeSpan

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the completion date.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Completed As Date

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether an iteration fitted best.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property IsBestFit As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the data for the anomaly shape for an iteration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Function AnomalyShape() As Single()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the fitted vulnerabilities for an iteration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Function Vulnerabilities() As Single(,)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply computed anomaly shape data and vulnerabilities to EwE.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Function Apply(core As cCore) As Boolean

End Interface

