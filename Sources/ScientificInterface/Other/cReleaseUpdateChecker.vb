Imports System.Net.Http
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Threading.Tasks

Namespace Other

    Friend Class cReleaseUpdateChecker

        <DataContract>
        Private Class cGitHubRelease
            <DataMember(Name:="tag_name", IsRequired:=True)>
            Public Property TagName As String
        End Class

        Friend Shared Async Function GetLatestTagAsync() As Task(Of String)
            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromSeconds(15)
                client.DefaultRequestHeaders.UserAgent.ParseAdd("EwE6")
                client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json")
                Using response As HttpResponseMessage = Await client.GetAsync("https://api.github.com/repos/Official-EwE/Eii.Ecopath/releases/latest").ConfigureAwait(False)
                    response.EnsureSuccessStatusCode()
                    Using stream As IO.Stream = Await response.Content.ReadAsStreamAsync().ConfigureAwait(False)
                        Dim serializer As New DataContractJsonSerializer(GetType(cGitHubRelease))
                        Dim release As cGitHubRelease = TryCast(serializer.ReadObject(stream), cGitHubRelease)
                        If release Is Nothing Then Throw New SerializationException("The GitHub release response is empty.")
                        ParseVersion(release.TagName)
                        Return release.TagName
                    End Using
                End Using
            End Using
        End Function

        Friend Shared Function ParseVersion(value As String) As Version
            If String.IsNullOrWhiteSpace(value) Then Throw New FormatException("The release version is empty.")

            ' Ignore SemVer metadata on local builds; release tags use vMajor.Minor.Patch.
            Dim numericVersion As String = value.Trim().TrimStart("v"c).Split("+"c, "-"c)(0)
            Dim parsed As Version = Nothing
            If Not System.Version.TryParse(numericVersion, parsed) Then
                Throw New FormatException("Invalid release version: " & value)
            End If

            ' Version treats missing components as -1, so normalize them before comparison.
            Return New Version(parsed.Major, parsed.Minor, Math.Max(0, parsed.Build), Math.Max(0, parsed.Revision))
        End Function

    End Class

End Namespace
