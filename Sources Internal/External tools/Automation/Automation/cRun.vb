Imports EwECore
Imports EwEUtils.Core

Public Class cRun

    Public Class cEcosim

        Public Property Scenario As String = ""
        Public Property TimeSeries As String = ""
        Public Property AutosaveResults As Boolean = False
        Public Property RunYears As Integer = 0

    End Class

    Public Class cEcospace

        Friend m_connections As New List(Of cSpatTempConnection)

        Public Property Scenario As String = ""
        Public Property AutosaveBiomassMaps As Boolean = False
        Public Property AutosaveCatchMaps As Boolean = False
        Public Property AutosaveEffortMaps As Boolean = False
        Public Property ExternalDataConfigFile As String = ""
        Public Property SpinupYears As Integer = cCore.NULL_VALUE
        Public Property RunYears As Integer = 0

        Public Sub ClearDrivers()
            Me.m_connections.Clear()
        End Sub

        Public Sub DriveAbsoluteBiomass(DatasetName As String, LayerName As String, Optional Scalar As Single = -9999)
            Me.m_connections.Add(New cSpatTempConnection(DatasetName, eVarNameFlags.LayerBiomassForcing, LayerName, Scalar))
        End Sub

        Public Sub DriveRelativeBiomass(DatasetName As String, LayerName As String, Optional Scalar As Single = -9999)
            Me.m_connections.Add(New cSpatTempConnection(DatasetName, eVarNameFlags.LayerBiomassForcing, LayerName, Scalar))
        End Sub

        Public Sub DriveRelativePP(DatasetName As String, Scalar As Single)
            Me.m_connections.Add(New cSpatTempConnection(DatasetName, eVarNameFlags.LayerRelPP, "", Scalar))
        End Sub

        Public Sub DriveEnvironment(DatasetName As String, LayerName As String, Optional Scalar As Single = -9999)
            Me.m_connections.Add(New cSpatTempConnection(DatasetName, eVarNameFlags.LayerDriver, LayerName, Scalar))
        End Sub

        Public Sub DriveCapacity(DatasetName As String, LayerName As String, Optional Scalar As Single = -9999)
            Me.m_connections.Add(New cSpatTempConnection(DatasetName, eVarNameFlags.LayerHabitatCapacityInput, LayerName, Scalar))
        End Sub

        Public Sub DriveMPA(DatasetName As String, LayerName As String)
            Me.m_connections.Add(New cSpatTempConnection(DatasetName, eVarNameFlags.LayerMPA, LayerName, 1))
        End Sub

        Public Sub DriveHabitat(DatasetName As String, LayerName As String)
            Me.m_connections.Add(New cSpatTempConnection(DatasetName, eVarNameFlags.LayerHabitat, LayerName, 1))
        End Sub

        Public ReadOnly Property Connections As cSpatTempConnection()
            Get
                Return Me.m_connections.ToArray()
            End Get
        End Property

    End Class

    Public Sub New(Name As String)
        Me.Name = Name
    End Sub

    Public Property Name As String = ""
    Public Property Model As String = ""
    Public ReadOnly Property Ecospace As New cEcospace()
    Public ReadOnly Property Ecosim As New cEcosim()
    Public Property MPADynamicsFile As String = ""
    Public Property EcoIndEnabled As Boolean = False

    Public Function Copy(NewName As String) As cRun

        Dim r2 As New cRun(NewName)

        r2.Model = Me.Model

        r2.Ecosim.Scenario = Me.Ecosim.Scenario
        r2.Ecosim.TimeSeries = Me.Ecosim.TimeSeries

        r2.Ecospace.Scenario = Me.Ecospace.Scenario
        r2.Ecospace.RunYears = Me.Ecospace.RunYears
        r2.Ecospace.SpinupYears = Me.Ecospace.SpinupYears
        r2.Ecospace.AutosaveBiomassMaps = Me.Ecospace.AutosaveBiomassMaps
        r2.Ecospace.AutosaveCatchMaps = Me.Ecospace.AutosaveCatchMaps
        r2.Ecospace.AutosaveEffortMaps = Me.Ecospace.AutosaveEffortMaps
        r2.Ecospace.ExternalDataConfigFile = Me.Ecospace.ExternalDataConfigFile
        r2.Ecospace.m_connections.AddRange(Me.Ecospace.m_connections)

        r2.MPADynamicsFile = Me.MPADynamicsFile
        r2.EcoIndEnabled = Me.EcoIndEnabled

        Return r2
    End Function

End Class
