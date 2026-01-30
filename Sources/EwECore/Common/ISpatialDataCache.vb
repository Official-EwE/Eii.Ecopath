' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing



Namespace Common

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for classes that cache converted spatial-temporal data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ISpatialDataCache

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the path to the cache root folder.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property RootFolder As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the path to a cache for a dataset.
        ''' </summary>
        ''' <param name="ds"><see cref="ISpatialDataSet"/> to obtain the cache path for.</param>
        ''' <param name="ptfTL">Top-left location (in decimal degrees lon,lat) of the bounding box of the data.</param>
        ''' <param name="ptfBR">Bottom-right location (in decimal degrees lon,lat) of the bounding box of the data.</param>
        ''' <param name="dCellSize">Cell size to obtain the cache path for.</param>
        ''' <param name="time">Time to create the file name for.</param>
        ''' <param name="strFilter">Optional filter, may be empty.</param>
        ''' <param name="strExt">File extension to create the file name for.</param>
        ''' <returns>A cache path.</returns>
        ''' -------------------------------------------------------------------
        Function GetFileName(ds As ISpatialDataSet,
                             ptfTL As PointF, ptfBR As PointF, dCellSize As Double, time As DateTime,
                             strFilter As String, strExt As String) As String
    End Interface

End Namespace

