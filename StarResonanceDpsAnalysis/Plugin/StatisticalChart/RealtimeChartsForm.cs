using AntdUI;
using StarResonanceDpsAnalysis.Plugin;
using StarResonanceDpsAnalysis.Plugin.Charts;
using StarResonanceDpsAnalysis.Plugin.DamageStatistics;
using SystemPanel = System.Windows.Forms.Panel;

namespace StarResonanceDpsAnalysis.Forms
{
    /// <summary>
    /// ÊµÊ±Í¼±úĞ°¿Ú - Ê¹ÓÃ±âÆ½»¯×Ô¶¨ÒåÍ¼±úÛØ¼ş£¬×Ô¶¯¼ÓÔØËùÓĞÍ¼±E
    /// </summary>
    public partial class RealtimeChartsForm : BorderlessForm
    {
        private Tabs _tabControl;
        private FlatLineChart _dpsTrendChart;
        private FlatPieChart _skillPieChart;
        private FlatBarChart _teamDpsChart;
        private FlatScatterChart _multiDimensionChart;
        private FlatBarChart _damageTypeChart;
        private Dropdown _playerSelector;

        // ¿ØÖÆ°´Å¥
        private AntdUI.Button _refreshButton;
        private AntdUI.Button _closeButton;
        private AntdUI.Button _autoRefreshToggle;

        // ×Ô¶¯Ë¢ĞÂÏà¹Ø
        private System.Windows.Forms.Timer _autoRefreshTimer;
        private bool _autoRefreshEnabled = false;

        // ´°ÌåÍÏ¶¯Ïà¹Ø
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private SystemPanel _draggablePanel;

        public RealtimeChartsForm()
        {
            InitializeComponent();
            FormGui.SetDefaultGUI(this);

            Text = "ÊµÊ±Í¼±úÛÉÊÓ»¯";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;

            // ÉèÖÃ±E¼×ÖÌE
            Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular);

            InitializeControls();
            InitializeAutoRefreshTimer();

            // Ó¦ÓÃµ±Ç°Ö÷ÌE
            RefreshChartsTheme();

            // ×Ô¶¯¼ÓÔØËùÓĞÍ¼±E
            LoadAllCharts();

            // Ä¬ÈÏÆôÓÃ×Ô¶¯Ë¢ĞÂ
            EnableAutoRefreshByDefault();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // RealtimeChartsForm
            // 
            ClientSize = new Size(1000, 700);
            Name = "RealtimeChartsForm";
            Load += RealtimeChartsForm_Load;
            ResumeLayout(false);
        }

        private void InitializeControls()
        {
            // ´´½¨¿ØÖÆ°´Å¥Ãæ°å£¨¿ÉÍÏ¶¯£©
            _draggablePanel = new SystemPanel
            {
                Height = 50,
                Dock = DockStyle.Top,
                Padding = new Padding(10, 5, 10, 5),
                Cursor = Cursors.SizeAll // ÏÔÊ¾¿ÉÒÆ¶¯¹â±E
            };

            // ÎªÍÏ¶¯Ãæ°åÌúØÓÊó±EÂ¼ş
            _draggablePanel.MouseDown += DraggablePanel_MouseDown;
            _draggablePanel.MouseMove += DraggablePanel_MouseMove;
            _draggablePanel.MouseUp += DraggablePanel_MouseUp;

            _refreshButton = new AntdUI.Button
            {
                Text = "ÊÖ¶¯Ë¢ĞÂ",
                Type = TTypeMini.Primary,
                Size = new Size(80, 35),
                Location = new Point(10, 8),
                Font = Font
            };
            _refreshButton.Click += RefreshButton_Click;

            _autoRefreshToggle = new AntdUI.Button
            {
                Text = "×Ô¶¯Ë¢ĞÂ: ¿ª", // Ä¬ÈÏÏÔÊ¾Îª¿ªÆô×´Ì¬
                Type = TTypeMini.Primary, // Ä¬ÈÏÊ¹ÓÃPrimaryÑùÊ½
                Size = new Size(100, 35),
                Location = new Point(100, 8),
                Font = Font
            };
            _autoRefreshToggle.Click += AutoRefreshToggle_Click;

            _closeButton = new AntdUI.Button
            {
                Text = "¹Ø±Õ",
                Type = TTypeMini.Default,
                Size = new Size(60, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(_draggablePanel.Width - 70, 8),
                Font = Font
            };
            _closeButton.Click += CloseButton_Click;

            _draggablePanel.Controls.Add(_refreshButton);
            _draggablePanel.Controls.Add(_autoRefreshToggle);
            _draggablePanel.Controls.Add(_closeButton);

            // ´´½¨Ñ¡ÏûÛ¨¿Ø¼ş
            _tabControl = new Tabs
            {
                Dock = DockStyle.Fill,
                Font = Font
            };

            // ÌúØÓTabPage - ´¿ÎÄ±¾±EE
            _tabControl.Pages.Add(new AntdUI.TabPage
            {
                Text = "DPSÇ÷ÊÆÍ¼",
                Font = Font
            });
            _tabControl.Pages.Add(new AntdUI.TabPage
            {
                Text = "¼¼ÄÜÕ¼±ÈÍ¼",
                Font = Font
            });
            _tabControl.Pages.Add(new AntdUI.TabPage
            {
                Text = "ÍÅ¶ÓDPS¶Ô±È",
                Font = Font
            });
            _tabControl.Pages.Add(new AntdUI.TabPage
            {
                Text = "¶àÎ¬¶È¶Ô±È",
                Font = Font
            });
            _tabControl.Pages.Add(new AntdUI.TabPage
            {
                Text = "ÉËº¦·Ö²¼Í¼",
                Font = Font
            });

            // ×¼±¸¸÷Ò³ÃæÈİÆE
            for (int i = 0; i < 5; i++)
            {
                var panel = new SystemPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = AppConfig.IsLight ? Color.White : Color.FromArgb(31, 31, 31)
                };
                _tabControl.Pages[i].Controls.Add(panel);
            }

            // Îª¼¼ÄÜÕ¼±ÈÍ¼Ò³ÃæÌúØÓÍæ¼ÒÑ¡ÔñÆE
            var skillChartPage = _tabControl.Pages[1];
            var skillChartPanel = skillChartPage.Controls[0] as SystemPanel;

            var playerSelectorPanel = new SystemPanel
            {
                Height = 50,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };

            var playerLabel = new AntdUI.Label
            {
                Text = "Ñ¡ÔñÍæ¼Ò£º",
                Location = new Point(10, 15),
                AutoSize = true,
                Font = Font
            };

            _playerSelector = new Dropdown
            {
                Location = new Point(90, 10),
                Size = new Size(200, 30),
                Font = Font
            };
            _playerSelector.SelectedValueChanged += PlayerSelector_SelectedValueChanged;

            playerSelectorPanel.Controls.Add(playerLabel);
            playerSelectorPanel.Controls.Add(_playerSelector);
            skillChartPanel.Controls.Add(playerSelectorPanel);

            Controls.Add(_tabControl);
            Controls.Add(_draggablePanel);
        }

        #region ´°ÌåÍÏ¶¯ÊÂ¼ş´¦ÀE

        private void DraggablePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _dragStartPoint = e.Location;
                _draggablePanel.Cursor = Cursors.Hand;
            }
        }

        private void DraggablePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.Button == MouseButtons.Left)
            {
                // ¼ÆËãÒÆ¶¯¾àÀE
                var deltaX = e.Location.X - _dragStartPoint.X;
                var deltaY = e.Location.Y - _dragStartPoint.Y;

                // ÒÆ¶¯´°ÌE
                this.Location = new Point(this.Location.X + deltaX, this.Location.Y + deltaY);
            }
        }

        private void DraggablePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
                _draggablePanel.Cursor = Cursors.SizeAll;
            }
        }

        #endregion

        /// <summary>
        /// Ä¬ÈÏÆôÓÃ×Ô¶¯Ë¢ĞÂ
        /// </summary>
        private void EnableAutoRefreshByDefault()
        {
            _autoRefreshEnabled = true;
            _autoRefreshTimer.Enabled = true;
            _autoRefreshToggle.Text = "×Ô¶¯Ë¢ĞÂ: ¿ª";
            _autoRefreshToggle.Type = TTypeMini.Primary;
        }

        /// <summary>
        /// ×Ô¶¯¼ÓÔØËùÓĞÍ¼±E
        /// </summary>
        private void LoadAllCharts()
        {
            try
            {
                // ¼ÓÔØDPSÇ÷ÊÆÍ¼£¨ÒÆ³ı»¬¿é¿ØÖÆ£©
                var dpsTrendPanel = _tabControl.Pages[0].Controls[0] as SystemPanel;
                _dpsTrendChart = ChartVisualizationService.CreateDpsTrendChart();
                dpsTrendPanel.Controls.Add(_dpsTrendChart);

                // ¼ÓÔØ¼¼ÄÜÕ¼±ÈÍ¼
                var skillChartPanel = _tabControl.Pages[1].Controls[0] as SystemPanel;
                UpdatePlayerSelector();
                var selectedPlayer = _playerSelector.SelectedValue as PlayerSelectorItem;
                ulong playerId = selectedPlayer?.Uid ?? 0;
                _skillPieChart = ChartVisualizationService.CreateSkillDamagePieChart(playerId);
                skillChartPanel.Controls.Add(_skillPieChart);

                // ¼ÓÔØÍÅ¶ÓDPS¶Ô±ÈÍ¼
                var teamDpsPanel = _tabControl.Pages[2].Controls[0] as SystemPanel;
                _teamDpsChart = ChartVisualizationService.CreateTeamDpsBarChart();
                teamDpsPanel.Controls.Add(_teamDpsChart);

                // ¼ÓÔØ¶àÎ¬¶È¶Ô±ÈÍ¼
                var multiDimensionPanel = _tabControl.Pages[3].Controls[0] as SystemPanel;
                _multiDimensionChart = ChartVisualizationService.CreateDpsRadarChart();
                multiDimensionPanel.Controls.Add(_multiDimensionChart);

                // ¼ÓÔØÉËº¦·Ö²¼Í¼
                var damageTypePanel = _tabControl.Pages[4].Controls[0] as SystemPanel;
                _damageTypeChart = ChartVisualizationService.CreateDamageTypeStackedChart();
                damageTypePanel.Controls.Add(_damageTypeChart);

                // ³õÊ¼Ë¢ĞÂËùÓĞÍ¼±úæı¾İ
                RefreshAllCharts();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"¼ÓÔØÍ¼±úæ±³ö´E {ex.Message}");
                MessageBox.Show($"¼ÓÔØÍ¼±úæ±³ö´E {ex.Message}", "´úê", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeAutoRefreshTimer()
        {
            _autoRefreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 100, // 0.1ÃE(100ºÁÃE ¸ßÆµË¢ĞÂ
                Enabled = false
            };
            _autoRefreshTimer.Tick += AutoRefreshTimer_Tick;
        }

        #region ÊÂ¼ş´¦ÀE

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshAllCharts();

            // ÏÔÊ¾Ë¢ĞÂ×´Ì¬
            _refreshButton.Text = "Ë¢ĞÂÖĞ...";
            _refreshButton.Enabled = false;

            var resetTimer = new System.Windows.Forms.Timer { Interval = 300 };
            resetTimer.Tick += (s, args) =>
            {
                _refreshButton.Text = "ÊÖ¶¯Ë¢ĞÂ";
                _refreshButton.Enabled = true;
                resetTimer.Stop();
                resetTimer.Dispose();
            };
            resetTimer.Start();
        }

        private void AutoRefreshToggle_Click(object sender, EventArgs e)
        {
            _autoRefreshEnabled = !_autoRefreshEnabled;
            _autoRefreshTimer.Enabled = _autoRefreshEnabled;

            _autoRefreshToggle.Text = $"×Ô¶¯Ë¢ĞÂ: {(_autoRefreshEnabled ? "¿ª" : "¹Ø")}";
            _autoRefreshToggle.Type = _autoRefreshEnabled ? TTypeMini.Primary : TTypeMini.Default;
        }

        private void AutoRefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshAllCharts();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PlayerSelector_SelectedValueChanged(object sender, ObjectNEventArgs e)
        {
            if (_playerSelector.SelectedValue is PlayerSelectorItem item && _skillPieChart != null)
            {
                ChartVisualizationService.RefreshSkillDamagePieChart(_skillPieChart, item.Uid);
            }
        }

        #endregion

        private void RefreshAllCharts()
        {
            try
            {
                // ¸EÂÊı¾İµE
                ChartVisualizationService.UpdateAllDataPoints();

                // Ë¢ĞÂËùÓĞÍ¼±ú¿¬±ÜÃâÓÃ»§¼ÇÂ¼¶ªÊ§
                if (_dpsTrendChart != null)
                {
                    ChartVisualizationService.RefreshDpsTrendChart(_dpsTrendChart, null, ChartDataType.Damage);
                    _dpsTrendChart.ReloadPersistentData(); // ÖØĞÂ¼ÓÔØÊı¾İ·ÀÖ¹¶ªÊ§
                }

                if (_skillPieChart != null)
                {
                    var selectedPlayer = _playerSelector.SelectedValue as PlayerSelectorItem;
                    ChartVisualizationService.RefreshSkillDamagePieChart(_skillPieChart, selectedPlayer?.Uid ?? 0);
                }

                if (_teamDpsChart != null)
                    ChartVisualizationService.RefreshTeamDpsBarChart(_teamDpsChart);

                if (_multiDimensionChart != null)
                    ChartVisualizationService.RefreshDpsRadarChart(_multiDimensionChart);

                if (_damageTypeChart != null)
                    ChartVisualizationService.RefreshDamageTypeStackedChart(_damageTypeChart);

                // ¸EÂÍæ¼ÒÑ¡ÔñÆE
                UpdatePlayerSelector();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ë¢ĞÂÍ¼±úæ±³ö´E {ex.Message}");
            }
        }

        private void UpdatePlayerSelector()
        {
            var players = StatisticData._manager.GetPlayersWithCombatData().ToList();

            // ±£´æµ±Ç°Ñ¡ÔE
            var currentSelection = _playerSelector.SelectedValue as PlayerSelectorItem;

            _playerSelector.Items.Clear();

            foreach (var player in players)
            {
                var displayName = string.IsNullOrEmpty(player.Nickname) ? $"Íæ¼Ò{player.Uid}" : player.Nickname;
                var item = new PlayerSelectorItem { Uid = player.Uid, DisplayName = displayName };
                _playerSelector.Items.Add(item);

                // »Ö¸´Ñ¡Ôñ»òÄ¬ÈÏÑ¡ÔñµÚÒ»¸E
                if ((currentSelection != null && currentSelection.Uid == player.Uid) ||
                    (currentSelection == null && _playerSelector.Items.Count == 1))
                {
                    _playerSelector.SelectedValue = item;
                }
            }
        }

        /// <summary>
        /// Ë¢ĞÂÍ¼±úò÷ÌE
        /// </summary>
        public void RefreshChartsTheme()
        {
            var isDark = !AppConfig.IsLight;

            // ÉèÖÃ´°¿ÚÖ÷ÌE
            FormGui.SetColorMode(this, AppConfig.IsLight);

            // ¸EÂËùÓĞÍ¼±úò÷ÌE
            if (_dpsTrendChart != null)
                _dpsTrendChart.IsDarkTheme = isDark;

            if (_skillPieChart != null)
                _skillPieChart.IsDarkTheme = isDark;

            if (_teamDpsChart != null)
                _teamDpsChart.IsDarkTheme = isDark;

            if (_multiDimensionChart != null)
                _multiDimensionChart.IsDarkTheme = isDark;

            if (_damageTypeChart != null)
                _damageTypeChart.IsDarkTheme = isDark;
        }

        /// <summary>
        /// Çå¿ÕËùÓĞÍ¼±úæı¾İ
        /// </summary>
        public void ClearAllChartData()
        {
            _dpsTrendChart?.ClearSeries();
            _skillPieChart?.ClearData();
            _teamDpsChart?.ClearData();
            _multiDimensionChart?.ClearSeries();
            _damageTypeChart?.ClearData();
            _playerSelector?.Items.Clear();
        }

        /// <summary>
        /// ÊÖ¶¯Ë¢ĞÂËùÓĞÍ¼±E
        /// </summary>
        public void ManualRefreshCharts()
        {
            RefreshAllCharts();
        }

        /// <summary>
        /// ÉèÖÃ×Ô¶¯Ë¢ĞÂ¼ä¸E
        /// </summary>
        public void SetAutoRefreshInterval(int milliseconds)
        {
            if (_autoRefreshTimer != null)
            {
                _autoRefreshTimer.Interval = Math.Max(50, milliseconds); // ×ûì¡50ºÁÃE¬Ö§³Ö¸EßÆµÂÊ
            }
        }

        /// <summary>
        /// »ñÈ¡µ±Ç°×Ô¶¯Ë¢ĞÂ×´Ì¬
        /// </summary>
        public bool IsAutoRefreshEnabled => _autoRefreshEnabled;

        /// <summary>
        /// »ñÈ¡µ±Ç°Ë¢ĞÂ¼ä¸E
        /// </summary>
        public int GetRefreshInterval => _autoRefreshTimer?.Interval ?? 100;

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _autoRefreshTimer?.Stop();
            _autoRefreshTimer?.Dispose();
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // ´°¿Ú¼ÓÔØºó×Ô¶¯Ë¢ĞÂÒ»´ÎÍ¼±E
            if (_dpsTrendChart != null)
            {
                RefreshAllCharts();
            }
        }

        private void RealtimeChartsForm_Load(object sender, EventArgs e)
        {

        }
    }

    /// <summary>
    /// Íæ¼ÒÑ¡ÔñÆ÷ÏE
    /// </summary>
    public class PlayerSelectorItem
    {
        public ulong Uid { get; set; }
        public string DisplayName { get; set; } = "";

        public override string ToString()
        {
            return DisplayName;
        }
    }
}