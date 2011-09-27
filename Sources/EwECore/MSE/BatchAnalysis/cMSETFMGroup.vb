
#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region

Namespace MSE

    Public Class cMSETFMGroup
        Inherits cCoreGroupBase

        Private m_BLimValues() As Single
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

            ' MSETFMNIteration
            'MSETFMBLimLower()
            ' MSETFMBLimUpper()
            ' MSETFMBLimValues()

            'meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            'val = New cValue(New Integer, eVarNameFlags.MSETFMNIteration, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMNIteration))
            'm_values.Add(val.varName, val)

            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSETFMBLimLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMBLimLower))
            m_values.Add(val.varName, val)


            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MSETFMBLimUpper, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMBLimUpper))
            m_values.Add(val.varName, val)


            'meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            'val = New cValue(New Single, eVarNameFlags.MSETFMBLimLower, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.MSETFMBLimUpper))
            'm_values.Add(val.varName, val)



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
                Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTFM Then
                    Return Me.m_BLimValues(IterationIndex)
                End If
                'OH My.....
                Return cCore.NULL_VALUE
            End Get

            Set(ByVal value As Single)
                Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTFM Then
                    Me.m_BLimValues(IterationIndex) = value
                End If
            End Set
        End Property

        Public Sub SetToDefaults()


            Me.calcDefaults(BLim, BLimLower, BLimUpper, Me.m_BatchData.nTFM, Me.m_BLimValues)

        End Sub


        Private Sub calcDefaults(Value As Single, LowPercent As Single, UPPercent As Single, n As Integer, values() As Single)

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


    End Class

End Namespace
