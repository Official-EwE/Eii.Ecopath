' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.SpatialData

Public Class cRegionAdapter
    Inherits cSpatialDataAdapter

    Public Sub New(core As cCore, var As eVarNameFlags, cc As eCoreCounterTypes)
        MyBase.New(core, var, cc)
    End Sub

    Protected Overrides Function SetCell(layer As cEcospaceLayer, conn As cSpatialDataConnection, iRow As Integer, iCol As Integer, sCellValueAtT As Double) As Boolean
        Return MyBase.SetCell(layer, conn, iRow, iCol, CInt(sCellValueAtT))
    End Function

End Class
