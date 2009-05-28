'==============================================================================
'
' $Log: cPropertyOperand.vb,v $
' Revision 1.2  2009/05/28 12:37:02  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.1  2009/04/02 13:19:40  jeroens
' Separated out of cFormulaExpression.vb
'
'==============================================================================

Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Style

Namespace Properties

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A numerical expression that derives its value from a <see cref="cSingleProperty">cSingleProperty</see>.
    ''' </summary>
    ''' <remarks>
    ''' This expression monitors its property for value changes, and will broadcast a change if such an
    ''' event occurs.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Class cPropertyOperand
        : Inherits cExpression

        ''' <summary>The <see cref="cProperty">cProperty</see> to observe.</summary>
        Private m_prop As cProperty = Nothing

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cPropertyOperand
        ''' </summary>
        ''' <param name="prop">The <see cref="cSingleProperty">cSingleProperty</see> to observe.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal prop As cSingleProperty)
            ' Store property
            Me.m_prop = prop
            ' Start listening to property events
            AddHandler prop.PropertyChanged, AddressOf onPropertyChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cPropertyOperand
        ''' </summary>
        ''' <param name="prop">The <see cref="cBooleanProperty">cBooleanProperty</see> to observe.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal prop As cBooleanProperty)
            ' Store property
            Me.m_prop = prop
            ' Start listening to property events
            AddHandler prop.PropertyChanged, AddressOf onPropertyChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Destructor
        ''' </summary>
        ''' ---------------------------------------------------------------
        Protected Overrides Sub Finalize()
            ' Stop listening to property events
            RemoveHandler Me.m_prop.PropertyChanged, AddressOf onPropertyChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the value of the <see cref="cSingleProperty">cSingleProperty</see>.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return CSng(Me.m_prop.GetValue())
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="cStyleGuide.eStyleFlags">style</see>
        ''' of the <see cref="cSingleProperty">cSingleProperty</see>.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As cStyleGuide.eStyleFlags
            Return Me.m_prop.GetStyle()
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Event handler; filters property change events for value changes.
        ''' </summary>
        ''' <param name="prop">The property that changed.</param>
        ''' <param name="changeFlag">Information on what changed.</param>
        ''' ---------------------------------------------------------------
        Public Sub onPropertyChanged(ByVal prop As cProperty, ByVal changeFlag As cProperty.eChangeFlags)
            ' Is this a value or status change?
            If (changeFlag And (cProperty.eChangeFlags.Value Or cProperty.eChangeFlags.CoreStatus)) <> 0 Then
                ' #Yes: that's for us. Fire a change.
                Me.FireChangeNotification()
            End If
        End Sub

    End Class

End Namespace
