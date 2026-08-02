using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EndpointChecker
{
    public class EndpointManagementDialog : Form
    {
        private readonly CheckerMainForm _owner;
        private readonly string _filePath;

        public bool FileWasModified { get; private set; }

        // File is modelled as a mixed list of comment/blank lines and endpoint lines.
        // Only endpoint lines are shown in the grid; comments are preserved on save.
        private sealed class FileLine
        {
            public bool IsEndpoint;
            public string Name;
            public string Url;
            public string Raw;   // original text for comment / blank lines
        }

        private List<FileLine> _lines;
        private DataGridView _grid;

        public EndpointManagementDialog(CheckerMainForm owner)
        {
            _owner = owner;
            _filePath = Program.endpointDefinitionsFile;
            BuildUI();
            LoadFile();
            CheckerMainForm.ApplyDarkTheme(this);
        }

        // ── UI ─────────────────────────────────────────────────────────────────

        private void BuildUI()
        {
            Text = "Endpoint Management";
            Size = new Size(720, 540);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9f);

            _grid = new DataGridView
            {
                Location = new Point(8, 8),
                Size = new Size(688, 420),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                EditMode = DataGridViewEditMode.EditOnEnter,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText,
            };

            var colName = new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Name = "colName",
                Width = 180,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };
            var colUrl = new DataGridViewTextBoxColumn
            {
                HeaderText = "URL",
                Name = "colUrl",
                Width = 492,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };
            _grid.Columns.AddRange(colName, colUrl);

            var btnAdd = new Button { Text = "Add", Size = new Size(80, 26), Location = new Point(8, 442) };
            var btnDelete = new Button { Text = "Delete", Size = new Size(80, 26), Location = new Point(96, 442) };
            var btnUp = new Button { Text = "▲ Move Up", Size = new Size(88, 26), Location = new Point(184, 442) };
            var btnDown = new Button { Text = "▼ Move Down", Size = new Size(96, 26), Location = new Point(280, 442) };
            var btnSave = new Button { Text = "Save", Size = new Size(80, 26), Location = new Point(520, 442), DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Size = new Size(80, 26), Location = new Point(608, 442), DialogResult = DialogResult.Cancel };

            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnUp.Click += BtnUp_Click;
            btnDown.Click += BtnDown_Click;
            btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[] { _grid, btnAdd, btnDelete, btnUp, btnDown, btnSave, btnCancel });
            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        // ── File I/O ───────────────────────────────────────────────────────────

        private void LoadFile()
        {
            _lines = new List<FileLine>();
            _grid.Rows.Clear();

            if (!File.Exists(_filePath))
                return;

            foreach (string raw in File.ReadAllLines(_filePath, Encoding.Default))
            {
                string trimmed = raw.Trim();
                bool isEndpoint = !string.IsNullOrEmpty(trimmed)
                                  && trimmed != "|"
                                  && !trimmed.StartsWith("#");

                if (isEndpoint)
                {
                    string[] parts = trimmed.Split(new char[] { '|' }, 2);
                    string name = parts[0].Trim();
                    string url = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                    _lines.Add(new FileLine { IsEndpoint = true, Name = name, Url = url });
                    _grid.Rows.Add(name, url);
                }
                else
                {
                    _lines.Add(new FileLine { IsEndpoint = false, Raw = raw });
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Commit any pending cell edit
            _grid.EndEdit();

            // Sync grid values back to the endpoint FileLine objects
            int gridIdx = 0;
            foreach (FileLine fl in _lines)
            {
                if (!fl.IsEndpoint)
                    continue;
                if (gridIdx < _grid.Rows.Count)
                {
                    fl.Name = (_grid.Rows[gridIdx].Cells[0].Value ?? string.Empty).ToString().Trim();
                    fl.Url = (_grid.Rows[gridIdx].Cells[1].Value ?? string.Empty).ToString().Trim();
                    gridIdx++;
                }
            }

            // Reconstruct the file
            var sb = new StringBuilder();
            foreach (FileLine fl in _lines)
            {
                if (fl.IsEndpoint)
                {
                    if (string.IsNullOrEmpty(fl.Name) && string.IsNullOrEmpty(fl.Url))
                        continue;   // skip blank rows added then left empty
                    sb.AppendLine(string.IsNullOrEmpty(fl.Name)
                        ? fl.Url
                        : fl.Name + "|" + fl.Url);
                }
                else
                {
                    sb.AppendLine(fl.Raw);
                }
            }

            try
            {
                File.WriteAllText(_filePath, sb.ToString(), Encoding.Default);
                FileWasModified = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save endpoint list:\n" + ex.Message,
                                "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        // ── Grid toolbar ───────────────────────────────────────────────────────

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            int newRow = _grid.Rows.Add("New Endpoint", "http://");
            _lines.Add(new FileLine { IsEndpoint = true, Name = "New Endpoint", Url = "http://" });
            _grid.ClearSelection();
            _grid.Rows[newRow].Selected = true;
            _grid.CurrentCell = _grid.Rows[newRow].Cells[0];
            _grid.BeginEdit(true);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_grid.CurrentRow == null)
                return;
            int gridRow = _grid.CurrentRow.Index;
            _grid.Rows.RemoveAt(gridRow);
            RemoveEndpointLine(gridRow);
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            if (_grid.CurrentRow == null || _grid.CurrentRow.Index == 0)
                return;
            int idx = _grid.CurrentRow.Index;
            SwapGridRows(idx, idx - 1);
            SwapEndpointLines(idx, idx - 1);
            _grid.ClearSelection();
            _grid.Rows[idx - 1].Selected = true;
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            if (_grid.CurrentRow == null || _grid.CurrentRow.Index >= _grid.Rows.Count - 1)
                return;
            int idx = _grid.CurrentRow.Index;
            SwapGridRows(idx, idx + 1);
            SwapEndpointLines(idx, idx + 1);
            _grid.ClearSelection();
            _grid.Rows[idx + 1].Selected = true;
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private void SwapGridRows(int a, int b)
        {
            var row = _grid.Rows[a];
            object nameA = row.Cells[0].Value;
            object urlA = row.Cells[1].Value;
            var rowB = _grid.Rows[b];
            row.Cells[0].Value = rowB.Cells[0].Value;
            row.Cells[1].Value = rowB.Cells[1].Value;
            rowB.Cells[0].Value = nameA;
            rowB.Cells[1].Value = urlA;
        }

        // Swap endpoint data at grid indices a and b within _lines (skipping comment lines).
        private void SwapEndpointLines(int a, int b)
        {
            FileLine lineA = GetEndpointLine(a);
            FileLine lineB = GetEndpointLine(b);
            if (lineA == null || lineB == null)
                return;
            string tmpName = lineA.Name;
            string tmpUrl = lineA.Url;
            lineA.Name = lineB.Name;
            lineA.Url = lineB.Url;
            lineB.Name = tmpName;
            lineB.Url = tmpUrl;
        }

        private FileLine GetEndpointLine(int gridIndex)
        {
            int count = 0;
            foreach (FileLine fl in _lines)
            {
                if (!fl.IsEndpoint)
                    continue;
                if (count == gridIndex)
                    return fl;
                count++;
            }
            return null;
        }

        private void RemoveEndpointLine(int gridIndex)
        {
            int count = 0;
            for (int i = 0; i < _lines.Count; i++)
            {
                if (!_lines[i].IsEndpoint)
                    continue;
                if (count == gridIndex)
                {
                    _lines.RemoveAt(i);
                    return;
                }
                count++;
            }
        }
    }
}
