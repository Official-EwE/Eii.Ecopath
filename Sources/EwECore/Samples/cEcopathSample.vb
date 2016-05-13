' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports EwEUtils.Core
Imports EwECore.ValueWrapper

#End Region ' Imports

Namespace Samples

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Parameter sample snapshot for an Ecopath model, recorded from 
    ''' Monte Carlo model perturbations.
    ''' <seealso cref="cEcopathSampleManager"/>.
    ''' <seealso cref="cEcopathSampleDatastructures"/>.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cEcopathSample
        Inherits cCoreInputOutputBase

        Private m_b As Single()
        Private m_pb As Single()
        Private m_qb As Single()
        Private m_ee As Single()
        Private m_ba As Single()
        Private m_dc As Single(,)
        Private m_landing As Single(,)
        Private m_discard As Single(,)

        Public Sub New(core As cCore, ByVal DBID As Integer, ByVal iIndex As Integer)

            MyBase.New(core)

            Dim val As cValue = Nothing
            Dim meta As cVariableMetaData = Nothing

            Me.m_coreComponent = eCoreComponentType.EcopathSample
            Me.m_dataType = eDataTypes.EcopathSample

            Me.AllowValidation = False
            Me.Index = iIndex
            Me.DBID = DBID
            Me.Name = "Sample " & iIndex

            'Rating
            meta = New cVariableMetaData(0, 5, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
            val = New cValue(New Integer, eVarNameFlags.SampleRating, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            Me.m_values.Add(val.varName, val)

            Me.AllowValidation = True

            Me.ResetStatusFlags()

            ReDim Me.m_b(Me.m_core.nGroups)
            ReDim Me.m_pb(Me.m_core.nGroups)
            ReDim Me.m_qb(Me.m_core.nGroups)
            ReDim Me.m_ee(Me.m_core.nGroups)
            ReDim Me.m_ba(Me.m_core.nGroups)
            ReDim Me.m_dc(Me.m_core.nGroups, Me.m_core.nGroups)
            ReDim Me.m_landing(Me.m_core.nFleets, Me.m_core.nGroups)
            ReDim Me.m_discard(Me.m_core.nFleets, Me.m_core.nGroups)

        End Sub

        ''' <summary>
        ''' Get/set the MD5 hash of the Ecopath input set a sample was generated for.
        ''' </summary>
        Public Property Hash As String = ""

        ''' <summary>
        ''' Get/set the source computer that a sample was generated on.
        ''' </summary>
        Public Property Source As String = ""

        ''' <summary>
        ''' Get/set the date that a sample was generated.
        ''' </summary>
        Public Property Generated As Date

        Public Function B() As Single()
            Return Me.m_b
        End Function

        Public Function PB() As Single()
            Return Me.m_pb
        End Function

        Public Function QB() As Single()
            Return Me.m_qb
        End Function

        Public Function BA() As Single()
            Return Me.m_ba
        End Function

        Public Function EE() As Single()
            Return Me.m_ee
        End Function

        Public Function DC() As Single(,)
            Return Me.m_dc
        End Function

        Public Function Landing() As Single(,)
            Return Me.m_landing
        End Function

        Public Function Discard() As Single(,)
            Return Me.m_discard
        End Function

#Region " Variable access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cEcopathDataStructures.Emig">emigration rate relative to biomass</see>
        ''' ratio for this group.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Rating() As Integer
            Get
                Return CInt(GetVariable(eVarNameFlags.SampleRating))
            End Get
            Set(ByVal value As Integer)
                SetVariable(eVarNameFlags.SampleRating, value)
            End Set
        End Property

        Public Property RatingStatus As eStatusFlags
            Get
                Return Me.GetStatus(eVarNameFlags.SampleRating)
            End Get
            Set(value As eStatusFlags)
                Me.SetStatus(eVarNameFlags.SampleRating, value)
            End Set
        End Property

#End Region ' Variable access 

    End Class

End Namespace
