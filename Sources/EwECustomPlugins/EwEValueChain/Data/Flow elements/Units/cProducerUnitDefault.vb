#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' This class holds defaults for <see cref="cProducerUnit">producer units</see>
''' in the Ecost model. Defaults are used as blueprints for spawning their base 
''' class objects.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)), _
    Serializable()> _
Public Class cProducerUnitDefault
    Inherits cProducerUnit

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

    <Browsable(False)> _
    Public Overrides Property EcopathFleetID() As Integer
        Get
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property

    <Browsable(False)> _
    Public Overrides Property EcopathGroupID() As Integer
        Get
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property

    <Browsable(False)> _
    Public Overrides Property Group() As EwECore.cEcoPathGroupInput
        Get
            Return Nothing
        End Get
        Friend Set(ByVal value As EwECore.cEcoPathGroupInput)
        End Set
    End Property

    <Browsable(False)> _
    Public Overrides Property Fleet() As EwECore.cFleetInput
        Get
            Return Nothing
        End Get
        Friend Set(ByVal value As EwECore.cFleetInput)
        End Set
    End Property

End Class
