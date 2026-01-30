' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Grid control, implements the Ecospace interface to assign species to habitats.
    ''' </summary>
    ''' =======================================================================

    Public Class gridHabitatPreference
        Inherits cEwEGrid

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Construction / destruction

#Region " Overrides "

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            'Call base class InitStyle method. 
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            Me.Redim(Me.Core.nGroups + 2, Me.Core.nHabitats + 2)

            'Set header cells # (0,0)
            Me(0, 0) = New cEwEColumnHeaderCell(My.Resources.ECOSPACE_HEADER_GROUP_HABITAT)
            Me(0, 0).ColumnSpan = 2

            'Dynamic row header - group name 
            For i As Integer = 1 To Me.Core.nGroups
                source = Me.Core.EcospaceGroupInputs(i)
                Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                ' # Group name row header cells
                Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            'Dynamic column header - Habitat name
            For j As Integer = 0 To Me.Core.nHabitats - 1
                source = Me.Core.EcospaceHabitats(j)
                ' +1 to compensate for header column, +1 to compensate for zero-based habitat index.
                Me(0, j + 2) = New cEwEColumnHeaderCell(source.Name)
            Next

            Me.FixedColumns = 2
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim groupEcospace As cEcospaceGroupInput = Nothing
            Dim groupEcopath As cEcoPathGroupInput = Nothing
            Dim hab As cEcospaceHabitat = Nothing
            Dim cell As cEwECellBase = Nothing

            For iGroup As Integer = 1 To Me.Core.nGroups

                ' Get sources
                groupEcospace = Me.Core.EcospaceGroupInputs(iGroup)
                groupEcopath = Me.Core.EcopathGroupInputs(iGroup)

                For iHabitat As Integer = 0 To Me.Core.nHabitats - 1

                    hab = Me.Core.EcospaceHabitats(iHabitat)

                    ' Create proportion cell (was checkbox)
                    cell = New cPropertyCell(Me.PropertyManager, groupEcospace, eVarNameFlags.PreferredHabitat, hab)
                    cell.Behaviors.Add(Me.EwEEditHandler)
                    cell.SuppressZero = True
                    Me(iGroup, iHabitat + 2) = cell

                Next

            Next

        End Sub

        Public Overrides ReadOnly Property CoreComponents() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {eCoreComponentType.Ecopath, eCoreComponentType.Ecospace}
            End Get
        End Property

#End Region ' Overrides

    End Class

End Namespace

