Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcotracerGroupOutput
    Inherits cCoreInputOutputBase

    Private m_TracerData As cContaminantTracerDataStructures

#Region "Constructor"

    Public Sub New(ByVal TheCore As cCore)
        MyBase.New(TheCore)

        'Dim val As cValue
        Me.m_dataType = eDataTypes.EcotracerSimOutput
        Me.m_coreComponent = eCoreComponentType.Ecotracer
        Me.m_TracerData = Me.m_core.m_tracerData

        Me.DBID = 1
        Me.Index = 1

        '' Add dummy values
        'val = New cValue()
        'Me.m_values.Add(eVarNameFlags.Concentration, val)
        'Me.m_values.Add(eVarNameFlags.CEnvironment, val)
        'Me.m_values.Add(eVarNameFlags.CSum, val)

    End Sub

    ''' <inheritdoc cref="cCoreInputOutputBase.Dispose"/>
    Public Overrides Sub Dispose()
        MyBase.Dispose()
        Me.Clear()
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
                    Return Me.m_TracerData.TracerConc(iGroup, iTimeStep)
                Case eVarNameFlags.CEnvironment
                    'environment data stored in zero group
                    Return Me.m_TracerData.TracerConc(0, iTimeStep)
            End Select
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function

    Public Overloads Function SetVariable(ByVal varName As eVarNameFlags, ByVal newValue As Single, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Boolean
        Try
            Debug.Assert(False, Me.ToString & " variable " & varName.ToString & " is ReadOnly.")
            Return False

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

#End Region

End Class
