#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEPlugin

#End Region ' Imports

Public Class ucOptionsPluginAssemblyDetails

    Private m_pa As cPluginAssembly = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Plug-in settings interface for
    ''' showing details on a plug-in assembly.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal pa As cPluginAssembly)

        Me.InitializeComponent()

        Me.m_tbCompany.Text = pa.Company
        Me.m_tbCopyright.Text = pa.Copyright
        Me.m_tbDescription.Text = pa.Description
        Me.m_tbFile.Text = pa.Filename
        Me.m_tbVersion.Text = pa.Version

        Me.m_pa = pa


    End Sub

End Class
