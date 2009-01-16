'==============================================================================
'
' $Log: cPedigreeManager.vb,v $
' Revision 1.3  2009/01/16 18:30:33  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/11/28 16:54:00  joeb
' Cleaned up ToDo's
'
' Revision 1.1  2008/09/26 07:30:09  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/07/21 18:46:47  jeroens
' Loads correctly
'
' Revision 1.1  2008/07/21 14:12:35  jeroens
' Initial version, under development
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports System.Collections.Generic

#End Region ' Imports directive

Public Class cPedigreeLevel
    Inherits cCoreInputOutputBase

    Private m_data As cEcopathDataStructures = Nothing
    Private m_manager As cPedigreeManager = Nothing

    Friend Sub New(ByVal core As cCore, ByVal manager As cPedigreeManager, ByVal iDBID As Integer)
        MyBase.New(core)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim desc() As Char

        Me.DBID = iDBID
        Me.m_data = core.m_EcoPathData
        Me.m_manager = manager
        Me.m_dataType = eDataTypes.PedigreeLevel

        'VarName
        meta = New cVariableMetaData(0, 1000, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Integer, eVarNameFlags.VariableName, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'IndexValue
        meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.IndexValue, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        'ConfidenceInterval
        meta = New cVariableMetaData(0, 100, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.ConfidenceInterval, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        ' Description
        meta = New cVariableMetaData(60000)
        val = New cValue(New String(desc), eVarNameFlags.Description, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, _
                            meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

    End Sub

    Public Property VariableName() As eVarNameFlags
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.VariableName), eVarNameFlags)
        End Get
        Set(ByVal value As eVarNameFlags)
            Me.SetVariable(eVarNameFlags.VariableName, value)
        End Set
    End Property

    Public Property IndexValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.IndexValue))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.IndexValue, value)
        End Set
    End Property

    Public Property ConfidenceInterval() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.ConfidenceInterval))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.ConfidenceInterval, value)
        End Set
    End Property

    Public Property Description() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Description))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Description, value)
        End Set
    End Property

    Friend Function Update() As Boolean
        Dim iLevel As Integer = Array.IndexOf(Me.m_data.PedigreeLevelDBID, Me.DBID)
        Debug.Assert(iLevel > -1)
        Me.m_data.PedigreeLevelIndexValue(iLevel) = Me.IndexValue
        Me.m_data.PedigreeLevelConfidence(iLevel) = Me.ConfidenceInterval
        Me.m_data.PedigreeLevelDescription(iLevel) = Me.Description
    End Function

    Friend Function Load() As Boolean
        Dim iLevel As Integer = Array.IndexOf(Me.m_data.PedigreeLevelDBID, Me.DBID)
        Debug.Assert(iLevel > -1)
        Me.IndexValue = Me.m_data.PedigreeLevelIndexValue(iLevel)
        Me.ConfidenceInterval = Me.m_data.PedigreeLevelConfidence(iLevel)
        Me.Description = Me.m_data.PedigreeLevelDescription(iLevel)
    End Function

End Class

Public Class cPedigreeManager
    Implements ICoreInterface

#Region " Private classes "

    Private Class cPedigreeLevelListSorter
        Implements IComparer(Of cPedigreeLevel)

        Public Function Compare(ByVal x As cPedigreeLevel, ByVal y As cPedigreeLevel) As Integer _
            Implements IComparer(Of cPedigreeLevel).Compare
            If x.Index < y.Index Then Return -1
            If x.Index > y.Index Then Return 1
            If x.ConfidenceInterval < y.ConfidenceInterval Then Return -1
            If x.ConfidenceInterval > y.ConfidenceInterval Then Return 1
            Return 0
        End Function

    End Class

#End Region ' Private classes

    Protected Shared g_varNameSupported As eVarNameFlags() = {eVarNameFlags.Biomass, eVarNameFlags.PBInput, eVarNameFlags.QBInput, eVarNameFlags.DietComp}
    Protected m_core As cCore = Nothing
    Protected m_varName As eVarNameFlags = eVarNameFlags.NotSet
    Protected m_levels As New List(Of cPedigreeLevel)
    Protected m_dataType As eDataTypes = eDataTypes.PedigreeLevel
    Protected m_messageSource As eCoreComponentType = eCoreComponentType.Core

    Friend Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags)
        Me.m_core = core
        Me.m_varName = varName
    End Sub

    Public Shared Function SupportVarNames() As eVarNameFlags()
        Return cPedigreeManager.g_varNameSupported
    End Function

    Public Overridable Function CreateNewLevel(ByVal strDescription As String, ByVal iPosition As Integer, _
            ByVal varName As eVarNameFlags, ByVal sIndexValue As Single, ByVal sConfidence As Single) As cPedigreeLevel

        Dim iDBID As Integer = 0
        Dim level As cPedigreeLevel = Nothing
        Dim bSucces As Boolean = True

        'Add storage to the underlying data arrays and the db
        'AddShape() will NOT preserve the existing data  
        'All the data in the Ecosim data structures will be reloaded from the database
        If m_core.AddPedigreeLevel(varName, iPosition, sIndexValue, sConfidence, strDescription, iDBID) Then

            'get the index from the dbid for the new shape
            'iEcoSimIndex = Array.IndexOf(m_Data.ForcingDBIDs, DBID)

            'create a new shape that contains a database ID to the underlying ecosim data
            level = New cPedigreeLevel(Me.m_core, Me, DBID)
            level.Index = iPosition
            level.Load()

            'Add the new shape to the list 
            Me.m_levels.Add(level)
            Me.m_levels.Sort(New cPedigreeLevelListSorter())
            Me.FixIndexes()

            ' Reload
            m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

            Return level

        End If

        Return Nothing
    End Function

    Public Overridable Function Remove(ByVal level As cPedigreeLevel) As Boolean

        Try

            'Remove all references to ShapeToRemove from Databse, EcoSim data arrays and All Shape Managers
            'this will remove this record from the database and re-load all EcoSim Data Arrays that are related to the shapes
            If Not m_core.RemovePedigreeLevel(level.DBID) Then Return False

            'remove the shape from the shape managers memory
            Me.m_levels.Remove(level)

            'The structure of the underlying EcoSim data has changed because it was re-loaded above
            'So re-init both Forcing and Eggprod shape managers from the underlying EcoSim Data
            'it is not good enough to just init this manager as other shape managers were affected by changing the data
            m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

            Return True

        Catch ex As Exception
            Debug.Assert(False)
        End Try


    End Function

#Region " Saving, loading and updating "

    ''' <summary>
    ''' Called by a level to tell the manager that it has changed data. 
    ''' </summary>
    ''' <remarks>Tell the core that a level has changed.</remarks>
    Friend Overridable Sub LevelChanged(Optional ByVal level As cPedigreeLevel = Nothing)
        m_core.onChanged(Me, eMessageType.DataModified)
    End Sub

    Public Overridable Function Update() As Boolean
        Try
            'have each level will update the underlying EcoSim data
            For Each level As cPedigreeLevel In Me.m_levels
                If Not level.Update() Then
                    cLog.Write(Me.ToString & ".Update() level failed to update DBID=" & level.DBID)
                    Debug.Assert(False, Me.ToString & ".Update() level failed to update DBID=" & level.DBID)
                    'this will keep trying to update the rest of the data
                    'even if there was a problem with one of the shapes
                End If
            Next level

            Return True
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Error: " & ex.Message)
        End Try

    End Function

    Public Overridable Function Load() As Boolean
        Try
            Dim level As cPedigreeLevel = Nothing
            Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData

            Me.m_levels.Clear()
            For iLevel As Integer = 1 To data.NumPedigreeLevels
                If data.PedigreeLevelVarName(iLevel) = Me.m_varName Then
                    level = New cPedigreeLevel(Me.m_core, Me, data.PedigreeLevelDBID(iLevel))
                    level.Load()
                    Me.m_levels.Add(level)
                End If
            Next

            Return True
        Catch ex As Exception
            Return False
            Debug.Assert(False, Me.ToString & ".Load() Error: " & ex.Message)
        End Try
    End Function

    Public Sub Changed()

    End Sub

    Private Sub FixIndexes()
        Dim level As cPedigreeLevel = Nothing
        For iLevel As Integer = 0 To Me.m_levels.Count - 1
            level = Me.m_levels(iLevel)
            level.AllowValidation = False
            level.Index = iLevel
            level.AllowValidation = True
        Next
    End Sub

#End Region ' Saving, loading and updating

#Region " ICoreInterface Implementation "

    Public ReadOnly Property DataType() As EwEUtils.Core.eDataTypes Implements ICoreInterface.DataType
        Get
            Return Me.m_dataType
        End Get
    End Property

    Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            Return Me.m_messageSource
        End Get
    End Property

    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

    Public Function GetID() As String Implements ICoreInterface.GetID
        Dim id As Integer = CType(m_dataType, Integer)
        Return cValueID.getDataTypeID(m_dataType, id)
    End Function

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return Me.ToString
        End Get
        Set(ByVal value As String)
            Debug.Assert(False, "Not Implemented")
        End Set
    End Property

#End Region ' ICoreInterface Implementation

End Class
