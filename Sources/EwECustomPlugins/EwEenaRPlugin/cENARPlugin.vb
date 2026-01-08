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

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwECore.Common
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
Public Class cENARPlugin
    Implements IPlugin
    Implements EwECore.ICorePlugin
    Implements EwECore.IEcopathPlugin
    Implements EwECore.IEcopathRunInitializedPlugin
    Implements EwECore.IEcosimInitializedPlugin
    Implements EwECore.IEcospaceInitializedPlugin
    Implements EwECore.IUIContextPlugin
    Implements EwECore.IMenuItemTogglePlugin
    Implements EwECore.IAutoSavePlugin
    Implements EwECore.IEcospacePlugin

#Region " Local variables"

    ''' <summary>The core that this plug-in can use</summary>
    Private m_core As cCore

    Private m_EcoPath As cEcopathModel
    Private m_EcoSim As cEcosimModel
    Private m_EcoSpace As cEcoSpace
    Private m_EcoPathData As cEcopathDataStructures
    Private m_EcoSimData As cEcosimDatastructures
    Private m_EcoSpaceData As cEcospaceDataStructures

    Private m_uic As cUIContext = Nothing

#End Region

#Region "Ecopath, Ecosim and Ecospace events"

    ''' <summary>
    ''' Every plug-in is told to initialize to the EwE core as soon as it is loaded. 
    ''' Typically, plug-ins use this opportunity to store a reference to the core
    ''' for later use.
    ''' </summary>
    ''' <param name="CoreAsObject">The core, casted to a generic object</param>
    Public Sub Initialize(CoreAsObject As Object) Implements IPlugin.Initialize
        Try
            Me.m_core = DirectCast(CoreAsObject, cCore)
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
    Public Sub CoreInitialized(ByRef EcopathAsObject As Object, ByRef EcoSimAsObject As Object, ByRef EcoSpaceAsObject As Object) Implements EwECore.ICorePlugin.CoreInitialized
        Try

            Me.m_EcoPath = TryCast(EcopathAsObject, cEcopathModel)
            Me.m_EcoSim = TryCast(EcoSimAsObject, cEcosimModel)
            Me.m_EcoSpace = TryCast(EcoSpaceAsObject, cEcoSpace)

            Debug.Assert((Me.m_EcoPath IsNot Nothing) And (Me.m_EcoSim IsNot Nothing) And (Me.m_EcoSpace IsNot Nothing),
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
    Public Function LoadModel(dataSource As Object) As Boolean Implements EwECore.IEcopathPlugin.LoadModel
        Return True
    End Function

    ''' <summary>
    ''' An Ecopath model has been saved.
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    Public Function SaveModel(dataSource As Object) As Boolean Implements EwECore.IEcopathPlugin.SaveModel
        Return True
    End Function

    ''' <summary>
    ''' An Ecopath model has been closed.
    ''' </summary>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    Public Function CloseModel() As Boolean Implements EwECore.IEcopathPlugin.CloseModel
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
    Public Sub EcopathRunInitialized(EcopathDataAsObject As Object, TaxonDataAsObject As Object, StanzaDataAsObject As Object) Implements EwECore.IEcopathRunInitializedPlugin.EcopathRunInitialized
        Me.m_EcoPathData = TryCast(EcopathDataAsObject, cEcopathDataStructures)
    End Sub

    Public Sub EcosimInitialized(EcosimDatastructures As Object) Implements EwECore.IEcosimInitializedPlugin.EcosimInitialized
        Me.m_EcoSimData = TryCast(EcosimDatastructures, cEcosimDatastructures)
    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements EwECore.IEcospaceInitializedPlugin.EcospaceInitialized
        Me.m_EcoSpaceData = TryCast(EcospaceDatastructures, cEcospaceDataStructures)
        Me.DisableEnaR()
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
    Public Sub UIContext(uic As Object) Implements EwECore.IUIContextPlugin.UIContext

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
    Public ReadOnly Property DisplayName() As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.MENU_SAVE_ENAR
        End Get
    End Property

    ''' ----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what image to show for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage() As Object Implements EwECore.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what text to display when the user hovers the mouse cursor
    ''' over a user interface element for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlTooltipText() As String Implements EwECore.IGUIPlugin.ControlTooltipText
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
    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef form As Object) Implements EwECore.IGUIPlugin.OnControlClick

        Try
            If (Me.m_EcoSpaceData IsNot Nothing) Then
                Me.m_EcoSpaceData.bENA = Not Me.m_EcoSpaceData.bENA
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public ReadOnly Property IsChecked As Boolean Implements IMenuItemTogglePlugin.IsChecked
        Get
            Return Me.AutoSave
        End Get
    End Property

    Private Sub DisableEnaR()
        Try
            If (Me.m_EcoSpaceData IsNot Nothing) Then
                Me.m_EcoSpaceData.bENA = False
            End If
        Catch ex As Exception
            'just in case it all goes sideways
            Debug.Assert(False, Me.ToString + ".DisableEnaR() Exception: " + ex.Message)
        End Try
    End Sub

    Public Sub LoadEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.LoadEcospaceScenario
        Me.DisableEnaR()
    End Sub

    Public Sub SaveEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.SaveEcospaceScenario
        ' NOP
    End Sub

    Public Sub CloseEcospaceScenario() Implements IEcospacePlugin.CloseEcospaceScenario
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 where to place an item in its main menu.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MenuItemLocation() As String Implements EwECore.IMenuItemPlugin.MenuItemLocation
        Get
            ' For example, a plug-in menu item should be placed in the main the 'Tools' menu. 
            Return "MenuEcospace"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 when during application execution this plug-in should be accessible 
    ''' to users.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState() As eCoreExecutionState Implements EwECore.IGUIPlugin.EnabledState
        Get
            ' This plug-in is available at any time during EwE execution
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

#End Region ' User Interface plug-in implementation

#Region " Auto-save plug-in implementation "

    Public Function AutoSaveType() As eAutosaveTypes Implements IAutoSavePlugin.AutoSaveType
        Return eAutosaveTypes.Ecospace
    End Function

    Public Function AutoSaveOutputPath() As String Implements IAutoSavePlugin.AutoSaveOutputPath
        Return Path.Combine(Me.m_core.DefaultOutputPath(Me.AutoSaveType), "ena_data")
    End Function

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave
        Get
            If (Me.m_EcoSpaceData Is Nothing) Then Return False
            Return Me.m_EcoSpaceData.bENA
        End Get
        Set(value As Boolean)
            If (Me.m_EcoSpaceData Is Nothing) Then Return
            Me.m_EcoSpaceData.bENA = value
        End Set
    End Property

#End Region ' Auto-save plug-in implementation

#Region "IPlugin implementation"

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Write an enaR SCOR format file for each cell at each time step"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "enaREcoSpaceFileWriter"
        End Get
    End Property

#End Region

End Class

