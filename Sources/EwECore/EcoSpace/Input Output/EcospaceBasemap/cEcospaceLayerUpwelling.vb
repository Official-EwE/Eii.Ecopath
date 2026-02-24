' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Layer providing access to Ecospace upwelling data.
''' </summary>
Public Class cEcospaceLayerUpwelling
    Inherits cEcospaceLayerSingle

#Region " Private vars "

    ''' <summary>Month [1, 12] to operate on.</summary>
    Private m_iMonth As Integer = 1

#End Region ' Private vars

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerUpwelling, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerUpwelling
        Me.m_ccSecundaryIndex = eCoreCounterTypes.nMonths
    End Sub

#Region " Overrides "

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Dim data As Single()(,) = DirectCast(Me.Data, Single()(,))
            If (iIndexSec = cCore.NULL_VALUE) Then iIndexSec = Me.SecundaryIndex
            Return data(iIndexSec)(iRow, iCol)
        End Get
        Set(value As Object)
            Dim d As Single()(,) = DirectCast(Me.Data, Single()(,))
            Dim s As Single = Convert.ToSingle(value)
            If (iIndexSec = cCore.NULL_VALUE) Then iIndexSec = Me.SecundaryIndex
            d(iIndexSec)(iRow, iCol) = s
            Me.Invalidate()
        End Set
    End Property

    Protected Overrides Function DefaultName() As String
        Return My.Resources.CoreDefaults.CORE_DEFAULT_UPWELLING
    End Function

#End Region ' Overrides

End Class
