


Public Class cMapResponseInteractionManager
    Implements ICoreInterface

#Region "Private data"

    Private m_maps As List(Of IEnviroInputMap)
    Private m_MedData As cMediationDataStructures
    Private m_core As cCore

#End Region

#Region "Public Methods and Properties"

    Public Sub Init(ByVal theCore As cCore, ByVal MediationData As cMediationDataStructures, ByVal InputMaps As List(Of IEnviroInputMap))
        Me.m_maps = InputMaps
        Me.m_MedData = MediationData
        Me.m_core = theCore

    End Sub

    Public Sub Load()
        Try

            'Set the manager of each map in the list
            'this was not done by Ecospace because it does not have an instance of the core or the manager
            'it could be done by the core but I think it makes more sense for the manager to do it
            'Kind of hack as the maps should already know this but it's not the simple
            For Each map As IEnviroInputMap In Me.m_maps
                map.setManager(Me)
            Next

            'update the maps with the newly loaded data
            Me.Update()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Load() Exception: " & ex.Message)
        End Try

    End Sub

    Public Sub Update()
        Try

            For Each map As IEnviroInputMap In Me.m_maps
                map.Update()
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Exception: " & ex.Message)
        End Try

    End Sub


    Public ReadOnly Property nMaps() As Integer
        Get
            Return Me.m_maps.Count
        End Get
    End Property


    Public ReadOnly Property Map(ByVal MapIndex As Integer) As IEnviroInputMap
        Get
            If MapIndex > 0 And MapIndex <= Me.m_maps.Count Then
                Return Me.m_maps(MapIndex - 1)
            End If
            Return Nothing
        End Get

    End Property

    Public Function onChanged() As Boolean
        Try
            Me.m_core.onChanged(Me, eMessageType.DataModified)
            Return True
        Catch ex As Exception

        End Try
        Return False
    End Function

    Public Function AddMap(ByVal dataArray(,) As Single, ByVal MapName As String) As Boolean
        Dim breturn As Boolean
        Try

            'Create a map from the input data
            'this will init the map to the data in the Manager
            Dim map As New cEnviroInputMap(Of Single)(Me.m_core, Me, dataArray)

            map.AllowValidation = False
            map.Name = MapName
            map.AllowValidation = True

            'add the new map to the list of maps
            Me.m_maps.Add(map)

            'tell the core that the MapResponse  data has change
            Me.onChanged()

            breturn = True

        Catch ex As Exception
            breturn = False
            Debug.Assert(False, Me.ToString & ".AddMap() ")
        End Try

        Return breturn

    End Function


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


#End Region

#Region "ICoreInterface Implementation"

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
