' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database



''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.70.0.11:</para>
''' <para>
''' Catchabilities now driven through time series.
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_70_00_11
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.700011!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Catchabilities now driven through time series"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' At some pont, the on-board EwE database templates received an erroneous
    ''' index on a value column. This update removes the index if it exists.
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns>Always true</returns>
    ''' <remarks>This update is re-issued because the index error returned in the
    ''' on-board database templates, thus re-instating the error. Good lord.</remarks>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Try
            db.Execute("DROP TABLE EcosimScenarioFleetGroupCatchability")
        Catch ex As Exception
            ' Caught because of bad update numbering earlier
        End Try
        Return True

    End Function

End Class
