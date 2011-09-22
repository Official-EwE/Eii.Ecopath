#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Layer providing access to Ecospace habitat capacity data.
''' </summary>
Public Class cEcospaceLayerHabitatCapacity
    Inherits cEcospaceLayerSingle
    Implements ICoreGroupFilter

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal dt As eDataTypes, ByVal vn As eVarNameFlags)
        MyBase.New(theCore, manager, vn)
        Me.m_dataType = dt
    End Sub

#Region " Cell interaction "

    Private m_iGroup As Integer = 1

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the group that this layer represents.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Group() As Integer _
        Implements ICoreGroupFilter.Group
        Get
            Return Me.m_iGroup
        End Get
        Set(ByVal value As Integer)
            If value <> Me.m_iGroup Then
                Me.m_iGroup = value
            End If
        End Set
    End Property

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            If Me.ValidateCellPosition(iRow, iCol) Then Return data(iRow, iCol, Me.m_iGroup)
            Return 0
        End Get
        Set(ByVal value As Object)
            Dim data As Single(,,) = DirectCast(Me.Data, Single(,,))
            If Me.ValidateCellPosition(iRow, iCol) then  data(iRow, iCol, Me.m_iGroup) = CSng(value)
        End Set
    End Property

    Public Overrides ReadOnly Property MaxValue() As Single
        Get
            Return 1.0!
        End Get
    End Property

#End Region ' Cell interaction

End Class
