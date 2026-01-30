' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Style
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources



Public Class cUnitHeaderFormatter

    Private m_uic As cUIContext

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic
    End Sub

    Public Function Format(var As eVarNameFlags) As String

        Dim fmt As New cVarnameTypeFormatter()
        Dim units As New cUnits(Me.m_uic.Core)

        Dim md As cVariableMetaData = cVariableMetaData.Get(var)
        Dim s1 As String = fmt.ToString(var)
        Dim s2 As String = units.ToString(md)

        If (Not String.IsNullOrWhiteSpace(s2)) Then
            Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, s1, s2)
        End If
        Return s1

    End Function

End Class
