#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcospaceLayerSail
    Inherits cEcospaceLayerSingleNxM
    Implements IEcospaceLayerFleet

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerSail)
        Me.m_dataType = eDataTypes.EcospaceLayerSail
    End Sub

#Region " Cell interaction "

    Private m_iFleet As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the fleet that this layer represents.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Fleet() As Integer _
        Implements IEcospaceLayerFleet.Fleet
        Get
            Return Me.m_iFleet
        End Get
        Set(ByVal value As Integer)
            If value <> Me.m_iFleet Then
                Me.m_iFleet = value
            End If
        End Set
    End Property

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            Return data(Me.m_iFleet, iRow, iCol)
        End Get
        Set(ByVal value As Single)
            ' Cannot set
        End Set
    End Property

#End Region ' Cell interaction

End Class
