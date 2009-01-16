'==============================================================================
'
' $Log: cEcotracerRegionGroupOutput.vb,v $
' Revision 1.2  2009/01/16 18:30:25  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:10  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/05/29 22:22:46  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.3  2008/03/26 21:00:56  joeb
' Added CBEnvironment
'
' Revision 1.2  2007/12/08 00:55:50  jeroens
' + Added time dimension
'
' Revision 1.1  2007/12/07 21:44:37  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcotracerRegionGroupOutput
    Inherits cCoreInputOutputBase

    Private m_data(,,) As Single
    Private m_cb(,,) As Single
    Private m_nRegions As Integer = 0
    Private m_nGroups As Integer = 0
    Private m_nTimeSteps As Integer = 0

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore)
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
        Me.m_nRegions = TheCore.GetCoreCounter(eCoreCounterTypes.nRegions)
        Me.m_nTimeSteps = TheCore.GetCoreCounter(eCoreCounterTypes.nEcospaceTimeSteps)

        ReDim m_data(Me.m_nRegions, Me.m_nGroups + 1, Me.m_nTimeSteps)
        ReDim m_cb(Me.m_nRegions, Me.m_nGroups + 1, Me.m_nTimeSteps)

    End Sub

#End Region

#Region "Implementation of GetVariable() GetVariable() GetStatus() SetStatus()"

    Public Overloads Function GetVariable(ByVal varName As eVarNameFlags, ByVal iRegion As Integer, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Single

        Try
            Select Case varName
                Case eVarNameFlags.Concentration
                    Return m_data(iRegion, iGroup, iTimeStep)
                Case eVarNameFlags.CEnvironment
                    Return m_data(iRegion, 0, iTimeStep)
                Case eVarNameFlags.CBEnvironment
                    Return m_cb(iRegion, 0, iTimeStep)
                Case eVarNameFlags.CSum
                    Return m_data(iRegion, Me.m_nGroups + 1, iTimeStep)
                Case eVarNameFlags.ConcBio
                    Return m_cb(iRegion, iGroup, iTimeStep)
            End Select

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function

    Public Overloads Function SetVariable(ByVal varName As eVarNameFlags, ByVal newValue As Single, ByVal iRegion As Integer, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Boolean

        Try

            Select Case varName
                Case eVarNameFlags.Concentration
                    m_data(iRegion, iGroup, iTimeStep) = newValue
                Case eVarNameFlags.CEnvironment
                    m_data(iRegion, 0, iTimeStep) = newValue
                Case eVarNameFlags.CSum
                    m_data(iRegion, Me.m_nGroups + 1, iTimeStep) = newValue
                Case eVarNameFlags.ConcBio
                    m_cb(iRegion, iGroup, iTimeStep) = newValue
                Case eVarNameFlags.CBEnvironment
                    m_cb(iRegion, 0, iTimeStep) = newValue
            End Select

            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Public Overloads Function GetStatus(ByVal varName As eVarNameFlags, ByVal iRegion As Integer, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As eStatusFlags
        Return eStatusFlags.OK Or eStatusFlags.NotEditable
    End Function

    Public Overloads Function SetStatus(ByVal varName As eVarNameFlags, ByVal newValue As eStatusFlags, ByVal iRegion As Integer, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Boolean
        Debug.Assert(False, "Not implemented yet.")
    End Function
#End Region

#Region "Variable via dot '.' operator"

    Public Property Concentration(ByVal iRegion As Integer, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Single
        Get
            Try
                Return GetVariable(eVarNameFlags.Concentration, iRegion, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.Concentration, value, iRegion, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property


    Public Property CB(ByVal iRegion As Integer, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As Single
        Get
            Try
                Return GetVariable(eVarNameFlags.ConcBio, iRegion, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.ConcBio, value, iRegion, iGroup, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property

    Public Property CEnvironment(ByVal iRegion As Integer, ByVal iTimeStep As Integer) As Single
        Get
            Try
                Return GetVariable(eVarNameFlags.CEnvironment, iRegion, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.CEnvironment, value, iRegion, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property


    Public Property CBEnvironment(ByVal iRegion As Integer, ByVal iTimeStep As Integer) As Single

        Get
            Try
                Return GetVariable(eVarNameFlags.CBEnvironment, iRegion, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try
        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.CBEnvironment, value, iRegion, 0, iTimeStep)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

    End Property

    'Public Property CSum(ByVal iRegion As Integer, ByVal iTimeStep As Integer) As Single
    '    Get
    '        Try
    '            Return GetVariable(eVarNameFlags.CEnvironment, iRegion, Me.m_nGroups + 1, iTimeStep)
    '        Catch ex As Exception
    '            Debug.Assert(False, ex.Message)
    '            Return cCore.NULL_VALUE
    '        End Try

    '    End Get

    '    Set(ByVal value As Single)
    '        Try
    '            SetVariable(eVarNameFlags.CEnvironment, value, iRegion, Me.m_nGroups + 1, iTimeStep)
    '        Catch ex As Exception
    '            Debug.Assert(False, ex.Message)
    '        End Try
    '    End Set
    'End Property

#End Region

#Region "Status Flags via dot '.' operator"

    Public Property ConcentrationStatus(ByVal iRegion As Integer, ByVal iGroup As Integer, ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.Concentration, iRegion, iGroup, iTimeStep)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Concentration, value, iRegion, iGroup, iTimeStep)
        End Set
    End Property

    Public Property CEnvironmentStatus(ByVal iRegion As Integer, ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.CEnvironment, iRegion, 0, iTimeStep)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CEnvironment, value, iRegion, 0, iTimeStep)
        End Set
    End Property

    Public Property CSumStatus(ByVal iRegion As Integer, ByVal iTimeStep As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.CSum, iRegion, Me.m_nGroups + 1, iTimeStep)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.CSum, value, iRegion, Me.m_nGroups + 1, iTimeStep)
        End Set
    End Property

#End Region

End Class
