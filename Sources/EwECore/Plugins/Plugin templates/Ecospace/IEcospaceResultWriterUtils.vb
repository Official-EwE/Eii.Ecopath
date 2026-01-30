' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for overriding functionality in the Ecospace Results Writers
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceResultWriterUtils
        Inherits IPlugin

        ''' <summary>
        ''' Overwrite the default file name of the Ecospace area/region averaged .csv files
        ''' </summary>
        ''' <param name="FileName">
        ''' Name of the Ecospace area/region average file. 
        ''' This is the file name only the path will be set by the Ecospace output directory. </param>
        ''' <param name="DataSourceAsObject">
        ''' cEcospaceResultsWriterDataSourceBase as Object the contains the data to be written to file. 
        ''' This is supplied here so you can tell what type of data is being written to this file.</param>
        ''' <param name="AvgType">Time step averageing of the data Annual or Ecospace timestep</param>
        ''' <returns>True if you want to new file name to be used. False otherwise.</returns>
        ''' <remarks>Overrides the cEcospaceAvgModelAreaResultsWriter.getFileName() function</remarks>
        Function ModelAreaFileName(ByRef FileName As String, DataSourceAsObject As Object, AvgType As eEcospaceResultsAverageType) As Boolean

        ''' <summary>
        ''' Overwrite the default group file name of the Ecospace map outputs.
        ''' </summary>
        ''' <param name="FileName">
        ''' New name of the file. 
        ''' This is the file name only the path will be set by the Ecospace output directory. </param>
        ''' <param name="varname">eVarNameFlags of the data</param>
        ''' <param name="iGrp">Group Index</param>
        ''' <param name="strExt">Extention of the file</param>
        ''' <param name="iModelTimeStep">Model time step of the file</param>
        ''' <returns>Return True if the new file name should be used</returns>
        ''' <remarks>Overrides the cEcospaceBaseResultsWriter.GetGroupFileName(...) function</remarks>
        Function MapGroupFileName(ByRef FileName As String, varname As eVarNameFlags, iGrp As Integer,
                                   strExt As String, iModelTimeStep As Integer) As Boolean

        ''' <summary>
        ''' Overwrite the default Fleet file name of the Ecospace map outputs.
        ''' </summary>
        ''' <param name="FileName">
        ''' New name of the file. 
        ''' This is the file name only the path will be set by the Ecospace output directory. </param>
        ''' <param name="varname">eVarNameFlags of the data</param>
        ''' <param name="iFlt">Group Index</param>
        ''' <param name="strExt">Extention of the file</param>
        ''' <param name="iModelTimeStep">Model time step of the file</param>
        ''' <returns>Return True if the new file name should be used</returns>
        ''' <remarks>Overrides the cEcospaceBaseResultsWriter.GetFleetFileName(...) function</remarks>
        Function MapFleetFileName(ByRef FileName As String, varname As eVarNameFlags, iFlt As Integer,
                                  strExt As String, iModelTimeStep As Integer) As Boolean


    End Interface

End Namespace