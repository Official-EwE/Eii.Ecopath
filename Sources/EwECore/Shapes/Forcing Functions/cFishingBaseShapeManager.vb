' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public MustInherit Class cFishingBaseShapeManager
    Inherits cBaseShapeManager

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)
        Me.Init()
    End Sub

    Public Overrides ReadOnly Property NPoints() As Integer
        Get
            Return Me.m_SimData.NTimes
        End Get
    End Property

    Public MustOverride Sub ResetToDefaults()

    Public MustOverride Function EcopathBaseValue(iShape As Integer) As Single

End Class

