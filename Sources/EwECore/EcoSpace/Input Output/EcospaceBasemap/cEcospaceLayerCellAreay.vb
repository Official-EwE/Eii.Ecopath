' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Layer providing access to Ecospace cell areas.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceLayerCellArea
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, My.Resources.CoreDefaults.CORE_DEFAULT_CELLAREA, eVarNameFlags.LayerCellArea, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerCellArea
    End Sub

End Class
