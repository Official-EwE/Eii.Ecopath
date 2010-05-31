#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Core

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Taxonomy data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITaxonData

        ReadOnly Property CommonName() As String
        ReadOnly Property [Class]() As String
        ReadOnly Property Order() As String
        ReadOnly Property Family() As String
        ReadOnly Property Genus() As String
        ReadOnly Property Species() As String
        ReadOnly Property ISSCAAP() As String
        ReadOnly Property URL() As String
        ReadOnly Property Source() As String
        ReadOnly Property LastUpdated() As Date

    End Interface

End Namespace
