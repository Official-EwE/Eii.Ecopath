'==============================================================================
'
' $Log: IEcospaceDatasource.vb,v $
' Revision 1.1  2008/09/26 07:30:13  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.22  2008/08/09 01:29:16  jeroens
' Weight layers -> Importance layers
'
' Revision 1.21  2008/08/08 23:14:38  jeroens
' Added proper 'SaveAs' interfaces
'
' Revision 1.20  2007/12/17 14:08:38  jeroens
' * Basemap created with basic parameters
'
' Revision 1.19  2007/10/30 19:22:41  jeroens
' + Added author, description to model and scenarios
'
' Revision 1.18  2007/09/19 18:49:00  jeroens
' + Added ResizeEcospaceBasemap
'
' Revision 1.17  2007/09/15 00:22:54  jeroens
' * MPAs added with full data
'
' Revision 1.16  2007/09/10 20:58:38  jeroens
' * Changed the way habitats are maintained
'
' Revision 1.15  2007/07/17 16:24:25  jeroens
' + Added basis for copying across datasources
' * Changed TS support
'
' Revision 1.14  2007/06/15 15:00:19  jeroens
' + AppendEcospaceScenario requires description
'
' Revision 1.13  2007/03/07 18:15:31  jeroens
' + Added interfaces to query data changed state
'
' Revision 1.12  2007/02/27 14:58:15  jeroens
' * Habitats added with an index
'
' Revision 1.11  2007/02/26 18:55:51  jeroens
' * Simplified SaveEcospaceScenario interface
'
' Revision 1.10  2007/02/23 01:10:16  jeroens
' Simplifying Ecospace modification interfaces
'
' Revision 1.9  2007/02/22 16:07:35  jeroens
' - Simplifying interfaces - Scenario ID will be implied
'
' Revision 1.8  2007/01/25 12:56:21  jeroens
' + Maint. routines need scenario IDs
'
' Revision 1.7  2007/01/19 21:56:57  jeroens
' - Ecospace fleets do not require append/remove since they are synchronized with ecopath fleets
'
' Revision 1.6  2007/01/17 16:45:34  jeroens
' + Added fleet interfaces
'
' Revision 1.5  2007/01/16 17:18:30  jeroens
' * Adding more Ecospace interfaces
'
' Revision 1.4  2007/01/14 21:03:14  jeroens
' Discontinued iDatasourcePlugin
'
' Revision 1.3  2006/12/08 18:42:00  jeroens
' * Updated ecospace region interfaces
'
' Revision 1.2  2006/12/04 15:24:45  jeroens
' + Added Habitat and Region methods
'
' Revision 1.1  2006/12/04 14:36:59  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Namespace DataSources

    Public Interface IEcospaceDatasource
        Inherits IEcopathDataSource

#Region " Generic "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Copies all current Ecospace data to a target datasource.
        ''' </summary>
        ''' <param name="ds">The datasource to copy data to.</param>
        ''' <returns>True if sucessful.</returns>
        ''' -------------------------------------------------------------------
        Overloads Function CopyTo(ByVal ds As IEcospaceDatasource) As Boolean

#End Region ' Generic

#Region " Diagnostics "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States if the datasource has unsaved changes for Ecospace.
        ''' </summary>
        ''' <returns>True if the datasource has pending changes for Ecospace.</returns>
        ''' -------------------------------------------------------------------
        Function IsEcospaceModified() As Boolean

#End Region ' Diagnostics

#Region " Scenarios "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Loads an ecospace scenario from the datasource.
        ''' </summary>
        ''' <param name="iDBID">Database ID of the scenario to load.</param>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>An implementing class should ensure that this load will cascade to
        ''' load all information pertaining to a scenario.</remarks>
        ''' -------------------------------------------------------------------
        Function LoadEcospaceScenario(ByVal iDBID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Updates the active ecospace scenario under the given ID in the datasource.
        ''' </summary>
        ''' <param name="iDBID">Database ID of the scenario to update the current
        ''' scenario to. This parameter is optional; if left blank the current scenario
        ''' is saved.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function SaveEcospaceScenario(ByVal iDBID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the current active Ecospace scenario in the datasource under
        ''' a given database ID.
        ''' </summary>
        ''' <param name="iScenarioID">Database ID to save the current scenario to.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function SaveEcospaceScenarioAs(ByVal strScenarioName As String, ByVal strDescription As String, _
                ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds an ecospace scenario to the datasource.
        ''' </summary>
        ''' <param name="strScenarioName">Name to assign to new scenario.</param>
        ''' <param name="strDescription">Description to assign to new scenario.</param>
        ''' <param name="strAuthor">Author to assign to the new scenario.</param>
        ''' <param name="strContact">Contact info to assign to the new scenario.</param>
        ''' <param name="InRow">Number of rows in new basemap.</param>
        ''' <param name="InCol">Number of columns in new basemap.</param>
        ''' <param name="sOriginLat">Latitude of origin of basemap.</param>
        ''' <param name="sOriginLon">Longitude of origin of basemap.</param>
        ''' <param name="sCellSize">Cell size, in degrees. Cells are presumed square.</param>
        ''' <param name="iDBID">Database ID assigned to the new scenario.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AppendEcospaceScenario(ByVal strScenarioName As String, ByVal strDescription As String, _
            ByVal strAuthor As String, ByVal strContact As String, _
            ByVal InRow As Integer, ByVal InCol As Integer, _
            ByVal sOriginLat As Single, ByVal sOriginLon As Single, ByVal sCellSize As Single, _
            ByRef iDBID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Removes an ecospace scenario from the datasource.
        ''' </summary>
        ''' <param name="iDBID">Database ID of the scenario to remove.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function RemoveEcospaceScenario(ByVal iDBID As Integer) As Boolean

#End Region ' Scenarios 

#Region " Basemap "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Resizes the basemap for the current Ecospace scenario.
        ''' </summary>
        ''' <param name="InRow">New number of rows to assign to the basemap.</param>
        ''' <param name="InCol">New number of columns to assign to the basemap.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function ResizeEcospaceBasemap(ByVal InRow As Integer, ByVal InCol As Integer) As Boolean

#End Region ' Basemap

#Region " Habitats "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds an ecospace habitat to the datasource at a given position.
        ''' </summary>
        ''' <param name="strHabitatName">Name to assign to new habitat.</param>
        ''' <param name="iHabitatID">Database ID assigned to the new habitat.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AddEcospaceHabitat(ByVal strHabitatName As String, ByRef iHabitatID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Removes an ecospace habitat from the datasource.
        ''' </summary>
        ''' <param name="iHabitatID">Database ID of the habitat to remove.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function RemoveEcospaceHabitat(ByVal iHabitatID As Integer) As Boolean

#End Region ' Habitats

#Region " Regions "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds an ecospace region to active scenario in the datasource.
        ''' </summary>
        ''' <param name="strRegionName">Name to assign to new region.</param>
        ''' <param name="iDBID">Database ID assigned to the new region.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AppendEcospaceRegion(ByVal strRegionName As String, ByRef iDBID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Removes an ecospace region from the active scenario in the datasource.
        ''' </summary>
        ''' <param name="iDBID">Database ID of the region to remove.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function RemoveEcospaceRegion(ByVal iDBID As Integer) As Boolean

#End Region ' Regions

#Region " MPAs "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds an ecospace MPA to the active scenario in the datasource.
        ''' </summary>
        ''' <param name="iDBID">Database ID assigned to the new MPA.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AppendEcospaceMPA(ByVal strScenarioName As String, ByVal bMPAMonths() As Boolean, ByRef iDBID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Removes an ecospace MPA from the datasource.
        ''' </summary>
        ''' <param name="iDBID">Database ID of the MPA to remove.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function RemoveEcospaceMPA(ByVal iDBID As Integer) As Boolean

#End Region ' MPAs

#Region " Importance layers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds an ecospace Importance Layer to the active scenario in the
        ''' datasource.
        ''' </summary>
        ''' <param name="strName"></param>
        ''' <param name="strDescription"></param>
        ''' <param name="sWeight"></param>
        ''' <param name="iDBID">Database ID assigned to the new layer.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function AppendEcospaceImportanceLayer(ByVal strName As String, ByVal strDescription As String, ByVal sWeight As Single, ByRef iDBID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds an ecospace Importance Layer from the active scenario in the
        ''' datasource.
        ''' </summary>
        ''' <param name="iDBID">Database ID of the layer to remove.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function RemoveEcospaceImportanceLayer(ByVal iDBID As Integer) As Boolean

#End Region ' Importance layers

    End Interface

End Namespace
