'由于包含加解密等安全信息，本文件中的部分代码已被删除

Friend Module ModSecret

    '标注 PCL 的不同分支，仅用于替换标记
    Public Const VersionBranchMain As String = "OpenSource"
    '在开源版的注册表与常规版的注册表隔离，以防数据冲突
    Public Const RegFolder As String = "PCLDebug"
    '用于微软登录的 ClientId
    Public OAuthClientId As String = If(Environment.GetEnvironmentVariable("PCL_MS_CLIENT_ID"), "c36a9fb6-4f2a-41ff-90bd-ae7cc92031eb") '开源社区公开注册的启动器 Client ID（来自 PrismLauncher 仓库），建议自行注册后替换
    'CurseForge API Key
    Public CurseForgeAPIKey As String = If(Environment.GetEnvironmentVariable("PCL_CURSEFORGE_API_KEY"), "")
    '用于匿名数据收集的腾讯云日志服务上报 URL，形如 https://{region}.cls.tencentcs.com/track?topic_id={topic_id}
    Public Const ClsBaseUrl As String = ""

#Region "网络鉴权"

    Public Const RestrictMcDownload As Boolean = False
    Friend Function SecretCdnSign(UrlWithMark As String) As String
        If Not UrlWithMark.EndsWithF("{CDN}") Then Return UrlWithMark
        Return UrlWithMark.Replace("{CDN}", "").Replace(" ", "%20")
    End Function
    ''' <summary>
    ''' 设置 Headers 的 UA、Referer。
    ''' </summary>
    Friend Sub SecretHeadersSign(Url As String, ByRef Req As HttpRequestMessage, Optional SimulateBrowserHeaders As Boolean = False)
        If Not Req.Headers.UserAgent.Any Then
            If Url.Contains("baidupcs.com") OrElse Url.Contains("baidu.com") Then
                Req.Headers.Add("User-Agent", "LogStatistic")  '#4951
            ElseIf SimulateBrowserHeaders Then
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)} Mozilla/5.0 AppleWebKit/537.36 Chrome/63.0.3239.132 Safari/537.36")
            Else
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)}")
            End If
        End If
        If Not SimulateBrowserHeaders Then Req.Headers.Add("Referer", $"http://{VersionCode}.open.pcl2.server/")
        If Url.Contains("api.curseforge.com") OrElse Url.Contains("forgecdn.net") Then Req.Headers.Add("x-api-key", CurseForgeAPIKey)
    End Sub

#End Region

#Region "主题"

    Public Color1 As New MyColor(52, 61, 74)
    Public Color2 As New MyColor(11, 91, 203)
    Public Color3 As New MyColor(19, 112, 243)
    Public Color4 As New MyColor(72, 144, 245)
    Public Color5 As New MyColor(150, 192, 249)
    Public Color6 As New MyColor(213, 230, 253)
    Public Color7 As New MyColor(222, 236, 253)
    Public Color8 As New MyColor(234, 242, 254)
    Public ColorBg0 As New MyColor(150, 192, 249)
    Public ColorBg1 As New MyColor(190, Color7)
    Public ColorGray1 As New MyColor(64, 64, 64)
    Public ColorGray2 As New MyColor(115, 115, 115)
    Public ColorGray3 As New MyColor(140, 140, 140)
    Public ColorGray4 As New MyColor(166, 166, 166)
    Public ColorGray5 As New MyColor(204, 204, 204)
    Public ColorGray6 As New MyColor(235, 235, 235)
    Public ColorGray7 As New MyColor(240, 240, 240)
    Public ColorGray8 As New MyColor(245, 245, 245)
    Public ColorSemiTransparent As New MyColor(1, Color8)

    Public ThemeNow As Integer = -1
    Public ColorHue As Integer = 210, ColorSat As Integer = 85, ColorLightAdjust As Integer = 0, ColorHueTopbarDelta As OneOf(Of Integer, Integer()) = 0
    Public ThemeDontClick As Integer = 0

    Public Sub ThemeRefresh(Optional NewTheme As Integer = -1)
        Try
            If ThemeNow = NewTheme AndAlso NewTheme >= 0 Then Return
            If NewTheme >= 0 Then ThemeNow = NewTheme

            '主题 ID → 颜色参数映射（开源版补全；无参调用时按当前主题重新应用，12/13 号彩系主题借此逐帧推进色相实现动画）
            Select Case ThemeNow
                Case 0 '龙猫蓝
                    ColorHue = 210 : ColorSat = 85 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 1 '甜柠青
                    ColorHue = 165 : ColorSat = 80 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 2 '小草绿
                    ColorHue = 120 : ColorSat = 65 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 3 '菠萝黄
                    ColorHue = 48 : ColorSat = 90 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 4 '橡木棕
                    ColorHue = 28 : ColorSat = 55 : ColorLightAdjust = -8 : ColorHueTopbarDelta = 0
                Case 5 '玄素黑
                    ColorHue = 0 : ColorSat = 0 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 6 '铁杆粉
                    ColorHue = 340 : ColorSat = 75 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 7 '神秘紫
                    ColorHue = 280 : ColorSat = 70 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 8 '秋仪金
                    ColorHue = 42 : ColorSat = 85 : ColorLightAdjust = 5 : ColorHueTopbarDelta = 0
                Case 9 '活跃橙
                    ColorHue = 25 : ColorSat = 90 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 10 '跳票红
                    ColorHue = 0 : ColorSat = 80 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 11 '极客蓝
                    ColorHue = 240 : ColorSat = 75 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
                Case 12 '滑稽彩：色相随时间轮转（由主时钟每 250ms 无参调用 ThemeRefresh 推进）
                    ColorHue = (ColorHue + 4) Mod 360 : ColorSat = 85 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 30
                Case 13 '欧皇彩：色相轮转更快，顶条三段异色
                    ColorHue = (ColorHue + 7) Mod 360 : ColorSat = 90 : ColorLightAdjust = 0 : ColorHueTopbarDelta = New Integer(){-120, 0, 120}
                Case 14 '自定义：读取个性化滑条设置
                    ColorHue = Settings.Get(Of Integer)("UiLauncherHue")
                    ColorSat = Settings.Get(Of Integer)("UiLauncherSat")
                    ColorHueTopbarDelta = Settings.Get(Of Integer)("UiLauncherDelta")
                    ColorLightAdjust = Settings.Get(Of Integer)("UiLauncherLight") - 20
                Case Else
                    ColorHue = 210 : ColorSat = 85 : ColorLightAdjust = 0 : ColorHueTopbarDelta = 0
            End Select

            Color1 = New MyColor().FromHSL2(ColorHue, ColorSat * 0.2, 25 + ColorLightAdjust * 0.3)
            Color2 = New MyColor().FromHSL2(ColorHue, ColorSat, 45 + ColorLightAdjust)
            Color3 = New MyColor().FromHSL2(ColorHue, ColorSat, 55 + ColorLightAdjust)
            Color4 = New MyColor().FromHSL2(ColorHue, ColorSat, 65 + ColorLightAdjust)
            Color5 = New MyColor().FromHSL2(ColorHue, ColorSat, 80 + ColorLightAdjust * 0.4)
            Color6 = New MyColor().FromHSL2(ColorHue, ColorSat, 91 + ColorLightAdjust * 0.1)
            Color7 = New MyColor().FromHSL2(ColorHue, ColorSat, 95)
            Color8 = New MyColor().FromHSL2(ColorHue, ColorSat, 97)
            ColorBg0 = Color4 * 0.4 + Color5 * 0.4 + ColorGray4 * 0.2
            ColorBg1 = New MyColor(190, Color7)

            ColorSemiTransparent = New MyColor(1, Color8)
            Application.Current.Resources("ColorBrush1") = New SolidColorBrush(Color1)
            Application.Current.Resources("ColorBrush2") = New SolidColorBrush(Color2)
            Application.Current.Resources("ColorBrush3") = New SolidColorBrush(Color3)
            Application.Current.Resources("ColorBrush4") = New SolidColorBrush(Color4)
            Application.Current.Resources("ColorBrush5") = New SolidColorBrush(Color5)
            Application.Current.Resources("ColorBrush6") = New SolidColorBrush(Color6)
            Application.Current.Resources("ColorBrush7") = New SolidColorBrush(Color7)
            Application.Current.Resources("ColorBrush8") = New SolidColorBrush(Color8)
            Application.Current.Resources("ColorBrushBg0") = New SolidColorBrush(ColorBg0)
            Application.Current.Resources("ColorBrushBg1") = New SolidColorBrush(ColorBg1)
            Application.Current.Resources("ColorObject1") = CType(Color1, Color)
            Application.Current.Resources("ColorObject2") = CType(Color2, Color)
            Application.Current.Resources("ColorObject3") = CType(Color3, Color)
            Application.Current.Resources("ColorObject4") = CType(Color4, Color)
            Application.Current.Resources("ColorObject5") = CType(Color5, Color)
            Application.Current.Resources("ColorObject6") = CType(Color6, Color)
            Application.Current.Resources("ColorObject7") = CType(Color7, Color)
            Application.Current.Resources("ColorObject8") = CType(Color8, Color)
            Application.Current.Resources("ColorObjectBg0") = CType(ColorBg0, Color)
            Application.Current.Resources("ColorObjectBg1") = CType(ColorBg1, Color)
            ThemeRefreshMain()
        Catch ex As Exception
            Logger.Error(ex, "刷新主题颜色失败", LogBehavior.Toast)
        End Try
    End Sub
    Public Sub ThemeRefreshMain()
        RunInUi(
        Sub()
            If Not FrmMain.IsLoaded Then Return
            '顶部条背景
            Dim Brush = New LinearGradientBrush With {.EndPoint = New Point(1, 0), .StartPoint = New Point(0, 0)}
            Dim Deltas = ColorHueTopbarDelta.Switch(Function(d) New Integer() {-d, 0, d}, Function(d) d)
            Brush.GradientStops.Add(New GradientStop With {.Offset = 0, .Color = New MyColor().FromHSL2(ColorHue + Deltas(0), ColorSat, 48 + ColorLightAdjust)})
            Brush.GradientStops.Add(New GradientStop With {.Offset = 0.5, .Color = New MyColor().FromHSL2(ColorHue + Deltas(1), ColorSat, 54 + ColorLightAdjust)})
            Brush.GradientStops.Add(New GradientStop With {.Offset = 1, .Color = New MyColor().FromHSL2(ColorHue + Deltas(2), ColorSat, 48 + ColorLightAdjust)})
            FrmMain.PanTitle.Background = Brush
            FrmMain.PanTitle.Background.Freeze()
            '主页面背景
            If Settings.Get(Of Boolean)("UiBackgroundColorful") Then
                Brush = New LinearGradientBrush With {.EndPoint = New Point(0.1, 1), .StartPoint = New Point(0.9, 0)}
                Brush.GradientStops.Add(New GradientStop With {.Offset = -0.1, .Color = New MyColor().FromHSL2(ColorHue - 15, ColorSat * 0.8, 91)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.4, .Color = New MyColor().FromHSL2(ColorHue, ColorSat * 0.8, 91)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 1.1, .Color = New MyColor().FromHSL2(ColorHue + 15, ColorSat * 0.8, 91)})
                FrmMain.PanForm.Background = Brush
            Else
                FrmMain.PanForm.Background = New MyColor(245, 245, 245)
            End If
            FrmMain.PanForm.Background.Freeze()
        End Sub)
    End Sub
    Friend Sub ThemeCheckAll(EffectSetup As Boolean)
        '开源版本地解锁全部隐藏主题
        RunInUi(
        Sub()
            If IsNothing(FrmSetupUI) Then Return
            With FrmSetupUI
                .RadioLauncherTheme5.Opacity = 1
                .RadioLauncherTheme5Gray.Opacity = 0
                .LabLauncherTheme5Unlock.Visibility = Visibility.Collapsed
                .LabLauncherTheme8Copy.Visibility = Visibility.Collapsed
                .LabLauncherTheme9Copy.Visibility = Visibility.Collapsed
                .LabLauncherTheme11Click.Visibility = Visibility.Collapsed
                .PanLauncherHide.Visibility = Visibility.Collapsed
                .RadioLauncherTheme6.IsEnabled = True
                .RadioLauncherTheme7.IsEnabled = True
                .RadioLauncherTheme8.IsEnabled = True
                .RadioLauncherTheme9.IsEnabled = True
                .RadioLauncherTheme10.IsEnabled = True
                .RadioLauncherTheme11.IsEnabled = True
                .RadioLauncherTheme12.IsEnabled = True
                .RadioLauncherTheme13.IsEnabled = True
                .RadioLauncherTheme14.IsEnabled = True
            End With
        End Sub)
    End Sub
    Friend Function ThemeCheckOne(Id As Integer) As Boolean
        Return True
    End Function
    Friend Function ThemeUnlock(Id As Integer, Optional ShowDoubleHint As Boolean = True, Optional UnlockHint As String = Nothing) As Boolean
        Return False '所有主题已由 ThemeCheckAll 解锁，无需再次弹窗提示
    End Function

#End Region

#Region "更新"

    Friend Sub UpdateCheckByButton()
        RunInNewThread(
        Sub()
            Try
                Dim Release = GetLatestRelease()
                If Release Is Nothing Then
                    Hint("无法连接到更新服务器，请检查网络后重试……", HintType.Red)
                    Return
                End If
                Dim NewName As String = Release("tag_name").ToString
                If CompareVersionName(NewName, VersionBaseName) > 0 Then
                    If MyMsgBox($"发现新版本 {NewName}（当前版本 {VersionBaseName}）：" & vbCrLf & vbCrLf &
                                Release("body").ToString, "发现新版本", "打开下载页面", "以后再说") = 1 Then
                        OpenWebsite(Release("html_url").ToString)
                    End If
                Else
                    Hint($"当前已经是最新版本（{VersionBaseName}）", HintType.Green)
                End If
            Catch ex As Exception
                ex.ThrowIfCanceled()
                Logger.Error(ex, "检查更新失败", LogBehavior.Toast)
            End Try
        End Sub, "检查更新")
    End Sub
    ''' <summary>
    ''' 从 GitHub Releases 获取最新发布信息，失败时依次尝试镜像源。获取失败返回 Nothing。
    ''' </summary>
    Private Function GetLatestRelease() As JObject
        For Each Url In {"https://api.github.com/repos/Meloong-Git/PCL/releases/latest",
                         "https://gh-proxy.com/https://api.github.com/repos/Meloong-Git/PCL/releases/latest"}
            Try
                Return NetRequestByClientRetry(Url, RequireJson:=True).DeserializeJson()
            Catch ex As Exception
                Logger.Warn(ex, "更新源获取失败：" & Url)
            End Try
        Next
        Return Nothing
    End Function
    ''' <summary>
    ''' 比较两个显示用版本名（如 2.13.1.1），A 比 B 新时返回正数。
    ''' </summary>
    Private Function CompareVersionName(A As String, B As String) As Integer
        Dim PA = A.TrimStart("v"c).Split("."c), PB = B.TrimStart("v"c).Split("."c)
        For i As Integer = 0 To Math.Max(PA.Length, PB.Length) - 1
            Dim VA As Integer = 0, VB As Integer = 0
            If i < PA.Length Then Integer.TryParse(PA(i), VA)
            If i < PB.Length Then Integer.TryParse(PB(i), VB)
            If VA <> VB Then Return VA - VB
        Next
        Return 0
    End Function

    Friend IsUpdateWaitingRestart As Boolean = False
    Public Sub UpdateRestart(TriggerRestartAndByEnd As Boolean)
    End Sub
    Public Sub UpdateReplace(ProcessId As Integer, OldFileName As String, NewFileName As String, TriggerRestart As Boolean)
    End Sub

    ''' <summary>
    ''' 确保 PathTemp/Latest.exe 是最新正式版的 PCL，它会被用于整合包打包。
    ''' 如果不是，则下载一个。
    ''' </summary>
    Friend Sub DownloadLatestPCL(Optional LoaderToSyncProgress As LoaderBase = Nothing)
        '注意：如果要自行实现这个功能，请换用另一个文件路径，以免与官方版本冲突
    End Sub

#End Region

#Region "联网配置"

    ''' <summary>
    ''' 联网获取的配置信息。
    ''' 若获取失败或仍在获取中，可能为 Nothing。
    ''' </summary>
    Public ServerConfig As JObject

    Public ServerLoader As New LoaderTask(Of Integer, Integer)("PCL 配置更新",
        Sub()
            Try
                Dim Release = GetLatestRelease()
                If Release Is Nothing Then Return
                Dim NewName As String = Release("tag_name").ToString
                If CompareVersionName(NewName, VersionBaseName) > 0 AndAlso Settings.Get(Of String)("CacheUpdateNoticeTag") <> NewName Then
                    Settings.Set("CacheUpdateNoticeTag", NewName)
                    If MyMsgBox($"发现新版本 {NewName}（当前版本 {VersionBaseName}）：" & vbCrLf & vbCrLf &
                                Release("body").ToString, "更新通知", "打开下载页面", "以后再说") = 1 Then
                        OpenWebsite(Release("html_url").ToString)
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "检查更新通知失败")
            End Try
        End Sub, Priority:=ThreadPriority.BelowNormal) With
        {.ReloadTimeout = 1000 * 60 * 60} '超时 1 小时

#End Region

#Region "赞助等级"

    Public ReadOnly Property CurrentRank As DonationRank
        Get
            Return DonationRank.None
        End Get
    End Property

    Public Sub InputPotatoCode(IsUpdating As Boolean)
    End Sub
    Friend Sub GeneratePotatoCode()
    End Sub

    ''' <summary>
    ''' 获取设备识别码。
    ''' </summary>
    Friend Function GetIdentify() As String
        Return "0000-0000-0000-0000"
    End Function

#End Region

End Module
