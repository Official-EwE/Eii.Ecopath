#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Properties

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Base class for a cFormulaProperty formula
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public MustInherit Class cExpression
        Implements IDisposable

#Region " Disposal "

        ''' <summary>To detect redundant calls.</summary>
        Private m_bDisposed As Boolean = False        ' 

        Public Sub Dispose() _
            Implements IDisposable.Dispose
            If Not Me.m_bDisposed Then
                Me.Dispose(True)
                Me.m_bDisposed = True
            End If
            GC.SuppressFinalize(Me)
        End Sub

        Protected MustOverride Sub Dispose(ByVal bDisposing As Boolean)

#End Region ' Disposal

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the value of this expression.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public MustOverride Function GetValue() As Single

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the style of this expression.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public MustOverride Function GetStyle() As cStyleGuide.eStyleFlags

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Change notification event delegate.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Delegate Sub ValueChangedEventHandler(ByVal exp As cExpression)

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Change notification event that must be fired when
        ''' the value of this expression has changed.
        ''' </summary>
        ''' ---------------------------------------------------------------
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
