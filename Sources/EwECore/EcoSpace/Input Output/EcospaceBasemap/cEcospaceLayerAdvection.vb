#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace advection data.
''' </summary>
Public Class cEcospaceLayerAdvection
    Inherits cEcospaceLayerVector

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer that derives its data and identity from 
    ''' a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, eVarNameFlags.LayerAdvection, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerAdvection
    End Sub

#End Region ' Construction

#Region " Private bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get X velocity data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Property XVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            If Me.ValidateCellPosition(iRow, iCol) Then
                Return DirectCast(Me.Data, Single()(,))(0)(iRow, iCol)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Single)
            If Me.ValidateCellPosition(iRow, iCol) Then
                DirectCast(Me.Data, Single()(,))(0)(iRow, iCol) = value
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
                Return DirectCast(Me.Data, Single()(,))(1)(iRow, iCol)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Single)
            If Me.ValidateCellPosition(iRow, iCol) Then
                DirectCast(Me.Data, Single()(,))(1)(iRow, iCol) = value
            End If
        End Set

    End Property

#End Region ' Private bits

End Class
