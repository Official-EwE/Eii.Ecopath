' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel

Namespace ValueChainMigrator.LegacyData

    <Obsolete(), Serializable()>
    Public MustInherit Class cUnit
        Inherits cOOPStorable

        ''' <summary>Units that receive outputs from this unit.</summary>
        Protected m_llinkOutput As New List(Of cLink)
        ''' <summary>Units that provide inputs for this unit.</summary>
        Protected m_llinkInput As New List(Of cLink)

#Region " Constructor "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
        End Sub

#End Region ' Constructor

#Region " Links "

        Public Function LinkOutCount() As Integer
            Return Me.m_llinkOutput.Count
        End Function

        Public Function LinkOut(iIndex As Integer) As cLink
            Return Me.m_llinkOutput(iIndex)
        End Function

        ''' <summary>
        ''' Get all links directly linking to a target.
        ''' </summary>
        ''' <param name="unitTarget"></param>
        ''' <returns></returns>
        Public Function Links(unitTarget As cUnit) As cLink()
            Dim lLinks As New List(Of cLink)
            For Each link As cLink In Me.m_llinkOutput
                If ReferenceEquals(link.Target, unitTarget) Then
                    lLinks.Add(link)
                End If
            Next
            Return lLinks.ToArray
        End Function

        Public Sub AddLink(link As cLink)

            ' Sanity check
            Debug.Assert(ReferenceEquals(link.Source, Me))

            Me.m_llinkOutput.Add(link)
            link.Target.AddInputLink(link)
        End Sub

        Public Sub RemoveLink(link As cLink)
            Me.m_llinkOutput.Remove(link)
            link.Target.RemoveInputLink(link)
        End Sub

        Public Function LinkInCount() As Integer
            Return Me.m_llinkInput.Count
        End Function

        Public Function LinkIn(iIndex As Integer) As cLink
            Return Me.m_llinkInput(iIndex)
        End Function

        Protected Sub AddInputLink(link As cLink)
            ' Sanity check
            Debug.Assert(ReferenceEquals(link.Target, Me))
            Me.m_llinkInput.Add(link)
        End Sub

        Protected Sub RemoveInputLink(link As cLink)
            Me.m_llinkInput.Remove(link)
        End Sub

        Public Function IsLoop(unit As cUnit) As Boolean

            ' Linked to self?
            Dim bIsLoop As Boolean = ReferenceEquals(unit, Me)

            ' If no loop yet
            If Not bIsLoop Then
                ' Follow each output link
                For Each link As cLink In Me.m_llinkOutput
                    ' See the target link is the requesting unit
                    If link.Target.IsLoop(unit) Then bIsLoop = True : Exit For
                Next link
            End If

            Return bIsLoop
        End Function

#End Region ' Links 


        <Browsable(False)>
        Public Property Sequence() As Integer

        <Browsable(False)>
        Public MustOverride ReadOnly Property UnitType() As cUnitFactory.eUnitType

        Public Overridable Property Name() As String

        Public Overridable Property Nationality() As Integer

        Public Overridable Property NameLocal() As String

    End Class

End Namespace
