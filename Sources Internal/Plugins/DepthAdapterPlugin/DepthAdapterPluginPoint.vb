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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cDepthChangePluginPoint
    Implements EwEPlugin.IPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcopathRunInitializedPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcospaceInitializedPlugin

#Region " Private variables "

    ''' <summary>The core that this plug-in can use</summary>
    Private m_core As cCore

    Private m_EcoPath As cEcoPathModel
    Private m_EcoSim As cEcoSimModel
    Private m_EcoSpace As cEcoSpace
    Private m_EcoPathData As cEcopathDataStructures
    Private m_EcoSimData As cEcosimDatastructures
    Private m_EcoSpaceData As cEcospaceDataStructures

    Private m_uic As cUIContext

    Private m_SpatialDataLoader As cSpatialDataLoader
    Private m_EwEIsChanged As Boolean

#End Region ' Private variables

#Region " Modeling Code "

#Region " Public Methods and Properties "

    Friend ReadOnly Property SpatialDataLoader As cSpatialDataLoader
        Get
            Return Me.m_SpatialDataLoader
        End Get
    End Property

#End Region ' Public Methods and Properties

#Region " Private stuff "

    Private Sub InitSpatialData()
        m_SpatialDataLoader = New cSpatialDataLoader(Me)
    End Sub

#End Region ' Private stuff

#End Region ' Modeling Code

#Region " Plugin Code "

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

            Debug.Assert((m_EcoPath IsNot Nothing) And (m_EcoSim IsNot Nothing) And (m_EcoSpace IsNot Nothing), _
                         Me.ToString + ".CoreInitialized() Failed to initialize data.")

            'Only Initialize once
            If Me.SpatialDataLoader Is Nothing Then
                Me.InitSpatialData()
                Me.SpatialDataLoader.AddedDepthAdapter()
            End If

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

    Public ReadOnly Property Ecospace As cEcoSpace
        Get
            Debug.Assert(Me.m_EcoSpace IsNot Nothing, Me.ToString + ".Ecospace() Ecospace has not been initialized correctly.")
            Return Me.m_EcoSpace
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

#End Region ' User Interface plug-in implementation

#Region " IPlugin implementation "

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return Me.ToString
        End Get
    End Property

#End Region ' IPlugin implementation

#End Region ' Plugin Code

End Class

