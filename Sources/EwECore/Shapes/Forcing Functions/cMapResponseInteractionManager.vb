


Public Class cMapResponseInteractionManager

    Dim m_maps As List(Of IEnviroInputMap)
    Dim m_MedData As cMediationDataStructures

    Public Sub Init(ByVal MediationData As cMediationDataStructures, ByVal InputMaps As List(Of IEnviroInputMap))
        Me.m_maps = InputMaps
        Me.m_MedData = MediationData
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


End Class
