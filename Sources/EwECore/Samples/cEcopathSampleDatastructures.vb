' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Samples

    ''' <summary>
    ''' Data structures for sampled Ecopath models.
    ''' <seealso cref="cEcopathSampleManager"/>.
    ''' <seealso cref="cEcopathSample"/>.
    ''' </summary>
    Public Class cEcopathSampleDatastructures

        Private m_ecopathds As cEcopathDataStructures = Nothing
        Friend m_samples As New List(Of cEcopathSample)
        Friend m_loaded As cEcopathSample = Nothing
        Friend m_backup As cEcopathSample = Nothing

        Friend Sub New(ecopathDS As cEcopathDataStructures)
            Me.m_ecopathds = ecopathDS
        End Sub

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an available sample.
        ''' <seealso cref="nSamples"/>
        ''' </summary>
        ''' <param name="iSample">The one-based index of the sample to retrieve.
        ''' This index cannot exceed <see cref="nSamples">the total number of
        ''' available samples</see></param>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Sample(iSample As Integer) As cEcopathSample
            Get
                If (iSample < 1 Or iSample > Me.nSamples) Then Return Nothing
                Return Me.m_samples(iSample - 1)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of available samples
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property nSamples As Integer
            Get
                Return Me.m_samples.Count
            End Get
        End Property

#End Region ' Public access

    End Class

End Namespace
