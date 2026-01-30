' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modifications of layers where cells
    ''' can have a range of values.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorRange
        Inherits cLayerEditorRaster

#Region " Construction "

        Public Sub New()
            Me.New(GetType(ucLayerEditorRange))
        End Sub

        Public Sub New(typeGUI As Type)
            MyBase.New(typeGUI)
        End Sub

#End Region ' Construction

    End Class

End Namespace
