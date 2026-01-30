' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.Utilities



Namespace Style

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a localized description to a method in a class. This localized
    ''' description will show up in smart controls such as <see cref="PropertyGrid"/>.
    ''' </summary>
    ''' <example>
    ''' <para>This example shows you how to use a localized <see cref="cLocalizedDescriptionAttribute"/>, where
    ''' "res_myprop_descr1" and "res_myprop_descr2" are string resources defined in the resources.</para>
    ''' <para>Note that the class can be redirected to the resources of another assembly.</para>
    ''' <code>
    ''' Class TestClass
    '''     
    '''     &gt;cLocalizedDescriptionAttribute("res_myprop1_descr")&lt; _
    '''     Public Property MyProp1 As String
    '''
    '''     &gt;cLocalizedDescriptionAttribute("res_myprop2_descr", GetType(ScientificInterfaceShared.My.Resources))&lt; _
    '''     Public Property MyProp2 As String
    '''
    ''' 
    ''' End Class
    ''' </code>
    ''' </example>
    ''' -----------------------------------------------------------------------
    Public Class cLocalizedDescriptionAttribute
        Inherits DescriptionAttribute

#Region " Private vars "

        ''' <summary>Name of the resource string to find.</summary>
        Private m_strResName As String = ""
        ''' <summary>Default string to return if no suitable resource string could be found.</summary>
        Private m_strDefault As String = ""
        ''' <summary>Assembly that contains the resource string.</summary>
        Private m_typeAssem As Type = Nothing

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="strResName">Name of the resource string to find.</param>
        ''' <param name="typeAssem">Assembly that contains the resource string.</param>
        ''' <param name="strDefault">Default string to return if no suitable resource string could be found.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(strResName As String,
                       typeAssem As Type,
                       Optional strDefault As String = "")

            MyBase.New()

            Me.m_strResName = strResName
            Me.m_typeAssem = typeAssem
            Me.m_strDefault = strDefault

            If String.IsNullOrWhiteSpace(Me.m_strDefault) Then
                Me.m_strDefault = strResName
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="strResName">Name of the resource string to find.</param>
        ''' <param name="strDefault">Default string to return if no suitable resource string could be found.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(strResName As String,
                      Optional strDefault As String = "")
            Me.New(strResName, GetType(cLocalizedDescriptionAttribute), strDefault)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the localized display name, or the default string if
        ''' a localized string could not be found.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Dim strDescr As String = cResourceUtils.LoadString(Me.m_strResName, Me.m_typeAssem)
                If Not String.IsNullOrWhiteSpace(strDescr) Then Return strDescr
                Return Me.m_strDefault
            End Get
        End Property

    End Class

End Namespace
