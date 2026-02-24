' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Samples

Namespace DataSources

    ''' =======================================================================
    ''' <summary>
    ''' Base interface for implementing a datasource that reads and writes 
    ''' alternate input sets to an existing Ecopath model.
    ''' </summary>
    ''' =======================================================================
    Public Interface IEcopathSampleDataSource
        Inherits IEcopathDataSource

#Region " Generic "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Copies all current Ecopath data to a target datasource.
        ''' </summary>
        ''' <param name="ds">The datasource to copy data to.</param>
        ''' <returns>True if sucessful.</returns>
        ''' -------------------------------------------------------------------
        Overloads Function CopyTo(ds As IEcopathSampleDataSource) As Boolean

#End Region ' Generic

#Region " Samples "

        Function LoadSamples() As Boolean

        Function SaveEcopathSamples() As Boolean

        Function AddSample(sample As cEcopathSample, ByRef iDBID As Integer) As Boolean

        Function RemoveSample(sample As cEcopathSample) As Boolean

#End Region ' Models

    End Interface

End Namespace
