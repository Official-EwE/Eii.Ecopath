#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace vector data.
''' </summary>
Public Class cEcospaceLayerWind
    Inherits cEcospaceLayerVector
    Implements ICoreMonthFilter

#Region " Private vars "

    ''' <summary>Month [1, 12] to operate on.</summary>
    Private m_iMonth As Integer = 1

#End Region ' Private vars

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for the wind layer.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, eVarNameFlags.LayerWind, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerWind
    End Sub

#End Region ' Construction

#Region " Filter "

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="ICoreMonthFilter.Month"/>
    ''' -----------------------------------------------------------------------
    Public Property Month() As Integer _
        Implements EwEUtils.Core.ICoreMonthFilter.Month
        Get
            Return Me.m_iMonth
        End Get
        Set(ByVal value As Integer)
            value = Math.Max(1, Math.Min(cCore.N_MONTHS, value))
            If (value <> Me.m_iMonth) Then
                Me.m_iMonth = value
                Me.Invalidate()
            End If
        End Set
    End Property

#End Region ' Filter

#Region " Private bits "


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get X velocity data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Property XVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            If Me.ValidateCellPosition(iRow, iCol) Then
                Return DirectCast(Me.Data, Single()(,,))(0)(iRow, iCol, Me.m_iMonth)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Single)
            If Me.ValidateCellPosition(iRow, iCol) Then
                DirectCast(Me.Data, Single()(,,))(0)(iRow, iCol, Me.m_iMonth) = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get Y velocity data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Property YVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            If Me.ValidateCellPosition(iRow, iCol) Then
                Return DirectCast(Me.Data, Single()(,,))(1)(iRow, iCol, Me.m_iMonth)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Single)
            If Me.ValidateCellPosition(iRow, iCol) Then
                DirectCast(Me.Data, Single()(,,))(1)(iRow, iCol, Me.m_iMonth) = value
            End If
        End Set

    End Property

#End Region ' Private bits

End Class
