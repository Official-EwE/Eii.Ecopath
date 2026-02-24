' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSY

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' ToDo
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cFMSYResults

#Region " Private vars "

        Private m_FMSY As Single()
        Private m_CMSY As Single()
        Private m_CMSYBase As Single()
        Private m_FBase As Single()
        Private m_Value As Single()
        Private m_ValueBase As Single()
        Private m_IsFopt As Boolean()
        Private m_CatchAtFMSY As Single()
        Private m_ValueAtFMSY As Single()

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nGroups"></param>
        ''' -------------------------------------------------------------------
        Friend Sub New(nGroups As Integer)
            ReDim Me.m_FMSY(nGroups)
            ReDim Me.m_CMSY(nGroups)
            ReDim Me.m_CMSYBase(nGroups)
            ReDim Me.m_FBase(nGroups)
            ReDim Me.m_Value(nGroups)
            ReDim Me.m_ValueBase(nGroups)
            ReDim Me.m_IsFopt(nGroups)
            ReDim Me.m_CatchAtFMSY(nGroups)
            ReDim Me.m_ValueAtFMSY(nGroups)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FMSY(iGroup As Integer) As Single
            Get
                Return Me.m_FMSY(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_FMSY(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property CMSY(iGroup As Integer) As Single
            Get
                Return Me.m_CMSY(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_CMSY(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property CMSYBase(iGroup As Integer) As Single
            Get
                Return Me.m_CMSYBase(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_CMSYBase(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FBase(iGroup As Integer) As Single
            Get
                Return Me.m_FBase(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_FBase(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Value(iGroup As Integer) As Single
            Get
                Return Me.m_Value(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_Value(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ValueBase(iGroup As Integer) As Single
            Get
                Return Me.m_ValueBase(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_ValueBase(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsFopt(iGroup As Integer) As Boolean
            Get
                Return Me.m_IsFopt(iGroup)
            End Get
            Friend Set(value As Boolean)
                Me.m_IsFopt(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property CatchAtFMSY(iGroup As Integer) As Single
            Get
                Return Me.m_CatchAtFMSY(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_CatchAtFMSY(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ValueAtFMSY(iGroup As Integer) As Single
            Get
                Return Me.m_ValueAtFMSY(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_ValueAtFMSY(iGroup) = value
            End Set
        End Property

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' ToDo
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cMSYOptimum

        ''' <summary>FMSY per group.</summary>
        Private m_Fopt As Single()
        ''' <summary>Flag stating whether Fmsy was actually found.</summary>
        Private m_IsFopt As Boolean()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nGroups"></param>
        ''' <remarks></remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(nGroups As Integer)
            ReDim Me.m_Fopt(nGroups)
            ReDim Me.m_IsFopt(nGroups)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FOpt(iGroup) As Single
            Get
                Return Me.m_Fopt(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_Fopt(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsFopt(iGroup) As Boolean
            Get
                Return Me.m_IsFopt(iGroup)
            End Get
            Friend Set(value As Boolean)
                Me.m_IsFopt(iGroup) = value
            End Set
        End Property

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' MSY Results 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cMSYFResult

#Region " Private fields "

        Private m_FCur As Single
        Private m_TotalValue As Single

        ''' <summary>Biomass at the current F, by group.</summary>
        Private m_B As Single()
        ''' <summary>Catch by group at the current F, by group.</summary>
        Private m_Catch As Single()
        ''' <summary>Fishing Mortality by group.</summary>
        Private m_FishingMort As Single()

#End Region ' Private fields

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nGroups"></param>
        ''' <param name="F"></param>
        ''' <param name="Value"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(nGroups As Integer, F As Single, Value As Single)
            Me.m_FCur = F
            Me.m_TotalValue = Value
            ReDim Me.m_B(nGroups)
            ReDim Me.m_Catch(nGroups)
            ReDim Me.m_FishingMort(nGroups)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property B(iGroup As Integer) As Single
            Get
                Return Me.m_B(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_B(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property [Catch](iGroup As Integer) As Single
            Get
                Return Me.m_Catch(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_Catch(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' ToDo
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FishingMort(iGroup As Integer) As Single
            Get
                Return Me.m_FishingMort(iGroup)
            End Get
            Friend Set(value As Single)
                Me.m_FishingMort(iGroup) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the current F.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property FCur As Single
            Get
                Return Me.m_FCur
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the total value.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property TotalValue As Single
            Get
                Return Me.m_TotalValue
            End Get
        End Property

    End Class

End Namespace
