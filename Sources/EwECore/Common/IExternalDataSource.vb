' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' ===========================================================================
    ''' <summary>
    ''' Interface for implementing external data sources.
    ''' </summary>
    ''' ===========================================================================
    Public Interface IExternalDataSource

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' States whether a plug-in capable of delivering external data is available.
        ''' </summary>
        ''' <param name="runtype">The core run type to check availability for.</param>
        ''' <returns>True if available.</returns>
        ''' -----------------------------------------------------------------------
        Function IsDataAvailable(runtype As IRunType) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether data delivering plug-ins are supposed to be enabled or
        ''' disabled for a certain core run type.
        ''' </summary>
        ''' <param name="runtype">The core run type to check availability for.</param>
        ''' <returns>True if available.</returns>
        ''' -----------------------------------------------------------------------
        Property EnableData(runtype As IRunType) As Boolean

    End Interface

End Namespace ' Core
