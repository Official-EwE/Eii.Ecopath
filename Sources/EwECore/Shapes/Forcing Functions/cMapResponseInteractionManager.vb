#Region " Imports "

Option Strict On

Imports EwEUtils.Core

#End Region ' Imports

Public Class cMapResponseInteractionManager
    Implements ICoreInterface

#Region "Private data"

    Private m_maps As List(Of IEnviroInputMap)
    Private m_SpaceData As cEcospaceDataStructures
    Private m_MedData As cMediationDataStructures
    Private m_core As cCore

#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

#End Region ' Constructor

#Region " Public Methods and Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the number of maps managed by the manager.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nMaps() As Integer
        Get
            Return Me.m_maps.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the map at a given index [1, <see cref="nMaps"/>].
    ''' </summary>
    ''' <param name="MapIndex">The one-based index of the map to return.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Map(ByVal MapIndex As Integer) As IEnviroInputMap
        Get
            If MapIndex > 0 And MapIndex <= Me.m_maps.Count Then
                Return Me.m_maps(MapIndex - 1)
            End If
            Return Nothing
        End Get

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a map to the database.
    ''' </summary>
    ''' <param name="strMapName">The name of the map to add.</param>
    ''' <param name="variable">Variable to link the map to, which should be listed
    ''' among the <see cref="SupportedVariables"/>.</param>
    ''' <param name="iDBID">The database ID that will be assigned to the new map.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddMap(ByVal strMapName As String, ByVal variable As eVarNameFlags, ByRef iDBID As Integer) As Boolean
        Try
            ' Check if variable is supported
            If (Array.IndexOf(Me.SupportedVariables(), variable) = -1) Then Return False
            ' Let the core do the work
            Return Me.m_core.AddEcospaceCapacityMap(strMapName, variable, iDBID)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".AddMap() ")
            Return False
        End Try
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a map from the database.
    ''' </summary>
    ''' <param name="iDBID"><see cref="ICoreInterface.DBID"/> of the map to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveMap(ByVal iDBID As Integer) As Boolean
        Try
            Return Me.m_core.RemoveEcospaceCapacityMap(iDBID)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RemoveMap() ")
            Return False
        End Try
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns all supported map variables.
    ''' </summary>
    ''' <returns>
    ''' All supported map variables.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function SupportedVariables() As eVarNameFlags()
        Return New eVarNameFlags() {eVarNameFlags.LayerDepth, eVarNameFlags.LayerRelPP, eVarNameFlags.LayerRelCin}
    End Function

#End Region ' Public Methods and Properties

#Region " Friend interfaces "

    Friend Sub Init(ByVal theCore As cCore, ByVal spaceData As cEcospaceDataStructures, ByVal MediationData As cMediationDataStructures)
        Me.m_core = theCore
        Me.m_SpaceData = spaceData
        Me.m_MedData = MediationData
        Me.m_maps = New List(Of IEnviroInputMap)
    End Sub

    Friend Function Load() As Boolean

        Dim objMap As IEnviroInputMap = Nothing
        Dim bSuccess As Boolean = True

        Try

            Me.m_maps.Clear()

            'populate the list of IEnviroInputMap objects that the user will interact with 
            'to change region related parameters from the interface
            For iMap As Integer = 1 To Me.m_SpaceData.NumCapMaps
                Try

                    Dim data As Object = Me.m_core.EcospaceBasemap.GetLayerData(Me.m_SpaceData.CapMapVariable(iMap), Me.m_SpaceData.CapMapVarIndex(iMap))
                    Dim map As IEnviroInputMap = Nothing

                    Debug.Assert(data IsNot Nothing)
                    Debug.Assert(data.GetType.IsArray)

                    ' Get type of Ecospace element data
                    Dim tElm As Type = data.GetType.GetElementType

                    If (tElm Is GetType(Integer)) Then
                        map = New cEnviroInputMap(Of Integer)(Me.m_core, Me.m_SpaceData.CapMapDBID(iMap), iMap, Me.m_SpaceData.CapMapName(iMap), _
                                                              Me, DirectCast(data, Integer(,)), Me.m_SpaceData.CapMapVariable(iMap), Me.m_SpaceData.CapMapVarIndex(iMap))
                    ElseIf (tElm Is GetType(Single)) Then
                        map = New cEnviroInputMap(Of Single)(Me.m_core, Me.m_SpaceData.CapMapDBID(iMap), iMap, Me.m_SpaceData.CapMapName(iMap), _
                                                             Me, DirectCast(data, Single(,)), Me.m_SpaceData.CapMapVariable(iMap), Me.m_SpaceData.CapMapVarIndex(iMap))
                    Else
                        ' Not supported
                    End If

                    DirectCast(map, cCoreInputOutputBase).AllowValidation = False
                    For iGroup As Integer = 1 To Me.m_SpaceData.NGroups
                        map.ResponseIndexForGroup(iGroup) = Me.m_SpaceData.CapMapFunctions(iMap, iGroup)
                    Next
                    DirectCast(map, cCoreInputOutputBase).AllowValidation = True

                    If (map IsNot Nothing) Then
                        Me.m_maps.Add(map)
                    End If

                Catch ex As Exception
                    Debug.Assert(False, "InitAndLoadCapacityMaps Error: " & ex.Message)
                    bSuccess = False
                End Try

            Next iMap

            Me.m_SpaceData.CapMaps = Me.m_maps.ToArray

            'update the maps with the newly loaded data
            Me.Update()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Load() Exception: " & ex.Message)
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    Friend Function onChanged() As Boolean
        Try
            For Each map As IEnviroInputMap In Me.m_maps
                For iGroup As Integer = 1 To Me.m_SpaceData.NGroups
                    Me.m_SpaceData.CapMapFunctions(DirectCast(map, cCoreInputOutputBase).Index, iGroup) = map.ResponseIndexForGroup(iGroup)
                Next
            Next

            Me.m_core.onChanged(Me, eMessageType.DataModified)
            Return True
        Catch ex As Exception

        End Try
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update all the maps in the manager.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub Update()
        Try

            For Each map As IEnviroInputMap In Me.m_maps
                map.Update()
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Exception: " & ex.Message)
        End Try

    End Sub

    Friend Sub Clear()

        For Each map As IEnviroInputMap In Me.m_maps
            DirectCast(map, cCoreInputOutputBase).Dispose()
        Next
        Me.m_maps.Clear()

    End Sub

    Friend ReadOnly Property MediationData() As cMediationDataStructures
        Get
            Return Me.m_MedData
        End Get
    End Property

    Friend ReadOnly Property SpaceData() As cEcospaceDataStructures
        Get
            Return Me.m_core.m_EcoSpaceData
        End Get
    End Property

#End Region ' Friend interfaces

#Region " ICoreInterface Implementation "

    Public ReadOnly Property CoreComponent() As EwEUtils.Core.eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            'I've add a new eCoreComponentType for Map and Response functions manager (eCoreComponentType.MapResponseInteractionManager) 
            'as well as a new Datatype for IEnviroInputMap (eDataTypes.MapResponse)
            Return EwEUtils.Core.eCoreComponentType.MapResponseInteractionManager
        End Get
    End Property

    Public ReadOnly Property DataType() As EwEUtils.Core.eDataTypes Implements ICoreInterface.DataType
        Get
            '
            Return EwEUtils.Core.eDataTypes.EcospaceMapResponse
        End Get
    End Property

    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            ' NOP
        End Set
    End Property

    Public Function GetID() As String Implements ICoreInterface.GetID
        Return cCore.NULL_VALUE.ToString
    End Function

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)

        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return Me.ToString
        End Get
        Set(ByVal value As String)

        End Set
    End Property

#End Region

End Class
