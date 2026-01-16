' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




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
        Public Sub New(prop As cSingleProperty)
            ' Store property
            Me.m_prop = prop
            ' Start listening to property events
            AddHandler prop.PropertyChanged, AddressOf Me.onPropertyChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cPropertyOperand
        ''' </summary>
        ''' <param name="prop">The <see cref="cBooleanProperty">cBooleanProperty</see> to observe.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(prop As cBooleanProperty)
            ' Store property
            Me.m_prop = prop
            ' Start listening to property events
            AddHandler prop.PropertyChanged, AddressOf Me.onPropertyChanged
        End Sub

        Protected Overrides Sub Dispose(bDisposing As Boolean)
            ' Stop listening to property events
            RemoveHandler Me.m_prop.PropertyChanged, AddressOf Me.onPropertyChanged
            Me.m_prop = Nothing
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
        Public Sub onPropertyChanged(prop As cProperty, changeFlag As cProperty.eChangeFlags)
            ' Is this a value or status change?
            If (changeFlag And (cProperty.eChangeFlags.Value Or cProperty.eChangeFlags.CoreStatus)) <> 0 Then
                ' #Yes: that's for us. Fire a change.
                Me.FireChangeNotification()
            End If
        End Sub

    End Class

End Namespace
