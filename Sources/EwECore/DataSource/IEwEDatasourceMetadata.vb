' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace DataSources

    ''' =======================================================================
    ''' <summary>
    ''' Base interface for implementing metadata functionality on to a datasource.
    ''' </summary>
    ''' =======================================================================
    Public Interface IEwEDatasourceMetadata
        Inherits IEwEDataSource

        ''' <summary>
        ''' Returns a name for a given data type and DBID.
        ''' </summary>
        ''' <param name="dt"><see cref="eDataTypes"/> to obtain a description for.</param>
        ''' <param name="iDBID">Unique ID of this datatype to obtain a description for.</param>
        ''' <returns>A textual description, or an empty string if the request could not be honoured.</returns>
        Function GetDescription(dt As eDataTypes, iDBID As Integer) As String

    End Interface

End Namespace
