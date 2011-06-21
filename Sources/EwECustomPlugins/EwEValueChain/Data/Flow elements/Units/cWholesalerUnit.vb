#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

<TypeConverter(GetType(cPropertySorter)), _
    DefaultProperty("Name"), _
    Serializable()> _
Public Class cWholesalerUnit
    Inherits cEconomicUnit

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Calculations "

    Protected Overrides Function Calculate(ByVal results As cResults, _
            ByVal sInputBiomass As Single, ByVal sInputValue As Single, _
            ByVal sOutputBiomass As Single, ByVal sOutputValue As Single, _
            ByVal iTimeStep As Integer) As Boolean

        Return MyBase.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

    End Function

#End Region ' Calculations

#Region " Properties "

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Wholesaler"
        End Get
    End Property

    <Browsable(False)> _
    Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
        Get
            Return cUnitFactory.eUnitType.Wholesaler
        End Get
    End Property

#End Region ' Properties

End Class
