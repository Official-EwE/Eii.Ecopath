Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Results, over all the time steps, at the end of an ecospace model run
''' </summary>
''' <remarks></remarks>
Public Class cEcospaceGroupOutput
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal iGroup As Integer)
        MyBase.New(TheCore)

        Dim val As cValue = Nothing

        Me.DBID = iGroup '????
        Me.Index = iGroup
        Me.m_DataType = eDataTypes.EcospaceBiomassResults
        'no validators

        'jb It would be nice to include the number of timestep as a property of this class
        'however, this it would have to be readonly and cCoreInputOutputBase.SetVariable() would violate this
        'it will have to be retrieved from cCore.nEcospaceTimeSteps or cCore.getCounter()


        ' Biomass over all the time steps
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceBiomassOverTime, eStatusFlags.OK, eCoreCounterTypes.nEcospaceTimeSteps, _
                                AddressOf m_core.GetCoreCounter, Nothing, TheCore.m_validators.getValidator(eVarNameFlags.EcospaceBiomassOverTime))
        m_values.Add(val.varName, val)


        ' Relative Biomass over all the time steps
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceRelativeBiomassOverTime, eStatusFlags.OK, eCoreCounterTypes.nEcospaceTimeSteps, _
                                 AddressOf m_core.GetCoreCounter, Nothing, TheCore.m_validators.getValidator(eVarNameFlags.EcospaceBiomassOverTime))
        m_values.Add(val.varName, val)

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


 
#End Region

#Region "Properties via dot '.' operator"

    Public Property Biomass(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceBiomassOverTime, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceBiomassOverTime, value, iTime)
        End Set

    End Property

    Public Property RelativeBiomass(ByVal iTime As Integer) As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceRelativeBiomassOverTime, iTime))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceRelativeBiomassOverTime, value, iTime)
        End Set

    End Property


#End Region

#Region "Status via dot '.' operator"

    Public Property BiomassStatus(ByVal iTime As Integer) As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.EcospaceBiomassOverTime, iTime)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceBiomassOverTime, value, iTime)
        End Set

    End Property

    Public Property RelativeBiomassStatus(ByVal iTime As Integer) As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.EcospaceRelativeBiomassOverTime, iTime)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceRelativeBiomassOverTime, value, iTime)
        End Set

    End Property


#End Region

End Class
