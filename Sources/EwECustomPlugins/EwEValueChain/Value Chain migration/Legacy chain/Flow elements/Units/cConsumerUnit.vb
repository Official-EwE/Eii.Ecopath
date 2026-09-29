' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel

Namespace ValueChainMigrator.LegacyData

    ''' ===========================================================================
    ''' <summary>
    ''' This class represents a group of Consumers in the Ecost economic model.
    ''' Consumers form the end of economic flow chains.
    ''' </summary>
    ''' ===========================================================================
    <Obsolete(), Serializable()>
    Public Class cConsumerUnit
        Inherits cUnit

        Public Sub New()
            MyBase.New()
        End Sub

        <Browsable(False)>
        Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
            Get
                Return cUnitFactory.eUnitType.Consumer
            End Get
        End Property

    End Class

End Namespace
