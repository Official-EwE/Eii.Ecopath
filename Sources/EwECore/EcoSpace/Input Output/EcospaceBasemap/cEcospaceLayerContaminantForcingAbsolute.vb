' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Layer providing access to Ecospace contaminant forcing data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceLayerContaminantForcingAbsolute
    Inherits cEcospaceLayerSingle

    Public Sub New(theCore As cCore, manager As cEcospaceBasemap)
        ' Nassssty! We canot use zero-indexed layers in EwE
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerContaminantForcingAbsolute, 1)
        Me.m_dataType = eDataTypes.EcospaceLayerContaminantForcingAbs
    End Sub

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Return d(iRow, iCol, 0)
            Catch ex As Exception

            End Try
            Return cCore.NULL_VALUE
        End Get
        Set(value As Object)
            Try
                Dim d As Single(,,) = DirectCast(Me.Data, Single(,,))
                Dim s As Single = Convert.ToSingle(value)
                d(iRow, iCol, 0) = s
                Me.Invalidate()
            Catch ex As Exception

            End Try
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overriden to include the group name into this layer's name
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function DefaultName() As String
        Return My.Resources.CoreDefaults.CORE_DEFAULT_ENVIRONMENT
    End Function

End Class

