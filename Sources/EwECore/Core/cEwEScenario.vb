'==============================================================================
'
' $Log: cEwEScenario.vb,v $
' Revision 1.1  2008/09/26 07:30:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/08/15 17:14:09  jeroens
' Cannot delete a loaded scenario
'
' Revision 1.7  2008/05/29 22:22:47  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.6  2008/05/08 00:50:18  jeroens
' Fixed bug 467
'
' Revision 1.5  2008/02/13 03:53:41  jeroens
' Added IsLoaded()
'
' Revision 1.4  2008/01/11 09:53:44  jeroens
' LastSaved date changed to Single to include time
'
' Revision 1.3  2008/01/08 23:13:29  jeroens
' Added LastSaved date
'
' Revision 1.2  2007/12/09 22:12:46  jeroens
' * Uses new dataentity Datasource
'
' Revision 1.1  2007/12/07 14:40:44  jeroens
' Initial version, bundles all replicated code across three different scenario classes
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public MustInherit Class cEwEScenario
    Inherits cCoreInputOutputBase

#Region " Constructor "

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim desc() As Char

        Try

            m_DataType = eDataTypes.NotSet
            ' Scenario definition changes do not affect the running state of the model
            m_messageSource = eMessageSource.DataSource

            'default OK status used for setVariable
            'see comment setVariable(...)
            m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimScenario, eMessageSource.EcoSim, Index, cCore.NULL_VALUE)

            ' Description
            meta = New cVariableMetaData(60000)
            val = New cValue(New String(desc), eVarNameFlags.Description, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Author
            meta = New cVariableMetaData(60)
            val = New cValue(New String(desc), eVarNameFlags.Author, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Contact
            meta = New cVariableMetaData(250)
            val = New cValue(New String(desc), eVarNameFlags.Contact, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Last saved julian date
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(New Single, eVarNameFlags.LastSaved, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.LastSaved))
            m_values.Add(val.varName, val)

            'set status flags to their default values
            ResetStatusFlags()
            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEwEScenario.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEwEScenario. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Public access "

    Public MustOverride Function IsLoaded() As Boolean

#End Region ' Public access

#Region " Variable via dot(.) operator"

    Public Property Description() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.Description))
        End Get

        Set(ByVal str As String)
            SetVariable(eVarNameFlags.Description, str)
        End Set
    End Property

    Public Property Author() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.Author))
        End Get

        Set(ByVal str As String)
            SetVariable(eVarNameFlags.Author, str)
        End Set
    End Property

    Public Property Contact() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.Contact))
        End Get

        Set(ByVal str As String)
            SetVariable(eVarNameFlags.Contact, str)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the Julian date the scenario was last saved.
    ''' </summary>
    Public Property LastSaved() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.LastSaved))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.LastSaved, value)
        End Set
    End Property

#End Region ' Variable via dot(.) operator

#Region " Status Flags via dot(.) operator"

    Public Property DescriptionStatus() As eStatusFlags

        Get
            Return GetStatus(eVarNameFlags.Description)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.Description, value)
        End Set

    End Property

#End Region ' Status Flags via dot(.) operator

End Class
