'==============================================================================
'
' $Log: cPSDParameters.vb,v $
' Revision 1.1  2009/03/16 16:55:57  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwEUtils.Core
Imports EwECore.ValueWrapper

''' <summary>
''' This class wraps the underlying particle size distribution data structures
''' </summary>
Public Class cPSDParameters
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Public Sub New(ByRef m_core As cCore)
        MyBase.New(m_core)


        Try
            'no data validation at this time
            Me.AllowValidation = False
            m_coreComponent = eCoreComponentType.EcoPath
            m_dataType = eDataTypes.ParticleSizeDistribution

            Dim val As cValue
            Dim meta As cVariableMetaData

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, m_dataType, m_coreComponent, Index, cCore.NULL_VALUE)

            ' Define variables here

            Me.AllowValidation = True

        Catch ex As Exception

            Debug.Assert(False, ex.Message)
            cLog.Write(Me.ToString & ".New() Error: " & ex.Message)

        End Try

    End Sub

#End Region

#Region "Variables via dot (.) operator"

    ''' <summary>
    ''' </summary>
    ''' <value></value>
    Public Property MyFristParameterYippee() As Single

        Get
            'Return CType(GetVariable(eVarNameFlags.NotSet), Single)
            Return 42.0!
        End Get

        Set(ByVal value As Single)
            'SetVariable(eVarNameFlags.NotSet, value)
        End Set

    End Property

#End Region

End Class
