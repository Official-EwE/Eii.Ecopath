'==============================================================================
'
' $Log: IEcosimDatasource.vb,v $
' Revision 1.1  2008/09/26 07:30:13  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.25  2008/08/08 23:14:37  jeroens
' Added proper 'SaveAs' interfaces
'
' Revision 1.24  2008/06/06 15:55:57  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.23  2008/02/11 03:24:37  jeroens
' Datasets are separate entities now, no longer just defined by name in Time Series
'
' Revision 1.22  2008/01/31 17:06:07  jeroens
' Added interface to load one single dataset
'
' Revision 1.21  2008/01/18 01:35:46  jeroens
' Added dataset manipulation method to sim datasource interface
'
' Revision 1.20  2007/11/26 14:49:14  jeroens
' * Fixed bug 351
'
' Revision 1.19  2007/11/20 00:51:53  jeroens
' * Shapedata to copy can now be passed to AppendShape logic
'
' Revision 1.18  2007/10/30 19:22:41  jeroens
' + Added author, description to model and scenarios
'
' Revision 1.17  2007/10/29 14:35:01  jeroens
' * TS sequence properly calculated
'
' Revision 1.16  2007/10/14 17:21:39  jeroens
' * Solved compiler warnings
'
' Revision 1.15  2007/10/12 20:20:01  jeroens
' + Added LoadTS(datasets) interface
'
' Revision 1.14  2007/09/27 21:20:12  jeroens
' + Dataset name added when creating new Time Series
'
' Revision 1.13  2007/08/09 00:32:00  jeroens
' + Added Weight to AddTimeSeries
'
' Revision 1.12  2007/07/20 23:57:48  jeroens
' * New time series require firstyear, values
'
' Revision 1.11  2007/07/20 04:14:27  jeroens
' + Add Time series requires position parameter
'
' Revision 1.10  2007/07/17 16:24:24  jeroens
' + Added basis for copying across datasources
' * Changed TS support
'
' Revision 1.9  2007/07/11 07:02:03  jeroens
' * Implemented ecospace scenario load., create, delete, rename
'
' Revision 1.8  2007/06/11 02:19:53  jeroens
' * Changed ImportTimeSeries prototype
'
' Revision 1.7  2007/06/08 15:55:33  jeroens
' + Added TS modification interfaces
'
' Revision 1.6  2007/05/25 18:41:36  jeroens
' * Changed AppendEcosimScenario definition
'
' Revision 1.5  2007/05/16 17:10:52  jeroens
' + Added import time series functionality
'
' Revision 1.4  2007/03/07 18:15:31  jeroens
' + Added interfaces to query data changed state
'
' Revision 1.3  2007/01/14 21:03:14  jeroens
' Discontinued iDatasourcePlugin
'
' Revision 1.2  2006/12/08 18:41:32  jeroens
' * Fixed comment error
'
' Revision 1.1  2006/12/03 18:55:21  jeroens
' Initial version, separated from IEwEDataSource
'
'==============================================================================

Option Strict On

Imports EwEUtils.Core

Namespace DataSources

    Public Interface IEcosimDatasource
        Inherits IEcopathDataSource

#Region " Generic "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Copies all current Ecosim data to a target datasource.
        ''' </summary>
        ''' <param name="ds">The datasource to copy data to.</param>
        ''' <returns>True if sucessful.</returns>
        ''' -------------------------------------------------------------------
        Overloads Function CopyTo(ByVal ds As IEcosimDatasource) As Boolean

#End Region ' Generic

#Region " Diagnostics "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States if the datasource has unsaved changes for Ecosim.
        ''' </summary>
        ''' <returns>True if the datasource has pending changes for Ecosim.</returns>
        ''' -------------------------------------------------------------------
        Function IsEcosimModified() As Boolean

#End Region ' Diagnostics

#Region " Ecosim Scenarios "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Loads an ecosim scenario from the datasource.
        ''' </summary>
        ''' <param name="iScenarioID">Database ID of the scenario to load.</param>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>An implementing class should ensure that this load will cascade to
        ''' load all information pertaining to a scenario.</remarks>
        ''' -------------------------------------------------------------------
        Function LoadEcosimScenario(ByVal iScenarioID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the current active Ecosim scenario in the datasource under
        ''' a given database ID.
        ''' </summary>
        ''' <param name="iScenarioID">Database ID to save the current scenario to.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function SaveEcosimScenario(ByVal iScenarioID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the current active Ecosim scenario in the datasource under
        ''' a given database ID.
        ''' </summary>
        ''' <param name="iScenarioID">Database ID to save the current scenario to.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function SaveEcosimScenarioAs(ByVal strScenarioName As String, ByVal strDescription As String, _
                ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds a new and empty ecosim scenario to the datasource.
        ''' </summary>
        ''' <param name="strScenarioName">Name to assign to new scenario.</param>
        ''' <param name="strDescription">Description to assign to new scenario.</param>
        ''' <param name="strAuthor">Author to assign to the new scenario.</param>
        ''' <param name="strContact">Contact info to assign to the new scenario.</param>
        ''' <param name="iScenarioID">Database ID assigned to the new scenario.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AppendEcosimScenario(ByVal strScenarioName As String, ByVal strDescription As String, _
                ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Removes an ecosim scenario from the datasource.
        ''' </summary>
        ''' <param name="iScenarioID">Database ID of the scenario to remove.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function RemoveEcosimScenario(ByVal iScenarioID As Integer) As Boolean

#End Region ' Ecosim scenarios 

#Region " Forcing and Mediation shapes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Appends a forcing shape to the datasource.
        ''' </summary>
        ''' <param name="strShapeName">Name to assign to new shape.</param>
        ''' <param name="shapeDataType"><see cref="eDataTypes">Type of the shape</see> to add.</param>
        ''' <param name="iDBID">Database ID assigned to the new shape.</param>
        ''' <param name="asData">Shape point data.</param>
        ''' <param name="sYZero">Zero data point shape primitive was created from.</param>
        ''' <param name="sYBase">Base Y shape primitive was created from.</param>
        ''' <param name="sYend">End Y shape primitve was created from.</param>
        ''' <param name="sSteep">Steep value that shape primitive was created from.</param>
        ''' <param name="functionType">Primitive function type shape was created from.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AppendShape(ByVal strShapeName As String, ByVal shapeDataType As eDataTypes, ByRef iDBID As Integer, _
            ByVal asData As Single(), ByVal sYZero As Single, ByVal sYBase As Single, ByVal sYend As Single, ByVal sSteep As Single, ByVal functionType As eShapeFunctionType) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a forcing shape from the datasource.
        ''' </summary>
        ''' <param name="iDBID">Database ID of the shape to remove.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>Note that an implementing datasource will have to ensure the
        ''' shape is removed from the correct scenario.</remarks>
        ''' -------------------------------------------------------------------
        Function RemoveShape(ByVal iDBID As Integer) As Boolean

#End Region ' Forcing and Mediation shapes

#Region " Time Series "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load all time series for a given dataset.
        ''' </summary>
        ''' <param name="iDataset">Index of the dataset to load.</param>
        ''' <returns>True if succesful</returns>
        ''' -------------------------------------------------------------------
        Function LoadTimeSeriesDataset(ByVal iDataset As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds an time series dataset to the datasource.
        ''' </summary>
        ''' <param name="strDatasetName">Name to assign to new dataset.</param>
        ''' <param name="strDescription">Description to assign to new dataset.</param>
        ''' <param name="strAuthor">Author to assign to the new dataset.</param>
        ''' <param name="strContact">Contact info to assign to the new dataset.</param>
        ''' <param name="iFirstYear">First year of the dataset.</param>
        ''' <param name="iNumYears">Number of years in the dataset.</param>
        ''' <param name="iDatasetID">Database ID assigned to the new dataset.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AppendTimeSeriesDataset(ByVal strDatasetName As String, ByVal strDescription As String, _
                ByVal strAuthor As String, ByVal strContact As String, _
                ByVal iFirstYear As Integer, ByVal iNumYears As Integer, _
                ByRef iDatasetID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Removes all time series belonging to a specific dataset from the datasource.
        ''' </summary>
        ''' <param name="iDataset">Index of the dataset to remove.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function RemoveTimeSeriesDataset(ByVal iDataset As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Import a complete <see cref="cTimeSeriesImport">cTimeSeriesImport</see>
        ''' instance into the datasource.
        ''' </summary>
        ''' <param name="ts">The time series data to import.</param>
        ''' <param name="iDataset">Index of the dataset to add time series to.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function ImportTimeSeries(ByVal ts As cTimeSeriesImport, ByVal iDataset As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds a time series to the datasource.
        ''' </summary>
        ''' <param name="strName">Name of the new Time Series to add.</param>
        ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of the time series.</param>
        ''' <param name="asValues">Initial values to set in the TS.</param>
        ''' <param name="iDBID">Database ID assigned to the new TS.</param>
        ''' <param name="iPool"></param>
        ''' <param name="sWeight"></param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AppendTimeSeries(ByVal strName As String, _
            ByVal iPool As Integer, ByVal timeSeriesType As eTimeSeriesType, _
            ByVal sWeight As Single, ByVal asValues() As Single, _
            ByRef iDBID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Removes a time series from the datasource.
        ''' </summary>
        ''' <param name="iTimeSeriesID">Database ID of the time series to remove.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function RemoveTimeSeries(ByVal iTimeSeriesID As Integer) As Boolean

#End Region ' Time series

    End Interface

End Namespace
