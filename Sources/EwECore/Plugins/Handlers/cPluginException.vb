' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Plugin exception
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginException
    Inherits Exception

    Private m_assembly As cPluginAssembly = Nothing

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginException.
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="exception"></param>
    ''' <param name="assembly"></param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(assembly As cPluginAssembly, strMessage As String, exception As Exception)
        MyBase.New(strMessage, exception)
        Me.m_assembly = assembly
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginException.
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="assembly"></param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(assembly As cPluginAssembly, strMessage As String)
        Me.New(assembly, strMessage, Nothing)
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginException.
    ''' </summary>
    ''' <param name="exception"></param>
    ''' <param name="assembly"></param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(assembly As cPluginAssembly, exception As Exception)
        Me.New(assembly, exception.Message)
    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Get the assembly that caused the exception.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public ReadOnly Property Assembly() As cPluginAssembly
        Get
            Return Me.m_assembly
        End Get
    End Property

End Class
