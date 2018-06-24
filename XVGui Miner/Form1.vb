Imports System
Imports System.IO
Imports System.Net
Imports System.Management
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions

Public Class Form1

    Dim CPUProcess As New Process()
    Dim NvidiaGPUProcess As New Process()
    Dim AMDGPUProcess As New Process()

    'cpu spec check
    Private Sub LogInButton7_Click(sender As Object, e As EventArgs) Handles LogInButton7.Click
        Dim cpuname As String
        cpuname = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\SYSTEM\CentralProcessor\0", "ProcessorNameString", Nothing)
        TextBox13.Text = cpuname
        Dim cpuSpeed As String
        cpuSpeed = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\SYSTEM\CentralProcessor\0", "~MHz", Nothing) & " MHz"
        TextBox14.Text = cpuSpeed
        Dim cpuCount As Integer = My.Computer.Registry.LocalMachine.OpenSubKey("HARDWARE\DESCRIPTION\System\CentralProcessor", False).SubKeyCount
        TextBox15.Text = cpuCount
    End Sub

    Private Sub LogInButton8_Click(sender As Object, e As EventArgs) Handles LogInButton8.Click
        Dim Location As New ObjectQuery("SELECT * FROM Win32_VideoController")
        Dim Search As New ManagementObjectSearcher(Location)

        For Each Found As ManagementObject In Search.Get
            TextBox16.Text = Found("Name")
            TextBox17.Text = Found("VideoProcessor")
            TextBox18.Text = Convert.ToUInt64(Found("AdapterRAM") / 1024 / 1024).ToString()
        Next
    End Sub


    'cpu save settings
    Private Sub LogInButton3_Click(sender As Object, e As EventArgs) Handles LogInButton3.Click
        Dim x As New System.Threading.Thread(AddressOf cpusavez)
        x.Start()
    End Sub

    Private Sub cpusavez()
        Try
            Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory
            Dim filepath As String = (strPath + "\Config Files\Settings\")
            Dim filename As String = ("CPU-Settings.db")

            If Not System.IO.File.Exists(filepath + filename) Then
                System.IO.Directory.CreateDirectory(filepath)
                System.IO.File.Create(filepath + filename).Dispose()
                MessageBox.Show("Setting File Was NOT FOUND - Setting Files Created... Again - Please Save Again")
            Else
                Using sw As New IO.StreamWriter(filepath + filename)
                    'wallet / username  
                    sw.Write(TextBox1.Text)
                    sw.Write("|")
                    'password
                    sw.Write(TextBox19.Text)
                    sw.Write("|")
                    'algo
                    sw.Write(LogInComboBox1.Text)
                    sw.Write("|")
                    'pool
                    If LogInComboBox3.Text = "CUSTOM" Then
                        sw.Write(TextBox2.Text)
                        sw.Write("|")
                    Else
                        sw.Write("|")
                    End If
                    'cpu threads
                    sw.Write(LogInNumeric1.Value)
                    sw.Close()
                End Using
                MessageBox.Show("CPU Settings Successfully Saved")
            End If

        Catch ex As Exception
            MessageBox.Show("Error Occurred - " + ex.Message)
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False

        Dim c As New System.Threading.Thread(AddressOf Checkz)
        c.Start()

        Dim cpu As New System.Threading.Thread(AddressOf CPUSettings)
        cpu.Start()

        Dim gpu As New System.Threading.Thread(AddressOf GPUSettings)
        gpu.Start()

        XVGValue.RunWorkerAsync()
    End Sub


    Private Sub Checkz()

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

                Else
                    Me.Enabled = False
                    MessageBox.Show("Server Offline - Download The Latest Version")
                    LogInTabControl1.Enabled = False
                    Application.Exit()
                End If
            End While
            request2.Abort()
            response2.Close()

        Catch ex As Exception
            Me.Enabled = False
            MessageBox.Show("Server Offline - Download The Latest Version")
            Me.Close()
            Application.Exit()
        Finally
            Try
                request2.Abort()
                response2.Close()
            Catch
            End Try
        End Try
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
                    Label3.ForeColor = Color.DarkGray
                    Label3.Text = "Version: " + key + " - Up-To-Date"
                Else
                    Label3.ForeColor = Color.Red
                    Label3.Text = "Version: " + key + " - Out-Of-Date"
                End If
            End While
            request2.Abort()
            response2.Close()
        Catch ex As Exception
            MessageBox.Show("Version Error - Could Not Retrieve Version Information")
        End Try
    End Sub

    Private Sub CPUSettings()
        Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim filepath As String = (strPath + "\Config Files\Settings\")
        Dim filename As String = ("CPU-Settings.db")

        Try
            If Not System.IO.File.Exists(filepath + filename) Then
                System.IO.Directory.CreateDirectory(filepath)
                System.IO.File.Create(filepath + filename).Dispose()
                MessageBox.Show("First Time User - Setting Files Created")
            Else
                Dim inputstream As New IO.StreamReader(filepath + filename)
                Dim newstr(5) As String

                'Read while there is mor characters to read
                Do While inputstream.Peek > 0
                    'Split each line containing Account|Password into the array
                    newstr = inputstream.ReadLine().Split("|")
                    'Assigm the values to the variables

                    'Add them to the list
                    TextBox1.Text = (newstr(0))
                    TextBox19.Text = (newstr(1))
                    LogInComboBox1.Text = (newstr(2))
                    TextBox2.Text = (newstr(3))
                    LogInNumeric1.Value = (newstr(4))
                Loop
                inputstream.Close()
                inputstream.Dispose()
            End If
        Catch ex As Exception
            MessageBox.Show("Error Occurred - " + ex.Message)
        End Try
    End Sub

    Private Sub GPUSettings()
        Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim filepath As String = (strPath + "\Config Files\Settings\")
        Dim filename As String = ("GPU-Settings.db")

        Try
            If Not System.IO.File.Exists(filepath + filename) Then
                System.IO.Directory.CreateDirectory(filepath)
                System.IO.File.Create(filepath + filename).Dispose()
            Else
                Dim inputstream As New IO.StreamReader(filepath + filename)
                Dim newstr(5) As String

                'Read while there is mor characters to read
                Do While inputstream.Peek > 0
                    'Split each line containing Account|Password into the array
                    newstr = inputstream.ReadLine().Split("|")
                    'Assigm the values to the variables

                    'Add them to the list
                    'username
                    TextBox7.Text = (newstr(0))
                    'pass
                    TextBox20.Text = (newstr(1))
                    'gpu type
                    LogInComboBox2.Text = (newstr(2))
                    'algo
                    LogInComboBox4.Text = (newstr(3))
                    'pool
                    TextBox8.Text = (newstr(4))
                Loop
                inputstream.Close()
                inputstream.Dispose()
            End If
        Catch ex As Exception
            MessageBox.Show("Error Occurred - " + ex.Message)
        End Try
    End Sub

    'cpu start
    Private Sub LogInButton1_Click(sender As Object, e As EventArgs) Handles LogInButton1.Click
        If TextBox1.Text.Count > 1 Then
            If TextBox2.Text.Count > 1 Then
                If TextBox19.Text = "" Then
                    TextBox19.Text = "x"
                End If
                CPUMine.WorkerSupportsCancellation = True
                    CPUMine.RunWorkerAsync()
                    LogInButton1.Enabled = False
                    LogInButton2.Enabled = True
                LogInButton3.Enabled = False
                LogInButton9.Enabled = False
                LogInComboBox1.Enabled = False
                    LogInComboBox3.Enabled = False
                    LogInNumeric1.Enabled = False
                    TextBox2.Enabled = False
                    TextBox1.Enabled = False
                    TextBox19.Enabled = False
                    TextBox6.Text = "Starting..."
                Else
                    MessageBox.Show("No Pool Address / Wallet? ", "CPU Pool Settings Error", MessageBoxButtons.OKCancel)
            End If
        Else
            MessageBox.Show("No Pool Address / Wallet? ", "CPU Pool Settings Error", MessageBoxButtons.OKCancel)
        End If
    End Sub

    'cpu stop
    Private Sub LogInButton2_Click(sender As Object, e As EventArgs) Handles LogInButton2.Click
        Try
            CPUProcess.Kill()
            CPUMine.CancelAsync()
        Catch
        End Try
        LogInButton1.Enabled = True
        LogInButton2.Enabled = False
        LogInButton3.Enabled = True
        LogInButton9.Enabled = True
        LogInComboBox1.Enabled = True
        LogInComboBox3.Enabled = True
        LogInNumeric1.Enabled = True
        If LogInComboBox3.Text = "CUSTOM" Then
            TextBox2.Enabled = True
        Else
        End If
        TextBox1.Enabled = True
        TextBox19.Enabled = True
        TextBox6.Text = "Stopped..."
        TextBox3.Text = "0"
        TextBox4.Text = "0"
        TextBox5.Text = "0"
    End Sub

    'cpu backworker
    Private Sub CPUMine_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs) Handles CPUMine.DoWork
        Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim pool_address As String = TextBox2.Text
        Dim wallet As String = TextBox1.Text
        Dim pass As String = TextBox19.Text
        Dim threads As String = LogInNumeric1.Value
        Dim algo As String = LogInComboBox1.Text

        Dim sOutput As String
        Dim splitOutput() As String
        Try
            Dim oStartInfo As New ProcessStartInfo(strPath + "\Config Files\Config1\cpuminer-multi.exe",
                                       "-a " + algo + " -o " + pool_address + " -u " + wallet + " -p " + pass + " -t " + threads)

            oStartInfo.UseShellExecute = False
            oStartInfo.RedirectStandardOutput = True
            oStartInfo.CreateNoWindow = True
            CPUProcess.StartInfo = oStartInfo
            TextBox6.Text = "Starting Process"
            CPUProcess.Start()
        Catch ex As Exception
            TextBox6.Text = "ERROR - " + ex.Message
            GoTo endz
        End Try

        Try
            Using oStreamReader As System.IO.StreamReader = CPUProcess.StandardOutput
                While True
                    sOutput = oStreamReader.ReadLine()

                    splitOutput = sOutput.Split(New Char() {",", " ", "/"})
                    If sOutput.Contains("yes!") Then
                        TextBox4.Text = (splitOutput(3))
                        TextBox5.Text = (splitOutput(4) - splitOutput(3))
                        'TextBox8.Text = Val(TextBox9.Text) + Val(TextBox10.Text)
                        TextBox3.Text = (splitOutput(10))
                    End If
                    TextBox6.Text = sOutput
                End While
            End Using
        Catch ex As Exception
            GoTo endz
        End Try
endz:
        LogInButton1.Enabled = True
        LogInButton2.Enabled = False
        LogInButton3.Enabled = True
        LogInComboBox1.Enabled = True
        LogInComboBox3.Enabled = True
        LogInNumeric1.Enabled = True
        If LogInComboBox3.Text = "CUSTOM" Then
            TextBox2.Enabled = True
        Else
        End If
        TextBox1.Enabled = True
        TextBox19.Enabled = True
        TextBox3.Text = "0"
        TextBox4.Text = "0"
        TextBox5.Text = "0"
    End Sub

    'GPU save settings
    Private Sub LogInButton4_Click(sender As Object, e As EventArgs) Handles LogInButton4.Click
        Dim x As New System.Threading.Thread(AddressOf gpusavez)
        x.Start()
    End Sub

    Private Sub gpusavez()
        Try
            Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory
            Dim filepath As String = (strPath + "\Config Files\Settings\")
            Dim filename As String = ("GPU-Settings.db")

            If Not System.IO.File.Exists(filepath + filename) Then
                System.IO.Directory.CreateDirectory(filepath)
                System.IO.File.Create(filepath + filename).Dispose()
                MessageBox.Show("Setting File Was NOT FOUND - Setting Files Created... Again - Please Save Again")
            Else
                Using sw As New IO.StreamWriter(filepath + filename)
                    'username
                    sw.Write(TextBox7.Text)
                    sw.Write("|")
                    'password
                    sw.Write(TextBox20.Text)
                    sw.Write("|")
                    'gpu type
                    sw.Write(LogInComboBox2.Text)
                    sw.Write("|")
                    'algo
                    sw.Write(LogInComboBox4.Text)
                    sw.Write("|")
                    'pool
                    If LogInComboBox5.Text = "CUSTOM" Then
                        sw.Write(TextBox8.Text)
                        sw.Write("|")
                    Else
                        sw.Write("|")
                    End If
                End Using
                MessageBox.Show("GPU Settings Successfully Saved")
            End If

        Catch ex As Exception
            MessageBox.Show("Error Occurred - " + ex.Message)
        End Try
    End Sub

    'start gpu mine
    Private Sub LogInButton6_Click(sender As Object, e As EventArgs) Handles LogInButton6.Click
        Try
            If TextBox7.Text.Count > 1 Then
                If TextBox8.Text.Count > 1 Then
                    If TextBox20.Text = "" Then
                        TextBox20.Text = "x"
                    End If
                    If LogInComboBox2.Text = "NVIDIA" Then
                        LogInButton4.Enabled = False
                        LogInButton5.Enabled = True
                        LogInButton6.Enabled = False
                        LogInButton10.Enabled = False
                        LogInComboBox2.Enabled = False
                        LogInComboBox4.Enabled = False
                        LogInComboBox5.Enabled = False
                        TextBox20.Enabled = False
                        TextBox7.Enabled = False
                        If LogInComboBox5.Text = "CUSTOM" Then
                            TextBox8.Enabled = False
                        End If
                        TextBox12.Text = "NVIDIA Starting..."
                        NVIDIAGPUMine.WorkerSupportsCancellation = True
                        NVIDIAGPUMine.RunWorkerAsync()
                    ElseIf LogInComboBox2.Text = "AMD" Then
                        LogInButton4.Enabled = False
                        LogInButton5.Enabled = True
                        LogInButton6.Enabled = False
                        LogInComboBox2.Enabled = False
                        LogInComboBox4.Enabled = False
                        LogInComboBox5.Enabled = False
                        TextBox20.Enabled = False
                        TextBox7.Enabled = False
                        If LogInComboBox5.Text = "CUSTOM" Then
                            TextBox8.Enabled = False
                        End If
                        TextBox12.Text = "AMD Starting..."
                        AMDGPUMine.WorkerSupportsCancellation = True
                        AMDGPUMine.RunWorkerAsync()
                    Else
                        MessageBox.Show("No Pool Address / Wallet? ", "GPU Pool Settings Error", MessageBoxButtons.OKCancel)
                    End If
                Else
                    MessageBox.Show("No Pool Address / Wallet? ", "GPU Pool Settings Error", MessageBoxButtons.OKCancel)
                End If
            End If
        Catch ex As Exception
            TextBox12.Text = "Error Occurred - " + ex.Message
        End Try
    End Sub

    'gui stop mine
    Private Sub LogInButton5_Click(sender As Object, e As EventArgs) Handles LogInButton5.Click
        Try
            NvidiaGPUProcess.Kill()
            NVIDIAGPUMine.CancelAsync()
        Catch
        End Try

        Try
            AMDGPUProcess.Kill()
            AMDGPUMine.CancelAsync()
        Catch
        End Try
        LogInButton4.Enabled = True
        LogInButton5.Enabled = False
        LogInButton6.Enabled = True
        LogInButton10.Enabled = True
        LogInComboBox2.Enabled = True
        LogInComboBox4.Enabled = True
        LogInComboBox5.Enabled = True
        TextBox7.Enabled = True
        If LogInComboBox5.Text = "CUSTOM" Then
            TextBox8.Enabled = True
        Else
        End If
        TextBox20.Enabled = True
        TextBox12.Text = "Stopped..."
        TextBox11.Text = "0"
        TextBox9.Text = "0"
        TextBox10.Text = "0"
    End Sub

    Private Sub GPUMine_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs) Handles NVIDIAGPUMine.DoWork
        Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim pool_address As String = TextBox8.Text
        Dim wallet As String = TextBox7.Text
        Dim pass As String = TextBox20.Text
        Dim algo As String = LogInComboBox4.Text

        Dim sOutput As String
        Dim splitOutput() As String

        Try
            Dim oStartInfo As New ProcessStartInfo(strPath + "\Config Files\Config2\ccminer.exe",
                                       "-a " + algo + " -o " + pool_address + " -u " + wallet + " -p " + pass)

            oStartInfo.UseShellExecute = False
            oStartInfo.RedirectStandardOutput = True
            oStartInfo.CreateNoWindow = True
            NvidiaGPUProcess.StartInfo = oStartInfo
            TextBox12.Text = "NVIDIA - Starting Process"
            NvidiaGPUProcess.Start()
        Catch ex As Exception
            TextBox12.Text = "ERROR - " + ex.Message
            GoTo endz
        End Try

        Try
            Using oStreamReader As System.IO.StreamReader = NvidiaGPUProcess.StandardOutput

                While True
                    sOutput = oStreamReader.ReadLine()

                    splitOutput = sOutput.Split(New Char() {",", " ", "/"})
                    If sOutput.Contains("yes!") Then
                        TextBox10.Text = (splitOutput(3))
                        TextBox11.Text = (splitOutput(4) - splitOutput(3))
                        'TextBox18.Text = Val(TextBox17.Text) + Val(TextBox16.Text)
                        TextBox9.Text = (splitOutput(8))
                    End If
                    TextBox12.Text = sOutput
                End While
            End Using
        Catch ex As Exception
            If LogInButton6.Enabled = True Then
                GoTo endz
            Else
                TextBox12.Text = "ERROR - Your GPU is Not Nvidia Or Not Supported By This Miner"
                GoTo endz
            End If
        End Try
endz:

        LogInButton4.Enabled = True
        LogInButton5.Enabled = False
        LogInButton6.Enabled = True
        LogInComboBox2.Enabled = True
        LogInComboBox4.Enabled = True
        LogInComboBox5.Enabled = True
        TextBox7.Enabled = True
        If LogInComboBox5.Text = "CUSTOM" Then
            TextBox8.Enabled = True
        Else
        End If
        TextBox20.Enabled = True
        TextBox11.Text = "0"
        TextBox9.Text = "0"
        TextBox10.Text = "0"
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            NvidiaGPUProcess.Kill()
            NVIDIAGPUMine.CancelAsync()
        Catch
        End Try
        Try
            CPUProcess.Kill()
            CPUMine.CancelAsync()
        Catch
        End Try
        My.Forms.Splashscreen.Close()
    End Sub

    'block explorer
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("https://verge-blockchain.info/")
    End Sub

    'donate
    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        My.Forms.Donate.Show()
    End Sub

    Private Sub LinkLabel4_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        My.Forms.Credits.Show()
    End Sub

    Private Sub XVGValue_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs) Handles XVGValue.DoWork
topz:
        Try
            Label4.Text = "1 VergeCoin"
        Catch ex As Exception
            MessageBox.Show("Error - " + ex.Message)
        End Try

        Try
            Dim client As New System.Net.WebClient
            Dim jsoon As String = client.DownloadString("https://api.binance.com/api/v1/ticker/price?symbol=XVGBTC")
            Dim root As JObject = JObject.Parse(jsoon)
            Dim bitcoinz As String = root("price")
            Label1.Text = (bitcoinz + " Bitcoins")

        Catch ex As Exception
            MessageBox.Show("Error Retrieving Bitcoin Value - " + ex.Message)
        End Try

        Try
            Dim client As New System.Net.WebClient
            Dim jsoon As String = client.DownloadString("https://api.coinmarketcap.com/v2/ticker/693/")
            Dim root As JObject = JObject.Parse(jsoon)
            Dim dollar As String = root("data")("quotes")("USD")("price")
            Label2.Text = (dollar + " Dollars")
        Catch ex As Exception
            MessageBox.Show("Error Retrieving Dollar Value - " + ex.Message)
        End Try
        Threading.Thread.Sleep(300000)

        GoTo topz
    End Sub

    Private Sub PictureBox9_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox9.MouseClick
        If PictureBox9.Visible = True Then
            PictureBox10.Visible = True
            PictureBox9.Visible = False
            Dim x As New System.Threading.Thread(AddressOf Stealthz)
            x.start
        End If

    End Sub


    Private Sub PictureBox10_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox10.MouseClick
        If PictureBox10.Visible = True Then
            PictureBox10.Visible = False
            PictureBox9.Visible = True
            Dim x As New System.Threading.Thread(AddressOf Lightz)
            x.Start()
        End If
    End Sub

    Private Sub Stealthz()


        'dollar
        Label2.BackColor = Color.FromArgb(7, 18, 27)
        PictureBox7.BackColor = Color.FromArgb(7, 18, 27)
        'bitcoin
        Label1.BackColor = Color.FromArgb(7, 18, 27)
        PictureBox6.BackColor = Color.FromArgb(7, 18, 27)
        'verge
        Label4.BackColor = Color.FromArgb(7, 18, 27)
        PictureBox5.BackColor = Color.FromArgb(7, 18, 27)
        'version
        Label3.BackColor = Color.FromArgb(7, 18, 27)
        'blockchain
        LinkLabel1.BackColor = Color.FromArgb(7, 18, 27)
        'credit
        LinkLabel4.BackColor = Color.FromArgb(7, 18, 27)
        'donate
        LinkLabel2.BackColor = Color.FromArgb(7, 18, 27)
        'cpu tab
        GroupBox1.BackColor = Color.FromArgb(0, 59, 84)
        GroupBox1.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel1.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel2.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel3.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel4.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel23.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel24.ForeColor = Color.FromArgb(231, 235, 238)
        GroupBox2.BackColor = Color.FromArgb(0, 59, 84)
        GroupBox2.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel5.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel9.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel10.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel11.ForeColor = Color.FromArgb(231, 235, 238)
        'gpu tab
        GroupBox3.BackColor = Color.FromArgb(0, 59, 84)
        GroupBox3.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel13.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel14.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel15.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel16.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel26.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel27.ForeColor = Color.FromArgb(231, 235, 238)
        GroupBox4.BackColor = Color.FromArgb(0, 59, 84)
        GroupBox4.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel6.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel7.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel8.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel12.ForeColor = Color.FromArgb(231, 235, 238)
        'hardware specs
        GroupBox5.BackColor = Color.FromArgb(0, 59, 84)
        GroupBox5.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel17.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel18.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel19.ForeColor = Color.FromArgb(231, 235, 238)
        GroupBox6.BackColor = Color.FromArgb(0, 59, 84)
        GroupBox6.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel20.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel21.ForeColor = Color.FromArgb(231, 235, 238)
        LogInLabel22.ForeColor = Color.FromArgb(231, 235, 238)

        PictureBox3.Visible = False
        PictureBox11.Size = New Size(610, 467)
        PictureBox11.Location = New Point(2, 89)
    End Sub

    Private Sub Lightz()

        'dollar
        Label2.BackColor = Color.FromArgb(11, 197, 239)
        PictureBox7.BackColor = Color.FromArgb(11, 197, 239)
        'bitcoin
        Label1.BackColor = Color.FromArgb(11, 197, 239)
        PictureBox6.BackColor = Color.FromArgb(11, 197, 239)
        'verge
        Label4.BackColor = Color.FromArgb(11, 197, 239)
        PictureBox5.BackColor = Color.FromArgb(11, 197, 239)
        'version
        Label3.BackColor = Color.Transparent
        'blockchain
        LinkLabel1.BackColor = Color.Transparent
        'credit
        LinkLabel4.BackColor = Color.Transparent
        'donate
        LinkLabel2.BackColor = Color.Transparent
        'cpu tab
        GroupBox1.BackColor = Color.FromArgb(231, 235, 238)
        GroupBox1.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel1.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel2.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel3.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel4.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel23.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel24.ForeColor = Color.FromArgb(0, 59, 84)
        GroupBox2.BackColor = Color.FromArgb(231, 235, 238)
        GroupBox2.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel5.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel9.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel10.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel11.ForeColor = Color.FromArgb(0, 59, 84)
        'gpu tab
        GroupBox3.BackColor = Color.FromArgb(231, 235, 238)
        GroupBox3.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel13.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel14.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel15.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel16.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel26.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel27.ForeColor = Color.FromArgb(0, 59, 84)
        GroupBox4.BackColor = Color.FromArgb(231, 235, 238)
        GroupBox4.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel6.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel7.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel8.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel12.ForeColor = Color.FromArgb(0, 59, 84)
        'hardware specs
        GroupBox5.BackColor = Color.FromArgb(231, 235, 238)
        GroupBox5.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel17.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel18.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel19.ForeColor = Color.FromArgb(0, 59, 84)
        GroupBox6.BackColor = Color.FromArgb(231, 235, 238)
        GroupBox6.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel20.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel21.ForeColor = Color.FromArgb(0, 59, 84)
        LogInLabel22.ForeColor = Color.FromArgb(0, 59, 84)

        PictureBox11.Size = New Size(128, 114)
        PictureBox11.Location = New Point(274, 313)
        PictureBox3.Visible = True
    End Sub

    'cpu algos
    Private Sub LogInComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox3.SelectedIndexChanged
        If LogInComboBox3.Text = "CUSTOM" Then
            TextBox2.Enabled = True
            TextBox2.ReadOnly = False
            TextBox2.Text = ""
        Else
            TextBox2.Enabled = False
            TextBox2.ReadOnly = True
            TextBox2.Text = LogInComboBox3.Text
        End If
    End Sub

    Private Sub LogInComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox1.SelectedIndexChanged
        If LogInComboBox1.Text = "x17" Then
            LogInComboBox3.Items.Clear()
            LogInComboBox3.Items.Add("stratum+tcp://yiimp.eu:3737")
            LogInComboBox3.Items.Add("stratum+tcp://xvg.antminepool.com:9008")
            LogInComboBox3.Items.Add("CUSTOM")
            LogInComboBox3.SelectedIndex = 0
        End If

        If LogInComboBox1.Text = "scrypt" Then
            LogInComboBox3.Items.Clear()
            LogInComboBox3.Items.Add("CUSTOM")
            LogInComboBox3.SelectedIndex = 0
        End If

        If LogInComboBox1.Text = "scrypt" Then
            LogInComboBox3.Items.Clear()
            LogInComboBox3.Items.Add("CUSTOM")
            LogInComboBox3.SelectedIndex = 0
        End If

        If LogInComboBox1.Text = "lyra2v2" Then
            LogInComboBox3.Items.Clear()
            LogInComboBox3.Items.Add("stratum+tcp://xvg-lyra.suprnova.cc:2595")
            LogInComboBox3.Items.Add("CUSTOM")
            LogInComboBox3.SelectedIndex = 0
        End If

        If LogInComboBox1.Text = "groestl" Then
            LogInComboBox3.Items.Clear()
            LogInComboBox3.Items.Add("CUSTOM")
            LogInComboBox3.SelectedIndex = 0
        End If

        If LogInComboBox1.Text = "blake2s" Then
            LogInComboBox3.Items.Clear()
            LogInComboBox3.Items.Add("CUSTOM")
            LogInComboBox3.SelectedIndex = 0
        End If
    End Sub

    'gpu algos
    Private Sub LogInComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox5.SelectedIndexChanged
        If LogInComboBox5.Text = "CUSTOM" Then
            TextBox8.Enabled = True
            TextBox8.ReadOnly = False
            TextBox8.Text = ""
        Else
            TextBox8.Enabled = False
            TextBox8.ReadOnly = True
            TextBox8.Text = LogInComboBox5.Text
        End If
    End Sub

    'gpu algo
    Private Sub LogInComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox4.SelectedIndexChanged

        'NVIDIA POOLS
        If LogInComboBox4.Text = "x17" Then
            LogInComboBox5.Items.Clear()
            LogInComboBox5.Items.Add("stratum+tcp://yiimp.eu:3737")
            LogInComboBox5.Items.Add("stratum+tcp://xvg.antminepool.com:9008")
            LogInComboBox5.Items.Add("CUSTOM")
            LogInComboBox5.SelectedIndex = 0
        End If

        If LogInComboBox4.Text = "scrypt" Then
            LogInComboBox5.Items.Clear()
            LogInComboBox5.Items.Add("CUSTOM")
            LogInComboBox5.SelectedIndex = 0
        End If

        If LogInComboBox4.Text = "lyra2v2" Then
            LogInComboBox5.Items.Clear()
            LogInComboBox5.Items.Add("stratum+tcp://xvg-lyra.suprnova.cc:2595")
            LogInComboBox5.Items.Add("CUSTOM")
            LogInComboBox5.SelectedIndex = 0
        End If

        If LogInComboBox4.Text = "groestl" Then
            LogInComboBox5.Items.Clear()
            LogInComboBox5.Items.Add("CUSTOM")
            LogInComboBox5.SelectedIndex = 0
        End If

        If LogInComboBox4.Text = "blake2s" Then
            LogInComboBox5.Items.Clear()
            LogInComboBox5.Items.Add("CUSTOM")
            LogInComboBox5.SelectedIndex = 0
        End If

        'AMD pool ONLY
        If LogInComboBox4.Text = "Lyra2RE" Then
            LogInComboBox5.Items.Clear()
            LogInComboBox5.Items.Add("stratum+tcp://lyra2re.eu.nicehash.com:3342")
            LogInComboBox5.Items.Add("CUSTOM")
            LogInComboBox5.SelectedIndex = 0
        End If

    End Sub

    Private Sub AMDGPUMine_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs) Handles AMDGPUMine.DoWork
        Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory
        Dim pool_address As String = TextBox8.Text
        Dim wallet As String = TextBox7.Text
        Dim pass As String = TextBox20.Text
        Dim algo As String = LogInComboBox4.Text

        Dim sOutput As String
        Dim splitOutput() As String

        Try
            Dim oStartInfo As New ProcessStartInfo(strPath + "\Config Files\Config3\sgminer.exe",
                                       "--no-submit-stale --kernel " + algo + " -o " + pool_address + " -u " + wallet + " -p " + pass)

            'oStartInfo.UseShellExecute = False
            'oStartInfo.RedirectStandardOutput = True
            'oStartInfo.CreateNoWindow = True
            AMDGPUProcess.StartInfo = oStartInfo
            TextBox12.Text = "AMD - Starting Process"
            AMDGPUProcess.Start()
        Catch ex As Exception
            TextBox12.Text = "ERROR - " + ex.Message
            GoTo endz
        End Try

        Try
            Using oStreamReader As System.IO.StreamReader = AMDGPUProcess.StandardOutput

                While True
                    sOutput = oStreamReader.ReadLine()

                    'splitOutput = sOutput.Split(New Char() {",", " ", "/"})
                    'If sOutput.Contains("yes!") Then
                    'TextBox10.Text = (splitOutput(3))
                    'TextBox11.Text = (splitOutput(4) - splitOutput(3))
                    'TextBox18.Text = Val(TextBox17.Text) + Val(TextBox16.Text)
                    'TextBox9.Text = (splitOutput(8))
                    'End If
                    TextBox12.Text = sOutput
                End While
            End Using
        Catch ex As Exception
            If LogInButton6.Enabled = True Then
                GoTo endz
            Else
                TextBox12.Text = "ERROR - Your GPU is Not AMD Or Not Supported By This Miner"
                GoTo endz
            End If
        End Try
endz:

        LogInButton4.Enabled = True
        LogInButton5.Enabled = False
        LogInButton6.Enabled = True
        LogInComboBox2.Enabled = True
        LogInComboBox4.Enabled = True
        LogInComboBox5.Enabled = True
        TextBox7.Enabled = True
        If LogInComboBox5.Text = "CUSTOM" Then
            TextBox8.Enabled = True
        Else
        End If
        TextBox20.Enabled = True
        TextBox11.Text = "0"
        TextBox9.Text = "0"
        TextBox10.Text = "0"
    End Sub

    'load cpu settings
    Private Sub LogInButton9_Click(sender As Object, e As EventArgs) Handles LogInButton9.Click
        Dim cpu As New System.Threading.Thread(AddressOf CPUSettings)
        cpu.Start()
    End Sub

    'load gpu settings
    Private Sub LogInButton10_Click(sender As Object, e As EventArgs) Handles LogInButton10.Click
        Dim gpu As New System.Threading.Thread(AddressOf GPUSettings)
        gpu.Start()
    End Sub

    Private Sub LogInComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox2.SelectedIndexChanged
        If LogInComboBox2.Text = "NVIDIA" Then
            LogInComboBox4.Items.Clear()
            LogInComboBox4.Items.Add("x17")
            LogInComboBox4.Items.Add("scrypt")
            LogInComboBox4.Items.Add("lyra2v2")
            LogInComboBox4.Items.Add("groestl")
            LogInComboBox4.Items.Add("blake2s")
            LogInComboBox4.SelectedIndex = 0
        ElseIf LogInComboBox2.Text = "AMD" Then
            LogInComboBox4.Items.Clear()
            LogInComboBox4.Items.Add("Lyra2RE")
            'LogInComboBox4.Items.Add("myr-groestl")
            LogInComboBox4.SelectedIndex = 0
        End If
    End Sub

    Private Sub PictureBox9_Click(sender As Object, e As EventArgs)

    End Sub
End Class
