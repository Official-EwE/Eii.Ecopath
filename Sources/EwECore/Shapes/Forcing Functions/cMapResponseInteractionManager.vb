


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

        'Set the manager of each map in the list
        'this was not done by Ecospace because it does not have an instance of the core or the manager
        'it could be done by the core but I think it makes more sense for the manager to do it
        'Kind of hack as the maps should already know this but it's not the simple
        For Each map As IEnviroInputMap In Me.m_maps
            map.setManager(Me)
        Next
    End Sub


    Public ReadOnly Property nMaps() As Integer
        Get
            Return Me.m_maps.Count
        End Get
    End Property


    Public ReadOnly Property Maps(ByVal MapIndex As Integer) As IEnviroInputMap
        Get
            If MapIndex > 0 And MapIndex <= Me.m_maps.Count Then
                Return Me.m_maps(MapIndex - 1)
            End If
            Return Nothing
        End Get

    End Property

    Public Function update() As Boolean
        Try
            Me.m_core.onChanged(Me, eMessageType.DataModified)
            Return True
        Catch ex As Exception

        End Try
        Return False
    End Function

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
            Return EwEUtils.Core.eDataTypes.MapResponse
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
