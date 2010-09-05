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

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer that derives its data and identity from 
    ''' a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' <param name="varName"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal theCore As cCore, _
                   ByVal manager As cEcospaceBasemap, _
                   ByVal varName As eVarNameFlags)
        MyBase.New(theCore, cCore.NULL_VALUE, manager, varName, cCore.NULL_VALUE, Nothing)
    End Sub

#End Region ' Construction

#Region " Cell interaction "

    Private m_iMonth As Integer = 1

    Public Property Month() As Integer
        Get
            Return Me.m_iMonth
        End Get
        Set(ByVal value As Integer)
            Me.m_iMonth = value
        End Set
    End Property

#End Region ' Cell interaction

#Region " Private bits "

    ''' <summary>
    ''' Get X velocity data
    ''' </summary>
    Public Overrides Property XVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            Return DirectCast(Me.Data, Single()(,,))(0)(iRow, iCol, Me.m_iMonth)
        End Get
        Set(ByVal value As Single)
            DirectCast(Me.Data, Single()(,,))(0)(iRow, iCol, Me.m_iMonth) = value
        End Set
    End Property

    ''' <summary>
    ''' Get Y velocity data
    ''' </summary>
    Public Overrides Property YVelocity(ByVal iRow As Integer, ByVal iCol As Integer) As Single
        Get
            Return DirectCast(Me.Data, Single()(,,))(1)(iRow, iCol, Me.m_iMonth)
        End Get
        Set(ByVal value As Single)
            DirectCast(Me.Data, Single()(,,))(1)(iRow, iCol, Me.m_iMonth) = value
        End Set

    End Property

#End Region ' Private bits

End Class
