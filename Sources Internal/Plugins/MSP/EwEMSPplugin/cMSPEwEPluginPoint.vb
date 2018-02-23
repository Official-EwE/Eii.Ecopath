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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports EwEShell
Imports EwECore
Imports EwEMSPPlugin.UI

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in point for the EwE MSP plug-in, integrating the plug-in into
''' <list type="bullet">
''' <item>The EwE6 menu</item>
''' <item>Ecospace initialization events,</item>
''' <item>Ecospace scenario lifespan events,</item>
''' <item>Ecospace run preparation events,</item>
''' <item>The EwE6 help system.</item>
''' </list>
''' </summary>
''' <seealso cref="EwEPlugin.IMenuItemPlugin" />
''' <seealso cref="EwEPlugin.IUIContextPlugin" />
''' <seealso cref="EwEPlugin.IEcospacePlugin" />
''' <seealso cref="EwEPlugin.IEcospaceInitializedPlugin" />
''' <seealso cref="EwEPlugin.IEcospaceInitRunCompletedPlugin" />
''' <seealso cref="EwEPlugin.IHelpPlugin" />
''' ---------------------------------------------------------------------------
Public Class cMSPEwEPluginPoint
    Implements IMenuItemPlugin
    Implements IUIContextPlugin
    Implements IEcospacePlugin
    Implements IEcospaceInitializedPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IHelpPlugin

#Region " Private fields "

    ''' <summary>The main UI.</summary>
    Private m_frm As frmGameDesigner = Nothing
    ''' <summary>The <see cref="cUIContext">UI Context</see> to use.</summary>
    Private m_uic As cUIContext = Nothing
    ''' <summary>The <see cref="cEwEShell">MSP shell</see> to use.</summary>
    Private m_shell As cEwEShell = Nothing
    ''' <summary>The <see cref="cEcospaceDataStructures">ecospace data structures</see> to use.</summary>
    Private m_spaceDS As cEcospaceDataStructures = Nothing

#End Region ' Private fields

#Region " Generic bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to receive the <see cref="cUIContext">user interface context</see> 
    ''' to operate onto.
    ''' </summary>
    ''' <param name="uic">The <see cref="cUIContext">user interface context</see> 
    ''' to operate onto</param>
    ''' -----------------------------------------------------------------------
    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to report the author of the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to report contact information about the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "jeroen@ecopathinternational.org"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to report the control image that the EwE6 UI should show
    ''' for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage As System.Drawing.Image Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to report the control text for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlText As String Implements IGUIPlugin.ControlText
        Get
            Return My.Resources.NODE_CONFIG
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to report the tool tip text to display this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to describe the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return My.Resources.NODE_CONFIG
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to report the core run state that needs to be met to 
    ''' enable the user interfaces for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to report the EwE menu item location to place this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in property to report the unique name of this plug-in. This name 
    ''' is meant to distinguish plug-ins, and will not be visiable to users.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "MSP2050_MSPConfig"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to report the location of the help file for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpURL As String Implements IHelpPlugin.HelpURL
        Get
            Return ".\UserGuide\EwE tools for MSP user guide.pdf"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to report the topic within the help file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpTopic As String Implements IHelpPlugin.HelpTopic
        Get
            Return Me.HelpURL
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to initialize to the EwE core. Ignored here in favor of 
    ''' plug-in point <see cref="UIContext(Object)"/>
    ''' </summary>
    ''' <param name="core">The core this plug-in is initialized for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point event handler, that will be called when the control for 
    ''' this plug-in is clicked or activated.
    ''' </summary>
    ''' <param name="sender">The control that was clicked or activated.</param>
    ''' <param name="e">Event parameters pertaining the control.</param>
    ''' <param name="frmPlugin">A reference to the user interface for this
    ''' plug-in.</param>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

#End Region ' Generic bits

#Region " Ecospace "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that is called when Ecospace has loaded a scenario, exposing
    ''' the data source that the scenario was loaded from. This point is needed to
    ''' initialize an instance of EwE shell against the newly loaded Ecospace data.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source from which
    ''' data is being loaded.</param>
    ''' -----------------------------------------------------------------------
    Public Sub LoadEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.LoadEcospaceScenario
        ' Refresh EwE shell data
        Me.m_shell = New cEwEShell(Me.m_uic.Core)
        Me.m_shell.LoadConfiguration()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that is called when Ecospace has saved a scenario, exposing
    ''' the data source that the scenario was loaded from. The plug-in point is
    ''' ignored here.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source to which
    ''' data is being saved.</param>
    ''' -----------------------------------------------------------------------
    Public Sub SaveEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.SaveEcospaceScenario
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that is called when an Ecospace scenario has been closed.
    ''' This plug-in point is used to close the plug-in UI if it exists, and to
    ''' terminate the EwE shell.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub CloseEcospaceScenario() Implements IEcospacePlugin.CloseEcospaceScenario
        If Me.HasUI Then Me.m_frm.Close()
        Me.m_frm = Nothing
        Me.m_shell = Nothing
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that is called when Ecospace has loaded a new scenario, is
    ''' initialized, and is ready to be used. The plug-in point is used to store 
    ''' a reference to the Ecospace data structures that will be used while
    ''' Ecospace is alive.
    ''' </summary>
    ''' <param name="EcospaceDatastructures">The ecospace data structures that
    ''' just received new scenario data.</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
        Me.m_spaceDS = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that is called when Ecospace is about to start running.
    ''' The plug-in point is needed to set run flags that may have been reset
    ''' by Ecospace at the end of the last execution.
    ''' </summary>
    ''' <param name="EcospaceDatastructures">The ecospace data structures.</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) Implements IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted
        If Me.HasUI Then
            Dim g As cGame = Me.m_frm.SelectedGame
            If (g IsNot Nothing) Then
                Me.m_spaceDS.bCalTrophicLevel = g.CalculateIndicators
            End If
        End If
    End Sub

#End Region ' Ecospace

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Determines whether this instance has an active user interface.
    ''' </summary>
    ''' <returns>
    ''' True if this instance has an active user interface, false otherwise.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Function HasUI() As Boolean
        If (Me.m_frm Is Nothing) Then Return False
        Return Not Me.m_frm.IsDisposed
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates and returns the main user interface for the plug-in.
    ''' </summary>
    ''' <returns>An active user interface.</returns>
    ''' -----------------------------------------------------------------------
    Private Function GetUI() As frmGameDesigner
        If Not Me.HasUI() Then
            Me.m_frm = New frmGameDesigner(Me.m_uic, Me.m_shell, Me.m_spaceDS)
        End If
        Return Me.m_frm
    End Function

#End Region ' Internals

End Class
