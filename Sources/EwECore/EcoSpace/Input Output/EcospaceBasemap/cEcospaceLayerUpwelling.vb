' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'





 

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
