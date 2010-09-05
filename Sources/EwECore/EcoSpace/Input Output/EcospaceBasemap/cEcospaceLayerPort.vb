#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace port data.
''' </summary>
Public Class cEcospaceLayerPort
    Inherits cEcospaceLayerInteger
    Implements ICoreFleetFilter

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerPort)
        Me.m_dataType = eDataTypes.EcospaceLayerPort
    End Sub

#Region " Cell interaction "

    Private m_iFleet As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the fleet that this layer represents.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Fleet() As Integer _
        Implements ICoreFleetFilter.Fleet
        Get
            Return Me.m_iFleet
        End Get
        Set(ByVal value As Integer)
            If value <> Me.m_iFleet Then
                Me.m_iFleet = value
            End If
        End Set
    End Property

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))
            If (Me.m_iFleet = 0) Then
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    If data(iFleet, iRow, iCol) Then Return 1.0!
                Next
                Return cCore.NULL_VALUE
            Else
                Return CSng(IIf(data(Me.m_iFleet, iRow, iCol), 1.0!, 0.0!))
            End If
        End Get
        Set(ByVal value As Object)
            Dim data As Boolean(,,) = DirectCast(Me.Data, Boolean(,,))
            ' ToDo: only allow coastal cells to be set
            If (Me.m_iFleet = 0) Then
                For iFleet As Integer = 1 To Me.m_core.nFleets
                    data(iFleet, iRow, iCol) = (CSng(value) <> 0.0!)
                Next
            Else
                data(Me.m_iFleet, iRow, iCol) = (CSng(value) <> 0.0!)
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return 1.0!
        End Get
    End Property

    Public Overrides ReadOnly Property MinValue() As Single
        Get
            Return 0.0!
        End Get
    End Property

#End Region ' Cell interaction

End Class
