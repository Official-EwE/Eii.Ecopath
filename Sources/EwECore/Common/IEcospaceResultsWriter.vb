' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' <summary>
    ''' Interface for writing Ecospace time step results to file
    ''' </summary>
    Public Interface IEcospaceResultsWriter
        Inherits IResultsWriter

        ''' <summary>
        ''' Save time step data to file.
        ''' </summary>
        ''' <param name="SpaceTimeStepResults">cEcospaceTimestep as object containing the data to save.</param>
        Sub WriteResults(SpaceTimeStepResults As Object)

    End Interface

End Namespace
