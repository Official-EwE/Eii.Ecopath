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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwECore
Imports EwEPlugin
Imports ScientificInterfaceShared.Controls

''' <summary>
''' Base code that can be used as a template to create a new plugin that integrates
''' with the EwE user interface.
''' </summary>
''' <remarks>
''' <para>This plug-in integrates with:</para>
''' <list type="bullet">
''' <item><description>the EwE6 navigation tree,</description>></item>
''' <item><description>the EwE6 main menu.</description>></item>
''' </list>
''' <para>In order to run and test this plugin it must be integrated within the EwE6 scientific interface. 
''' To achieve this, add this project to the EwE6 solution, and reference this project from within the 
''' ScientificInterface. This ensures that your plug-in will be built with EwE6, and will be loaded by the 
''' EwE6 plug-in manager when you run EwE6.</para>
''' </remarks>
''' 
Public Class BaseUserInterfacePlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin

#Region " Private variables "

    Private m_uic As cUIContext = Nothing
    Private m_form As frmEwEPlugin = Nothing

#End Region ' Private variables

#Region " User Interface plug-in implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User Interfaces require a UIContext, which provides not only access to
    ''' a running core, but also to a styleguide, command handler, and other
    ''' aspects that binds user interface elements in the EwE 6 application. 
    ''' </summary>
    ''' <param name="uic">The <see cref="cUIContext"/> to connect to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub UIContext(uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext

        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception
            Me.m_uic = Nothing
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what text to display in controls that provide access to 
    ''' this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "BaseUserInterfacePlugin"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what image to show for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            ' Use an image from the pool of shared resources
            Return ScientificInterfaceShared.My.Resources.fish
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what text to display when the user hovers the mouse cursor
    ''' over a user interface element for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            ' Show the description as a tooltip text
            Return Me.Description
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what text to display for describing the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "An example of nesting a plug-in in the EwE6 interface"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide EwE6 with a method to execute when a user interface control for 
    ''' this plug-in is clicked by the user.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef form As Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        Dim bHasInterface As Boolean = False

        ' Initialized ok?
        If m_uic IsNot Nothing Then

            ' Test if form still exists. This is a two-step test: the interface needs to be defined, and has not been closed previously.
            If Me.m_form IsNot Nothing Then
                If Not Me.m_form.IsDisposed Then
                    bHasInterface = True
                End If
            End If

            ' Create the interface if needed
            If Not bHasInterface Then

                ' Create the EwE form-derived user interface for this plug-in
                Me.m_form = New frmEwEPlugin()
                ' Pass on the UI context to the form
                Me.m_form.UIContext = m_uic

                ' This is really not necessary but it looks nice :)
                Me.m_form.Icon = Icon.FromHandle(ScientificInterfaceShared.My.Resources.fish.GetHicon)
            End If

            ' Activate the interface
            Me.m_form.Show()

            ' Pass a reference to the new interface back to whomever invoked us
            form = Me.m_form

            ' Just to show what can be done: test where this function was invoked from
            If TypeOf sender Is System.Windows.Forms.TreeNode Then
                ' Plug-in was invoked from the EwE6 navigation panel
            ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
                ' Plug-in was invoked from the EwE6 main menu
            End If
        Else
            Debug.Assert(False, "Plugin was not initialized properly.")
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 where to place an item in its main menu.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            ' For example, a plug-in menu item should be placed in the main the 'Tools' menu. 
            Return "MenuTools"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 when during application execution this plug-in should be accessible 
    ''' to users.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            ' This plug-in is available at any time during EwE execution
            Return EwEUtils.Core.eCoreExecutionState.Idle
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 where to place an item in its navigation tree.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NavigationTreeItemLocation() As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            ' As an example, place a navigation tree item under the main 'tools' node.
            Return "ndTools"
        End Get
    End Property

#End Region ' User Interface plug-in implementation

#Region " Generic plug-in point implementation bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the plug-in. This is called only once when the EwE6 first 
    ''' loads the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize

        ' Ignore this method; we really want the UI context

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide the internal name of the plug-in to EwE6. This name will not be 
    ''' shown to the user, but will be used to sort plug-in items in container 
    ''' user interface structures such as the EwE6 main menu and navigation tree.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "any name"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide EwE6 with author information to display for the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide EwE6 with contact information to display for the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

#End Region ' Generic plug-in point implementation bits

End Class
