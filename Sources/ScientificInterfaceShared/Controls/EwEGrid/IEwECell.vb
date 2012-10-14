Imports ScientificInterfaceShared.Style
Imports EwECore
Imports ScientificInterfaceShared.Properties

Namespace Controls.EwEGrid

    Public Interface IEwECell
        Inherits IUIElement

        ReadOnly Property Core As cCore
        ReadOnly Property PropertyManager As cPropertyManager
        ReadOnly Property StyleGuide As cStyleGuide

        Property Style() As cStyleGuide.eStyleFlags

    End Interface

End Namespace
