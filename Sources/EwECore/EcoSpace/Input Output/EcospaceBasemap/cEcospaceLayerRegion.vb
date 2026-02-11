' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Layer providing access to Ecospace region data.
''' </summary>
Public Class cEcospaceLayerRegion
    Inherits cEcospaceLayerInteger

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerRegion, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerRegion
    End Sub

    ''' <summary>
    ''' Overridden to return the max region value.
    ''' </summary>
    Public Overrides ReadOnly Property MaxValue As Single
        Get
            Return Me.m_core.nRegions
        End Get
    End Property

    Protected Overrides Function DefaultName() As String
        Return My.Resources.CoreDefaults.CORE_DEFAULT_REGION
    End Function

End Class
