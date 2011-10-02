#Region " Imports "

Option Strict On

Imports EwEUtils.Core

#End Region ' Imports

Public Class cMapResponseInteractionManager
    Inherits cCoreInputOutputBase

#Region "Private data"

    Private m_maps As List(Of IEnviroInputMap)
    Private m_SpaceData As cEcospaceDataStructures
    Private m_MedData As cMediationDataStructures

#End Region

#Region " Constructor "

    Public Sub New(ByVal core As cCore)
        MyBase.New(core)
        Me.m_coreComponent = eCoreComponentType.MapResponseInteractionManager
        Me.m_dataType = eDataTypes.EcospaceMapResponse
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

#End Region ' Public Methods and Properties

#Region " Friend interfaces "

    Friend Sub Init(ByVal spaceData As cEcospaceDataStructures, ByVal MediationData As cMediationDataStructures)
        Me.m_SpaceData = spaceData
        Me.m_MedData = MediationData
        Me.m_maps = New List(Of IEnviroInputMap)
    End Sub

    Friend Function Load() As Boolean

        Dim map As IEnviroInputMap = Nothing
        Dim layer As cEcospaceLayer = Nothing
        Dim bSuccess As Boolean = True

        Me.AllowValidation = False
        Try

            Me.m_maps.Clear()

            ' Hard-code the depth map at position 0
            layer = Me.m_core.EcospaceBasemap.LayerDepth()
            map = New cEnviroInputMap(Me.m_core.CapacitMapInteractionManager, layer)
            For iGroup As Integer = 1 To Me.m_SpaceData.NGroups
                map.ResponseIndexForGroup(iGroup) = Me.m_SpaceData.CapMapFunctions(0, iGroup)
            Next
            Me.m_maps.Add(map)

            'populate the list of IEnviroInputMap objects that the user will interact with 
            'to change region related parameters from the interface
            For iMap As Integer = 1 To Me.m_SpaceData.nDriverLayers
                Try

                    layer = Me.m_core.EcospaceBasemap.LayerDriver(iMap)
                    map = New cEnviroInputMap(Me.m_core.CapacitMapInteractionManager, layer)
                    For iGroup As Integer = 1 To Me.m_SpaceData.NGroups
                        map.ResponseIndexForGroup(iGroup) = Me.m_SpaceData.CapMapFunctions(iMap, iGroup)
                    Next
                    Me.m_maps.Add(map)

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
        Me.AllowValidation = True

        Return bSuccess

    End Function

    Friend Function onChanged() As Boolean

        Try

            For iMap As Integer = 1 To Me.m_SpaceData.nDriverLayers
                For iGroup As Integer = 1 To Me.m_SpaceData.NGroups
                    Me.m_SpaceData.CapMapFunctions(iMap, iGroup) = Me.Map(iMap).ResponseIndexForGroup(iGroup)
                Next
            Next

            If Me.AllowValidation Then
                Me.m_core.onChanged(Me, eMessageType.DataModified)
            End If

        Catch ex As Exception
            Return False
        End Try
        Return True

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

    Public Overrides Sub Clear()
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

End Class
