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

Option Strict On
Imports EwEUtils.Core
Imports System.ComponentModel

#Region " Shape manager base class "

''' <summary>
''' Base class for a Shape Manager. Provides implementation to make a derived Shape Manager behave like a List (For Each). 
''' </summary>
''' <remarks>This provides For Each functionality to a Shape Manager by implementing the IEnumerable.GetEnumerator() interface. 
''' Behaviour specific to a Shape Manager must be implemented in a derived class.
''' </remarks>
Public MustInherit Class cBaseShapeManager
    Implements Collections.IEnumerable
    Implements ICoreInterface
    Implements IDisposable

#Region "Protected Variables"

    ''' <summary>underlying <see cref="cEcosimDatastructures">EcoSim data</see></summary>
    Protected m_Data As cEcosimDatastructures
    ''' <summary>List of shapes owned by this manager.</summary>
    Protected m_shapes As New List(Of cForcingFunction)
    ''' <summary>Reference to the <see cref="cCore">core</see>.</summary>
    Protected m_core As cCore = Nothing
    ''' <summary><see cref="eDataTypes">Type of shape</see> this manager operates on.</summary>
    Protected m_DataType As eDataTypes = eDataTypes.NotSet

#End Region

#Region " Obligatory overrides "

    ''' <summary>
    ''' Initialize/build all the shapes that belong to this shape manager
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend MustOverride Function Init() As Boolean

    ''' <summary>
    ''' Shapes can not be created outside the Shape Manager; they must be created by a ShapeManager.
    ''' </summary>
    ''' <returns>A valid shape if successfull. Otherwise Nothing</returns>
    ''' <remarks>This is so that shapes are attached to there underlying EcoSim data when they are created </remarks>
    Public MustOverride Function CreateNewShape(ByVal strName As String, ByVal asData As Single(), _
            Optional ByVal sYZero As Single = 0, Optional ByVal sYBase As Single = 0, _
            Optional ByVal sYEnd As Single = 0, Optional ByVal sSteep As Single = 0, _
            Optional ByVal shapeType As Long = eShapeFunctionType.NotSet) As cForcingFunction

    ''' <summary>
    ''' Number of points in the data for this type of shape. This is specific to a ShapeManger implementation.
    ''' </summary>
    Public MustOverride ReadOnly Property NPoints() As Integer

#End Region ' Obligatory overrides

#Region " Constructor "

    ''' <summary>
    ''' Creates a new ShapeManager from the EcoSim data
    ''' </summary>
    ''' <param name="EcoSimData">EcoSim data used to populate the Shapes</param>
    ''' <remarks>New ShapeMangers can only be created by the Core so this is Declares as a Friend. Derived class should override the Init() function to initialize the Shapes. </remarks>
    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        m_Data = EcoSimData
        m_core = theCore
        m_DataType = DataType
    End Sub

    ''' <inheritdocs cref="IDisposable.Dispose"/>
    Friend Sub Dispose() _
        Implements IDisposable.Dispose
        Me.Clear()
        Me.m_core = Nothing
        Me.m_Data = Nothing
        GC.SuppressFinalize(Me)
    End Sub

#End Region ' Constructor

#Region " Interface for List of shapes "

    ''' <summary>
    ''' Add a cForcingFunction object to the list
    ''' </summary>
    ''' <param name="ForcingFunction">cForcingFunction or derived object to add to the ShapeManager and the underlying EcoSim data.</param>
    ''' <returns>True if Successfull</returns>
    ''' <remarks>Override this in a derived class to add the data in the cForcingFunction to the underlying EcoSim data. 
    ''' This will also work for cMediationFunction Objects as they use cForcingFunction as a base class.</remarks>
    Protected Overridable Overloads Function Add(ByVal ForcingFunction As cForcingFunction) As Boolean
        Try
            Me.m_shapes.Add(ForcingFunction)
            Me.UpdateIDs()
            Return True
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Default Public Overridable ReadOnly Property Item(ByVal ItemIndex As Integer) As cForcingFunction
        Get
            Try
                Return m_shapes.Item(ItemIndex)
            Catch ex As Exception
                cLog.Write(Me.ToString & ".Item() Error: " & ex.Message)
                Return Nothing
            End Try

        End Get

    End Property

    ''' <summary>
    ''' Use a Core one based index to retrieve an item
    ''' </summary>
    ''' <param name="CoreOneBasedIndex">One based index to the item</param>
    Public Overridable ReadOnly Property CoreItem(ByVal CoreOneBasedIndex As Integer) As cForcingFunction
        Get
            Try
                'convert core one based index to zero base for list
                Return m_shapes.Item(CoreOneBasedIndex - 1)
            Catch ex As Exception
                cLog.Write(Me.ToString & ".CoreIndex() Error: " & ex.Message)
                Return Nothing
            End Try

        End Get
    End Property

    ''' <summary>
    ''' Number of Items(shapes) in this Shape Manager
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>The collection is zero based(0). So Count is one more then the last index i.e. ShapeManager.Item(ShapeManager.Count - 1) Will return the last Item in the list.  </remarks>
    Public ReadOnly Property Count() As Integer
        Get
            Return m_shapes.Count
        End Get
    End Property

    ''' <summary>
    ''' Implementation of IEnumerable.GetEnumerator provides access to the For Each statment
    ''' </summary>
    ''' <returns>The Enumerator of the List used by this object</returns>
    ''' <remarks></remarks>
    Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return m_shapes.GetEnumerator
    End Function

    ''' <summary>
    ''' Does this ShapeManager contain this cForcingFunction
    ''' </summary>
    ''' <param name="ForcingFunction">A cForcingFunction or cMediation object</param>
    ''' <returns>True if this cForcingFunction is in the Manager. False otherwise.</returns>
    ''' <remarks></remarks>
    Public Function Contains(ByRef ForcingFunction As cForcingFunction) As Boolean
        Try
            Return m_shapes.Contains(ForcingFunction)
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Remove a shape from the Manager and the underlying EcoSim Data
    ''' </summary>
    ''' <param name="ShapeToRemove">Valid shape to remove</param>
    ''' <returns>True if successful</returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByRef ShapeToRemove As cForcingFunction) As Boolean
        'ToDo_jb cForcingFunctionManager.Remove() Do I need to check if the shape exists before I try to remove it?????????
        Try

            'Remove all references to ShapeToRemove from Databse, EcoSim data arrays and All Shape Managers
            'this will remove this record from the database and re-load all EcoSim Data Arrays that are related to the shapes
            If Not m_core.RemoveShape(ShapeToRemove.DBID) Then Return False

            'remove the shape from the shape managers memory
            Me.m_shapes.Remove(ShapeToRemove)

            Me.UpdateIDs()

            'The structure of the underlying EcoSim data has changed because it was re-loaded above
            'So re-init both Forcing and Eggprod shape managers from the underlying EcoSim Data
            'it is not good enough to just init this manager as other shape managers were affected by changing the data
            m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

            Return True

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Function

    Protected Sub UpdateIDs()
        Dim shape As cForcingFunction = Nothing
        For iShape As Integer = 0 To Me.Count - 1
            shape = Me.m_shapes(iShape)
            shape.ID = iShape
        Next iShape
    End Sub

#End Region ' Interface for List of shapes

#Region " Saving, loading and updating "

    ''' <summary>
    ''' Clear shapes from memory.
    ''' </summary>
    Friend Overridable Sub Clear()

        For Each shp As cForcingFunction In Me.m_shapes
            shp.Dispose()
        Next
        Me.m_shapes.Clear()

    End Sub

    ''' <summary>
    ''' Called by a shape to tell the manager that it has changed data. 
    ''' </summary>
    ''' <remarks>Tell the core that a shape has changed. </remarks>
    Friend Overridable Sub ShapeChanged(Optional ByVal shape As cShapeData = Nothing)
        m_core.onChanged(Me, eMessageType.DataModified)

        ' Send a shape changed message
        'Me.m_core.Messages.SendMessage()
    End Sub

    ''' <summary>
    ''' Populate the underlying EcoSim data structures with the forcing function data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This is dumb it will clear all the existing data and repopulte all the data. It has no idea what has changed </remarks>
    Public Overridable Function Update() As Boolean

        Try
            'have each shape will update the underlying EcoSim data
            For Each shape As cForcingFunction In Me
                If Not shape.Update() Then
                    cLog.Write(Me.ToString & ".Update() Shape failed to update DBID=" & shape.DBID.ToString)
                    Debug.Assert(False, Me.ToString & ".Update() Shape failed to update DBID=" & shape.DBID.ToString)
                    'this will keep trying to update the rest of the data
                    'even if there was a problem with one of the shapes
                End If
            Next shape

            Return True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Error: " & ex.Message)
        End Try

    End Function


    ''' <summary>
    ''' Load the existing shape with the underlying Ecosim data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function Load() As Boolean
        Try
            'loop over all the shapes that belong to this shape manager and ask it to load
            'the shapes were constructed with a database ID and the underlying ecosim data
            For Each shape As cForcingFunction In Me
                If Not shape.Load() Then
                    cLog.Write(Me.ToString & ".Load() Shape failed to load DBID=" & shape.DBID.ToString)
                    Debug.Assert(False, Me.ToString & ".Load() Shape failed to load DBID=" & shape.DBID.ToString)
                    'keep loading the other shapes??????
                    'Return False
                End If

            Next

            Return True
        Catch ex As Exception
            Return False
            Debug.Assert(False, Me.ToString & ".Load() Error: " & ex.Message)
        End Try

    End Function

#End Region ' Saving, loading and updating

#Region " Protected methods "

    ''' <summary>
    ''' Convert an array index from the underlying data in EcoSim into the Forcing function that is stored in the list
    ''' </summary>
    ''' <param name="iEcoSimIndex"></param>
    ''' <param name="theForcingShape"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function getShapeForEcoSimArrayIndex(ByVal iEcoSimIndex As Integer, ByRef theForcingShape As cForcingFunction) As Boolean
        Dim ff As cForcingFunction

        'Hack loop over each forcing function until one is found with a matching iEcoSimIndex
        'iEcoSimIndex was populated in init() with the array index of this forcing function
        'return the actual forcing shape in the argument theForcingShape
        For Each ff In Me
            If ff.Index = iEcoSimIndex Then
                theForcingShape = ff
                Return True
            End If
        Next ff

        'ToDo something better then this. Failed to find forcing function in the list
        'Debug.Assert(False, "Failed to find forcing Function for " & iEcoSimIndex.ToString)
        'cLog.Write(Me.ToString & ".getShapeForEcoSimArrayIndex() Failed to find forcing Function for " & iEcoSimIndex.ToString)
        theForcingShape = Nothing
        Return False

    End Function


#End Region ' Protected methods

#Region " Public Properties "

    Public ReadOnly Property Core() As cCore
        Get
            Return Me.m_core
        End Get
    End Property

#End Region

#Region " ICoreInterface Implementation "

    ''' <inheritdocs cref="ICoreInterface.DataType"/>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
        Get
            Return m_DataType
        End Get
    End Property

    ''' <inheritdocs cref="ICoreInterface.CoreComponent"/>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            Return eCoreComponentType.ShapesManager
        End Get
    End Property

    ''' <inheritdocs cref="ICoreInterface.DBID"/>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

    ''' <inheritdocs cref="ICoreInterface.GetID"/>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Function GetID() As String Implements ICoreInterface.GetID
        Dim id As Integer = CType(m_DataType, Integer)
        Return cValueID.getDataTypeID(m_DataType, id)
    End Function

    ''' <inheritdocs cref="ICoreInterface.Index"/>
    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

    ''' <inheritdocs cref="ICoreInterface.Name"/>
    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return Me.ToString
        End Get
        Set(ByVal value As String)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

#End Region ' ICoreInterface Implementation

End Class

#End Region ' Shape manager base class

#Region " Forcing Function shape manager "

''' <summary>
''' Extents the base class to manage the Forcing Shapes
''' </summary>
''' <remarks>cBaseShapeManager contains the code to manage the list. This will load the Forcing data only</remarks>
Public Class cForcingFunctionManager
    Inherits cBaseShapeManager

    ''' <summary>
    ''' Creates and loads a new Forcing shape manager out from the EcoSim data
    ''' </summary>
    ''' <param name="EcoSimData">EcoSim data structures to load the forcing shapes from</param>
    ''' <param name="theCore">Reference to the Core that is used for functionality that only the core can know</param>
    ''' <param name="DataType"><see cref="eDataTypes">Data type</see> of shapes to load</param>
    ''' <remarks>This will create the new manager and load the data into shapes</remarks>
    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)

        Init()

    End Sub

    ''' <summary>
    ''' Number of points in the underlying Shape data
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is provided for convenience. So the number of points can be retrieved without getting a shape.</remarks>
    Public Overrides ReadOnly Property NPoints() As Integer
        Get
            Return m_Data.ForcePoints
        End Get
    End Property

    ''' <summary>
    ''' Creates a new shape
    ''' </summary>
    ''' <returns>A shape that has been added to the Shape Manager</returns>
    ''' <remarks>A shape cannot be created on its own. It must be created by this factory so that it is hooked up to the core data on creation. </remarks>
    Public Overrides Function CreateNewShape(ByVal strName As String, ByVal asData As Single(), _
            Optional ByVal sYZero As Single = 0, Optional ByVal sYBase As Single = 0, _
            Optional ByVal sYEnd As Single = 0, Optional ByVal sSteep As Single = 0, _
            Optional ByVal shapeType As Long = eShapeFunctionType.NotSet) As cForcingFunction

        Dim dbID As Integer
        Dim shape As cForcingFunction
        Dim iEcoSimIndex As Integer
        Dim bSucces As Boolean = True

        'Add storage to the underlying data arrays and the db
        'AddShape() will NOT preserve the existing data  
        'All the data in the Ecosim data structures will be reloaded from the database
        If m_core.AddShape(strName, m_DataType, dbID, asData, sYZero, sYBase, sYEnd, sSteep, shapeType) Then

            'get the index from the dbid for the new shape
            iEcoSimIndex = Array.IndexOf(m_Data.ForcingDBIDs, dbID)

            'create a new shape that contains a database ID to the underlying ecosim data
            shape = New cForcingFunction(m_Data, Me, dbID, m_DataType)

            shape.ID = m_shapes.Count

            'tell the shape to load from the ecosim data
            'the call below to onChanged() will reload all the data this is not really necessary 
            'but it makes me feel safe
            shape.Load()

            'Add the new shape to the list 
            MyBase.Add(shape)

            'When the new shape was added to the EcoSim data all the existing data in memory was erased and re-loaded when the arrays where resized
            'Now tell all the Shape Managers to re-load the Ecosim data into their existing shapes
            m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

            Return shape

        End If

        Return Nothing
    End Function

    Friend Overrides Function Init() As Boolean
        Dim forcing As cForcingFunction

        'clear out any existing data
        m_shapes.Clear()
        For isp As Integer = 1 To m_Data.ForcingShapes

            If m_Data.ForcingShapeType(isp) = m_DataType Then

                forcing = New cForcingFunction(m_Data, Me, m_Data.ForcingDBIDs(isp), m_DataType)
                'keep the index of this forcing function in the list in the function itself
                'it will be used later to return the list item for a given EcoSim array index
                forcing.ID = m_shapes.Count
                forcing.Load()

                'now Add it to the base class list so that it does not try to Add via the overridden Add in this class
                MyBase.Add(forcing)

            End If

        Next isp

        Me.Load()

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class

#End Region ' Forcing Function shape manager

#Region " Mediation shape manager "

''' <summary>
''' Implemenation of the Base class for Mediation shapes
''' </summary>
''' <remarks>
''' </remarks>
Public Class cMediationManager
    Inherits cBaseShapeManager

    Private m_medData As cMediationDataStructures


    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)

        Init()

    End Sub


    Public Overrides ReadOnly Property NPoints() As Integer
        Get
            Return m_medData.NMedPoints
        End Get
    End Property

    ''' <summary>
    ''' Create a new Mediation shape
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function CreateNewShape(ByVal strName As String, ByVal asData As Single(), _
            Optional ByVal sYZero As Single = 0, Optional ByVal sYBase As Single = 0, _
            Optional ByVal sYEnd As Single = 0, Optional ByVal sSteep As Single = 0, _
            Optional ByVal shapeType As Long = eShapeFunctionType.NotSet) As cForcingFunction

        Dim dbID As Integer
        Dim medFunct As cMediationFunction

        If m_core.AddShape(strName, m_DataType, dbID, asData, sYZero, sYBase, sYEnd, sSteep, shapeType) Then

            'create a new shape that is hooked up to the underlying ecosim data
            medFunct = New cMediationFunction(m_Data, Me, Me.m_medData, dbID, m_DataType)
            medFunct.ID = m_shapes.Count
            medFunct.Load()

            'Add the new shape to the list 
            MyBase.Add(medFunct)

            m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

            Return medFunct

        End If

        Return Nothing

    End Function

    Friend Overrides Function Init() As Boolean
        Dim medFunct As cMediationFunction

        m_medData = Me.m_Data.BioMedData

        'clear out any existing data
        m_shapes.Clear()

        For imed As Integer = 1 To m_medData.MediationShapes
            'All mediation shapes from the core will have an object 
            'A mediation function may have a shape but not have any Mediating Groups or weights (MedIsUsed(iMed) = False) 
            'Mediation function objects load there own group and weight data from the ecosim data via the Load() method
            medFunct = New cMediationFunction(m_Data, Me, Me.m_medData, m_medData.MediationDBIDs(imed), Me.m_DataType)
            medFunct.ID = m_shapes.Count
            medFunct.Load()
            m_shapes.Add(medFunct)

        Next imed
        Me.Load()

    End Function

End Class

#End Region ' Mediation shape manager

#Region " Landings Mediation shape manager "

''' <summary>
''' Implemenation of the Base class for Mediation shapes
''' </summary>
''' <remarks>
''' </remarks>
Public Class cLandingsMediationManager
    Inherits cBaseShapeManager

    Private m_medData As cMediationDataStructures


    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)

        Init()

    End Sub


    Public Overrides ReadOnly Property NPoints() As Integer
        Get
            Return m_medData.NMedPoints
        End Get
    End Property

    ''' <summary>
    ''' Create a new Mediation shape
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function CreateNewShape(ByVal strName As String, ByVal asData As Single(), _
            Optional ByVal sYZero As Single = 0, Optional ByVal sYBase As Single = 0, _
            Optional ByVal sYEnd As Single = 0, Optional ByVal sSteep As Single = 0, _
            Optional ByVal shapeType As Long = eShapeFunctionType.NotSet) As cForcingFunction

        Dim dbID As Integer
        Dim medFunct As cLandingsMediationFunction

        If m_core.AddShape(strName, m_DataType, dbID, asData, sYZero, sYBase, sYEnd, sSteep, shapeType) Then

            'create a new shape that is hooked up to the underlying ecosim data
            medFunct = New cLandingsMediationFunction(m_Data, Me, Me.m_medData, dbID, m_DataType)
            medFunct.ID = m_shapes.Count
            medFunct.Load()

            'Add the new shape to the list 
            MyBase.Add(medFunct)

            m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

            Return medFunct

        End If

        Return Nothing

    End Function

    Friend Overrides Function Init() As Boolean
        Dim medFunct As cLandingsMediationFunction

        Me.m_medData = Me.m_Data.PriceMedData

        'clear out any existing data
        m_shapes.Clear()

        For imed As Integer = 1 To m_medData.MediationShapes
            'All mediation shapes from the core will have an object 
            'A mediation function may have a shape but not have any Mediating Groups or weights (MedIsUsed(iMed) = False) 
            'Mediation function objects load there own group and weight data from the ecosim data via the Load() method
            medFunct = New cLandingsMediationFunction(m_Data, Me, Me.m_medData, Me.m_medData.MediationDBIDs(imed), Me.m_DataType)
            medFunct.ID = m_shapes.Count
            medFunct.Load()
            m_shapes.Add(medFunct)

        Next imed
        Me.Load()

    End Function

End Class

#End Region ' Mediation shape manager

#Region " Capacity shape manager "

''' <summary>
''' Implemenation of the Base class for capacity shapes
''' </summary>
Public Class cCapMapResponseManager
    Inherits cBaseShapeManager

    Private m_medData As cMediationDataStructures
    Private m_spaceData As cEcospaceDataStructures


    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByVal SpaceData As cEcospaceDataStructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)

        Me.m_spaceData = SpaceData
        Init()

    End Sub


    Public Overrides ReadOnly Property NPoints() As Integer
        Get
            Return m_medData.NMedPoints
        End Get
    End Property

    ''' <summary>
    ''' Create a new Mediation shape
    ''' </summary>
    Public Overrides Function CreateNewShape(ByVal strName As String, ByVal asData As Single(), _
            Optional ByVal sYZero As Single = 0, Optional ByVal sYBase As Single = 0, _
            Optional ByVal sYEnd As Single = 0, Optional ByVal sSteep As Single = 0, _
            Optional ByVal shapeType As Long = eShapeFunctionType.NotSet) As cForcingFunction

        Dim dbID As Integer

        If m_core.AddShape(strName, m_DataType, dbID, asData, sYZero, sYBase, sYEnd, sSteep, shapeType) Then

            Dim medFunct As cEnviroResponseFunction

            'create a new shape that is hooked up to the underlying ecosim data
            medFunct = New cEnviroResponseFunction(m_Data, Me, Me.m_medData, dbID, m_DataType)
            medFunct.ID = m_shapes.Count
            medFunct.Load()

            medFunct.ShapeFunctionType = shapeType

            'Add the new shape to the list 
            MyBase.Add(medFunct)

            m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

            Return medFunct

        End If

        Return Nothing

    End Function

    Friend Overrides Function Init() As Boolean
        Dim medFunct As cEnviroResponseFunction

        'get the Enviromental response function for Capacity 
        m_medData = Me.m_Data.CapEnvResData

        'clear out any existing data
        m_shapes.Clear()

        For imed As Integer = 1 To m_medData.MediationShapes
            'All mediation shapes from the core will have an object 
            medFunct = New cEnviroResponseFunction(m_Data, Me, Me.m_medData, m_medData.MediationDBIDs(imed), Me.m_DataType)
            medFunct.ID = m_shapes.Count
            medFunct.Load()
            m_shapes.Add(medFunct)

        Next imed
        Me.Load()

    End Function

End Class

#End Region ' Capacity shape manager

#Region " Egg Production shape manager "

''' <summary>
'''Manager for the Egg Production Shapes
''' </summary>
''' <remarks> Egg production and Forcing shapes are stored in the same data structures in EcoSim so most of their functionality is in cForcingFunctionManager. 
''' The only real difference is in how the data is applied to groups. Egg Production can only be applied to a Stanza Group.
'''  So the manager contains a list of all the Stanza Groups that have an associated Egg Production shape.</remarks>
Public Class cEggProductionManager
    Inherits cForcingFunctionManager

    Private m_grplist As cGroupShapeList

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)

    End Sub

    Public Overrides Function Load() As Boolean

        Dim rv As Boolean

        rv = MyBase.Load()
        rv = rv And Me.LoadGroupShapeList()

        Return rv

    End Function

    ''' <summary>
    ''' Overrides the base class CForcingFunctionManager InitAppliesTo() to initialize the cAppliesToList with the EggProduction forcing data from EcoSim
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>EggProduction forcing data points are stored in the same data structures as Time Forcing data cEcoSimDataStructures.zscale(). 
    ''' so cEggProductionManager can use the same InitShapes routine as it's base class cForcingFunctionManager</remarks>
    Private Function LoadGroupShapeList() As Boolean
        Try
            Dim shape As cForcingFunction
            m_grplist = New cGroupShapeList(m_Data, MyBase.m_core.m_Stanza, Me)

            'EggProdShape data is only for Stanza groups data 
            'EggProdShapeSplit(iStanza) holds the forcing function shape index for this stanza group
            For iStanza As Integer = 1 To m_core.m_Stanza.Nsplit 'nSplit is the number of stanza groups

                Me.getShapeForEcoSimArrayIndex(m_core.m_Stanza.EggProdShapeSplit(iStanza), shape)
                'make the stanza index zero based 
                m_grplist.Add(New cGroupShapePair(Me, shape, iStanza))

            Next iStanza

            Return True

        Catch ex As Exception
            cLog.Write(Me.ToString & ".InitAppliesTo() Error:" & ex.Message)
            Debug.Assert(False, Me.ToString & ".InitAppliesTo() Error:" & ex.Message)
        End Try


    End Function

    Public ReadOnly Property GroupShapeList() As cGroupShapeList
        Get
            Return m_grplist
        End Get
    End Property

    Friend Sub validationFailedMessage()
        ' ToDo: globalize this
        m_core.Messages.SendMessage(New cMessage("Validataion Failed. Egg Production no shape with this index.", eMessageType.DataValidation, _
                                    eCoreComponentType.ShapesManager, eMessageImportance.Information, eDataTypes.EggProd))
    End Sub


    ''' <summary>
    ''' Tell the core that data has been changed
    ''' </summary>
    ''' <remarks>Called by a GroupShapePair when its data has changed</remarks>
    Friend Function OnChanged(ByRef GroupShapePair As cGroupShapePair) As Boolean

        Try
            'neither of these should ever happen 
            Debug.Assert(GroupShapePair.iShape <= m_core.m_EcoSimData.ForcingShapes, Me.ToString & ".OnChanged() shape index out of bounds.")
            Debug.Assert(GroupShapePair.iCoreStanzaIndex <= m_core.m_Stanza.Nsplit, Me.ToString & ".OnChanged() stanza index out of bounds.")

            'update the cores data
            m_core.m_Stanza.EggProdShapeSplit(GroupShapePair.iCoreStanzaIndex) = GroupShapePair.iShape

            'Tell the core that this data has changed
            m_core.onChanged(Me, eMessageType.DataModified)
            Return True


        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
        End Try

    End Function

End Class

#Region "Group Shape List"

''' <summary>
''' This object represents the iStanza Group (index of stanza group) and Egg Production Forcing Shape index that is used by this Stanza group
''' </summary>
''' <remarks></remarks>
Public Class cGroupShapePair

    Private m_iStanza As Integer
    ''' <summary>Index of a shape in the cEggProductionManager.Item() list</summary>
    Private m_iManager As Integer
    Private m_shape As cForcingFunction
    Private m_manager As cEggProductionManager


    Public Function Clear() As Boolean
        ShapeID = cCore.NULL_VALUE
        m_shape = Nothing
    End Function

    ''' <summary>
    ''' Index of this Shape in the cEggProductionManager.Item() lists
    ''' </summary>
    ''' <remarks>The shape to use for this pair from the cEggProductionManager. 
    ''' <example>
    ''' 'get the cGroupShapePair for the first stanza group from the EggProdManager.GroupShapeList
    ''' 'this cGroupShapePair will have an iStanzaGroup=0
    ''' Dim pair As cGroupShapePair = EggProdManager.GroupShapeList.Item(0)
    ''' 'make this cGroupShapePair use the first shape in the EggProdManager
    ''' pair.ShapeMangerIndex = 0
    ''' </example>
    '''</remarks>
    Public Property ShapeID() As Integer
        Get
            Return Me.m_iManager
        End Get

        Set(ByVal value As Integer)
            If (value < m_manager.Count And value >= 0) Or (value = cCore.NULL_VALUE) Then
                'only set the value if it passed the lame validation
                Me.m_iManager = value

                If value >= 0 Then
                    Me.m_shape = Me.m_manager.Item(Me.m_iManager)
                Else
                    Me.m_shape = Nothing
                End If
                Update()
            Else
                Me.m_manager.validationFailedMessage()
            End If
        End Set

    End Property

    ''' <summary>
    ''' A zero based index to the Stanzas. This is the same as is used by cCore.StanzaGroups.Item(iStanza) list
    ''' </summary>
    Public ReadOnly Property iStanzaGroup() As Integer
        Get
            Return m_iStanza
        End Get
    End Property

    ''' <summary>
    ''' Index of the shape in the underlying core data 
    ''' </summary>
    ''' <remarks>
    ''' This is a friend because only the manager should care what the underlying core shape index is.
    ''' </remarks>
    Friend ReadOnly Property iShape() As Integer
        Get
            If m_shape IsNot Nothing Then
                Return m_shape.Index
            Else
                Return 0
            End If
        End Get
    End Property

    ''' <summary>
    ''' Index used by the core to update data
    ''' </summary>
    ''' <remarks>Stanzas are stored in a zeor base list for the interface. This is the one based index used by the core.</remarks>
    Friend ReadOnly Property iCoreStanzaIndex() As Integer
        Get
            Return m_iStanza + 1
        End Get
    End Property



    Sub New(ByRef theManager As cEggProductionManager, ByRef Shape As cForcingFunction, ByVal StanzaIndex As Integer)
        m_manager = theManager

        'Zero based public stanza index for stanza list 
        'this is the same index as in the cCore.StanzaGroups.Item(iStanza) 
        m_iStanza = StanzaIndex - 1

        If Shape Is Nothing Then
            m_shape = Nothing
            m_iManager = cCore.NULL_VALUE
        Else
            m_shape = Shape
            m_iManager = Shape.ID
        End If
    End Sub


    Friend Function Update() As Boolean
        Try

            'tell the manager that this pair has changed it data
            'this will validate the data

            If m_shape IsNot Nothing Then
                'index of the shape in the managers list stored in ID during construction of the shape
                m_iManager = m_shape.ID
            Else
                m_iManager = cCore.NULL_VALUE
            End If

            Return m_manager.OnChanged(Me)

        Catch ex As Exception
            cLog.Write(ex)
            Return False
        End Try

    End Function

End Class


''' <summary>
''' This is a collection of cShapeGroupPair 
''' </summary>
''' <remarks></remarks>
Public Class cGroupShapeList
    Implements Collections.IEnumerable

    Private m_list As New List(Of cGroupShapePair)
    Private m_data As cEcosimDatastructures
    Private m_stanza As cStanzaDatastructures
    Private m_manager As cEggProductionManager

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef StanzaData As cStanzaDatastructures, ByRef EggProdManager As cEggProductionManager)
        m_data = EcoSimData
        m_manager = EggProdManager
        m_stanza = StanzaData
    End Sub

    Friend Sub Add(ByRef shapeGroupPair As cGroupShapePair)

        'ToDo_jb cAppliesToList.Add()  Make sure the shapeGroupPair.iStanzaGroup is a valid stanza group
        m_list.Add(shapeGroupPair)

    End Sub


    Default Public Property Item(ByVal Index As Integer) As cGroupShapePair
        Get
            Try
                Return m_list.Item(Index)
            Catch ex As Exception
                Return Nothing
            End Try
        End Get
        Set(ByVal value As cGroupShapePair)
            Try
                m_list.Item(Index) = value
            Catch ex As Exception
                Return
            End Try
        End Set
    End Property


    Public Function Count() As Integer
        Return m_list.Count
    End Function

    Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return m_list.GetEnumerator
    End Function


End Class

#End Region

#End Region ' Egg Production shape manager

#Region " Effort shape managers "

#Region " Fishing shape manager base class "

Public MustInherit Class cFishingBaseShapeManager
    : Inherits cBaseShapeManager

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)
        Init()
    End Sub

    Public Overrides ReadOnly Property NPoints() As Integer
        Get
            Return m_Data.NTimes
        End Get
    End Property

    Public MustOverride Sub ResetToDefaults()

    Public MustOverride Function EcopathBaseValue(ByVal iShape As Integer) As Single

End Class

#End Region ' Effort shape manager base class

#Region " Fishing Rate Shape Manager "

Public Class cFishingEffortManger
    : Inherits cFishingBaseShapeManager

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)
    End Sub

    ''' <summary>
    ''' Fishing Rate shapes can not be created
    ''' </summary>
    ''' <returns>Always Nothing.</returns>
    Public Overrides Function CreateNewShape(ByVal strName As String, ByVal asData() As Single, Optional ByVal sYZero As Single = 0.0, Optional ByVal sYBase As Single = 0.0, Optional ByVal sYEnd As Single = 0.0, Optional ByVal sSteep As Single = 0.0, Optional ByVal shapeType As Long = eShapeFunctionType.NotSet) As cForcingFunction
        Return Nothing
    End Function

    Friend Overrides Function Init() As Boolean
        Dim shape As cFishingRateShape = Nothing
        Dim iFleet As Integer

        'clear out any existing data
        m_shapes.Clear()
        For iFleet = 1 To m_Data.nGear ' number of fishing fleets

            shape = New cFishingRateShape(m_Data, Me, m_Data.FishRateGearDBID(iFleet), m_core.m_EcoPathData.FleetName(iFleet))
            'keep the index of this forcing function in the list in the function itself
            'it will be used later to return the list item for a given EcoSim array index
            shape.ID = m_shapes.Count
            shape.Index = iFleet
            shape.Load()
            m_shapes.Add(shape)

        Next iFleet

        If m_Data.nGear > 0 Then
            'Add the Combined Gear types shape to the end of the list
            'Its iFleet index is m_Data.nGear + 1 
            'this is critical as that is how the shape itself decides that it the Combined Fleets shape
            'the Combined Fleets shape updates all the other fleets as well as the FishMort shapes
            shape = New cFishingRateShape(m_Data, Me, cCore.NULL_VALUE, My.Resources.CoreDefaults.CORE_ALL_FLEETS)
            shape.ID = m_shapes.Count
            shape.Index = m_Data.nGear + 1
            shape.Load()
            m_shapes.Add(shape)
        End If

        Me.Load()

    End Function

    Public Overrides Sub ResetToDefaults()
        Me.m_Data.DefaultFishingRates()
        Me.Load()
        Me.ShapeChanged()
    End Sub

    Public Overrides Function EcopathBaseValue(ByVal iShape As Integer) As Single
        Return 1
    End Function

End Class

#End Region ' Fishing Rate shape manager

#Region " Fish Mortality shape manager "

Public Class cFishingMortalityManger
    : Inherits cFishingBaseShapeManager

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, ByVal DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)
    End Sub

    ''' <summary>
    ''' Fish Mort shapes can not be created
    ''' </summary>
    ''' <returns>Always Nothing</returns>
    Public Overrides Function CreateNewShape(ByVal strName As String, ByVal asData() As Single, Optional ByVal sYZero As Single = 0.0, Optional ByVal sYBase As Single = 0.0, Optional ByVal sYEnd As Single = 0.0, Optional ByVal sSteep As Single = 0.0, Optional ByVal shapeType As Long = eShapeFunctionType.NotSet) As cForcingFunction
        Return Nothing
    End Function

    Friend Overrides Function Init() As Boolean
        Dim shape As cFishingMortShape

        'clear out any existing data
        m_shapes.Clear()
        For iGroup As Integer = 1 To m_Data.nGroups ' one shape for each group 
            ' Fishing rate shapes are no longer loaded from the DB
            m_Data.FishRateNoDBID(iGroup) = Me.m_core.m_EcoSimData.GroupDBID(iGroup)

            shape = New cFishingMortShape(m_Data, Me, m_Data.FishRateNoDBID(iGroup), m_core.m_EcoPathData.GroupName(iGroup))
            'keep the index of this forcing function in the list in the function itself
            'it will be used later to return the list item for a given EcoSim array index
            shape.ID = m_shapes.Count
            shape.Index = iGroup
            shape.Load()
            m_shapes.Add(shape)

        Next iGroup
        Me.Load()

    End Function

    Public Overrides Sub ResetToDefaults()
        Me.m_Data.DefaultFishMortalityRates()
        Me.Load()
        Me.ShapeChanged()
    End Sub

    Public Overrides Function EcopathBaseValue(ByVal iShape As Integer) As Single
        Return Me.m_core.m_EcoSimData.Fish1(iShape)
    End Function

End Class

#End Region ' Fish Mortality shape manager

#End Region ' Effort shape managers
