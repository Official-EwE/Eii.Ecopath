'==============================================================================
'
' $Log: cExpression.vb,v $
' Revision 1.1  2009/04/02 13:19:40  jeroens
' Separated out of cFormulaExpression.vb
'
'==============================================================================

Option Strict On
Imports ScientificInterfaceShared.Style

Namespace Properties

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Base class for a cFormulaProperty formula
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public MustInherit Class cExpression

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the value of this expression
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public MustOverride Function GetValue() As Single
        Public MustOverride Function GetStyle() As StyleGuide.eStyleFlags

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Change notification event that must be fired when
        ''' the value of this expression has changed.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Delegate Sub ValueChangedEventHandler(ByVal exp As cExpression)
        Public Event OnValueChanged As ValueChangedEventHandler

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Fire the change event for this expression.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Protected Sub FireChangeNotification()
            RaiseEvent OnValueChanged(Me)
        End Sub

    End Class

End Namespace
