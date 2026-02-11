' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style

''' <summary>
''' Layer providing access to Ecospace Primarey Production data.
''' </summary>
Public Class cEcospaceLayerRelPP
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerRelPP, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerRelPP
    End Sub

    Protected Overrides Function DefaultName() As String
        Dim vnf As New cVarnameTypeFormatter()
        Return vnf.ToString(eVarNameFlags.LayerRelPP)
    End Function

End Class
