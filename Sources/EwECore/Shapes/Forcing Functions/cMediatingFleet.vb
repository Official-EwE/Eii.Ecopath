' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

#Region " cMediationFleet "

''' <summary>
''' Fleet and Weight of a Fleet that make up a Mediating Fleet for a Mediation function. There 
''' can be more then one cMediatingFleet for a Mediation Function
''' </summary>
''' <remarks>This defines the Fleet(s) that provide the Biomass for the X axis of a mediation function.</remarks>
Public Class cMediatingFleet

    Public iFleetIndex As Integer
    Public Weight As Single

    ''' <summary>
    ''' Build a new Mediation Fleet
    ''' </summary>
    ''' <param name="iFleet">Index to the EcoPath/EcoSim fleet.</param>
    ''' <param name="theWeight">Weight that is applied to this fleet [0-1]</param>
    ''' <remarks></remarks>
    Public Sub New(iFleet As Integer, theWeight As Single)

        Me.iFleetIndex = iFleet
        'weight does not have to one or zero it can be any value it 
        Me.Weight = theWeight

    End Sub

    Public Sub New()
        Me.iFleetIndex = 0
        Me.Weight = 0
    End Sub

    Public Overrides Function ToString() As String
        Return "Fleet Index=" & Me.iFleetIndex.ToString & " Weight=" & Me.Weight.ToString
    End Function

End Class

#End Region

