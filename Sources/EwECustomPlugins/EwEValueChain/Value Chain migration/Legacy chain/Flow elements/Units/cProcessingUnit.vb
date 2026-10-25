' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel

Namespace ValueChainMigrator.LegacyData

    <Obsolete(), Serializable()>
    Public Class cProcessingUnit
        Inherits cEconomicUnit

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property AgriculturalProducts() As Single

        Public Property AgriculturalInput() As Single

        <Browsable(False)>
        Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
            Get
                Return cUnitFactory.eUnitType.Processing
            End Get
        End Property

    End Class

End Namespace
