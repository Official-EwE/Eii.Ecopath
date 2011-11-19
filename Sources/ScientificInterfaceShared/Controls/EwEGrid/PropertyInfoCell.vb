#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Reflection
Imports System.ComponentModel
Imports SourceGrid2
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' ===========================================================================
    ''' <summary>
    ''' Cell that manages a single value in an object via a PropertyInfo instance.
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class cPropertyInfoCell
        : Inherits EwECell

#Region " Privates "

        ''' <summary>Object instance to manage the property value for.</summary>
        Private m_obj As Object = Nothing
        ''' <summary>PropertyInfo instance to manage the value for.</summary>
        Private m_pi As PropertyInfo = Nothing

#End Region ' Privates

#Region " Constructor "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="obj">The object instance to manage the property value for.</param>
        ''' <param name="pi">The PropertyInfo instance to manage the value for.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal obj As Object, ByVal pi As PropertyInfo)

            ' Set the cell value to the intial property value and type
            MyBase.New(pi.GetValue(obj, Nothing), _
                       pi.PropertyType, _
                       DirectCast(IIf(pi.CanWrite, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.NotEditable), cStyleGuide.eStyleFlags))

            ' Sanity checks
            Debug.Assert(obj IsNot Nothing)
            Debug.Assert(pi IsNot Nothing)

            ' Store refs
            Me.m_obj = obj
            Me.m_pi = pi

            Me.SuppressZero = True

            ' ToDo: respond to property changes by refreshing the cell value

        End Sub

#End Region ' Constructor

#Region " Overrides "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Set the value in the underlying cell and PropertyInfo.
        ''' </summary>
        ''' <param name="pos"></param>
        ''' <param name="objValue"></param>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub SetValue(ByVal pos As SourceGrid2.Position, ByVal objValue As Object)

            ' Update the cell value
            MyBase.SetValue(pos, objValue)

            ' Has attached object and property?
            If (Me.m_obj IsNot Nothing) And (Me.m_pi IsNot Nothing) Then

                ' #Yes: use the (hopefully adjusted) cell value to update the 
                '       property in the underlying object.

                Try
                    If (Me.m_pi.PropertyType Is GetType(Single)) Then
                        Me.m_pi.SetValue(Me.m_obj, CSng(Val(Me.Value)), Nothing)
                    ElseIf (Me.m_pi.PropertyType Is GetType(String)) Then
                        Me.m_pi.SetValue(Me.m_obj, CStr(Me.Value), Nothing)
                    ElseIf (Me.m_pi.PropertyType Is GetType(Integer)) Then
                        Me.m_pi.SetValue(Me.m_obj, CInt(Val(Me.Value)), Nothing)
                    ElseIf (Me.m_pi.PropertyType Is GetType(Double)) Then
                        Me.m_pi.SetValue(Me.m_obj, CDbl(Val(Me.Value)), Nothing)
                    ElseIf (Me.m_pi.PropertyType Is GetType(Boolean)) Then
                        Me.m_pi.SetValue(Me.m_obj, Convert.ToBoolean(Me.Value), Nothing)
                    Else
                        Debug.Assert(False, String.Format("Value type '{0}' not supported yet in PICell", Me.m_pi.PropertyType))
                    End If
                Catch ex As Exception
                    ' Kaboom
                    Debug.Assert(False, ex.Message)
                End Try

            End If
        End Sub

#End Region ' Overrides

    End Class

End Namespace
