' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

Public MustInherit Class cResultsCollector_HCR_F
    Inherits cResultsCollector_1DArray

    Public Sub New()

    End Sub

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return -9999
        End Get
    End Property

    Public Overrides ReadOnly Property Dim_Name As String
        Get
            Return "Group"
        End Get
    End Property

    Public Overrides ReadOnly Property nElements As Integer
        Get
            Return Me.m_MSE.Core.nGroups
        End Get
    End Property

    Public Overrides ReadOnly Property ElementName(iElement As Integer) As String
        Get
            Return Me.m_MSE.Core.EcopathGroupInputs(iElement).Name
        End Get
    End Property

    Public Overrides ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, iElement As Integer, iTime As Integer) As Object
        Get
            Return cStringUtils.FormatNumber(Me.GetValue(iStrategy, iElement, iTime))
        End Get
    End Property

End Class
