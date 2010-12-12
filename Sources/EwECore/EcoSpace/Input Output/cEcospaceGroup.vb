Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceGroup
    Inherits cCoreGroupBase

#Region "Constructor"

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Me.DBID = DBID
        m_dataType = eDataTypes.EcospaceGroup
        m_coreComponent = eCoreComponentType.EcoSpace

        Dim val As cValue
        Dim meta As cVariableMetaData

        Try

            m_dataType = eDataTypes.EcospaceGroup
            m_coreComponent = eCoreComponentType.EcoSpace

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' Mvel
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.MVel, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' RelMoveBad
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.RelMoveBad, eStatusFlags.Null, eValueTypes.Int, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' RelVulBad
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.RelVulBad, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' EatEffBad
            meta = New cVariableMetaData(0.01!, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.EatEffBad, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' IsAdvected
            meta = New cVariableMetaData(False)
            val = New cValue(New Boolean, eVarNameFlags.IsAdvected, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' IsMigratory
            meta = New cVariableMetaData(False)
            val = New cValue(New Boolean, eVarNameFlags.IsMigratory, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' MigrationConcRow N/S concentration
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MigrationConcRow, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' MigrationConcCol E/W concentration
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.MigrationConcCol, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' PredictEffort
            meta = New cVariableMetaData()
            val = New cValue(New Boolean, eVarNameFlags.PredictEffort, eStatusFlags.Null, eValueTypes.Bool, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Barrier avoidance weight
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.BarrierAvoidanceWeight, eStatusFlags.Null, eValueTypes.Sng, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Array variables

            ' PreferredRow as an array of Point objects
            val = New cValueArray(eValueTypes.PointArray, eVarNameFlags.PreferredCell, eStatusFlags.OK, eCoreCounterTypes.nMonths, AddressOf m_core.GetCoreCounter)
            m_values.Add(val.varName, val)

            'PreferredHabitat()
            meta = New cVariableMetaData(False)
            val = New cValueArray(eValueTypes.BoolArray, eVarNameFlags.PreferredHabitat, eStatusFlags.Null, eCoreCounterTypes.nHabitats, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'set status flags to their default values
            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceGroup.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceGroup. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Properties by dot (.) operator "

    ''' <summary>Base dispersal</summary>
    Public Property MVel() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MVel))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MVel, value)
        End Set
    End Property

    ''' <summary>Relative dispersal in bad habitat</summary>
    Public Property RelMoveBad() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.RelMoveBad))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.RelMoveBad, value)
        End Set
    End Property

    ''' <summary>Relative vulnerability in bad habitat</summary>
    Public Property RelVulBad() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.RelVulBad))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.RelVulBad, value)
        End Set
    End Property

    ''' <summary>Relative feeding in bad habitat</summary>
    Public Property EatEffBad() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EatEffBad))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EatEffBad, value)
        End Set
    End Property


    Public Property IsAdvected() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.IsAdvected))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.IsAdvected, value)
        End Set
    End Property

    Public Property IsMigratory() As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.IsMigratory))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.IsMigratory, value)
        End Set
    End Property

    Public Property PreferredCell(ByVal iMonth As Integer) As Drawing.Point
        Get
            Return DirectCast(GetVariable(eVarNameFlags.PreferredCell, iMonth), Drawing.Point)
        End Get

        Set(ByVal value As Drawing.Point)
            SetVariable(eVarNameFlags.PreferredCell, value, iMonth)
        End Set
    End Property

    Public Property PreferredHabitat(ByVal iHabitat As Integer) As Boolean
        Get
            Return CBool(GetVariable(eVarNameFlags.PreferredHabitat, iHabitat))
        End Get

        Set(ByVal value As Boolean)
            SetVariable(eVarNameFlags.PreferredHabitat, value, iHabitat)
        End Set
    End Property

    Public Property MigrationNSCon() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MigrationConcRow))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MigrationConcRow, value)
        End Set
    End Property

    Public Property MigrationEWCon() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MigrationConcCol))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.MigrationConcCol, value)
        End Set
    End Property

    Public Property BarrierAvoidanceWeight() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.BarrierAvoidanceWeight))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.BarrierAvoidanceWeight, value)
        End Set
    End Property

#End Region

#Region "Status by dot (.) operator"

    Public Property MVelStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MVel)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MVel, value)
        End Set
    End Property

    Public Property RelMoveBadStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.RelMoveBad)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.RelMoveBad, value)
        End Set
    End Property

    Public Property RelVulBadStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.RelVulBad)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.RelVulBad, value)
        End Set
    End Property

    Public Property EatEffBadStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EatEffBad)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EatEffBad, value)
        End Set
    End Property

    Public Property IsAdvectedStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.IsAdvected)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.IsAdvected, value)
        End Set
    End Property

    Public Property IsMigratoryStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.IsMigratory)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.IsMigratory, value)
        End Set
    End Property

    Public Property PreferredCellStatus(ByVal iMonth As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.PreferredCell, iMonth)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PreferredCell, value, iMonth)
        End Set
    End Property

    Public Property PreferredHabitatStatus(ByVal iHabitat As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.PreferredHabitat, iHabitat)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.PreferredHabitat, value, iHabitat)
        End Set
    End Property


    Public Property MigrationNSConStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MigrationConcRow)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MigrationConcRow, value)
        End Set
    End Property

    Public Property MigrationEWConStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.MigrationConcCol)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.MigrationConcCol, value)
        End Set
    End Property

#End Region

End Class
