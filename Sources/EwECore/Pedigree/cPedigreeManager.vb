#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports System.Collections.Generic

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class that contains and distributes <see cref="cPedigreeLevel">pedigree levels</see>,
''' and maintains group <see cref="cPedigreeManager.Pedigree">pedigree assignments</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPedigreeManager
    Inherits cCoreInputOutputBase

#Region " Private vars "

    ''' <summary>The variable that this manager is responsible for.</summary>
    Private m_varName As eVarNameFlags = eVarNameFlags.NotSet
    ''' <summary>The pedigree levels that belong to the variable of this manager.</summary>
    Private m_levels As New cCoreInputOutputList(Of cPedigreeLevel)(eDataTypes.PedigreeLevel, 1)
    ''' <summary>Mapping of Core level index to local level ID.</summary>
    Private m_dictID As New Dictionary(Of Integer, Integer)

#End Region ' Private vars

#Region " Constructor and Cleanup"

    Friend Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal iDBID As Integer)
        MyBase.New(core)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Me.m_dataType = eDataTypes.PedigreeManager
        Me.m_coreComponent = eCoreComponentType.EcoPath
        Me.m_varName = varName
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        Me.AllowValidation = False

        Me.DBID = iDBID

        'Array variables
        'Pedigree
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValueArray(eValueTypes.IntArray, eVarNameFlags.Pedigree, eStatusFlags.Null, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.Pedigree))
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

    Public Overrides Sub Clear()
        MyBase.Clear()

        For Each ped As cPedigreeLevel In Me.m_levels
            ped.Clear()
        Next
        Me.m_levels.Clear()

        Me.m_dictID.Clear()

    End Sub

#End Region ' Constructor

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the pedigree index for a given variable. 
    ''' </summary>
    ''' <param name="iGroup">One-based index of the group.</param>
    ''' -----------------------------------------------------------------------
    Public Property Pedigree(ByVal iGroup As Integer) As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.Pedigree, iGroup))
        End Get
        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.Pedigree, value, iGroup)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the pedigree index status for a given variable. 
    ''' </summary>
    ''' <param name="iVariable">One-based index of the variable for which to access the status.</param>
    ''' -----------------------------------------------------------------------
    Public Property PedigreeStatus(ByVal iVariable As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.Pedigree, iVariable)
        End Get
        Friend Set(ByVal value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Pedigree, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of pedigree levels in the manager.
    ''' </summary>
    ''' <remarks>
    ''' Level indexing is one-base. It's just so that you know it. Really. ONE
    ''' based; let there be no confusion. At least as little confusion as
    ''' possibly possible. Right. There you go. I hope.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumLevels() As Integer
        Get
            Return Me.m_levels.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a pedigree level from the manager.
    ''' </summary>
    ''' <param name="iLevel">The one-based index of the level to obtain.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Level(ByVal iLevel As Integer) As cPedigreeLevel
        Get
            If (iLevel <= 0) Then Return Nothing
            Return Me.m_levels(iLevel)
        End Get
    End Property

#End Region ' Properties

#Region " Public update interfaces "

    Private m_bInUpdate As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Commit pedigree levels to the EwE core.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function UpdatePedigreeLevels() As Boolean

        Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim level As cPedigreeLevel = Nothing

        Me.m_bInUpdate = True

        Try
            For iLevel As Integer = 1 To Me.NumLevels

                Try

                    level = Me.m_levels(iLevel)
                    data.PedigreeLevelName(level.Index) = level.Name
                    data.PedigreeLevelDescription(level.Index) = level.Description
                    data.PedigreeLevelColor(level.Index) = level.PoolColor
                    data.PedigreeLevelIndexValue(level.Index) = level.IndexValue
                    data.PedigreeLevelConfidence(level.Index) = level.ConfidenceInterval

                    ' Issue #796: in some daily build databases the description field cannot be empty
                    If String.IsNullOrEmpty(data.PedigreeLevelDescription(level.Index)) Then data.PedigreeLevelDescription(level.Index) = " "

                    Me.m_core.onChanged(level, eMessageType.DataModified)

                Catch ex As Exception
                    cLog.Write(Me.ToString & ".Update() level failed to update DBID=" & level.DBID)
                    Debug.Assert(False, Me.ToString & ".Update() level failed to update DBID=" & level.DBID)
                End Try

            Next iLevel

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Error: " & ex.Message)
            Return False
        End Try

        Me.m_bInUpdate = False

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Commit pedigree assignments to the EwE core.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function UpdatePedigree() As Boolean

        Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim iVariable As Integer = Me.m_core.PedigreeVariableIndex(Me.m_varName)
        Dim iIndex As Integer = 0
        Dim iLevel As Integer = 0

        ' Map local manager indexes to core level indexes 
        For iGroup As Integer = 1 To Me.m_core.nGroups
            ' Get local index
            iIndex = Me.Pedigree(iGroup)
            ' Is in valid range?
            If (iIndex > 0) And (iIndex <= Me.m_levels.Count) Then
                ' #Yes: obtain actual core index for this level
                iLevel = Me.m_levels(iIndex).Index
            Else
                ' #No: assume 'no assignment'
                iLevel = 0
            End If
            Try
                ' Store
                data.Pedigree(iGroup, iVariable) = iLevel
            Catch ex As Exception
                cLog.Write(Me.ToString & ".UpdatePedigree() group failed to update DBID=" & iGroup)
                Debug.Assert(False, Me.ToString & ".UpdatePedigree() group failed to update DBID=" & iGroup)
            End Try
        Next
        Return True

    End Function

#End Region ' Public update interfaces

#Region " Level management "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a pedigree level to the model.
    ''' </summary>
    ''' <param name="strName">Name of the new level.</param>
    ''' <param name="strDescription">Description of the new level.</param>
    ''' <param name="iPosition">The position of the level to assign in the manager.</param>
    ''' <param name="varName"></param>
    ''' <param name="sIndexValue"></param>
    ''' <param name="sConfidence"></param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>This method does NOT instruct the core to reload!</remarks>
    ''' -----------------------------------------------------------------------
    Public Function AddLevel(ByVal iPosition As Integer, _
                             ByVal varName As eVarNameFlags, _
                             ByVal strName As String, _
                             ByVal iColor As Integer, _
                             ByVal strDescription As String, _
                             ByVal sIndexValue As Single, _
                             ByVal sConfidence As Single, _
                             ByRef iDBID As Integer) As Boolean
        ' Issue #796: in some daily build databases the description field cannot be empty
        If String.IsNullOrEmpty(strDescription) Then strDescription = " "
        Return m_core.AddPedigreeLevel(varName, iPosition, strName, iColor, strDescription, sIndexValue, sConfidence, iDBID)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Move a pedigree level to a new position in the level list.
    ''' </summary>
    ''' <param name="level"></param>
    ''' <param name="iPosTo"></param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>This method does NOT instruct the core to reload!</remarks>
    ''' -----------------------------------------------------------------------
    Public Function MoveLevel(ByVal level As cPedigreeLevel, ByVal iPosTo As Integer) As Boolean
        Return Me.m_core.MovePedigreeLevel(level.Index, iPosTo)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a pedigree level from the model.
    ''' </summary>
    ''' <param name="level"></param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>This method does NOT instruct the core to reload!</remarks>
    ''' -----------------------------------------------------------------------
    Public Function RemoveLevel(ByVal level As cPedigreeLevel) As Boolean
        Return m_core.RemovePedigreeLevel(level.DBID)
    End Function

#End Region ' Level management

#Region " Configuration "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create pedigree levels and load all data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub Init()

        Dim level As cPedigreeLevel = Nothing
        Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData

        Me.m_levels.Clear()
        Me.m_dictID.Clear()

        For iLevel As Integer = 1 To data.NumPedigreeLevels
            If data.PedigreeLevelVarName(iLevel) = Me.m_varName Then

                level = New cPedigreeLevel(Me.m_core, Me, data.PedigreeLevelDBID(iLevel))

                ' Config level
                level.AllowValidation = False
                level.ID = Me.m_levels.Count + 1 ' one based
                level.Index = iLevel
                level.AllowValidation = True

                ' Update local admin
                Me.m_levels.Add(level)
                Me.m_dictID(iLevel) = level.ID

            End If
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load pedigree levels.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadPedigreeLevels() As Boolean

        Dim bSucces As Boolean = True

        If (Me.m_bInUpdate) Then Return bSucces

        Try
            Dim level As cPedigreeLevel = Nothing
            Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData

            For iLevel As Integer = 1 To Me.NumLevels

                level = Me.m_levels(iLevel)

                level.AllowValidation = False
                level.Name = data.PedigreeLevelName(level.Index)
                level.Description = data.PedigreeLevelDescription(level.Index)
                level.PoolColor = data.PedigreeLevelColor(level.Index)
                level.IndexValue = data.PedigreeLevelIndexValue(level.Index)
                level.ConfidenceInterval = data.PedigreeLevelConfidence(level.Index)
                level.VariableName = Me.m_varName
                level.IsEstimated = data.PedigreeLevelEstimated(level.Index)
                level.ResetStatusFlags()
                level.AllowValidation = True

            Next

        Catch ex As Exception
            bSucces = False
            Debug.Assert(False, Me.ToString & ".Load() Error: " & ex.Message)
        End Try

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load or update pedigree assignment values.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadPedigree() As Boolean

        Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim iVariable As Integer = Me.m_core.PedigreeVariableIndex(Me.m_varName)
        Dim iLevel As Integer = 0
        Dim iIndex As Integer = 0

        Me.AllowValidation = False

        ' Map core level indexes to local manager indexes
        For iGroup As Integer = 1 To Me.m_core.nGroups

            iLevel = data.Pedigree(iGroup, iVariable)
            If (iLevel > 0) Then
                iIndex = Me.m_dictID(iLevel)
            Else
                iIndex = -1
            End If
            Me.Pedigree(iGroup) = iIndex

        Next
        Me.ResetStatusFlags()
        Me.AllowValidation = True

        Return True

    End Function

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean

        MyBase.ResetStatusFlags(bForceReset)
        For iGroup As Integer = 1 To Me.m_core.nGroups
            Me.Set_Pedigree_Flags(Me.m_core.EcoPathGroupInputs(iGroup))
        Next
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the status flags of pedigree.
    ''' </summary>
    ''' <param name="group">The group to update.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub Set_Pedigree_Flags(ByVal group As cEcoPathGroupInput)

        Dim epdata As cEcopathDataStructures = Me.m_core.m_EcoPathData

        ' Borrow status flags from groups
        Me.AllowValidation = False

        If group.IsDetritus Then
            Me.SetStatusFlags(eVarNameFlags.Pedigree, eStatusFlags.NotEditable Or eStatusFlags.Null, group.Index)
        Else
            Select Case Me.m_varName

                Case eVarNameFlags.PBInput, eVarNameFlags.QBInput
                    If (group.IsProducer()) Then
                        Me.SetStatusFlags(eVarNameFlags.Pedigree, eStatusFlags.NotEditable Or eStatusFlags.Null, group.Index)
                    Else
                        Me.ClearStatusFlags(eVarNameFlags.Pedigree, eStatusFlags.NotEditable, group.Index)
                    End If

                Case eVarNameFlags.DietComp
                    If group.IsConsumer Then
                        Me.ClearStatusFlags(eVarNameFlags.Pedigree, eStatusFlags.NotEditable, group.Index)
                    Else
                        Me.SetStatusFlags(eVarNameFlags.Pedigree, eStatusFlags.NotEditable, group.Index)
                    End If

                Case eVarNameFlags.TCatchInput
                    If epdata.fCatch(group.Index) > 0 Then
                        Me.ClearStatusFlags(eVarNameFlags.Pedigree, eStatusFlags.NotEditable, group.Index)
                    Else
                        Me.SetStatusFlags(eVarNameFlags.Pedigree, eStatusFlags.NotEditable, group.Index)
                    End If

            End Select
        End If

        Me.AllowValidation = True

    End Sub

#End Region ' Configuration

End Class