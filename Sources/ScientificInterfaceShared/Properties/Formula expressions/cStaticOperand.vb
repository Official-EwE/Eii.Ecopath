'==============================================================================
'
' $Log: cStaticOperand.vb,v $
' Revision 1.2  2009/05/28 12:37:02  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.1  2009/04/02 13:19:40  jeroens
' Separated out of cFormulaExpression.vb
'
'==============================================================================

Option Strict On
Imports ScientificInterfaceShared.Style

Namespace Properties

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A numerical expression that does not change value
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cStaticOperand
        : Inherits cExpression

        ''' <summary>The constant value of this expression</summary>
        Private m_sValue As Single

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="s">The value of this expression</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal s As Single)
            Me.m_sValue = s
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
