using EndpointChecker.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EndpointChecker
{
    public partial class CloudflareBypassSettingsDialog : Form
    {
        private bool _playwrightAvailable;

        public CloudflareBypassSettingsDialog()
        {
            InitializeComponent();
            _playwrightAvailable = CloudflareBypassChecker.IsPlaywrightAvailable();
            BuildComboItems();
            LoadSettings();
        }

        private void BuildComboItems()
        {
            comboBox_BypassMethod.Items.Clear();
            comboBox_BypassMethod.Items.Add("Disabled  (show CF status only, no bypass)");
            comboBox_BypassMethod.Items.Add("FlareSolverr  (external proxy — github.com/FlareSolverr)");

            if (_playwrightAvailable)
            {
                comboBox_BypassMethod.Items.Add("Playwright  (detected — uses local Chromium browser)");
                lbl_PlaywrightStatus.Text = "Playwright: Chromium detected in %LOCALAPPDATA%\\ms-playwright";
                lbl_PlaywrightStatus.ForeColor = Color.DarkGreen;
            }
            else
            {
                comboBox_BypassMethod.Items.Add("Playwright  (not installed)");
                lbl_PlaywrightStatus.Text = "Playwright: not detected — run 'playwright install chromium' to enable";
                lbl_PlaywrightStatus.ForeColor = Color.Gray;
            }
        }

        private void LoadSettings()
        {
            int saved = Settings.Default.Config_CloudflareBypass_Method;
            int maxIndex = comboBox_BypassMethod.Items.Count - 1;
            comboBox_BypassMethod.SelectedIndex = (saved >= 0 && saved <= maxIndex) ? saved : 0;

            string savedUrl = Settings.Default.Config_FlareSolverr_URL;
            textBox_FlareSolverrUrl.Text = string.IsNullOrWhiteSpace(savedUrl)
                ? "http://localhost:8191"
                : savedUrl;

            UpdatePanelVisibility();
        }

        private void UpdatePanelVisibility()
        {
            int sel = comboBox_BypassMethod.SelectedIndex;
            panel_FlareSolverr.Visible = sel == (int)CloudflareBypassMethod.FlareSolverr;
            lbl_PlaywrightStatus.Visible = sel == (int)CloudflareBypassMethod.Playwright;
        }

        private void comboBox_BypassMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Block selection of Playwright when it isn't installed
            if (comboBox_BypassMethod.SelectedIndex == (int)CloudflareBypassMethod.Playwright
                && !_playwrightAvailable)
            {
                MessageBox.Show(
                    "Playwright Chromium is not installed on this machine.\r\n\r\n" +
                    "To enable it, install Playwright browsers via one of:\r\n\r\n" +
                    "  • .NET:   dotnet tool install --global Microsoft.Playwright.CLI\r\n" +
                    "            playwright install chromium\r\n\r\n" +
                    "  • npm:    npm install -g playwright\r\n" +
                    "            npx playwright install chromium\r\n\r\n" +
                    "After installation, reopen this dialog — the option will be enabled.",
                    "Playwright Not Installed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                comboBox_BypassMethod.SelectedIndex = 0;
                return;
            }

            UpdatePanelVisibility();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            Settings.Default.Config_CloudflareBypass_Method = comboBox_BypassMethod.SelectedIndex;
            Settings.Default.Config_FlareSolverr_URL = textBox_FlareSolverrUrl.Text.Trim();
            Settings.Default.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
