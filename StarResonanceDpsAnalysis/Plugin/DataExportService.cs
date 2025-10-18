using ClosedXML.Excel;
using StarResonanceDpsAnalysis.Plugin.DamageStatistics;
using System.Text;

namespace StarResonanceDpsAnalysis.Plugin
{
    /// <summary>
    /// Êı¾İµ¼³ö·şÎñ£¬Ö§³ÖExcelºÍCSV¸ñÊ½
    /// </summary>
    public static class DataExportService
    {
        #region Excelµ¼³E

        /// <summary>
        /// µ¼³öDPSÊı¾İµ½ExcelÎÄ¼ş
        /// </summary>
        /// <param name="players">Íæ¼ÒÊı¾İÁĞ±E/param>
        /// <param name="includeSkillDetails">ÊÇ·ñ°E¬¼¼ÄÜÏEE/param>
        /// <returns>ÊÇ·ñµ¼³ö³É¹¦</returns>
        public static bool ExportToExcel(List<PlayerData> players, bool includeSkillDetails = true)
        {
            try
            {
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "ExcelÎÄ¼ş (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx",
                    FileName = $"DPSÍ³¼Æ_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx",
                    Title = "±£´æDPSÍ³¼ÆÊı¾İ"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return false;

                using var workbook = new XLWorkbook();

                // ´´½¨Íæ¼Ò×ÜÀÀ±E
                CreatePlayerOverviewSheet(workbook, players);

                if (includeSkillDetails)
                {
                    // ´´½¨¼¼ÄÜÏEé±E
                    CreateSkillDetailsSheet(workbook, players);

                    // ´´½¨ÍÅ¶Ó¼¼ÄÜÍ³¼Æ±E
                    CreateTeamSkillStatsSheet(workbook, players);
                }

                workbook.SaveAs(saveDialog.FileName);

                MessageBox.Show($"Êı¾İÒÑ³É¹¦µ¼³öµ½:\n{saveDialog.FileName}", "µ¼³ö³É¹¦",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"µ¼³öExcelÎÄ¼şÊ±·¢Éú´úêE\n{ex.Message}", "µ¼³öÊ§°Ü",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// ´´½¨Íæ¼Ò×ÜÀÀ¹¤×÷±E
        /// </summary>
        private static void CreatePlayerOverviewSheet(XLWorkbook workbook, List<PlayerData> players)
        {
            var worksheet = workbook.Worksheets.Add("Íæ¼Ò×ÜÀÀ");

            // ÉèÖÃ±úé·
            var headers = new[]
            {
                "Íæ¼ÒE³Æ", "Ö°Òµ", "Õ½Á¦", "×ÜÉËº¦", "×ÜDPS", "±©»÷ÉËº¦", "ĞÒÔËÉËº¦",
                "±©»÷ÂÊ", "ĞÒÔËÂÊ", "Ë²Ê±DPS·åÖµ", "×ÜÖÎÁÆ", "×ÜHPS", "³ĞÊÜÉËº¦", "ÃEĞ´ÎÊı"
            };

            // Ğ´ÈEúé·
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            // Ğ´ÈEı¾İ
            int row = 2;
            foreach (var player in players.OrderByDescending(p => p.DamageStats.Total))
            {
                worksheet.Cell(row, 1).Value = player.Nickname;
                worksheet.Cell(row, 2).Value = player.Profession;
                worksheet.Cell(row, 3).Value = player.CombatPower;
                worksheet.Cell(row, 4).Value = (double)player.DamageStats.Total;
                worksheet.Cell(row, 5).Value = Math.Round(player.GetTotalDps(), 1);
                worksheet.Cell(row, 6).Value = (double)player.DamageStats.Critical;
                worksheet.Cell(row, 7).Value = (double)player.DamageStats.Lucky;
                worksheet.Cell(row, 8).Value = $"{player.DamageStats.GetCritRate()}%";
                worksheet.Cell(row, 9).Value = $"{player.DamageStats.GetLuckyRate()}%";
                worksheet.Cell(row, 10).Value = (double)player.DamageStats.RealtimeMax;
                worksheet.Cell(row, 11).Value = (double)player.HealingStats.Total;
                worksheet.Cell(row, 12).Value = Math.Round(player.GetTotalHps(), 1);
                worksheet.Cell(row, 13).Value = (double)player.TakenDamage;
                worksheet.Cell(row, 14).Value = player.DamageStats.CountTotal;

                row++;
            }

            // ×Ô¶¯µ÷ÕûÁĞ¿E
            worksheet.ColumnsUsed().AdjustToContents();

            // ÌúØÓÉ¸Ñ¡
            worksheet.Range(1, 1, row - 1, headers.Length).SetAutoFilter();
        }

        /// <summary>
        /// ´´½¨¼¼ÄÜÏEé¹¤×÷±E
        /// </summary>
        private static void CreateSkillDetailsSheet(XLWorkbook workbook, List<PlayerData> players)
        {
            var worksheet = workbook.Worksheets.Add("¼¼ÄÜÏEE");

            // ÉèÖÃ±úé·
            var headers = new[]
            {
                "Íæ¼ÒE³Æ", "¼¼ÄÜÃû³Æ", "×ÜÉËº¦", "ÃEĞ´ÎÊı", "Æ½¾ùÉËº¦",
                "±©»÷ÂÊ", "ĞÒÔËÂÊ", "¼¼ÄÜDPS", "ÉËº¦Õ¼±È"
            };

            // Ğ´ÈEúé·
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
            }

            // Ğ´ÈEı¾İ
            int row = 2;
            foreach (var player in players.OrderByDescending(p => p.DamageStats.Total))
            {
                var skills = StatisticData._manager.GetPlayerSkillSummaries(
                    player.Uid, topN: null, orderByTotalDesc: true);

                foreach (var skill in skills)
                {
                    worksheet.Cell(row, 1).Value = player.Nickname;
                    worksheet.Cell(row, 2).Value = skill.SkillName;
                    worksheet.Cell(row, 3).Value = (double)skill.Total;
                    worksheet.Cell(row, 4).Value = skill.HitCount;
                    worksheet.Cell(row, 5).Value = Math.Round(skill.AvgPerHit, 1);
                    worksheet.Cell(row, 6).Value = $"{skill.CritRate * 100:F1}%";
                    worksheet.Cell(row, 7).Value = $"{skill.LuckyRate * 100:F1}%";
                    worksheet.Cell(row, 8).Value = Math.Round(skill.TotalDps, 1);
                    worksheet.Cell(row, 9).Value = $"{skill.ShareOfTotal * 100:F1}%";

                    row++;
                }
            }

            // ×Ô¶¯µ÷ÕûÁĞ¿E
            worksheet.ColumnsUsed().AdjustToContents();

            // ÌúØÓÉ¸Ñ¡
            if (row > 2)
                worksheet.Range(1, 1, row - 1, headers.Length).SetAutoFilter();
        }

        /// <summary>
        /// ´´½¨ÍÅ¶Ó¼¼ÄÜÍ³¼Æ¹¤×÷±E
        /// </summary>
        private static void CreateTeamSkillStatsSheet(XLWorkbook workbook, List<PlayerData> players)
        {
            var worksheet = workbook.Worksheets.Add("ÍÅ¶Ó¼¼ÄÜÍ³¼Æ");

            // »ñÈ¡ÍÅ¶Ó¼¼ÄÜÊı¾İ
            var teamSkills = StatisticData._manager.GetTeamTopSkillsByTotal(50);

            // ÉèÖÃ±úé·
            var headers = new[]
            {
                "¼¼ÄÜÃû³Æ", "×ÜÉËº¦", "×ÜÃEĞ´ÎÊı", "ÍÅ¶ÓÕ¼±È"
            };

            // Ğ´ÈEúé·
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightYellow;
            }

            // ¼ÆËã×ÜÉËº¦ÓÃÓÚ°Ù·Ö±È¼ÆËE
            ulong totalTeamDamage = (ulong)teamSkills.Sum(s => (double)s.Total);

            // Ğ´ÈEı¾İ
            int row = 2;
            foreach (var skill in teamSkills)
            {
                worksheet.Cell(row, 1).Value = skill.SkillName;
                worksheet.Cell(row, 2).Value = (double)skill.Total;
                worksheet.Cell(row, 3).Value = skill.HitCount;
                worksheet.Cell(row, 4).Value = totalTeamDamage > 0 ?
                    $"{((double)skill.Total / totalTeamDamage) * 100:F1}%" : "0%";

                row++;
            }

            // ×Ô¶¯µ÷ÕûÁĞ¿E
            worksheet.ColumnsUsed().AdjustToContents();

            // ÌúØÓÉ¸Ñ¡
            if (row > 2)
                worksheet.Range(1, 1, row - 1, headers.Length).SetAutoFilter();
        }

        #endregion

        #region CSVµ¼³E

        /// <summary>
        /// µ¼³öDPSÊı¾İµ½CSVÎÄ¼ş
        /// </summary>
        /// <param name="players">Íæ¼ÒÊı¾İÁĞ±E/param>
        /// <returns>ÊÇ·ñµ¼³ö³É¹¦</returns>
        public static bool ExportToCsv(List<PlayerData> players)
        {
            try
            {
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "CSVÎÄ¼ş (*.csv)|*.csv",
                    DefaultExt = "csv",
                    FileName = $"DPSÍ³¼Æ_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv",
                    Title = "±£´æDPSÍ³¼ÆÊı¾İ"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return false;

                var csv = new StringBuilder();

                // ÌúØÓBOMÒÔÈ·±£ExcelÕıÈ·ÏÔÊ¾ÖĞÎÄ
                csv.Append('\uFEFF');

                // CSV±úé·
                csv.AppendLine("Íæ¼ÒE³Æ,Ö°Òµ,Õ½Á¦,×ÜÉËº¦,×ÜDPS,±©»÷ÉËº¦,ĞÒÔËÉËº¦,±©»÷ÂÊ,ĞÒÔËÂÊ,Ë²Ê±DPS·åÖµ,×ÜÖÎÁÆ,×ÜHPS,³ĞÊÜÉËº¦,ÃEĞ´ÎÊı");

                // Êı¾İĞĞ
                foreach (var player in players.OrderByDescending(p => p.DamageStats.Total))
                {
                    csv.AppendLine($"\"{EscapeCsvField(player.Nickname)}\"," +
                                 $"\"{EscapeCsvField(player.Profession)}\"," +
                                 $"{player.CombatPower}," +
                                 $"{player.DamageStats.Total}," +
                                 $"{player.GetTotalDps():F1}," +
                                 $"{player.DamageStats.Critical}," +
                                 $"{player.DamageStats.Lucky}," +
                                 $"{player.DamageStats.GetCritRate()}%," +
                                 $"{player.DamageStats.GetLuckyRate()}%," +
                                 $"{player.DamageStats.RealtimeMax}," +
                                 $"{player.HealingStats.Total}," +
                                 $"{player.GetTotalHps():F1}," +
                                 $"{player.TakenDamage}," +
                                 $"{player.DamageStats.CountTotal}");
                }

                File.WriteAllText(saveDialog.FileName, csv.ToString(), Encoding.UTF8);

                MessageBox.Show($"Êı¾İÒÑ³É¹¦µ¼³öµ½:\n{saveDialog.FileName}", "µ¼³ö³É¹¦",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"µ¼³öCSVÎÄ¼şÊ±·¢Éú´úêE\n{ex.Message}", "µ¼³öÊ§°Ü",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// ×ªÒåCSV×Ö¶ÎÖĞµÄÌØÊâ×Ö·E
        /// </summary>
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // Èç¹û°E¬¶ººÅ¡¢ÒıºÅ»ò»»ĞĞ·û£¬ĞèÒªÓÃÒıºÅ°E§²¢×ªÒåÄÚ²¿ÒıºÅ
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return field.Replace("\"", "\"\"");
            }

            return field;
        }

        #endregion

        #region ½ØÍ¼¹¦ÄÜ

        /// <summary>
        /// ±£´æ´°¿Ú½ØÍ¼
        /// </summary>
        /// <param name="form">Òª½ØÍ¼µÄ´°¿Ú</param>
        /// <returns>ÊÇ·ñ±£´æ³É¹¦</returns>
        public static bool SaveScreenshot(Form form)
        {
            try
            {
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "PNGÍ¼Æ¬ (*.png)|*.png|JPEGÍ¼Æ¬ (*.jpg)|*.jpg",
                    DefaultExt = "png",
                    FileName = $"DPS½ØÍ¼_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png",
                    Title = "±£´æDPS½çÃæ½ØÍ¼"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return false;

                // ´´½¨ÓE°¿Ú´óĞ¡ÏàÍ¬µÄÎ»Í¼
                var bounds = form.Bounds;
                using var bitmap = new System.Drawing.Bitmap(bounds.Width, bounds.Height);
                using var graphics = System.Drawing.Graphics.FromImage(bitmap);

                // ½ØÈ¡´°¿ÚÄÚÈİ
                graphics.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);

                // ¸ù¾İÎÄ¼şÀ©Õ¹Ãû±£´E
                var extension = Path.GetExtension(saveDialog.FileName).ToLower();
                var format = extension switch
                {
                    ".jpg" or ".jpeg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                    _ => System.Drawing.Imaging.ImageFormat.Png
                };

                bitmap.Save(saveDialog.FileName, format);

                MessageBox.Show($"½ØÍ¼ÒÑ³É¹¦±£´æµ½:\n{saveDialog.FileName}", "½ØÍ¼³É¹¦",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"±£´æ½ØÍ¼Ê±·¢Éú´úêE\n{ex.Message}", "½ØÍ¼Ê§°Ü",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region ¸¨Öú·½·¨

        /// <summary>
        /// »ñÈ¡µ±Ç°ÓĞÕ½¶·Êı¾İµÄÍæ¼ÒÁĞ±E
        /// </summary>
        /// <returns>Íæ¼ÒÊı¾İÁĞ±E/returns>
        public static List<PlayerData> GetCurrentPlayerData()
        {
            return StatisticData._manager
                .GetPlayersWithCombatData()
                .ToList();
        }

        /// <summary>
        /// ¼EéÊÇ·ñÓĞÊı¾İ¿Éµ¼³E
        /// </summary>
        /// <returns>ÊÇ·ñÓĞÊı¾İ</returns>
        public static bool HasDataToExport()
        {
            return GetCurrentPlayerData().Count > 0;
        }

        #endregion
    }
}