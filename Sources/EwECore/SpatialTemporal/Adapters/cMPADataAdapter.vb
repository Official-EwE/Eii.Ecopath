' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data Adapter specific to MPA layers.
    ''' </summary>
    ''' <remarks>
    ''' Needed to decide what coverage ratio closes a cell for fishing.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cMPADataAdapter
        Inherits cSpatialDataAdapter

        ''' <summary>The threshold that determines when a cell is closed for fishing.</summary>
        ''' <remarks>This parameter must be configurable. Perhaps via Ecospace scenario parameters?</remarks>
        Private Shared cTHRESHOLD As Single = 0.333!

        Public Sub New(core As cCore, varName As eVarNameFlags, cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

        Protected Overrides Function SetCell(layer As cEcospaceLayer, conn As cSpatialDataConnection, iRow As Integer, iCol As Integer, sCellValueAtT As Double) As Boolean
            Return MyBase.SetCell(layer, conn, iRow, iCol, If(sCellValueAtT >= cTHRESHOLD, 1, 0))
        End Function

    End Class

End Namespace
