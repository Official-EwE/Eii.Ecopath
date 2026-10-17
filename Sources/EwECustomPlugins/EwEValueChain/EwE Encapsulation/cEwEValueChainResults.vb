Imports ValueChain

Public Class cEwEValueChainResults
    Inherits cValueChainResults

    Private m_runType As cEwEValueChainModel.eRunTypes = cEwEValueChainModel.eRunTypes.Ecopath

    Public Sub New(data As cEwEValueChainData)
        MyBase.New(data)
    End Sub

    Public Property RunType As cEwEValueChainModel.eRunTypes
        Get
            Return Me.m_runType
        End Get
        Private Set(value As cEwEValueChainModel.eRunTypes)
            Me.m_runType = value
        End Set
    End Property

    Public Overloads Sub Reset(nFleets As Integer, nGroups As Integer, nTimesteps As Integer, runtype As cEwEValueChainModel.eRunTypes)
        MyBase.Reset(nFleets, nGroups, nTimesteps)
        Me.RunType = runtype
    End Sub

End Class
