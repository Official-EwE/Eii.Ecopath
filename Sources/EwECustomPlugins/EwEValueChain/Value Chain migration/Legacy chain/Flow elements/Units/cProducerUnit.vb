' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel

Namespace ValueChainMigrator.LegacyData

    ''' <summary>
    ''' 
    ''' </summary>
    <Obsolete(), Serializable()>
    Public Class cProducerUnit
        Inherits cEconomicUnit

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property ObserverCost() As Single

        Public Property ObserverRate() As Single

        Public Property TicketProducts() As Single

        Public Property EcopathFleetID() As Integer
            Get
                If (String.IsNullOrWhiteSpace(Me.GearCode)) Then Return 0
                Return CInt(Me.GearCode)
            End Get
            Set(value As Integer)
                Me.GearCode = CStr(value)
            End Set
        End Property

        Public Overridable Property GearCode() As String


        <Browsable(False)>
        Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
            Get
                Return cUnitFactory.eUnitType.Producer
            End Get
        End Property

    End Class

End Namespace
