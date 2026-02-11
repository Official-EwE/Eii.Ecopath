' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database

''' --------------------------------------------------------------------------
''' <summary>
''' <para>Database update 6.50.0.05:</para>
''' <para>
''' <list type="bullet">
''' <item><description>Enabled monthly time series</description></item>
''' </list>
''' </para>
''' </summary>
''' --------------------------------------------------------------------------
Friend Class cDBUpdate6_50_00_05
    Inherits cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateVersion"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateVersion() As Single
        Get
            Return 6.500005!
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cDBUpdate.UpdateDescription"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property UpdateDescription() As String
        Get
            Return "Enabled monthly time series"
        End Get
    End Property

    Public Overrides Function ApplyUpdate(ByRef db As cEwEDatabase) As Boolean

        Dim bSucces As Boolean = True

        bSucces = db.Execute("ALTER TABLE EcosimTimeSeriesDataset ADD COLUMN DataInterval LONG")
        bSucces = bSucces And db.Execute("UPDATE EcosimTimeSeriesDataset SET DataInterval = " & CStr(CInt(eTSDataSetInterval.Annual)))
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimTimeSeriesDataset ADD COLUMN NumPoints LONG")
        bSucces = bSucces And db.Execute("UPDATE EcosimTimeSeriesDataset SET NumPoints = NumYears")
        bSucces = bSucces And db.Execute("ALTER TABLE EcosimTimeSeriesDataset DROP COLUMN NumYears")
        Return bSucces

    End Function
End Class
