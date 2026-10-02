' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel

Namespace ValueChainMigrator.LegacyData

    ''' ===========================================================================
    ''' <summary>
    ''' This class represents a group of distribution units in the Ecost economic model.
    ''' </summary>
    ''' ===========================================================================
    <Obsolete(), Serializable()>
    Public Class cDistributionUnit
        Inherits cEconomicUnit
        Public Sub New()
            MyBase.New()
        End Sub

        <Browsable(False)>
        Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
            Get
                Return cUnitFactory.eUnitType.Distribution
            End Get
        End Property

    End Class

End Namespace
