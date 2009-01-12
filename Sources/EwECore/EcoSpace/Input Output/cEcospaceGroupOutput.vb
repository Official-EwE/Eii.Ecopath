Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Results, over all the time steps, at the end of an ecospace model run
''' </summary>
''' <remarks></remarks>
Public Class cEcospaceGroupOutput
    Inherits cCoreInputOutputBase

    Private m_spaceData As cEcospaceDataStructures
    Private m_Vars As New Dictionary(Of eVarNameFlags, IResultsWrapper)

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal EcoSpaceData As cEcospaceDataStructures, ByVal iGroup As Integer)
        MyBase.New(TheCore)

        Dim val As cValue = Nothing

        Me.DBID = iGroup '????
        Me.Index = iGroup
        Me.m_DataType = eDataTypes.EcospaceGroupOuput

        m_spaceData = EcoSpaceData

        'Data is loaded in Init
        'no validators


    End Sub


    Public Sub Init()

        m_Vars.Clear()
        'SpaceTSData(group,var,time)
        m_Vars.Add(eVarNameFlags.EcospaceBiomassOverTime, New c3DResultsWrapper2Fixed(m_spaceData.ResultsByGroup, eSpaceResultsGroups.Biomass, Me.Index))
        m_Vars.Add(eVarNameFlags.EcospaceRelativeBiomassOverTime, New c3DResultsWrapper2Fixed(m_spaceData.ResultsByGroup, eSpaceResultsGroups.RelativeBiomass, Me.Index))

    End Sub

#End Region

#Region "Data Validation and Status flag setting"
    'Status of ouput should be set to eStatusFlags.NotEditable Or eStatusFlags.Null for all timesteps that are not computed 
    'Once the data has be populate with the results from the last model run status should be set to eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
    'This allows an interface to tell if the data at a timestep has been populated by the model run


    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray
                        For i = 1 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Str

                        If CStr(value.Value) = "" Then
                            value.Status = eStatusFlags.NotEditable Or eStatusFlags.Null
                        Else
                            value.Status = eStatusFlags.NotEditable Or eStatusFlags.OK
                        End If

                    Case Else
                        value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                End Select

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function

    Public Overrides Function GetVariable(ByVal VarName As EwEUtils.Core.eVarNameFlags, Optional ByVal iIndex1 As Integer = -9999, Optional ByVal iIndex2 As Integer = -9999) As Object

        If Not m_Vars.ContainsKey(VarName) Then
            'NOT in list of sim vars so get the value from the base class GetVariable(...)
            Return MyBase.GetVariable(VarName, iIndex1, iIndex2)
        Else
            'Varname is access directly via the core data
            Return m_Vars.Item(VarName).Value(iIndex1, iIndex2)
        End If

    End Function


    Public Overrides Function GetStatus(ByVal VarName As EwEUtils.Core.eVarNameFlags, Optional ByVal iIndex As Integer = -9999) As eStatusFlags

        If Not m_Vars.ContainsKey(VarName) Then
            'NOT in list of sim vars so get the value from the base class GetStatus(...)
            Return MyBase.GetStatus(VarName, iIndex)
        Else
            'all data managed by cEcospaceGroupOutput are read only outputs 
            Return eStatusFlags.NotEditable Or eStatusFlags.OK
        End If

    End Function


#End Region

#Region "Properties via dot '.' operator"

    Public ReadOnly Property Biomass(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceBiomassOverTime, iTime))
        End Get

    End Property

    Public ReadOnly Property RelativeBiomass(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceRelativeBiomassOverTime, iTime))
        End Get

    End Property

#End Region

    '#Region "Status via dot '.' operator"

    '    Public Property BiomassStatus(ByVal iTime As Integer) As eStatusFlags

    '        Get
    '            Return GetStatus(eVarNameFlags.EcospaceBiomassOverTime, iTime)
    '        End Get

    '        Friend Set(ByVal value As eStatusFlags)
    '            SetStatus(eVarNameFlags.EcospaceBiomassOverTime, value, iTime)
    '        End Set

    '    End Property

    '    Public Property RelativeBiomassStatus(ByVal iTime As Integer) As eStatusFlags

    '        Get
    '            Return GetStatus(eVarNameFlags.EcospaceRelativeBiomassOverTime, iTime)
    '        End Get

    '        Friend Set(ByVal value As eStatusFlags)
    '            SetStatus(eVarNameFlags.EcospaceRelativeBiomassOverTime, value, iTime)
    '        End Set

    '    End Property


    '#End Region

End Class
