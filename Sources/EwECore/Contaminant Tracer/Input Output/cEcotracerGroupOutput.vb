Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcotracerGroupOutput
    Inherits cCoreInputOutputBase

    Private m_data(,) As Single
    Private m_nGroups As Integer = 0
    Private m_nTimeSteps As Integer = 0

#Region "Constructor"

    Public Sub New(ByVal TheCore As cCore)
        MyBase.New(TheCore)

        Dim val As cValue
        Me.m_dataType = eDataTypes.EcotracerSimOutput
        Me.m_coreComponent = eCoreComponentType.Ecotracer

        Me.DBID = 1
        Me.Index = 1

        ' Add dummy values
        val = New cValue()
        Me.m_values.Add(eVarNameFlags.Concentration, val)
        Me.m_values.Add(eVarNameFlags.CEnvironment, val)
        Me.m_values.Add(eVarNameFlags.CSum, val)

        Me.m_nGroups = TheCore.GetCoreCounter(eCoreCounterTypes.nGroups)
        Me.m_nTimeSteps = TheCore.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
        ReDim m_data(Me.m_nGroups + 1, Me.m_nTimeSteps)

    End Sub

    ''' <inheritdoc cref="cCoreInputOutputBase.Dispose"/>
    Public Overrides Sub Dispose()
        MyBase.Dispose()
        Me.Clear()
        Me.m_data = Nothing
    End Sub

    ''' <inheritdoc cref="cCoreInputOutputBase.Clear"/>
    Public Overrides Sub Clear()
        MyBase.Clear()
    End Sub

#End Region

#Region "Implementation of GetVariable() GetVariable() GetStatus() SetStatus()"

    Public Overloads Function GetVariable(ByVal varName As eVarNameFlags, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Single
        Try
            Select Case varName
                Case eVarNameFlags.Concentration
                    Return m_data(iGroup, iTimeStep)
                Case eVarNameFlags.CEnvironment
                    Return m_data(0, iTimeStep)
                Case eVarNameFlags.CSum
                    Return m_data(Me.m_nGroups + 1, iTimeStep)
            End Select
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function

    Public Overloads Function SetVariable(ByVal varName As eVarNameFlags, ByVal newValue As Single, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Boolean
        Try
            Select Case varName
                Case eVarNameFlags.Concentration
                    m_data(iGroup, iTimeStep) = newValue
                Case eVarNameFlags.CEnvironment
                    m_data(0, iTimeStep) = newValue
                Case eVarNameFlags.CSum
                    m_data(Me.m_nGroups + 1, iTimeStep) = newValue
            End Select

            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Public Overloads Function GetStatus(ByVal varName As eVarNameFlags, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As eStatusFlags
        Return eStatusFlags.OK Or eStatusFlags.NotEditable
    End Function

    Public Overloads Function SetStatus(ByVal varName As eVarNameFlags, ByVal newValue As eStatusFlags, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Boolean
        Debug.Assert(False, "Not implemented yet.")
    End Function
#End Region

#Region "Variable via dot '.' operator"

    Public Property Concentration(ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Single
        Get
            Try
                Return GetVariable(eVarNameFlags.Concentration, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.Concentration, value, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property

    Public Property CEnvironment(ByVal iTimeStep As Integer) As Single
        Get
            Try
                Return GetVariable(eVarNameFlags.CEnvironment, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.CEnvironment, value, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property

    Public Property CSum(ByVal iTimeStep As Integer) As Single
        Get
            Try
                Return GetVariable(eVarNameFlags.CEnvironment, Me.m_nGroups + 1, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.CEnvironment, value, Me.m_nGroups + 1, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property

#End Region

#Region "Status Flags via dot '.' operator"

    Public Property ConcentrationStatus(ByVal iGroup As Integer, ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.Concentration, iGroup, iTimeStep)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Concentration, value, iGroup, iTimeStep)
        End Set
    End Property

    Public Property CEnvironmentStatus(ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.CEnvironment, 0, iTimeStep)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CEnvironment, value, 0, iTimeStep)
        End Set
    End Property

    Public Property CSumStatus(ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.CSum, Me.m_nGroups + 1, iTimeStep)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CSum, value, Me.m_nGroups + 1, iTimeStep)
        End Set
    End Property

#End Region

End Class
