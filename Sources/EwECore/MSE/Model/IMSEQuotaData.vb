' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSE

    ''' <summary>
    ''' Minimal data contract required by <see cref="cMSEQuotaCalculator"/>. Implemented by <see cref="cMSEDataStructures"/>,
    ''' but can be implemented independently to use the quota calculator without the full core data structures.
    ''' </summary>
    ''' <remarks>All arrays are 1-based (index 0 unused) following the EwE engine conventions.</remarks>
    Public Interface IMSEQuotaData

        ''' <summary>Total number of groups.</summary>
        ReadOnly Property NGroups() As Integer

        ''' <summary>Number of living groups.</summary>
        ReadOnly Property nLiving() As Integer

        ''' <summary>Number of fishing fleets.</summary>
        ReadOnly Property nFleets() As Integer

        ''' <summary>Total allowable catch by group.</summary>
        Property TAC() As Single()

        ''' <summary>Fixed escapement by group.</summary>
        Property FixedEscapement() As Single()

        ''' <summary>Fixed fishing mortality by group.</summary>
        Property FixedF() As Single()

        ''' <summary>Max fishing mortality by group.</summary>
        Property Fopt() As Single()

        ''' <summary>Minimum fishing mortality by group. Only set to non-zero by the batch manager.</summary>
        Property Fmin() As Single()

        ''' <summary>Biomass of group when fishing mortality is at Fopt(igroup) (max mortality).</summary>
        Property Bbase() As Single()

        ''' <summary>Biomass of group when fishing mortality is at zero or Fmin(igroup).</summary>
        Property Blim() As Single()

        ''' <summary>Biomass estimated for the current year by the stock assessment model.</summary>
        Property Bestimate() As Single()

        ''' <summary>Biomass coefficient of variation by group.</summary>
        Property CVbiomEst() As Single()

        ''' <summary>Target fishing mortality by group.</summary>
        Property FTarget() As Single()

        ''' <summary>Percentage of total catch by a fleet on a group (fleet, group). Sums to one across fleets.</summary>
        Property Quotashare() As Single(,)

        ''' <summary>Quota for the current year by (fleet, group), updated at the start of a year.</summary>
        Property QuotaTime() As Single(,)

    End Interface

End Namespace
