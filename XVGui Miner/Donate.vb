Public Class Donate
    'litecoin address
    Private Sub LogInButton2_Click(sender As Object, e As EventArgs) Handles LogInButton2.Click
        Try
            Clipboard.SetText(TextBox13.Text)
            MessageBox.Show("Litecoin Address Copied")
        Catch ex As Exception
            MessageBox.Show("Error Occured Copying Litecoin Address - " + ex.Message)
        End Try
    End Sub

    'vergecoin
    Private Sub LogInButton1_Click(sender As Object, e As EventArgs) Handles LogInButton1.Click
        Try
            Clipboard.SetText(TextBox1.Text)
            MessageBox.Show("Vergecoin Address Copied")
        Catch ex As Exception
            MessageBox.Show("Error Occured Copying Vergecoin Address - " + ex.Message)
        End Try
    End Sub

    'stealth vergecoin
    Private Sub LogInButton3_Click(sender As Object, e As EventArgs) Handles LogInButton3.Click
        Try
            Clipboard.SetText(TextBox2.Text)
            MessageBox.Show("Stealth Vergecoin Address Copied")
        Catch ex As Exception
            MessageBox.Show("Error Occured Copying Stealth Vergecoin Address - " + ex.Message)
        End Try
    End Sub
End Class