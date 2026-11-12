
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace dizain_hiiv
{
    public partial class Form1 : Form
    {
        class Word
        {
            public int Id;
            public string English, Mongolian, Russian;
            public bool Saved;

            public override string ToString()
            {
                return English + " - " + Mongolian + " - " + Russian;
            }
        }

        List<Word> words = new List<Word>();
        int nextId = 1;

        TextBox search, english, mongolian, russian;
        ComboBox language;
        DataGridView table;
        ListBox savedList;
        Label status;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
            AddSamples();
            RefreshTable();
        }

        void BuildUI()
        {
            Controls.Clear();

            Text = "Гурван хэлний толь бичиг";
            Size = new Size(1050, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);
            BackColor = Color.White;

            // Үндсэн хэсэг
            TableLayoutPanel main = new TableLayoutPanel();
            main.Dock = DockStyle.Fill;
            main.Padding = new Padding(12);
            main.ColumnCount = 1;
            main.RowCount = 4;

            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 125));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            Controls.Add(main);

            // ХАЙЛТ
            FlowLayoutPanel top = new FlowLayoutPanel();
            top.Dock = DockStyle.Fill;
            top.Padding = new Padding(5, 10, 0, 0);

            top.Controls.Add(new Label()
            {
                Text = "Хайх:",
                Width = 50,
                Height = 30
            });

            search = new TextBox();
            search.Width = 480;
            search.Height = 30;
            search.Font = new Font("Segoe UI", 11);
            search.TextChanged += (s, e) => RefreshTable();
            top.Controls.Add(search);

            top.Controls.Add(new Label()
            {
                Text = "  Хэл:",
                Width = 50
            });

            language = new ComboBox();
            language.Width = 130;
            language.DropDownStyle = ComboBoxStyle.DropDownList;
            language.Items.AddRange(new object[]
            {
                "Бүгд", "Англи", "Монгол", "Орос"
            });
            language.SelectedIndex = 0;
            language.SelectedIndexChanged += (s, e) => RefreshTable();
            top.Controls.Add(language);

            main.Controls.Add(top, 0, 0);

            // ХҮСНЭГТ БА САНАСАН ҮГ
            TableLayoutPanel center = new TableLayoutPanel();
            center.Dock = DockStyle.Fill;
            center.ColumnCount = 2;
            center.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 72));
            center.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 28));

            table = new DataGridView();
            table.Dock = DockStyle.Fill;
            table.ReadOnly = true;
            table.AllowUserToAddRows = false;
            table.AllowUserToDeleteRows = false;
            table.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            table.MultiSelect = false;
            table.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            table.RowHeadersVisible = false;
            table.BackgroundColor = Color.White;

            table.Columns.Add("Id", "№");
            table.Columns.Add("English", "Англи");
            table.Columns.Add("Mongolian", "Монгол");
            table.Columns.Add("Russian", "Орос");

            table.Columns[0].FillWeight = 35;

            table.SelectionChanged += (s, e) =>
            {
                Word w = SelectedWord();
                if (w != null)
                {
                    english.Text = w.English;
                    mongolian.Text = w.Mongolian;
                    russian.Text = w.Russian;
                }
            };

            center.Controls.Add(table, 0, 0);

            // САНАСАН ҮГ
            TableLayoutPanel right = new TableLayoutPanel();
            right.Dock = DockStyle.Fill;
            right.Padding = new Padding(12, 0, 0, 0);
            right.RowCount = 2;
            right.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35));
            right.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            right.Controls.Add(new Label()
            {
                Text = "▤ Санасан үг",
                Dock = DockStyle.Fill
            }, 0, 0);

            savedList = new ListBox();
            savedList.Dock = DockStyle.Fill;
            right.Controls.Add(savedList, 0, 1);

            center.Controls.Add(right, 1, 0);
            main.Controls.Add(center, 0, 1);

            // ОРУУЛАХ ХЭСЭГ
            TableLayoutPanel bottom = new TableLayoutPanel();
            bottom.Dock = DockStyle.Fill;
            bottom.ColumnCount = 6;
            bottom.RowCount = 2;
            bottom.Padding = new Padding(0, 10, 0, 0);

            bottom.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 55));
            bottom.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33));
            bottom.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 60));
            bottom.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33));
            bottom.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 50));
            bottom.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 34));

            bottom.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45));
            bottom.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            AddInput(bottom, "Англи", out english, 0);
            AddInput(bottom, "Монгол", out mongolian, 2);
            AddInput(bottom, "Орос", out russian, 4);

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Fill;
            buttons.Padding = new Padding(0, 8, 0, 0);

            buttons.Controls.Add(MakeButton("＋ Нэмэх",
                AddWord));

            buttons.Controls.Add(MakeButton("✎ Засах",
                EditWord));

            buttons.Controls.Add(MakeButton("▣ Устгах",
                DeleteWord));

            buttons.Controls.Add(MakeButton("⟳ Цэвэрлэх",
                ClearInputs));

            Button save = MakeButton("▣ Санах", ToggleSaved);
            save.BackColor = Color.LightGreen;
            buttons.Controls.Add(save);

            bottom.Controls.Add(buttons, 0, 1);
            bottom.SetColumnSpan(buttons, 6);

            main.Controls.Add(bottom, 0, 2);

            status = new Label();
            status.Dock = DockStyle.Fill;
            status.TextAlign = ContentAlignment.MiddleLeft;

            main.Controls.Add(status, 0, 3);
        }

        void AddInput(TableLayoutPanel panel,
            string title, out TextBox box, int column)
        {
            panel.Controls.Add(new Label()
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            }, column, 0);

            box = new TextBox();
            box.Dock = DockStyle.Fill;

            panel.Controls.Add(box, column + 1, 0);
        }

        Button MakeButton(string title, Action action)
        {
            Button b = new Button();
            b.Text = title;
            b.AutoSize = true;
            b.Height = 38;
            b.Margin = new Padding(3);
            b.Click += (s, e) => action();
            return b;
        }

        void AddSamples()
        {
            AddSample("one", "нэг", "один");
            AddSample("two", "хоёр", "два");
            AddSample("three", "гурав", "три");
            AddSample("four", "дөрөв", "четыре");
            AddSample("five", "тав", "пять");
            AddSample("six", "зургаа", "шесть");
            AddSample("seven", "долоо", "семь");
            AddSample("eight", "найм", "восемь");
            AddSample("nine", "ес", "девять");
            AddSample("ten", "арав", "десять");
        }

        void AddSample(string en, string mn, string ru)
        {
            words.Add(new Word()
            {
                Id = nextId++,
                English = en,
                Mongolian = mn,
                Russian = ru
            });
        }

        Word SelectedWord()
        {
            if (table.SelectedRows.Count == 0)
                return null;

            int id = Convert.ToInt32(
                table.SelectedRows[0].Cells[0].Value);

            return words.FirstOrDefault(x => x.Id == id);
        }

        void RefreshTable()
        {
            if (table == null) return;

            string key = search.Text.Trim();
            string lang = language.Text;

            IEnumerable<Word> result = words;

            if (key.Length > 0)
            {
                if (lang == "Англи")
                    result = result.Where(x =>
                        x.English.IndexOf(key,
                        StringComparison.OrdinalIgnoreCase) >= 0);

                else if (lang == "Монгол")
                    result = result.Where(x =>
                        x.Mongolian.IndexOf(key,
                        StringComparison.OrdinalIgnoreCase) >= 0);

                else if (lang == "Орос")
                    result = result.Where(x =>
                        x.Russian.IndexOf(key,
                        StringComparison.OrdinalIgnoreCase) >= 0);

                else
                    result = result.Where(x =>
                        x.English.IndexOf(key,
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                        x.Mongolian.IndexOf(key,
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                        x.Russian.IndexOf(key,
                        StringComparison.OrdinalIgnoreCase) >= 0);
            }

            table.Rows.Clear();

            foreach (Word w in result)
            {
                table.Rows.Add(
                    w.Id, w.English,
                    w.Mongolian, w.Russian);
            }

            savedList.Items.Clear();

            foreach (Word w in words.Where(x => x.Saved))
                savedList.Items.Add(w);

            status.Text = "Нийт: " + words.Count +
                " үг - Санасан: " +
                words.Count(x => x.Saved);
        }

        bool ValidInput()
        {
            if (String.IsNullOrWhiteSpace(english.Text) ||
                String.IsNullOrWhiteSpace(mongolian.Text) ||
                String.IsNullOrWhiteSpace(russian.Text))
            {
                MessageBox.Show(
                    "Гурван хэлний талбарыг бөглөнө үү!");
                return false;
            }

            return true;
        }

        void AddWord()
        {
            if (!ValidInput()) return;

            words.Add(new Word()
            {
                Id = nextId++,
                English = english.Text.Trim(),
                Mongolian = mongolian.Text.Trim(),
                Russian = russian.Text.Trim()
            });

            RefreshTable();
            ClearInputs();
        }

        void EditWord()
        {
            Word w = SelectedWord();

            if (w == null)
            {
                MessageBox.Show("Засах үгээ сонгоно уу!");
                return;
            }

            if (!ValidInput()) return;

            w.English = english.Text.Trim();
            w.Mongolian = mongolian.Text.Trim();
            w.Russian = russian.Text.Trim();

            RefreshTable();
        }

        void DeleteWord()
        {
            Word w = SelectedWord();

            if (w == null)
            {
                MessageBox.Show("Устгах үгээ сонгоно уу!");
                return;
            }

            if (MessageBox.Show("Үгийг устгах уу?",
                "Баталгаажуулах",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                words.Remove(w);
                RefreshTable();
                ClearInputs();
            }
        }

        void ToggleSaved()
        {
            Word w = SelectedWord();

            if (w == null)
            {
                MessageBox.Show("Санах үгээ сонгоно уу!");
                return;
            }

            w.Saved = !w.Saved;
            RefreshTable();
        }

        void ClearInputs()
        {
            english.Clear();
            mongolian.Clear();
            russian.Clear();
            table.ClearSelection();
        }
    }
}
