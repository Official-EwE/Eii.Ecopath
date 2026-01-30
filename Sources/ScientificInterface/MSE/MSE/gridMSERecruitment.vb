' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources



Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' Grid to allow species quota interaction.
    ''' </summary>
    ''' ===========================================================================

    Public Class gridMSERecruitment
        Inherits cEwEGrid

#Region " Internal defs "

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            ForcastGain
            RHalfB
            RecruitmentCV
        End Enum

#End Region ' Internal defs

#Region " Constructor "

        Public Sub New()
            MyBase.New()
        End Sub

#End Region ' Constructor

#Region " Public interfaces "

        Public Property Group() As cMSEGroupInput
            Get
                Try

                    If Me.Selection.SelectedRows.Length = 1 Then
                        Return DirectCast(Me.Selection.SelectedRows(0).Tag, cMSEGroupInput)
                    End If
                Catch ex As Exception
                    Debug.Assert(False, "Invalid cast!!!! maybe..." & ex.Message)
                End Try

                Return Nothing

            End Get
            Set(value As cMSEGroupInput)
                Me.Selection.Clear()
                If value IsNot Nothing Then
                    Me.Selection.Add(New Position(value.Index, 0))
                End If
                Me.RaiseSelectionChangeEvent()
            End Set
        End Property

#End Region ' Public interfaces

#Region " Overrides "

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length

            Me.Redim(1, iNumCols)

            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.RHalfB) = New cEwEColumnHeaderCell(SharedResources.HEADER_RHALFB0RATIO)
            Me(0, eColumnTypes.ForcastGain) = New cEwEColumnHeaderCell(SharedResources.HEADER_FORCASTGAIN)
            Me(0, eColumnTypes.RecruitmentCV) = New cEwEColumnHeaderCell(SharedResources.HEADER_RECRUITMENT_CV)

            Me.FixedColumns = 2
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cMSEGroupInput = Nothing

            ' For each group
            For iGroup As Integer = 1 To Me.Core.nLivingGroups

                'Get the group info!!!!
                group = Me.Core.MSEManager.GroupInputs(iGroup)

                Me.AddRow()

                Me(iGroup, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(iGroup))
                Me(iGroup, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                Me(iGroup, eColumnTypes.RHalfB) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.RHalfB0Ratio)
                Me(iGroup, eColumnTypes.ForcastGain) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSEForcastGain)
                Me(iGroup, eColumnTypes.RecruitmentCV) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSERecruitmentCV)

                Me.Rows(iGroup).Tag = group

            Next iGroup

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.Selection.SelectionMode = GridSelectionMode.Row
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.MSE
            End Get
        End Property

#End Region ' Overrides

    End Class

End Namespace ' Ecosim
