#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace upwelling data.
''' </summary>
Public Class cEcospaceLayerUpwelling
    Inherits cEcospaceLayerSingle
    Implements ICoreMonthFilter

#Region " Private vars "

    ''' <summary>Month [1, 12] to operate on.</summary>
    Private m_iMonth As Integer = 1

#End Region ' Private vars

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerUpwelling, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerUpwelling
    End Sub

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

End Class
