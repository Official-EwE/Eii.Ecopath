' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style

Namespace Properties

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A numerical expression that does not change value
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cStaticOperand
        Inherits cExpression

        ''' <summary>The constant value of this expression</summary>
        Private m_sValue As Single

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="s">The value of this expression</param>
        ''' ---------------------------------------------------------------
        Public Sub New(s As Single)
            Me.m_sValue = s
        End Sub

        Protected Overrides Sub Dispose(bDisposing As Boolean)
            ' NOP
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the value of this expression
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return Me.m_sValue
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the static <see cref="cStyleGuide.eStyleFlags">style</see>
        ''' of this expression.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As cStyleGuide.eStyleFlags
            Return cStyleGuide.eStyleFlags.OK
        End Function

    End Class

End Namespace
