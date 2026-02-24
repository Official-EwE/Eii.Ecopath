' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Properties

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface class for defining <see cref="cProperty"/>-driven cells.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IPropertyCell

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the property in the cell
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function GetProperty() As cProperty

    End Interface

End Namespace
