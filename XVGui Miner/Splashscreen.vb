Imports System.IO
Imports System.Net

Public Class Splashscreen
    Private Sub Splashscreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        Dim PicList As New List(Of PictureBox)

        PicList.Add(PictureBox1)
        PicList.Add(PictureBox2)

        Dim rnd = New Random()
        Dim randomColour = PicList(rnd.Next(0, PicList.Count))

        randomColour.Visible = False

        Dim cpuCount As Integer = My.Computer.Registry.LocalMachine.OpenSubKey("HARDWARE\DESCRIPTION\System\CentralProcessor", False).SubKeyCount
        My.Forms.Form1.LogInNumeric1.Maximum = cpuCount

        Dim x As New System.Threading.Thread(AddressOf Process1)
        x.Start()
    End Sub

    Private Sub Process1()
        'check online
        Threading.Thread.Sleep(5000)
        Label5.Text = "Connecting To Server..."
        Threading.Thread.Sleep(2000)
        Dim key As String = "ONLINE"
        Dim request2 As HttpWebRequest
        Dim response2 As HttpWebResponse

        Try
            request2 = DirectCast(WebRequest.Create("http://pastebin.com/raw/HY4GDwNG"), HttpWebRequest)
            request2.Timeout = 20000
            request2.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, Like Gecko) Chrome/32.0.1700.107 Safari/537.36"


            response2 = DirectCast(request2.GetResponse(), HttpWebResponse)
            Dim streamweb2 As Stream = response2.GetResponseStream()
            Dim readstreamweb2 As New StreamReader(streamweb2)
            While readstreamweb2.Peek >= 0
                Dim verp As String = readstreamweb2.ReadLine
                If verp.Contains(key) Then
                    Console.WriteLine(verp)

                    Dim c As New System.Threading.Thread(AddressOf Versioncheckz)
                    c.Start()
                    Label4.ForeColor = Color.Green
                    LogInProgressBar1.Increment(1)
                    Label5.Text = "Connection To Server Successful..."
                Else
                    Me.Enabled = False
                    Label5.Text = "Connecting To Server Failed"
                    MessageBox.Show("Server Offline - Download The Latest Version")
                    My.Forms.Form1.LogInTabControl1.Enabled = False
                    Me.Close()
                    My.Forms.Splashscreen.Close()
                    Application.Exit()
                    GoTo endz
                End If
            End While
            request2.Abort()
            response2.Close()

        Catch ex As Exception
            Me.Enabled = False
            MessageBox.Show("Server Offline - Download The Latest Version")
            My.Forms.Form1.LogInTabControl1.Enabled = False
            Me.Close()
            My.Forms.Splashscreen.Close()
            Application.Exit()
            GoTo endz
        Finally
            Try
                request2.Abort()
                response2.Close()
            Catch
            End Try
        End Try
        LogInProgressBar1.Increment(1)
        Threading.Thread.Sleep(5000)

        'check cpu miner
        Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim filepath As String = (strPath + "\Config Files\Config1\")
        Dim filename As String = ("cpuminer-multi.exe")

        Try
            Label5.Text = "Detecting CPU Components..."
            If Not System.IO.File.Exists(filepath + filename) Then
                LogInProgressBar1.Increment(1)
                MessageBox.Show("CPU Mining Component Missing")
            Else
                Label1.ForeColor = Color.Green
                LogInProgressBar1.Increment(1)
            End If
        Catch ex As Exception
        End Try

        Threading.Thread.Sleep(5000)

        'check nvidia gpu miner
        Dim strPath1 As String = AppDomain.CurrentDomain.BaseDirectory
        Dim filepath1 As String = (strPath + "\Config Files\Config2\")
        Dim filename1 As String = ("ccminer.exe")

        Try
            Label5.Text = "Detecting Nvidia GPU Components..."
            If Not System.IO.File.Exists(filepath1 + filename1) Then
                LogInProgressBar1.Increment(1)
                MessageBox.Show("Nvidia GPU Mining Component Missing")
            Else
                Label3.ForeColor = Color.Green
                LogInProgressBar1.Increment(1)
            End If
        Catch ex As Exception
        End Try

        Threading.Thread.Sleep(5000)

        'check amd gpu miner
        Dim strPath2 As String = AppDomain.CurrentDomain.BaseDirectory
        Dim filepath2 As String = (strPath + "\Config Files\Config3\")
        Dim filename2 As String = ("sgminer.exe")

        Try
            Label5.Text = "Detecting AMD GPU Components..."
            If Not System.IO.File.Exists(filepath2 + filename2) Then
                LogInProgressBar1.Increment(1)
                MessageBox.Show("AMD GPU Mining Component Missing")
            Else
                Label2.ForeColor = Color.Green
                LogInProgressBar1.Increment(1)
            End If
        Catch ex As Exception
        End Try
        Threading.Thread.Sleep(5000)
        Label5.Text = "Ready..."
        LogInProgressBar1.Increment(1)
        LogInButton1.Text = "Click To Continue"
        LogInButton1.Enabled = True
endz:
        PictureBox3.Visible = False
    End Sub

    Private Sub Versioncheckz()
        Try
            Dim key As String = "1.0.3"

            Dim request2 As HttpWebRequest = DirectCast(WebRequest.Create("http://pastebin.com/raw/HY4GDwNG"), HttpWebRequest)
            request2.Timeout = 20000
            request2.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, Like Gecko) Chrome/32.0.1700.107 Safari/537.36"

            Dim response2 As HttpWebResponse
            response2 = DirectCast(request2.GetResponse(), HttpWebResponse)
            Dim streamweb2 As Stream = response2.GetResponseStream()
            Dim readstreamweb2 As New StreamReader(streamweb2)
            While readstreamweb2.Peek >= 0
                Dim verp As String = readstreamweb2.ReadLine
                If verp.Contains(key) Then
                    Console.WriteLine(verp)
                    My.Forms.Form1.Label3.ForeColor = Color.DarkGray
                    My.Forms.Form1.Label3.Text = "Version: " + key + " - Up-To-Date"
                Else
                    My.Forms.Form1.Label3.ForeColor = Color.Red
                    My.Forms.Form1.Label3.Text = "Version: " + key + " - Out-Of-Date"
                    MessageBox.Show("This Version Is Detected Out-Of-Date")
                End If
            End While
            request2.Abort()
            response2.Close()
        Catch ex As Exception
            MessageBox.Show("Version Error - Could Not Retrieve Version Information")
        End Try
    End Sub

    Private Sub LogInButton1_Click(sender As Object, e As EventArgs) Handles LogInButton1.Click
        Try
            Me.Hide()
            My.Forms.Form1.Show()
        Catch ex As Exception
        End Try
    End Sub
End Class