#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports System.Collections.Generic

#End Region ' Imports

Public Class cPedigreeManager
    Implements ICoreInterface
    'Implements IEnumerable(Of cPedigreeLevel)

    Private Shared g_varNameSupported As eVarNameFlags() = {eVarNameFlags.Biomass, eVarNameFlags.PBInput, eVarNameFlags.QBInput, eVarNameFlags.DietComp}
    Private m_core As cCore = Nothing
    Private m_varName As eVarNameFlags = eVarNameFlags.NotSet
    Private m_levels As New List(Of cPedigreeLevel)
    Private m_dataType As eDataTypes = eDataTypes.PedigreeLevel
    Private m_messageSource As eCoreComponentType = eCoreComponentType.Core

    Friend Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags)
        Me.m_core = core
        Me.m_varName = varName
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the list of variables for which pedigree information is supported.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function SupportVariables() As eVarNameFlags()
        Return cPedigreeManager.g_varNameSupported
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strDescription"></param>
    ''' <param name="iPosition">The position of the level to assign in the manager.</param>
    ''' <param name="varName"></param>
    ''' <param name="sIndexValue"></param>
    ''' <param name="sConfidence"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function AddLevel(ByVal strDescription As String, _
                                         ByVal iPosition As Integer, _
                                         ByVal varName As eVarNameFlags, _
                                         ByVal sIndexValue As Single, _
                                         ByVal sConfidence As Single, _
                                         ByRef iDBID As Integer) As Boolean

        Dim level As cPedigreeLevel = Nothing
        Dim bSucces As Boolean = True

        If m_core.AddPedigreeLevel(varName, iPosition, sIndexValue, sConfidence, strDescription, iDBID) Then
            ' Reload me
            bSucces = Me.Load()
            ' Give core a chance to respond
            Me.m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)
        End If

        Return bSucces

    End Function

    Public Function MoveLevel(ByVal level As cPedigreeLevel, ByVal iPosTo As Integer) As Boolean
        ' NOP for now
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="level"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function RemoveLevel(ByVal level As cPedigreeLevel) As Boolean

        Dim bSucces As Boolean = True

        If m_core.RemovePedigreeLevel(level.DBID) Then
            ' Reload me
            bSucces = Me.Load()
            ' Give core a chance to respond
            Me.m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)
        End If

        Return bSucces

    End Function

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create and load pedigree levels.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Overridable Function Load() As Boolean
        Try
            Dim level As cPedigreeLevel = Nothing
            Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData

            Me.m_levels.Clear()
            For iLevel As Integer = 1 To data.NumPedigreeLevels
                If data.PedigreeLevelVarName(iLevel) = Me.m_varName Then

                    level = New cPedigreeLevel(Me.m_core, Me, data.PedigreeLevelDBID(iLevel))

                    level.AllowValidation = False
                    level.Index = iLevel
                    level.Name = data.PedigreeLevelName(iLevel)
                    level.Description = data.PedigreeLevelDescription(iLevel)
                    level.IndexValue = data.PedigreeLevelIndexValue(iLevel)
                    level.ConfidenceInterval = data.PedigreeLevelConfidence(iLevel)
                    level.AllowValidation = True

                    Me.m_levels.Add(level)

                End If
            Next

        Catch ex As Exception
            Return False
            Debug.Assert(False, Me.ToString & ".Load() Error: " & ex.Message)
        End Try

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Commit pedigree levels to the EwE core.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Update() As Boolean

        Dim data As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim level As cPedigreeLevel = Nothing

        Try
            For Each level In Me.m_levels
                Try

                    data.PedigreeLevelName(level.Index) = level.Name
                    data.PedigreeLevelDescription(level.Index) = level.Description
                    data.PedigreeLevelIndexValue(level.Index) = level.IndexValue
                    data.PedigreeLevelConfidence(level.Index) = level.ConfidenceInterval

                Catch ex As Exception
                    cLog.Write(Me.ToString & ".Update() level failed to update DBID=" & level.DBID)
                    Debug.Assert(False, Me.ToString & ".Update() level failed to update DBID=" & level.DBID)
                End Try
            Next level

            Me.m_core.onChanged(Me, eMessageType.DataModified)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Update() Error: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

#End Region ' Internals

#Region " Properties "

    Public ReadOnly Property NumLevels() As Integer
        Get
            Return Me.m_levels.Count
        End Get
    End Property

    Public ReadOnly Property Level(ByVal iLevel As Integer) As cPedigreeLevel
        Get
            Return Me.m_levels(iLevel)
        End Get
    End Property

#End Region ' Properties

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

#Region " ICollection implementation "
#If 0 Then

    ''' <summary>
    ''' Add a cForcingFunction object to the list
    ''' </summary>
    ''' <param name="level"><see cref="cPedigreeLevel">pedigree level</see>
    ''' or derived object to add to the manager and the underlying Ecopath data.</param>
    ''' <returns>True if Successfull</returns>
    Protected Overridable Overloads Function Add(ByVal level As cPedigreeLevel) As Boolean
        Try
            Me.m_levels.Add(level)
            Me.UpdateIDs()
            Return True
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    Public Overridable ReadOnly Property Item(ByVal iIndex As Integer) As cPedigreeLevel
        Get
            Try
                Return Me.m_levels.Item(iIndex)
            Catch ex As Exception
                cLog.Write(Me.ToString & ".Add() Error: " & ex.Message)
                Return Nothing
            End Try

        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Number of pedigrees in this manager.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>The collection is zero (0) based.</remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Count() As Integer
        Get
            Return Me.m_levels.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of IEnumerable.GetEnumerator provides access to the For Each statment
    ''' </summary>
    ''' <returns>The Enumerator of the List used by this object</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetEnumerator() As System.Collections.IEnumerator _
        Implements System.Collections.IEnumerable.GetEnumerator
        Return m_levels.GetEnumerator
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether this manager contain a given <see cref="cPedigreeLevel">pedigree level</see>.
    ''' </summary>
    ''' <param name="level">The <see cref="cPedigreeLevel">pedigree level</see> to test.</param>
    ''' <returns>True if this manager contains the given level.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Contains(ByRef level As cPedigreeLevel) As Boolean
        Try
            Return m_levels.Contains(level)
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a pedigree level from the manager and the underlying Ecopath data.
    ''' </summary>
    ''' <param name="level">Level to remove</param>
    ''' <returns>True if successful</returns>
    ''' -----------------------------------------------------------------------
    Public Overloads Function Remove(ByVal level As cPedigreeLevel) As Boolean
        Try
            If Not m_core.RemovePedigreeLevel(level.DBID) Then Return False

            'Remove the shape from the shape managers memory
            Me.m_levels.Remove(level)

            Me.UpdateIDs()

            'The structure of the underlying Ecopath data has changed because it was re-loaded above
            m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

        Catch ex As Exception
            Debug.Assert(False)
            Return False
        End Try
        Return True

    End Function

    Private Function GetEnumerator1() As IEnumerator(Of cPedigreeLevel) _
        Implements IEnumerable(Of EwECore.cPedigreeLevel).GetEnumerator
        Return Nothing
    End Function

    Protected Sub UpdateIDs()
        Dim shape As cPedigreeLevel = Nothing
        For iShape As Integer = 0 To Me.Count - 1
            shape = Me.Level(iShape)
            shape.ID = iShape
        Next iShape
    End Sub

#End If
#End Region ' IEnumerable implementation

End Class
