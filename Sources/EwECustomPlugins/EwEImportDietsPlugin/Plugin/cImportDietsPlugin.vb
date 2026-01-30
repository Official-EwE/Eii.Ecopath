' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwECore.Plugins
Imports EwECore.Plugins.Core
Imports EwECore.Plugins.Ecopath
Imports EwECore.Plugins.Ecosim
Imports EwECore.Plugins.Ecospace
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls

''' <summary>
''' 
''' </summary>
''' <seealso cref="IPlugin" />
''' <seealso cref="ICorePlugin" />
''' <seealso cref="IEcopathPlugin" />
''' <seealso cref="IEcopathRunInitializedPlugin" />
''' <seealso cref="IEcosimInitializedPlugin" />
''' <seealso cref="IEcospaceInitializedPlugin" />
''' <seealso cref="IUIContextPlugin" />
''' <seealso cref="IMenuItemPlugin" />
''' <seealso cref="INavigationTreeItemPlugin" />
Public Class cImportDietsPlugin
    Implements IPlugin
    Implements ICorePlugin
    Implements IEcopathPlugin
    Implements IEcopathRunInitializedPlugin
    Implements IEcosimInitializedPlugin
    Implements IEcospaceInitializedPlugin
    Implements IUIContextPlugin
    Implements INavigationTreeItemPlugin

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

    'xxxxxxxxxxxxxxxxxxxxxxxxxxx
    'We don't need a UI at this time
    'A Menu Item will work just fine
    ' Private m_form As frmEwEPlugin = Nothing
    'xxxxxxxxxxxxxxxxxxxxxxxxxxx

#End Region

#Region "Public Methods"

    Public Sub OpenModel(filename As String)
        Me.m_core.LoadModel(filename)
    End Sub

#End Region

#Region " Plug-in points "

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Every plug-in is told to initialize to the EwE core as soon as it is loaded. 
    ''' Typically, plug-ins use this opportunity to store a reference to the core
    ''' for later use.
    ''' </summary>
    ''' <param name="CoreAsObject">The core, casted to a generic object</param>
    ''' -----------------------------------------------------------------------------
    Public Sub Initialize(CoreAsObject As Object) Implements IPlugin.Initialize
        Try
            Me.m_core = DirectCast(CoreAsObject, cCore)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".Initialize() Exception " + ex.Message)
        End Try
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that is called when the core has initialized its models
    ''' Ecopath, Ecosim and Ecospace. This is the only opportunity for plug-ins to grab 
    ''' references to these models.
    ''' </summary>
    ''' <param name="EcopathAsObject"></param>
    ''' <param name="EcoSimAsObject"></param>
    ''' <param name="EcoSpaceAsObject"></param>
    ''' -----------------------------------------------------------------------------
    Public Sub CoreInitialized(ByRef EcopathAsObject As Object, ByRef EcoSimAsObject As Object, ByRef EcoSpaceAsObject As Object) Implements ICorePlugin.CoreInitialized
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' An Ecopath model has loaded.
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    ''' -----------------------------------------------------------------------------
    Public Function LoadModel(dataSource As Object) As Boolean Implements IEcopathPlugin.LoadModel
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' An Ecopath model has been saved.
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    ''' -----------------------------------------------------------------------------
    Public Function SaveModel(dataSource As Object) As Boolean Implements IEcopathPlugin.SaveModel
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' An Ecopath model has been closed.
    ''' </summary>
    ''' <returns>True if the plug-in point executed successfully.</returns>
    ''' -----------------------------------------------------------------------------
    Public Function CloseModel() As Boolean Implements IEcopathPlugin.CloseModel
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' An Ecopath model is about to run.
    ''' </summary>
    ''' <param name="EcopathDataAsObject"></param>
    ''' <param name="TaxonDataAsObject"></param>
    ''' <param name="StanzaDataAsObject"></param>
    ''' -----------------------------------------------------------------------------
    Public Sub EcopathRunInitialized(EcopathDataAsObject As Object, TaxonDataAsObject As Object, StanzaDataAsObject As Object) Implements IEcopathRunInitializedPlugin.EcopathRunInitialized
        Me.m_EcoPathData = TryCast(EcopathDataAsObject, cEcopathDataStructures)
        Debug.Assert(Me.m_EcoPathData IsNot Nothing, Me.ToString + ".EcopathRunInitialized() Failed to get EcopathDataStructures.")
    End Sub

    Public Sub EcosimInitialized(EcosimDatastructures As Object) Implements IEcosimInitializedPlugin.EcosimInitialized
        Me.m_EcoSimData = TryCast(EcosimDatastructures, cEcosimDatastructures)
        Debug.Assert(Me.m_EcoSimData IsNot Nothing, Me.ToString + ".EcosimInitialized() Failed to get EcosimDataStructures.")
    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
        Me.m_EcoSpaceData = TryCast(EcospaceDatastructures, cEcospaceDataStructures)
        Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcospaceInitialized() Failed to get EcosimDataStructures.")
    End Sub

#End Region ' Plug-in points

#Region " Data access "

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

#End Region ' Data access

#Region " User Interface plug-in implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User Interfaces require a UIContext, which provides not only access to
    ''' a running core, but also to a styleguide, command handler, and other
    ''' aspects that binds user interface elements in the EwE 6 application. 
    ''' </summary>
    ''' <param name="uic">The <see cref="cUIContext"/> to connect to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext

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
    Public ReadOnly Property DisplayName() As String Implements IGUIPlugin.DisplayName
        Get
            Return My.Resources.CONTROL_TEXT
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what image to show for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage() As Object Implements IGUIPlugin.ControlImage
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
    Public ReadOnly Property ControlTooltipText() As String Implements IGUIPlugin.ControlTooltipText
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
    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef form As Object) Implements IGUIPlugin.OnControlClick
        Dim ofd As New OpenFileDialog()

        ofd.Filter = ScientificInterfaceShared.My.Resources.FILEFILTER_MODEL_SAVE
        ofd.FilterIndex = 1

        If ofd.ShowDialog = DialogResult.OK Then
            If Me.SetEcopathRunState Then

                Dim ImportDiets As New cDietImporter(Me.Core, Me.EcoPathData)
                ImportDiets.Run(ofd.FileName)

            End If 'Me.setEcopathState
        End If 'ofd.ShowDialog = DialogResult.OK 

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 when during application execution this plug-in should be accessible 
    ''' to users.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState() As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            ' This plug-in is available at any time during EwE execution
            Return eCoreExecutionState.EcopathInitialized
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 where to place an item in its navigation tree.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NavigationTreeItemLocation() As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndParameterization\ndEcopathInput\ndEcopathInputTools"
        End Get
    End Property

#End Region ' User Interface plug-in implementation

#Region " IPlugin implementation "

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Szymon Surma"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return My.Resources.CONTROL_DESCRIPTION
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "EwEImportDietsPlugin"
        End Get
    End Property

#End Region ' IPlugin implementation

#Region " Internals "

    Private Function SetEcopathRunState() As Boolean
        If Not Me.m_core.StateMonitor.HasEcopathRan Then
            Return Me.m_core.RunEcopath()
        End If
        Return True
    End Function

#End Region ' Internals

End Class
