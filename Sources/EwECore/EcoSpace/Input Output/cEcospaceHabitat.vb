Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceHabitat
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Me.DBID = DBID
        m_dataType = eDataTypes.EcospaceHabitat
        m_coreComponent = eCoreComponentType.EcoSpace

        Dim val As cValue
        Dim meta As cVariableMetaData

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcospaceHabitat, eCoreComponentType.EcoSpace, Index, cCore.NULL_VALUE)

            ' HabAreaProportion
            meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Single, eVarNameFlags.HabAreaProportion, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceHabitat.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceHabitat. Error: " & ex.Message)
        End Try

    End Sub

#End Region

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of cells in a Habitat.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumCells() As Integer 
        Get
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim l As cEcospaceLayerHabitat = bm.LayerHabitat
            Dim iIndex As Integer = Me.Index
            Dim iNumCells As Integer = 0

            For iRow As Integer = 1 To bm.InRow
                For iCol As Integer = 1 To bm.InCol
                    If CInt(l.Cell(iRow, iCol)) = iIndex Then
                        iNumCells += 1
                    End If
                Next
            Next
            Return iNumCells

        End Get
    End Property

#Region "Properties by dot (.) operator "

    Public Property HabAreaProportion() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.HabAreaProportion))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.HabAreaProportion, value)
        End Set
    End Property

#End Region

#Region "Status by dot (.) operator"

    Public Property HabAreaProportionStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.HabAreaProportion)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.HabAreaProportion, value)
        End Set
    End Property

#End Region

End Class
