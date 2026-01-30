' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.Utilities



<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
Public Class cWholesalerUnit
    Inherits cEconomicUnit

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Calculations "

    Protected Overrides Function Calculate(results As cResults,
            sInputBiomass As Single, sInputValue As Single,
            sOutputBiomass As Single, sOutputValue As Single,
            iTimeStep As Integer) As Boolean

        Return MyBase.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

    End Function

#End Region ' Calculations

#Region " Properties "

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Wholesaler"
        End Get
    End Property

    <Browsable(False)>
    Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
        Get
            Return cUnitFactory.eUnitType.Wholesaler
        End Get
    End Property

#End Region ' Properties

End Class
