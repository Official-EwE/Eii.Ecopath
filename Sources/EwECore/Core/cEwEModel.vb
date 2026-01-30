' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging

''' <summary>
''' Class to encapsulate and expose ecopath model for a single model
''' </summary>
Public Class cEwEModel
    Inherits cCoreInputOutputBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEwEModel)()

#Region " Constructor "

    Sub New(core As cCore)
        MyBase.New(core)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim desc() As Char

        Try

            Me.m_dataType = eDataTypes.EwEModel
            Me.m_coreComponent = eCoreComponentType.Ecopath

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' Description - use private metadata to allow more than the standard 254 characters
            meta = New cVariableMetaData(60000)
            val = New cValue(core, New String(desc), eVarNameFlags.Description, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str, meta)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New String(desc), eVarNameFlags.Author, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Contact
            val = New cValue(core, New String(desc), eVarNameFlags.Contact, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Area
            val = New cValue(core, New Single, eVarNameFlags.Area, eStatusFlags.OK, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            ' NumDigits
            val = New cValue(core, New Integer, eVarNameFlags.NumDigits, eStatusFlags.OK, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' FirstYear
            val = New cValue(core, New Integer, eVarNameFlags.EcopathFirstYear, eStatusFlags.OK, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' NumYears
            val = New cValue(core, New Integer, eVarNameFlags.EcopathNumYears, eStatusFlags.OK, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' North
            val = New cValue(core, New Single, eVarNameFlags.North, eStatusFlags.OK, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' South
            val = New cValue(core, New Single, eVarNameFlags.South, eStatusFlags.OK, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' East
            val = New cValue(core, New Single, eVarNameFlags.East, eStatusFlags.OK, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' West
            val = New cValue(core, New Single, eVarNameFlags.West, eStatusFlags.OK, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' GroupDigits
            val = New cValue(core, New Boolean, eVarNameFlags.GroupDigits, eStatusFlags.OK, eValueTypes.Bool)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Time unit (enum)
            val = New cValue(core, New Integer, eVarNameFlags.UnitTime, eStatusFlags.OK, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Time unit (text)
            val = New cValue(core, New String(desc), eVarNameFlags.UnitTimeCustomText, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Currency unit (enum)
            val = New cValue(core, New Integer, eVarNameFlags.UnitCurrency, eStatusFlags.OK, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Currency unit (text)
            val = New cValue(core, New String(desc), eVarNameFlags.UnitCurrencyCustomText, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Monetary unit (enum)
            val = New cValue(core, New String(desc), eVarNameFlags.UnitMonetary, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Area unit (enum)
            val = New cValue(core, New Integer, eVarNameFlags.UnitArea, eStatusFlags.OK, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Area unit (text)
            val = New cValue(core, New String(desc), eVarNameFlags.UnitAreaCustomText, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Map georeferencing unit (enum)
            val = New cValue(core, New Integer, eVarNameFlags.UnitMapRef, eStatusFlags.OK, eValueTypes.Int)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Country
            val = New cValue(core, New String(desc), eVarNameFlags.Country, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New String(desc), eVarNameFlags.EcosystemType, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Ecobase code
            val = New cValue(core, New String(desc), eVarNameFlags.CodeEcobase, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' DOI
            val = New cValue(core, New String(desc), eVarNameFlags.PublicationDOI, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            Me.m_values.Add(val.varName, val)

            ' URI
            val = New cValue(core, New String(desc), eVarNameFlags.PublicationURI, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Reference
            val = New cValue(core, New String(desc), eVarNameFlags.PublicationReference, eStatusFlags.OK Or eStatusFlags.Null, eValueTypes.Str)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' Last saved julian date
            val = New cValue(core, New Double, eVarNameFlags.LastSaved, eStatusFlags.OK, eValueTypes.Sng)
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            ' IsEcopaceCoupled
            val = New cValue(core, New Boolean, eVarNameFlags.IsEcospaceModelCoupled, eStatusFlags.OK, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            ' DiversityIndex (enum)
            val = New cValue(core, New Integer, eVarNameFlags.DiversityIndex, eStatusFlags.OK, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'set status flags to their default values
            Me.ResetStatusFlags()

        Catch ex As Exception
            m_logger.LogError(ex, "Error creating new cModel.")
        End Try

    End Sub

#End Region ' Constructor

#Region " Variable via dot(.) operator "

    Public Property Description() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Description))
        End Get

        Set(str As String)
            Me.SetVariable(eVarNameFlags.Description, str)
        End Set
    End Property

    Public Property Author() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Author))
        End Get

        Set(str As String)
            Me.SetVariable(eVarNameFlags.Author, str)
        End Set
    End Property

    Public Property Contact() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Contact))
        End Get

        Set(str As String)
            Me.SetVariable(eVarNameFlags.Contact, str)
        End Set
    End Property

    Public Property Area() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.Area))
        End Get

        Set(sArea As Single)
            Me.SetVariable(eVarNameFlags.Area, sArea)
        End Set
    End Property

    Public Property NumDigits() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.NumDigits))
        End Get

        Set(iNumDigits As Integer)
            Me.SetVariable(eVarNameFlags.NumDigits, iNumDigits)
        End Set
    End Property

    Public Property GroupDigits() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.GroupDigits))
        End Get

        Set(bGroupDigits As Boolean)
            Me.SetVariable(eVarNameFlags.GroupDigits, bGroupDigits)
        End Set
    End Property

    Public Property DiversityIndexType() As eDiversityIndexType
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.DiversityIndex), eDiversityIndexType)
        End Get

        Set(i As eDiversityIndexType)
            Me.SetVariable(eVarNameFlags.DiversityIndex, CInt(i))
        End Set
    End Property

    Public Property UnitTime() As eUnitTimeType
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.UnitTime), eUnitTimeType)
        End Get

        Set(i As eUnitTimeType)
            Me.SetVariable(eVarNameFlags.UnitTime, CInt(i))
        End Set
    End Property

    Public Property UnitTimeCustomText() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.UnitTimeCustomText))
        End Get

        Set(str As String)
            Me.SetVariable(eVarNameFlags.UnitTimeCustomText, str)
        End Set
    End Property

    Public Property UnitCurrency() As eUnitCurrencyType
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.UnitCurrency), eUnitCurrencyType)
        End Get

        Set(i As eUnitCurrencyType)
            Me.SetVariable(eVarNameFlags.UnitCurrency, CInt(i))
        End Set
    End Property

    Public Property UnitCurrencyCustomText() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.UnitCurrencyCustomText))
        End Get

        Set(str As String)
            Me.SetVariable(eVarNameFlags.UnitCurrencyCustomText, str)
        End Set
    End Property

    Public Property UnitMonetary() As String
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.UnitMonetary), String)
        End Get

        Set(strUnit As String)
            Me.SetVariable(eVarNameFlags.UnitMonetary, strUnit)
        End Set
    End Property

    Public Property UnitArea() As eUnitAreaType
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.UnitArea), eUnitAreaType)
        End Get

        Set(i As eUnitAreaType)
            Me.SetVariable(eVarNameFlags.UnitArea, CInt(i))
        End Set
    End Property

    Public Property UnitAreaCustomText() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.UnitAreaCustomText))
        End Get

        Set(str As String)
            Me.SetVariable(eVarNameFlags.UnitAreaCustomText, str)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the first year that a model represents.
    ''' </summary>
    Public Property FirstYear() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcopathFirstYear))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcopathFirstYear, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the number of years that a model represents.
    ''' </summary>
    Public Property NumYears() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.EcopathNumYears))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.EcopathNumYears, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the southern extent of the model bounding box in decimal degrees.
    ''' </summary>
    Public Property South() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.South))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.South, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the northern extent of the model bounding box in decimal degrees.
    ''' </summary>
    Public Property North() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.North))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.North, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the western extent of the model bounding box in decimal degrees.
    ''' </summary>
    Public Property West() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.West))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.West, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the eastern extent of the model bounding box in decimal degrees.
    ''' </summary>
    Public Property East() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.East))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.East, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the top left extent of the model bounding box in decimal degrees.
    ''' </summary>
    Public Property PosTopLeft As Drawing.PointF
        Get
            Return New Drawing.PointF(Me.West, Me.North)
        End Get
        Set(value As Drawing.PointF)
            Me.West = value.X
            Me.North = value.Y
        End Set
    End Property

    ''' <summary>
    ''' Get/set the bottom right extent of the model bounding box in decimal degrees.
    ''' </summary>
    Public Property PosBottomRight As Drawing.PointF
        Get
            Return New Drawing.PointF(Me.East, Me.South)
        End Get
        Set(value As Drawing.PointF)
            Me.East = value.X
            Me.South = value.Y
        End Set
    End Property

    ''' <summary>
    ''' Get/set the Julian date the model was last saved.
    ''' </summary>
    Public Property LastSaved() As Double
        Get
            Return CDbl(Me.GetVariable(eVarNameFlags.LastSaved))
        End Get

        Set(value As Double)
            Me.SetVariable(eVarNameFlags.LastSaved, value)
        End Set
    End Property

    Public Property IsEcoSpaceModelCoupled() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.IsEcospaceModelCoupled))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.IsEcospaceModelCoupled, value)
        End Set
    End Property

    Public Property Country As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Country))
        End Get

        Set(value As String)
            Me.SetVariable(eVarNameFlags.Country, value)
        End Set
    End Property

    Public Property EcosystemType As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.EcosystemType))
        End Get

        Set(value As String)
            Me.SetVariable(eVarNameFlags.EcosystemType, value)
        End Set
    End Property

    Public Property EcobaseCode As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.CodeEcobase))
        End Get

        Set(value As String)
            Me.SetVariable(eVarNameFlags.CodeEcobase, value)
        End Set
    End Property

    Public Property PublicationDOI As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.PublicationDOI))
        End Get

        Set(value As String)
            Me.SetVariable(eVarNameFlags.PublicationDOI, value)
        End Set
    End Property

    Public Property PublicationURI As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.PublicationURI))
        End Get

        Set(value As String)
            Me.SetVariable(eVarNameFlags.PublicationURI, value)
        End Set
    End Property

    Public Property PublicationReference As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.PublicationReference))
        End Get

        Set(value As String)
            Me.SetVariable(eVarNameFlags.PublicationReference, value)
        End Set
    End Property

#End Region ' Variable via dot(.) operator

#Region " Status Flags via dot(.) operator"

    Public Property DescriptionStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.Description)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.Description, value)
        End Set

    End Property

    Public Property NumDigitsStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.NumDigits)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.NumDigits, value)
        End Set

    End Property

    ' ToDo: all all other vars

#End Region ' Status Flags via dot(.) operator

End Class
