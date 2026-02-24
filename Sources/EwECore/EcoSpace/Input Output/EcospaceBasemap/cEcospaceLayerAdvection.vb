' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

''' <summary>
''' Layer providing access to Ecospace advection data.
''' </summary>
Public Class cEcospaceLayerAdvection
    Inherits cEcospaceLayerVelocity

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for the advection layer.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)

        MyBase.New(theCore, manager, "", eVarNameFlags.LayerAdvection)
        Me.m_dataType = eDataTypes.EcospaceLayerAdvection
        Me.m_ccSecundaryIndex = eCoreCounterTypes.nMonths
        Me.m_coreComponent = eCoreComponentType.Ecospace

    End Sub

#End Region ' Construction

#Region " Overrides "

    Protected Overrides Function DefaultName() As String
        Return cStringUtils.Localize(My.Resources.CoreDefaults.CORE_DEFAULT_ADVECTION)
    End Function

#End Region ' Overrides

End Class
