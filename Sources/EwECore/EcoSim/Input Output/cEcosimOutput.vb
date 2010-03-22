Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcosimOutput
    Inherits cCoreInputOutputBase

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)

        Me.DBID = cCore.NULL_VALUE
        Me.m_dataType = eDataTypes.EcosimOutput
        Me.m_coreComponent = eCoreComponentType.EcoSim

    End Sub

    ''' <summary>
    ''' Get/set the fishing in-balance (FIB) index.
    ''' </summary>
    Public ReadOnly Property FIB(ByVal iTimeStep As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.FIB, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property FIBStatus(ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.FIB, iTimeStep)
        End Get
    End Property

    Public ReadOnly Property TLCatch(ByVal iTimeStep As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.TLCatch, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property TLCatchStatus(ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.TLCatch, iTimeStep)
        End Get
    End Property

    Public ReadOnly Property TotalCatch(ByVal iTimeStep As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.TotalCatch, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property TotalCatchStatus(ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.TotalCatch, iTimeStep)
        End Get
    End Property

    Public ReadOnly Property KemptonsQ(ByVal iTimeStep As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.KemptonsQ, iTimeStep))
        End Get
    End Property

    Public ReadOnly Property KemptonsQStatus(ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.KemptonsQ, iTimeStep)
        End Get
    End Property

    Public Overrides Function GetVariable(ByVal VarName As EwEUtils.Core.eVarNameFlags, _
                                          Optional ByVal iIndex As Integer = -9999, _
                                          Optional ByVal iIndex2 As Integer = -9999, _
                                          Optional ByVal iIndex3 As Integer = -9999) As Object

        Try

            Select Case VarName
                Case eVarNameFlags.FIB
                    Return Me.m_core.m_EcoSimData.FIB(iIndex)
                Case eVarNameFlags.TLCatch
                    Return Me.m_core.m_EcoSimData.TLC(iIndex)
                Case eVarNameFlags.KemptonsQ
                    Return Me.m_core.m_EcoSimData.Kemptons(iIndex)
                Case eVarNameFlags.TotalCatch
                    Return Me.m_core.m_EcoSimData.CatchSim(iIndex)

            End Select

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function

    Public Overrides Function GetStatus(ByVal VarName As EwEUtils.Core.eVarNameFlags, Optional ByVal iIndex As Integer = -9999) As eStatusFlags
        Return eStatusFlags.NotEditable And eStatusFlags.ValueComputed
    End Function

End Class
