' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor base class that supports manual modification of Ecospace 
    ''' layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorRegion
        Inherits cLayerEditorRaster

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorRegion))
            Me.CellValue = 1
        End Sub

#End Region ' Construction

    End Class

End Namespace ' Controls.Map.Layers
