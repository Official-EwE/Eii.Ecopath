#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Data for one time series contained in an Ecosim scenario.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cGroupTimeSeries
    Inherits cTimeSeries

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByVal core As cCore, ByVal iDBID As Integer)
        MyBase.New(core, iDBID)
        Me.m_datatype = eDataTypes.GroupTimeSeries
        Me.m_coreComponent = eCoreComponentType.EcoSim
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the Group this time series applies to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GroupIndex() As Integer
        Get
            Return Me.DatPool
        End Get

        Set(ByVal iGroup As Integer)
            Me.DatPool = iGroup
        End Set
    End Property

End Class