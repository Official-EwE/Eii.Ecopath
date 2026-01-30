' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Base class for data source objects used by the <see cref="cEcospaceRegionAvgResultsWriter">cEcospaceAvgModelAreaResultsWriter</see>
''' to write averaged Ecospace results to a csv file. 
''' </summary>
''' <remarks></remarks>
Public MustInherit Class cEcospaceResultsWriterDataSourceBase
    Protected m_core As cCore
    Protected m_spaceData As cEcospaceDataStructures

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        Me.m_core = Core
        Me.m_spaceData = EcospaceData
    End Sub

    ''' <summary>
    ''' Number of results in the data source. This can be ngroups, nfleets, ngroups * nfleets depending on the data.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    MustOverride ReadOnly Property nResults As Integer

    ''' <summary>
    ''' File identifier use to build the file name
    ''' </summary>
    MustOverride ReadOnly Property FilenameIdentifier As String

    ''' <summary>
    ''' Description of the data used in the header of the file
    ''' </summary>
    MustOverride ReadOnly Property DataDescriptor As String

    ''' <summary>
    ''' Description of the area that is covered by the data. This can be the total area or a region
    ''' </summary>
    MustOverride ReadOnly Property AreaDescriptor As String

    ''' <summary>
    ''' Number of water cells in the area
    ''' </summary>
    ''' <value></value>
    MustOverride ReadOnly Property nWaterCells As Integer

    ''' <summary>
    ''' Init the data source
    ''' </summary>
    MustOverride Sub Init(Optional OptionalIndex As Integer = 0)

    ''' <summary>
    ''' Return the result for this index and time step
    ''' </summary>
    ''' <param name="OneBasedIndex">One based index of the result to return</param>
    ''' <param name="TimeIndex">One based time step of the result</param>
    MustOverride Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single

    ''' <summary>
    ''' Name of the result field. This can be a group name, fleet name, or a combo of both
    ''' </summary>
    MustOverride Function FieldName(OneBasedIndex As Integer) As String

    ''' <summary>
    ''' Index of the Region for this datasource
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    MustOverride ReadOnly Property AreaIndex As Integer

End Class




