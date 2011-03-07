Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Taxonomy definition that contributes to a functional group.
''' </summary>
Public Class cTaxon
    Inherits cCoreInputOutputBase
    Implements ITaxonSearchData
    Implements ITaxonDetailsData

#Region " Construction and Intialization "

    Friend Sub New(ByRef theCore As cCore, ByVal DBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing
        Dim cbuf() As Char
        Dim validator As cValidatorDefault

        Me.AllowValidation = False

        Me.m_coreComponent = eCoreComponentType.EcoPath
        Me.m_dataType = eDataTypes.Taxon
        Me.DBID = DBID
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        ' Taxon group
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.TaxonGroup, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(cbuf), eVarNameFlags.Class, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(cbuf), eVarNameFlags.Phylum, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(cbuf), eVarNameFlags.Order, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(cbuf), eVarNameFlags.Family, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(cbuf), eVarNameFlags.Genus, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(cbuf), eVarNameFlags.Species, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(cbuf), eVarNameFlags.CommonName, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(2)
        val = New cValue(New String(cbuf), eVarNameFlags.CodeISSCAAP, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(13)
        val = New cValue(New String(cbuf), eVarNameFlags.CodeTaxon, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(3)
        val = New cValue(New String(cbuf), eVarNameFlags.Code3A, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(250)
        val = New cValue(New String(cbuf), eVarNameFlags.Source, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        meta = New cVariableMetaData(1024)
        val = New cValue(New String(cbuf), eVarNameFlags.SourceKey, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, meta, validator)
        m_values.Add(val.varName, val)

        ' North
        meta = New cVariableMetaData(-90, 90, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.North, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        ' South
        meta = New cVariableMetaData(-90, 90, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.South, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        ' East
        meta = New cVariableMetaData(-180, 180, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.East, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        ' West
        meta = New cVariableMetaData(-180, 180, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), cCore.NULL_VALUE)
        val = New cValue(New Single, eVarNameFlags.West, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

        ' Proportion
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.TaxonProp, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.TaxonProp))
        m_values.Add(val.varName, val)

        ' EcologyType
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.EcologyType, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.EcologyType))
        m_values.Add(val.varName, val)

        ' OrganismType
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.OrganismType, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.OrganismType))
        m_values.Add(val.varName, val)

        ' Exploited
        meta = New cVariableMetaData(False)
        val = New cValue(New Boolean, eVarNameFlags.Exploited, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.Exploited))
        m_values.Add(val.varName, val)

        ' IUCNConservationStatus
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.IUCNConservationStatus, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.IUCNConservationStatus))
        m_values.Add(val.varName, val)

        ' OccurrenceStatus
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Integer, eVarNameFlags.OccurrenceStatus, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.OccurrenceStatus))
        m_values.Add(val.varName, val)

        ' TaxonMeanWeight
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.TaxonMeanWeight, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.TaxonMeanWeight))
        m_values.Add(val.varName, val)

        ' TaxonMeanLength
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.TaxonMeanLength, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.TaxonMeanLength))
        m_values.Add(val.varName, val)

        ' TaxonMaxLength
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.TaxonMaxLength, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.TaxonMaxLength))
        m_values.Add(val.varName, val)

        ' TaxonMeanLifespan
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.TaxonMeanLifespan, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.TaxonMeanLifespan))
        m_values.Add(val.varName, val)

        ' Last updated julian date
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThan))
        val = New cValue(New Single, eVarNameFlags.LastUpdated, eStatusFlags.OK, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.LastUpdated))
        m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

#End Region

#Region " Variables via dot (.) operator "

    ''' <summary>
    ''' Get/set the index of the Ecopath group that a taxonomy definition contributes to.
    ''' </summary>
    Public Property Group() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.TaxonGroup))
        End Get
        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.TaxonGroup, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the proportion that a taxonomy definition contributes to a <see cref="Group">group</see>.
    ''' </summary>
    Public Property Proportion() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonProp))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.TaxonProp, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the phylum of a taxonomy definition.
    ''' </summary>
    Public Property Phylum() As String _
        Implements ITaxonDetailsData.Phylum
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Phylum))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Phylum, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the class of a taxonomy definition.
    ''' </summary>
    Public Property [Class]() As String _
        Implements ITaxonDetailsData.Class
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Class))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Class, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the order of a taxonomy definition.
    ''' </summary>
    Public Property Order() As String _
        Implements ITaxonDetailsData.Order
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Order))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Order, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the family of a taxonomy definition.
    ''' </summary>
    Public Property Family() As String _
        Implements ITaxonDetailsData.Family
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Family))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Family, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the genus of a taxonomy definition.
    ''' </summary>
    Public Property Genus() As String _
        Implements ITaxonDetailsData.Genus
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Genus))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Genus, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the species of a taxonomy definition.
    ''' </summary>
    Public Property Common() As String _
        Implements ITaxonDetailsData.Common
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.CommonName))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.CommonName, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the species of a taxonomy definition.
    ''' </summary>
    Public Property Species() As String _
        Implements ITaxonDetailsData.Species
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Species))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Species, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the 2 digit ISSCAAP code of a taxonomy definition.
    ''' </summary>
    Public Property CodeISSCAAP() As String _
        Implements ITaxonDetailsData.CodeISSCAAP
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.CodeISSCAAP))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.CodeISSCAAP, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the 10 digit Taxonomy code of a taxonomy definition.
    ''' </summary>
    Public Property CodeTaxon() As String _
        Implements ITaxonDetailsData.CodeTaxon
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.CodeTaxon))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.CodeTaxon, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the 3 character 3A code of a taxonomy definition.
    ''' </summary>
    Public Property Code3A() As String _
        Implements ITaxonDetailsData.Code3A
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Code3A))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Code3A, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the name of the source that a taxonomy definition was obtained from.
    ''' </summary>
    Public Property Source() As String _
        Implements ITaxonDetailsData.Source
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Source))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.Source, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the key to refresh a taxonomy definition from the <see cref="Source">source</see>.
    ''' </summary>
    Public Property SourceKey() As String _
        Implements ITaxonDetailsData.SourceKey
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.SourceKey))
        End Get
        Set(ByVal value As String)
            Me.SetVariable(eVarNameFlags.SourceKey, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the southern extent of the model bounding box.
    ''' </summary>
    Public Property South() As Single _
        Implements ITaxonDetailsData.South
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.South))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.South, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the northern extent of the model bounding box.
    ''' </summary>
    Public Property North() As Single _
        Implements ITaxonDetailsData.North
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.North))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.North, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the western extent of the model bounding box.
    ''' </summary>
    Public Property West() As Single _
        Implements ITaxonDetailsData.West
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.West))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.West, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the eastern extent of the model bounding box.
    ''' </summary>
    Public Property East() As Single _
        Implements ITaxonDetailsData.East
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.East))
        End Get

        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.East, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eEcologyTypes"/> for a taxon.
    ''' </summary>
    Public Property EcologyType As eEcologyTypes
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcologyType), eEcologyTypes)
        End Get
        Set(ByVal value As eEcologyTypes)
            Me.SetVariable(eVarNameFlags.EcologyType, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eOrganismTypes"/> for a taxon.
    ''' </summary>
    Public Property OrganismType As eOrganismTypes
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.OrganismType), eOrganismTypes)
        End Get
        Set(ByVal value As eOrganismTypes)
            Me.SetVariable(eVarNameFlags.OrganismType, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set whether the taxon is exploited.
    ''' </summary>
    Public Property Exploited() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.Exploited))
        End Get
        Set(ByVal value As Boolean)
            Me.SetVariable(eVarNameFlags.Exploited, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eIUCNConservationStatusTypes"/> for a taxon.
    ''' </summary>
    Public Property IUCNConservationStatus As eIUCNConservationStatusTypes
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.IUCNConservationStatus), eIUCNConservationStatusTypes)
        End Get
        Set(ByVal value As eIUCNConservationStatusTypes)
            Me.SetVariable(eVarNameFlags.IUCNConservationStatus, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eOccurrenceStatusTypes"/> for a taxon.
    ''' </summary>
    Public Property OccurrenceStatus As eOccurrenceStatusTypes
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.OccurrenceStatus), eOccurrenceStatusTypes)
        End Get
        Set(ByVal value As eOccurrenceStatusTypes)
            Me.SetVariable(eVarNameFlags.OccurrenceStatus, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean weight for a taxon.
    ''' </summary>
    Public Property MeanWeight As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonMeanWeight))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.TaxonMeanWeight, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean length for a taxon.
    ''' </summary>
    Public Property MeanLength As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonMeanLength))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.TaxonMeanLength, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the max length for a taxon.
    ''' </summary>
    Public Property MaxLength As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonMaxLength))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.TaxonMaxLength, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean life span for a taxon.
    ''' </summary>
    Public Property MeanLifespan As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonMeanLifespan))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.TaxonMeanLifespan, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the Julian date the taxonomy definition was last updated.
    ''' </summary>
    Public Property LastUpdated() As Double _
        Implements ITaxonDetailsData.LastUpdated
        Get
            Return CDbl(GetVariable(eVarNameFlags.LastUpdated))
        End Get

        Set(ByVal value As Double)
            SetVariable(eVarNameFlags.LastUpdated, value)
        End Set
    End Property

#End Region

End Class
