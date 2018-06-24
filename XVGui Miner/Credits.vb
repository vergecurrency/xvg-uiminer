Imports System.IO
Imports System.Net

Public Class Credits
    Private Sub LinkLabel4_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        System.Diagnostics.Process.Start("https://gridcoregraphics.co.uk/")
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("https://vergecurrency.com/")
    End Sub

    Private Sub Credits_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim x As New System.Threading.Thread(AddressOf Donorz)
        x.Start()
    End Sub

    Private Sub Donorz()
        Try
            Dim request As WebRequest = WebRequest.Create("http://pastebin.com/raw/h2whBJVu")
            ' Get the response.
            Dim response As HttpWebResponse = CType(request.GetResponse, HttpWebResponse)
            ' Get the stream containing content returned by the server.
            Dim dataStream As Stream = response.GetResponseStream
            ' Open the stream using a StreamReader for easy access.
            Dim reader As StreamReader = New StreamReader(dataStream)
            ' Read the content.
            Dim responseFromServer As String = reader.ReadToEnd
            Dim str As String = responseFromServer
            Dim values() As String = str.Split("|")
            For Each value As String In values
                If (value.Trim = "") Then
                    'TODO: Warning!!! continue If
                End If

                ListBox1.Items.Add(value.Trim)
            Next
        Catch ex As Exception
            MessageBox.Show("Error Retrieving Donor List")
        End Try
    End Sub
End Class