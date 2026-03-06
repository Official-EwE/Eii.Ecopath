' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Layer providing access to Ecospace effort zone data.
''' </summary>
Public Class cEcospaceLayerEffortZone
    Inherits cEcospaceLayerInteger

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerEffortZone)
        Me.m_dataType = eDataTypes.EcospaceLayerEffortZone
    End Sub

    Protected Overrides Function DefaultName() As String
        Return My.Resources.CoreDefaults.CORE_DEFAULT_EFFORTZONE
    End Function

End Class
