#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' This class holds defaults for <see cref="cDistributionUnit">distribution units</see>
''' in the Ecost model. Defaults are used as blueprints for spawning their base 
''' class objects.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)), _
    Serializable()> _
Public Class cDistributionUnitDefault
    Inherits cDistributionUnit

    <Browsable(False)> _
    Public Overrides Property Name() As String
        Get
            Return "Default"
        End Get
        Set(ByVal value As String)
        End Set
    End Property

    <Browsable(False)> _
    Public Overrides ReadOnly Property Category() As String
        Get
            Return ""
        End Get
    End Property

End Class
