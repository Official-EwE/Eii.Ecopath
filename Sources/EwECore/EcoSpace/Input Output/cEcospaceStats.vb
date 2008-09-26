Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Statistics for the last Ecospace run.
''' </summary>
''' <remarks>One object for all the groups and stats</remarks>
Public Class cEcospaceStats
    Inherits cCoreInputOutputBase

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Me.DBID = DBID
        m_DataType = eDataTypes.EcospaceGroup
        m_messageSource = eMessageSource.EcoSpace

        Dim val As cValue

        Try

            m_DataType = eDataTypes.EcospaceGroup
            m_messageSource = eMessageSource.EcoSpace

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimGroupInput, eMessageSource.EcoSim, Index, cCore.NULL_VALUE)
            'SS
            val = New cValue(New Single, eVarNameFlags.EcospaceSS, eStatusFlags.Null, eValueTypes.Sng)
            m_values.Add(val.varName, val)

            'Region SS
            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionSS, eStatusFlags.NotEditable, eCoreCounterTypes.nRegions, _
                         AddressOf m_core.GetCoreCounter)
            m_values.Add(val.varName, val)

            'set status flags to their default values
            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceGroup.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceGroup. Error: " & ex.Message)
        End Try

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        'tell the base class to do the default values
        MyBase.ResetStatusFlags(bForceReset)

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.PointArray, eValueTypes.BoolArray
                        For i = 0 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Sng, eValueTypes.Int
                        value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed

                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function


    Public Property SS() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceSS))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceSS, value)
        End Set
    End Property


    Public Property RegionSS(ByVal iRegion As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceRegionSS, iRegion))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceRegionSS, value, iRegion)
        End Set
    End Property


    Public Property SSStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcospaceSS)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceSS, value)
        End Set
    End Property


    Public Property RegionSSStatus(ByVal iRegion As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcospaceRegionSS, iRegion)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceRegionSS, value, iRegion)
        End Set
    End Property






End Class
