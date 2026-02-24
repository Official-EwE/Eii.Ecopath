' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace SpatialData

    ''' <summary>
    ''' This scalar data adapter deliberately does NOT preserve and restore layer data.
    ''' </summary>
    Public Class cSpatialScalarNonRestoringDataAdapter
        Inherits cSpatialScalarDataAdapterBase

#Region " Constructor "

        Public Sub New(core As cCore, varName As eVarNameFlags, cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

        Public Overrides Sub InitRun(bPreserveLayerData As Boolean)
            MyBase.InitRun(False)
        End Sub

#End Region

    End Class

End Namespace
