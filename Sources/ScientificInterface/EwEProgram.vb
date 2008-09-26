#Region "Imports Directive"

Option Explicit On
Option Strict On

Imports System
Imports System.Windows.Forms
Imports System.Resources


#End Region

''' <summary>
''' The main entry class for the application
''' </summary>
''' <remarks>
''' Default namespace for the application is EwE6.Source.ScientificInterface. It is being set 
''' at the project setting's root namespace.
''' </remarks>
Public Class EwEProgram

#Region "Public Methods"
    ''' <summary>
    ''' The main entry point for the application
    ''' </summary>
    <System.STAThread()> _
    Public Shared Sub Main()

        Application.EnableVisualStyles()
        My.Settings.Upgrade()
        My.Settings.Reload()
        Application.Run(New AppLauncher())

    End Sub

#End Region

End Class



