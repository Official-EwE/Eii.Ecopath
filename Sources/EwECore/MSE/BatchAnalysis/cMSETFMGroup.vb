
#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region

Namespace MSE

    Public Class cMSETFMGroup
        Inherits cCoreGroupBase

        Private m_BLimValues() As Single
        Private m_BBaseValues() As Single
        Private m_FMaxValues() As Single
        Private m_BatchData As MSEBatchManager.cMSEBatchDataStructures

        Public Sub New(ByRef theCore As cCore, ByRef MSEBatchData As MSEBatchManager.cMSEBatchDataStructures, ByVal theGroupDBID As Integer)
            MyBase.New(theCore)

            Dim val As cValue
            Dim meta As cVariableMetaData

            Me.m_dataType = eDataTypes.MSEBatchTFMInput
            Me.m_coreComponent = eCoreComponentType.MSE
            Me.AllowValidation = False
            Me.DBID = theGroupDBID

            Me.m_BatchData = MSEBatchData

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSETFMBLimLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMBLimLower))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSETFMBLimUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMBLimUpper))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSETFMBBaseLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMBBaseLower))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSETFMBBaseUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMBBaseUpper))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSETFMFOptLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMFOptLower))
            m_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSETFMFOptUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMFOptUpper))
            m_values.Add(val.varName, val)


            'bBase
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEBBase, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBBase))
            m_values.Add(val.varName, val)
            'bLim
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEBLim, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEBLim))
            m_values.Add(val.varName, val)
            'FOpt
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEFmax, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEFmax))
            m_values.Add(val.varName, val)
            'Fmin
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSEFmin, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSEFmin))
            m_values.Add(val.varName, val)

            Me.AllowValidation = True

        End Sub


        Public Property BLim As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSEBLim))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEBLim, value)
            End Set
        End Property

        Public Property BLimLower As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSETFMBLimLower))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSETFMBLimLower, value)
            End Set
        End Property

        Public Property BLimUpper As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSETFMBLimUpper))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSETFMBLimUpper, value)
            End Set
        End Property

        Public Property BLimValue(IterationIndex As Integer) As Single
            Get
                ' Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTFM Then
                    Return Me.m_BLimValues(IterationIndex)
                End If
                'OH My.....
                Return cCore.NULL_VALUE
            End Get

            Set(ByVal value As Single)
                'Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTFM Then
                    Me.m_BLimValues(IterationIndex) = value
                End If
            End Set
        End Property


        Public Property BBase As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSEBBase))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEBBase, value)
            End Set
        End Property

        Public Property BBaseLower As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSETFMBBaseLower))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSETFMBBaseLower, value)
            End Set
        End Property

        Public Property BBaseUpper As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSETFMBBaseUpper))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSETFMBBaseUpper, value)
            End Set
        End Property



        Public Property BBaseValue(IterationIndex As Integer) As Single
            Get
                'Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTFM Then
                    Return Me.m_BBaseValues(IterationIndex)
                End If
                'OH My.....
                Return cCore.NULL_VALUE
            End Get

            Set(ByVal value As Single)
                'Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTFM Then
                    Me.m_BBaseValues(IterationIndex) = value
                End If
            End Set
        End Property


        Public Property FMax As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSEFmax))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSEFmax, value)
            End Set
        End Property

        Public Property FMaxLower As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSETFMFOptLower))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSETFMFOptLower, value)
            End Set
        End Property

        Public Property FMaxUpper As Single
            Get
                Return CSng(GetVariable(eVarNameFlags.MSETFMFOptUpper))
            End Get

            Set(ByVal value As Single)
                SetVariable(eVarNameFlags.MSETFMFOptUpper, value)
            End Set
        End Property



        Public Property FMaxValue(IterationIndex As Integer) As Single
            Get
                ' Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTFM Then
                    Return Me.m_FMaxValues(IterationIndex)
                End If
                'OH My.....
                Return cCore.NULL_VALUE
            End Get

            Set(ByVal value As Single)
                ' Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTFM Then
                    Me.m_FMaxValues(IterationIndex) = value
                End If
            End Set
        End Property

        Public Overrides Function GetVariable(VarName As EwEUtils.Core.eVarNameFlags, Optional iIndex As Integer = -9999, Optional iIndex2 As Integer = -9999, Optional iIndex3 As Integer = -9999) As Object

            Select Case VarName
                Case eVarNameFlags.MSETFMBLimValues
                    Return Me.BLimValue(Index)
                Case eVarNameFlags.MSETFMBBaseValues
                    Return Me.BBaseValue(Index)
                Case eVarNameFlags.MSETFMFOptValues
                    Return Me.FMaxValue(Index)
            End Select

            Return MyBase.GetVariable(VarName, iIndex, iIndex2, iIndex3)

        End Function


        Public Overrides Function SetVariable(VarName As EwEUtils.Core.eVarNameFlags, newValue As Object, Optional iSecondaryIndex As Integer = -9999) As Boolean

            Select Case VarName
                Case eVarNameFlags.MSETFMBLimValues
                    Me.BLimValue(iSecondaryIndex) = CSng(newValue)
                    Return True
                Case eVarNameFlags.MSETFMBBaseValues
                    Me.BBaseValue(Index) = CSng(newValue)
                    Return True
                Case eVarNameFlags.MSETFMFOptValues
                    Me.FMaxValue(Index) = CSng(newValue)
                    Return True
            End Select

            Return MyBase.SetVariable(VarName, newValue, iSecondaryIndex)

        End Function

        Public Sub SetToDefaults()

            Me.calcDefaults(BLim, BLimLower, BLimUpper, Me.m_BatchData.nTFM, Me.m_BLimValues)
            Me.calcDefaults(BBase, BBaseLower, BBaseUpper, Me.m_BatchData.nTFM, Me.m_BBaseValues)
            Me.calcDefaults(FMax, FMaxLower, FMaxUpper, Me.m_BatchData.nTFM, Me.m_FMaxValues)

        End Sub


        Private Sub calcDefaults(Value As Single, LowPercent As Single, UPPercent As Single, n As Integer, ByRef values() As Single)

            Try
                ReDim values(n)
                Dim LowB As Single, UpB As Single
                LowB = Value - Value * LowPercent
                UpB = Value + Value * UPPercent
                Dim dx As Single = (UpB - LowB) / (n - 1)
                For i As Integer = 1 To n
                    values(i) = LowB + dx * (i - 1)
                Next
            Catch ex As Exception

            End Try

        End Sub


        Friend Sub updateN()
            Try

                ReDim Preserve Me.m_BLimValues(Me.m_BatchData.nTFM)
                ReDim Preserve Me.m_BBaseValues(Me.m_BatchData.nTFM)
                ReDim Preserve Me.m_FMaxValues(Me.m_BatchData.nTFM)

            Catch ex As Exception

            End Try

        End Sub


    End Class

End Namespace
