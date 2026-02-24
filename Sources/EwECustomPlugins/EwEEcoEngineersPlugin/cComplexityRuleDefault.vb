' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' -----------------------------------------------------------------------
''' <summary>
''' A default complexity rule for the user to select.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cComplexityRuleDefault
    Inherits cComplexityRule

#Region " Construction "

    Public Sub New(strName As String, a As Single, b As Single, c As Single)
        MyBase.New(strName, a, b, c)
    End Sub

#End Region ' Construction

#Region " Overrides "

    Public Overrides Function IsDefault() As Boolean
        Return True
    End Function

#End Region ' Overrides

End Class
