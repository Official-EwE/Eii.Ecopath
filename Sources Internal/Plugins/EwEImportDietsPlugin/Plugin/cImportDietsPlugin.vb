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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls


#End Region

''' <summary>
''' Base code that can be used as a template to create a new plug-in.
''' </summary>
''' <remarks>
''' <para>This plugin responds to:</para>
''' <list type="bullet">
''' <item><description>loading a model,</description>></item>
''' <item><description>saving a model,</description>></item>
''' <item><description>closing a model,</description>></item>
''' <item><description>initialization of the Core,</description>></item>
''' <item><description>initialization of Ecopath,</description>></item>
''' <item><description>initialization of Ecosim,</description>></item>
''' <item><description>initialization of Ecospace.</description>></item>
''' </list>
''' <para>In order to run and test this plugin it must be integrated within the EwE6 scientific interface. 
''' To achieve this, add this project to the EwE6 solution, and reference this project from within the 
''' ScientificInterface. This ensures that your plug-in will be built with EwE6, and will be loaded by the 
''' EwE6 plug-in manager when you run EwE6.</para>
''' </remarks>
''' 


Public Class cImportDietsPlugin
    Implements EwEPlugin.IPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcopathRunInitializedPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcospaceInitializedPlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin

    ' ToDo Add your own EwEPlugin interface implementations here
    ' With the cursor at the end of the new Implements line press the enter key
    ' and one or more empty place holder methods will be added to the bottom of the code

#Region " Local variables"

    ''' <summary>The core that this plug-in can use</summary>
    Private m_core As cCore

    Private m_EcoPath As cEcoPathModel
    Private m_EcoSim As cEcoSimModel
    Private m_EcoSpace As cEcoSpace
    Private m_EcoPathData As cEcopathDataStructures
    Private m_EcoSimData As cEcosimDatastructures
    Private m_EcoSpaceData As cEcospaceDataStructures

    Private m_uic As cUIContext = Nothing
    ' Private m_form As frmEwEPlugin = Nothing

#End Region

#Region "Public Methods"

    Public Sub DoSomething(ByVal Value As Single)

        MsgBox("Hi from DoSomething(). Your value = " + Value.ToString, MsgBoxStyle.Information)
        System.Console.WriteLine(Value.ToString)

    End Sub

    Public Sub OpenModel(ByVal filename As String)
        Me.m_core.LoadModel(filename)
    End Sub

#End Region

#Region "Ecopath, Ecosim and Ecospace events"

    ''' <summary>
    ''' Every plug-in is told to initialize to the EwE core as soon as it is loaded. 
    ''' Typically, plug-ins use this opportunity to store a reference to the core
    ''' for later use.
    ''' </summary>
    ''' <param name="CoreAsObject">The core, casted to a generic object</param>
    Public Sub Initialize(CoreAsObject As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            m_core = DirectCast(CoreAsObject, cCore)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".Initialize() Exception " + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Plug-in point that is called when the core has initialized its models
    ''' Ecopath, Ecosim and Ecospace. This is the only opportunity for plug-ins to grab 
    ''' references to these models.
    ''' </summary>
    ''' <param name="EcopathAsObject"></param>
    ''' <param name="EcoSimAsObject"></param>
    ''' <param name="EcoSpaceAsObject"></param>
    Public Sub CoreInitialized(ByRef EcopathAsObject As Object, ByRef EcoSimAsObject As Object, ByRef EcoSpaceAsObject As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        Try

            m_EcoPath = TryCast(EcopathAsObject, cEcoPathModel)
            m_EcoSim = TryCast(EcoSimAsObject, cEcoSimModel)
            m_EcoSpace = TryCast(EcoSpaceAsObject, cEcoSpace)

            Debug.Assert((m_EcoPath IsNot Nothing) And (m_EcoSim IsNot Nothing) And (m_EcoSpace IsNot Nothing),
                         Me.ToString + ".CoreInitialized() Failed to initialize data.")

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".CoreInitialized() Exception " + ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' An Ecopath model has loaded.
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    Public Function LoadModel(dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.LoadModel
        Try

            'Cast the datasource 
            Dim ModelDataBase As EwECore.DataSources.cDBDataSource
            ModelDataBase = DirectCast(dataSource, EwECore.DataSources.cDBDataSource)

            System.Console.WriteLine(Me.ToString + ".LoadModel() " + ModelDataBase.FileName)

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".LoadModel() Exception " + ex.Message)
        End Try

        Return True

    End Function

    ''' <summary>
    ''' An Ecopath model has been saved.
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    Public Function SaveModel(dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.SaveModel
        System.Console.WriteLine(Me.ToString + ".SaveModel()")

        Return True
    End Function

    ''' <summary>
    ''' An Ecopath model has been closed.
    ''' </summary>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    Public Function CloseModel() As Boolean Implements EwEPlugin.IEcopathPlugin.CloseModel
        System.Console.WriteLine(Me.ToString + ".CloseModel()")

        Try
            'A user has closed the database
            'Clear out the old data so that we are 
            'not holding on to data that belongs to a closed model
            Me.m_EcoPath = Nothing
            Me.m_EcoPathData = Nothing
            Me.m_EcoSim = Nothing
            Me.m_EcoSimData = Nothing
            Me.m_EcoSpace = Nothing
            Me.m_EcoSpaceData = Nothing
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".CloseModel() Exception " + ex.Message)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' An Ecopath model is about to run.
    ''' </summary>
    ''' <param name="EcopathDataAsObject"></param>
    ''' <param name="TaxonDataAsObject"></param>
    ''' <param name="StanzaDataAsObject"></param>
    Public Sub EcopathRunInitialized(EcopathDataAsObject As Object, TaxonDataAsObject As Object, StanzaDataAsObject As Object) Implements EwEPlugin.IEcopathRunInitializedPlugin.EcopathRunInitialized

        Me.m_EcoPathData = TryCast(EcopathDataAsObject, cEcopathDataStructures)
        Debug.Assert(Me.m_EcoPathData IsNot Nothing, Me.ToString + ".EcopathRunInitialized() Failed to get EcopathDataStructures.")

    End Sub

    Public Sub EcosimInitialized(EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        System.Console.WriteLine(Me.ToString + ".EcosimInitialized()")

        Me.m_EcoSimData = TryCast(EcosimDatastructures, cEcosimDatastructures)
        Debug.Assert(Me.m_EcoSimData IsNot Nothing, Me.ToString + ".EcosimInitialized() Failed to get EcosimDataStructures.")

    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements EwEPlugin.IEcospaceInitializedPlugin.EcospaceInitialized
        System.Console.WriteLine(Me.ToString + ".EcospaceInitialized()")

        Me.m_EcoSpaceData = TryCast(EcospaceDatastructures, cEcospaceDataStructures)
        Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcospaceInitialized() Failed to get EcosimDataStructures.")
    End Sub

#End Region

#Region "Core, Ecopath, Ecosim and Ecospace Datastructures"

    Public ReadOnly Property Core As cCore
        Get
            Debug.Assert(Me.m_core IsNot Nothing, Me.ToString + ".Core() EwE Core has not been initialized correctly.")
            Return Me.m_core
        End Get
    End Property

    Public ReadOnly Property EcoPathData As cEcopathDataStructures
        Get
            Debug.Assert(Me.m_EcoPathData IsNot Nothing, Me.ToString + ".EcopathData() Ecopath has not been initialized correctly.")
            Return Me.m_EcoPathData
        End Get
    End Property

    Public ReadOnly Property EcoSimData As cEcosimDatastructures
        Get
            Debug.Assert(Me.m_EcoSimData IsNot Nothing, Me.ToString + ".EcoSimData() EcoSim has not been initialized correctly.")
            Return Me.m_EcoSimData
        End Get
    End Property

    Public ReadOnly Property EcoSpaceData As cEcospaceDataStructures
        Get
            Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcoSpaceData() EcoSpace has not been initialized correctly.")
            Return Me.m_EcoSpaceData
        End Get
    End Property

#End Region

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
            Return "Import Diets"
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
            ' Return ScientificInterfaceShared.My.Resources.fish
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
    ''' Provide EwE6 with a method to execute when a user interface control for 
    ''' this plug-in is clicked by the user.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef form As Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        'Dim bHasInterface As Boolean = False

        '' Initialized ok?
        'If m_uic IsNot Nothing Then

        '    ' Test if form still exists. This is a two-step test: the interface needs to be defined, and has not been closed previously.
        '    If Me.m_form IsNot Nothing Then
        '        If Not Me.m_form.IsDisposed Then
        '            bHasInterface = True
        '        End If
        '    End If

        '    ' Create the interface if needed
        '    If Not bHasInterface Then

        '        ' Create the EwE form-derived user interface for this plug-in
        '        Me.m_form = New frmEwEPlugin()
        '        Me.m_form.Init(Me)
        '        ' Pass on the UI context to the form
        '        Me.m_form.UIContext = m_uic

        '    End If

        '    ' Pass a reference to the new interface back to whomever invoked us
        '    form = Me.m_form

        '    ' Just to show what can be done: test where this function was invoked from
        '    If TypeOf sender Is System.Windows.Forms.TreeNode Then
        '        ' Plug-in was invoked from the EwE6 navigation panel
        '    ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
        '        ' Plug-in was invoked from the EwE6 main menu
        '    End If
        'Else
        '    Debug.Assert(False, "Plugin was not initialized properly.")
        'End If
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
            Return EwEUtils.Core.eCoreExecutionState.EcopathLoaded
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

#Region "IPlugin implementation"

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Me"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "you@someplace.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Import diets from external source."
        End Get
    End Property


    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ImportDiets"
        End Get
    End Property

#End Region


End Class
