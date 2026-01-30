' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style



''' <summary>
''' Layer providing access to Ecospace relative contaminants data.
''' </summary>
Public Class cEcospaceLayerContaminantRelativeDistribution
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerContaminantRelativeDistribution, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerContaminantRelativeDistribution
    End Sub

    Protected Overrides Function DefaultName() As String
        Dim vnf As New cVarnameTypeFormatter()
        Return vnf.ToString(eVarNameFlags.LayerContaminantRelativeDistribution) ' My.Resources.CoreDefaults.CORE_DEFAULT_RELCIN
    End Function

End Class
