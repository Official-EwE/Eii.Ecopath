#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Data for one time series contained in an Ecosim scenario.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cFleetTimeSeries
    Inherits cTimeSeries

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByVal core As cCore, ByVal iDBID As Integer)
        MyBase.New(core, iDBID)
        Me.m_datatype = eDataTypes.FleetTimeSeries
        Me.m_coreComponent = eCoreComponentType.EcoSim
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the fleet this time series applies to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FleetIndex() As Integer
        Get
            Return Me.DatPool
        End Get
        Set(ByVal iFleet As Integer)
            Me.DatPool = iFleet
        End Set
    End Property

End Class
