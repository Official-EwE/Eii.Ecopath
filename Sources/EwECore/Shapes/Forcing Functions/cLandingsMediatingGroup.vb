' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

#Region "cLandingsMediatingGroup"

''' <summary>
''' Mediation group for Price Elasticity.
''' cPriceMediatingGroup "Is A" cMediatingGroup with a fleet index that tell you what Fleet to get the Landings from
''' </summary>
''' <remarks></remarks>
Public Class cLandingsMediatingGroup
    Inherits cMediatingGroup

    Public iFleetIndex As Integer

    ''' <summary>
    ''' Build a new Mediation Group
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(iGroup As Integer, iFleet As Integer, theWeight As Single)
        MyBase.New(iGroup, theWeight)

        Me.iFleetIndex = iFleet

    End Sub

    Public Sub New()
        MyBase.New()
        Me.iFleetIndex = 0
    End Sub

End Class

#End Region

