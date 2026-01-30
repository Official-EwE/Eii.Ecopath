' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Collections.Generic
Imports ScientificInterfaceShared.Style
Imports EwECore.Common
Imports ValueChain

Public Interface IGraphView

    Sub SetData(strGraphTitle As String,
                strXAxisLabel As String,
                strYAxisLabel As String,
                aVars() As cValueChainResults.eVariableType)

End Interface
