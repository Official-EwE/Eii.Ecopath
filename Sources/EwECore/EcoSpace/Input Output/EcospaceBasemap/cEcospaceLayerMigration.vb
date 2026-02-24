' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Layer providing access to Ecospace migration data.
''' </summary>
Public Class cEcospaceLayerMigration
    Inherits cEcospaceLayerSingle

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer that derives its data and identity from 
    ''' a manager.
    ''' </summary>
    ''' <param name="theCore"></param>
    ''' <param name="manager"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef theCore As cCore, manager As cEcospaceBasemap, iIndex As Integer)
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerMigration, iIndex)
        Me.m_dataType = eDataTypes.EcospaceLayerMigration
        Me.m_ccSecundaryIndex = eCoreCounterTypes.nMonths
    End Sub

#End Region ' Construction

#Region " Cell interaction "

    Public Overrides Property Cell(iRow As Integer, iCol As Integer,
                                   Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            If (iIndexSec = cCore.NULL_VALUE) Then iIndexSec = Me.SecundaryIndex
            Return DirectCast(Me.Data, Single(,)(,))(Me.Index, iIndexSec)(iRow, iCol)
        End Get
        Set(value As Object)
            If (iIndexSec = cCore.NULL_VALUE) Then iIndexSec = Me.SecundaryIndex
            DirectCast(Me.Data, Single(,)(,))(Me.Index, iIndexSec)(iRow, iCol) = CSng(value)
        End Set
    End Property

#End Region ' Cell interaction

#Region " Overrides "

    Protected Overrides Function DefaultName() As String
        Return Me.m_core.EcopathGroupInputs(Me.Index).Name
    End Function

#End Region ' Overrides

End Class
