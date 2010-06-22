Imports EwECore

Public Class cPredatorPreySelection

#Region "Private fields"

    Private m_Predator As String
    Private m_Prey As List(Of String)
    Private m_core As cCore

#End Region

#Region "Constructor(s)"

    Public Sub New(ByRef Predator As String)
        Me.m_core = cCore.GetInstance()
        m_Predator = Predator
        m_Prey = New List(Of String)
    End Sub

#End Region

#Region "Properties"

    Public Property PredatorName() As String
        Get
            Return m_Predator
        End Get
        Set(ByVal value As String)
            m_Predator = value
        End Set
    End Property

    Public Property PreyName(ByVal i As Integer) As String
        Get
            Return m_Prey(i)
        End Get
        Set(ByVal value As String)
            m_Prey(i) = value
        End Set
    End Property

#End Region

#Region "Subroutines"

    Public Sub AddPrey(ByVal PreyName As String)
        m_Prey.Add(PreyName)
    End Sub

    Public Sub RemovePrey(ByVal i As Integer)
        m_Prey.RemoveAt(i)
    End Sub

#End Region

#Region "Functions"

    Public Function CountPrey() As Integer
        Return m_Prey.Count
    End Function

    Public Function GetIndexPredatorForEcoSim() As Integer
        Dim PredIndexEcosim As Integer = 1

        While m_core.EcoSimGroupOutputs(PredIndexEcosim).Name <> m_Predator
            PredIndexEcosim += 1
        End While
        Return PredIndexEcosim

    End Function

    Public Function GetIndexPreyForEcoSim(ByVal i As Integer) As Integer
        Dim PreyIndexEcosim As Integer = 1

        While m_core.EcoSimGroupOutputs(PreyIndexEcosim).Name <> m_Prey(i)
            PreyIndexEcosim += 1
        End While
        Return PreyIndexEcosim

    End Function

#End Region


End Class

