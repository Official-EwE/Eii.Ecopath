' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Layer providing access to Ecospace MPA seed data.
''' </summary>
Public Class cEcospaceLayerMPASeed
    Inherits cEcospaceLayerInteger

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerMPASeed, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerMPASeed
    End Sub

    Protected Overrides Function DefaultName() As String
        Return My.Resources.CoreDefaults.CORE_DEFAULT_MPASEED
    End Function

End Class